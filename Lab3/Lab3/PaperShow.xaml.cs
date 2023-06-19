using System.Windows;

namespace Lab3 {
    /// <summary>
    /// ShowPaper.xaml 的交互逻辑
    /// </summary>
    public partial class ShowPaper : Window {
        public ShowPaper() {
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

        public bool CheckPaperID { get; set; } = false;
        private static readonly string dialogVerifier = "ShowPaperDialog";

        private async void OKButtonClick(object sender, RoutedEventArgs e) {
            Global.newPaper = view.GetPaperRecord();
            if (Global.newPaper.authors.Count == 0) {
                await Utils.MessageTips("请添加作者。", dialogVerifier);
                return;
            }
            if (CheckPaperID && Global.newPaper.authors[0].id != Global.teacher.ID) {
                await Utils.MessageTips("您只能申报第一作者是您的论文。", dialogVerifier);
                return;
            }
            if (CheckPaperID && await Database.CheckExistsOfPaperID(Global.newPaper.id)) {
                await Utils.MessageTips("指定的论文序号已存在。", dialogVerifier);
                return;
            }
            if (Global.newPaper.level == 0) {
                await Utils.MessageTips("请选择论文级别。", dialogVerifier);
                return;
            }
            if (Global.newPaper.type == 0) {
                await Utils.MessageTips("请选择论文类型。", dialogVerifier);
                return;
            }
            int res = Utils.VerifyPaperAuthors(Global.newPaper);
            if (res == 1) await Utils.MessageTips("作者工号的格式不正确。", dialogVerifier);
            else if (res == 2) await Utils.MessageTips("一篇论文只能有一个通讯作者。", dialogVerifier);
            else if (res == 3) await Utils.MessageTips("作者的排名应唯一。", dialogVerifier);
            else {
                var verRes = await Database.VerifiPaperAuthors(Global.newPaper);
                if (verRes != "ok")
                    await Utils.MessageTips($"工号{verRes[1..]}不存在。", dialogVerifier);
                else {
                    ((Operation)Owner).verifyToModifyPaper = true;
                    Close();
                }
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e) {
            Title = Message.Content.ToString();
            ((Operation)Owner).paperCreateWindowOpen = true;
        }

        private void WindowClosed(object sender, System.EventArgs e) {
            ((Operation)Owner).paperCreateWindowOpen = false;
        }

        private void WindowMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DragMove();
        }
    }
}
