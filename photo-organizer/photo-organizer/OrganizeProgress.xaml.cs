using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using photo_organizer.Model;
using System.ComponentModel;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;

namespace photo_organizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrganizeProgress : Page, INotifyPropertyChanged
    {
        public PhotoMover Mover = new PhotoMover();
        Configuration configurationSettings;
        DispatcherTimer timer;
        DateTime startTime;
        int currentSecond;

        private string _duration;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Duration"));
            }
        }

        private string _photosPerSecond;
        public string PhotosPerSecond
        {
            get { return _photosPerSecond; }
            set
            {
                _photosPerSecond = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PhotosPerSecond"));
            }
        }

        private string _photosInFolders;
        public string PhotosInFolders
        {
            get { return _photosInFolders; }
            set
            {
                _photosInFolders = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PhotosInFolders"));
            }
        }


        private double _maxPhotosPerSecond = 0;
        public double MaxPhotosPerSecond
        {
            get { return _maxPhotosPerSecond; }
            set
            {
                _maxPhotosPerSecond = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxPhotosPerSecond"));
            }
        }

        public OrganizeProgress()
        {
            this.InitializeComponent();
        }

        void AddToStream (string message)
        {

        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            configurationSettings = e.Parameter as Model.Configuration;
            if (null != configurationSettings)
            {
                AddToStream("About to start photo organization complete");

                base.DataContext = Mover;

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;

                startTime = DateTime.Now;
                currentSecond = startTime.Second;

                timer.Start();

                RestartButton.IsEnabled = false;

                await Mover.Execute(configurationSettings.SourceFolder, configurationSettings.DestinationFolder, configurationSettings.MoveFilesToDestination);

                timer.Stop();

                await new MessageDialog("Completed").ShowAsync();
  
                AddToStream("Photo organization complete");
            }
            else
            {
                AddToStream("Invalid configuration, execution aborted");
            }
        }

        private async void Timer_Tick(object sender, object e)
        {
            DateTime tick = DateTime.Now;
            currentSecond = tick.Second;

            var tsDuration = tick - startTime;
            Duration = tsDuration.ToString("%m' min, '%s' sec.'");

            var photosPerSecond = Mover.PhotoCount / (double)tsDuration.TotalSeconds;
            MaxPhotosPerSecond = System.Math.Max(photosPerSecond, MaxPhotosPerSecond);

            PhotosPerSecond = string.Format("{0} ({1})", photosPerSecond.ToString("N2"), MaxPhotosPerSecond.ToString("N2"));

            PhotosPerSecondBar.Maximum = MaxPhotosPerSecond;
            PhotosPerSecondBar.Value = photosPerSecond;

            PhotosInFolders = string.Format("{0} photo's in {1} folders", Mover.PhotoCount, Mover.FolderCount);

            if (null != Mover.LastImageLocation)
                await ChangePreviewImage();
        }

        private async System.Threading.Tasks.Task ChangePreviewImage()
        {
            try
            {
                using (IRandomAccessStream fileStream = await Mover.LastImageLocation.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    await bitmapImage.SetSourceAsync(fileStream);
                    LastProcessedImage.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                AddToStream(ex.Message);
            }
        }

        private void PauzeButton_Click(object sender, RoutedEventArgs e)
        {
            Mover.CancelExecute = true;
            RestartButton.IsEnabled = true;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            Mover.CancelExecute = true;
            this.Frame.Navigate(typeof(ConfigureOrganize));
        }
    }
}