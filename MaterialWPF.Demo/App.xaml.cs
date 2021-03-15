﻿// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// Copyright (C) Leszek Pomianowski and MaterialWPF Contributors.
// All Rights Reserved.

using System;
using System.Windows;

namespace MaterialWPF.Demo
{
    public enum Theme
    {
        Light,
        Dark
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Theme CurrentTheme = Theme.Dark;

        public void SwitchTheme(Theme theme)
        {
            //ATTENTION! This code only works if the resources formula is the same as in App.xaml.
            Resources.MergedDictionaries[0] = new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialWPF;component/Styles/Theme" + (theme == Theme.Light ? "Light" : "Dark") + ".xaml", UriKind.RelativeOrAbsolute) };
        }
    }
}