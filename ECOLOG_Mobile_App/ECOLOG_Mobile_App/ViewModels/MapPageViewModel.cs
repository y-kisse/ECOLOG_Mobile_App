using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Prism.Mvvm;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Xamarin.Forms;
using System.Reactive.Linq;
using ECOLOG_Mobile_App.Models;
using ECOLOG_Mobile_App.Views;

namespace ECOLOG_Mobile_App.ViewModels
{
	public class MapPageViewModel : BindableBase
	{
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;
        private bool _didFinishedNavigation;
        public static ReactiveTimer Timer { get; set; }
        public ReactiveProperty<string> Location { get; set; }
        public DelegateCommand NavigateToInsertionPageCommand { get; set; }
        public DelegateCommand NavigateToDemoPageCommand { get; set; }

        public MapPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
            _didFinishedNavigation = false;
            Location = new ReactiveProperty<string>();
            NavigateToInsertionPageCommand =
                new DelegateCommand(() => { _navigationService.NavigateAsync(nameof(DataInsertionPage)); });
            NavigateToDemoPageCommand =
                new DelegateCommand(() => { _navigationService.NavigateAsync(nameof(ECGsDemoPage)); });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            CrossGeolocator.Current.PositionChanged -= OnPositionChanged;
            Timer?.Stop();
            Timer = null;
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            App.AppStatus = "MapPage";
            App.EventList.Add(Observable.FromEventPattern<PositionEventArgs>(
                h => CrossGeolocator.Current.PositionChanged += h, h => CrossGeolocator.Current.PositionChanged -= h)
                              .Subscribe(e => OnPositionChanged(e.Sender, e.EventArgs)));

            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await _pageDialogService.DisplayAlertAsync("Need location", "Gunna need that location", "OK");
                }
                await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            }

            //CrossGeolocator.Current.PositionChanged += OnPositionChanged;
            if (CrossGeolocator.Current.IsListening == false)
            {
                CrossGeolocator.Current.DesiredAccuracy = 1;
                await CrossGeolocator.Current.StartListeningAsync(minimumTime: TimeSpan.FromMilliseconds(1000), minimumDistance: 0, includeHeading: false);
            }

            /*** テストコード ***/
            /*var positions = TestPosition.TestPositions;
            Timer = new ReactiveTimer(TimeSpan.FromMilliseconds(200));
            Timer.Subscribe(x =>
            {
				Device.BeginInvokeOnMainThread(() =>
				{
					OnPositionChanged(null, new PositionEventArgs(positions[TestPosition.Index]));
					TestPosition.Index++;
				});
            });
			Timer.Start();*/
            /*** テストコード ***/
        }

        private void OnPositionChanged(object sender, PositionEventArgs e)
        {
            //Debug.WriteLine("MapPage.OnPositionChanged, " + TestPosition.Index);

            Device.BeginInvokeOnMainThread(async () =>
            {
                App.CurrentPosition = e.Position;

                if (_didFinishedNavigation)
                    return;

                Location.Value = $"{e.Position.Latitude}, {e.Position.Longitude}, {e.Position.Timestamp.LocalDateTime}";

                if (Coordinate.TommyHome.LatitudeStart < e.Position.Latitude
                    && Coordinate.TommyHome.LatitudeEnd > e.Position.Latitude
                    && Coordinate.TommyHome.LongitudeStart < e.Position.Longitude
                    && Coordinate.TommyHome.LongitudeEnd > e.Position.Longitude)
                {
                    SemanticLink.TargetSemanticLinks = SemanticLink.OutwardSemanticLinks;
                    Debug.WriteLine("出発地点を自宅にセット");
                }
                else if (Coordinate.Ynu.LatitudeStart < e.Position.Latitude
                         && Coordinate.Ynu.LatitudeEnd > e.Position.Latitude
                         && Coordinate.Ynu.LongitudeStart < e.Position.Longitude
                         && Coordinate.Ynu.LongitudeEnd > e.Position.Longitude)
                {
                    SemanticLink.TargetSemanticLinks = SemanticLink.HomewardSemanticLinks;
                    Debug.WriteLine("出発地点を学校にセット");
                }

                if (SemanticLink.TargetSemanticLinks == null)
                {
                    await _pageDialogService.DisplayActionSheetAsync("位置検知エラー", "出発地点を特定できませんでした", "OK");
                    return;
                }

                var semanticLink = SemanticLink.TargetSemanticLinks
                    .FirstOrDefault(v => e.Position.Latitude > v.MinLatitude
                                         && e.Position.Latitude < v.MaxLatitude
                                         && e.Position.Longitude > v.MinLongitude
                                         && e.Position.Longitude < v.MaxLongitude);

                //semanticLink = SemanticLinkCurrent.TargetSemanticLinks.FirstOrDefault(v => v.SemanticLinkId == 196);

                if (semanticLink != null)
                {
                    Timer?.Stop();
                    _didFinishedNavigation = true;

                    var navigationParameters = new NavigationParameters
                    {
                        {ECGsPageViewModel.ParamSemanticLink, semanticLink}
                    };
                    await _navigationService.NavigateAsync("/ECGsPage", navigationParameters);
                }
            });
        }
    }
}
