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
    public static class AddItemShopList
    {
        [FunctionName("AddItemShopList")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var responseMessage = new
            {
                Status = "Processing",
                Message = "INSERT RESULT:"
            };

            string userIDString = req.Query["user_ID"];
            string itemIDString = req.Query["item_ID"];
            string itemNumString = req.Query["item_num"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userIDString = userIDString ?? data?.user_ID;
            itemIDString = itemIDString ?? data?.item_ID;
            itemNumString = itemNumString ?? data?.item_num;

            if (int.TryParse(userIDString, out int userID) && int.TryParse(itemIDString, out int itemID)
                && int.TryParse(itemNumString, out int itemNum))
            {
                try
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = "m3hsaitojimotiesdb.database.windows.net";
                    builder.UserID = "sqladmin";
                    builder.Password = "Jimoties5";
                    builder.InitialCatalog = "m3h-saito-jimotiesDB";

                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        // 在庫数をチェックするSQLクエリ
                        string stockCheckSql = @"
                            SELECT item_stock FROM ItemTable WHERE item_ID = @ItemID;
                        ";

                        using (SqlCommand checkCommand = new SqlCommand(stockCheckSql, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@ItemID", itemID);
                            connection.Open();

                            int itemStock = (int)await checkCommand.ExecuteScalarAsync();

                            // 在庫数がカートに追加する数より少ない場合、エラーメッセージを返す
                            if (itemNum > itemStock)
                            {
                                return new OkObjectResult(new
                                {
                                    Status = "Error",
                                    Message = $"在庫が不足しています。在庫数: {itemStock}"
                                });
                            }

                            // 在庫数が足りている場合、カートに追加または更新処理を行う
                            string sql = @"
                                IF EXISTS (SELECT 1 FROM ShoplistTable WHERE user_ID = @UserID AND item_ID = @ItemID)
                                BEGIN
                                    UPDATE ShoplistTable
                                    SET item_num = item_num + @ItemNum
                                    WHERE user_ID = @UserID AND item_ID = @ItemID;
                                END
                                ELSE
                                BEGIN
                                    INSERT INTO ShoplistTable (user_ID, item_ID, item_num) VALUES (@UserID, @ItemID, @ItemNum);
                                END";

                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@UserID", userID);
                                command.Parameters.AddWithValue("@ItemID", itemID);
                                command.Parameters.AddWithValue("@ItemNum", itemNum);

                                int result = await command.ExecuteNonQueryAsync();

                                responseMessage = new
                                {
                                    Status = result > 0 ? "Success" : "Info",
                                    Message = result > 0
                                        ? "カートに追加されました"
                                        : "カートの数量が更新されました"
                                };
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
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

            return new OkObjectResult(responseMessage);
        }
    }
}
