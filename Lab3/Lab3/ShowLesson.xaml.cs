using System.Windows;

namespace Lab3 {
    /// <summary>
    /// ShowLesson.xaml 的交互逻辑
    /// </summary>
    public partial class ShowLesson : Window {
        public ShowLesson() {
            InitializeComponent();
        }

        public bool CheckLessonID { get; set; } = false;
        private static readonly string dialogVerifier = "ShowLessonDialog";

        private async void OKButtonClick(object sender, RoutedEventArgs e) {
            try {
                Global.newLesson = view.GetLessonRecord();
            }
            catch (System.ArgumentException) {
                await Utils.MessageTips("学时数格式不正确。", dialogVerifier);
                return;
            }
            if (Global.newLesson.teachers.Count == 0) {
                await Utils.MessageTips("请添加教师。", dialogVerifier);
                return;
            }
            if (CheckLessonID) {
                bool met = false;
                foreach (var (id, _, _, _, _) in Global.newLesson.teachers)
                    if (Global.teacher.ID == id) met = true;
                if (!met) {
                    await Utils.MessageTips("您只能添加您教学的课程。", dialogVerifier);
                    return;
                }
            }
            if (CheckLessonID && await Database.CheckExistsOfLessonID(Global.newLesson.id)) {
                await Utils.MessageTips("指定的课程号已存在。", dialogVerifier);
                return;
            }
            if (Global.newLesson.type == 0) {
                await Utils.MessageTips("请选择课程性质。", dialogVerifier);
                return;
            }
            int res = Utils.VerifyLessonTeachers(Global.newLesson);
            if (res == 1) await Utils.MessageTips("教师工号的格式不正确。", dialogVerifier);
            else if (res == 2) await Utils.MessageTips("每年每学期所有教师的主讲学时总额应等于课程的总学时。", dialogVerifier);
            else if (res == 3) await Utils.MessageTips("出现重复数据。", dialogVerifier);
            else if (res == 4) await Utils.MessageTips("请选择学期。", dialogVerifier);
            else {
                var verRes = await Database.VerifiLessonTeachers(Global.newLesson);
                if (verRes != "ok")
                    await Utils.MessageTips($"工号{verRes}不存在。", dialogVerifier);
                else {
                    ((Operation)Owner).verifyToModifyLesson = true;
                    Close();
                }
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e) {
            Title = Message.Content.ToString();
            ((Operation)Owner).lessonCreateWindowOpen = true;
        }

        private void WindowClosed(object sender, System.EventArgs e) {
            ((Operation)Owner).lessonCreateWindowOpen = false;
        }

        private void WindowMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DragMove();
        }
    }
}
