#import <GamePackageManagerWebView/GamePackageManagerWebView.h>
#import "GPMWebViewPlugin.h"
#import "GPMCommunicatorPlugin.h"
#import "GPMCommunicatorReceiver.h"
#import "GPMWebViewMessage.h"
#import "GPMWebViewJsonUtil.h"
#import "GPMCommunicatorMessage.h"


#define GPM_WEBVIEW_DOMAIN @"GPM_WEBVIEW"
#define GPM_WEBVIEW_API_SHOW_URL @"gpmwebview://showUrl"
#define GPM_WEBVIEW_API_SHOW_HTML_FILE @"gpmwebview://showHtmlFile"
#define GPM_WEBVIEW_API_SHOW_HTML_STRING @"gpmwebview://showHtmlString"
#define GPM_WEBVIEW_API_CLOSE @"gpmwebview://close"
#define GPM_WEBVIEW_API_EXECUTE_JAVASCRIPT @"gpmwebview://executeJavaScript"
#define GPM_WEBVIEW_API_SET_FILE_DOWNLOAD_PATH @"gpmwebview://setFileDownloadPath"
#define GPM_WEBVIEW_API_CAN_GO_BACK @"gpmwebview://canGoBack"
#define GPM_WEBVIEW_API_CAN_GO_FORWARD @"gpmwebview://canGoForward"
#define GPM_WEBVIEW_API_GO_BACK @"gpmwebview://goBack"
#define GPM_WEBVIEW_API_GO_FORWARD @"gpmwebview://goForward"

#define GPM_WEBVIEW_CALLBACK_SCHEME_EVENT @"gpmwebview://schemeEvent"
#define GPM_WEBVIEW_CALLBACK_CLOSE_CALLBACK @"gpmwebview://closeCallback";
#define GPM_WEBVIEW_CALLBACK_CALLBACK @"gpmwebview://callback";
#define GPM_WEBVIEW_CALLBACK_PAGE_LOAD @"gpmwebview://pageLoadCallback";

@implementation GPMWebViewPlugin

- (id)init {
    if((self = [super init]) == nil) {
        return nil;
    }
    
    GPMCommunicatorReceiver* receiver = [[GPMCommunicatorReceiver alloc] init];
    
    receiver.onRequestMessageSync = ^GPMCommunicatorMessage*(GPMCommunicatorMessage *message) {
        return [self onSyncMessage:message];
    };
    
    receiver.onRequestMessageAsync = ^(GPMCommunicatorMessage *message) {
        [self onAsyncMessage:message];
    };
    
    [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] addReceiverWithDomain:GPM_WEBVIEW_DOMAIN receiver:receiver];
    return self;
}

- (GPMCommunicatorMessage*)onSyncMessage: (GPMCommunicatorMessage*)message {
    GPMWebViewMessage* webviewMessage = [[GPMWebViewMessage alloc]initWithJsonString:message.data];
    GPMCommunicatorMessage* returnMessage = nil;
    
    if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_CAN_GO_BACK]) {
        returnMessage = [self canGoBack];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_CAN_GO_FORWARD]) {
        returnMessage = [self canGoForward];
    }
    
    return returnMessage;
}

- (void)onAsyncMessage: (GPMCommunicatorMessage*)message {
    GPMWebViewMessage* webviewMessage = [[GPMWebViewMessage alloc]initWithJsonString:message.data];
    
    if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SHOW_URL]) {
        [self showUrl:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SHOW_HTML_FILE]) {
        [self showHtmlFile:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SHOW_HTML_STRING]) {
        [self showHtmlString:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_CLOSE]) {
        [self close];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_EXECUTE_JAVASCRIPT]) {
        [self executeJavaScript:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_SET_FILE_DOWNLOAD_PATH]) {
        [self setFileDownloadPath:webviewMessage];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_GO_BACK]) {
        [self goBack];
    } else if([webviewMessage.scheme isEqualToString:GPM_WEBVIEW_API_GO_FORWARD]) {
        [self goForward];
    }
}

- (void)showUrl:(GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    
    NSString* url = dataDic[@"data"];
    NSInteger closeCallback = [dataDic[@"closeCallback"] intValue];
    NSInteger pageLoadCallback = [dataDic[@"pageLoadCallback"] intValue];
    NSInteger schemeEvent = [dataDic[@"schemeEvent"] intValue];
    NSArray* schemeArray = (NSArray*)dataDic[@"schemeList"];
    GPMWebViewConfiguration* configuration = nil;
    NSDictionary* configurationDic = dataDic[@"configuration"];
    
    if(configurationDic != nil && [configurationDic isEqual:[NSNull null]] == NO) {
        configuration = [[GPMWebViewConfiguration alloc] init];
        configuration.style = (GPMWebViewStyle)[configurationDic[@"style"] intValue];
        configuration.isClearCookie = [configurationDic[@"isClearCookie"] boolValue];
        configuration.isClearCache = [configurationDic[@"isClearCache"] boolValue];
        configuration.isNavigationBarVisible = [configurationDic[@"isNavigationBarVisible"] boolValue];
        configuration.navigationBarColor = [self colorFromHexString:configurationDic[@"navigationBarColor"]];
        configuration.navigationBarTitle = configurationDic[@"title"];
		configuration.isBackButtonVisible = (GPMWebViewContent)[configurationDic[@"isBackButtonVisible"] boolValue];
        configuration.isForwardButtonVisible = [configurationDic[@"isForwardButtonVisible"] boolValue];
        configuration.contentMode = [configurationDic[@"contentMode"] intValue];
    }
    
    [GPMWebView showWithURL:url viewController:UnityGetGLViewController() configuration:configuration openCompletion:^(GPMWebViewError *error) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_CALLBACK;
        requestMessage.callback = webViewMessage.callback;
        requestMessage.error = [error jsonString];
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    } closeCompletion:^(GPMWebViewError *error) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_CLOSE_CALLBACK;
        requestMessage.callback = closeCallback;
        requestMessage.extra = [@(schemeEvent) stringValue];
        requestMessage.error = [error jsonString];
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    } pageLoadCompletion:^(NSString *url) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_PAGE_LOAD;
        requestMessage.callback = pageLoadCallback;
        requestMessage.data = url;
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    } schemeList:schemeArray schemeEvent:^(NSString *fullUrl, GPMWebViewError *error) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_SCHEME_EVENT;
        requestMessage.callback = schemeEvent;
        requestMessage.data = fullUrl;
        requestMessage.error = [error jsonString];
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    }];
}

- (void) showHtmlFile: (GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    
    NSString* filePath = dataDic[@"data"];
    NSInteger closeCallback = [dataDic[@"closeCallback"] intValue];
    NSInteger pageLoadCallback = [dataDic[@"pageLoadCallback"] intValue];
    NSInteger schemeEvent = [dataDic[@"schemeEvent"] intValue];
    NSArray* schemeArray = (NSArray*)dataDic[@"schemeList"];
    
    GPMWebViewConfiguration* configuration = nil;
    NSDictionary* configurationDic = dataDic[@"configuration"];
    if(configurationDic != nil && [configurationDic isEqual:[NSNull null]] == NO) {
        configuration = [[GPMWebViewConfiguration alloc] init];
        configuration.style = (GPMWebViewStyle)[configurationDic[@"style"] intValue];
        configuration.isClearCookie = [configurationDic[@"isClearCookie"] boolValue];
        configuration.isClearCache = [configurationDic[@"isClearCache"] boolValue];
        configuration.isNavigationBarVisible = [configurationDic[@"isNavigationBarVisible"] boolValue];
        configuration.navigationBarColor = [self colorFromHexString:configurationDic[@"navigationBarColor"]];
        configuration.navigationBarTitle = configurationDic[@"title"];
		configuration.isBackButtonVisible = (GPMWebViewContent)[configurationDic[@"isBackButtonVisible"] boolValue];
        configuration.isForwardButtonVisible = [configurationDic[@"isForwardButtonVisible"] boolValue];
        configuration.contentMode = [configurationDic[@"contentMode"] intValue];
    }
    
    [GPMWebView showWithHTMLFile:filePath viewController:UnityGetGLViewController() configuration:configuration openCompletion:^(GPMWebViewError *error) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_CALLBACK;
        requestMessage.callback = webViewMessage.callback;
        requestMessage.error = [error jsonString];
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    } closeCompletion:^(GPMWebViewError *error) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_CLOSE_CALLBACK;
        requestMessage.callback = closeCallback;
        requestMessage.extra = [@(schemeEvent) stringValue];
        requestMessage.error = [error jsonString];
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    } pageLoadCompletion:^(NSString *url) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_PAGE_LOAD;
        requestMessage.callback = pageLoadCallback;
        requestMessage.data = url;
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    } schemeList:schemeArray schemeEvent:^(NSString *fullUrl, GPMWebViewError *error) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_SCHEME_EVENT;
        requestMessage.callback = schemeEvent;
        requestMessage.data = fullUrl;
        requestMessage.error = [error jsonString];
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    }];
}

- (void) showHtmlString: (GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    
    NSString* htmlString = dataDic[@"data"];
    NSInteger closeCallback = [dataDic[@"closeCallback"] intValue];
    NSInteger pageLoadCallback = [dataDic[@"pageLoadCallback"] intValue];
    NSInteger schemeEvent = [dataDic[@"schemeEvent"] intValue];
    NSArray* schemeArray = (NSArray*)dataDic[@"schemeList"];
    
    GPMWebViewConfiguration* configuration = nil;
    NSDictionary* configurationDic = dataDic[@"configuration"];
    if(configurationDic != nil && [configurationDic isEqual:[NSNull null]] == NO) {
        configuration = [[GPMWebViewConfiguration alloc] init];
        configuration.style = (GPMWebViewStyle)[configurationDic[@"style"] intValue];
        configuration.isClearCookie = [configurationDic[@"isClearCookie"] boolValue];
        configuration.isClearCache = [configurationDic[@"isClearCache"] boolValue];
        configuration.isNavigationBarVisible = [configurationDic[@"isNavigationBarVisible"] boolValue];
        configuration.navigationBarColor = [self colorFromHexString:configurationDic[@"navigationBarColor"]];
        configuration.navigationBarTitle = configurationDic[@"title"];
		configuration.isBackButtonVisible = (GPMWebViewContent)[configurationDic[@"isBackButtonVisible"] boolValue];
        configuration.isForwardButtonVisible = [configurationDic[@"isForwardButtonVisible"] boolValue];
        configuration.contentMode = [configurationDic[@"contentMode"] intValue];
    }
    
    [GPMWebView showWithHTMLString:htmlString viewController:UnityGetGLViewController() configuration:configuration openCompletion:^(GPMWebViewError *error) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_CALLBACK;
        requestMessage.callback = webViewMessage.callback;
        requestMessage.error = [error jsonString];
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    } closeCompletion:^(GPMWebViewError *error) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_CLOSE_CALLBACK;
        requestMessage.callback = closeCallback;
        requestMessage.extra = [@(schemeEvent) stringValue];
        requestMessage.error = [error jsonString];
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    } pageLoadCompletion:^(NSString *url) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_PAGE_LOAD;
        requestMessage.callback = pageLoadCallback;
        requestMessage.data = url;
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    } schemeList:schemeArray schemeEvent:^(NSString *fullUrl, GPMWebViewError *error) {
        GPMWebViewMessage* requestMessage = [[GPMWebViewMessage alloc] init];
        requestMessage.scheme = GPM_WEBVIEW_CALLBACK_SCHEME_EVENT;
        requestMessage.callback = schemeEvent;
        requestMessage.data = fullUrl;
        requestMessage.error = [error jsonString];
        
        GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
        message.domain = GPM_WEBVIEW_DOMAIN;
        message.data = [requestMessage toJsonString];
        
        [[GPMCommunicatorPlugin sharedGPMCommunicatorPlugin] sendResponseWithMessage:message];
    }];
}

- (void) executeJavaScript: (GPMWebViewMessage*)webViewMessage {
    NSDictionary* dataDic = [webViewMessage.data JSONDictionary];
    NSString* script = dataDic[@"script"];
    
    [GPMWebView executeJavaScriptWithScript:script];
}

- (void) close {
    [GPMWebView close];
}

- (void) setFileDownloadPath: (GPMWebViewMessage*)webViewMessage {
    
}

- (GPMCommunicatorMessage*) canGoBack {
    BOOL result = [GPMWebView canGoBack];
    
    GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
    message.domain = GPM_WEBVIEW_DOMAIN;
    message.data = result ? @"true" : @"false";
    
    return message;
}

- (GPMCommunicatorMessage*) canGoForward {
    BOOL result = [GPMWebView canGoForward];
    
    GPMCommunicatorMessage* message = [[GPMCommunicatorMessage alloc] init];
    message.domain = GPM_WEBVIEW_DOMAIN;
    message.data = result ? @"true" : @"false";
    
    return message;
}

- (void) goBack {
    [GPMWebView goForward];
}

- (void) goForward {
    [GPMWebView goForward];
}

- (UIColor *)colorFromHexString:(NSString *)hexString {
    unsigned rgbValue = 0;
    NSScanner *scanner = [NSScanner scannerWithString:hexString];
    [scanner setScanLocation:1]; // bypass '#' character
    [scanner scanHexInt:&rgbValue];
    return [UIColor colorWithRed:((rgbValue & 0xFF0000) >> 16)/255.0 green:((rgbValue & 0xFF00) >> 8)/255.0 blue:(rgbValue & 0xFF)/255.0 alpha:1.0];
}
@end

