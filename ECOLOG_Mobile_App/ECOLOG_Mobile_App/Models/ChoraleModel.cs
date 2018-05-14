using System;
using System.Collections.Generic;
using System.Text;
using ECOLOG_Mobile_App.Utils;
using Realms;
using System.Linq;

namespace ECOLOG_Mobile_App.Models
{
    // From 2017MobileApp
    class ChoraleModel
    {
        public int ClassNumber { get; set; }
        public float MinLostEnegry { get; set; }
        public float MaxLostEnergy { get; set; }
        public float ClassWidthEnergy { get; set; }
        public int MinTransitTime { get; set; }
        public int MaxTransitTime { get; set; }
        public float ClassWidthTransitTime { get; set; }
        public double[,] Data { get; set; }

        public static ChoraleModel CreateChoraleModel(SemanticLink semanticLink)
        {
            var data = Realm.GetInstance()
                .All<GraphDatum>()
                .Where(v => v.SemanticLinkId == semanticLink.SemanticLinkId)
                .ToList();

            var quartilesEnergy = MathUtil.Quartiles(data.OrderBy(d => d.LostEnergy).Select(d => (double)d.LostEnergy).ToArray());
            var firstQuartileEnergy = quartilesEnergy.Item1;
            var thirdQuartileEnergy = quartilesEnergy.Item3;
            var iqrEnergy = thirdQuartileEnergy - firstQuartileEnergy;

            var quartilesTransitTime = MathUtil.Quartiles(data.OrderBy(d => d.TransitTime).Select(d => (double)d.TransitTime).ToArray());
            var firstQuartileTransitTime = quartilesTransitTime.Item1;
            var thirdQuartileTransitTime = quartilesTransitTime.Item3;
            var iqrTransitTime = thirdQuartileTransitTime - firstQuartileTransitTime;

            data = data.Where(d => d.LostEnergy > firstQuartileEnergy - 1.5 * iqrEnergy)
                .Where(d => d.LostEnergy < thirdQuartileEnergy + 1.5 * iqrEnergy)
                .ToList();

            data = data.Where(d => d.TransitTime > firstQuartileTransitTime - 1.5 * iqrTransitTime)
                .Where(d => d.TransitTime < thirdQuartileTransitTime + 1.5 * iqrTransitTime)
                .ToList();

            var model = new ChoraleModel
            {
                ClassNumber = MathUtil.CalculateClassNumber(data),
                MinLostEnegry = data.Min(d => d.LostEnergy),
                MaxLostEnergy = data.Max(d => d.LostEnergy),
                ClassWidthEnergy = (data.Max(d => d.LostEnergy) - data.Min(d => d.LostEnergy)) / MathUtil.CalculateClassNumber(data),
                MinTransitTime = data.Min(d => d.TransitTime),
                MaxTransitTime = data.Max(d => d.TransitTime),
                ClassWidthTransitTime = (float)(data.Max(d => d.TransitTime) - data.Min(d => d.TransitTime)) / MathUtil.CalculateClassNumber(data)
            };
            model.SetData(data);

            return model;
        }

        private void SetData(IList<GraphDatum> list)
        {
            Data = new double[ClassNumber + 1, ClassNumber + 1];

            double preTimeLevel = 0;
            double currentTimeLevel = MinTransitTime;

            for (int i = 0; i < ClassNumber + 1; i++)
            {
                double preEnergyLevel = 0;
                double currentEnergyLevel = MinLostEnegry;

                for (int j = 0; j < ClassNumber + 1; j++)
                {
                    // ReSharper disable once ReplaceWithSingleCallToCount
                    Data[i, j] = list
                        .Where(d => d.LostEnergy > preEnergyLevel)
                        .Where(d => d.LostEnergy <= currentEnergyLevel)
                        .Where(d => d.TransitTime > preTimeLevel)
                        .Where(d => d.TransitTime <= currentTimeLevel)
                        .Count();

                    if (j == 0)
                    {
                        preEnergyLevel = MinLostEnegry;
                    }
                    else
                    {
                        preEnergyLevel += ClassWidthEnergy;
                    }

                    currentEnergyLevel += ClassWidthEnergy;
                }

                if (i == 0)
                {
                    preTimeLevel = MinTransitTime;
                }
                else
                {
                    preTimeLevel += ClassWidthTransitTime;
                }

                currentTimeLevel += ClassWidthTransitTime;
            }
        }
    }
}
