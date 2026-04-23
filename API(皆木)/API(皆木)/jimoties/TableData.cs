using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jimoties
{
    //商品テーブル
    [JsonObject]
    public class ItemTableList
    {
        [JsonProperty("ItemTableList")]
        public List<ItemTableRow> List { get; set; } = new List<ItemTableRow>();
    }

    [JsonObject]
    public class ItemTableRow
    {
        [JsonProperty("Item_ID")]
        public int Item_ID { get; set; }

        [JsonProperty("Item_name")]
        public string Item_name { get; set; }

        [JsonProperty("Item_category")]
        public string Item_category { get; set; }

        [JsonProperty("Item_prefecture")]
        public string Item_prefecture { get; set; }

        [JsonProperty("Item_price")]
        public int Item_price { get; set; }

        [JsonProperty("Item_stock")]
        public int Item_stock { get; set; }

        [JsonProperty("Item_image")]
        public string Item_image { get; set; }

        [JsonProperty("Item_ie")]
        public string Item_ie { get; set; }
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
        [JsonProperty("Buy_ID")]
        public int Buy_ID { get; set; }

        [JsonProperty("User_ID")]
        public int User_ID { get; set; }

        [JsonProperty("Item_ID")]
        public int Item_ID { get; set; }

        [JsonProperty("Item_name")]
        public string Item_name { get; set; }

        [JsonProperty("Item_num")]
        public int Item_num { get; set; }

        [JsonProperty("Item_price")]
        public int Item_price { get; set; }

        [JsonProperty("Buy_date")]
        public string Buy_date { get; set; }

        [JsonProperty("Item_image")]
        public string Item_image { get; set; }

        [JsonProperty("Item_prefecture")]
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
        public int Fa_ID { get; set; }

        [JsonProperty("User_ID")]
        public int User_ID { get; set; }

        [JsonProperty("Item_ID")]
        public int Item_ID { get; set; }
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
        public int User_ID { get; set; }

        [JsonProperty("Item_ID")]
        public int Item_ID { get; set; }

        [JsonProperty("Item_num")]
        public int Item_num { get; set; }
    }



}