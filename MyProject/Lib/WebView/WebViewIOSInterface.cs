using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if UNITY_IPHONE

namespace Kogarasi.WebView
{
	public class WebViewIOS : IWebView
	{
		#region Interface Method
		public void Init( string name )
		{
			_WebViewPlugin_Init( name );
		}
		public void Term()
		{
			_WebViewPlugin_Destroy( );
		}

		public void SetMargins( int left, int top, int right, int bottom )
		{
			_WebViewPlugin_SetMargins( left, top, right, bottom );
		}
		public void SetVisibility( bool state )
		{
			_WebViewPlugin_SetVisibility( state );
		}

		public void LoadURL( string url )
		{
			_WebViewPlugin_LoadURL( url );
		}

		public void EvaluateJS( string js )
		{
			_WebViewPlugin_EvaluateJS(  js );
		}

		#endregion
		
		#region Native Access Method
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_Init(string gameObject);
		
		[DllImport("__Internal")]
		private static extern int _WebViewPlugin_Destroy();
		
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_SetMargins(int left, int top, int right, int bottom);
		
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_SetVisibility( bool visibility);
		
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_LoadURL(string url);
		
		[DllImport("__Internal")]
		private static extern void _WebViewPlugin_EvaluateJS(string url);
		
		#endregion
	}
}

#endif