using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

using PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class E2ETest : PageTest
{
    private LoginPage _loginPage;
    private SignupPage _signupPage;
    private const string email = "tiboralex99@gmail.com";
    private const string password = "PassWord.123";
    private const string companyName = "Test LTD";
    private const string countryType = "hunga";
    private const string street = "Test Street";
    private const string buildingNumber = "123";
    private const string city = "Test City";
    private const string zipCode = "12345";

    
    [SetUp]
    public async Task SetUp()
    {
        _loginPage = new LoginPage(Page);
        _signupPage = new SignupPage(Page);
        await _loginPage.GoToLoginPage("/");
        await _loginPage.LoginToPage(email, password);
    }

    [Test]
    public async Task FillinBusinessDetailsTest()
    {
        await _signupPage.FillinBusinessDetails(
            companyName,
            countryType,
            street,
            buildingNumber,
            city,
            zipCode);
    }

    [Test]
    public async Task CheckSelectJurisdictionTestByIndex()
    {
        await _signupPage.FillinBusinessDetails(
            companyName,
            countryType,
            street,
            buildingNumber,
            city,
            zipCode);
        await _signupPage.SelectTargetCountryByIndex(0);
        await _signupPage.CheckYesNoAnswers();
        await _signupPage.SelectRetrospectivePeriod("May");
        await _signupPage.ToggleSubscriptionPeriodToMonthly();
        await _signupPage.AcceptTermsOfService();
        await _signupPage.AssertAnnualFeeSumIsZero();
    }

    [Test]
    public async Task CheckSelectJurisdictionTestByText()
    {
        await _signupPage.FillinBusinessDetails(
            companyName,
            countryType,
            street,
            buildingNumber,
            city,
            zipCode);
        await _signupPage.SelectTargetCountryByText("Belgium");
        await _signupPage.CheckYesNoAnswers();
        await _signupPage.SelectRetrospectivePeriod("Jan");
        await _signupPage.ToggleSubscriptionPeriodToMonthly();
        await _signupPage.AcceptTermsOfService();
        await _signupPage.AssertAnnualFeeSumIsZero();
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return BrowserContextHelper.GetDefaultOptions();
    }
}