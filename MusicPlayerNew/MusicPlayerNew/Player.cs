using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace MusicPlayerNew
{
    public class Player
    {
        private readonly WebBrowser _browser;
        private readonly Action<Info> _updateCallback;

        public Player(WebBrowser browser, Action<Info> updateCallback)
        {
            _browser = browser;
            _updateCallback = updateCallback;

            _browser.Source = new Uri("https://musik.telmore.dk/playlister");
            _browser.LoadCompleted += UpdateBrowser;

        }

        public void PlayPause()
        {
            Run("play");
        }

        public void Previous()
        {
            Run("previous");
        }

        public void Next()
        {
            Run("next");
        }

        private void Run(string name, params object[] args)
        {
            _browser.InvokeScript(name, args);
        }

        private void UpdateBrowser(object sender, NavigationEventArgs e)
        {
            var jsCode = File.ReadAllText("script.js");
            _browser.InvokeScript("execScript", jsCode, "JavaScript");

            var timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle)
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            timer.Tick += GetInfo;
            timer.Start();
        }

        private void GetInfo(object sender, EventArgs e)
        {
            var info = Run<Info>("getInfo");

            if (info?.Cover == null)
            {
                return;
            }

            _updateCallback(info);
        }

        private T Run<T>(string name) where T : new()
        {
            try
            {
                var result = _browser.InvokeScript(name).ToString();
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception e)
            {
            }

            return new T();
        }
    }
}
