/*
 * Copyright (C) 2011 Keijiro Takahashi
 * Copyright (C) 2012 GREE, Inc.
 *
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

#import <UIKit/UIKit.h>

extern UIViewController *UnityGetGLViewController();

@interface WebViewPlugin : NSObject<UIWebViewDelegate>
{
    UIWebView *webView;
    NSString *gameObjectName;
}
@end

@implementation WebViewPlugin

- (void)initWithGameObjectName:(const char *)gameObjectName_
{
    UIView *view = UnityGetGLViewController().view;
    webView = [[UIWebView alloc] initWithFrame:view.frame];
    webView.delegate = self;
    webView.hidden = YES;
    [view addSubview:webView];
    gameObjectName = [NSString stringWithUTF8String:gameObjectName_];}

- (void)dealloc
{
    webView.delegate = nil;
    
    [webView removeFromSuperview];
}

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
    /*
     NSString *url = [[request URL] absoluteString];
     UnitySendMessage([gameObjectName UTF8String], "onLoadStart", [url UTF8String] );
     */
    return YES;
}

- (void)webViewDidStartLoad: (UIWebView*)webView
{
    NSString* url = [[[webView request] URL] absoluteString];
    UnitySendMessage( [gameObjectName UTF8String], "onLoadStart", [url UTF8String] );
}

- (void)webViewDidFinishLoad: (UIWebView*)webView
{
    NSString* url = [webView stringByEvaluatingJavaScriptFromString:@"document.URL"];
    UnitySendMessage( [gameObjectName UTF8String], "onLoadFinish", [url UTF8String] );
}
- (void)webViewDidFailLoadWithError: (NSError*)error
{
    [UIApplication sharedApplication].networkActivityIndicatorVisible = NO;
    
    NSInteger err_code = [error code];
    if( err_code == NSURLErrorCancelled )
    {
        return;
    }
    
}

- (void)setMargins:(int)left top:(int)top right:(int)right bottom:(int)bottom
{
    UIView *view = UnityGetGLViewController().view;
    
    CGRect frame = view.frame;
    CGFloat scale = view.contentScaleFactor;
    if (frame.size.width < frame.size.height) {
        CGFloat tmp = frame.size.width;
        frame.size.width = frame.size.height;
        frame.size.height = tmp;
    }
    frame.size.width -= (left + right) / scale;
    frame.size.height -= (top + bottom) / scale;
    frame.origin.x += left / scale;
    frame.origin.y += top / scale;
    webView.frame = frame;
    webView.backgroundColor = [UIColor clearColor];
    webView.opaque = NO;
    
    // remove shadow view when drag web view
    for (UIView *subView in [webView subviews]) {
        if ([subView isKindOfClass:[UIScrollView class]]) {
            for (UIView *shadowView in [subView subviews]) {
                if ([shadowView isKindOfClass:[UIImageView class]]) {
                    shadowView.hidden = YES;
                }
            }
        }
    }
}

- (void)setVisibility:(BOOL)visibility
{
    webView.hidden = visibility ? NO : YES;
}

- (void)loadURL:(const char *)url
{
    NSString *urlStr = [NSString stringWithUTF8String:url];
    NSURL *nsurl = [NSURL URLWithString:urlStr];
    NSURLRequest *request = [NSURLRequest requestWithURL:nsurl];
    [webView loadRequest:request];
    [webView reload];
}

- (void)evaluateJS:(const char *)js
{
    NSString *jsStr = [NSString stringWithUTF8String:js];
    [webView stringByEvaluatingJavaScriptFromString:jsStr];
}
+(id)sharedHelper
{
    static WebViewPlugin *sharedData = nil;
    @synchronized(self)
    {
        if(sharedData == nil)
        {
            sharedData = [[WebViewPlugin alloc] init];
        }
    }
    return sharedData;
}
@end

extern "C" {
    void _WebViewPlugin_Init(const char *gameObjectName);
    void _WebViewPlugin_Destroy();
    void _WebViewPlugin_SetMargins(int left, int top, int right, int bottom);
    void _WebViewPlugin_SetVisibility(BOOL visibility);
    void _WebViewPlugin_LoadURL(const char *url);
    void _WebViewPlugin_EvaluateJS(const char *url);
}

void _WebViewPlugin_Init(const char *gameObjectName)
{
    [[WebViewPlugin sharedHelper] initWithGameObjectName:gameObjectName];
}

void _WebViewPlugin_Destroy()
{
    
}

void _WebViewPlugin_SetMargins(int left, int top, int right, int bottom)
{
    [[WebViewPlugin sharedHelper] setMargins:left top:top right:right bottom:bottom];
}

void _WebViewPlugin_SetVisibility(BOOL visibility)
{
    [[WebViewPlugin sharedHelper] setVisibility:visibility];
}

void _WebViewPlugin_LoadURL(const char *url)
{
    [[WebViewPlugin sharedHelper] loadURL:url];
}

void _WebViewPlugin_EvaluateJS(const char *js)
{
    [[WebViewPlugin sharedHelper] evaluateJS:js];
}
