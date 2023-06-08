using System.Collections.Generic;

namespace Lab3 {
    class Paper {
        public int ID { get; set; } = 1;
        public string Name { get; set; } = "";
        public string Source { get; set; } = "";
        public int Year { get; set; } = 2010;
        public int Type { get; set; } = 1;
        public int Level { get; set; } = 1;
        public int Rank { get; set; } = 1;
        public int Corresponding { get; set; } = 0;
    }

    class Project {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public string Source { get; set; } = "";
        public int Type { get; set; } = 1;
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
        public int Type { get; set; } = 1;
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

    static class Global {
        public static Teacher teacher = new();
        public static IList<Paper> userPaper = new List<Paper>();
        public static IList<Project> userProject = new List<Project>();
        public static IList<Lesson> userLesson = new List<Lesson>();
    }
}
