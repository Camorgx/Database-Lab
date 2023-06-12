using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3 {
    /// <summary>
    /// ProjectFullView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectFullView : UserControl {
        public ProjectFullView() {
            InitializeComponent();
        }

        public class Pair {
            public string 工号 { get; set; } = "";
            public float 承担经费 { get; set; } = 0;
        }

        private bool isReadOnly = false;
        public bool IsReadOnly {
            get { return isReadOnly; }
            set {
                isReadOnly = value;
                projectID.IsReadOnly = value;
                projectName.IsReadOnly = value;
                projectSouce.IsReadOnly = value;
                projectType.IsEnabled = !value;
                totalMoney.IsReadOnly = value;
                startYear.IsEnabled = !value;
                endYear.IsEnabled = !value;
                teachers.IsReadOnly = value;
            }
        }

        public ProjectRecord Record { get; set; } = new();

        public List<Pair> Authors { get; set; } = new();

        private void UserControlLoaded(object sender, System.Windows.RoutedEventArgs e) {
            foreach (var (id, _, money) in Record.teachers)
                Authors.Add(new Pair { 工号 = id, 承担经费 = money });
            teachers.ItemsSource = Authors;
            projectID.Text = Record.id;
            projectName.Text = Record.name;
            projectSouce.Text = Record.source;
            startYear.Text = Record.startYear.ToString();
            endYear.Text = Record.endYear.ToString();
            projectType.SelectedIndex = Record.type;
            totalMoney.Text = Record.totalMoney.ToString();
        }

        private void InputNumberOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        public ProjectRecord GetProjectRecord() {
            var res = new ProjectRecord {
                id = projectID.Text,
                name = projectName.Text,
                source = projectSouce.Text,
                startYear = int.Parse(startYear.Text),
                endYear = int.Parse(endYear.Text),
                type = projectType.SelectedIndex,
            };
            if (!float.TryParse(totalMoney.Text, out res.totalMoney))
                throw new System.ArgumentException("totalMoney");
            foreach (var item in teachers.Items) {
                Pair pair = (item as Pair) ?? new Pair();
                string id = pair.工号;
                float money = pair.承担经费;
                if (id.Length != 0) res.teachers.Add((id, "", money));
            }
            return res;
        }

        private void DataGridPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            teachers.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
                RoutedEvent = MouseWheelEvent,
                Source = sender
            });
        }
    }
}
