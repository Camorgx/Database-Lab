using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3 {
    /// <summary>
    /// SearchLesson.xaml 的交互逻辑
    /// </summary>
    public partial class SearchLesson : UserControl {
        public SearchLesson() {
            InitializeComponent();
        }

        public Operation? Owner { get; set; } = null;

        public string DialogIdentifier { get; set; } = "OperationDialog";

        private async Task<string> GenerateReqString() {
            IList<string> arguments = new List<string>();
            int startHour = 0, endHour = 0;
            if (totalHour.Text.Length > 0) {
                string input = totalHour.Text;
                int pos = input.IndexOf("-");
                if (pos == -1) {
                    await Utils.MessageTips("学时数范围格式不正确。", DialogIdentifier);
                    throw new System.ArgumentException();
                }
                if (!int.TryParse(input[..pos], out startHour)
                    || !int.TryParse(input[(pos + 1)..], out endHour)) {
                    await Utils.MessageTips("学时数范围格式不正确。", DialogIdentifier);
                    throw new System.ArgumentException();
                }
            }
            int startYear = 0, endYear = 0;
            if (this.startYear.Text.Length > 0) startYear = int.Parse(this.startYear.Text);
            if (this.endYear.Text.Length > 0) endYear = int.Parse(this.endYear.Text);
            if (endYear < startYear) {
                await Utils.MessageTips("终止年份应晚于起始年份。", DialogIdentifier);
                throw new System.ArgumentException();
            }
            string union = "select lesson.lessonID as lessonID from lesson, teach ";
            string single = "select lessonID from lesson ";
            string initString = single;
            if (teacherID.Text.Length > 0) {
                arguments.Add($"teacherID = {teacherID.Text}");
                arguments.Add("lesson.lessonID = teach.lessonID");
                initString = union;
            }
            if (lessonID.Text.Length > 0) arguments.Add($"lesson.lessonID = {lessonID.Text}");
            if (lessonName.Text.Length > 0) arguments.Add($"lessonName = '{lessonName.Text}'");
            if (this.startYear.Text.Length > 0) arguments.Add($"lessonYear >= {startYear}");
            if (this.endYear.Text.Length > 0) arguments.Add($"lessonYear <= {endYear}");
            if (lessonType.SelectedIndex > 0) arguments.Add($"lessonType = {lessonType}");
            if (totalHour.Text.Length > 0) arguments.Add($"totalHour >= {startHour} and totalMoney <= {endHour}");
            if (lessonTerm.SelectedIndex > 0) {
                arguments.Add($"term = {lessonTerm.SelectedIndex}");
                initString = union;
            }
            if (arguments.Count == 0) return initString;
            var sb = new StringBuilder(initString + "where ");
            for (int i = 0; i < arguments.Count; i++) {
                sb.Append(arguments[i]);
                if (i < arguments.Count - 1) sb.Append(" and ");
            }
            return sb.ToString();
        }

        private void InputNumberOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void DataGridMouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var show = new ShowLesson {
                Message = { Content = "查看课程详情" },
                view = {
                    Record = records[((Displayed)searchResult.SelectedItem).课程号],
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Owner
            };
            show.IsReadOnly = true;
            show.ShowDialog();
        }

        private Dictionary<string, LessonRecord> records = new();

        private class Displayed {
            public string 课程号 { get; set; } = "";
            public string 课程名称 { get; set; } = "";
            public int 学时数 { get; set; } = 0;
            public string 课程性质 { get; set; } = "";
        }

        private readonly List<Displayed> displayData = new();

        private void UpdateDisplayData() {
            displayData.Clear();
            foreach (var (key, value) in records) {
                displayData.Add(new Displayed {
                    课程号 = key,
                    课程名称 = value.name,
                    学时数 = value.totalHour,
                    课程性质 = ItemTranslation.LessonType[value.type],
                });
            }
        }

        private async void SearchButtonClick(object sender, RoutedEventArgs e) {
            var window = new PleaseWait() {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Owner
            };
            window.Show();
            window.Activate();
            try {
                string req = await GenerateReqString();
                records = await Database.SearchLessonWithRequirement(req);
                UpdateDisplayData();
                searchResult.ItemsSource = displayData;
                searchResult.Items.Refresh();
            }
            catch (System.ArgumentException) { }
            finally {
                window.Close();
                Owner?.Activate();
            }
        }

        private void SearchResultPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            searchResult.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) {
                RoutedEvent = MouseWheelEvent,
                Source = sender
            });
        }
    }
}
