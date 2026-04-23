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

public static class UpdateItem
{
    [FunctionName("UpdateItem")]
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
        string itemIDString = req.Query["Item_ID"];
        string itemName = req.Query["Item_name"];
        string itemCategory = req.Query["Item_category"];
        string itemPrefecture = req.Query["Item_prefecture"];
        string itemPriceString = req.Query["Item_price"];
        string itemStockString = req.Query["Item_stock"];
        string itemImage = req.Query["Item_image"];

        // アップデート用のパラメーター取得（POSTメソッド用）
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        itemIDString = itemIDString ?? data?.Item_ID;
        itemName = itemName ?? data?.Item_name;
        itemCategory = itemCategory ?? data?.Item_category;
        itemPrefecture = itemPrefecture ?? data?.Item_prefecture;
        itemPriceString = itemPriceString ?? data?.Item_price;
        itemStockString = itemStockString ?? data?.Item_stock;
        itemImage = itemImage ?? data?.Item_image;

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

        // 数値データの変換
        int itemPrice = 0, itemStock = 0;
        if (!string.IsNullOrWhiteSpace(itemPriceString) && !int.TryParse(itemPriceString, out itemPrice))
        {
            log.LogError("Invalid item_price value");
            return new BadRequestObjectResult(new
            {
                Status = "Error",
                Message = "Invalid item_price value"
            });
        }
        if (!string.IsNullOrWhiteSpace(itemStockString) && !int.TryParse(itemStockString, out itemStock))
        {
            log.LogError("Invalid item_stock value");
            return new BadRequestObjectResult(new
            {
                Status = "Error",
                Message = "Invalid item_stock value"
            });
        }

        // 少なくとも1つのパラメーターがある場合のみ処理
        if (!string.IsNullOrWhiteSpace(itemName) ||
            !string.IsNullOrWhiteSpace(itemCategory) ||
            !string.IsNullOrWhiteSpace(itemPrefecture) ||
            itemPrice > 0 ||
            itemStock > 0 ||
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

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    // SQL文を組み立て（必要な項目のみ更新）
                    string sql = "UPDATE ItemTable SET ";
                    if (!string.IsNullOrWhiteSpace(itemName)) sql += "item_name = @ItemName, ";
                    if (!string.IsNullOrWhiteSpace(itemCategory)) sql += "item_category = @ItemCategory, ";
                    if (!string.IsNullOrWhiteSpace(itemPrefecture)) sql += "item_prefecture = @ItemPrefecture, ";
                    if (itemPrice > 0) sql += "item_price = @ItemPrice, ";
                    if (itemStock > 0) sql += "item_stock = @ItemStock, ";
                    if (!string.IsNullOrWhiteSpace(itemImage)) sql += "item_image = @ItemImage, ";
                    sql = sql.TrimEnd(',', ' '); // 最後のカンマを削除
                    sql += " WHERE item_ID = @ItemID";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // パラメーターを設定
                        command.Parameters.AddWithValue("@ItemID", itemID);
                        if (!string.IsNullOrWhiteSpace(itemName)) command.Parameters.AddWithValue("@ItemName", itemName);
                        if (!string.IsNullOrWhiteSpace(itemCategory)) command.Parameters.AddWithValue("@ItemCategory", itemCategory);
                        if (!string.IsNullOrWhiteSpace(itemPrefecture)) command.Parameters.AddWithValue("@ItemPrefecture", itemPrefecture);
                        if (itemPrice > 0) command.Parameters.AddWithValue("@ItemPrice", itemPrice);
                        if (itemStock > 0) command.Parameters.AddWithValue("@ItemStock", itemStock);
                        if (!string.IsNullOrWhiteSpace(itemImage)) command.Parameters.AddWithValue("@ItemImage", itemImage);

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