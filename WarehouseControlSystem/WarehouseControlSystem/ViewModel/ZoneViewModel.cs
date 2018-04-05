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

using WarehouseControlSystem.Resx;
using WarehouseControlSystem.ViewModel.Base;
using WarehouseControlSystem.Helpers.Containers.StateContainer;
using Xamarin.Forms;
using System.Windows.Input;
using WarehouseControlSystem.Model;
using WarehouseControlSystem.Model.NAV;
using WarehouseControlSystem.Helpers.NAV;
using System.Collections.ObjectModel;
using System.Threading;

namespace WarehouseControlSystem.ViewModel
{
    public class ZoneViewModel : BaseViewModel
    {
        public Location Location { get; set; }
        public Zone Zone { get; set; }

        public string LocationCode
        {
            get { return locationcode; }
            set
            {
                if (locationcode != value)
                {
                    locationcode = value;
                    Changed = true;
                    OnPropertyChanged(nameof(LocationCode));
                }
            }
        } string locationcode;
        public string Code
        {
            get { return code; }
            set
            {
                if (code != value)
                {
                    code = value;
                    Changed = true;
                    OnPropertyChanged(nameof(Code));
                }
            }
        } string code;
        public string CodeWarningText
        {
            get { return codewarningtext; }
            set
            {
                if (codewarningtext != value)
                {
                    codewarningtext = value;
                    OnPropertyChanged(nameof(CodeWarningText));
                }
            }
        } string codewarningtext;

        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    Changed = true;
                    OnPropertyChanged(nameof(Description));
                }
            }
        } string description;
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    Changed = true;
                    OnPropertyChanged(nameof(Color));
                }
            }
        } Color color;

        public string BinTypeCode
        {
            get { return bintypecode; }
            set
            {
                if (bintypecode != value)
                {
                    bintypecode = value;
                    Changed = true;
                    OnPropertyChanged(nameof(BinTypeCode));
                }
            }
        }
        string bintypecode;

        public SchemeElementEditMode EditMode
        {
            get { return editmode; }
            set
            {
                if (editmode != value)
                {
                    editmode = value;
                    switch (editmode)
                    {
                        case SchemeElementEditMode.None:
                            EditModeText = "";
                            break;

                        case SchemeElementEditMode.Move:
                            EditModeText = AppResources.ZoneView_EditMode1;
                            break;

                        case SchemeElementEditMode.Resize:
                            EditModeText = AppResources.ZoneView_EditMode2;
                            break;
                    }
                    Changed = true;
                    OnPropertyChanged("EditMode");
                }
            }
        } SchemeElementEditMode editmode;
        public bool SchemeVisible
        {
            get { return schemevisible; }
            set
            {
                if (schemevisible != value)
                {
                    schemevisible = value;
                    Changed = true;
                    OnPropertyChanged(nameof(SchemeVisible));
                }
            }
        } bool schemevisible;
        public int RackQuantity
        {
            get { return rackquantity; }
            set
            {
                if (rackquantity != value)
                {
                    rackquantity = value;
                    OnPropertyChanged(nameof(RackQuantity));
                }
            }
        } int rackquantity;
        public int BinQuantity
        {
            get { return binquantity; }
            set
            {
                if (binquantity != value)
                {
                    binquantity = value;
                    OnPropertyChanged(nameof(BinQuantity));
                }
            }
        } int binquantity;

        public string EditModeText
        {
            get { return editmodetext; }
            set
            {
                if (editmodetext != value)
                {
                    editmodetext = value;
                    OnPropertyChanged(nameof(EditModeText));
                }
            }
        } string editmodetext = "";
        public bool CreateMode
        {
            get { return createmode; }
            set
            {
                if (createmode != value)
                {
                    createmode = value;
                    OnPropertyChanged(nameof(CreateMode));
                }
            }
        } bool createmode;

        public double PrevWidth { get; set; }
        public double PrevHeight { get; set; }

        public int PlanWidth
        {
            get { return planwidth; }
            set
            {
                if (planwidth != value)
                {
                    planwidth = value;
                    OnPropertyChanged(nameof(PlanWidth));
                }
            }
        } int planwidth;
        public int PlanHeight
        {
            get { return planheight; }
            set
            {
                if (planheight != value)
                {
                    planheight = value;
                    OnPropertyChanged(nameof(PlanHeight));
                }
            }
        } int planheight;

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public ICommand TapCommand { protected set; get; }
        public event Action<ZoneViewModel> OnTap;

        public bool Changed
        {
            get { return changed; }
            set
            {
                if (changed != value)
                {
                    changed = value;
                    OnPropertyChanged(nameof(Changed));
                }
            }
        } bool changed;

        public ObservableCollection<Location> Locations { get; set; } = new ObservableCollection<Location>();
        public bool LocationsIsLoaded
        {
            get { return locationsisloaded; }
            set
            {
                if (locationsisloaded != value)
                {
                    locationsisloaded = value;
                    OnPropertyChanged(nameof(LocationsIsLoaded));
                }
            }
        } bool locationsisloaded;
        public bool LocationsIsBeingLoaded
        {
            get { return locationsisbeingloaded; }
            set
            {
                if (locationsisbeingloaded != value)
                {
                    locationsisbeingloaded = value;
                    OnPropertyChanged(nameof(LocationsIsBeingLoaded));
                }
            }
        } bool locationsisbeingloaded;

        public ObservableCollection<BinType> BinTypes { get; set; } = new ObservableCollection<BinType>();
        public bool BinTypesIsLoaded
        {
            get { return bintypesisloaded; }
            set
            {
                if (bintypesisloaded != value)
                {
                    bintypesisloaded = value;
                    OnPropertyChanged(nameof(BinTypesIsLoaded));
                }
            }
        } bool bintypesisloaded;
        public bool BinTypesIsBeingLoaded
        {
            get { return bintypesisbeingloaded; }
            set
            {
                if (bintypesisbeingloaded != value)
                {
                    bintypesisbeingloaded = value;
                    OnPropertyChanged(nameof(BinTypesIsBeingLoaded));
                }
            }
        } bool bintypesisbeingloaded;

        public bool CanChangeLocationCode
        {
            get { return canchangelocationCode; }
            set
            {
                if (canchangelocationCode != value)
                {
                    canchangelocationCode = value;
                    OnPropertyChanged(nameof(CanChangeLocationCode));
                }
            }
        } bool canchangelocationCode;


        public List<SubSchemeElement> SubSchemeElements { get; set; } = new List<SubSchemeElement>();
        public bool RacksIsLoaded
        {
            get { return racksisloaded; }
            set
            {
                if (racksisloaded != value)
                {
                    racksisloaded = value;
                    OnPropertyChanged(nameof(RacksIsLoaded));
                }
            }
        } bool racksisloaded;
        public bool RacksIsBeingLoaded
        {
            get { return racksisbeingloaded; }
            set
            {
                if (racksisbeingloaded != value)
                {
                    racksisbeingloaded = value;
                    OnPropertyChanged(nameof(RacksIsBeingLoaded));
                }
            }
        } bool racksisbeingloaded;

        public ZoneViewModel(INavigation navigation, Zone zone) : base(navigation)
        {
            Zone = zone;
            FillFields(zone);
            EditMode = SchemeElementEditMode.None;
            TapCommand = new Command<object>(Tap);

            OKCommand = new Command(OK);
            CancelCommand = new Command(Cancel);
            CancelChangesCommand = new Command(CancelChanges);

            State = State.Normal;
            Changed = false;
        }

        public void FillFields(Zone zone)
        {
            LocationCode = zone.LocationCode;
            Code = zone.Code;
            Description = zone.Description;
            Color = Color.FromHex(zone.HexColor);
            BinTypeCode = zone.BinTypeCode;
            SchemeVisible = zone.SchemeVisible;
            RackQuantity = zone.RackQuantity;
            BinQuantity = zone.BinQuantity;
            PlanWidth = zone.PlanWidth;
            PlanHeight = zone.PlanHeight;
        }

        public void SaveFields(Zone zone)
        {
            zone.LocationCode = LocationCode;
            zone.Code = Code;
            zone.Description = Description;
            zone.HexColor = ColorToHex(Color);
            zone.BinTypeCode = BinTypeCode;
            zone.SchemeVisible = SchemeVisible;
            zone.PlanWidth = PlanWidth;
            zone.PlanHeight = PlanHeight;
        }

        
        public void SavePrevSize(double width, double height)
        {
            PrevWidth = width;
            PrevHeight = height;
        }

        public void Tap(object sender)
        {
            if (OnTap is Action<ZoneViewModel>)
            {
                OnTap(this);
            }
        }

        public async void OK()
        {
            SaveFields(Zone);
            if (CreateMode)
            {
                try
                {
                    await NAV.CreateZone(Zone,ACD.Default);
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    State = State.Error;
                    ErrorText = ex.Message;
                }
            }
            else
            {
                try
                {
                    await NAV.ModifyZone(Zone, ACD.Default);
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    State = State.Error;
                    ErrorText = ex.Message;
                }
            }
        }

        public async void Cancel()
        {
            await Navigation.PopAsync();
        }

        public void CancelChanges()
        {
            FillFields(Zone);
        }

        public async void CheckZoneCode()
        {
            if (CreateMode)
            {
                CodeWarningText = "";
                if ((LocationCode != "") && (Code != ""))
                {
                    int exist = await NAV.GetZoneCount(LocationCode, Code, false, ACD.Default);
                    if (exist > 0)
                    {
                        CodeWarningText = AppResources.NewZonePage_CodeAlreadyExist;
                    }
                }
            }
        }

        public async void Load()
        {
            try
            {
                LocationsIsBeingLoaded = true;
                List<Location> locations = await NAV.GetLocationList("", false, 1, int.MaxValue, ACD.Default);
                Locations.Clear();
                foreach (Location location in locations)
                {
                    Locations.Add(location);
                }
                LocationsIsLoaded = locations.Count > 0 && CanChangeLocationCode;
                MessagingCenter.Send<ZoneViewModel>(this, "LocationsIsLoaded");

                BinTypesIsBeingLoaded = true;
                List<BinType> bintypes = await NAV.GetBinTypeList(1, int.MaxValue, ACD.Default);
                BinTypes.Clear();
                foreach (BinType bt in bintypes)
                {
                    BinTypes.Add(bt);
                }
                BinTypesIsLoaded = bintypes.Count > 0;
                MessagingCenter.Send<ZoneViewModel>(this, "BinTypesIsLoaded");
            }
            catch (OperationCanceledException ex)
            {
                ErrorText = ex.Message;
            }
            catch { }
            finally
            {
                BinTypesIsBeingLoaded = false;
                LocationsIsBeingLoaded = false;
            }
        }

        public async void LoadRacks()
        {
            RacksIsLoaded = false;
            RacksIsBeingLoaded = true;
            try
            {
                List<Rack> racks = await NAV.GetRackList(LocationCode,Code, true, 1, int.MaxValue, ACD.Default);
                if (!IsDisposed)
                {
                    SubSchemeElements.Clear();
                    foreach (Rack rack in racks)
                    {
                        SubSchemeElement sse = new SubSchemeElement();
                        sse.Left = rack.Left;
                        sse.Top = rack.Top;
                        sse.Height = rack.Height;
                        sse.Width = rack.Width;
                        sse.RackOrientation = rack.RackOrientation;

                        if (Global.SearchResponses is List<SearchResponse>)
                        {
                            List<SearchResponse> list = Global.SearchResponses.FindAll(
                                x => x.ZoneCode == Code &&
                                x.RackNo == rack.No);
                            if (list is List<SearchResponse>)
                            {
                                sse.Selection = new List<SubSchemeSelect>();
                                foreach (SearchResponse sr in list)
                                {
                                    SubSchemeSelect sss = new SubSchemeSelect();
                                    sss.Section = sr.Section;
                                    sss.Level = sr.Level;
                                    sss.Depth = sr.Depth;
                                    sse.Selection.Add(sss);
                                }
                            }
                        }
                        SubSchemeElements.Add(sse);
                    }
                }
            }
            finally
            {
                RacksIsLoaded = true;
                RacksIsBeingLoaded = false;
            }
        }

        public override void Dispose()
        {
            Locations.Clear();
            Locations = null;
            BinTypes.Clear();
            BinTypes = null;
            SubSchemeElements.Clear();
            SubSchemeElements = null;
            TapCommand = null;
            Location = null;
            Zone = null;
            if (OnTap is Action<ZoneViewModel>)
            {
                Delegate[] clientList = OnTap.GetInvocationList();
                foreach (var d in clientList)
                    OnTap -= (d as Action<ZoneViewModel>);
            }
            Zone = null;
            base.Dispose();
        }
    }
}