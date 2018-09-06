using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if UNITY_WP8

namespace Kogarasi.WebView
{
	public class WebViewWp8Interface : IWebView
	{
        public static IWebView impl { get; set; }
        public void Init(string name) { if (impl != null) { impl.Init(name); } }
        public void Term() { if (impl != null) { impl.Term(); } }
		public void SetMargins( int left, int top, int right, int bottom )
		{
            if (impl != null) { impl.SetMargins(left, top, right, bottom); }
		}
		public void SetVisibility( bool state )
		{
            if (impl != null) { impl.SetVisibility(state); }
		}
		public void LoadURL( string url )
		{
            if (impl != null) { impl.LoadURL(url); }
		}
        public void EvaluateJS(string js) { if (impl != null) { impl.EvaluateJS(js); } }
	}
}

#endif