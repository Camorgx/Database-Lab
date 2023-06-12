using MySqlConnector;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lab3 {
    static class Database {
        private static readonly string connectString = new MySqlConnectionStringBuilder {
            Server = ConfigurationManager.AppSettings["DatabaseAddress"],
            UserID = ConfigurationManager.AppSettings["DatabaseUID"],
            Password = ConfigurationManager.AppSettings["DatabasePassword"],
            Database = ConfigurationManager.AppSettings["DatabaseName"]
        }.ConnectionString;
        private static readonly MySqlConnection connection = new(connectString);

        public static async Task<bool> Activate() {
            await connection.OpenAsync();
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
            using var paperWriter = new MySqlConnection(connectString);
            await paperWriter.OpenAsync();
            using var command = paperWriter.CreateCommand();
            command.CommandText = $"select paper.paperID as paperID, paperName as name, paperSource as source, " +
                                  $"    paperYear as year, paperType as type, level, paperRank, corresponding " +
                                  $"from paper, publish " +
                                  $"where teacherID = '{teacherID}' and paper.paperID = publish.paperID";
            using var reader = await command.ExecuteReaderAsync();
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
            using var projectWriter = new MySqlConnection(connectString);
            await projectWriter.OpenAsync();
            using var command = projectWriter.CreateCommand();
            command.CommandText = $"select project.projectID as projectID, projectName as name, projectSource as source, " +
                                  $"    projectType as type, totalMoney, startYear, endYear, projectRank, money " +
                                  $"from project, undertake " +
                                  $"where teacherID = '{teacherID}' and project.projectID = undertake.projectID";
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) {
                Global.userProject.Add(new Project {
                    项目号 = reader.GetString("projectID"),
                    项目名称 = reader.GetString("name"),
                    项目来源 = reader.GetString("source"),
                    项目类型 = ItemTranslation.ProjectType[reader.GetInt32("type")],
                    总经费 = reader.GetFloat("totalMoney"),
                    开始年份 = reader.GetInt32("startYear"),
                    结束年份 = reader.GetInt32("endYear"),
                    排名 = reader.GetInt32("projectRank"),
                    承担金额 = reader.GetFloat("money")
                });
            }
            return true;
        }

        public static async Task<bool> LoadLessonData(string teacherID) {
            Global.userLesson.Clear();
            using var lessonWriter = new MySqlConnection(connectString);
            await lessonWriter.OpenAsync();
            using var command = lessonWriter.CreateCommand();
            command.CommandText = $"select lesson.lessonID as lessonID, lessonName as name, lessonHour as totalHour, " +
                                  $"    lessonType as type, year, term, hour " +
                                  $"from lesson, teach " +
                                  $"where teacherID = '{teacherID}' and lesson.lessonID = teach.lessonID";
            using var reader = await command.ExecuteReaderAsync();
            Global.teacher.ID = teacherID;
            while (await reader.ReadAsync()) {
                Global.userLesson.Add(new Lesson {
                    课程号 = reader.GetString("lessonID"),
                    课程名称 = reader.GetString("name"),
                    学时数 = reader.GetInt32("totalHour"),
                    课程性质 = ItemTranslation.LessonType[reader.GetInt32("type")],
                    年份 = reader.GetInt32("year"),
                    学期 = reader.GetInt32("term"),
                    承担学时 = reader.GetInt32("hour")
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

        public static async Task<PaperRecord> SearchPaper(int id, MySqlTransaction? trans = null) {
            PaperRecord res;
            using var command = connection.CreateCommand();
            var transaction = trans ?? await connection.BeginTransactionAsync();
            command.Transaction = transaction;
            command.CommandText = $"select paperID as id, paperName as name, paperSource as source, " +
                                  $"    paperYear as year, paperType as type, level " +
                                  $"from paper where paperID = {id}";
            using (var reader = await command.ExecuteReaderAsync()) {
                await reader.ReadAsync();
                res = new PaperRecord {
                    id = id,
                    name = reader.GetString("name"),
                    source = reader.GetString("source"),
                    year = reader.GetInt32("year"),
                    type = reader.GetInt32("type"),
                    level = reader.GetInt32("level")
                };
            }
            command.CommandText = $"select publish.teacherID as teacherID, teacherName as name, corresponding as cor " +
                                  $"    from publish, teacher " +
                                  $"where paperID = {id} and publish.teacherID = teacher.teacherID " +
                                  $"order by paperRank";
            using (var reader = await command.ExecuteReaderAsync()) {
                while (await reader.ReadAsync()) {
                    res.authors.Add((reader.GetString("teacherID"), reader.GetString("name"), reader.GetInt32("cor")));
                }
            }
            if (trans is null) await transaction.CommitAsync();
            return res;
        }

        public enum UpdateMode {
            All,
            AttrOnly,
            TeacherOnly,
            None
        }

        public static async Task<string> VerifiPaperAuthors(PaperRecord paper) {
            using var command = connection.CreateCommand();
            var transaction = await connection.BeginTransactionAsync();
            command.Transaction = transaction;
            for (int i = 0; i < paper.authors.Count; i++) {
                var (id, _, _) = paper.authors[i];
                command.CommandText =
                    $"select exists(select teacherID from Teacher where teacherID = '{id}')";
                using var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();
                if (reader.GetInt32(0) == 0) {
                    await transaction.CommitAsync();
                    return "#" + id;
                }
            }
            await transaction.CommitAsync();
            return "ok";
        }

        public static async Task<bool> UpdatePaper(PaperRecord paper, UpdateMode mode,
            MySqlTransaction? trans = null) {
            if (mode == UpdateMode.None) return true;
            var transaction = trans ?? await connection.BeginTransactionAsync();
            try {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                if (mode == UpdateMode.AttrOnly || mode == UpdateMode.All) {
                    command.CommandText =
                        $"update Paper set " +
                        $"  paperName = '{paper.name}', " +
                        $"  paperSource = '{paper.source}', " +
                        $"  paperYear = {paper.year}, " +
                        $"  paperType = {paper.type}, " +
                        $"  level = {paper.level} " +
                        $"where paperID = {paper.id}";
                    await command.ExecuteNonQueryAsync();
                }
                if (mode == UpdateMode.TeacherOnly || mode == UpdateMode.All) {
                    command.CommandText = $"delete from Publish where paperID = {paper.id}";
                    await command.ExecuteNonQueryAsync();
                    for (int i = 0; i < paper.authors.Count; i++) {
                        var (id, _, cor) = paper.authors[i];
                        command.CommandText =
                            $"insert into Publish value ('{id}', {paper.id}, {i + 1}, {cor != 0})";
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException) {
                await transaction.RollbackAsync();
                return false;
            }
            if (trans is null) await transaction.CommitAsync();
            return true;
        }

        public static async Task<bool> CheckExistsOfPaperID(int id) {
            using var command = connection.CreateCommand();
            command.CommandText =
                $"select exists(select paperID from paper where paperID = {id})";
            using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            return reader.GetInt32(0) != 0;
        }

        public static async Task<bool> AddPaper(PaperRecord paper) {
            var transaction = await connection.BeginTransactionAsync();
            try {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    $"insert into Paper value ({paper.id}, '{paper.name}', '{paper.source}', " +
                    $"  {paper.year}, {paper.type}, {paper.level})";
                await command.ExecuteNonQueryAsync();
                if (!await UpdatePaper(paper, UpdateMode.TeacherOnly, transaction))
                    return false;
            }
            catch (MySqlException) {
                await transaction.RollbackAsync();
                return false;
            }
            await transaction.CommitAsync();
            return true;
        }

        public static async Task<int> RemoveProject(string id) {
            using var command = connection.CreateCommand();
            command.CommandText = "deleteProject";
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

        public static async Task<ProjectRecord> SearchProject(string id, MySqlTransaction? trans = null) {
            ProjectRecord res;
            using var command = connection.CreateCommand();
            var transaction = trans ?? await connection.BeginTransactionAsync();
            command.Transaction = transaction;
            command.CommandText = $"select projectID as id, projectName as name, projectSource as source, " +
                                  $"    projectType as type, totalMoney, startYear, endYear " +
                                  $"from project where projectID = '{id}'";
            using (var reader = await command.ExecuteReaderAsync()) {
                await reader.ReadAsync();
                res = new ProjectRecord {
                    id = id,
                    name = reader.GetString("name"),
                    source = reader.GetString("source"),
                    type = reader.GetInt32("type"),
                    totalMoney = reader.GetFloat("totalMoney"),
                    startYear = reader.GetInt32("startYear"),
                    endYear = reader.GetInt32("endYear")
                };
            }
            command.CommandText = $"select undertake.teacherID as teacherID, teacherName as name, money " +
                                  $"    from undertake, teacher " +
                                  $"where projectID = '{id}' and undertake.teacherID = teacher.teacherID " +
                                  $"order by projectRank";
            using (var reader = await command.ExecuteReaderAsync()) {
                while (await reader.ReadAsync()) {
                    res.teachers.Add((reader.GetString("teacherID"), reader.GetString("name"), reader.GetFloat("money")));
                }
            }
            if (trans is null) await transaction.CommitAsync();
            return res;
        }

        public static async Task<string> VerifiProjectTeachers(ProjectRecord project) {
            using var command = connection.CreateCommand();
            var transaction = await connection.BeginTransactionAsync();
            command.Transaction = transaction;
            for (int i = 0; i < project.teachers.Count; i++) {
                var (id, _, _) = project.teachers[i];
                command.CommandText =
                    $"select exists(select teacherID from Teacher where teacherID = '{id}')";
                using var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();
                if (reader.GetInt32(0) == 0) {
                    await transaction.CommitAsync();
                    return "#" + id;
                }
            }
            await transaction.CommitAsync();
            return "ok";
        }

        public static async Task<bool> UpdateProject(ProjectRecord project, UpdateMode mode,
            MySqlTransaction? trans = null) {
            if (mode == UpdateMode.None) return true;
            var transaction = trans ?? await connection.BeginTransactionAsync();
            try {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                if (mode == UpdateMode.AttrOnly || mode == UpdateMode.All) {
                    command.CommandText =
                        $"update Project set " +
                        $"  projectName = '{project.name}', " +
                        $"  projectSource = '{project.source}', " +
                        $"  projectType = {project.type}, " +
                        $"  totalMoney = {project.totalMoney}, " +
                        $"  startYear = {project.startYear}, " +
                        $"  endYear = {project.endYear} " +
                        $"where projectID = '{project.id}'";
                    await command.ExecuteNonQueryAsync();
                }
                if (mode == UpdateMode.TeacherOnly || mode == UpdateMode.All) {
                    command.CommandText = $"delete from Undertake where projectID = '{project.id}'";
                    await command.ExecuteNonQueryAsync();
                    for (int i = 0; i < project.teachers.Count; i++) {
                        var (id, _, money) = project.teachers[i];
                        command.CommandText =
                            $"insert into Undertake value ('{id}', '{project.id}', {i + 1}, {money})";
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException) {
                await transaction.RollbackAsync();
                return false;
            }
            if (trans is null) await transaction.CommitAsync();
            return true;
        }

        public static async Task<bool> CheckExistsOfProjectID(string id) {
            using var command = connection.CreateCommand();
            command.CommandText =
                $"select exists(select projectID from Project where projectID = '{id}')";
            using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            return reader.GetInt32(0) != 0;
        }

        public static async Task<bool> AddProject(ProjectRecord project) {
            var transaction = await connection.BeginTransactionAsync();
            try {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    $"insert into Project value ('{project.id}', '{project.name}', '{project.source}', " +
                    $"  {project.type}, {project.totalMoney}, {project.startYear}, {project.endYear})";
                await command.ExecuteNonQueryAsync();
                if (!await UpdateProject(project, UpdateMode.TeacherOnly, transaction))
                    return false;
            }
            catch (MySqlException) {
                await transaction.RollbackAsync();
                return false;
            }
            await transaction.CommitAsync();
            return true;
        }

        public static async Task<int> RemoveLesson(string id) {
            using var command = connection.CreateCommand();
            command.CommandText = "deleteLesson";
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

        public static async Task<LessonRecord> SearchLesson(string id, MySqlTransaction? trans = null) {
            LessonRecord res;
            using var command = connection.CreateCommand();
            var transaction = trans ?? await connection.BeginTransactionAsync();
            command.Transaction = transaction;
            command.CommandText = $"select lessonID as id, lessonName as name, lessonHour as hour, " +
                                  $"    lessonType as type " +
                                  $"from lesson where lessonID = '{id}'";
            using (var reader = await command.ExecuteReaderAsync()) {
                await reader.ReadAsync();
                res = new LessonRecord {
                    id = id,
                    name = reader.GetString("name"),
                    totalHour = reader.GetInt32("hour"),
                    type = reader.GetInt32("type")
                };
            }
            command.CommandText = $"select teach.teacherID as teacherID, teacherName as name, year, term, hour " +
                                  $"    from teach, teacher " +
                                  $"where lessonID = '{id}' and teach.teacherID = teacher.teacherID " +
                                  $"order by year";
            using (var reader = await command.ExecuteReaderAsync()) {
                while (await reader.ReadAsync()) {
                    res.teachers.Add((reader.GetString("teacherID"), reader.GetString("name"), 
                        reader.GetInt32("year"), reader.GetInt32("term"), reader.GetInt32("hour")));
                }
            }
            if (trans is null) await transaction.CommitAsync();
            return res;
        }

        public static async Task<string> VerifiLessonTeachers(LessonRecord lesson) {
            using var command = connection.CreateCommand();
            var transaction = await connection.BeginTransactionAsync();
            command.Transaction = transaction;
            for (int i = 0; i < lesson.teachers.Count; i++) {
                var (id, _, _, _, _) = lesson.teachers[i];
                command.CommandText =
                    $"select exists(select teacherID from Teacher where teacherID = '{id}')";
                using var reader = await command.ExecuteReaderAsync();
                await reader.ReadAsync();
                if (reader.GetInt32(0) == 0) {
                    await transaction.CommitAsync();
                    return "#" + id;
                }
            }
            await transaction.CommitAsync();
            return "ok";
        }

        public static async Task<bool> UpdateLesson(LessonRecord lesson, UpdateMode mode,
            MySqlTransaction? trans = null) {
            if (mode == UpdateMode.None) return true;
            var transaction = trans ?? await connection.BeginTransactionAsync();
            try {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                if (mode == UpdateMode.AttrOnly || mode == UpdateMode.All) {
                    command.CommandText =
                        $"update Lesson set " +
                        $"  lessonName = '{lesson.name}', " +
                        $"  lessonHour = {lesson.totalHour}, " +
                        $"  lessonType = {lesson.type} " +
                        $"where lessonID = '{lesson.id}'";
                    await command.ExecuteNonQueryAsync();
                }
                if (mode == UpdateMode.TeacherOnly || mode == UpdateMode.All) {
                    command.CommandText = $"delete from Teach where lessonID = '{lesson.id}'";
                    await command.ExecuteNonQueryAsync();
                    for (int i = 0; i < lesson.teachers.Count; i++) {
                        var (id, _, year, term, hour) = lesson.teachers[i];
                        command.CommandText =
                            $"insert into Teach value ('{id}', '{lesson.id}', {year}, {term}, {hour})";
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (MySqlException) {
                await transaction.RollbackAsync();
                return false;
            }
            if (trans is null) await transaction.CommitAsync();
            return true;
        }

        public static async Task<bool> CheckExistsOfLessonID(string id) {
            using var command = connection.CreateCommand();
            command.CommandText =
                $"select exists(select lessonID from Lesson where lessonID = '{id}')";
            using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            return reader.GetInt32(0) != 0;
        }

        public static async Task<bool> AddLesson(LessonRecord lesson) {
            var transaction = await connection.BeginTransactionAsync();
            try {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText =
                    $"insert into Lesson value ('{lesson.id}', '{lesson.name}', {lesson.totalHour}, {lesson.type})";
                await command.ExecuteNonQueryAsync();
                if (!await UpdateLesson(lesson, UpdateMode.TeacherOnly, transaction))
                    return false;
            }
            catch (MySqlException) {
                await transaction.RollbackAsync();
                return false;
            }
            await transaction.CommitAsync();
            return true;
        }

        public static async Task<Dictionary<int, PaperRecord>> SearchPaperWithRequirement(string reqString) {
            var transaction = await connection.BeginTransactionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = reqString;
            command.Transaction = transaction;
            var reader = await command.ExecuteReaderAsync();
            Dictionary<int, PaperRecord> res = new();
            IList<int> ids = new List<int>();
            while (await reader.ReadAsync()) 
                ids.Add(reader.GetInt32(0));
            await reader.DisposeAsync();
            foreach (int id in ids)
                res[id] = await SearchPaper(id, transaction);
            await transaction.CommitAsync();
            return res;
        }

        public static async Task<List<Teacher>> SearchTeacherWithRequirement(string reqString) {
            using var command = connection.CreateCommand();
            command.CommandText = reqString;
            using var reader = await command.ExecuteReaderAsync();
            List<Teacher> res = new();
            while (await reader.ReadAsync()) {
                res.Add(new Teacher {
                    ID = reader.GetString(0),
                    Name = reader.GetString(1),
                    Gender = reader.GetInt32(2),
                    Title = reader.GetInt32(3),
                });
            }
            return res;
        }

        public static async Task<Dictionary<string, ProjectRecord>> SearchProjectWithRequirement(string reqString) {
            var transaction = await connection.BeginTransactionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = reqString;
            command.Transaction = transaction;
            var reader = await command.ExecuteReaderAsync();
            Dictionary<string, ProjectRecord> res = new();
            IList<string> ids = new List<string>();
            while (await reader.ReadAsync())
                ids.Add(reader.GetString(0));
            await reader.DisposeAsync();
            foreach (string id in ids)
                res[id] = await SearchProject(id, transaction);
            await transaction.CommitAsync();
            return res;
        }
    }
}
