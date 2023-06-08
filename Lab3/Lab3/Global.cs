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
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public string Source { get; set; } = "";
        public string Type { get; set; } = "";
        public float TotalMoney { get; set; } = 0;
        public int StartYear { get; set; } = 2010;
        public int EndYear { get; set; } = 2020;
        public int Rank { get; set; } = 1;
        public float Money { get; set; } = 0;
    }

    class Lesson {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public int TotalHour { get; set; } = 0;
        public string Type { get; set; } = "";
        public int Year { get; set; } = 2010;
        public int Term { get; set; } = 1;
        public int Hour { get; set; } = 0;
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

    static class Global {
        public static Teacher teacher = new();
        public static List<Paper> userPaper = new();
        public static List<Project> userProject = new();
        public static List<Lesson> userLesson = new();
        public static List<Paper> ownPaper = new();
        public static List<Paper> partedPaper = new();
    }
}
