using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTests;

public class LoginPage(IPage page)
{
    private readonly IPage _page = page;
    private readonly ILocator _emailInput = page.Locator("#email");
    private readonly ILocator _passwordInput = page.Locator("#password");
    private readonly ILocator _loginButton = page.Locator("#next");

    public async Task GoToLoginPage(string text)
    {
        await _page.GotoAsync(text);
    }

    public async Task LoginToPage(string email, string password)
    {
        await _emailInput.FillAsync(email);
        await _passwordInput.FillAsync(password);
        await _loginButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    }
}