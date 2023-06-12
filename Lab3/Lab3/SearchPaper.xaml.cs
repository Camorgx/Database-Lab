using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3 {
    /// <summary>
    /// SearchPaper.xaml 的交互逻辑
    /// </summary>
    public partial class SearchPaper : UserControl {
        public SearchPaper() {
            InitializeComponent();
        }

        public Operation? Owner { get; set; } = null;

        public string DialogIdentifier { get; set; } = "OperationDialog";

        private async Task<string> GenerateReqString() {
            IList<string> arguments = new List<string>();
            int start = 0, end = 0;
            if (startYear.Text.Length > 0) start = int.Parse(startYear.Text);
            if (endYear.Text.Length > 0) end = int.Parse(endYear.Text);
            if (end < start) {
                await Utils.MessageTips("终止年份应晚于起始年份。", DialogIdentifier);
                throw new System.ArgumentException();
            }
            string initString;
            if (teacherID.Text.Length > 0) {
                arguments.Add($"teacherID = {teacherID.Text}");
                arguments.Add("paper.paperID = publish.paperID");
                initString = "select paper.paperID as paperID from paper, publish ";
            }
            else initString = "select paperID from paper ";
            if (paperID.Text.Length > 0) arguments.Add($"paper.paperID = {paperID.Text}");
            if (paperName.Text.Length > 0) arguments.Add($"paperName = '{paperName.Text}'");
            if (paperSource.Text.Length > 0) arguments.Add($"paperSource = '{paperSource.Text}'");
            if (startYear.Text.Length > 0) arguments.Add($"paperYear >= {start}");
            if (endYear.Text.Length > 0) arguments.Add($"paperYear <= {end}");
            if (paperType.SelectedIndex > 0) arguments.Add($"paperType = {paperType.SelectedIndex}");
            if (paperLevel.SelectedIndex > 0) arguments.Add($"level = {paperLevel.SelectedIndex}");
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
            var show = new ShowPaper {
                Message = { Content = "查看论文详情" },
                view = {
                    Record = records[((Displayed)searchResult.SelectedItem).序号],
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Owner
            };
            show.IsReadOnly = true;
            show.ShowDialog();
        }

        private Dictionary<int, PaperRecord> records = new();

        private class Displayed {
            public int 序号 { get; set; } = 1;
            public string 论文名称 { get; set; } = "";
            public string 发表源 { get; set; } = "";
            public int 发表年份 { get; set; } = 2010;
            public string 类型 { get; set; } = "";
            public string 级别 { get; set; } = "";
        }

        private readonly List<Displayed> displayData = new();

        private void UpdateDisplayData() {
            displayData.Clear();
            foreach (var (key, value) in records) {
                displayData.Add(new Displayed {
                    序号 = key,
                    论文名称 = value.name,
                    发表源 = value.source,
                    发表年份 = value.year,
                    类型 = ItemTranslation.PaperType[value.type],
                    级别 = ItemTranslation.PaperLevel[value.level],
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
                records = await Database.SearchPaperWithRequirement(req);
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
