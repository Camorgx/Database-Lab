using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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

        public static int VerifyReaderAuthors(PaperRecord record) {
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
    }
}
