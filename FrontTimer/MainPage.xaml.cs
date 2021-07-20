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
        private int countMaxSeconds = 60 * 10;
        private MediaElement mediaElement = new MediaElement();
        private int hour = 0;
        private int minute = 10;
        private int second = 0;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Hour_Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider_h = sender as Slider;
            if (slider_h != null)
            {
                this.hour = (int)slider_h.Value * 60 * 60;
                this.countMaxSeconds = this.hour + this.minute + this.second;
                TimeSpan previewTime = new TimeSpan(0, 0, this.countMaxSeconds);
                timerTextBlock.Text = previewTime.ToString(@"hh\:mm\:ss");
            }
        }

        private void Minute_Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider_m = sender as Slider;
            if (slider_m != null)
            {
                this.minute = (int)slider_m.Value * 60;
                this.countMaxSeconds = this.hour + this.minute + this.second;
                TimeSpan previewTime = new TimeSpan(0, 0, this.countMaxSeconds);
                timerTextBlock.Text = previewTime.ToString(@"hh\:mm\:ss");
            }
        }

        private void Second_Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider slider_s = sender as Slider;
            if (slider_s != null)
            {
                this.second = (int)slider_s.Value;
                this.countMaxSeconds = this.hour + this.minute + this.second;
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
                timerTextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x33, 0x95, 0xF7));
                Resetbutton.IsEnabled = true;
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
                Resetbutton.IsEnabled = false;
            }
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Reset();
        }

        private async void timer_Tick(object sender, object e)
        {
            int pastSeconds = (int)this.stopwatch.Elapsed.TotalSeconds;
            int remainingTime = countMaxSeconds - pastSeconds;
            if (remainingTime >= 0)
            {
                TimeSpan previewTime = new TimeSpan(0, 0, remainingTime);
                timerTextBlock.Text = previewTime.ToString(@"hh\:mm\:ss");
            }
            else if (stopwatch.IsRunning)
            {
                var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
                Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync("時間じゃよ");
                mediaElement.SetSource(stream, stream.ContentType);
                mediaElement.Play();
                timerTextBlock.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            }
        }
    }
}
