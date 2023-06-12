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
        public float 承担金额 { get; set; } = 0;
    }

    class Lesson {
        public string 课程号 { get; set; } = "";
        public string 课程名称 { get; set; } = "";
        public int 学时数 { get; set; } = 0;
        public string 课程性质 { get; set; } = "";
        public int 年份 { get; set; } = 2010;
        public int 学期 { get; set; } = 1;
        public int 承担学时 { get; set; } = 0;
    }

    class Teacher {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public int Gender { get; set; } = 1;
        public int Title { get; set; } = 1;
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

        public static List<Teacher> resultTeacher = new();
        public static List<Paper> reaultPaper = new();
        public static List<Project> resultProject = new();
        public static List<Lesson> resultLesson = new();
    }
}
