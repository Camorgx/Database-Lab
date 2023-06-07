using MaterialDesignThemes.Wpf;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void MoveWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private async Task<string> GetYiYanWords() {
            string res;
            try {
                var client = new HttpClient();
                res = await client.GetStringAsync("https://v1.hitokoto.cn/?c=k&encode=text");
            }
            catch (Exception) {
                res = "我们所爱之物昭示着我们究竟是谁。";
            }
            return res;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            var text = GetYiYanWords();
            var openDatabase = Database.Activate();
            string isPasswordSaved = ConfigurationManager.AppSettings["IsPasswordSaved"] ?? "False";
            if (isPasswordSaved == "True") {
                rememberMe.IsChecked = true;
                teacherID.Text = ConfigurationManager.AppSettings["ID"];
                password.Password = ConfigurationManager.AppSettings["Password"];
            }
            else rememberMe.IsChecked = false;
            apothegm.Text = await text;
            await openDatabase;
        }

        private void ShowRegisterWindow(object sender, MouseButtonEventArgs e) {
            var registerWindow = new Register() {
                Owner = this, 
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left,
                Top = Top,
            };
            registerWindow.Activate();
            registerWindow.Show();
            Hide();
        }

        private void ShowResetPasswordWindow(object sender, MouseButtonEventArgs e) {
            var resetPasswordWindow = new ResetPassword() {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left,
                Top = Top,
            };
            resetPasswordWindow.Activate();
            resetPasswordWindow.Show();
            Hide();
        }

        private async void LoginButtonClick(object sender, RoutedEventArgs e) {
            var id = teacherID.Text;
            var pwd = password.Password;
            var res = Database.TryLogin(id, pwd);
            var window = new PleaseWaiting() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            bool status = await res;
            window.Close();
            if (status) {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                bool save = rememberMe.IsChecked.GetValueOrDefault();
                configuration.AppSettings.Settings["IsPasswordSaved"].Value = save ? "True" : "False";
                if (save) {
                    configuration.AppSettings.Settings["ID"].Value = teacherID.Text;
                    configuration.AppSettings.Settings["Password"].Value = password.Password;
                }
                configuration.Save();
                ConfigurationManager.RefreshSection("appSettings");
                var operation = new Operation() { Owner = this };
                operation.Show();
                Hide();
            }
            else {
                Utils.MessageTips("工号或密码不正确！");
            }
        }
    }
}
