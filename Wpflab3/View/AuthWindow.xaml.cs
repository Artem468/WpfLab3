using System.Windows;
using Wpflab3.ViewModels;

namespace Wpflab3.View
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            DataContext = new AuthViewModel();
        }
    }
}