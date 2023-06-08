using System.Windows;
using System.Windows.Input;

namespace Lab3 {
    /// <summary>
    /// Register.xaml 的交互逻辑
    /// </summary>
    public partial class Register : Window {
        public Register() {
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

        private async void RegisterButtonClick(object sender, RoutedEventArgs e) {
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
                await Utils.MessageTips("两次输入的密码不一致。", "RegisterDialog");
                return;
            }
            if (!Utils.VerifyPassword(pwd)) {
                await Utils.MessageTips("密码格式不正确。", "RegisterDialog");
                return;
            }
            int res = await Database.TryRegister(id, pwd, verify);
            window.Close();
            if (res == 0) {
                await Utils.MessageTips("注册成功，请登陆系统以补充个人信息。", "RegisterDialog");
                CloseWindowClick(sender, e);
            }
            else if (res == 1) {
                await Utils.MessageTips("您注册的工号已存在。", "RegisterDialog");
                return;
            }
            else if (res == 2) {
                await Utils.MessageTips("数据库错误。", "RegisterDialog");
                return;
            }
            else {
                await Utils.MessageTips("未知错误。", "RegisterDialog");
                return;
            }
        }
    }
}
