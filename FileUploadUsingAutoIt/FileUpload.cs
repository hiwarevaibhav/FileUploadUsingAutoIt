using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FileUpload
{
    public class FileUpload
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            try
            {
                //chromedriver.exe Path
                string path = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\Drivers\"); 
                driver = new ChromeDriver(path);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        [Test]
        public void FileUploadWithNonStandardHTMLTest()
        {
            driver.Url = "https://smallpdf.com/pdf-to-word";
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.FindElement(By.XPath("//span[text()='Choose Files']")).Click();

            string fileToBeUploadedPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\ToBeUploadedFiles\samplefile.pdf");
            string autoItScriptPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\AutoItScripts\FileUploadExample.au3");
            string autoItExecutablePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\AutoItScripts\FileUploadExample.exe");
            string cmdToGenerateAutoItExecutable = "Aut2exe.exe /in " + autoItScriptPath + " /out " + autoItExecutablePath + "";

            try
            {
                //Change directory location of cmd to navigate C:\Program Files (x86)\AutoIt3\Aut2Exe silently
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.Start();
                process.StandardInput.WriteLine(@"cd C:\Program Files (x86)\AutoIt3\Aut2Exe");
                process.StandardInput.WriteLine(cmdToGenerateAutoItExecutable);
                Thread.Sleep(3000);

                //Execute fileupload operation using AutoIt runtime created FileUploadExample.exe
                Process.Start(autoItExecutablePath, fileToBeUploadedPath);

                //Verify fileuploaded                
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[text()='samplefile.pdf']")));
                Assert.IsTrue(driver.FindElement(By.XPath("//div[text()='samplefile.pdf']")).Displayed, "File not uploaded");
            }
            finally
            {
                if (File.Exists(autoItExecutablePath))
                {
                    File.Delete(autoItExecutablePath);
                }                
            }
        }

        [TearDown]
        public void TearDown()
        {
            driver.Close();
        }
    }
}