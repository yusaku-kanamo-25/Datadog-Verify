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
    public static class ListFunctition
    {
        //商品テーブルから一覧表示
        [FunctionName("ListItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //レスポンス用文字列
            string responseMessage = "SQL RESULT:";

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
                    String sql = "SELECT * FROM ItemTable";

                    //SQL実行オブジェクトの初期化
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        log.LogInformation("Connecting to database...");
                        //DBと接続
                        connection.Open();

                        //SQLを実行し、結果をオブジェクトに格納
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //結果を格納するためのオブジェクトを初期化
                            ItemTableList resultList = new ItemTableList();
                            //結果を1行ずつ処理
                            while (reader.Read())
                            {
                                //オブジェクトに結果を格納
                                resultList.List.Add(new ItemTableRow { Item_ID = reader.GetInt32("item_ID"), Item_name = reader.GetString("item_name"), Item_category = reader.GetString("item_category"), Item_prefecture = reader.GetString("item_prefecture"), Item_price = reader.GetInt32("item_price"), Item_stock = reader.GetInt32("item_stock"), Item_image = reader.GetString("item_image") });
                            }
                            //JSONオブジェクトを文字列に変換
                            responseMessage = JsonConvert.SerializeObject(resultList);
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
