using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Kogarasi.WebView;

public class WebViewBehavior : MonoBehaviour
{
    IWebView webView;
    IWebViewCallback callback;
    public static WebViewBehavior Instance { get; private set; }

	public void Awake()
	{
        Instance = this;
#if UNITY_EDITOR
        webView = new WebViewNull();
#elif UNITY_IPHONE
        webView = new WebViewIOS();
#elif UNITY_ANDROID
		webView = new WebViewAndroid();
#elif UNITY_WP8
        webView = new WebViewWp8Interface();
#endif
        webView.Init( name );
		callback = null;
	}

	public void OnDestroy()
	{
		webView.Term();
        Instance = null;
	}

	public void SetMargins( int left, int top, int right, int bottom )
	{
		webView.SetMargins( left, top, right, bottom );
	}
	public void SetVisibility( bool state )
	{
		webView.SetVisibility( state );
	}

	public void LoadURL( string url )
	{
		webView.LoadURL( url );
	}

	public void EvaluateJS( string js )
	{
		webView.EvaluateJS( js );
	}

	/*
	public void CallFromJS( string message )
	{
		Debug.Log( "CallFromJS : " + message );
	}
	*/

	public void setCallback( IWebViewCallback _callback )
	{
		callback = _callback;
	}

    public void onLoadStart(string url)
    {
        if (callback != null) { callback.onLoadStart(url); }
	}

    public void onLoadFinish(string url)
    {
        if (callback != null) { callback.onLoadFinish(url); }
	}

    public void onLoadFail(string url)
    {
        if (callback != null) { callback.onLoadFail(url); }
	}
}
