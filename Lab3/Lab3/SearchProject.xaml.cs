using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3 {
    /// <summary>
    /// SearchProject.xaml 的交互逻辑
    /// </summary>
    public partial class SearchProject : UserControl {
        public SearchProject() {
            InitializeComponent();
        }

        public Operation? Owner { get; set; } = null;

        public string DialogIdentifier { get; set; } = "OperationDialog";

        private async Task<string> GenerateReqString() {
            IList<string> arguments = new List<string>();
            float startMoney = 0, endMoney = 0;
            if (totalMoney.Text.Length > 0) {
                string input = totalMoney.Text;
                int pos = input.IndexOf("-");
                if (pos == -1) {
                    await Utils.MessageTips("总经费范围格式不正确。", DialogIdentifier);
                    throw new System.ArgumentException();
                }
                if (!float.TryParse(input[..pos], out startMoney)
                    || !float.TryParse(input[(pos + 1)..], out endMoney)) {
                    await Utils.MessageTips("总经费范围格式不正确。", DialogIdentifier);
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
            string initString;
            if (teacherID.Text.Length > 0) {
                arguments.Add($"teacherID = {teacherID.Text}");
                arguments.Add("project.projectID = undertake.projectID");
                initString = "select project.projectID as projectID from project, undertake ";
            }
            else initString = "select projectID from project ";
            if (projectID.Text.Length > 0) arguments.Add($"project.projectID = {projectID.Text}");
            if (projectName.Text.Length > 0) arguments.Add($"projectName = '{projectName.Text}'");
            if (projectSource.Text.Length > 0) arguments.Add($"projectSource = '{projectSource.Text}'");
            if (this.startYear.Text.Length > 0) arguments.Add($"projectYear >= {startYear}");
            if (this.endYear.Text.Length > 0) arguments.Add($"projectYear <= {endYear}");
            if (projectType.SelectedIndex > 0) arguments.Add($"projectType = {projectType.SelectedIndex}");
            if (totalMoney.Text.Length > 0) arguments.Add($"totalMoney >= {startMoney} and totalMoney <= {endMoney}");
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
            var show = new ShowProject {
                Message = { Content = "查看项目详情" },
                view = {
                    Record = records[((Displayed)searchResult.SelectedItem).项目号],
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Owner
            };
            show.IsReadOnly = true;
            show.ShowDialog();
        }

        private Dictionary<string, ProjectRecord> records = new();

        private class Displayed {
            public string 项目号 { get; set; } = "";
            public string 项目名称 { get; set; } = "";
            public string 项目来源 { get; set; } = "";
            public string 项目类型 { get; set; } = "";
            public float 总经费 { get; set; } = 0;
            public int 开始年份 { get; set; } = 2010;
            public int 结束年份 { get; set; } = 2020;
        }

        private readonly List<Displayed> displayData = new();

        private void UpdateDisplayData() {
            displayData.Clear();
            foreach (var (key, value) in records) {
                displayData.Add(new Displayed {
                    项目号 = key,
                    项目名称 = value.name,
                    项目来源 = value.source,
                    开始年份 = value.startYear,
                    结束年份 = value.endYear,
                    总经费 = value.totalMoney,
                    项目类型 = ItemTranslation.ProjectType[value.type],
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
                records = await Database.SearchProjectWithRequirement(req);
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
