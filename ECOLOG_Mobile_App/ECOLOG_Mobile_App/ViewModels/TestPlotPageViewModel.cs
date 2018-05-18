using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using Xamarin.Forms;

namespace ECOLOG_Mobile_App.ViewModels
{
	public class TestPlotPageViewModel : BindableBase
	{
        public PlotModel Model { get; }
        public TestPlotPageViewModel()
        {
            this.Model = new PlotModel { Title = "Hello OxyPlot" };
            this.Model.Series.Add(new LineSeries
            {
                Points =
                {
                    new DataPoint(0, 10),
                    new DataPoint(1, 20),
                    new DataPoint(2, 15),
                    new DataPoint(3, 40)
                }
            });

            
        }
	}
}
