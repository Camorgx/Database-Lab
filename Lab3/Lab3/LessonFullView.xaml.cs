using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3 {
    /// <summary>
    /// LessonFullView.xaml 的交互逻辑
    /// </summary>
    public partial class LessonFullView : UserControl {
        public LessonFullView() {
            InitializeComponent();
        }

        public class Pair {
            public string 工号 { get; set; } = "";
            public int 年份 { get; set; } = 2010;
            public Term 学期 { get; set; } = Term.请选择;
            public int 承担学时 { get; set; } = 0;
        }

        public enum Term {
            请选择,
            春季学期,  
            夏季学期,  
            秋季学期,  
        }

        private bool isReadOnly = false;
        public bool IsReadOnly {
            get { return isReadOnly; }
            set {
                isReadOnly = value;
                lessonID.IsReadOnly = value;
                lessonName.IsReadOnly = value;
                lessonHour.IsReadOnly = value;
                lessonType.IsEnabled = !value;
                teachers.IsReadOnly = value;
            }
        }

        public LessonRecord Record { get; set; } = new();

        public List<Pair> Authors { get; set; } = new();

        private void UserControlLoaded(object sender, System.Windows.RoutedEventArgs e) {
            foreach (var (id, _, year, term, hour) in Record.teachers)
                Authors.Add(new Pair { 工号 = id, 年份 = year, 学期 = (Term)term, 承担学时 = hour });
            teachers.ItemsSource = Authors;
            lessonID.Text = Record.id;
            lessonName.Text = Record.name;
            lessonHour.Text = Record.totalHour.ToString();
            lessonType.SelectedIndex = Record.type;
        }

        private void InputNumberOnly(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        public LessonRecord GetLessonRecord() {
            var res = new LessonRecord {
                id = lessonID.Text,
                name = lessonName.Text,
                type = lessonType.SelectedIndex,
            };
            if (!int.TryParse(lessonHour.Text, out res.totalHour))
                throw new System.ArgumentException();
            foreach (var item in teachers.Items) {
                Pair pair = (item as Pair) ?? new Pair();
                string id = pair.工号;
                int year = pair.年份;
                int term = (int)pair.学期;
                int hour = pair.承担学时;
                if (id.Length != 0) res.teachers.Add((id, "", year, term, hour));
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
