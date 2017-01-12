using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace photo_organizer
{
    /// <summary>
    /// This is the application host page, it is responsible for the main navigation and the main conten frame
    /// </summary>
    public sealed partial class ApplicationHost : Page
    {
        public ApplicationHost()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            HamburgerMenu.SelectedIndex = 0;
        }

        private void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e)
        {
            HamburgerMenuGlyphItem item = e.ClickedItem as HamburgerMenuGlyphItem;
            if (null != item)
            {
                Header.Text = item.Label;
                RootFrame.Navigate(item.TargetPageType);
            }
        }

        private async void HamburgerMenu_OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {
            if (Resources.Keys.Contains("AboutMessage"))
            {
                await new MessageDialog(Resources["AboutMessage"].ToString()).ShowAsync();
            }
        }
    }
}
