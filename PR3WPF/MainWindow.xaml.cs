using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PR3WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartSliderThread();
        }
//------------------------------------------переменные----------------------------
        private List<string> music = new List<string>();
        private bool isRepeatEnabled = false;
        private bool isPlaying = false;
        private int currentTrackIndex = 0;
        private Thread sliderThread;

//------------------------------------------кнопки мэйн--------------------------------
        private void Choise_Click(object sender, RoutedEventArgs e)
        {
            if (music.Count == 0)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.Multiselect = true;
                dialog.Filters.Add(new CommonFileDialogFilter("MP3 Files", "*.mp3"));//mp4 записей не было

                CommonFileDialogResult result = dialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok)
                {
                    music.Clear();
                    FileListBox.Items.Clear();

                    foreach (string fileName in dialog.FileNames)
                    {
                        music.Add(fileName);
                        FileListBox.Items.Add(fileName);
                    }

                    if (music.Count > 0)
                    {
                        currentTrackIndex = 0;
                        media.Source = new Uri(music[currentTrackIndex]);
                        media.Play();
                        media.MediaEnded += Media_MediaEnded;
                    }
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex < music.Count - 1)
            {
                currentTrackIndex++;
                media.Source = new Uri(music[currentTrackIndex]);
                media.Play();
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            ManagePlayback();
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex > 0)
            {
                currentTrackIndex--;
                media.Source = new Uri(music[currentTrackIndex]);
                media.Play();
            }
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            if (!isRepeatEnabled)
            {
                isRepeatEnabled = true;
                media.MediaEnded -= Media_MediaEnded;
                media.MediaEnded += Repeat_MediaEnded;
            }
            else
            {
                isRepeatEnabled = false;
                media.MediaEnded -= Repeat_MediaEnded;
                media.MediaEnded += Media_MediaEnded;
            }
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {

        }



//----------------------------------------кнопка истории--------------------
        private void History_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow(music);
            if (historyWindow.ShowDialog() == true)
            {
                int selectedTrackIndex = historyWindow.i;
                if (selectedTrackIndex >= 0 && selectedTrackIndex < music.Count)
                {
                    currentTrackIndex = selectedTrackIndex;
                    media.Source = new Uri(music[currentTrackIndex]);
                    media.Play();
                }
            }
        }
//--------------------------------------дополнительные методы (не все же в событиях писать)----------------
        private void ManagePlayback()
        {
            if (media.Position == TimeSpan.Zero || media.Position == media.NaturalDuration.TimeSpan)
            {
                media.Play();
                ConfigureMediaEndedHandler();
            }
            else
            {
                if (media.CanPause)
                {
                    media.Pause();
                }
            }
        }

        private void ConfigureMediaEndedHandler()
        {
            media.MediaEnded -= Media_MediaEnded;
            media.MediaEnded += Media_MediaEnded;
        }


        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            PlayNextSong();
        }

        private void Repeat_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (isRepeatEnabled)
            {
                media.Position = TimeSpan.Zero;
                media.Play();
            }
        }

        private void PlayNextSong()
        {
            currentTrackIndex++;
            if (currentTrackIndex < music.Count)
            {
                media.Source = new Uri(music[currentTrackIndex]);
                media.Play();
            }
        }

        private void StartSliderThread()
        {
            sliderThread = new Thread(new ThreadStart(UpdateSliderPosition));
            sliderThread.IsBackground = true;
            sliderThread.Start();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (media.NaturalDuration.HasTimeSpan)
            {
                double newPosition = media.NaturalDuration.TimeSpan.TotalSeconds * (Slider.Value / Slider.Maximum);
                media.Position = TimeSpan.FromSeconds(newPosition);
            }
        }
        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (media != null)
            {
                media.Volume = Volume.Value;
            }
        }

        private async void UpdateSliderPosition()
        {
            /*while (true)
            {
                if (media.NaturalDuration.HasTimeSpan)
                {
                    double newPosition = media.Position.TotalSeconds / media.NaturalDuration.TimeSpan.TotalSeconds * Slider.Maximum;

                    await Task.Run(() =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Slider.Value = newPosition;
                        });
                    });
                }
                await Task.Delay(500);
            }*/
        }
    }
}