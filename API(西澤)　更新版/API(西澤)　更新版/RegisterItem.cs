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

public static class RegisterItem
{
    [FunctionName("RegisterItem")]
    public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        // レスポンスメッセージをJSON形式にする
        var responseMessage = new
        {
            Status = "Success",
            Message = "INSERT RESULT:"
        };

        // インサート用のパラメーター取得（GETメソッド用）
        string itemName = req.Query["item_name"];
        string itemCategory = req.Query["item_category"];
        string itemPrefecture = req.Query["item_prefecture"];
        string itemPriceString = req.Query["item_price"];
        string itemStockString = req.Query["item_stock"];
        string itemImage = req.Query["item_image"];

        // インサート用のパラメーター取得（POSTメソッド用）
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        itemName = itemName ?? data?.item_name;
        itemCategory = itemCategory ?? data?.item_category;
        itemPrefecture = itemPrefecture ?? data?.item_prefecture;
        itemPriceString = itemPriceString ?? data?.item_price;
        itemStockString = itemStockString ?? data?.item_stock;
        itemImage = itemImage ?? data?.item_image;

        // 数値データの変換
        if (!int.TryParse(itemPriceString, out int itemPrice))
        {
            log.LogError("Invalid item_price value");
            responseMessage = new
            {
                Status = "Error",
                Message = "Invalid item_price value"
            };
            return new BadRequestObjectResult(responseMessage);
        }
        if (!int.TryParse(itemStockString, out int itemStock))
        {
            log.LogError("Invalid item_stock value");
            responseMessage = new
            {
                Status = "Error",
                Message = "Invalid item_stock value"
            };
            return new BadRequestObjectResult(responseMessage);
        }

        // 全てのパラメーターを取得できた場合のみ処理
        if (!string.IsNullOrWhiteSpace(itemName) &&
            !string.IsNullOrWhiteSpace(itemCategory) &&
            !string.IsNullOrWhiteSpace(itemPrefecture) &&
            itemPrice > 0 &&
            itemStock > 0 &&
            !string.IsNullOrWhiteSpace(itemImage))
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

                // SQLコネクションを初期化
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    // 実行するSQL（パラメーター付き）
                    string sql = @"
                    INSERT INTO ItemTable (item_name, item_category, item_prefecture, item_price, item_stock, item_image)
                    VALUES (@ItemName, @ItemCategory, @ItemPrefecture, @ItemPrice, @ItemStock, @ItemImage)";

                    // SQLコマンドを初期化
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // パラメーターを設定
                        command.Parameters.AddWithValue("@ItemName", itemName);
                        command.Parameters.AddWithValue("@ItemCategory", itemCategory);
                        command.Parameters.AddWithValue("@ItemPrefecture", itemPrefecture);
                        command.Parameters.AddWithValue("@ItemPrice", itemPrice);
                        command.Parameters.AddWithValue("@ItemStock", itemStock);
                        command.Parameters.AddWithValue("@ItemImage", itemImage);

                        // コネクションオープン（＝ SQLDatabaseに接続）
                        connection.Open();

                        // SQLコマンドを実行し結果行数を取得
                        int result = await command.ExecuteNonQueryAsync();

                        // 結果をレスポンスに格納
                        responseMessage = new
                        {
                            Status = "Success",
                            Message = $"{result}商品登録されました"
                        };
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
                Message = "必要なパラメーターが設定されていません"
            };
        }

        return new OkObjectResult(responseMessage);
    }
}
