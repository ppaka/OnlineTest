using System;
using System.Collections;
using System.Collections.Generic;
using Gpm.WebView;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            ShowUrl();
        if(Input.GetButtonDown("Cancel"))
            CloseUrl();
            
    }

    public void CloseUrl()
    {
        GpmWebView.Close();
    }
    
    public void ShowUrl()
    {
        GpmWebView.ShowUrl(
            "https://google.com/",
            new GpmWebViewRequest.Configuration()
            {
                style = GpmWebViewStyle.FULLSCREEN,
                isClearCookie = false,
                isClearCache = false,
                isNavigationBarVisible = true,
                navigationBarColor = "#4B96E6",
                title = "The page title.",
                isBackButtonVisible = true,
                isForwardButtonVisible = true,
#if UNITY_IOS
            contentMode = GpmWebViewContentMode.MOBILE
#elif UNITY_ANDROID
            supportMultipleWindows = true
#endif
            },
            OnOpenCallback,
            OnCloseCallback,
            OnPageLoadCallback,
            new List<string>()
            {
                "USER_ CUSTOM_SCHEME"
            },
            OnSchemeEvent);
    }

    private void OnOpenCallback(GpmWebViewError error)
    {
        if (error == null)
        {
            Debug.Log("[OnOpenCallback] succeeded.");
        }
        else
        {
            Debug.Log(string.Format("[OnOpenCallback] failed. error:{0}", error));
        }
    }

    private void OnCloseCallback(GpmWebViewError error)
    {
        if (error == null)
        {
            Debug.Log("[OnCloseCallback] succeeded.");
        }
        else
        {
            Debug.Log(string.Format("[OnCloseCallback] failed. error:{0}", error));
        }
    }

    private void OnPageLoadCallback(string url)
    {
        if (string.IsNullOrEmpty(url) == false)
        {
            Debug.LogFormat("[OnPageLoadCallback] Loaded Page:{0}", url);
        }
    }

    private void OnSchemeEvent(string data, GpmWebViewError error)
    {
        if (error == null)
        {
            Debug.Log("[OnSchemeEvent] succeeded.");
        
            if (data.Equals("USER_ CUSTOM_SCHEME") == true || data.Contains("CUSTOM_SCHEME") == true)
            {
                Debug.Log(string.Format("scheme:{0}", data));
            }
        }
        else
        {
            Debug.Log(string.Format("[OnSchemeEvent] failed. error:{0}", error));
        }
    }
}
