using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;

namespace jimoties
{
    public static class LoginFunction
    {
        [FunctionName("EmployeeLogin")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string employeeID, employeePassword;

            //通信による場合分け
            if (req.Method == HttpMethods.Post)
            {
                // POSTリクエストからボディを読み取る
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<EmployeeLoginRequest>(requestBody);
                employeeID = data.Employee_ID;
                employeePassword = data.Employee_password;
            }
            else if (req.Method == HttpMethods.Get)
            {
                // GETリクエストからクエリパラメータを取得する
                employeeID = req.Query["Employee_ID"];
                employeePassword = req.Query["Employee_password"];
            }
            else
            {
                return new BadRequestObjectResult(new { error = "許可されていない通信です" });
            }

            // SQL Server への接続を開く
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "m3hsaitojimotiesdb.database.windows.net",
                UserID = "sqladmin",
                Password = "Jimoties5",
                InitialCatalog = "m3h-saito-jimotiesDB"
            };

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                //非同期型で開く
                await connection.OpenAsync();

                var query = "SELECT * FROM EmployeeTable WHERE employee_ID = @Employee_ID AND employee_password = @Employee_password";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Employee_ID", employeeID);
                    cmd.Parameters.AddWithValue("@Employee_password", employeePassword);

                    var reader = await cmd.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        return new OkObjectResult(new { message = "ログイン成功", employeeID = employeeID });
                    }
                    else
                    {
                        return new UnauthorizedObjectResult(new { error = "ログイン失敗" });
                    }
                }
            }
        }

        [FunctionName("UserLogin")]
        public static async Task<IActionResult> UserLogin(
         [HttpTrigger(AuthorizationLevel.Anonymous, "post", "get", Route = null)] HttpRequest req,
         ILogger log)
        {
            string userEmail, userPassword;

            //通信による場合分け
            if (req.Method == HttpMethods.Post)
            {
                // POSTリクエストからボディを読み取る
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<UserLoginRequest>(requestBody);
                userEmail = data.Email;
                userPassword = data.User_password;
            }
            else if (req.Method == HttpMethods.Get)
            {
                // GETリクエストからクエリパラメータを取得する
                userEmail = req.Query["Email"];
                userPassword = req.Query["User_password"];
            }
            else
            {
                return new BadRequestObjectResult(new { error = "許可されていない通信です" });
            }

            // SQL Server への接続を開く
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "m3hsaitojimotiesdb.database.windows.net",
                UserID = "sqladmin",
                Password = "Jimoties5",
                InitialCatalog = "m3h-saito-jimotiesDB"
            };

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                //非同期型で開く
                await connection.OpenAsync();

                //とる情報がややこしいので書き込み型
                var query = @"
                SELECT user_ID, user_name, user_password, mail, address, telephone, email 
                FROM UserTable 
                WHERE email = @User_email AND user_password = @User_password";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@User_email", userEmail);
                    cmd.Parameters.AddWithValue("@User_password", userPassword);

                    var reader = await cmd.ExecuteReaderAsync();

                    // 結果を格納するためのオブジェクトを初期化
                    var resultList = new UserTableList();

                    // 結果を1行ずつ処理
                    while (reader.Read())
                    {
                        // オブジェクトに結果を格納
                        resultList.List.Add(new UserTableRow
                        {
                            User_ID = reader.GetInt32(reader.GetOrdinal("user_ID")),
                            User_name = reader.GetString(reader.GetOrdinal("user_name")),
                            User_password = reader.GetString(reader.GetOrdinal("user_password")),
                            Mail = reader.GetString(reader.GetOrdinal("mail")),
                            Address = reader.GetString(reader.GetOrdinal("address")),
                            Telephone = reader.GetString(reader.GetOrdinal("telephone")),
                            Email = reader.GetString(reader.GetOrdinal("email"))
                        });
                    }

                    // レコードが存在すれば、結果を返す
                    if (resultList.List.Count > 0)
                    {
                        return new OkObjectResult(new { message = "ログイン成功", users = resultList.List });
                    }
                    else
                    {
                        return new UnauthorizedObjectResult(new { error = "ログイン失敗" });
                    }
                }
            }
        }
    }
}