using System.Collections.Generic;

namespace Lab3 {
    class Paper {
        public int 序号 { get; set; } = 1;
        public string 论文名称 { get; set; } = "";
        public string 发表源 { get; set; } = "";
        public int 发表年份 { get; set; } = 2010;
        public string 类型 { get; set; } = "";
        public string 级别 { get; set; } = "";
        public int 排名 { get; set; } = 1;
        public string 是否通讯作者 { get; set; } = "";

        public override string ToString() {
            return $"{论文名称}，{发表源}，{发表年份}，{类型}，排名第 {排名}" +
                (是否通讯作者 == "是" ? "，通讯作者" : "");
        }
    }

    class Project {
        public string 项目号 { get; set; } = "";
        public string 项目名称 { get; set; } = "";
        public string 项目来源 { get; set; } = "";
        public string 项目类型 { get; set; } = "";
        public float 总经费 { get; set; } = 0;
        public int 开始年份 { get; set; } = 2010;
        public int 结束年份 { get; set; } = 2020;
        public int 排名 { get; set; } = 1;
        public float 承担经费 { get; set; } = 0;

        public override string ToString() {
            return $"{项目名称}，{项目来源}，{项目类型}，{开始年份}-{结束年份}，" +
                   $"总经费：{总经费}，承担经费：{承担经费}";
        }
    }

    class Lesson {
        public string 课程号 { get; set; } = "";
        public string 课程名称 { get; set; } = "";
        public int 学时数 { get; set; } = 0;
        public string 课程性质 { get; set; } = "";
        public int 年份 { get; set; } = 2010;
        public string 学期 { get; set; } = "";
        public int 承担学时 { get; set; } = 0;

        public override string ToString() {
            return $"课程号：{课程号}\t课程名：{课程名称}\t" +
                   $"主讲学时：{承担学时}\t学期：{年份} {学期[0]}";
        }
    }

    class Teacher {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public int Gender { get; set; } = 1;
        public int Title { get; set; } = 1;

        public override string ToString() {
            return $"工号：{ID}\t姓名：{Name}\t性别：{ItemTranslation.Gender[Gender]}\t" +
                   $"职称：{ItemTranslation.TeacherType[Title]}";
        }
    }

    public class PaperRecord {
        public int id = 0;
        public string name = "";
        public string source = "";
        public int year = 2010;
        public int type = 0;
        public int level = 0;
        public List<(string id, string name, int cor)> authors = new();
    }

    public class ProjectRecord {
        public string id = "";
        public string name = "";
        public string source = "";
        public int type = 0;
        public float totalMoney;
        public int startYear = 2010;
        public int endYear = 2020;
        public List<(string id, string name, float money)> teachers = new();
    }

    public class LessonRecord {
        public string id = "";
        public string name = "";
        public int totalHour = 0;
        public int type = 0;
        public List<(string id, string name, int year, int term, int hour)> teachers = new();
    }

    static class Global {
        public static Teacher teacher = new();
        public static List<Paper> userPaper = new();
        public static List<Project> userProject = new();
        public static List<Lesson> userLesson = new();
        public static List<Paper> ownPaper = new();
        public static List<Paper> partedPaper = new();
        public static List<Project> ownProject = new();
        public static List<Project> partedProject = new();

        public static PaperRecord newPaper = new();
        public static ProjectRecord newProject = new();
        public static LessonRecord newLesson = new();

        public static List<Lesson> totalLesson = new();
        public static List<Paper> totalPaper = new();
        public static List<Project> totalProject = new();

        public static void Init() {
            teacher = new();
            userPaper.Clear();
            userProject.Clear();
            userLesson.Clear();
            ownPaper.Clear();
            partedPaper.Clear();
            ownProject.Clear();
            partedProject.Clear();

            newPaper = new();
            newProject = new();
            newLesson = new();

            totalLesson.Clear();
            totalPaper.Clear();
            totalProject.Clear();
    }
    }
}
