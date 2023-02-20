using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace PocSeleniumVnDocs
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ChromeOptions();
            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");

            using (IWebDriver driver = new ChromeDriver(ConfigurationManager.AppSettings["Path_chromedriver"], options))
            {
                driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["Path_website"]);
                WebDriverHelper.SleepRandomly();

                if (!driver.ValidateTitle("Index - DocsRoot", 20000))
                {
                    Console.WriteLine("Cannot validate title");
                }

                var loginElm = driver.FindElement(By.XPath("//span[contains(@class, 'glyphicon-log-in')]"));
                loginElm.Click();

                if (!driver.ValidateForm("auth", 20000))
                {
                    Console.WriteLine("Cannot validate login form");
                }
                WebDriverHelper.SleepRandomly();

                if (!driver.FindAndCheckReCaptchaCheckbox(20000))
                {
                    Console.WriteLine("Cannot validate captcha checkbox");
                }

                var emailElm = driver.FindElement(By.Id("Email"));
                emailElm.SendKeys(ConfigurationManager.AppSettings["Account_Email"]);
                WebDriverHelper.SleepRandomly();

                var pwdElm = driver.FindElement(By.Id("AccessCode"));
                pwdElm.SendKeys(ConfigurationManager.AppSettings["Account_Pwd"]);
                WebDriverHelper.SleepRandomly();

                var submitElm = driver.FindElement(By.XPath("//button[@type='submit']"));
                submitElm.Click();
                WebDriverHelper.SleepRandomly();

                driver.ValidateAfterLogin(20000);
            }
        }
    }

    static class WebDriverHelper
    {
        /// <summary>
        /// Slowdown the script, to not being detected as a script but like a human
        /// </summary>
        public static void SleepRandomly()
        {
            var sleep = new Random().Next(5, 20);
            Thread.Sleep(sleep * 1000);
        }

        public static bool ValidateAfterLogin(this IWebDriver driver, int sleepTimeout)
        {
            var tempCount = 0;

            while (sleepTimeout > tempCount)
            {
                try
                {
                    var logoutElm = driver.FindElement(By.XPath("//span[contains(@class, 'glyphicon-log-out')]"));
                    var h2Elements = driver.FindElements(By.TagName("h2"));

                    if (logoutElm != null && h2Elements.First().Text.Equals("List of Spaces"))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                var sleep = 100;
                tempCount += sleep;

                Thread.Sleep(sleep);
            }

            Console.WriteLine("Cannot validate after login");
            return false;
        }

        public static bool FindAndCheckReCaptchaCheckbox(this IWebDriver driver, int sleepTimeout)
        {
            var parentHandle = driver.CurrentWindowHandle;
            var tempCount = 0;

            while (sleepTimeout > tempCount)
            {
                try
                {
                    var frames = driver.FindElements(By.TagName("iframe"));
                    if (frames.Count > 0)
                    {
                        driver.Manage().Window.Size = new Size(1000, 800);
                        SleepRandomly();

                        driver.SwitchTo().Frame(0);
                        var recaptchElm = driver.FindElement(By.Id("recaptcha-anchor"));
                        recaptchElm.Click();

                        SleepRandomly();

                        driver.SwitchTo().Window(parentHandle);

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                var sleep = 100;
                tempCount += sleep;

                Thread.Sleep(sleep);
            }

            return false;
        }

        public static bool ValidateForm(this IWebDriver driver, string id, int sleepTimeout)
        {
            var tempCount = 0;

            while (sleepTimeout > tempCount)
            {
                try
                {
                    var formElm = driver.FindElement(By.Id(id));
                    if (formElm != null)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                var sleep = 100;
                tempCount += sleep;

                Thread.Sleep(sleep);
            }

            return false;
        }

        public static bool ValidateTitle(this IWebDriver driver, string title, int sleepTimeout)
        {
            var tempCount = 0;

            while (sleepTimeout > tempCount)
            {
                if (title.Equals(driver.Title, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                var sleep = 100;
                tempCount += sleep;

                Thread.Sleep(sleep);
            }

            return false;
        }
    }
}
