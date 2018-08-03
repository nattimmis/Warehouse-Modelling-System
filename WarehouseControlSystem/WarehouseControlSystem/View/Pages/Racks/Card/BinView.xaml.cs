﻿// ----------------------------------------------------------------------------------
// Copyright © 2018, Oleg Lobakov, Contacts: <oleg.lobakov@gmail.com>
// Licensed under the GNU GENERAL PUBLIC LICENSE, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// https://github.com/OlegLobakov/WarehouseControlSystem/blob/master/LICENSE
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WarehouseControlSystem.Model;
using WarehouseControlSystem.ViewModel;

namespace WarehouseControlSystem.View.Pages.Racks.Card
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BinView : ContentView
    {
        public BinViewModel Model { get; set; }
        public double CodeFontSize
        {
            get { return codefontsize; }
            set
            {
                if (codefontsize != value)
                {
                    codefontsize = value;
                    OnPropertyChanged(nameof(CodeFontSize));
                }
            }
        }
        double codefontsize;

        public double ValuesFontSize
        {
            get { return valuesfontsize; }
            set
            {
                if (valuesfontsize != value)
                {
                    valuesfontsize = value;
                    OnPropertyChanged(nameof(ValuesFontSize));
                }
            }
        }
        double valuesfontsize;

        public BinView(BinViewModel bvm)
        {
            Model = bvm;
            BindingContext = Model;
            InitializeComponent();
        }

        private void StackLayout_SizeChanged(object sender, EventArgs e)
        {
            StackLayout sl = (StackLayout)sender;
            CodeFontSize = sl.Width / 5;
            ValuesFontSize = sl.Width / 9;
        }
    }
}