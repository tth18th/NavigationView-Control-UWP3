﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NavigationView_Control_UWP3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home",typeof(HomePage)),
            ("customer",typeof(Customers)),
            ("lotto",typeof(LottoPage)),
            ("euro",typeof(EuroPage))
        };


        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // This part allows for the addition of menu items in the cs code.
            NavView.MenuItems.Add(new NavigationViewItemSeparator());
            NavView.MenuItems.Add(new NavigationViewItem
            {
                Content = "My content",
                Icon = new SymbolIcon((Symbol)0xF1AD),
                Tag = "content"
            });
            _pages.Add(("content", typeof(MyPageContent)));
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;
            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate("home", new EntranceNavigationTransitionInfo());
            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);
            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
        }
        private void NavView_ItemInvoked(NavigationView sender,
         NavigationViewItemInvokedEventArgs args)
        {

            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate("settings",
               args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag,
               args.RecommendedNavigationTransitionInfo);

            }
        }
        /* **** NavView_SelectionChanged is not used in this example, but is shown for
        completeness. You will typically handle either ItemInvoked or SelectionChanged to
        perform navigation,but not both.*** */
        private void NavView_SelectionChanged(NavigationView sender,
         NavigationViewSelectionChangedEventArgs
        args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate("settings",
               args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag,
               args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo
       transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(SettingPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;
            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, null, transitionInfo);
            }
        }
       
        private void NavView_BackRequested(NavigationView sender,
         NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }
        private void BackInvoked(KeyboardAccelerator sender,
         KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }
        private bool On_BackRequested()
        {
            if (!ContentFrame.CanGoBack)
                return false;
            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
            (NavView.DisplayMode == NavigationViewDisplayMode.Compact ||
            NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;
            ContentFrame.GoBack();
            return true;
        }
        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;
            if (ContentFrame.SourcePageType == typeof(SettingPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a tag
                NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
                NavView.Header = "Settings";
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
                NavView.SelectedItem = NavView.MenuItems
                .OfType<NavigationViewItem>()
                .First(n => n.Tag.Equals(item.Tag));
                NavView.Header =
                ((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            }
        }
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
     



    }
}
