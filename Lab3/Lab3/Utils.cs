using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;

namespace Lab3 {
    static class Utils {
        public static async Task<bool> MessageTips(string message, string identifier) {
            var sampleMessageDialog = new MessageDialog {
                Message = { Text = message }
            };
            await DialogHost.Show(sampleMessageDialog, identifier);
            return true;
        }

        public static bool VerifyPassword(string password) {
            if (password.Length < 8 || password.Length > 20) return false;
            bool findAlpha = false, findNumber = false;
            foreach (char ch in password) {
                if (char.IsDigit(ch)) findNumber = true;
                if (char.IsLetter(ch)) findAlpha = true;
            }
            if (!findNumber || !findAlpha) return false;
            return true;
        }

        public static void UpdatePaperView() {
            Global.ownPaper.Clear();
            Global.partedPaper.Clear();
            foreach (var paper in Global.userPaper) {
                if (paper.排名 == 1)
                    Global.ownPaper.Add(paper);
                else Global.partedPaper.Add(paper);
            }
        }
    }
}
