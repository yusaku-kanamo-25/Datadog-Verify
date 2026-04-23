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

namespace jimoties
{
    public static class DeleteItem
    {
        [FunctionName("DeleteItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a delete item request.");

            // HTTPレスポンスで返すオブジェクトを定義
            var responseMessage = new
            {
                Status = "Processing",
                Message = "DELETE ITEM RESULT:"
            };

            // 削除用のパラメーター取得（GETメソッド用）
            string itemIDString = req.Query["item_ID"];

            // 削除用のパラメーター取得（POSTメソッド用）
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            itemIDString = itemIDString ?? data?.item_ID;

            // item_IDのチェック
            if (!int.TryParse(itemIDString, out int itemID))
            {
                log.LogError("Invalid item_ID value");
                return new BadRequestObjectResult(new
                {
                    Status = "Error",
                    Message = "Invalid item_ID value"
                });
            }

            // データベース接続設定
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "m3hsaitojimotiesdb.database.windows.net",
                UserID = "sqladmin",
                Password = "Jimoties5",
                InitialCatalog = "m3h-saito-jimotiesDB"
            };

            try
            {
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    // コネクションオープン
                    connection.Open();

                    // 商品が存在するかチェック
                    string checkQuery = @"
                        SELECT COUNT(*) FROM ItemTable 
                        WHERE item_ID = @ItemId";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@ItemId", itemID);

                        int count = (int)await checkCmd.ExecuteScalarAsync();

                        if (count == 0)
                        {
                            return new NotFoundObjectResult(new
                            {
                                Status = "Not Found",
                                Message = "Item not found."
                            });
                        }
                    }

                    // 商品を削除
                    string deleteQuery = @"
                        DELETE FROM ItemTable
                        WHERE item_ID = @ItemId";

                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCmd.Parameters.AddWithValue("@ItemId", itemID);

                        int rowsAffected = await deleteCmd.ExecuteNonQueryAsync();

                        // 結果をレスポンスに格納
                        responseMessage = new
                        {
                            Status = rowsAffected > 0 ? "Success" : "Failure",
                            Message = rowsAffected > 0
                                ? $"{rowsAffected} 行削除されました"
                                : "削除処理に失敗しました。"
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

            // HTTPレスポンスをJSON形式で返却
            return new OkObjectResult(responseMessage);
        }
    }
}
