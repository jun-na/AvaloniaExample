using System;

namespace AvaloniaExample.ViewModels;

public sealed class TodoItemViewModel : ViewModelBase
{
    private string _title = string.Empty;
    private string _note = string.Empty;
    private string _section = string.Empty;
    private string _dueLabel = string.Empty;
    private bool _isToday;
    private bool _isCompleted;
    private bool _isImportant;

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.Now;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Note
    {
        get => _note;
        set
        {
            if (SetProperty(ref _note, value))
            {
                OnPropertyChanged(nameof(HasNote));
            }
        }
    }

    public string Section
    {
        get => _section;
        set => SetProperty(ref _section, value);
    }

    public string DueLabel
    {
        get => _dueLabel;
        set => SetProperty(ref _dueLabel, value);
    }

    public bool IsToday
    {
        get => _isToday;
        set => SetProperty(ref _isToday, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (SetProperty(ref _isCompleted, value))
            {
                OnPropertyChanged(nameof(CardOpacity));
                OnPropertyChanged(nameof(StateLabel));
            }
        }
    }

    public bool IsImportant
    {
        get => _isImportant;
        set
        {
            if (SetProperty(ref _isImportant, value))
            {
                OnPropertyChanged(nameof(ImportanceLabel));
            }
        }
    }

    public bool HasNote => !string.IsNullOrWhiteSpace(Note);

    public double CardOpacity => IsCompleted ? 0.58 : 1.0;

    public string ImportanceLabel => IsImportant ? "固定中" : "固定";

    public string StateLabel => IsCompleted ? "完了" : DueLabel;
}
