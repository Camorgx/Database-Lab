using System.Collections.Generic;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

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
            public YesOrNo 是否为通讯作者 { get; set; } = YesOrNo.否;
        }

        private bool isReadOnly = false;
        public bool IsReadOnly {
            get { return isReadOnly; }
            set {
                isReadOnly = value;
                paperID.IsReadOnly = value;
                paperName.IsReadOnly = value;
                paperSouce.IsReadOnly = value;
                paperType.IsEnabled = !value;
                paperYear.IsReadOnly = value;
                paperLevel.IsEnabled = !value;
                authors.IsReadOnly = value;
            }
        }

        public PaperRecord Record { get; set; } = new();

        public List<Pair> Authors { get; set; } = new();

        public enum YesOrNo : byte {
            是 = 1,
            否 = 0
        }

        private void UserControlLoaded(object sender, System.Windows.RoutedEventArgs e) {
            foreach (var (id, _, cor) in Record.authors)
                Authors.Add(new Pair { 作者工号 = id, 是否为通讯作者 = (YesOrNo)cor });
            authors.ItemsSource = Authors;
            paperID.Text = Record.id.ToString();
            paperName.Text = Record.name;
            paperSouce.Text = Record.source;
            paperYear.Text = Record.year.ToString();
            paperType.SelectedIndex = Record.type;
            paperLevel.SelectedIndex = Record.level;
        }

        private void PaperIDPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        public PaperRecord GetPaperRecord() {
            var res = new PaperRecord {
                id = int.Parse(paperID.Text),
                name = paperName.Text,
                source = paperSouce.Text,
                year = int.Parse(paperYear.Text),
                type = paperType.SelectedIndex,
                level = paperLevel.SelectedIndex,
            };
            foreach (var item in authors.Items) {
                Pair pair = (item as Pair) ?? new Pair();
                string id = pair.作者工号;
                int cor = (int)pair.是否为通讯作者;
                if (id.Length != 0)
                    res.authors.Add((id, "", cor));
            }
            return res;
        }

        private void DataGridPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            authors.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
                RoutedEvent = MouseWheelEvent,
                Source = sender
            });
        }
    }
}
