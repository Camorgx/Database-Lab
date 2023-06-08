using MySqlConnector;
using System.Data;
using System.Threading.Tasks;

namespace Lab3 {
    static class Database {
        private static readonly string connectString = new MySqlConnectionStringBuilder {
            Server = "localhost",
            UserID = "root",
            Password = "2004",
            Database = "lab3"
        }.ConnectionString;
        private static readonly MySqlConnection connection = new(connectString);
        private static readonly MySqlConnection paperWriter = new(connectString);
        private static readonly MySqlConnection projectWriter = new(connectString);
        private static readonly MySqlConnection lessonWriter = new(connectString);

        public static async Task<bool> Activate() {
            var conOpener = connection.OpenAsync();
            var paperOpener = paperWriter.OpenAsync();
            var projectOpener = projectWriter.OpenAsync();
            var lessonOpener = lessonWriter.OpenAsync();
            await conOpener;
            await paperOpener;
            await projectOpener;
            await lessonOpener;
            return true;
        }

        public static async Task<bool> TryLogin(string id, string password) {
            using var command = connection.CreateCommand();
            command.CommandText = $"select password from TeacherAccount where teacherID = {id}";
            using var reader = await command.ExecuteReaderAsync();
            string correctPassword = "";
            await reader.ReadAsync();
            correctPassword = reader.GetString(0);
            return correctPassword == password;
        }

        public static async Task<int> TryRegister(string id, string password, string verification) {
            using var regCommand = connection.CreateCommand();
            regCommand.CommandText = "register";
            regCommand.CommandType = CommandType.StoredProcedure;
            var idParam = new MySqlParameter {
                Value = id,
                ParameterName = "ID",
                Direction = ParameterDirection.Input,
            };
            var pwdParam = new MySqlParameter {
                Value = password,
                ParameterName = "pwd",
                Direction = ParameterDirection.Input,
            };
            var verParam = new MySqlParameter {
                Value = verification,
                ParameterName = "verify",
                Direction = ParameterDirection.Input,
            };
            var statusParam = new MySqlParameter {
                ParameterName = "result",
                Direction = ParameterDirection.Output,
            };
            regCommand.Parameters.AddRange(new MySqlParameter[] {idParam,  pwdParam, verParam, statusParam});
            await regCommand.ExecuteNonQueryAsync();
            return (int)(statusParam.Value ?? 2);
        }

        public static async Task<int> TryResetPassword(string id, string password, string verification) {
            using var command = connection.CreateCommand();
            command.CommandText = "resetPassword";
            command.CommandType = CommandType.StoredProcedure;
            var idParam = new MySqlParameter {
                Value = id,
                ParameterName = "ID",
                Direction = ParameterDirection.Input,
            };
            var pwdParam = new MySqlParameter {
                Value = password,
                ParameterName = "pwd",
                Direction = ParameterDirection.Input,
            };
            var verParam = new MySqlParameter {
                Value = verification,
                ParameterName = "verify",
                Direction = ParameterDirection.Input,
            };
            var statusParam = new MySqlParameter {
                ParameterName = "result",
                Direction = ParameterDirection.Output,
            };
            command.Parameters.AddRange(new MySqlParameter[] { idParam, pwdParam, verParam, statusParam });
            await command.ExecuteNonQueryAsync();
            return (int)(statusParam.Value ?? 3);
        }

        public static async Task<bool> LoadTeacherData(string teacherID) {
            using var command = connection.CreateCommand();
            command.CommandText = $"select teacherName as name, gender, title " +
                                  $"from teacher where teacherID = '{teacherID}'";
            using var reader = await command.ExecuteReaderAsync();
            Global.teacher.ID = teacherID;
            await reader.ReadAsync();
            Global.teacher.Name = reader.GetString("name");
            Global.teacher.Title = reader.GetInt32("title");
            Global.teacher.Gender = reader.GetInt32("gender");
            return true;
        }

        public static async Task<bool> LoadPaperData(string teacherID) {
            Global.userPaper.Clear();
            using var command = paperWriter.CreateCommand();
            command.CommandText = $"select paper.paperID as paperID, paperName as name, paperSource as source, " +
                                  $"    paperYear as year, paperType as type, level, paperRank, corresponding " +
                                  $"from paper, publish " +
                                  $"where teacherID = '{teacherID}' and paper.paperID = publish.paperID";
            using var reader = await command.ExecuteReaderAsync();
            Global.teacher.ID = teacherID;
            while (await reader.ReadAsync()) {
                Global.userPaper.Add(new Paper {
                    序号 = reader.GetInt32("paperID"),
                    论文名称 = reader.GetString("name"),
                    发表源 = reader.GetString("source"),
                    发表年份 = reader.GetInt32("year"),
                    类型 = ItemTranslation.PaperType[reader.GetInt32("type")],
                    级别 = ItemTranslation.PaperLevel[reader.GetInt32("level")],
                    排名 = reader.GetInt32("paperRank"),
                    是否通讯作者 = ItemTranslation.Corresponding[reader.GetInt32("corresponding")]
                });
            }
            return true;
        }

        public static async Task<bool> LoadProjectData(string teacherID) {
            Global.userProject.Clear();
            using var command = projectWriter.CreateCommand();
            command.CommandText = $"select project.projectID as projectID, projectName as name, projectSource as source, " +
                                  $"    projectType as type, totalMoney, startYear, endYear, projectRank, money " +
                                  $"from project, undertake " +
                                  $"where teacherID = '{teacherID}' and project.projectID = undertake.projectID";
            using var reader = await command.ExecuteReaderAsync();
            Global.teacher.ID = teacherID;
            while (await reader.ReadAsync()) {
                Global.userProject.Add(new Project {
                    ID = reader.GetString("projectID"),
                    Name = reader.GetString("name"),
                    Source = reader.GetString("source"),
                    Type = ItemTranslation.ProjectType[reader.GetInt32("type")],
                    TotalMoney = reader.GetFloat("totalMoney"),
                    StartYear = reader.GetInt32("startYear"),
                    EndYear = reader.GetInt32("endYear"),
                    Rank = reader.GetInt32("projectRank"),
                    Money = reader.GetFloat("money")
                });
            }
            return true;
        }

        public static async Task<bool> LoadLessonData(string teacherID) {
            Global.userLesson.Clear();
            using var command = lessonWriter.CreateCommand();
            command.CommandText = $"select lesson.lessonID as lessonID, lessonName as name, lessonHour as totalHour, " +
                                  $"    lessonType as type, year, term, hour " +
                                  $"from lesson, teach " +
                                  $"where teacherID = '{teacherID}' and lesson.lessonID = teach.lessonID";
            using var reader = await command.ExecuteReaderAsync();
            Global.teacher.ID = teacherID;
            while (await reader.ReadAsync()) {
                Global.userLesson.Add(new Lesson {
                    ID = reader.GetString("lessonID"),
                    Name = reader.GetString("name"),
                    TotalHour = reader.GetInt32("totalHour"),
                    Type = ItemTranslation.LessonType[reader.GetInt32("type")],
                    Year = reader.GetInt32("year"),
                    Term = reader.GetInt32("term"),
                    Hour = reader.GetInt32("hour")
                });
            }
            return true;
        }

        public static async Task<bool> LoadUserData(string teacherID) {
            var loadTeacherData = LoadTeacherData(teacherID);
            var loadPaperData = LoadPaperData(teacherID);
            var loadProjectData = LoadProjectData(teacherID);
            var loadLessonData = LoadLessonData(teacherID);
            await loadTeacherData;
            await loadLessonData;
            await loadPaperData;
            await loadProjectData;
            return true;
        }

        public static async Task<bool> UpdateTeacherData(string id, string name, int gender, int title) {
            using var command = connection.CreateCommand();
            command.CommandText = $"update Teacher set teacherName = '{name}', gender = {gender}, title = {title} " +
                                  $"where teacherID = '{id}'";
            var res = await command.ExecuteNonQueryAsync();
            return res != 0;
        }

        public static async Task<int> UpdatePassword(string id, string oldPwd, string newPwd) {
            using var command = connection.CreateCommand();
            command.CommandText = "updatePassword";
            command.CommandType = CommandType.StoredProcedure;
            var idParam = new MySqlParameter {
                Value = id,
                ParameterName = "ID",
                Direction = ParameterDirection.Input,
            };
            var oldPwdParam = new MySqlParameter {
                Value = oldPwd,
                ParameterName = "oldPwd",
                Direction = ParameterDirection.Input,
            };
            var newPwdParam = new MySqlParameter {
                Value = newPwd,
                ParameterName = "newPwd",
                Direction = ParameterDirection.Input,
            };
            var statusParam = new MySqlParameter {
                ParameterName = "result",
                Direction = ParameterDirection.Output,
            };
            command.Parameters.AddRange(new MySqlParameter[] { idParam, oldPwdParam, newPwdParam, statusParam });
            await command.ExecuteNonQueryAsync();
            return (int)(statusParam.Value ?? 2);
        }

        public static async Task<int> RemovePaper(int id) {
            using var command = connection.CreateCommand();
            command.CommandText = "deletePaper";
            command.CommandType = CommandType.StoredProcedure;
            var idParam = new MySqlParameter {
                Value = id,
                ParameterName = "ID",
                Direction = ParameterDirection.Input,
            };
            var statusParam = new MySqlParameter {
                ParameterName = "result",
                Direction = ParameterDirection.Output,
            };
            command.Parameters.AddRange(new MySqlParameter[] { idParam, statusParam });
            await command.ExecuteNonQueryAsync();
            return (int)(statusParam.Value ?? 1);
        }
    }
}
