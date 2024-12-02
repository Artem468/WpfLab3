using System.Windows;
using Wpflab3.ViewModels;

namespace Wpflab3;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window 
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}