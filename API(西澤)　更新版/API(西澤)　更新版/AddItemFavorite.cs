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

namespace Jimoties
{
    public static class AddItemFavorites
    {
        [FunctionName("AddItemFavorites")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // HTTPレスポンスで返すオブジェクトを定義
            var responseMessage = new
            {
                Status = "Processing",
                Message = "INSERT RESULT:"
            };

            // インサート用のパラメーター取得（GETメソッド用）
            string userIDString = req.Query["user_ID"];
            string itemIDString = req.Query["Item_ID"];

            // インサート用のパラメーター取得（POSTメソッド用）
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userIDString = userIDString ?? data?.user_ID;
            itemIDString = itemIDString ?? data?.Item_ID;

            // 文字列からint型に変換
            if (int.TryParse(userIDString, out int userID) && int.TryParse(itemIDString, out int itemID))
            {
                try
                {
                    // DB接続設定（接続文字列の構築）
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = "m3hsaitojimotiesdb.database.windows.net";  // Azure SQL Database のサーバー名
                    builder.UserID = "sqladmin";                                    // ユーザー名
                    builder.Password = "Jimoties5";                                  // パスワード
                    builder.InitialCatalog = "m3h-saito-jimotiesDB";                 // データベース名

                    // SQLコネクションを初期化
                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        // 実行するSQL（パラメーター付き）
                        string sql = @"
                            IF NOT EXISTS (SELECT 1 FROM FavoriteTable WHERE user_ID = @UserID AND item_ID = @ItemID)
                            BEGIN
                                INSERT INTO FavoriteTable (user_ID, item_ID) VALUES (@UserID, @ItemID);
                            END";

                        // SQLコマンドを初期化
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            // パラメーターを設定
                            command.Parameters.AddWithValue("@UserID", userID);
                            command.Parameters.AddWithValue("@ItemID", itemID);

                            // コネクションオープン（＝ SQLDatabaseに接続）
                            connection.Open();

                            // SQLコマンドを実行し結果行数を取得
                            int result = await command.ExecuteNonQueryAsync();

                            // 結果をレスポンスに格納
                            responseMessage = new
                            {
                                Status = result > 0 ? "Success" : "Info",
                                Message = result > 0
                                    ? "お気に入りに追加されました"
                                    : "既にお気に入りに登録されています"
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
                    Message = "無効なパラメーターが指定されました"
                };
            }

            // HTTPレスポンスをJSON形式で返却
            return new OkObjectResult(responseMessage);
        }
    }
}
