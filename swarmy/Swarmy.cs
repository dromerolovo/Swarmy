using System;
using Microsoft.Playwright;


namespace Swarmy
{   
        public class SwarmyCore
        
    {
        private string imageBucket = Path.Combine(Environment.CurrentDirectory, "imageBucket", "screenshot.png") ;
        
        public async Task PlayWrightInitialization()
        {

            using var playwright = await Playwright.CreateAsync();
            var chromium = playwright.Chromium;
            var browser = await chromium.LaunchAsync(new BrowserTypeLaunchOptions {Channel = "chrome"});
            var page = await browser.NewPageAsync();
            await page.GotoAsync("https://thispersondoesnotexist.com/");
            var content = await page.ContentAsync();
            await page.Locator("id=face").ScreenshotAsync(new LocatorScreenshotOptions { Path = imageBucket });
        }
    }

}
