namespace Gpm.WebView.Internal
{
    using System.Collections.Generic;

    public static class NativeRequest
    {
        public class Configuration
        {
            public int style;
            public bool isClearCookie;
            public bool isClearCache;
            public bool isNavigationBarVisible;
            public string navigationBarColor;
            public string title;
            public bool isBackButtonVisible;
            public bool isForwardButtonVisible;
            /// <summary>
            /// iOS only
            /// </summary>
            public int contentMode;
            /// <summary>
            /// Android Only
            /// </summary>
            public bool supportMultipleWindows;
        }

        public class ShowWebView
        {
            public string data;
            public Configuration configuration;
            public int callback = -1;
            public int closeCallback = -1;
            public List<string> schemeList;
            public int schemeEvent = -1;
            public int pageLoadCallback = -1;
        }        

        public class ExecuteJavaScript
        {
            public string script;
        }
    }
}
