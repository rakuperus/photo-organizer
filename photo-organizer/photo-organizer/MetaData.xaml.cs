using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class MetaData : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _metaDataFolder;
        public string MetaDataFolder
        {
            get { return _metaDataFolder; }
            set
            {
                _metaDataFolder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MetaDataFolder"));
            }
        }

        private string _metaData;

        public string MetadataContent
        {
            get { return _metaData; }
            set
            {
                _metaData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MetadataContent"));
            }
        }

        public MetaData()
        {
            this.InitializeComponent();
        }

        private void MetaDataGo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MetaDataFolder_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
