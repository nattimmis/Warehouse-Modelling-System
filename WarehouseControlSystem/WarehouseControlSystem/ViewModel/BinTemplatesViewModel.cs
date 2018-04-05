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
using WarehouseControlSystem.Model;
using WarehouseControlSystem.Model.NAV;
using WarehouseControlSystem.Resx;
using WarehouseControlSystem.ViewModel.Base;
using WarehouseControlSystem.Helpers.NAV;
using WarehouseControlSystem.Helpers.Containers.StateContainer;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.ObjectModel;
using WarehouseControlSystem.View.Pages.BinTemplate;
using System.Threading;

namespace WarehouseControlSystem.ViewModel
{
    public class BinTemplatesViewModel : BaseViewModel
    {
        public BinTemplateViewModel SelectedTemplate { get; set; }
        public ObservableCollection<BinTemplateViewModel> BinTemplates { get; set; }

        public ICommand NewCommand { protected set; get; }
        public ICommand DeleteCommand { protected set; get; }
        public ICommand EditCommand { protected set; get; }
        public ICommand CopyCommand { protected set; get; }

        public BinTemplatesViewModel(INavigation navigation) : base(navigation)
        {
            BinTemplates = new ObservableCollection<BinTemplateViewModel>();
            NewCommand = new Command(NewTemplate);
            DeleteCommand = new Command<object>(DeleteTemplate);
            EditCommand = new Command<object>(EditTemplate);
            CopyCommand = new Command<object>(CopyTemplate);
        }

        public async void Load()
        {
            State = State.Loading;
            try
            {
                List<BinTemplate> bintemplates = await NAV.GetBinTemplateList(1, int.MaxValue, ACD.Default);
                if ((!IsDisposed) && (bintemplates is List<BinTemplate>))
                {
                    BinTemplates.Clear();
                    foreach (BinTemplate bt in bintemplates)
                    {
                        BinTemplateViewModel btvm = new BinTemplateViewModel(Navigation, bt);
                        BinTemplates.Add(btvm);
                    }
                    State = State.Normal;
                }
            }
            catch (OperationCanceledException ex)
            {
                ErrorText = ex.Message;
            }
            catch 
            {
                State = State.Error;
                ErrorText = AppResources.Error_LoadBinTemplates;
            }
        }

        public async void NewTemplate()
        {
            BinTemplate newbt = new BinTemplate();
            BinTemplateViewModel btvm = new BinTemplateViewModel(Navigation, newbt);
            btvm.CreateMode = true;
            NewBinTemplatePage nbtp = new NewBinTemplatePage(btvm);
            await Navigation.PushAsync(nbtp);
        }

        public async void DeleteTemplate(object sender)
        {
            BinTemplateViewModel btvm = (BinTemplateViewModel)sender;

            State = State.Loading;
            try
            {
                await NAV.DeleteBinTemplate(btvm.BinTemplate, ACD.Default);
                if (!IsDisposed)
                {
                    BinTemplates.Remove(btvm);
                    SelectedTemplate = null;
                    State = State.Normal;
                }
            }
            catch (Exception ex)
            {
                State = State.Error;
                ErrorText = ex.Message;
            }
        }

        public void EditTemplate(object sender)
        {
            BinTemplateViewModel btvm = (BinTemplateViewModel)sender;
            btvm.CreateMode = false;
            NewBinTemplatePage nbtp = new NewBinTemplatePage(btvm);
        }

        public void CopyTemplate(object sender)
        {


        }

        public override void Dispose()
        {
            SelectedTemplate = null;
            foreach (BinTemplateViewModel bt in BinTemplates)
            {
                bt.Dispose();
            }
            BinTemplates.Clear();
            BinTemplates = null;

            NewCommand = null;
            DeleteCommand = null;
            EditCommand = null;
            CopyCommand = null;
            base.Dispose();
        }
    }
}