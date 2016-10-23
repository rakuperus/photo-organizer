using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Organize : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string SourceFolder { get; set; }
        public string DestinationFolder { get; set; }
        public bool? MoveFiles { get; set; } = false;

        public Organize()
        {
            this.InitializeComponent();
        }

        private async Task<string> SelectImageFolder(string pickerTitle)
        {
            FolderPicker picker = new FolderPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.CommitButtonText = "Set source location";
            picker.FileTypeFilter.Add(".jpg");

            var folder = await picker.PickSingleFolderAsync();
            if (null != folder)
            {
                return folder.Path;
            }

            return null;
        }

        private async void SelectSource_Click(object sender, RoutedEventArgs e)
        {
            SourceFolder = await SelectImageFolder("Set source location");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SourceFolder"));
        }

        private async void SelectDestination_Click(object sender, RoutedEventArgs e)
        {
            DestinationFolder = await SelectImageFolder("Set destination location");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DestinationFolder"));
        }

        private async void Go_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SourceFolder))
            {
                await new MessageDialog("Select a source folder").ShowAsync();
                return;
            }

            if (string.IsNullOrWhiteSpace(DestinationFolder))
            {
                await new MessageDialog("Select a detination folder").ShowAsync();
                return;
            }

            if (string.Compare(SourceFolder, DestinationFolder) == 0)
            {
                await new MessageDialog("Source and destination cannot be the same").ShowAsync();
                return;
            }

            MoveFiles = MoveFiles == null ? false : (bool)MoveFiles;

            this.Frame.Navigate(typeof(Progress));
        }
    }
}
