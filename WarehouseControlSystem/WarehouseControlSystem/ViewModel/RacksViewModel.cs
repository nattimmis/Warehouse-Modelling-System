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
using WarehouseControlSystem.ViewModel.Base;
using WarehouseControlSystem.Model.NAV;
using WarehouseControlSystem.Model;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Plugin.Connectivity;
using WarehouseControlSystem.Helpers.Containers.StateContainer;
using WarehouseControlSystem.Helpers.NAV;
using WarehouseControlSystem.Resx;
using WarehouseControlSystem.View.Pages.RackScheme;
using System.Windows.Input;
using System.Threading;

namespace WarehouseControlSystem.ViewModel
{
    public class RacksViewModel : BaseViewModel
    {
        public Zone Zone { get; set; }

        public RackViewModel SelectedRackViewModel { get; set; }
        public ObservableCollection<RackViewModel> RackViewModels { get; set; }
        public ObservableCollection<RackViewModel> SelectedViewModels { get; set; }

        public RunModeEnum RunMode
        {
            get { return runmode; }
            set
            {
                if (runmode != value)
                {
                    runmode = value;
                    OnPropertyChanged("RunMode");
                }
            }
        } RunModeEnum runmode;

        public ICommand RackListCommand { protected set; get; }
        public ICommand NewRackCommand { protected set; get; }
        public ICommand EditRackCommand { protected set; get; }
        public ICommand DeleteRackCommand { protected set; get; }
        public ICommand ParamsCommand { protected set; get; }

        public int PlanHeight
        {
            get { return Zone.PlanHeight; }
            set
            {
                if (Zone.PlanHeight != value)
                {
                    Zone.PlanHeight = value;
                    OnPropertyChanged(nameof(PlanHeight));
                }
            }
        }
        public int PlanWidth
        {
            get { return Zone.PlanWidth; }
            set
            {
                if (Zone.PlanWidth != value)
                {
                    Zone.PlanWidth = value;
                    OnPropertyChanged(nameof(PlanWidth));
                }
            }
        }

        public int MinPlanHeight
        {
            get { return minheight; }
            set
            {
                if (minheight != value)
                {
                    minheight = value;
                    OnPropertyChanged(nameof(MinPlanHeight));
                }
            }
        } int minheight;
        public int MinPlanWidth
        {
            get { return minwidth; }
            set
            {
                if (minwidth != value)
                {
                    minwidth = value;
                    OnPropertyChanged(nameof(MinPlanWidth));
                }
            }
        } int minwidth;

        public bool IsSelectedList { get { return SelectedViewModels.Count > 0; } }

        public double ScreenWidth { get; set; }
        public double ScreenHeight { get; set; }

        public ObservableCollection<UserDefinedSelectionViewModel> UserDefinedSelectionViewModels { get; set; }
        public bool IsVisibleUDS
        {
            get { return isvisibleUDS; }
            set
            {
                if (isvisibleUDS != value)
                {
                    isvisibleUDS = value;
                    OnPropertyChanged(nameof(IsVisibleUDS));
                }
            }
        } bool isvisibleUDS;
        public int UDSPanelHeight
        {
            get { return udspanelheight; }
            set
            {
                if (udspanelheight != value)
                {
                    udspanelheight = value;
                    OnPropertyChanged(nameof(UDSPanelHeight));
                }
            }
        } int udspanelheight;

        public RacksViewModel(INavigation navigation, Zone zone) : base(navigation)
        {
            State = State.Normal;
            Zone = zone;
            RackViewModels = new ObservableCollection<RackViewModel>();
            SelectedViewModels = new ObservableCollection<RackViewModel>();
            UserDefinedSelectionViewModels = new ObservableCollection<UserDefinedSelectionViewModel>();

            RackListCommand = new Command(RackList);
            NewRackCommand = new Command(NewRack);
            EditRackCommand = new Command(EditRack);
            DeleteRackCommand = new Command(DeleteRack);
            ParamsCommand = new Command(Params);
        }

        public void ClearAll()
        {
            foreach (RackViewModel lvm in RackViewModels)
            {
                lvm.Dispose();
            };
            RackViewModels.Clear();
            SelectedViewModels.Clear();
            SelectedRackViewModel = null;
        }

        public async void Load()
        {
            if ((!CrossConnectivity.Current.IsConnected) || (Global.CurrentConnection == null))
            {
                State = State.NoInternet;
                return;
            }

            if (Zone.PlanHeight == 0)
                Zone.PlanHeight = Settings.DefaultZonePlanHeight;

            if (Zone.PlanWidth == 0)
                Zone.PlanWidth = Settings.DefaultZonePlanWidth;


            State = State.Loading;
            try
            {
                List<Rack> Racks = await NAV.GetRackList(Zone.LocationCode, Zone.Code, true, 1, int.MaxValue, ACD.Default);
                if ((!IsDisposed) && (Racks is List<Rack>))
                {
                    if (Racks.Count > 0)
                    {
                        RackViewModels.Clear();
                        SelectedViewModels.Clear();
                        State = State.Normal;
                        foreach (Rack rack in Racks)
                        {
                            RackViewModel rvm = new RackViewModel(Navigation, rack, false);
                            rvm.OnTap += Rvm_OnTap; ;
                            RackViewModels.Add(rvm);
                        }
                        ReDesign();
                        UpdateMinSizes();
                    }
                    else
                    {
                        State = State.NoData;
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Cancel Load", e.Message);
            }
            catch
            {
                State = State.Error;
                ErrorText = AppResources.Error_LoadRacks;
            }
        }

        public async void LoadAll()
        {
            State = State.Loading;
            try
            {
                List<Rack> racks = await NAV.GetRackList(Zone.LocationCode, Zone.Code, false, 1, int.MaxValue, ACD.Default);
                if ((!IsDisposed) && (racks is List<Rack>))
                {
                    if (racks.Count > 0)
                    {
                        State = State.Normal;
                        foreach (Rack rack in racks)
                        {
                            RackViewModel rvm = new RackViewModel(Navigation, rack, false);
                            RackViewModels.Add(rvm);
                        }
                    }
                    else
                    {
                        State = State.NoData;
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Cancel LoadAll", e.Message);
            }
            catch
            {
                State = State.Error;
                ErrorText = AppResources.Error_LoadRacksList;
            }
        }

        public async void LoadUDS()
        {
            try
            {
                UserDefinedSelectionViewModels.Clear();
                List<UserDefinedSelection> list = await NAV.LoadUDS(Zone.LocationCode, Zone.Code, ACD.Default);
                if (list is List<UserDefinedSelection>)
                {
                    foreach (UserDefinedSelection uds in list)
                    {
                        UserDefinedSelectionViewModel udsvm = new UserDefinedSelectionViewModel(Navigation, uds);
                        udsvm.UDSWidth = UDSPanelHeight;
                        udsvm.OnTap += RunUDS;
                        UserDefinedSelectionViewModels.Add(udsvm);
                    }
                }
                MessagingCenter.Send(this, "UDSListIsLoaded");
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Cancel LoadUDS", e.Message);
            }
            catch (Exception ex)
            {
                State = Helpers.Containers.StateContainer.State.Error;
                ErrorText = ex.ToString();
            }
        }

        private void Rvm_OnDblTap(RackViewModel obj)
        {
            //throw new NotImplementedException();
        }

        private async void Rvm_OnTap(RackViewModel rvm)
        {

            if (RunMode == RunModeEnum.View)
            {
                await Navigation.PushAsync(new RackCardPage(rvm));
            }
            else
            {
                foreach (RackViewModel rv in RackViewModels)
                {
                    if (rv != rvm)
                    {
                        rv.Selected = false;
                    }
                }
                rvm.Selected = !rvm.Selected;

                SelectedViewModels = new ObservableCollection<RackViewModel>(RackViewModels.ToList().FindAll(x => x.Selected == true));
            }
        }

        private async void RunUDS(UserDefinedSelectionViewModel udsvm)
        {
            udsvm.State = State.Loading;
            LoadAnimation = true;

            try
            {
                List<UserDefinedSelectionResult> list =  await NAV.RunUDS(Zone.LocationCode, Zone.Code, udsvm.ID, ACD.Default);
                if (list is List<UserDefinedSelectionResult>)
                {
                    foreach (RackViewModel rvm in RackViewModels)
                    {
                        rvm.UDSSelects.RemoveAll(x => x.FunctionID == udsvm.ID);
                    }

                    foreach (UserDefinedSelectionResult udsr in list)
                    {
                        RackViewModel rvm =  RackViewModels.ToList().Find(x => x.No == udsr.RackNo);
                        if (rvm is RackViewModel)
                        {
                            SubSchemeSelect sss = new SubSchemeSelect();
                            sss.FunctionID = udsr.FunctionID;
                            sss.Section = udsr.Section;
                            sss.Level = udsr.Level;
                            sss.Depth = udsr.Depth;
                            sss.Value = udsr.Value;
                            sss.HexColor = udsr.HexColor;
                            rvm.UDSSelects.Add(sss);
                        }
                    }
                }
                MessagingCenter.Send(this, "UDSRunIsDone");
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Cancel RunUDS", e.Message);
            }
            catch
            {
            }
            udsvm.State = State.Normal;
            LoadAnimation = false;
        }

        public void ReDesign()
        {
            double widthstep = (ScreenWidth / PlanWidth);
            double heightstep = (ScreenHeight / PlanHeight);
            foreach (RackViewModel rvm in RackViewModels)
            {
                rvm.Left = rvm.Rack.Left * widthstep;
                rvm.Top = rvm.Rack.Top * heightstep;
                rvm.Width = rvm.Rack.Width * widthstep;
                rvm.Height = rvm.Rack.Height * heightstep;
            }
            MessagingCenter.Send(this, "Rebuild");
        }

        public void UnSelectAll()
        {
            foreach (RackViewModel rvm in RackViewModels)
            {
                rvm.Selected = false;
            }
            SelectedViewModels.Clear();
        }

        public async void RackList()
        {
            RackListPage rlp = new RackListPage(Zone);
            await Navigation.PushAsync(rlp);
        }

        public async void NewRack()
        {
            Rack newrack = new Rack();
            newrack.Sections = Settings.DefaultRackSections;
            newrack.Levels = Settings.DefaultRackLevels;
            newrack.Depth = Settings.DefaultRackDepth;
            newrack.SchemeVisible = true;
            RackViewModel rvm = new RackViewModel(Navigation, newrack, true);
            rvm.LocationCode = Zone.LocationCode;
            rvm.ZoneCode = Zone.Code;
            rvm.CanChangeLocationAndZone = false;
            RackNewPage rnp = new RackNewPage(rvm);
            await Navigation.PushAsync(rnp);
        }

        public void EditRack(object obj)
        {

        }

        public void DeleteRack(object obj)
        {

        }

        public async void Params()
        {
            RacksFieldParamsPage rfpp = new RacksFieldParamsPage(this);
            await Navigation.PushAsync(rfpp);
        }

        public async void SaveZoneParams()
        {
            await NAV.ModifyZone(Zone, ACD.Default);
        }

        public async void SaveRacksChangesAsync()
        {
            List<RackViewModel> list = RackViewModels.ToList().FindAll(x => x.Selected == true);
            foreach (RackViewModel rvm in list)
            {
                try
                {
                    await NAV.ModifyRack(rvm.Rack, ACD.Default);
                }
                catch { }
            }
        }

        public Task<string> SaveRacksVisible()
        {
            var tcs = new TaskCompletionSource<string>();
            string rv = "";
            Task.Run(async () =>
            {
                try
                {
                    List<RackViewModel> list = RackViewModels.ToList().FindAll(x => x.Changed == true);
                    foreach (RackViewModel rvm in list)
                    {
                        Rack rack = new Rack();
                        rvm.SaveFields(rack);
                        int i = await NAV.SetRackVisible(rack, ACD.Default).ConfigureAwait(false);
                    }
                    tcs.SetResult(rv);
                }
                catch
                {
                    tcs.SetResult(rv);
                }
            });
            return tcs.Task;
        }

        public void UpdateMinSizes()
        {
            foreach (RackViewModel rvm in RackViewModels)
            {
                if ((rvm.Rack.Left + rvm.Rack.Width) > MinPlanWidth)
                {
                    MinPlanWidth = rvm.Rack.Left + rvm.Rack.Width;
                }
                if ((rvm.Rack.Top + rvm.Rack.Height) > MinPlanHeight)
                {
                    MinPlanHeight = rvm.Rack.Top + rvm.Rack.Height;
                }
            }
        }

        public void CancelAsync()
        {
            ACD.CancelAll();
        }

        public override void Dispose()
        {
            Zone = null;
            ClearAll();
            UserDefinedSelectionViewModels.Clear();
            RackListCommand = null;
            NewRackCommand = null;
            EditRackCommand = null;
            DeleteRackCommand = null;
            ParamsCommand = null;
            base.Dispose();
        }
    }
}