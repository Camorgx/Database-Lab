using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;

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

        private void InputNumberOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
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

        private void InitProject() {
            Utils.UpdateProjectView();
            ownProject.ItemsSource = Global.ownProject;
            partedProject.ItemsSource = Global.partedProject;
        }

        private void InitLesson() {
            userLesson.ItemsSource = Global.userLesson;
        }

        private void InitSearch() {
            searchPaper.Owner = this;
            searchTeacher.Owner = this;
            searchProject.Owner = this;
            searchLesson.Owner = this;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e) {
            InitMyInfo();
            InitPaper();
            InitProject();
            InitLesson();
            InitSearch();
        }

        private static readonly string dialogIdentifier = "OperationDialog";

        private async void UpdateButtonClick(object sender, RoutedEventArgs e) {
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
            var newName = name.Text;
            var newGender = gender.SelectedIndex;
            if (newGender == 0) {
                await Utils.MessageTips("请选择性别。", dialogIdentifier);
                return;
            }
            var newTitle = teacherTitle.SelectedIndex;
            if (newTitle == 0) {
                await Utils.MessageTips("请选择职称。", dialogIdentifier);
                return;
            }
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
            window.Hide();
            showPaper.ShowDialog();
            Activate();
            if (!verifyToModifyPaper) {
                window.Close();
                return;
            }
            verifyToModifyPaper = true;
            window.Show();
            bool authoreCmp = Utils.CompareAuthorList(currentRecord, Global.newPaper);
            bool attrCmp = Utils.ComparePaperAttr(currentRecord, Global.newPaper);
            Database.UpdateMode mode = attrCmp ?
                (authoreCmp ? Database.UpdateMode.None : Database.UpdateMode.TeacherOnly) :
                (authoreCmp ? Database.UpdateMode.AttrOnly : Database.UpdateMode.All);
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
                Message = { Content = "申报论文" },
                CheckPaperID = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            verifyToModifyPaper = false;
            showPaper.ShowDialog();
            Activate();
            if (!verifyToModifyPaper) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
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

        private async void RemoveProject(object sender, RoutedEventArgs e) {
            if (ownProject.SelectedItem is not Project project) return;
            if (!await Utils.VerificationDialog($"将删除项目号: {project.项目号}，不可恢复，是否确定？",
                dialogIdentifier)) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
            var id = project.项目号;
            int res = await Database.RemoveProject(id);
            await RefreshProject();
            window.Close();
            if (res == 0) await Utils.MessageTips("所选项目已删除。", dialogIdentifier);
            else if (res == 1) await Utils.MessageTips("数据库错误。", dialogIdentifier);
            else await Utils.MessageTips("未知错误。", dialogIdentifier);
        }

        private async Task<bool> RefreshProject() {
            await Database.LoadProjectData(Global.teacher.ID);
            Utils.UpdateProjectView();
            ownProject.Items.Refresh();
            partedProject.Items.Refresh();
            return true;
        }

        private async void RefreshProjectButtonClick(object sender, RoutedEventArgs e) {
            var refresh = RefreshProject();
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
            await refresh;
            window.Close();
        }

        public bool verifyToModifyProject = false;
        public bool projectCreateWindowOpen = false;

        private async void ModifyProjectButtonClick(object sender, RoutedEventArgs e) {
            verifyToModifyProject = false;
            if (ownProject.SelectedItem is not Project project) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            var record = Database.SearchProject(project.项目号);
            var currentRecord = await record;
            var showProject = new ShowProject {
                Message = { Content = "修改项目信息" },
                view = {
                    Record = currentRecord,
                    projectID = { IsEnabled = false },
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            window.Hide();
            showProject.ShowDialog();
            Activate();
            if (!verifyToModifyProject) {
                window.Close();
                return;
            }
            verifyToModifyProject = true;
            window.Show();
            bool authoreCmp = Utils.CompareTeacherList(currentRecord, Global.newProject);
            bool attrCmp = Utils.CompareProjectAttr(currentRecord, Global.newProject);
            Database.UpdateMode mode = attrCmp ?
                (authoreCmp ? Database.UpdateMode.None : Database.UpdateMode.TeacherOnly) :
                (authoreCmp ? Database.UpdateMode.AttrOnly : Database.UpdateMode.All);
            bool res = await Database.UpdateProject(Global.newProject, mode);
            window.Close();
            if (res) {
                await Utils.MessageTips("项目信息更新完成。", dialogIdentifier);
                RefreshProjectButtonClick(sender, e);
            }
            else await Utils.MessageTips($"项目信息更新失败。", dialogIdentifier);
        }

        private async void NewProjectButtonClick(object sender, RoutedEventArgs e) {
            var showProject = new ShowProject {
                Message = { Content = "申报项目" },
                CheckProjectID = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            verifyToModifyProject = false;
            showProject.ShowDialog();
            Activate();
            if (!verifyToModifyProject) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            bool res = await Database.AddProject(Global.newProject);
            window.Close();
            if (res) {
                await Utils.MessageTips("项目添加成功。", dialogIdentifier);
                RefreshProjectButtonClick(sender, e);
            }
            else await Utils.MessageTips("项目添加失败。", dialogIdentifier);
        }

        private async void RemoveLesson(object sender, RoutedEventArgs e) {
            if (userLesson.SelectedItem is not Lesson lesson) return;
            if (!await Utils.VerificationDialog($"将删除课程号: {lesson.课程号}，不可恢复，是否确定？",
                dialogIdentifier)) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
            var id = lesson.课程号;
            int res = await Database.RemoveLesson(id);
            await RefreshLesson();
            window.Close();
            if (res == 0) await Utils.MessageTips("所选课程已删除。", dialogIdentifier);
            else if (res == 1) await Utils.MessageTips("数据库错误。", dialogIdentifier);
            else await Utils.MessageTips("未知错误。", dialogIdentifier);
        }

        private async Task<bool> RefreshLesson() {
            await Database.LoadLessonData(Global.teacher.ID);
            userLesson.Items.Refresh();
            return true;
        }

        private async void RefreshLessonButtonClick(object sender, RoutedEventArgs e) {
            var refresh = RefreshLesson();
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Show();
            await refresh;
            window.Close();
        }

        public bool verifyToModifyLesson = false;
        public bool lessonCreateWindowOpen = false;

        private async void ModifyLessonButtonClick(object sender, RoutedEventArgs e) {
            verifyToModifyLesson = false;
            if (userLesson.SelectedItem is not Lesson lesson) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            var record = Database.SearchLesson(lesson.课程号);
            var currentRecord = await record;
            var showLesson = new ShowLesson {
                Message = { Content = "修改课程信息" },
                view = {
                    Record = currentRecord,
                    lessonID = { IsEnabled = false },
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            window.Hide();
            showLesson.ShowDialog();
            Activate();
            if (!verifyToModifyLesson) {
                window.Close();
                return;
            }
            verifyToModifyLesson = true;
            window.Show();
            bool authoreCmp = Utils.CompareTeacherList(currentRecord, Global.newLesson);
            bool attrCmp = Utils.CompareLessonAttr(currentRecord, Global.newLesson);
            Database.UpdateMode mode = attrCmp ?
                (authoreCmp ? Database.UpdateMode.None : Database.UpdateMode.TeacherOnly) :
                (authoreCmp ? Database.UpdateMode.AttrOnly : Database.UpdateMode.All);
            bool res = await Database.UpdateLesson(Global.newLesson, mode);
            window.Close();
            if (res) {
                await Utils.MessageTips("课程信息更新完成。", dialogIdentifier);
                RefreshLessonButtonClick(sender, e);
            }
            else await Utils.MessageTips($"课程信息更新失败。", dialogIdentifier);
        }

        private async void NewLessonButtonClick(object sender, RoutedEventArgs e) {
            var showLesson = new ShowLesson {
                Message = { Content = "添加新课程" },
                CheckLessonID = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            verifyToModifyLesson = false;
            showLesson.ShowDialog();
            Activate();
            if (!verifyToModifyLesson) return;
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = Left + Width / 2.5,
                Top = Top + Height / 2.5,
            };
            window.Show();
            bool res = await Database.AddLesson(Global.newLesson);
            window.Close();
            if (res) {
                await Utils.MessageTips("课程添加成功。", dialogIdentifier);
                RefreshLessonButtonClick(sender, e);
            }
            else await Utils.MessageTips("课程添加失败。", dialogIdentifier);
        }

        private void searchTypeSelectionChanged(object sender, SelectionChangedEventArgs e) {
            UserControl[] controls = { searchTeacher, searchPaper, searchProject, searchLesson };
            for (int i = 0; i < controls.Length; ++i) {
                if (controls[i] is not null)
                    controls[i].Visibility = (i == searchType.SelectedIndex - 1) ? 
                        Visibility.Visible : Visibility.Hidden;
            }
        }

        private void OwnPaperMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            ModifyPaperButtonClick(sender, e);
        }

        private void OwnProjectMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            ModifyProjectButtonClick(sender, e);
        }

        private void OwnLessonMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            ModifyLessonButtonClick(sender, e);
        }

        private void TotalButtonClick(object sender, RoutedEventArgs e) {

        }

        private void ExportButtonClick(object sender, RoutedEventArgs e) {

        }
    }
}
