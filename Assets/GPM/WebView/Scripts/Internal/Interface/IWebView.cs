namespace Gpm.WebView.Internal
{
    using System.Collections.Generic;

    public interface IWebView
    {
        void ShowUrl(
            string url,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewErrorDelegate openCallback,
            GpmWebViewCallback.GpmWebViewErrorDelegate closeCallback,
            List<string> schemeList,
            GpmWebViewCallback.GpmWebViewDelegate<string> schemeEvent,
            GpmWebViewCallback.GpmWebViewPageLoadDelegate pageLoadCallback);

        void ShowHtmlFile(
            string filePath,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewErrorDelegate openCallback,
            GpmWebViewCallback.GpmWebViewErrorDelegate closeCallback,
            List<string> schemeList,
            GpmWebViewCallback.GpmWebViewDelegate<string> schemeEvent,
            GpmWebViewCallback.GpmWebViewPageLoadDelegate pageLoadCallback);

        void ShowHtmlString(
            string htmlString,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewErrorDelegate openCallback,
            GpmWebViewCallback.GpmWebViewErrorDelegate closeCallback,
            List<string> schemeList,
            GpmWebViewCallback.GpmWebViewDelegate<string> schemeEvent,
            GpmWebViewCallback.GpmWebViewPageLoadDelegate pageLoadCallback);

        void Close();
        void ExecuteJavaScript(string script);
        void SetFileDownloadPath(string path);

        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void GoBack();
        void GoForward();
    }
}
