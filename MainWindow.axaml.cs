using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaExample.ViewModels;

namespace AvaloniaExample;

public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    private void FilterButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: FilterOptionViewModel option })
        {
            ViewModel.SetFilter(option.Filter);
        }
    }

    private void AddTaskButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.AddTask())
        {
            TitleTextBox.Focus();
        }
    }

    private void ToggleImportantButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: TodoItemViewModel task })
        {
            ViewModel.ToggleImportant(task);
        }
    }
}
