using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;

public static class UpdateUser
{
    [FunctionName("UpdateUser")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed an update request.");

        // HTTPレスポンスで返すオブジェクトを定義
        var responseMessage = new
        {
            Status = "Processing",
            Message = "UPDATE RESULT:"
        };

        // アップデート用のパラメーター取得（GETメソッド用）
        string userIDString = req.Query["user_ID"];
        string userName = req.Query["user_name"];
        string userPassword = req.Query["user_password"];
        string mail = req.Query["mail"];
        string address = req.Query["address"];
        string telephone = req.Query["telephone"];
        string email = req.Query["email"];

        // アップデート用のパラメーター取得（POSTメソッド用）
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        userIDString = userIDString ?? data?.user_ID;
        userName = userName ?? data?.user_name;
        userPassword = userPassword ?? data?.user_password;
        mail = mail ?? data?.mail;
        address = address ?? data?.address;
        telephone = telephone ?? data?.telephone;
        email = email ?? data?.email;

        // user_IDのチェック
        if (string.IsNullOrWhiteSpace(userIDString))
        {
            log.LogError("Invalid user_ID value");
            return new BadRequestObjectResult(new
            {
                Status = "Error",
                Message = "Invalid user_ID value"
            });
        }

        // 少なくとも1つのパラメーターがある場合のみ処理
        if (!string.IsNullOrWhiteSpace(userName) ||
            !string.IsNullOrWhiteSpace(userPassword) ||
            !string.IsNullOrWhiteSpace(mail) ||
            !string.IsNullOrWhiteSpace(address) ||
            !string.IsNullOrWhiteSpace(telephone) ||
            !string.IsNullOrWhiteSpace(email))
        {
            try
            {
                // DB接続設定（接続文字列の構築）
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = "m3hsaitojimotiesdb.database.windows.net",
                    UserID = "sqladmin",
                    Password = "Jimoties5",
                    InitialCatalog = "m3h-saito-jimotiesDB"
                };

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    // SQL文を組み立て（必要な項目のみ更新）
                    string sql = "UPDATE UserTable SET ";
                    if (!string.IsNullOrWhiteSpace(userName)) sql += "user_name = @UserName, ";
                    if (!string.IsNullOrWhiteSpace(userPassword)) sql += "user_password = @UserPassword, ";
                    if (!string.IsNullOrWhiteSpace(mail)) sql += "mail = @Mail, ";
                    if (!string.IsNullOrWhiteSpace(address)) sql += "address = @Address, ";
                    if (!string.IsNullOrWhiteSpace(telephone)) sql += "telephone = @Telephone, ";
                    if (!string.IsNullOrWhiteSpace(email)) sql += "email = @Email, ";
                    sql = sql.TrimEnd(',', ' '); // 最後のカンマを削除
                    sql += " WHERE user_ID = @UserID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // パラメーターを設定
                        command.Parameters.AddWithValue("@UserID", userIDString);
                        if (!string.IsNullOrWhiteSpace(userName)) command.Parameters.AddWithValue("@UserName", userName);
                        if (!string.IsNullOrWhiteSpace(userPassword)) command.Parameters.AddWithValue("@UserPassword", userPassword);
                        if (!string.IsNullOrWhiteSpace(mail)) command.Parameters.AddWithValue("@Mail", mail);
                        if (!string.IsNullOrWhiteSpace(address)) command.Parameters.AddWithValue("@Address", address);
                        if (!string.IsNullOrWhiteSpace(telephone)) command.Parameters.AddWithValue("@Telephone", telephone);
                        if (!string.IsNullOrWhiteSpace(email)) command.Parameters.AddWithValue("@Email", email);

                        // コネクションオープン
                        connection.Open();

                        // SQLコマンドを実行し結果行数を取得
                        int result = await command.ExecuteNonQueryAsync();

                        // 結果をレスポンスに格納
                        responseMessage = new
                        {
                            Status = result > 0 ? "Success" : "Failure",
                            Message = result > 0
                                ? $"{result} 行更新されました"
                                : "更新処理に失敗しました。"
                        };
                    }
                }
            }
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
                Status = "Warning",
                Message = "更新対象のパラメーターがありません"
            };
        }

        // HTTPレスポンスをJSON形式で返却
        return new OkObjectResult(responseMessage);
    }
}
