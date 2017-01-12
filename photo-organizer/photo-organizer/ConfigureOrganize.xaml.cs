using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
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
    public sealed partial class ConfigureOrganize : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Model.Configuration configurationSettings = new Model.Configuration();

        public string SourceFolder { get; set; }
        public string DestinationFolder { get; set; }
        public bool? MoveFiles { get; set; } = false;

        public ConfigureOrganize()
        {
            this.InitializeComponent();
        }

        private async Task<StorageFolder> SelectImageFolder(string pickerTitle)
        {
            FolderPicker picker = new FolderPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.CommitButtonText = pickerTitle;
            picker.FileTypeFilter.Add(".jpg");

            return await picker.PickSingleFolderAsync();
        }

        private async void SelectSource_Click(object sender, RoutedEventArgs e)
        {
            configurationSettings.SourceFolder = await SelectImageFolder("Select source location");

            SourceFolder = (null != configurationSettings.SourceFolder) ? configurationSettings.SourceFolder.Path : string.Empty;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceFolder"));
        }

        private async void SelectDestination_Click(object sender, RoutedEventArgs e)
        {
            configurationSettings.DestinationFolder = await SelectImageFolder("Select destination location");

            DestinationFolder = (null != configurationSettings.DestinationFolder) ? configurationSettings.DestinationFolder.Path : string.Empty;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DestinationFolder"));
        }

        private async void Go_Click(object sender, RoutedEventArgs e)
        {
            if (null == configurationSettings.SourceFolder)
            {
                await new MessageDialog("Select a source folder").ShowAsync();
                return;
            }

            if (null == configurationSettings.DestinationFolder)
            {
                await new MessageDialog("Select a detination folder").ShowAsync();
                return;
            }

            if (configurationSettings.SourceFolder == configurationSettings.DestinationFolder)
            {
                await new MessageDialog("Source and destination cannot be the same").ShowAsync();
                return;
            }

            configurationSettings.MoveFilesToDestination = (MoveFiles == null) ? false : (bool)MoveFiles;

            this.Frame.Navigate(typeof(OrganizeProgress), configurationSettings);
        }
    }
}
