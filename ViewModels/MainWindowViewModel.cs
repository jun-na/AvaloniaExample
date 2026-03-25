using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace AvaloniaExample.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly ObservableCollection<TodoItemViewModel> _allTasks = new();
    private readonly IReadOnlyList<FilterOptionViewModel> _filterLookup;
    private string _newTaskTitle = string.Empty;
    private string _newTaskNote = string.Empty;
    private TodoFilter _selectedFilter = TodoFilter.All;

    public MainWindowViewModel()
    {
        Filters =
        [
            new FilterOptionViewModel(TodoFilter.All, "受信トレイ", "いま見えている全タスク"),
            new FilterOptionViewModel(TodoFilter.Today, "今日", "今日の流れに乗せたい作業"),
            new FilterOptionViewModel(TodoFilter.Important, "固定", "優先して進めるタスク"),
            new FilterOptionViewModel(TodoFilter.Completed, "完了", "終わった作業を確認")
        ];

        _filterLookup = Filters.ToList();
        VisibleTasks = new ObservableCollection<TodoItemViewModel>();

        SeedTasks();
        SetFilter(TodoFilter.All);
    }

    public ObservableCollection<FilterOptionViewModel> Filters { get; }

    public ObservableCollection<TodoItemViewModel> VisibleTasks { get; }

    public string TodayLabel => DateTime.Now.ToString("M月d日 dddd", CultureInfo.GetCultureInfo("ja-JP"));

    public string NewTaskTitle
    {
        get => _newTaskTitle;
        set => SetProperty(ref _newTaskTitle, value);
    }

    public string NewTaskNote
    {
        get => _newTaskNote;
        set => SetProperty(ref _newTaskNote, value);
    }

    public int TotalTasks => _allTasks.Count;

    public int OpenTasks => _allTasks.Count(task => !task.IsCompleted);

    public int CompletedTasks => _allTasks.Count(task => task.IsCompleted);

    public int ImportantTasks => _allTasks.Count(task => task.IsImportant && !task.IsCompleted);

    public int TodayTasks => _allTasks.Count(task => task.IsToday && !task.IsCompleted);

    public double CompletionRate => TotalTasks == 0 ? 0 : CompletedTasks * 100.0 / TotalTasks;

    public string CompletionRateLabel => $"{CompletionRate:0}%";

    public string HeaderTitle => _selectedFilter switch
    {
        TodoFilter.Today => "今日の流れ",
        TodoFilter.Important => "優先レーン",
        TodoFilter.Completed => "完了したタスク",
        _ => "デスク全体"
    };

    public string HeaderSubtitle => _selectedFilter switch
    {
        TodoFilter.Today => $"今日のタスクはあと {TodayTasks} 件です。",
        TodoFilter.Important => $"優先して動かしたいタスクが {ImportantTasks} 件あります。",
        TodoFilter.Completed => $"完了済みのタスクは {CompletedTasks} 件です。",
        _ => $"進行中のタスクは {OpenTasks} 件あります。"
    };

    public int VisibleTaskCount => VisibleTasks.Count;

    public string VisibleTaskCountLabel => $"{VisibleTaskCount} 件";

    public bool HasVisibleTasks => VisibleTasks.Count > 0;

    public bool ShowEmptyState => !HasVisibleTasks;

    public TodoItemViewModel? FocusTask => _allTasks
        .Where(task => !task.IsCompleted)
        .OrderByDescending(task => task.IsImportant)
        .ThenBy(task => task.IsToday ? 0 : 1)
        .ThenByDescending(task => task.CreatedAt)
        .FirstOrDefault();

    public bool HasFocusTask => FocusTask is not null;

    public bool ShowFocusEmptyState => !HasFocusTask;

    public string FocusTaskTitle => FocusTask?.Title ?? "いまは空っぽです";

    public string FocusTaskNote => FocusTask?.Note ?? "新しいタスクを追加すると、ここに次のアクションが表示されます。";

    public string FocusTaskDueLabel => FocusTask?.DueLabel ?? "余白あり";

    public string FocusTaskSection => FocusTask?.Section ?? "概要";

    public string FocusHeading => FocusTask?.IsImportant == true ? "いちばん大事" : "次にやること";

    public bool AddTask()
    {
        var title = NewTaskTitle.Trim();
        if (title.Length == 0)
        {
            return false;
        }

        var note = NewTaskNote.Trim();
        var task = new TodoItemViewModel
        {
            Title = title,
            Note = note.Length == 0 ? "クイック追加から取り込んだ新しいタスクです。" : note,
            Section = _selectedFilter switch
            {
                TodoFilter.Important => "優先",
                TodoFilter.Today => "今日",
                TodoFilter.Completed => "受信",
                _ => "受信"
            },
            DueLabel = _selectedFilter switch
            {
                TodoFilter.Today => "今日中",
                TodoFilter.Important => "今日中",
                TodoFilter.Completed => "明日",
                _ => "今週"
            },
            IsToday = _selectedFilter is TodoFilter.Today or TodoFilter.Important,
            IsImportant = _selectedFilter == TodoFilter.Important
        };

        AddTask(task);

        NewTaskTitle = string.Empty;
        NewTaskNote = string.Empty;

        if (_selectedFilter == TodoFilter.Completed)
        {
            SetFilter(TodoFilter.All);
        }
        else
        {
            RefreshState();
        }

        return true;
    }

    public void SetFilter(TodoFilter filter)
    {
        _selectedFilter = filter;

        foreach (var option in _filterLookup)
        {
            option.IsSelected = option.Filter == filter;
        }

        RefreshState();
    }

    public void ToggleImportant(TodoItemViewModel task)
    {
        task.IsImportant = !task.IsImportant;
        RefreshState();
    }

    private void SeedTasks()
    {
        AddTask(new TodoItemViewModel
        {
            Title = "オンボーディング文言を整える",
            Note = "初回表示の空状態を、落ち着いたトーンで見直す。",
            Section = "デザイン",
            DueLabel = "今日中",
            IsToday = true,
            IsImportant = true,
            CreatedAt = DateTimeOffset.Now.AddMinutes(-35)
        });

        AddTask(new TodoItemViewModel
        {
            Title = "スプリントレビューのメモを用意する",
            Note = "午後の共有前に、プロダクト・QA・基盤の要点をまとめる。",
            Section = "計画",
            DueLabel = "14:00",
            IsToday = true,
            CreatedAt = DateTimeOffset.Now.AddMinutes(-70)
        });

        AddTask(new TodoItemViewModel
        {
            Title = "ユーザーインタビューの日程を確定する",
            Note = "最近登録した 3 人に連絡して、金曜の候補時間を押さえる。",
            Section = "リサーチ",
            DueLabel = "明日",
            IsImportant = true,
            CreatedAt = DateTimeOffset.Now.AddHours(-3)
        });

        AddTask(new TodoItemViewModel
        {
            Title = "クローズ済みの feature ブランチを整理する",
            Note = "リリース対応が終わったので、不要なブランチを片付ける。",
            Section = "開発",
            DueLabel = "完了",
            IsCompleted = true,
            CreatedAt = DateTimeOffset.Now.AddDays(-1)
        });
    }

    private void AddTask(TodoItemViewModel task)
    {
        _allTasks.Add(task);
        task.PropertyChanged += TaskOnPropertyChanged;
    }

    private void TaskOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(TodoItemViewModel.IsCompleted) or nameof(TodoItemViewModel.IsImportant))
        {
            RefreshState();
        }
    }

    private void RefreshState()
    {
        RefreshVisibleTasks();
        RefreshFilterCounts();
        RaiseDashboardProperties();
    }

    private void RefreshVisibleTasks()
    {
        VisibleTasks.Clear();

        foreach (var task in GetFilteredTasks())
        {
            VisibleTasks.Add(task);
        }

        OnPropertyChanged(nameof(VisibleTaskCount));
        OnPropertyChanged(nameof(VisibleTaskCountLabel));
        OnPropertyChanged(nameof(HasVisibleTasks));
        OnPropertyChanged(nameof(ShowEmptyState));
    }

    private IEnumerable<TodoItemViewModel> GetFilteredTasks()
    {
        IEnumerable<TodoItemViewModel> filtered = _selectedFilter switch
        {
            TodoFilter.Today => _allTasks.Where(task => task.IsToday),
            TodoFilter.Important => _allTasks.Where(task => task.IsImportant),
            TodoFilter.Completed => _allTasks.Where(task => task.IsCompleted),
            _ => _allTasks
        };

        return filtered
            .OrderBy(task => task.IsCompleted)
            .ThenByDescending(task => task.IsImportant)
            .ThenBy(task => task.IsToday ? 0 : 1)
            .ThenByDescending(task => task.CreatedAt);
    }

    private void RefreshFilterCounts()
    {
        foreach (var option in _filterLookup)
        {
            option.Count = option.Filter switch
            {
                TodoFilter.Today => _allTasks.Count(task => task.IsToday && !task.IsCompleted),
                TodoFilter.Important => _allTasks.Count(task => task.IsImportant && !task.IsCompleted),
                TodoFilter.Completed => _allTasks.Count(task => task.IsCompleted),
                _ => _allTasks.Count(task => !task.IsCompleted)
            };
        }
    }

    private void RaiseDashboardProperties()
    {
        OnPropertyChanged(nameof(TotalTasks));
        OnPropertyChanged(nameof(OpenTasks));
        OnPropertyChanged(nameof(CompletedTasks));
        OnPropertyChanged(nameof(ImportantTasks));
        OnPropertyChanged(nameof(TodayTasks));
        OnPropertyChanged(nameof(CompletionRate));
        OnPropertyChanged(nameof(CompletionRateLabel));
        OnPropertyChanged(nameof(HeaderTitle));
        OnPropertyChanged(nameof(HeaderSubtitle));
        OnPropertyChanged(nameof(FocusTask));
        OnPropertyChanged(nameof(HasFocusTask));
        OnPropertyChanged(nameof(ShowFocusEmptyState));
        OnPropertyChanged(nameof(FocusTaskTitle));
        OnPropertyChanged(nameof(FocusTaskNote));
        OnPropertyChanged(nameof(FocusTaskDueLabel));
        OnPropertyChanged(nameof(FocusTaskSection));
        OnPropertyChanged(nameof(FocusHeading));
    }
}
