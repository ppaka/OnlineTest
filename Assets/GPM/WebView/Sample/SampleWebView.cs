using System.Collections.Generic;
using Gpm.WebView;
using UnityEngine;

public class SampleWebView : MonoBehaviour
{
    public string sampleUrl = "https://www.google.com";

    public void OpenWebViewWithFullScreen()
    {
        GpmWebView.ShowUrl(sampleUrl,
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.FULLSCREEN,
                isClearCache = true,
                isClearCookie = true,
                title = "FULL SCREEN SAMPLE",
                isNavigationBarVisible = true,
                isBackButtonVisible = true,
                isForwardButtonVisible = true,
                contentMode = GpmWebViewContentMode.RECOMMENDED,
                supportMultipleWindows = true
            },
            OnOpenCallback,
            OnCloseCallback,
            OnPageLoadCallback,
            new List<string>() { "CUSTOM_SCHEME" },
            OnSchemeEvent);
    }

    public void OpenWebViewWithPopup()
    {
        GpmWebView.ShowUrl(sampleUrl,
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.POPUP,
                isClearCache = false,
                isClearCookie = false,
                title = "POPUP SAMPLE",
                navigationBarColor = "#CBA6CE",
                isNavigationBarVisible = true,
                isBackButtonVisible = true,
                contentMode = GpmWebViewContentMode.RECOMMENDED,
                supportMultipleWindows = true
            });
    }

    private void OnOpenCallback(GpmWebViewError error)
    {
        if (error == null)
        {
            Debug.Log("Open WebView Success");
        }
        else
        {
            Debug.LogFormat("Fail to open WebView. Error:{0}", error);
        }
    }

    private void OnCloseCallback(GpmWebViewError error)
    {
        if (error == null)
        {
            Debug.Log("Close WebView Success");
        }
        else
        {
            Debug.LogFormat("Fail to close WebView. Error:{0}", error);
        }
    }

    private void OnSchemeEvent(string data, GpmWebViewError error)
    {
        if (error == null)
        {
            Debug.LogFormat("Scheme:{0}", data);
        }
    }

    private void OnPageLoadCallback(string url)
    {
        if (string.IsNullOrEmpty(url) == false)
        {
            Debug.LogFormat("Loaded Page:{0}", url);
        }
    }
}
