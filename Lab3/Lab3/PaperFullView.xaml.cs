using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Lab3 {
    /// <summary>
    /// PaperFullView.xaml 的交互逻辑
    /// </summary>
    public partial class PaperFullView : UserControl {
        public PaperFullView() {
            InitializeComponent();
        }

        public class Pair {
            public string 作者工号 { get; set; } = "";
            public string 作者姓名 { get; set; } = "";
            public string 是否为通讯作者 { get; set; } = "";
        }

        public PaperRecord Record { get; set; } = new();

        public List<Pair> Authors { get; set; } = new();

        private void UserControlLoaded(object sender, System.Windows.RoutedEventArgs e) {
            foreach (var (id, name, cor) in Record.authors)
                Authors.Add(new Pair { 作者工号 = id, 作者姓名 = name, 是否为通讯作者 = ItemTranslation.Corresponding[cor] });
            authors.ItemsSource = Authors;
            paperID.Text = Record.id.ToString();
            paperName.Text = Record.name;
            paperSouce.Text = Record.source;
            paperYear.Text = Record.year.ToString();
            paperType.SelectedIndex = Record.type - 1;
            paperLevel.SelectedIndex = Record.level - 1;
        }

        private void paperIDPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}
