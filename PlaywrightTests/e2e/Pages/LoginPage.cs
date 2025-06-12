using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTests;

public class LoginPage(IPage page)
{
    private readonly ILocator _emailInput = page.Locator("#email");
    private readonly ILocator _passwordInput = page.Locator("#password");
    private readonly ILocator _loginButton = page.Locator("#next");

    public async Task GoToLoginPage(string text)
    {
        await page.GotoAsync(text);
    }

    public async Task LoginToPage(string email, string password)
    {
        await _emailInput.FillAsync(email);
        await _passwordInput.FillAsync(password);
        await _loginButton.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        Assert.That(await page.Locator(".page-title").TextContentAsync(), Does.Contain("Tell us about your business"), "Page title does not match expected value after login.");
    }
}