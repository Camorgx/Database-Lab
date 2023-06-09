using MaterialDesignThemes.Wpf;
using System.Windows;

namespace Lab3 {
    /// <summary>
    /// ShowPaper.xaml 的交互逻辑
    /// </summary>
    public partial class ShowPaper : Window {
        public ShowPaper() {
            InitializeComponent();
        }

        private async void OKButtonClick(object sender, RoutedEventArgs e) {
            Global.newPaper = view.GetPaperRecord();
            int res = Utils.VerifyReaderAuthors(Global.newPaper);
            if (res == 1) {
                await Utils.MessageTips("作者工号的格式不正确。", "ShowPaperDialog");
            }
            else if (res == 2) {
                await Utils.MessageTips("一篇论文只能有一个通讯作者。", "ShowPaperDialog");
            }
            else {
                var verRes = await Database.VerifiPaperAuthors(Global.newPaper);
                if (verRes != "ok")
                    await Utils.MessageTips($"工号{verRes}不存在。", "ShowPaperDialog");
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
