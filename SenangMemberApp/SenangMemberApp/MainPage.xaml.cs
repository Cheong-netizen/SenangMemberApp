namespace SenangMemberApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

#if ANDROID
            blazorWebView.BlazorWebViewInitialized += (_, args) =>
            {
                args.WebView.SetWebChromeClient(new MicrophoneWebChromeClient());
            };
#endif
        }
    }
}
