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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace photo_organizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrganizeProgress : Page
    {
        Model.PhotoMover Mover = new Model.PhotoMover();
        Model.Configuration configurationSettings;
        DispatcherTimer timer;
        DateTime startTime;
        int currentSecond;

        public string Duration
        {
            get;set;
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

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;

                startTime = DateTime.Now;
                currentSecond = startTime.Second;

                timer.Start();

                await Mover.Execute(configurationSettings.SourceFolder, configurationSettings.DestinationFolder, configurationSettings.MoveFilesToDestination);

                timer.Stop();
  
                AddToStream("Photo organization complete");
            }
            else
            {
                AddToStream("Invalid configuration, execution aborted");
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            throw new NotImplementedException();
        }
    }
}