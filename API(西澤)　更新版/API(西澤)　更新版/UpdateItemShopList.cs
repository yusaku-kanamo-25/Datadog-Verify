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

public static class UpdateItemShopList
{
    [FunctionName("UpdateItemShopList")]
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
        string itemIDString = req.Query["item_ID"];
        string itemNumString = req.Query["item_num"];

        // アップデート用のパラメーター取得（POSTメソッド用）
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        userIDString = userIDString ?? data?.user_ID;
        itemIDString = itemIDString ?? data?.item_ID;
        itemNumString = itemNumString ?? data?.item_num;

        // パラメーターのチェック
        if (!int.TryParse(userIDString, out int userID))
        {
            log.LogError("Invalid user_ID value");
            return new BadRequestObjectResult(new
            {
                Status = "Error",
                Message = "Invalid user_ID value"
            });
        }
        if (!int.TryParse(itemIDString, out int itemID))
        {
            log.LogError("Invalid item_ID value");
            return new BadRequestObjectResult(new
            {
                Status = "Error",
                Message = "Invalid item_ID value"
            });
        }
        if (!int.TryParse(itemNumString, out int itemNum) || itemNum <= 0)
        {
            log.LogError("Invalid item_num value");
            return new BadRequestObjectResult(new
            {
                Status = "Error",
                Message = "Invalid item_num value"
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

                // 商品がカートに存在するかチェック
                string checkQuery = @"
                    SELECT COUNT(*) FROM ShoplistTable 
                    WHERE user_ID = @UserId AND item_ID = @ItemId";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@UserId", userID);
                    checkCmd.Parameters.AddWithValue("@ItemId", itemID);

                    int count = (int)await checkCmd.ExecuteScalarAsync();

                    if (count == 0)
                    {
                        return new NotFoundObjectResult(new
                        {
                            Status = "NotFound",
                            Message = "Item not found in cart."
                        });
                    }
                }

                // 商品数を更新
                string updateQuery = @"
                    UPDATE ShoplistTable
                    SET item_num = @ItemNum
                    WHERE user_ID = @UserId AND item_ID = @ItemId";

                using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection))
                {
                    updateCmd.Parameters.AddWithValue("@UserId", userID);
                    updateCmd.Parameters.AddWithValue("@ItemId", itemID);
                    updateCmd.Parameters.AddWithValue("@ItemNum", itemNum);

                    int rowsAffected = await updateCmd.ExecuteNonQueryAsync();

                    // 結果をレスポンスに格納
                    responseMessage = new
                    {
                        Status = rowsAffected > 0 ? "Success" : "Failure",
                        Message = rowsAffected > 0
                            ? $"{rowsAffected} 行更新されました"
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

        // HTTPレスポンスをJSON形式で返却
        return new OkObjectResult(responseMessage);
    }
}
