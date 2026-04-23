using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;//DB接続用ライブラリ
using Newtonsoft.Json;
using System.Data;

namespace jimoties
{
    public static class ListByUseridFunction
    {
        //購入テーブルから一覧表示
        [FunctionName("ListBuy")]
        public static async Task<IActionResult> GetBuy(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //レスポンス用文字列
            string responseMessage = "パラメータが正しくありません";

            //パラメーター取得（GETメソッド用）
            string userId = req.Query["User_ID"];


            //パラメーター取得（POSTメソッド用）
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userId = userId ?? data?.User_ID;

            try
            {
                //接続文字列の設定
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "m3hsaitojimotiesdb.database.windows.net";
                builder.UserID = "sqladmin";
                builder.Password = "Jimoties5";
                builder.InitialCatalog = "m3h-saito-jimotiesDB";

                //接続用オブジェクトの初期化
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                   

                    //実行するクエリ
                    String sql = "SELECT * FROM BuyTable WHERE user_ID=@UserId";

                    //SQL実行オブジェクトの初期化
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        log.LogInformation("Connecting to database...");
                        //DBと接続
                        connection.Open();

                        command.Parameters.AddWithValue("@UserID", userId);
                        //SQLを実行し、結果をオブジェクトに格納
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            //結果を格納するためのオブジェクトを初期化
                            BuyTableList resultList = new BuyTableList();
                            //結果を1行ずつ処理
                            while (reader.Read())
                            {
                                var buyDate = reader.GetDateTime(reader.GetOrdinal("buy_date"));
                                //オブジェクトに結果を格納
                                resultList.List.Add(new BuyTableRow {
                                    Buy_ID = reader.GetInt32("buy_ID"), 
                                    User_ID = reader.GetInt32("user_ID"), 
                                    Item_ID = reader.GetInt32("item_ID"), 
                                    Item_name = reader.GetString("item_name"), 
                                    Item_num = reader.GetInt32("item_num"), 
                                    Item_price = reader.GetInt32("item_price"),
                                    Buy_date = buyDate.ToString("yyyy年MM月dd日"), // 年月日のみを取得
                                    Item_image = reader.GetString("item_image"),
                                    Item_prefecture = reader.GetString("item_prefecture")
                                });
                            }
                            // 結果が空の場合の処理
                            if (resultList.List.Count == 0)
                            {
                                responseMessage = "購入履歴がありません";
                            }
                            else
                            {
                                // JSONオブジェクトを文字列に変換
                                responseMessage = JsonConvert.SerializeObject(resultList);
                            }
                        }
                    }
                }
            }
            //DB操作でエラーが発生した場合はここでキャッチ
            catch (SqlException e)
            {
                //エラーをコンソールに出力
                Console.WriteLine(e.ToString());
            }
            //結果文字列を返却
            return new OkObjectResult(responseMessage);
        }

        //お気に入りテーブルから一覧表示
        [FunctionName("ListFavorite")]
        public static async Task<IActionResult> GetFavorite(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //レスポンス用文字列
            string responseMessage = "パラメータが正しくありません";

            //パラメーター取得（GETメソッド用）
            string userId = req.Query["User_ID"];

            //パラメーター取得（POSTメソッド用）
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            userId = userId ?? data?.User_ID;

            try
            {
                //接続文字列の設定
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "m3hsaitojimotiesdb.database.windows.net";
                builder.UserID = "sqladmin";
                builder.Password = "Jimoties5";
                builder.InitialCatalog = "m3h-saito-jimotiesDB";

                //接続用オブジェクトの初期化
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    
                    //実行するクエリ
                    String sql = "SELECT * FROM FavoriteTable WHERE user_ID = @UserId";

                    //SQL実行オブジェクトの初期化
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        log.LogInformation("Connecting to database...");
                        //DBと接続
                        connection.Open();

                        command.Parameters.AddWithValue("@UserId", userId);
                        //SQLを実行し、結果をオブジェクトに格納
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //結果を格納するためのオブジェクトを初期化
                            FavoriteTableList resultList = new FavoriteTableList();
                            //結果を1行ずつ処理
                            while (reader.Read())
                            {
                                //オブジェクトに結果を格納
                                resultList.List.Add(new FavoriteTableRow { 
                                    Fa_ID = reader.GetInt32("fa_ID"), 
                                    User_ID = reader.GetInt32("user_ID"), 
                                    Item_ID = reader.GetInt32("item_ID") 
                                });
                            }
                            // 結果が空の場合の処理
                            if (resultList.List.Count == 0)
                            {
                                responseMessage = "お気に入り商品がありません";
                            }
                            else
                            {
                                // JSONオブジェクトを文字列に変換
                                responseMessage = JsonConvert.SerializeObject(resultList);
                            }
                        }
                    }
                }
            }
            //DB操作でエラーが発生した場合はここでキャッチ
            catch (SqlException e)
            {
                //エラーをコンソールに出力
                Console.WriteLine(e.ToString());
            }
            //結果文字列を返却
            return new OkObjectResult(responseMessage);
        }

        //カートテーブルから一覧表示
        [FunctionName("ListShoplist")]
        public static async Task<IActionResult> Getshoplist(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //レスポンス用文字列
            string responseMessage = "パラメータが正しくありません";

            try
            {
                //接続文字列の設定
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "m3hsaitojimotiesdb.database.windows.net";
                builder.UserID = "sqladmin";
                builder.Password = "Jimoties5";
                builder.InitialCatalog = "m3h-saito-jimotiesDB";

                //接続用オブジェクトの初期化
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    string userId = req.Query["User_ID"]; // クエリパラメータから取得
                    //実行するクエリ
                    String sql = "SELECT * FROM ShoplistTable WHERE user_ID = @UserId";

                    //SQL実行オブジェクトの初期化
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        log.LogInformation("Connecting to database...");
                        //DBと接続
                        connection.Open();

                        command.Parameters.AddWithValue("@UserId", userId);
                        //SQLを実行し、結果をオブジェクトに格納
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //結果を格納するためのオブジェクトを初期化
                            ShoplistTableList resultList = new ShoplistTableList();
                            //結果を1行ずつ処理
                            while (reader.Read())
                            {
                                //オブジェクトに結果を格納
                                resultList.List.Add(new ShoplistTableRow { Shop_ID = reader.GetInt32("shop_ID"), User_ID = reader.GetInt32("user_ID"), Item_ID = reader.GetInt32("item_ID"), Item_num = reader.GetInt32("item_num") });
                            }
                            // 結果が空の場合の処理
                            if (resultList.List.Count == 0)
                            {
                                responseMessage = "カートは空です";
                            }
                            else
                            {
                                // JSONオブジェクトを文字列に変換
                                responseMessage = JsonConvert.SerializeObject(resultList);
                            }
                        }
                    }
                }
            }
            //DB操作でエラーが発生した場合はここでキャッチ
            catch (SqlException e)
            {
                //エラーをコンソールに出力
                Console.WriteLine(e.ToString());
            }
            //結果文字列を返却
            return new OkObjectResult(responseMessage);
        }

    }
}
