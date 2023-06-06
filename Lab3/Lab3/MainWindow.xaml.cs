using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab3 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void MoveWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private async Task<string> GetYiYanWords() {
            string res;
            try {
                var client = new HttpClient();
                res = await client.GetStringAsync("https://v1.hitokoto.cn/?c=k&encode=text");
            }
            catch (Exception) {
                res = "我们所爱之物昭示着我们究竟是谁。";
            }
            return res;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            apothegm.Text = await GetYiYanWords();
        }
    }
}
