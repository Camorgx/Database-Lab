using System.Windows;

namespace Lab3 {
    /// <summary>
    /// ShowProject.xaml 的交互逻辑
    /// </summary>
    public partial class ShowProject : Window {
        public ShowProject() {
            InitializeComponent();
        }

        private bool isReadOnly = false;
        public bool IsReadOnly {
            get { return isReadOnly; }
            set {
                isReadOnly = value;
                view.IsReadOnly = value;
            }
        }

        public bool CheckProjectID { get; set; } = false;
        private static readonly string dialogVerifier = "ShowProjectDialog";

        private async void OKButtonClick(object sender, RoutedEventArgs e) {
            try {
                Global.newProject = view.GetProjectRecord();
            }
            catch (System.ArgumentException) {
                await Utils.MessageTips("项目总金额格式不正确。", dialogVerifier);
                return;
            }
            if (Global.newProject.teachers.Count == 0) {
                await Utils.MessageTips("请添加教师。", dialogVerifier);
                return;
            }
            if (CheckProjectID && Global.newProject.teachers[0].id != Global.teacher.ID) {
                await Utils.MessageTips("您只能申报您负责的项目。", dialogVerifier);
                return;
            }
            if (CheckProjectID && await Database.CheckExistsOfProjectID(Global.newProject.id)) {
                await Utils.MessageTips("指定的项目号已存在。", dialogVerifier);
                return;
            }
            if (Global.newProject.type == 0) {
                await Utils.MessageTips("请选择项目类型。", dialogVerifier);
                return;
            }
            if (Global.newProject.endYear < Global.newProject.startYear) {
                await Utils.MessageTips("项目的结束年份需要晚于开始年份。", dialogVerifier);
                return;
            }
            int res = Utils.VerifyProjectTeachers(Global.newProject);
            if (res == 1) await Utils.MessageTips("教师工号的格式不正确。", dialogVerifier);
            else if (res == 2) await Utils.MessageTips("所有教师的分担金额应等于项目的总金额。", dialogVerifier);
            else if (res == 3) await Utils.MessageTips("教师的排名应唯一。", dialogVerifier);
            else {
                var verRes = await Database.VerifiProjectTeachers(Global.newProject);
                if (verRes != "ok")
                    await Utils.MessageTips($"工号{verRes}不存在。", dialogVerifier);
                else {
                    ((Operation)Owner).verifyToModifyProject = true;
                    Close();
                }
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e) {
            Title = Message.Content.ToString();
            ((Operation)Owner).projectCreateWindowOpen = true;
        }

        private void WindowClosed(object sender, System.EventArgs e) {
            ((Operation)Owner).projectCreateWindowOpen = false;
        }

        private void WindowMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DragMove();
        }
    }
}
