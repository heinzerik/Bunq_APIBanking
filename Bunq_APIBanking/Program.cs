using Bunq.Sdk.Context;
using System;
using Bunq;
using Bunq.Sdk.Model.Generated.Endpoint;
using Newtonsoft.Json.Linq;
using Bunq.Sdk.Model.Generated.Object;
using System.Dynamic;

namespace Bunq_APIBanking
{
    class Program
    {
        public string bunq_uid { get; set; }

        static void Main(string[] args)
        {
            //string apikey = "37bf47037adf654a1383b563bd5545cc78d5093a81290a8c364494f4e7efb46c";
            //var apiContext = ApiContext.Create(ApiEnvironmentType.PRODUCTION, "45c752ef48d48035a59f64e0b5c5dc2cc8269c94c0a7b0e0ad0e8a2b965d0a06", "SimplyTraderWinSrv");

            //apiContext.Save("./context.txt");


            BankingHelper.createAPIContextAndSave("3b50fbcdf37de070e2a3dce2746c8376561a3a03a6ac7e5ea1b17a49aedb52bf", "EriksSamsungGalaxy");
            //BankingHelper.payQuartalFee("DE77100110012623459943","1001a");
            //var response = BankingHelper.RequestPaymentFromBunqEmail("10", "Erik HEINZ", "erikheinz75@gmail.com", "Aufstockung SimplyTrader Depot");
            


        }
    }
}
