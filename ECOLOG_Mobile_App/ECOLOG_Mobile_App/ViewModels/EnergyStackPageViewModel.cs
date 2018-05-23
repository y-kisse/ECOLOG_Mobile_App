using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Plugin.Permissions;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Reflection;
using PCLStorage;
using Reactive.Bindings;
using ECOLOG_Mobile_App.Models;

namespace ECOLOG_Mobile_App.ViewModels
{
	public class EnergyStackPageViewModel : BindableBase
	{
        private static readonly string RootFolderPath = "/sdcard/Download";

        public ReactiveProperty<PlotModel> PlotModelChorale { get; set; }
        public ReactiveProperty<PlotModel> PlotModelEnergyStack { get; set; }
        public ICommand PlusCommand { get; set; }
        public SemanticLink SemanticLinkCurrent { get; set; }
        public SemanticLink SemanticLinkPrevious { get; set; }
        public String Direction { get; set; }
        public ChoraleModel ChoraleModel { get; set; }
        public IList<EnergyStackModel> EnergyStackModelList { get; set; }
        public ECOLOGCalculator Calculator { get; set; }
        public ReactiveProperty<string> LastGeoStr { get; set; }

        public ReactiveTimer Timer { get; set; }

        public EnergyStackPageViewModel()
        {
            PlotModelChorale = new ReactiveProperty<PlotModel>();
            PlotModelEnergyStack = new ReactiveProperty<PlotModel>();

            PlusCommand = new DelegateCommand(async () =>
            {
                var rootFolder = await FileSystem.Current.GetFolderFromPathAsync(RootFolderPath);
                var file = await rootFolder.CreateFileAsync($"Plus_{DateTime.Now:yyyy-MM-dd}.txt",
                    CreationCollisionOption.OpenIfExists);
                var writedText = await file.ReadAllTextAsync();
                await file.WriteAllTextAsync(writedText + $"{DateTime.Now},{SemanticLinkPrevious?.SemanticLinkId},{SemanticLinkPrevious?.Semantics}{System.Environment.NewLine}");
            });

            Calculator = new ECOLOGCalculator();
            Calculator.Init();

            App.EventList.Add(Observable.FromEventPattern<PositionEventArgs>(
                    h => CrossGeolocator.Current.PositionChanged += h,
                    h => CrossGeolocator.Current.PositionChanged -= h)
                .Subscribe(e => OnPositionChanged(e.Sender, e.EventArgs)));

            if (CrossGeolocator.Current.IsListening == false)
            {
                CrossGeolocator.Current.DesiredAccuracy = 1;
                CrossGeolocator.Current.StartListeningAsync(minimumTime: TimeSpan.FromMilliseconds(1000), minimumDistance: 0, includeHeading: false);
            }

            LastGeoStr = new ReactiveProperty<string>();

            /*** テストコード ***/
            /*var positions = TestPosition.TestPositions;
            Timer = new ReactiveTimer(TimeSpan.FromMilliseconds(200));
            Timer.Subscribe(x =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    OnPositionChanged(null, new PositionEventArgs(positions[TestPosition.Index]));
                    TestPosition.Index++;
                    Debug.WriteLine(TestPosition.Index);
                });
            });
            Timer.Start();*/
            /*** テストコード ***/
        }

        private void OnPositionChanged(object sender, PositionEventArgs e)
        {
            LastGeoStr.Value = (string)(e.Position.Latitude + ", " + e.Position.Longitude);
           
            Device.BeginInvokeOnMainThread(() =>
            {
                App.CurrentPosition = e.Position;
                Console.WriteLine(e.Position.Latitude + ", " + e.Position.Longitude);
                if (SemanticLink.TargetSemanticLinks == null)
                {
                    // HomeWardかOutWardかを決定
                    if (Coordinate.TommyHome.LatitudeStart < e.Position.Latitude
                        && Coordinate.TommyHome.LatitudeEnd > e.Position.Latitude
                        && Coordinate.TommyHome.LongitudeStart < e.Position.Longitude
                        && Coordinate.TommyHome.LongitudeEnd > e.Position.Longitude)
                    {
                        SemanticLink.TargetSemanticLinks = SemanticLink.OutwardSemanticLinks;
                        Direction = "outward";
                        Debug.WriteLine("出発地点を自宅にセット");
                    }
                    else if (Coordinate.Ynu.LatitudeStart < e.Position.Latitude
                             && Coordinate.Ynu.LatitudeEnd > e.Position.Latitude
                             && Coordinate.Ynu.LongitudeStart < e.Position.Longitude
                             && Coordinate.Ynu.LongitudeEnd > e.Position.Longitude)
                    {
                        SemanticLink.TargetSemanticLinks = SemanticLink.HomewardSemanticLinks;
                        Direction = "homeward";
                        Debug.WriteLine("出発地点を学校にセット");
                    }
                }
                else
                {
                    // HomeWardかOutWardか決定されている
                    if (SemanticLinkCurrent == null)
                    {
                        // 最初のセマンティックリンクを決定
                        SemanticLinkCurrent = SemanticLink.TargetSemanticLinks
                            .FirstOrDefault(v => e.Position.Latitude > v.MinLatitude
                                                 && e.Position.Latitude < v.MaxLatitude
                                                 && e.Position.Longitude > v.MinLongitude
                                                 && e.Position.Longitude < v.MaxLongitude);
                    }
                    else
                    {
                        if (e.Position.Latitude < SemanticLinkCurrent.MinLatitude - 0.0001
                            || e.Position.Latitude > SemanticLinkCurrent.MaxLatitude + 0.0001
                            || e.Position.Longitude < SemanticLinkCurrent.MinLongitude
                            || e.Position.Longitude > SemanticLinkCurrent.MaxLongitude)
                        {
                            // セマンティックリンクの変更を検知
                            // ChoraleとEnergyStackModelの描画を開始
                            ChoraleModel = ChoraleModel.CreateChoraleModel(SemanticLinkCurrent);
                            EnergyStackModelList =
                                EnergyStackModel.CreateEnergyStackSource(Calculator.GetGraphDatum(), SemanticLinkCurrent);

                            PlotModelChorale.Value = CreatePlotModelChorale();
                            PlotModelEnergyStack.Value = CreatePlotModelEnergyStack();

                            Calculator.Init();
                            SemanticLinkPrevious = SemanticLinkCurrent.Copy();
                            SemanticLinkCurrent = null;

                            // DependencyService.Get<IAudio>().PlayAudioFile("broadcasting.mp3");
                        }
                        else
                        {
                            Calculator.PositionCollection.Add(e.Position);
                        }
                    }
                }
            });
        }

        private PlotModel CreatePlotModelChorale()
        {
            var plotModel = new PlotModel
            {
                Subtitle = $"Semanantic Link: {SemanticLinkCurrent.SemanticLinkId}, Direction: {Direction}"
            };

            var colorAxis = new LinearColorAxis
            {
                HighColor = OxyColors.Gray,
                LowColor = OxyColors.Black,
                Position = AxisPosition.Right
            };
            plotModel.Axes.Add(colorAxis);

            var xAxis = new LinearAxis
            {
                Title = "Transit Time",
                Unit = "s",
                Position = AxisPosition.Bottom
            };
            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Title = "Lost Energy",
                Unit = "kWh"
            };
            plotModel.Axes.Add(yAxis);

            var heatMapSeries = new HeatMapSeries
            {
                LabelFormatString = "0",
                X0 = ChoraleModel.MinTransitTime,
                X1 = ChoraleModel.MaxTransitTime,
                Y0 = ChoraleModel.MinLostEnegry,
                Y1 = ChoraleModel.MaxLostEnergy,
                LabelFontSize = 0.2,
                Data = ChoraleModel.Data
            };
            plotModel.Series.Add(heatMapSeries);

            var scatterSeries = new ScatterSeries
            {
                MarkerFill = OxyColors.Black,
                MarkerType = MarkerType.Circle,
                MarkerSize = 30
            };
            var lostEnergy = Calculator.LostEnergyList.Sum();
            var transitTime = (int)(Calculator.PositionCollection.Last().Timestamp -
                                     Calculator.PositionCollection.First().Timestamp).TotalSeconds;
            scatterSeries.Points.Add(new ScatterPoint(transitTime, lostEnergy)
            {
                Value = 0
            });
            plotModel.Series.Add(scatterSeries);

            return plotModel;
        }

        private PlotModel CreatePlotModelEnergyStack()
        {
            var model = new PlotModel();

            var axisX = new CategoryAxis
            {
                ItemsSource = EnergyStackModelList,
                LabelField = "Category",
                Position = AxisPosition.Bottom
            };
            model.Axes.Add(axisX);

            var axisY = new LinearAxis
            {
                Title = "Energy",
                Unit = "kWh",
                Position = AxisPosition.Left
            };
            model.Axes.Add(axisY);

            foreach (var propertyInfo in typeof(EnergyStackModel).GetRuntimeProperties())
            {
                if (propertyInfo.Name == "Category")
                    continue;

                var series = new ColumnSeries
                {
                    ItemsSource = EnergyStackModelList,
                    ValueField = propertyInfo.Name,
                    IsStacked = true
                };
                if (propertyInfo.Name.Contains("Blank"))
                {
                    series.FillColor = OxyColors.White;
                }
                else if (propertyInfo.Name.Contains("Defeat"))
                {
                    series.FillColor = OxyColors.Black;
                }
                else if (propertyInfo.Name.Contains("Air"))
                {
                    series.FillColor = OxyColors.Yellow;
                }
                else if (propertyInfo.Name.Contains("Rolling"))
                {
                    series.FillColor = OxyColors.Orange;
                }
                else if (propertyInfo.Name.Contains("Regene"))
                {
                    series.FillColor = OxyColors.DeepPink;
                }
                else if (propertyInfo.Name.Contains("ConvertLoss"))
                {
                    series.FillColor = OxyColors.Red;
                }
                model.Series.Add(series);
            }

            return model;
        }
    }
}
