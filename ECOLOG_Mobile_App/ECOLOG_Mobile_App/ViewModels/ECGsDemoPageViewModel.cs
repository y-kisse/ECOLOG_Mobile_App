using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Plugin.Geolocator.Abstractions;
using Prism.Navigation;
using Reactive.Bindings;
using ECOLOG_Mobile_App.Models;

namespace ECOLOG_Mobile_App.ViewModels
{
	public class ECGsDemoPageViewModel : BindableBase
	{
        private readonly ECGModel _ecgModel;
        private readonly double _maximum;

        public ReactiveProperty<PlotModel> PlotModelConvertLoss { get; set; }
        public ReactiveProperty<PlotModel> PlotModelAirResistance { get; set; }
        public ReactiveProperty<PlotModel> PlotModelRollingResistance { get; set; }
        public ReactiveProperty<PlotModel> PlotModelRegeneLoss { get; set; }
        public ReactiveProperty<string> AtentionText { get; set; }

        public ECGsDemoPageViewModel()
        {
            PlotModelConvertLoss = new ReactiveProperty<PlotModel>();
            PlotModelAirResistance = new ReactiveProperty<PlotModel>();
            PlotModelRollingResistance = new ReactiveProperty<PlotModel>();
            PlotModelRegeneLoss = new ReactiveProperty<PlotModel>();
            AtentionText = new ReactiveProperty<string>();

            _ecgModel = ECGModel.GetECGModel(new SemanticLink { SemanticLinkId = 207 });
            _maximum = new double[]
            {
                _ecgModel.GraphData.Max(v => v.ConvertLoss),
                _ecgModel.GraphData.Max(v => v.AirResistance),
                _ecgModel.GraphData.Max(v => v.RollingResistance),
                _ecgModel.GraphData.Max(v => v.RegeneLoss)
            }.Max();

            PlotModelConvertLoss.Value = CreatePlotModel("ConvertLoss");
            PlotModelAirResistance.Value = CreatePlotModel("AirResistance");
            PlotModelRollingResistance.Value = CreatePlotModel("RollingResistance");
            PlotModelRegeneLoss.Value = CreatePlotModel("RegeneLoss");

            AtentionText.Value = _ecgModel.AtentionText;
        }

        private PlotModel CreatePlotModel(string propertyName)
        {
            string title = null;
            switch (propertyName)
            {
                case "ConvertLoss":
                    title = "Convert loss";
                    break;
                case "AirResistance":
                    title = "Air resistance";
                    break;
                case "RollingResistance":
                    title = "Rolling resistance";
                    break;
                case "RegeneLoss":
                    title = "Regene loss";
                    break;
            }

            var model = new PlotModel
            {
                Subtitle = title,
                PlotMargins = new OxyThickness(double.NaN, double.NaN, 80, double.NaN)
            };

            var colorAxis = new LinearColorAxis
            {
                HighColor = OxyColors.Gray,
                LowColor = OxyColors.Black,
                Position = AxisPosition.Right,
                MajorStep = 0.02,
                Minimum = 0,
                Maximum = _maximum,
                Unit = "kWh",
                AxisTitleDistance = 0
            };
            model.Axes.Add(colorAxis);

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

            var scatterSeries = new ScatterSeries();

            foreach (var datum in _ecgModel.GraphData)
            {
                scatterSeries.Points.Add(new ScatterPoint(datum.TransitTime, datum.LostEnergy)
                {
                    Value = (float)typeof(GraphDatum).GetRuntimeProperty(propertyName).GetValue(datum)
                });
            }

            model.Series.Add(scatterSeries);

            return model;
        }
    }
}
