using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Realms;
using ECOLOG_Mobile_App.Utils;

namespace ECOLOG_Mobile_App.Models
{
    public class ECGModel
    {
        public IList<GraphDatum> GraphData { get; set; }
        public string AtentionText { get; set; }

        public static ECGModel GetECGModel(SemanticLink semanticLink)
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

            string atentionText = null;
            switch (semanticLink.SemanticLinkId)
            {
                case 188:
                    atentionText = "回生ブレーキに注意！";
                    break;
                case 189:
                    atentionText = "加減速を減らして！";
                    break;
                case 190:
                    atentionText = "いつも通り運転してください";
                    break;
                case 191:
                    atentionText = "加減速を減らして！";
                    break;
                case 192:
                    atentionText = "いつも通り運転してください";
                    break;
                case 193:
                    atentionText = "回生ブレーキに注意！";
                    break;
                case 194:
                    atentionText = "いつも通り運転してください";
                    break;
                case 195:
                    atentionText = "いつも通り運転してください";
                    break;
                case 196:
                    atentionText = "回生ブレーキに注意！";
                    break;
                case 198:
                    atentionText = "加減速を減らして！";
                    break;
                case 199:
                    atentionText = "加減速を減らして！";
                    break;
                case 205:
                    atentionText = "加減速を減らして！";
                    break;
                case 206:
                    atentionText = "加減速を減らして！";
                    break;
                case 207:
                    atentionText = "加減速を減らして！";
                    break;
                case 209:
                    atentionText = "回生ブレーキに注意！";
                    break;
                case 210:
                    atentionText = "加減速と回生ブレーキに注意！";
                    break;
                case 211:
                    atentionText = "回生ブレーキに注意！";
                    break;
                case 212:
                    atentionText = "加減速を減らして！";
                    break;
                case 213:
                    atentionText = "加減速と回生ブレーキに注意！";
                    break;
                case 215:
                    atentionText = "加減速を減らして！";
                    break;
                case 216:
                    atentionText = "加減速を減らして！";
                    break;
                case 217:
                    atentionText = "加減速を減らして！";
                    break;
                case 155:
                    atentionText = "加減速を減らして！";
                    break;
                default:
                    atentionText = "SemanticLinkが見つかりません";
                    break;
            }

            return new ECGModel { GraphData = data, AtentionText = atentionText };
        }
    }
}
