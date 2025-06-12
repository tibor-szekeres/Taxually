using Microsoft.Playwright;

public class SignupPage(IPage page)
{
  private readonly ILocator _businessTypeIndex = page.Locator("label").Filter(new() { HasText = "E-commerceI sell physical" }).Locator("span").Nth(2);
  private readonly ILocator _legalStatusDropdown = page.Locator("#legalStatus").GetByRole(AriaRole.Textbox);
  private readonly ILocator _companyStatus = page.GetByRole(AriaRole.Option, new() { Name = "Company" }).Locator("div");
  private readonly ILocator _companyNameInput = page.GetByRole(AriaRole.Textbox, new() { Name = "What is the full legal name" });
  private readonly ILocator _countryDropdown = page.Locator("#establishmentCountryId").GetByRole(AriaRole.Textbox);
  private readonly ILocator _hungaryCountryDropdown = page.GetByLabel("Options list").GetByText("Hungary");
  private readonly ILocator _streetInput = page.GetByRole(AriaRole.Textbox, new() { Name = "Street" });
  private readonly ILocator _buildingNumberInput = page.GetByRole(AriaRole.Textbox, new() { Name = "Building number" });
  private readonly ILocator _cityInput = page.GetByRole(AriaRole.Textbox, new() { Name = "City" });
  private readonly ILocator _zipCodeInput = page.Locator("form div").Filter(new() { HasText = "City State/Province ZIP/Post" }).GetByRole(AriaRole.Textbox).Nth(2);
  private readonly ILocator _nextButton = page.GetByRole(AriaRole.Button, new() { Name = "Next" });
  private readonly ILocator _subscriptionSummaryText = page.GetByText("Subscription summary");
  private readonly ILocator _countryJurisdictionItem = page.Locator("app-jurisdiction-item");
  private readonly ILocator _countryJurisdictionItemByName = page.Locator("app-jurisdiction-item");
  private readonly ILocator _alertTextOneNo = page.GetByText("Please select if you have outstanding tax returns");
  private readonly ILocator _alertTextOneNoOneYes = page.GetByText("Please select the period for retroactive filling");
  private readonly ILocator _yesToFirstQuestion = page.GetByText("Help me to get a tax number");
  private readonly ILocator _noToFirstQuestion = page.GetByText("I already have a tax number");
  private readonly ILocator _yesToSecondQuestion = page.GetByText("Need to file tax returns");
  private readonly ILocator _noToSecondQuestion = page.GetByText("All tax returns filed");
  private readonly ILocator _dateSelector = page.GetByRole(AriaRole.Combobox, new() { Name = "YYYY-MM" });
  private readonly ILocator _subscriptionPeriodToggle = page.Locator("tuui-toggle label span");
  private readonly ILocator _termsOfServiceCheckbox = page.Locator("tuui-extended-checkbox label span.checkmark");
  private readonly ILocator _annualFeeSumText = page.GetByText("Annual fee €0");

  public async Task FillinBusinessDetails(string company, string country, string street, string buildingNumber, string city, string zipCode)
  {
    // Wait for a key element to ensure the page is fully loaded
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    await _businessTypeIndex.ClickAsync();
    await _businessTypeIndex.IsCheckedAsync();
    await _legalStatusDropdown.ClickAsync();
    await _companyStatus.ClickAsync();
    await _companyStatus.IsVisibleAsync();
    await _companyNameInput.FillAsync(company);
    var actualCompanyName = await _companyNameInput.InputValueAsync();
    Assert.That(actualCompanyName, Is.EqualTo(company));

    await _countryDropdown.FillAsync(country);
    await _hungaryCountryDropdown.ClickAsync();
    await _hungaryCountryDropdown.IsVisibleAsync();
    await _streetInput.FillAsync(street);
    var actualStreetName = await _streetInput.InputValueAsync();
    Assert.That(actualStreetName, Is.EqualTo(street));

    await _buildingNumberInput.FillAsync(buildingNumber);
    var actualBuildingNumberName = await _buildingNumberInput.InputValueAsync();
    Assert.That(actualBuildingNumberName, Is.EqualTo(buildingNumber));

    await _cityInput.FillAsync(city);
    var actualCityName = await _cityInput.InputValueAsync();
    Assert.That(actualCityName, Is.EqualTo(city));

    await _zipCodeInput.FillAsync(zipCode);
    var actualZipCode = await _zipCodeInput.InputValueAsync();
    Assert.That(actualZipCode, Is.EqualTo(zipCode));

    await _nextButton.ClickAsync();
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    // Assert that the page title is as expected after filling in business details
    await _subscriptionSummaryText.IsVisibleAsync();
  }

  public async Task SelectTargetCountryByIndex(int index)
  {
    // Click on the first N jurisdiction items based on the index provided 
    for (int i = 0; i < index; i++)
    {
      await _countryJurisdictionItem.Nth(i).ClickAsync();
      await _alertTextOneNo.IsVisibleAsync();
    }
  }

  public async Task SelectTargetCountryByText(string countryName)
  {
    if (await _yesToFirstQuestion.IsVisibleAsync() || await _noToFirstQuestion.IsVisibleAsync())
    // If the alert text is already visible, it means the user has already made a selection
    {
      return; // If the alert is already visible, no need to select again
    }
    else
    {
      await _countryJurisdictionItemByName.GetByText(countryName).IsVisibleAsync();
      await _countryJurisdictionItemByName.GetByText(countryName).ClickAsync();
      await _alertTextOneNo.IsVisibleAsync();
    }
  }
  public async Task CheckYesNoAnswers()
  {
    // This method checks the visibility of alert texts based on user selections
    await _yesToFirstQuestion.ClickAsync();
    await _alertTextOneNo.IsHiddenAsync();
    await _noToFirstQuestion.ClickAsync();
    await _alertTextOneNo.IsVisibleAsync();
    await _noToSecondQuestion.ClickAsync();
    await _alertTextOneNoOneYes.IsHiddenAsync();
    await _yesToSecondQuestion.ClickAsync();
    await _alertTextOneNoOneYes.IsVisibleAsync();
  }


  public async Task SelectRetrospectivePeriod(string period)
  {
    await _dateSelector.ClickAsync();
    await page.GetByText(period).ClickAsync();
    await _dateSelector.IsHiddenAsync();
  }
  public async Task ToggleSubscriptionPeriodToMonthly()
  {
    if (await AnnualFeeSumContainsAsync("€0"))
    {
      return; // If the annual fee sum is already zero, no need to toggle
    }
    else
    {
      // If the annual fee sum is not zero, toggle the subscription to monthly
      await _subscriptionPeriodToggle.ClickAsync();
    }
  }

  public async Task AcceptTermsOfService()
  {
    await _termsOfServiceCheckbox.ClickAsync();
    await _termsOfServiceCheckbox.IsCheckedAsync();
  }

  public async Task<bool> AnnualFeeSumContainsAsync(string expectedSum)
  {
    var actualSum = await _annualFeeSumText.InnerTextAsync();
    return actualSum.Contains(expectedSum);
  }

  public async Task AssertAnnualFeeSumIsZero()
  {
    var annualFeeSum = await _annualFeeSumText.InnerTextAsync();
    Assert.That(annualFeeSum, Does.Contain("€0"), "Annual fee sum is not zero as expected.");
  }
}