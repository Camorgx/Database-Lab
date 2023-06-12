using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lab3 {
    /// <summary>
    /// SearchTeacher.xaml 的交互逻辑
    /// </summary>
    public partial class SearchTeacher : UserControl {
        public SearchTeacher() {
            InitializeComponent();
        }
        
        public Operation? Owner { get; set; } = null;

        public string DialogIdentifier { get; set; } = "OperationDialog";

        private class Displayed {
            public string 工号 { get; set; } = "";
            public string 姓名 { get; set; } = "";
            public string 性别 { get; set; } = "";
            public string 职称 { get; set; } = "";
        }

        private readonly List<Displayed> displayData = new();
        private List<Teacher> records = new(); 

        private string GenerateReqString() {
            IList<string> arguments = new List<string>();
            string initString = "select teacherID, teacherName, gender, title from teacher ";
            if (teacherID.Text.Length > 0) arguments.Add($"teacherID = {teacherID.Text}");
            if (teacherName.Text.Length > 0) arguments.Add($"teacherName = '{teacherName.Text}'");
            if (gender.SelectedIndex > 0) arguments.Add($"gender = {gender.SelectedIndex}");
            if (title.SelectedIndex > 0) arguments.Add($"title = {title.SelectedIndex}");
            if (arguments.Count == 0) return initString;
            var sb = new StringBuilder(initString + "where ");
            for (int i = 0; i < arguments.Count; i++) {
                sb.Append(arguments[i]);
                if (i < arguments.Count - 1) sb.Append(" and ");
            }
            return sb.ToString();
        }

        private void UpdateDisplayData() {
            displayData.Clear();
            foreach (var value in records) {
                displayData.Add(new Displayed {
                    工号 = value.ID,
                    姓名 = value.Name,
                    性别 = ItemTranslation.Gender[value.Gender],
                    职称 = ItemTranslation.TeacherType[value.Title],
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
            string req = GenerateReqString();
            records = await Database.SearchTeacherWithRequirement(req);
            UpdateDisplayData();
            searchResult.ItemsSource = displayData;
            searchResult.Items.Refresh();
            window.Close();
            Owner?.Activate();
        }
    }
}
