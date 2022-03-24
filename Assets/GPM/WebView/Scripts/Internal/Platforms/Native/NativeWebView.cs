﻿namespace Gpm.WebView.Internal
{
    using System;
    using System.Collections.Generic;
    using Gpm.Common.ThirdParty.LitJson;
    using Gpm.Communicator;
    using UnityEngine;

    public class NativeWebView : IWebView
    {
        protected static class ApiScheme
        {
            public const string SHOW_URL = "gpmwebview://showUrl";
            public const string SHOW_HTML_FILE = "gpmwebview://showHtmlFile";
            public const string SHOW_HTML_STRING = "gpmwebview://showHtmlString";
            public const string CLOSE = "gpmwebview://close";
            public const string EXECUTE_JAVASCRIPT = "gpmwebview://executeJavaScript";
            public const string SET_FILE_DOWNLOAD_PATH = "gpmwebview://setFileDownloadPath";
            public const string CAN_GO_BACK = "gpmwebview://canGoBack";
            public const string CAN_GO_FORWARD = "gpmwebview://canGoForward";
            public const string GO_BACK = "gpmwebview://goBack";
            public const string GO_FORWARD = "gpmwebview://goForward";
        }

        protected static class CallbackScheme
        {
            public const string SCHEME_EVENT_CALLBACK = "gpmwebview://schemeEvent";
            public const string CLOSE_CALLBACK = "gpmwebview://closeCallback";
            public const string CALLBACK = "gpmwebview://callback";
            public const string PAGE_LOAD_CALLBACK = "gpmwebview://pageLoadCallback";
        }

        private const string DOMAIN = "GPM_WEBVIEW";
        protected string CLASS_NAME = string.Empty;

        public bool CanGoBack
        {
            get
            {
                NativeMessage message = new NativeMessage()
                {
                    scheme = ApiScheme.CAN_GO_BACK
                };

                var resultMessage = CallSync(JsonMapper.ToJson(message), string.Empty);

                return Convert.ToBoolean(resultMessage.data);
            }
        }


        public bool CanGoForward
        {
            get
            {
                NativeMessage message = new NativeMessage()
                {
                    scheme = ApiScheme.CAN_GO_FORWARD
                };

                var resultMessage = CallSync(JsonMapper.ToJson(message), string.Empty);

                return Convert.ToBoolean(resultMessage.data);
            }
        }

        public NativeWebView()
        {
            Initialize();
        }

        virtual protected void Initialize()
        {
            GpmCommunicatorVO.Configuration configuration = new GpmCommunicatorVO.Configuration()
            {
                className = CLASS_NAME
            };

            GpmCommunicator.InitializeClass(configuration);
            GpmCommunicator.AddReceiver(DOMAIN, OnAsyncEvent);
        }

        public void ShowUrl(
            string url,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewErrorDelegate openCallback,
            GpmWebViewCallback.GpmWebViewErrorDelegate closeCallback,
            List<string> schemeList,
            GpmWebViewCallback.GpmWebViewDelegate<string> schemeEvent,
            GpmWebViewCallback.GpmWebViewPageLoadDelegate pageLoadCallback)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SHOW_URL,
                callback = NativeCallbackHandler.RegisterCallback(openCallback)
            };

            NativeRequest.ShowWebView showWebView = MakeShowWebView(url, configuration, closeCallback, schemeList, schemeEvent, pageLoadCallback);

            nativeMessage.data = JsonMapper.ToJson(showWebView);
            CallAsync(JsonMapper.ToJson(nativeMessage), null);
        }

        public void ShowHtmlFile(
            string filePath,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewErrorDelegate openCallback,
            GpmWebViewCallback.GpmWebViewErrorDelegate closeCallback,
            List<string> schemeList,
            GpmWebViewCallback.GpmWebViewDelegate<string> schemeEvent,
            GpmWebViewCallback.GpmWebViewPageLoadDelegate pageLoadCallback)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SHOW_HTML_FILE,
                callback = NativeCallbackHandler.RegisterCallback(openCallback)
            };

            NativeRequest.ShowWebView showWebView = MakeShowWebView(filePath, configuration, closeCallback, schemeList, schemeEvent, pageLoadCallback);

            nativeMessage.data = JsonMapper.ToJson(showWebView);

            CallAsync(JsonMapper.ToJson(nativeMessage), null);
        }

        public void ShowHtmlString(
            string htmlString,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewErrorDelegate openCallback,
            GpmWebViewCallback.GpmWebViewErrorDelegate closeCallback,
            List<string> schemeList,
            GpmWebViewCallback.GpmWebViewDelegate<string> schemeEvent,
            GpmWebViewCallback.GpmWebViewPageLoadDelegate pageLoadCallback)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SHOW_HTML_STRING,
                callback = NativeCallbackHandler.RegisterCallback(openCallback)
            };

            NativeRequest.ShowWebView showWebView = MakeShowWebView(htmlString, configuration, closeCallback, schemeList, schemeEvent, pageLoadCallback);

            nativeMessage.data = JsonMapper.ToJson(showWebView);

            CallAsync(JsonMapper.ToJson(nativeMessage), null);
        }

        public void Close()
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.CLOSE
            };

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public void ExecuteJavaScript(string script)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.EXECUTE_JAVASCRIPT
            };

            nativeMessage.data = JsonMapper.ToJson(new NativeRequest.ExecuteJavaScript
            {
                script = script
            });

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public void SetFileDownloadPath(string path)
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.SET_FILE_DOWNLOAD_PATH
            };

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        private NativeRequest.ShowWebView MakeShowWebView(
            string data,
            GpmWebViewRequest.Configuration configuration,
            GpmWebViewCallback.GpmWebViewErrorDelegate closeCallback,
            List<string> schemeList,
            GpmWebViewCallback.GpmWebViewDelegate<string> schemeEvent,
            GpmWebViewCallback.GpmWebViewPageLoadDelegate pageLoadCallback)
        {
            NativeRequest.ShowWebView showWebView = new NativeRequest.ShowWebView
            {
                data = data,
                closeCallback = NativeCallbackHandler.RegisterCallback(closeCallback),
                schemeList = schemeList,
                schemeEvent = NativeCallbackHandler.RegisterCallback(schemeEvent),
                pageLoadCallback = NativeCallbackHandler.RegisterCallback(pageLoadCallback),
                configuration = new NativeRequest.Configuration()
                {
                    style = configuration.style,
                    isClearCookie = configuration.isClearCookie,
                    isClearCache = configuration.isClearCache,
                    isNavigationBarVisible = configuration.isNavigationBarVisible,
                    navigationBarColor = configuration.navigationBarColor,
                    supportMultipleWindows = configuration.supportMultipleWindows,
                    title = configuration.title,
                    isBackButtonVisible = configuration.isBackButtonVisible,
                    isForwardButtonVisible = configuration.isForwardButtonVisible,
                    contentMode = configuration.contentMode
                }
            };

            return showWebView;
        }

        private void CallAsync(string data, string extra)
        {
            GpmCommunicatorVO.Message message = new GpmCommunicatorVO.Message()
            {
                domain = DOMAIN,
                data = data,
                extra = extra
            };

            GpmCommunicator.CallAsync(message);
        }

        private GpmCommunicatorVO.Message CallSync(string data, string extra)
        {
            GpmCommunicatorVO.Message message = new GpmCommunicatorVO.Message()
            {
                domain = DOMAIN,
                data = data,
                extra = extra
            };

            return GpmCommunicator.CallSync(message);
        }

        private void OnAsyncEvent(GpmCommunicatorVO.Message message)
        {
            //Debug.Log("OnAsyncEvent : " + message.data);
            NativeMessage nativeMessage = JsonMapper.ToObject<NativeMessage>(message.data);

            if (nativeMessage == null)
            {
                return;
            }

            switch (nativeMessage.scheme)
            {
                case CallbackScheme.SCHEME_EVENT_CALLBACK:
                    {
                        OnSchemeEventCallback(nativeMessage);
                        break;
                    }
                case CallbackScheme.CLOSE_CALLBACK:
                    {
                        OnCloseCallback(nativeMessage);
                        break;
                    }
                case CallbackScheme.CALLBACK:
                    {
                        OnCallback(nativeMessage);
                        break;
                    }
                case CallbackScheme.PAGE_LOAD_CALLBACK:
                    {
                        OnPageLoadCallback(nativeMessage);
                        break;
                    }
            }
        }


        private void OnCallback(NativeMessage nativeMessage)
        {
            var callback = NativeCallbackHandler.GetCallback<GpmWebViewCallback.GpmWebViewErrorDelegate>(nativeMessage.callback);

            if (callback != null)
            {
                GpmWebViewError error = null;

                if (string.IsNullOrEmpty(nativeMessage.error) == false)
                {
                    error = JsonMapper.ToObject<GpmWebViewError>(nativeMessage.error);
                }

                NativeCallbackHandler.UnregisterCallback(nativeMessage.callback);

                callback(error);
            }
        }

        private void OnCloseCallback(NativeMessage nativeMessage)
        {
            var callback = NativeCallbackHandler.GetCallback<GpmWebViewCallback.GpmWebViewErrorDelegate>(nativeMessage.callback);

            if (callback != null)
            {
                GpmWebViewError error = null;

                if (string.IsNullOrEmpty(nativeMessage.error) == false)
                {
                    error = JsonMapper.ToObject<GpmWebViewError>(nativeMessage.error);
                }

                NativeCallbackHandler.UnregisterCallback(nativeMessage.callback);
                NativeCallbackHandler.UnregisterCallback(int.Parse(nativeMessage.extra));

                callback(error);
            }
        }

        private void OnSchemeEventCallback(NativeMessage nativeMessage)
        {
            var callback = NativeCallbackHandler.GetCallback<GpmWebViewCallback.GpmWebViewDelegate<string>>(nativeMessage.callback);

            if (callback != null)
            {
                GpmWebViewError error = null;

                if (string.IsNullOrEmpty(nativeMessage.error) == false)
                {
                    error = JsonMapper.ToObject<GpmWebViewError>(nativeMessage.error);
                }

                callback(nativeMessage.data, error);
            }
        }

        private void OnPageLoadCallback(NativeMessage nativeMessage)
        {
            var callback = NativeCallbackHandler.GetCallback<GpmWebViewCallback.GpmWebViewPageLoadDelegate>(nativeMessage.callback);

            if (callback != null)
            {
                callback(nativeMessage.data);
            }
        }

        public void GoBack()
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.GO_BACK
            };

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }

        public void GoForward()
        {
            NativeMessage nativeMessage = new NativeMessage
            {
                scheme = ApiScheme.GO_FORWARD
            };

            string jsonData = JsonMapper.ToJson(nativeMessage);

            CallAsync(jsonData, null);
        }
    }
}
