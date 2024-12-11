using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Controls;
using Wpflab3.Models;

namespace Wpflab3.ViewModels
{
    internal class AuthViewModel : INotifyPropertyChanged
    {
        private string login;

        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged("Login");
            }
        }

        private string password;

        public string LoginPassword
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("LoginPassword");
            }
        }

        private RelayCommand loginCommand;

        public RelayCommand LoginCommand
        {
            get
            {
                return loginCommand = new RelayCommand(async obj =>
                       {
                           
                           PasswordBox? password = obj as PasswordBox;
                           HttpClient client = new HttpClient();
                           User user = new User { Login = Login, Password = password!.Password };
                           JsonContent content = JsonContent.Create(user);
                           using var response = await client.PostAsync("http://localhost:5000/login", content);
                           string responseText = await response.Content.ReadAsStringAsync();
                           UserResponse? resp = JsonSerializer.Deserialize<UserResponse>(responseText);
                           if (resp != null)
                           {
                               if (resp.access_token != null)
                               {
                                   MainWindow window = new MainWindow(resp.access_token);
                                   window.Show();
                               }
                           }
                       });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}