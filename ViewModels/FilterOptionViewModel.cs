namespace AvaloniaExample.ViewModels;

public sealed class FilterOptionViewModel : ViewModelBase
{
    private int _count;
    private bool _isSelected;

    public FilterOptionViewModel(TodoFilter filter, string title, string subtitle)
    {
        Filter = filter;
        Title = title;
        Subtitle = subtitle;
    }

    public TodoFilter Filter { get; }

    public string Title { get; }

    public string Subtitle { get; }

    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
