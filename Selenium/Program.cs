using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V85.Page;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium
{
    class Program
    {
        static WebDriver driver;
        static void Main(string[] args)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddExtension(@"Buster--Captcha-Solver-for-Humans.crx");
            driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("https://www.google.com/recaptcha/api2/demo");
            driver.SwitchTo().Frame(0);

            IWebElement elementToCLick = driver.FindElement(By.Id("recaptcha-anchor"));
            elementToCLick.Click();

            Thread.Sleep(2000);
            driver.SwitchTo().DefaultContent();
            driver.SwitchTo().Frame(2);
            var element = driver.FindElement(By.Id("recaptcha-audio-button"));
            new Actions(driver).MoveToElement(element).MoveByOffset(50, 0).Click().Perform();


            driver.SwitchTo().DefaultContent();
            elementToCLick = driver.FindElement(By.Id("recaptcha-demo-submit"));
            elementToCLick.Click();
        }
       

    }
}
