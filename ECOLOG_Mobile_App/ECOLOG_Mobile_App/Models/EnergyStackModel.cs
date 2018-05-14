using System;
using System.Collections.Generic;
using System.Text;
using Realms;
using ECOLOG_Mobile_App.Utils;
using System.Linq;

namespace ECOLOG_Mobile_App.Models
{
    public class EnergyStackModel
    {
        public string Category { get; set; }
        public double RegeneLoss { get; set; }
        public double RegeneLossBlank { get; set; }
        public double RegeneLossDefeat { get; set; }
        public double RegeneLossWin { get; set; }
        public double AirResistance { get; set; }
        public double AirResistanceBlank { get; set; }
        public double AirResistanceDefeat { get; set; }
        public double AirResistanceWin { get; set; }
        public double RollingResistance { get; set; }
        public double RollingResistanceBlank { get; set; }
        public double RollingResistanceDefeat { get; set; }
        public double RollingResistanceWin { get; set; }
        public double ConvertLoss { get; set; }
        public double ConvertLossBlank { get; set; }
        public double ConvertLossDefeat { get; set; }
        public double ConvertLossWin { get; set; }

        public static IList<EnergyStackModel> CreateEnergyStackSource(GraphDatum datum, SemanticLink semanticLink)
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

            var regeneLossSigma = data.Average(v => v.RegeneLoss) - data.StdDev(v => v.RegeneLoss);
            var airResistanceSigma = data.Average(v => v.AirResistance) - data.StdDev(v => v.AirResistance);
            var rollingResistanceSigma = data.Average(v => v.RollingResistance) - data.StdDev(v => v.RollingResistance);
            var convertLossSigma = data.Average(v => v.ConvertLoss) - data.StdDev(v => v.ConvertLoss);

            return new List<EnergyStackModel>
            {
                new EnergyStackModel
                {
                    Category = "Defeat",
                    RegeneLossBlank = datum.RegeneLoss <= regeneLossSigma ? datum.RegeneLoss : regeneLossSigma,
                    RegeneLossDefeat = datum.RegeneLoss > regeneLossSigma ? datum.RegeneLoss - regeneLossSigma : 0,
                    AirResistanceBlank = datum.AirResistance <= airResistanceSigma ? datum.AirResistance : airResistanceSigma,
                    AirResistanceDefeat = datum.AirResistance > airResistanceSigma ? datum.AirResistance - airResistanceSigma : 0,
                    RollingResistanceBlank = datum.RollingResistance <= rollingResistanceSigma ? datum.RollingResistance : rollingResistanceSigma,
                    RollingResistanceDefeat = datum.RollingResistance > rollingResistanceSigma ? datum.RollingResistance - rollingResistanceSigma : 0,
                    ConvertLossBlank = datum.ConvertLoss <= convertLossSigma ? datum.ConvertLoss : convertLossSigma,
                    ConvertLossDefeat = datum.ConvertLoss > convertLossSigma ? datum.ConvertLoss - convertLossSigma : 0,
                },
                new EnergyStackModel
                {
                    Category = "Today",
                    RegeneLoss = datum.RegeneLoss,
                    AirResistance = datum.AirResistance,
                    RollingResistance = datum.RollingResistance,
                    ConvertLoss = datum.ConvertLoss,
                },
                new EnergyStackModel
                {
                    Category = "Win",
                    RegeneLossBlank = datum.RegeneLoss,
                    RegeneLossWin = datum.RegeneLoss <= regeneLossSigma ? regeneLossSigma - datum.RegeneLoss : 0,
                    AirResistanceBlank = datum.AirResistance - (datum.RegeneLoss <= regeneLossSigma ? regeneLossSigma - datum.RegeneLoss : 0),
                    AirResistanceWin = datum.AirResistance <= airResistanceSigma ? airResistanceSigma - datum.AirResistance : 0,
                    RollingResistanceBlank = datum.RollingResistance - (datum.AirResistance <= airResistanceSigma ? airResistanceSigma - datum.AirResistance : 0),
                    RollingResistanceWin = datum.RollingResistance <= rollingResistanceSigma ? rollingResistanceSigma - datum.RollingResistance : 0,
                    ConvertLossBlank = datum.ConvertLoss - (datum.RollingResistance <= rollingResistanceSigma ? rollingResistanceSigma - datum.RollingResistance : 0),
                    ConvertLossWin = datum.ConvertLoss <= convertLossSigma ? convertLossSigma - datum.ConvertLoss : 0
                },
            };
        }
    }
}
