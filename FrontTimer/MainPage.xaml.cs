using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace FrontTimer
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Stopwatch stopwatch = new Stopwatch();
        private DispatcherTimer timer;
        private int countMaxSeconds = 60 * 30;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = new MediaElement();
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync("時間じゃよ");
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        private void Timer_Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                this.countMaxSeconds = (int)slider.Value * 60;
                TimeSpan previewTime = new TimeSpan(0, 0, this.countMaxSeconds);
                timerTextBlock.Text = previewTime.ToString(@"hh\:mm\:ss");
            }
        }


        private async void Start_Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            if (stopwatch.IsRunning)
            {
                // nomalモードへ
                bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);

                stopwatch.Stop();
                StartStopbutton.Content = "Start";
            }
            else
            {
                // frontモードへ
                bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);

                this.timer = new DispatcherTimer();
                this.timer.Tick += timer_Tick;
                this.timer.Interval = new TimeSpan(0, 0, 1);
                this.timer.Start();
                stopwatch.Start();
                StartStopbutton.Content = "Stop";
            }
        }

        private async void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Reset();
        }

        private void timer_Tick(object sender, object e)
        {
            int pastSeconds = (int)this.stopwatch.Elapsed.TotalSeconds;
            TimeSpan previewTime = new TimeSpan(0, 0, countMaxSeconds - pastSeconds);
            timerTextBlock.Text = previewTime.ToString(@"hh\:mm\:ss");
        }
    }
}
