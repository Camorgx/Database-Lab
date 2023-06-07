using MaterialDesignThemes.Wpf;

namespace Lab3 {
    static class Utils {
        public static async void MessageTips(string message) {
            var sampleMessageDialog = new MessageDialog {
                Message = { Text = message }
            };
            await DialogHost.Show(sampleMessageDialog, "RootDialog");
        }
    }
}
