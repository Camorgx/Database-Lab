using System.Windows;
using System.Windows.Input;

namespace Lab3 {
    /// <summary>
    /// ResetPassword.xaml 的交互逻辑
    /// </summary>
    public partial class ResetPassword : Window {
        public ResetPassword() {
            InitializeComponent();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e) {
            Owner.Left = Left;
            Owner.Top = Top;
            Owner.Show();
            Close();
        }

        private void MoveWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }
    }
}
