using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jimoties
{
    // 商品テーブル
    [JsonObject]
    public class ItemTableList
    {
        [JsonProperty("ItemTableList")]
        public List<ItemTableRow> List { get; set; } = new List<ItemTableRow>();
    }

    [JsonObject]
    public class ItemTableRow
    {
        [JsonProperty("item_ID")]
        public int Item_ID { get; set; }

        [JsonProperty("item_name")]
        public string Item_name { get; set; }

        [JsonProperty("item_category")]
        public string Item_category { get; set; }

        [JsonProperty("item_prefecture")]
        public string Item_prefecture { get; set; }

        [JsonProperty("item_price")]
        public int Item_price { get; set; }

        [JsonProperty("item_stock")]
        public int Item_stock { get; set; }

        [JsonProperty("item_image")]
        public string Item_image { get; set; }
    }


    //購入テーブル
    [JsonObject]
    public class BuyTableList
    {
        [JsonProperty("BuyTableList")]
        public List<BuyTableRow> List { get; set; } = new List<BuyTableRow>();
    }

    [JsonObject]
    public class BuyTableRow
    {
        [JsonProperty("buy_ID")]
        public int Buy_ID { get; set; }

        [JsonProperty("user_ID")]
        public int User_ID { get; set; }

        [JsonProperty("item_ID")]
        public int Item_ID { get; set; }

        [JsonProperty("item_name")]
        public string Item_name { get; set; }

        [JsonProperty("item_num")]
        public int Item_num { get; set; }

        [JsonProperty("item_price")]
        public int Item_price { get; set; }

        [JsonProperty("buy_date")]
        public string Buy_date { get; set; }

        [JsonProperty("item_image")]
        public string Item_image { get; set; }

        [JsonProperty("item_prefecture")]
        public string Item_prefecture { get; set; }
    }


    //お気に入りテーブル
    [JsonObject]
    public class FavoriteTableList
    {
        [JsonProperty("FavoriteTableList")]
        public List<FavoriteTableRow> List { get; set; } = new List<FavoriteTableRow>();
    }

    [JsonObject]
    public class FavoriteTableRow
    {
        [JsonProperty("Fa_ID")]
        public int fa_ID { get; set; }

        [JsonProperty("User_ID")]
        public int user_ID { get; set; }

        [JsonProperty("Item_ID")]
        public int item_ID { get; set; }
    }

    //カートテーブル
    [JsonObject]
    public class ShoplistTableList
    {
        [JsonProperty("ShoplistList")]
        public List<ShoplistTableRow> List { get; set; } = new List<ShoplistTableRow>();
    }

    [JsonObject]
    public class ShoplistTableRow
    {
        [JsonProperty("Shop_ID")]
        public int Shop_ID { get; set; }

        [JsonProperty("User_ID")]
        public int user_ID { get; set; }

        [JsonProperty("Item_ID")]
        public int item_ID { get; set; }

        [JsonProperty("Item_num")]
        public int item_num { get; set; }
    }


    //ユーザーテーブル
    [JsonObject]
    public class UserTableList
    {
        [JsonProperty("UserlistList")]
        public List<UserlistTableRow> List { get; set; } = new List<UserlistTableRow>();
    }

    [JsonObject]
    public class UserlistTableRow
    {
        [JsonProperty("user_ID")]
        public int User_ID { get; set; }

        [JsonProperty("user_name")]
        public string User_name { get; set; }

        [JsonProperty("user_password")]
        public string User_password { get; set; }

        [JsonProperty("mail")]
        public string Mail { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("telephone")]
        public string Telephone { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }


    }



}