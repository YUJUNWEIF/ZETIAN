using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sanguo
{
    public enum PurchaseStatus
    {
        Doing,
        Succeed,
        Failed,
        Cancelled,
    }

    public enum VerifyStatus
    {
        Doing,
        Succeed,
        Failed,
    }

    public class InAppBuy
    {
        public string charactorId;
        public string identifier;
        public string receipt;
        public InAppBuy() { }
        public InAppBuy(string charId, string identifier, string receipt) 
        {
            this.charactorId = charId;
            this.identifier = identifier;
            this.receipt = receipt;
        }
        public static string SaveToString(InAppBuy buy)
        {
            return DeJson.Serializer.Serialize(buy);
		}
        public static InAppBuy LoadFromString(string buf)
		{
            return DeJson.Deserializer.Deserialize<InAppBuy>(buf);
		}
    }

    public class StoreProduct
    {
        public string identifier;
        public string title;
        public string description;
        public string price;
        public StoreProduct() { }
        public StoreProduct(string identifier, string title, string description, string price)
        {
            this.identifier = identifier;
            this.title = title;
            this.description = description;
            this.price = price;
        }
        static string[] SaveProduct(StoreProduct product)
        {
            return new string[] { product.identifier, product.title, product.description, product.price };
        }
        static StoreProduct LoadProduct(string[] buf)
        {
            return new StoreProduct(buf[0], buf[1], buf[2], buf[3]);
        }
        public static string SaveProducts(StoreProduct[] products)
        {
            return DeJson.Serializer.Serialize(products);
        }
        public static StoreProduct[] loadProducts(string buf)
        {
            return DeJson.Deserializer.Deserialize<StoreProduct[]>(buf);
        }
        public static string IndentifiersToString(string[] indentifiers)
        {
            return DeJson.Serializer.Serialize(indentifiers);
        }
        public static string[] StringToIndentifiers(string buf)
        {
            return DeJson.Deserializer.Deserialize<string[]>(buf);
        }
    }
}
