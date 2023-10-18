namespace Launcher;
public sealed partial class MainWindow : WindowEx
{
    private Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;
    private UISettings settings;
    public void GetAppWindowAndPresenter()
    {
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        _apw = AppWindow.GetFromWindowId(myWndId);
        _presenter = _apw.Presenter as OverlappedPresenter;
    }
    private AppWindow _apw;
    private OverlappedPresenter _presenter;
    public MainWindow()
    {
        InitializeComponent();
        DiscordRPC();
        GetAppWindowAndPresenter();
        _presenter.SetBorderAndTitleBar(false, false);
        Content = null;

        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged;
    }
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }
    private static readonly DiscordRpcClient client = new DiscordRpcClient("1155073793224609843");

    public static void InitRPC()
    {
        client.OnReady += (sender, e) => { };

        client.OnPresenceUpdate += (sender, e) => { };

        client.OnError += (sender, e) => { };

        client.Initialize();
    }

    public static void UpdateRPC()
    {
        var presence = new RichPresence()
        {
            State = "Minty",
            Details = "Hacking MHY <333",

            Assets = new Assets()
            {
                LargeImageKey = "virus",
                SmallImageKey = "",
                SmallImageText = "Minty Launcher"
            },
            Buttons = new DiscordRPC.Button[]
            {
                    new DiscordRPC.Button()
                    {
                        Label = "Join",
                        Url = "https://discord.gg/kindawindytoday"
                    }
            }
        };
        client.SetPresence(presence);
        client.Invoke();
    }

    public static void DiscordRPC()
    {
        if (!client.IsInitialized)
        {
            InitRPC();
        }

        UpdateRPC();
    }
}
