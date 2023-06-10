using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

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
            else if (Owner is MainWindow window) {
                window.rememberMe.IsChecked = false;
                window.password.Password = "";
            }
        }

        private void LogoutButtonClick(object sender, RoutedEventArgs e) {
            logOut = true;
            Owner.Show();
            Close();
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
            Utils.UpdatePaperView();
            ownPaper.ItemsSource = Global.ownPaper;
            partedPaper.ItemsSource = Global.partedPaper;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e) {
            InitMyInfo();
            InitPaper();
        }

        private static readonly string dialogIdentifier = "OperationDialog";

        private async void UpdateButtonClick(object sender, RoutedEventArgs e) {
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
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
                await Utils.MessageTips("修改成功。", dialogIdentifier);
            }
            else await Utils.MessageTips("修改失败。", dialogIdentifier);
        }

        private async void ResetPasswordButtonClick(object sender, RoutedEventArgs e) {
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
            var id = Global.teacher.ID;
            var oldPwd = oldPassword.Password;
            var newPwd = newPassword.Password;
            var verPwd = verifyPassword.Password;
            if (newPwd != verPwd) {
                await Utils.MessageTips("两次输入的密码不一致。", dialogIdentifier);
                return;
            }
            if (oldPwd == newPwd) {
                await Utils.MessageTips("旧密码与新密码相同。", dialogIdentifier);
                return;
            }
            if (!Utils.VerifyPassword(newPwd)) {
                await Utils.MessageTips("密码格式不正确。", dialogIdentifier);
                return;
            }
            int res = await Database.UpdatePassword(id, oldPwd, newPwd);
            window.Close();
            if (res == 0) {
                await Utils.MessageTips("密码更改成功，请重新登录系统。", dialogIdentifier);
                logOut = true;
                Owner.Show();
                Close();
            }
            else if (res == 1) await Utils.MessageTips("旧密码不正确。", dialogIdentifier);
            else if (res == 2) await Utils.MessageTips("数据库错误。", dialogIdentifier);
            else await Utils.MessageTips("未知错误。", dialogIdentifier);
        }

        private async void RemovePaper(object sender, RoutedEventArgs e) {
            if (ownPaper.SelectedItem is not Paper paper) return;
            if (!await Utils.VerificationDialog($"将删除论文序号: {paper.序号}，不可恢复，是否确定？", 
                dialogIdentifier)) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
            var id = paper.序号;
            int res = await Database.RemovePaper(id);
            await RefreshPaper();
            window.Close();
            if (res == 0) await Utils.MessageTips("所选论文已删除。", dialogIdentifier);
            else if (res == 1) await Utils.MessageTips("数据库错误。", dialogIdentifier); 
            else await Utils.MessageTips("未知错误。", dialogIdentifier);
        }

        private async Task<bool> RefreshPaper() {
            await Database.LoadPaperData(Global.teacher.ID);
            Utils.UpdatePaperView();
            ownPaper.Items.Refresh();
            partedPaper.Items.Refresh();
            return true;
        }

        private async void RefreshPaperButtonClick(object sender, RoutedEventArgs e) {
            var refresh = RefreshPaper();
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
            await refresh;
            window.Close();
        }

        public bool verifyToModifyPaper = false;
        public bool paperCreateWindowOpen = false;

        private async void ModifyPaperButtonClick(object sender, RoutedEventArgs e) {
            verifyToModifyPaper = false;
            if (ownPaper.SelectedItem is not Paper paper) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            var record = Database.SearchPaper(paper.序号);
            var currentRecord = await record;
            var showPaper = new ShowPaper {
                Message = { Content = "修改论文信息" },
                view = {
                    Record = currentRecord,
                    paperID = { IsEnabled = false },
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            showPaper.Show();
            window.Hide();
            await Task.Run(() => {
                while (paperCreateWindowOpen) ;
            });
            Activate();
            if (!verifyToModifyPaper) {
                window.Close();
                return;
            }
            verifyToModifyPaper = true;
            window.Show();
            bool authoreCmp = Utils.CompareAuthorList(currentRecord, Global.newPaper);
            bool attrCmp = Utils.ComparePaperAttr(currentRecord, Global.newPaper);
            Database.PaperUpdateMode mode = attrCmp ?
                (authoreCmp ? Database.PaperUpdateMode.None : Database.PaperUpdateMode.AuthorOnly) :
                (authoreCmp ? Database.PaperUpdateMode.AttrOnly : Database.PaperUpdateMode.All);
            bool res = await Database.UpdatePaper(Global.newPaper, mode);
            window.Close();
            if (res) {
                await Utils.MessageTips("论文信息更新完成。", dialogIdentifier);
                RefreshPaperButtonClick(sender, e);
            }
            else await Utils.MessageTips($"论文信息更新失败。", dialogIdentifier);
        }

        private async void NewPaperButtonClick(object sender, RoutedEventArgs e) {
            var showPaper = new ShowPaper {
                Message = { Content = "申报论文信息" },
                CheckPaperID = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            verifyToModifyPaper = false;
            showPaper.Show();
            await Task.Run(() => {
                while (paperCreateWindowOpen) ;
            }); 
            Activate();
            if (!verifyToModifyPaper) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            bool res = await Database.AddPaper(Global.newPaper);
            window.Close();
            if (res) {
                await Utils.MessageTips("论文添加成功。", dialogIdentifier);
                RefreshPaperButtonClick(sender, e);
            }
            else await Utils.MessageTips("论文添加失败。", dialogIdentifier);
        }
    }
}
