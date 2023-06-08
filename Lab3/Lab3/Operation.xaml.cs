using System;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Lab3 {
    /// <summary>
    /// Operation.xaml 的交互逻辑
    /// </summary>
    public partial class Operation : Window {
        public Operation() {
            InitializeComponent();
        }

        private bool logOut = false;

        private void WindowClosed(object sender, EventArgs e) {
            if (!logOut) Owner.Close();
        }

        private void CloseWindowClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void MoveWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void MaximizeWindowClick(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
                maxIcon.Kind = PackIconKind.Maximize;
            }
            else {
                WindowState = WindowState.Maximized;
                maxIcon.Kind = PackIconKind.WindowRestore;
            }
        }

        private void MinimizeWindowClick(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void InitMyInfo() {
            teacherID.Text = Global.teacher.ID;
            name.Text = Global.teacher.Name;
            gender.SelectedIndex = Global.teacher.Gender - 1;
            teacherTitle.SelectedIndex = Global.teacher.Title - 1;
        }

        private void InitPaper() {
            ownPaper.ItemsSource = Global.ownPaper;
            partedPaper.ItemsSource = Global.partedPaper;
            Utils.UpdatePaperView();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e) {
            InitMyInfo();
            InitPaper();
        }

        private async void UpdateButtonClick(object sender, RoutedEventArgs e) {
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            var newName = name.Text;
            var newGender = gender.SelectedIndex + 1;
            var newTitle = teacherTitle.SelectedIndex + 1;
            var update = Database.UpdateTeacherData(teacherID.Text, newName, newGender, newTitle);
            var res = await update;
            window.Close();
            if (res) {
                Global.teacher.Name = newName;
                Global.teacher.Gender = newGender;
                Global.teacher.Title = newTitle;
                await Utils.MessageTips("修改成功。", "OperationDialog");
            }
            else await Utils.MessageTips("修改失败。", "OperationDialog");
        }

        private async void ResetPasswordButtonClick(object sender, RoutedEventArgs e) {
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            var id = Global.teacher.ID;
            var oldPwd = oldPassword.Password;
            var newPwd = newPassword.Password;
            var verPwd = verifyPassword.Password;
            if (newPwd != verPwd) {
                await Utils.MessageTips("两次输入的密码不一致。", "OperationDialog");
                return;
            }
            if (oldPwd == newPwd) {
                await Utils.MessageTips("旧密码与新密码相同。", "OperationDialog");
                return;
            }
            if (!Utils.VerifyPassword(newPwd)) {
                await Utils.MessageTips("密码格式不正确。", "OperationDialog");
                return;
            }
            int res = await Database.UpdatePassword(id, oldPwd, newPwd);
            window.Close();
            if (res == 0) {
                await Utils.MessageTips("密码更改成功，请重新登录系统。", "OperationDialog");
                logOut = true;
                ((MainWindow)Owner).rememberMe.IsChecked = false;
                ((MainWindow)Owner).password.Password = "";
                Owner.Show();
                Close();
            }
            else if (res == 1) {
                await Utils.MessageTips("旧密码不正确。", "OperationDialog");
                return;
            }
            else if (res == 2) {
                await Utils.MessageTips("数据库错误。", "OperationDialog");
                return;
            }
            else {
                await Utils.MessageTips("未知错误。", "OperationDialog");
                return;
            }
        }

        private async void RemovePaper(object sender, RoutedEventArgs e) {

        }
    }
}
