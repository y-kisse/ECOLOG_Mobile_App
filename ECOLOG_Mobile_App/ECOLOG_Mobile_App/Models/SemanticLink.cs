using System;
using System.Collections.Generic;
using System.Text;

namespace ECOLOG_Mobile_App.Models
{
    // From TOD2017MobileApp
    public class SemanticLink
    {
        public static IList<SemanticLink> TargetSemanticLinks;
        // ハードコード...
        public SemanticLink Copy()
        {
            return new SemanticLink
            {
                Semantics = this.Semantics,
                SemanticLinkId = this.SemanticLinkId,
                MaxLatitude = this.MaxLatitude,
                MinLatitude = this.MinLatitude,
                MaxLongitude = this.MaxLongitude,
                MinLongitude = this.MinLongitude
            };
        }

        public static IList<SemanticLink> OutwardSemanticLinks = new List<SemanticLink>
        {
            // outward
            new SemanticLink
            {
                SemanticLinkId = 188,
                Semantics = "綾瀬市役所前～与蔵山下",
                MinLatitude = 35.4365974,
                MaxLatitude = 35.44097356,
                MinLongitude = 139.4276557,
                MaxLongitude = 139.4387653
            },
            new SemanticLink
            {
                SemanticLinkId = 189,
                Semantics = "与蔵山下～代官二丁目",
                MinLatitude = 35.43955833,
                MaxLatitude = 35.44318496,
                MinLongitude = 139.4387653,
                MaxLongitude = 139.4546107
            },
            new SemanticLink
            {
                SemanticLinkId = 190,
                Semantics = "代官二丁目～福田入口",
                MinLatitude = 35.43630294,
                MaxLatitude = 35.44318496,
                MinLongitude = 139.4546107,
                MaxLongitude = 139.4669059
            },
            new SemanticLink
            {
                SemanticLinkId = 191,
                Semantics = "福田入口～下和田",
                MinLatitude = 35.42603918,
                MaxLatitude = 35.43633438,
                MinLongitude = 139.4668446,
                MaxLongitude = 139.4683016
            },
            new SemanticLink
            {
                SemanticLinkId = 192,
                Semantics = "下和田～いちょう小学校入口",
                MinLatitude = 35.42595376,
                MaxLatitude = 35.43310906,
                MinLongitude = 139.4683809,
                MaxLongitude = 139.4774651
            },
            new SemanticLink
            {
                SemanticLinkId = 193,
                Semantics = "いちょう小学校入口～日向山団地入口",
                MinLatitude = 35.43308126,
                MaxLatitude = 35.44281711,
                MinLongitude = 139.4769315,
                MaxLongitude = 139.4804712
            },
            new SemanticLink
            {
                SemanticLinkId = 194,
                Semantics = "日向山団地入口～阿久和",
                MinLatitude = 35.44281711,
                MaxLatitude = 35.44740874,
                MinLongitude = 139.4804712,
                MaxLongitude = 139.5040412
            },
            new SemanticLink
            {
                SemanticLinkId = 195,
                Semantics = "阿久和～桃源台",
                MinLatitude = 35.44741043,
                MaxLatitude = 35.45429267,
                MinLongitude = 139.5043121,
                MaxLongitude = 139.5206251
            },
            new SemanticLink
            {
                SemanticLinkId = 196,
                Semantics = "桃源台～万騎が原小学校前",
                MinLatitude = 35.45074488,
                MaxLatitude = 35.45429267,
                MinLongitude = 139.5206251,
                MaxLongitude = 139.5331955
            },
            new SemanticLink
            {
                SemanticLinkId = 198,
                Semantics = "川上IC～今井IC",
                MinLatitude = 35.43111698,
                MaxLatitude = 35.44391016,
                MinLongitude = 139.5511129,
                MaxLongitude = 139.5619204
            },
            new SemanticLink
            {
                SemanticLinkId = 199,
                Semantics = "今井IC～藤塚料金所",
                MinLatitude = 35.44391016,
                MaxLatitude = 35.45277291,
                MinLongitude = 139.5619204,
                MaxLongitude = 139.5772564
            }
        };

        public static IList<SemanticLink> HomewardSemanticLinks = new List<SemanticLink>
        {
            // homeward
            new SemanticLink
            {
                SemanticLinkId = 205,
                Semantics = "藤塚料金所～保土ヶ谷バイパス加速車線",
                MinLatitude = 35.44963764,
                MaxLatitude = 35.45277291,
                MinLongitude = 139.5671481,
                MaxLongitude = 139.5772564
            },
            new SemanticLink
            {
                SemanticLinkId = 206,
                Semantics = "藤塚IC加速車線～南本宿IC",
                MinLatitude = 35.45132953,
                MaxLatitude = 35.46304083,
                MinLongitude = 139.5415328,
                MaxLongitude = 139.5671481
            },
            new SemanticLink
            {
                SemanticLinkId = 207,
                Semantics = "南本宿IC～本村IC",
                MinLatitude = 35.46304083,
                MaxLatitude = 35.46913823,
                MinLongitude = 139.533574,
                MaxLongitude = 139.5415328
            },
            new SemanticLink
            {
                SemanticLinkId = 209,
                Semantics = "下川井IC～西部病院入口",
                MinLatitude = 35.47433081,
                MaxLatitude = 35.48207426,
                MinLongitude = 139.5010665,
                MaxLongitude = 139.5132386
            },
            new SemanticLink
            {
                SemanticLinkId = 210,
                Semantics = "西部病院入口～南台",
                MinLatitude = 35.46641278,
                MaxLatitude = 35.47433081,
                MinLongitude = 139.491014,
                MaxLongitude = 139.5010665
            },
            new SemanticLink
            {
                SemanticLinkId = 211,
                Semantics = "南台～下瀬谷二丁目",
                MinLatitude = 35.45878911,
                MaxLatitude = 35.46641173,
                MinLongitude = 139.4808058,
                MaxLongitude = 139.4909644
            },
            new SemanticLink
            {
                SemanticLinkId = 212,
                Semantics = "下瀬谷二丁目～桜ヶ丘",
                MinLatitude = 35.44942028,
                MaxLatitude = 35.45866595,
                MinLongitude = 139.4675835,
                MaxLongitude = 139.4806114
            },
            new SemanticLink
            {
                SemanticLinkId = 213,
                Semantics = "桜ヶ丘～代官一丁目",
                MinLatitude = 35.44564735,
                MaxLatitude = 35.44942028,
                MinLongitude = 139.4597433,
                MaxLongitude = 139.4675835
            },
            new SemanticLink
            {
                SemanticLinkId = 214,
                Semantics = "代官一丁目～代官三丁目",
                MinLatitude = 35.43901117,
                MaxLatitude = 35.44564735,
                MinLongitude = 139.4530834,
                MaxLongitude = 139.4597433
            },
            new SemanticLink
            {
                SemanticLinkId = 215,
                Semantics = "代官三丁目～与蔵山下",
                MinLatitude = 35.43955833,
                MaxLatitude = 35.44101242,
                MinLongitude = 139.4387653,
                MaxLongitude = 139.4530834
            },
            new SemanticLink
            {
                SemanticLinkId = 216,
                Semantics = "与蔵山下～綾瀬市役所",
                MinLatitude = 35.4365974,
                MaxLatitude = 35.44097356,
                MinLongitude = 139.4276557,
                MaxLongitude = 139.4387653
            },
            new SemanticLink
            {
                SemanticLinkId = 217,
                Semantics = "綾瀬市役所～綾西小学校東",
                MinLatitude = 35.43419733,
                MaxLatitude = 35.43650195,
                MinLongitude = 139.4182168,
                MaxLongitude = 139.4274183
            },
            new SemanticLink
            {
                SemanticLinkId = 155,
                Semantics = "代官一丁目～代官二丁目～代官三丁目",
                MinLatitude = 35.43955833,
                MaxLatitude = 35.44574456,
                MinLongitude = 139.4530834,
                MaxLongitude = 139.4597433
            }
        };

        public int SemanticLinkId { get; set; }
        public string Semantics { get; set; }
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }
        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }
    }
}
