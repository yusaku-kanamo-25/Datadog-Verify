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
using Microsoft.VisualBasic;

namespace jimoties
{
    public static class ListFunction
    {
        //商品テーブルから商品名条件検索
        [FunctionName("SearchItemName")]
        public static async Task<IActionResult> SearchItemName(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //レスポンス用文字列
            string responseMessage = "パラメータが正しくありません";

            //パラメーター取得（GETメソッド用）
            string itemName = req.Query["Item_name"]; // クエリパラメータから取得

            //パラメーター取得（POSTメソッド用）
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            itemName = itemName ?? data?.Item_name;

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
                    String sql = "SELECT * FROM ItemTable WHERE item_name = @Itemname";

                    //SQL実行オブジェクトの初期化
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        log.LogInformation("Connecting to database...");
                        //DBと接続
                        connection.Open();

                        command.Parameters.AddWithValue("@Itemname", itemName);
                        //SQLを実行し、結果をオブジェクトに格納
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //結果を格納するためのオブジェクトを初期化
                            ItemTableList resultList = new ItemTableList();
                            //結果を1行ずつ処理
                            while (reader.Read())
                            {
                                //オブジェクトに結果を格納
                                resultList.List.Add(new ItemTableRow { 
                                    Item_ID = reader.GetInt32("item_ID"), 
                                    Item_name = reader.GetString("item_name"), 
                                    Item_category = reader.GetString("item_category"), 
                                    Item_prefecture = reader.GetString("item_prefecture"), 
                                    Item_price = reader.GetInt32("item_price"), 
                                    Item_stock = reader.GetInt32("item_stock"), 
                                    Item_image = reader.GetString("item_image") 
                                });
                            }
                            // 結果が空の場合の処理
                            if (resultList.List.Count == 0)
                            {
                                responseMessage = "一致する商品がありません";
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



        //商品テーブルから商品名条件検索
        [FunctionName("SearchItemId")]
        public static async Task<IActionResult> SearchItemId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //レスポンス用文字列
            string responseMessage = "パラメータが正しくありません";

            //パラメーター取得（GETメソッド用）
            string itemid = req.Query["Item_ID"];

            //パラメーター取得（POSTメソッド用）
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            itemid = itemid ?? data?.Item_ID;

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
                    String sql = "SELECT * FROM ItemTable WHERE item_ID = @Itemid";

                    //SQL実行オブジェクトの初期化
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        log.LogInformation("Connecting to database...");
                        //DBと接続
                        connection.Open();

                        command.Parameters.AddWithValue("@Itemid", itemid);
                        //SQLを実行し、結果をオブジェクトに格納
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //結果を格納するためのオブジェクトを初期化
                            ItemTableList resultList = new ItemTableList();
                            //結果を1行ずつ処理
                            while (reader.Read())
                            {
                                //オブジェクトに結果を格納
                                resultList.List.Add(new ItemTableRow {
                                    Item_ID = reader.GetInt32("item_ID"),
                                    Item_name = reader.GetString("item_name"), 
                                    Item_category = reader.GetString("item_category"),
                                    Item_prefecture = reader.GetString("item_prefecture"), 
                                    Item_price = reader.GetInt32("item_price"), 
                                    Item_stock = reader.GetInt32("item_stock"), 
                                    Item_image = reader.GetString("item_image") 
                                });
                            }
                            // 結果が空の場合の処理
                            if (resultList.List.Count == 0)
                            {
                                responseMessage = "一致する商品がありません";
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


        //商品テーブルから商品カテゴリー条件検索
        [FunctionName("SearchItemCategory")]
        public static async Task<IActionResult> SearchItemCategory(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //レスポンス用文字列
            string responseMessage = "パラメータが正しくありません";

            //パラメーター取得（GETメソッド用）
            string itemCategory = req.Query["Item_category"];

            //パラメーター取得（POSTメソッド用）
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            itemCategory = itemCategory ?? data?.Item_category;

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
                    String sql = "SELECT * FROM ItemTable WHERE item_category = @Itemcategory";

                    //SQL実行オブジェクトの初期化
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        log.LogInformation("Connecting to database...");
                        //DBと接続
                        connection.Open();

                        command.Parameters.AddWithValue("@Itemcategory", itemCategory);
                        //SQLを実行し、結果をオブジェクトに格納
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //結果を格納するためのオブジェクトを初期化
                            ItemTableList resultList = new ItemTableList();
                            //結果を1行ずつ処理
                            while (reader.Read())
                            {
                                //オブジェクトに結果を格納
                                resultList.List.Add(new ItemTableRow { 
                                    Item_ID = reader.GetInt32("item_ID"), 
                                    Item_name = reader.GetString("item_name"), 
                                    Item_category = reader.GetString("item_category"),
                                    Item_prefecture = reader.GetString("item_prefecture"), 
                                    Item_price = reader.GetInt32("item_price"), 
                                    Item_stock = reader.GetInt32("item_stock"), 
                                    Item_image = reader.GetString("item_image")
                                });
                            }
                            // 結果が空の場合の処理
                            if (resultList.List.Count == 0)
                            {
                                responseMessage = "正しいカテゴリーで検索してください";
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


        //商品テーブルから都道府県名条件検索
        [FunctionName("SearchItemPrefecture")]
        public static async Task<IActionResult> SearchItemPrefecture(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //レスポンス用文字列
            string responseMessage = "パラメータが正しくありません";

            //パラメーター取得（GETメソッド用）
            string itemPrefecture = req.Query["Item_prefecture"]; 

            //パラメーター取得（POSTメソッド用）
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            itemPrefecture = itemPrefecture ?? data?.Item_prefecture;

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
                    String sql = "SELECT * FROM ItemTable WHERE item_prefecture = @Itemprefecture";

                    //SQL実行オブジェクトの初期化
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        log.LogInformation("Connecting to database...");
                        //DBと接続
                        connection.Open();

                        command.Parameters.AddWithValue("@Itemprefecture", itemPrefecture);
                        //SQLを実行し、結果をオブジェクトに格納
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //結果を格納するためのオブジェクトを初期化
                            ItemTableList resultList = new ItemTableList();
                            //結果を1行ずつ処理
                            while (reader.Read())
                            {
                                //オブジェクトに結果を格納
                                resultList.List.Add(new ItemTableRow { 
                                    Item_ID = reader.GetInt32("item_ID"),
                                    Item_name = reader.GetString("item_name"), 
                                    Item_category = reader.GetString("item_category"),
                                    Item_prefecture = reader.GetString("item_prefecture"), 
                                    Item_price = reader.GetInt32("item_price"), 
                                    Item_stock = reader.GetInt32("item_stock"), 
                                    Item_image = reader.GetString("item_image")
                                });
                            }

                            // 結果が空の場合の処理
                            if (resultList.List.Count == 0)
                            {
                                responseMessage = "現在、商品がありません";
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


        //購入テーブルから一覧表示
        [FunctionName("SearchBuyId")]
        public static async Task<IActionResult> SearchBuyId(
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
                    String sql = "SELECT * FROM BuyTable WHERE user_ID = @UserId";

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
                                    Buy_date = buyDate.ToString("yyyy-MM-dd"), 
                                    Item_image = reader.GetString("item_image"), 
                                    Item_prefecture = reader.GetString("item_prefecture") 
                                });
                            }
                            // 結果が空の場合の処理
                            if (resultList.List.Count == 0)
                            {
                                responseMessage = "一致する購入履歴がありません";
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
