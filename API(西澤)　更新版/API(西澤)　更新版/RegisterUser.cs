using System;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace jimoties
{
    public static class RegisterUser
    {
        [FunctionName("RegisterUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // HTTPレスポンスで返すオブジェクトを定義
            var responseMessage = new
            {
                Status = "Processing",
                Message = "INSERT RESULT:",
            };

            // インサート用のパラメーター取得（GETメソッド用）
            string userName = req.Query["user_name"];
            string userPassword = req.Query["user_password"];
            string mail = req.Query["mail"];
            string address = req.Query["address"];
            string telephone = req.Query["telephone"];
            string email = req.Query["email"];

            // インサート用のパラメーター取得（POSTメソッド用）
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userName = userName ?? data?.user_name;
            userPassword = userPassword ?? data?.user_password;
            mail = mail ?? data?.mail;
            address = address ?? data?.address;
            telephone = telephone ?? data?.telephone;
            email = email ?? data?.email;

            // 全てのパラメーターを取得できた場合のみ処理
            if (!string.IsNullOrWhiteSpace(userName) &&
                !string.IsNullOrWhiteSpace(userPassword) &&
                !string.IsNullOrWhiteSpace(mail) &&
                !string.IsNullOrWhiteSpace(address) &&
                !string.IsNullOrWhiteSpace(telephone) &&
                !string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    // DB接続設定（接続文字列の構築）
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = "m3hsaitojimotiesdb.database.windows.net";
                    builder.UserID = "sqladmin";
                    builder.Password = "Jimoties5";
                    builder.InitialCatalog = "m3h-saito-jimotiesDB";

                    // SQLコネクションを初期化
                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        // 実行するSQL（パラメーター付き）
                        string sql = @"
                            INSERT INTO UserTable (user_name, user_password, mail, address, telephone, email)
                            VALUES (@UserName, @UserPassword, @Mail, @Address, @Telephone, @Email)";

                        // SQLコマンドを初期化
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // パラメーターを設定
                            command.Parameters.AddWithValue("@UserName", userName);
                            command.Parameters.AddWithValue("@UserPassword", userPassword);
                            command.Parameters.AddWithValue("@Mail", mail);
                            command.Parameters.AddWithValue("@Address", address);
                            command.Parameters.AddWithValue("@Telephone", telephone);
                            command.Parameters.AddWithValue("@Email", email);

                            // コネクションオープン（＝ SQLDatabaseに接続）
                            connection.Open();

                            // SQLコマンドを実行し結果行数を取得
                            int result = await command.ExecuteNonQueryAsync();

                            // 結果をレスポンスに格納
                            responseMessage = new
                            {
                                Status = "Success",
                                Message = $"{result}行挿入されました"
                            };
                        }
                    }
                }
                // DB処理でエラーが発生した場合
                catch (SqlException e)
                {
                    // コンソールにエラーを出力
                    log.LogError(e.ToString());
                    responseMessage = new
                    {
                        Status = "Error",
                        Message = "エラーが発生しました: " + e.Message
                    };
                }
            }
            else
            {
                responseMessage = new
                {
                    Status = "Error",
                    Message = "必要なパラメーターが設定されていません"
                };
            }

            // HTTPレスポンスをJSON形式で返却
            return new OkObjectResult(responseMessage);
        }
    }
}
