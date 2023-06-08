using MySqlConnector;
using System.Data;
using System.Threading.Tasks;

namespace Lab3 {
    static class Database {
        private static MySqlConnection connection = new MySqlConnection(
            new MySqlConnectionStringBuilder {
                Server = "localhost",
                UserID = "root",
                Password = "2004",
                Database = "lab3"
            }.ConnectionString);

        public static async Task<bool> Activate() {
            await connection.OpenAsync();
            return true;
        }

        public static async Task<bool> TryLogin(string id, string password) {
            using var command = connection.CreateCommand();
            command.CommandText = $"select password from TeacherAccount where teacherID = {id}";
            using var reader = await command.ExecuteReaderAsync();
            string correctPassword = "";
            while (await reader.ReadAsync()) {
                correctPassword = reader.GetString(0);
                break;
            }
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
            using var regCommand = connection.CreateCommand();
            regCommand.CommandText = "resetPassword";
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
            regCommand.Parameters.AddRange(new MySqlParameter[] { idParam, pwdParam, verParam, statusParam });
            await regCommand.ExecuteNonQueryAsync();
            return (int)(statusParam.Value ?? 3);
        }
    }
}
