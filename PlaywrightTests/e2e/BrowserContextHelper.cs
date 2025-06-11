using Microsoft.Playwright;

public static class BrowserContextHelper
{
    public static BrowserNewContextOptions GetDefaultOptions()
    {
        return new BrowserNewContextOptions
        {
            ColorScheme = ColorScheme.Dark,
            ViewportSize = new ViewportSize { Width = 1400, Height = 900 },
            BaseURL = "https://app.taxually.com",
        };
    }
}