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

public static class AddItemBuy
{
    [FunctionName("AddItemBuy")]
    public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processing a purchase request.");

        string userIDString = req.Query["user_ID"];
        string itemIDString = req.Query["item_ID"];

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        userIDString = userIDString ?? data?.user_ID;
        itemIDString = itemIDString ?? data?.item_ID;

        if (int.TryParse(userIDString, out int userID) && int.TryParse(itemIDString, out int itemID))
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = "m3hsaitojimotiesdb.database.windows.net",
                    UserID = "sqladmin",
                    Password = "Jimoties5",
                    InitialCatalog = "m3h-saito-jimotiesDB"
                };

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    string stockCheckSql = @"
                    SELECT it.item_stock, sl.item_num
                    FROM ShoplistTable sl
                    JOIN ItemTable it ON sl.item_ID = it.item_ID
                    WHERE sl.user_ID = @UserID AND it.item_ID = @ItemID;
                ";

                    using (SqlCommand checkCommand = new SqlCommand(stockCheckSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@UserID", userID);
                        checkCommand.Parameters.AddWithValue("@ItemID", itemID);

                        connection.Open();
                        using (SqlDataReader reader = await checkCommand.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int itemStock = reader.GetInt32(0);
                                int itemNum = reader.GetInt32(1);

                                if (itemNum > itemStock)
                                {
                                    return new BadRequestObjectResult(new
                                    {
                                        Status = "Error",
                                        Message = "在庫数が不足しています。"
                                    });
                                }
                            }
                            else
                            {
                                return new BadRequestObjectResult(new
                                {
                                    Status = "Error",
                                    Message = "商品が見つかりません。"
                                });
                            }
                        }
                    }

                    // 購入処理のSQL
                    string sql = @"
                    INSERT INTO BuyTable (user_ID, item_ID, item_name, item_num, item_price, buy_date, item_image, item_prefecture)
                    SELECT sl.user_ID, it.item_ID, it.item_name, sl.item_num, it.item_price, GETDATE(), it.item_image, it.item_prefecture
                    FROM ShoplistTable sl
                    JOIN ItemTable it ON sl.item_ID = it.item_ID
                    WHERE sl.user_ID = @UserID;

                    UPDATE ItemTable
                    SET item_stock = item_stock - sl.item_num
                    FROM ShoplistTable sl
                    WHERE ItemTable.item_ID = sl.item_ID AND sl.user_ID = @UserID;

                    DELETE FROM ShoplistTable WHERE user_ID = @UserID;
                ";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);

                        int result = await command.ExecuteNonQueryAsync();

                        if (result > 0)
                        {
                            return new OkObjectResult(new
                            {
                                Status = "Success",
                                Message = "購入が確定されました。"
                            });
                        }
                        else
                        {
                            return new BadRequestObjectResult(new
                            {
                                Status = "Error",
                                Message = "購入処理に失敗しました。"
                            });
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                log.LogError(e.ToString());
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        else
        {
            return new BadRequestObjectResult(new
            {
                Status = "Error",
                Message = "無効なパラメーターが指定されました"
            });
        }
    }

}

