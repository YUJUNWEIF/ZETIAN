using UnityEngine;
using System.Runtime.InteropServices;

public class Sdk91Connector  : MonoBehaviour
{
#if UNITY_IPHONE && IOS91
    [DllImport("__Internal")]
    static extern void _91Initialize(int appId, string appKey, string msgReceiver, bool isdebug);

    [DllImport("__Internal")]
    static extern void _91Login();

    [DllImport("__Internal")]
    static extern void _91LoginAnonymous();

    [DllImport("__Internal")]
    static extern int _91Logout(int param);

    [DllImport("__Internal")]
    static extern bool _91IsLogin();

    [DllImport("__Internal")]
    static extern int _91GetCurLoginState();

    [DllImport("__Internal")]
    static extern void _91Pause();

    [DllImport("__Internal")]
    static extern void _91ShowToolBar(int pos);

    [DllImport("__Internal")]
    static extern void _91HideToolBar();

    [DllImport("__Internal")]
    static extern int _91Pay(string cooOrderSerial, string productId, string productName, float productPrice, int productCount, string payDescription);

    [DllImport("__Internal")]
    static extern int _91PayAsync(string cooOrderSerial, string productId, string productName, float productPrice, int productCount, string payDescription);

    [DllImport("__Internal")]
    static extern int _91PayForCoin(string cooOrderSerial, int needPayCoins, string payDescription);

    [DllImport("__Internal")]
    static extern void _91EnterPlatform();

#elif UNITY_ANDROID && ANDROID91
#endif

    public void InitializeSdk91(int appId, string appKey, string msgReceiver, bool isdebug = false)
    {
#if UNITY_IPHONE && IOS91			
        _91Initialize(appId, appKey, msgReceiver, isdebug); 	
#elif UNITY_ANDROID && ANDROID91
#endif
    }// TODO: API：SDK初始化

    public void Login()// TODO: API：账号登陆
    {
#if UNITY_IPHONE && IOS91
        _91Login();  	
#elif UNITY_ANDROID && ANDROID91
#endif
    }
    public void LoginAnonymous()// TODO: API：游客登陆
    {
#if UNITY_IPHONE && IOS91
        _91LoginAnonymous(); 
#elif UNITY_ANDROID && ANDROID91
#endif
    }
    public int Logout(int param)// TODO: API：注销账号
    {
#if UNITY_IPHONE && IOS91
        return _91Logout(param); 
#elif UNITY_ANDROID && ANDROID91
#endif
        return 0;
    }

    public void ShowToolBar(int point)// TODO: API：显示工具条: point=1,2,3,4,5,6,分别表示：左上、右上、左中、右中、左下、右下 
    {
#if UNITY_IPHONE && IOS91
        _91ShowToolBar(point); 
#elif UNITY_ANDROID && ANDROID91
#endif
    }
    public void HideToolBar()// TODO: API：隐藏工具条
    {
#if UNITY_IPHONE && IOS91
        _91HideToolBar(); 
#elif UNITY_ANDROID && ANDROID91
#endif
    }
    public void Pause()// TODO: API：隐藏工具条
    {
#if UNITY_IPHONE && IOS91
        _91Pause(); 
#elif UNITY_ANDROID && ANDROID91
#endif
    }

    public bool isLogined// TODO: API：判断玩家是否登陆状态
    {
        get
        {
#if UNITY_IPHONE && IOS91
            return _91IsLogin(); 
#elif UNITY_ANDROID && ANDROID91
#endif
            return false;
        }
    }
    public int curLoginState// TODO: API：返回账号状态：0表示未登陆、1表示游客登陆、2表示普通账号登陆
    {
        get
        {
#if UNITY_IPHONE && IOS91
            return _91GetCurLoginState(); 
#elif UNITY_ANDROID && ANDROID91
#endif
            return 0;
        }
    }
    //public static string uin { get { return _91GetUin(); } }// TODO: API：获取账号主键ID
    //public static string session { get { return _91GetSession(); } }// TODO: API：获取会话ID
    //public static string nickName { get { return _91GetNickName(); } }// TODO: API：获取昵称
    // TODO: API：同步支付（订单号，道具ID，道具名，价格，数量，分区：不超过20个英文或数字的字符串）
    public int Pay(string cooOrderSerial, string productId, string productName, float productPrice, int productCount, string payDescription)
    {
#if UNITY_IPHONE && IOS91
        return _91Pay(cooOrderSerial, productId, productName, productPrice, productCount, payDescription);
#elif UNITY_ANDROID && ANDROID91
#endif
        return 0;
    }

    // TODO: API：异步支付（订单号，道具ID，道具名，价格，数量，分区：不超过20个英文或数字的字符串）
    public int PayAsync(string cooOrderSerial, string productId, string productName, float productPrice, int productCount, string payDescription)
    {
#if UNITY_IPHONE && IOS91
        return _91PayAsync(cooOrderSerial, productId, productName, productPrice, productCount, payDescription);
#elif UNITY_ANDROID && ANDROID91
#endif
        return 0;
    }

    // TODO: API：代币充值（订单号，代币数量，分区：不超过20个英文或数字的字符串）
    public int PayForCoin(string cooOrderSerial, int needPayCoins, string payDescription)
    {
#if UNITY_IPHONE && IOS91
        return _91PayForCoin(cooOrderSerial, needPayCoins, payDescription);
#elif UNITY_ANDROID && ANDROID91
#endif
        return 0;
    }
    
    public void EnterPlatform()
    {
#if UNITY_IPHONE && IOS91
        _91EnterPlatform();
#elif UNITY_ANDROID && ANDROID91
#endif
    }
}
