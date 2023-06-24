using XboxAuthNet.OAuth.CodeFlow;

namespace XboxAuthNet;

public class PlatformManager
{
    private static PlatformManager? _currentPlatformInstance;
    public static PlatformManager CurrentPlatform =>
        _currentPlatformInstance ??= new PlatformManager();

    public IWebUI CreateWebUI(WebUIOptions uiOptions)
    {
        IWebUI? ui = null;

#if ENABLE_WEBVIEW2
        ui = new XboxAuthNet.Platforms.WinForm.WebView2WebUI(uiOptions);
#endif

        if (ui == null)
            throw new PlatformNotSupportedException("Current platform does not support to provide default WebUI.");
        return ui;
    }
}