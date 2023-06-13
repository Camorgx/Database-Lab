using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lab3 {
    static class Utils {
        public static async Task<bool> MessageTips(string message, string identifier) {
            var sampleMessageDialog = new MessageDialog {
                Message = { Text = message }
            };
            await DialogHost.Show(sampleMessageDialog, identifier);
            return true;
        }

        public static async Task<bool> VerificationDialog(string message, string identifier) {
            var verDialog = new VerifyDialog {
                Message = { Text = message }
            };
            var res = await DialogHost.Show(verDialog, identifier) ?? "false";
            return bool.Parse((string)res);
        }

        public static bool VerifyPassword(string password) {
            if (password.Length < 8 || password.Length > 20) return false;
            bool findAlpha = false, findNumber = false;
            foreach (char ch in password) {
                if (char.IsDigit(ch)) findNumber = true;
                if (char.IsLetter(ch)) findAlpha = true;
            }
            if (!findNumber || !findAlpha) return false;
            return true;
        }

        public static void UpdatePaperView() {
            Global.ownPaper.Clear();
            Global.partedPaper.Clear();
            foreach (var paper in Global.userPaper) {
                if (paper.排名 == 1)
                    Global.ownPaper.Add(paper);
                else Global.partedPaper.Add(paper);
            }
        }

        public static bool CompareAuthorList(PaperRecord a, PaperRecord b) {
            if (a.authors.Count != b.authors.Count) return false;
            for (int i = 0; i < a.authors.Count; i++) {
                if (a.authors[i].id != b.authors[i].id
                    || a.authors[i].cor != b.authors[i].cor) return false;
            }
            return true;
        }

        public static bool ComparePaperAttr(PaperRecord a, PaperRecord b) {
            if (a.id != b.id) return false;
            if (a.type != b.type) return false;
            if (a.year != b.year) return false;
            if (a.name != b.name) return false;
            if (a.source != b.source) return false;
            if (a.level != b.level) return false;
            return true;
        }

        public static bool ComparePaperRecord(PaperRecord a, PaperRecord b) {
            return ComparePaperAttr(a, b) && CompareAuthorList(a, b);
        }

        public static int VerifyPaperAuthors(PaperRecord record) {
            bool metCor = false;
            ISet<string> authors = new HashSet<string>();
            foreach (var (id, _, cor) in record.authors) {
                if (id.Length != 5) return 1;
                if (cor != 0) {
                    if (metCor) return 2;
                    metCor = true;
                }
                if (authors.Contains(id)) return 3;
                else authors.Add(id);
            }
            return 0;
        }

        public static void UpdateProjectView() {
            Global.ownProject.Clear();
            Global.partedProject.Clear();
            foreach (var project in Global.userProject) {
                if (project.排名 == 1)
                    Global.ownProject.Add(project);
                else Global.partedProject.Add(project);
            }
        }

        public static bool CompareTeacherList(ProjectRecord a, ProjectRecord b) {
            if (a.teachers.Count != b.teachers.Count) return false;
            for (int i = 0; i < a.teachers.Count; i++) {
                if (a.teachers[i].id != b.teachers[i].id
                    || a.teachers[i].money != b.teachers[i].money) return false;
            }
            return true;
        }

        public static bool CompareProjectAttr(ProjectRecord a, ProjectRecord b) {
            if (a.id != b.id) return false;
            if (a.name != b.name) return false;
            if (a.source != b.source) return false;
            if (a.type != b.type) return false;
            if (a.totalMoney != b.totalMoney) return false;
            if (a.startYear != b.startYear) return false;
            if (a.endYear != b.endYear) return false;
            return true;
        }

        public static bool CompareProjectRecord(ProjectRecord a, ProjectRecord b) {
            return CompareProjectAttr(a, b) && CompareTeacherList(a, b);
        }

        public static int VerifyProjectTeachers(ProjectRecord record) {
            float total = 0;
            ISet<string> authors = new HashSet<string>();
            foreach (var (id, _, money) in record.teachers) {
                if (id.Length != 5) return 1;
                if (authors.Contains(id)) return 3;
                else authors.Add(id);
                total += money;
            }
            return Math.Abs(record.totalMoney - total) < 0.01 ? 0 : 2;
        }

        public static bool CompareTeacherList(LessonRecord a, LessonRecord b) {
            if (a.teachers.Count != b.teachers.Count) return false;
            for (int i = 0; i < a.teachers.Count; i++) {
                if (a.teachers[i].id != b.teachers[i].id) return false;
                if (a.teachers[i].year != b.teachers[i].year) return false;
                if (a.teachers[i].term != b.teachers[i].term) return false;
                if (a.teachers[i].hour != b.teachers[i].hour) return false;
            }
            return true;
        }

        public static bool CompareLessonAttr(LessonRecord a, LessonRecord b) {
            if (a.id != b.id) return false;
            if (a.name != b.name) return false;
            if (a.totalHour != b.totalHour) return false;
            if (a.type != b.type) return false;
            return true;
        }

        public static bool CompareLessonRecord(LessonRecord a, LessonRecord b) {
            return CompareLessonAttr(a, b) && CompareTeacherList(a, b);
        }

        public static int VerifyLessonTeachers(LessonRecord record) {
            Dictionary<(int year, int term), int> table = new();
            HashSet<(string id, int year, int term)> authors = new();
            foreach (var (id, _, year, term, hour) in record.teachers) {
                if (id.Length != 5) return 1;
                if (term == 0) return 4;
                var testTuple = (id, year, term);
                if (authors.Contains(testTuple)) return 3;
                else authors.Add(testTuple);
                if (table.ContainsKey((year, term))) table[(year, term)] += hour;
                else table[(year, term)] = hour;
            }
            foreach (int total in table.Values) 
                if (total != record.totalHour) return 2;
            return 0;
        }

        public static async Task<bool> UpdateTotal(int startYear, int endYear) {
            string teacherID = Global.teacher.ID;
            var updateLesson = Database.LoadLessonData(teacherID);
            var updateProject = Database.LoadProjectData(teacherID);
            var updatePaper = Database.LoadPaperData(teacherID);
            Global.totalLesson.Clear();
            Global.totalProject.Clear();
            Global.totalPaper.Clear();
            await updateLesson;
            await updateProject;
            await updatePaper;
            if (endYear == 0) endYear = int.MaxValue;
            foreach (var item in Global.userLesson) {
                if (item.年份 >= startYear && item.年份 <= endYear) 
                    Global.totalLesson.Add(item);
            }
            foreach (var item in Global.userPaper) {
                if (item.发表年份 >= startYear && item.发表年份 <= endYear)
                    Global.totalPaper.Add(item);
            }
            foreach (var item in Global.userProject) {
                if (item.开始年份 >= startYear && item.结束年份 <= endYear)
                    Global.totalProject.Add(item);
            }
            return true;
        }

        public static string GenerateTotalMDString(int startYear, int endYear) {
            var builder = new StringBuilder();
            string start = startYear == 0 ? "起始" : startYear.ToString();
            string end = endYear == 0 ? "至今" : endYear.ToString();
            builder.AppendLine($"# 教师教学科研工作统计（{start}-{end}）");
            builder.AppendLine("## 教师基本信息");
            builder.AppendLine(Global.teacher.ToString() + '\n');
            builder.AppendLine("## 教学情况");
            foreach (var item in Global.totalLesson) builder.AppendLine(item.ToString() + '\n');
            builder.AppendLine("## 发表论文情况");
            foreach (var item in Global.totalPaper) builder.AppendLine(item.ToString() + '\n');
            builder.AppendLine("## 承担项目情况");
            foreach (var item in Global.totalProject) builder.AppendLine(item.ToString() + '\n');
            return builder.ToString();
        }
    }
}
