﻿// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) Leszek Pomianowski and MaterialWPF Contributors.
// All Rights Reserved.

using MaterialWPF.UI;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MaterialWPF.Controls
{
    /// <summary>
    /// Interaction logic for Navigation.xaml
    /// </summary>
    public partial class Navigation : UserControl
    {
        public static readonly DependencyProperty
            MinBarWidthProperty = DependencyProperty.Register("MinBarWidth", typeof(int?), typeof(Navigation), new PropertyMetadata(44)),
            MaxBarWidthProperty = DependencyProperty.Register("MaxBarWidth", typeof(int?), typeof(Navigation), new PropertyMetadata(220));

        /// <summary>
        /// Gets or sets the list of <see cref="NavItem"/> that will be displayed on the menu.
        /// </summary>
        public ObservableCollection<NavItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the minimum width of the collapsed menu.
        /// </summary>
        public int MinBarWidth
        {
            get => (int)(this.GetValue(MinBarWidthProperty) as int?);
            set => this.SetValue(MinBarWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum width of the expanded menu.
        /// </summary>
        public int MaxBarWidth
        {
            get => (int)(this.GetValue(MaxBarWidthProperty) as int?);
            set => this.SetValue(MaxBarWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets absolute Namespace in the application files where the <see cref="System.Windows.Controls.Page"/> are located. <c>eg.: MyApp.Views.Pages</c>
        /// </summary>
        public string Catalog
        {
            get { return this._pagesFolder; }
            set { this._pagesFolder = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Controls.Frame"/> in which the <see cref="System.Windows.Controls.Page"/> will be loaded after navigation.
        /// </summary>
        public Frame Frame
        {
            get { return this._rootFrame; }
            set { this._rootFrame = value; }
        }

        /// <summary>
        /// Gets currently active <see cref="System.Windows.Controls.Page"/> tag.
        /// </summary>
        public string PageNow => this._currentPage;

        protected DoubleAnimation _navExpander;
        protected Frame _rootFrame;
        
        protected string 
            _currentPage = string.Empty,
            _pagesFolder = string.Empty;

        protected bool
            _navExpanded = false,
            _isLoading = false,
            _licenseAccepted = false;

        public Navigation()
        {
            InitializeComponent();
        }

        public void InitializeNavigation(string navigate = "", string activepage = "")
        {
            if (this.Items == null)
                return;

            if (!String.IsNullOrEmpty(navigate))
                this.Navigate(navigate, true);

            if (!String.IsNullOrEmpty(activepage))
                for (int i = 0; i < this.Items.Count; i++)
                    if (this.Items[i].Tag == activepage)
                        this.Items[i].IsActive = true;
        }

        /// <summary>
        /// Loads a <see cref="System.Windows.Controls.Page"/> instance into <see cref="Navigation.Frame"/> based on the <see cref="NavItem.Tag"/>.
        /// </summary>
        public void Navigate(string pageTypeName, bool refresh = false)
        {
            if (this.Items == null || this._rootFrame == null)
                return;

            if (pageTypeName == this._currentPage)
                return;

            for (int i = 0; i < this.Items.Count; i++)
                if (this.Items[i].Tag == pageTypeName)
                {
                    if (this.Items[i].Instance == null || refresh)
                    {
                        if(this.Items[i].Type != null)
                        {
                            this.Items[i].Instance = Activator.CreateInstance(this.Items[i].Type);
                        }
                        else if (this.Items[i].Type == null && !string.IsNullOrEmpty(this._pagesFolder))
                        {
                            //We assume that we will always enter the correct name
                            Type pageType = Type.GetType(this._pagesFolder + pageTypeName);

                            if (!refresh && this._rootFrame.Content != null && this._rootFrame.Content.GetType() == pageType)
                                return;

                            this.Items[i].Instance = Activator.CreateInstance(pageType);
                        }
                        else
                        {
                            //Brake!
                            return;
                        }
                    }

                    this._rootFrame.Navigate(this.Items[i].Instance);
                    this.Items[i].IsActive = true;
                }
                else
                {
                    this.Items[i].IsActive = false;
                }

            this._currentPage = pageTypeName;
        }

        private void ToggleNavigation()
        {
            if (this._navExpander == null)
                this._navExpander = new DoubleAnimation()
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                    Duration = TimeSpan.FromSeconds(0.4)
                };

            this._navExpander.From = navigationList.ActualWidth;
            this._navExpander.To = this._navExpanded ? MinBarWidth : MaxBarWidth;

            navigationList.BeginAnimation(ItemsControl.WidthProperty, this._navExpander);
            this._navExpanded = !this._navExpanded;
        }

        private void Button_Hamburger(object sender, RoutedEventArgs e)
        {
            this.ToggleNavigation();
        }

        private void Button_NavItem(object sender, RoutedEventArgs e)
        {
            this.Navigate((sender as Button).Tag.ToString());
        }
    }
}
