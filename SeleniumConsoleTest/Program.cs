using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IWebDriver driver = new ChromeDriver(@"D:\Wip\Practices\Github\PocSeleniumConsole\packages\chromedriver_win32"))
            {
                driver.Navigate().GoToUrl("https://www.google.com/");
                Thread.Sleep(2000);

                IWebElement ele = driver.FindElement(By.Name("q"));
                ele.SendKeys("javatpoint tutorials");
                Thread.Sleep(2000);

                IWebElement ele1 = driver.FindElement(By.Name("btnK"));
                ele1.Click();

                Thread.Sleep(3000);

            }
        }
    }
}
