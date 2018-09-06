using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class IAPInterface 
{
#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void _InitializeInAppPurchase();
	
	[DllImport("__Internal")]
	private static extern bool _UninitializeInAppPurchase();
	
	[DllImport("__Internal")]
	private static extern void _LoadProductsWithNames(string products);
	
    [DllImport("__Internal")]
	private static extern int _CanBuy();

	[DllImport("__Internal")]
	private static extern void _PurchaseProduct(string itemName);

    [DllImport("__Internal")]
	private static extern void _BuyVerifiedWithServer(bool succeed);
#elif UNITY_ANDROID
#endif

	public static void InitializeInAppPurchase()
	{
		if (Application.platform != RuntimePlatform.OSXEditor && 
            Application.platform != RuntimePlatform.WindowsEditor)		
		{
#if UNITY_IPHONE
			_InitializeInAppPurchase();	
#elif UNITY_ANDROID
#endif
        }
	}
	public static void UninitializeInAppPurchase()
	{
		if (Application.platform != RuntimePlatform.OSXEditor && 
            Application.platform != RuntimePlatform.WindowsEditor)		
		{
#if UNITY_IPHONE
			 _UninitializeInAppPurchase();	
#elif UNITY_ANDROID
#endif
        }		
	}  
    public static bool IsAppStoreReachable()
	{
        if (Application.platform != RuntimePlatform.OSXEditor && 
            Application.platform != RuntimePlatform.WindowsEditor)		
		{
#if UNITY_IPHONE
			
#elif UNITY_ANDROID
#endif
        }
		return true;
	}
    public static void LoadConcreteProducts(List<Sanguo.StoreProduct> abstractProducts)
    {
//         if (Application.platform != RuntimePlatform.OSXEditor &&
//             Application.platform != RuntimePlatform.WindowsEditor)
//         {
// #if UNITY_IPHONE
//             string[] productIdentifiers = new string[abstractProducts.Count];
//             for(int index = 0; index < productIdentifiers.Length; ++index)
//             {
//                 productIdentifiers[index] = abstractProducts[index].identifier;
//             }
// 			_LoadProductsWithNames(Sanguo.StoreProduct.IndentifiersToString(productIdentifiers));
// #elif UNITY_ANDRIOD
// #else
// #endif
//         }
//         else
//         {
//             var products = new Sanguo.StoreProduct[abstractProducts.Count];
//             for (int index = 0; index < products.Length; ++index)
//             {
//                 var ap = abstractProducts[index];
//                 Sanguo.StoreProduct sp = new Sanguo.StoreProduct();
//                 sp.identifier = ap.identifier;
//                 sp.title = ap.title;
//                 sp.description = ap.description;
//                 sp.price = @"10yuan";
//                 products[index] = sp;
//             }
//             var buf = Sanguo.StoreProduct.SaveProducts(products);
//             IOSAppleWrapper.Instance.OnReceiveProdducts(buf);
//         }
    }
	public static void PurchaseProduct(string who, string indentifier)
	{
//         Sanguo.InAppBuy buy = new Sanguo.InAppBuy();
//         buy.charactorId = who;
//         buy.identifier = indentifier;
//         buy.receipt = @"";
//         var buf = Sanguo.InAppBuy.SaveToString(buy);
// 
//         if (Application.platform != RuntimePlatform.OSXEditor &&
//             Application.platform != RuntimePlatform.WindowsEditor)
//         {
// 
// #if UNITY_IPHONE
// 			_PurchaseProduct(buf);
// #elif UNITY_ANDRIOD
// #else
// #endif
//         }
//         else
//         {
//             IOSAppleWrapper.Instance.OnPurchaseSucceed(buf);
//         }
	}
    public static void BuyVerifiedWithServer(bool succeed)
    {
        if (Application.platform != RuntimePlatform.OSXEditor &&
            Application.platform != RuntimePlatform.WindowsEditor)
        {

#if UNITY_IPHONE
			_BuyVerifiedWithServer(succeed);
#elif UNITY_ANDRIOD
#else
#endif
        }
    }
    public static int CanBuy()
    {
        if (Application.platform != RuntimePlatform.OSXEditor &&
            Application.platform != RuntimePlatform.WindowsEditor)
        {

#if UNITY_IPHONE
			return _CanBuy();
#elif UNITY_ANDRIOD
#else
#endif
        }
        return 0; 
    }
}



