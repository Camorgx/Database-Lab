using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3 {
    /// <summary>
    /// ResetPassword.xaml 的交互逻辑
    /// </summary>
    public partial class ResetPassword : Window {
        public ResetPassword() {
            InitializeComponent();
        }

        private void CloseWindowClick(object sender, RoutedEventArgs e) {
            Owner.Left = Left;
            Owner.Top = Top;
            Owner.Show();
            Close();
        }

        private void MoveWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private async void ResetPwdButtonClick(object sender, RoutedEventArgs e) {
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            var id = teacherID.Text;
            var pwd = passwordBox.Password;
            var verPwd = verifyPassword.Password;
            var verify = verification.Text;
            if (pwd != verPwd) {
                await Utils.MessageTips("两次输入的密码不一致。", "ResetPasswordDialog");
                return;
            }
            if (!Utils.VerifyPassword(pwd)) {
                await Utils.MessageTips("密码格式不正确。", "ResetPasswordDialog");
                return;
            }
            int res = await Database.TryResetPassword(id, pwd, verify);
            window.Close();
            if (res == 0) {
                await Utils.MessageTips("密码重置成功，请登录系统。", "ResetPasswordDialog");
                if (Owner is MainWindow owner) {
                    owner.rememberMe.IsChecked = false;
                    owner.password.Password = "";
                }
                CloseWindowClick(sender, e);
            }
            else if (res == 1) {
                await Utils.MessageTips("您请求的工号不存在。", "ResetPasswordDialog");
                return;
            }
            else if (res == 2) {
                await Utils.MessageTips("身份校验码不正确。", "ResetPasswordDialog");
                return;
            }
            else if (res == 3) {
                await Utils.MessageTips("数据库错误。", "ResetPasswordDialog");
                return;
            }
            else {
                await Utils.MessageTips("未知错误。", "ResetPasswordDialog");
                return;
            }
        }
    }
}
