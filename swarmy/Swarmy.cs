using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Playwright;


namespace Swarmy
{   
        

        public class SwarmyCore
        
    {
        
        //Programmatic profile in progress
        public async Task GetModelsProfiles(int repetition, InitializationCommand command, int startNumber = 0)
        {
            string [] files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, @"imageBucket\"), "*.png");
            List<string> filesNames = new List<string> {};

            List<int> lastModelNumberList = new List<int>{};
            int counter = 0;

            System.IO.DirectoryInfo directory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"imageBucket\"));

            if(command == InitializationCommand.Continue) {

                if(files == null || files.Length == 0) {

                    throw new ArgumentException("If there is no images in imageBucket you shouldn't use InitializationCommand.Continue, try again with InitializationCommand.StartOver");

                } else if (files.Length > 0) {

                    foreach(string file in files) {

                        var regex = new Regex("([0-9])+");
                        var number = Int16.Parse(regex.Match(Path.GetFileName(file)).Groups[0].Value);
                        lastModelNumberList.Add(number);
                    }

                    counter = lastModelNumberList.Max() + 1;
                }
            } else if(command == InitializationCommand.StartOver) {

                counter = startNumber;

                foreach(FileInfo file in directory.GetFiles()) {

                    file.Delete();
                }
            }

            
            
            for(var i = counter; i < repetition + counter; i++) {

                string imageBucket = Path.Combine(Environment.CurrentDirectory, "imageBucket", $"model{i}.png");

                using var playwright = await Playwright.CreateAsync();
                var chromium = playwright.Chromium;
                var browser = await chromium.LaunchAsync(new BrowserTypeLaunchOptions {Channel = "chrome"});
                var page = await browser.NewPageAsync();
                await page.GotoAsync("https://thispersondoesnotexist.com/");
                var content = await page.ContentAsync();
                await page.Locator("id=face").ScreenshotAsync(new LocatorScreenshotOptions { Path = imageBucket });
            }

        }

        public enum InitializationCommand {StartOver, Continue}
        
    }

        

}
