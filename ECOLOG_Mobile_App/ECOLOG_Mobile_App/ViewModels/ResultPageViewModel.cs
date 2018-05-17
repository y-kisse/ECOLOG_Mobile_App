using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Prism.Navigation;
using Reactive.Bindings;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Collections.Concurrent;
using ECOLOG_Mobile_App.Models;
using ECOLOG_Mobile_App.Views;

namespace ECOLOG_Mobile_App.ViewModels
{
	public class ResultPageViewModel : BindableBase
	{
        public const string ParamCalculator = "calculator";
        public const string ParamSemanticLink = "semantic_link";
        private readonly INavigationService _navigationService;
        private ECOLOGCalculator _calculatorParam;
        private ECOLOGCalculator _calculatorMine;
        private SemanticLink _semanticLink;
        private double _lostEnergy;
        private int _transitTime;
        private ECGModel _ecgModel;
        public ReactiveProperty<PlotModel> PlotModel { get; set; }
        public ReactiveProperty<bool> IsBusy { get; set; }
        private int _count;

        public ResultPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            PlotModel = new ReactiveProperty<PlotModel>();
            IsBusy = new ReactiveProperty<bool> { Value = true };
            _count = 0;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            CrossGeolocator.Current.PositionChanged -= OnPositionChanged;
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            App.AppStatus = "ResultPage";
            App.EventList.Add(Observable.FromEventPattern<PositionEventArgs>(
                h => CrossGeolocator.Current.PositionChanged += h, h => CrossGeolocator.Current.PositionChanged -= h)
                              .Subscribe(e => OnPositionChanged(e.Sender, e.EventArgs)));

            //CrossGeolocator.Current.PositionChanged += OnPositionChanged;
            if (CrossGeolocator.Current.IsListening == false)
            {
                CrossGeolocator.Current.DesiredAccuracy = 1;
                await CrossGeolocator.Current.StartListeningAsync(minimumTime: TimeSpan.FromMilliseconds(1000), minimumDistance: 0, includeHeading: false);
            }

            _calculatorParam = parameters[ParamCalculator] as ECOLOGCalculator;
            _calculatorMine = new ECOLOGCalculator();
            _calculatorMine.Init();

            _lostEnergy = _calculatorParam.LostEnergyList.Sum();
            _transitTime = (int)(_calculatorParam.PositionCollection.Last().Timestamp -
                                  _calculatorParam.PositionCollection.First().Timestamp).TotalSeconds;
            var semanticLink = parameters[ParamSemanticLink] as SemanticLink;
            _ecgModel = ECGModel.GetECGModel(semanticLink);

            //DependencyService.Get<IAudio>().PlayAudioFile("broadcasting.mp3");

            /*** デバッグコード ***/
            /*Debug.WriteLine("Count: " + _calculatorParam.PositionCollection.Count);
            Debug.WriteLine("LostEnergy: " + _calculatorParam.LostEnergyList.Sum());
            Debug.WriteLine("AirResistance: " + _calculatorParam.AirResistanceList.Sum());
            Debug.WriteLine("Rolling: " + _calculatorParam.RollingResistanceList.Sum());
            Debug.WriteLine("Convert: " + _calculatorParam.ConvertLossList.Sum());
            Debug.WriteLine("Regene: " + _calculatorParam.RegeneLossList.Sum());*/
            /*** デバッグコード ***/

            PlotModel.Value = CreatePlotModel();

            var timer = new ReactiveTimer(TimeSpan.FromSeconds(1));
            timer.Subscribe(v =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _count++;
                    /*** テストコード ***/
                    //OnPositionChanged(null, new PositionEventArgs(TestPosition.TestPositions[TestPosition.Index]));
                    //TestPosition.Index++;
                    /*** テストコード ***/

                    if (_count >= 10)
                    {
                        timer.Stop();
                        if (_semanticLink != null)
                        {
                            var parameter = new NavigationParameters
                            {
                                {ECGsPageViewModel.ParamCalculator, _calculatorMine},
                                {ECGsPageViewModel.ParamSemanticLink, _semanticLink}
                            };
                            _navigationService.NavigateAsync($"/{nameof(ECGsPage)}", parameter);
                        }
                        else
                        {
                            _navigationService.NavigateAsync($"/{nameof(MapPage)}");
                        }
                    }
                });
            });
            timer.Start();
        }

        private PlotModel CreatePlotModel()
        {
            var model = new PlotModel();

            var xAxis = new LinearAxis
            {
                Title = "Transit time",
                Unit = "s",
                Position = AxisPosition.Bottom
            };
            model.Axes.Add(xAxis);
            var yAxis = new LinearAxis
            {
                Title = "Lost energy",
                Unit = "kWh"
            };
            model.Axes.Add(yAxis);

            var scatterSeries = new ScatterSeries
            {
                MarkerFill = OxyColors.Gray
            };

            foreach (var datum in _ecgModel.GraphData)
            {
                scatterSeries.Points.Add(new ScatterPoint(datum.TransitTime, datum.LostEnergy));
            }
            model.Series.Add(scatterSeries);

            var trip = new ScatterSeries
            {
                MarkerFill = OxyColors.Blue,
                MarkerSize = 20,
                MarkerType = MarkerType.Circle,
                Points = { new ScatterPoint(_transitTime, _lostEnergy) }
            };
            model.Series.Add(trip);

            return model;
        }

        private void OnPositionChanged(object sender, PositionEventArgs e)
        {
            //Debug.WriteLine("ResultPage.OnPositionChanged");

            Device.BeginInvokeOnMainThread(() =>
            {
                App.CurrentPosition = e.Position;

                if (_semanticLink == null)
                    _semanticLink = SemanticLink.TargetSemanticLinks
                        .FirstOrDefault(v => e.Position.Latitude > v.MinLatitude
                                             && e.Position.Latitude < v.MaxLatitude
                                             && e.Position.Longitude > v.MinLongitude
                                             && e.Position.Longitude < v.MaxLongitude);

                _calculatorMine.PositionCollection.Add(e.Position);
            });
        }
    }
}
