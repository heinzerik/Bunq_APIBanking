using Bunq.Sdk.Context;
using Bunq.Sdk.Http;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bunq_APIBanking
{
    public class BankingHelper
    {

        public static void createAPIContextAndSave(string apikey,string deviceDescription)
        {
            var apiContext = ApiContext.Create(ApiEnvironmentType.PRODUCTION, apikey, deviceDescription);
            Directory.CreateDirectory(@"C:\SimplyTrader\apicontext");
            apiContext.Save(@"C:\SimplyTrader\apicontext\context.txt");

        }
        public static double CheckQuartalFee(string uid)
        {
            Console.WriteLine("needs to be implemented");
            throw new NotImplementedException("needs to be implemented");
        }

        public static BunqResponse<int> makePayment(string amount,string IssuerName,string iban)
        {
            LoadApiContext();
            string Amount = amount.Replace(',', '.');
            Pointer ibanpointer = new Pointer("IBAN", iban);
            ibanpointer.Name = IssuerName;
            BunqResponse<int>  response = Payment.Create(new Amount(amount,"EUR"),ibanpointer, IssuerName + " payed the quartal fee of " +  Amount  + " EUR");
            return response;

            
        }

        public static BunqResponse<int> RequestPaymentFromIban(string amount, string IssuerName, string iban,string requestdescription)
        {
           BunqResponse<int> response =  RequestInquiry.Create(
                new Amount(amount,"EUR"),
                new Pointer("IBAN", iban),
                requestdescription,
                true);

            return response;

        }

        public static BunqResponse<int> RequestPaymentFromBunqEmail(string amount, string issuerName, string email, string requestdescription)
        {
            LoadApiContext();
            BunqResponse<int> response = RequestInquiry.Create(
                 new Amount(amount,"EUR"),
                 new Pointer("EMAIL", email),
                 requestdescription,
                 true);

            return response;

        }

        public static string payQuartalFee(string iban,string uid)
        {
            BunqResponse<int> returnmsg = new BunqResponse<int>(0,new Dictionary<string,string>(),null);
            bool success = LoadApiContext();
            if(success)
            {
                var receivedpayments = Payment.List().Value;
                var accounts = MonetaryAccount.List().Value;
                double fee = BankingHelper.CheckQuartalFee(uid);
                if (accounts.Count >= 1)// check if a bunq account already exists
                {
                    foreach (var item in accounts)
                    {
                        JToken acc = JToken.Parse(Convert.ToString(item));
                        string currency = acc["MonetaryAccountBank"]["daily_limit"]["currency"].ToString();
                        string active = acc["MonetaryAccountBank"]["status"].ToString();
                        string username = acc["MonetaryAccountBank"]["display_name"].ToString();
                        if (active == "ACTIVE")
                        {
                            if (currency == "EUR")
                            {

                                string balance = acc["MonetaryAccountBank"]["balance"]["value"].ToString();
                                string overdraft_limit = acc["MonetaryAccountBank"]["overdraft_limit"]["value"].ToString();

                                if (Convert.ToDouble(balance.Replace('.', ',')) >= fee || fee <= Convert.ToDouble(overdraft_limit.Replace('.', ',')))
                                {
                                    returnmsg = BankingHelper.makePayment(fee.ToString(), username, iban);
                                    returnmsg.ToString();
                                }
                                else
                                {
                                    throw new BankingException("Not enough money to make the transaction");
                                }
                            }
                            else
                            {
                                throw new BankingException("Currency not supported. Please set Currency to Euro!");
                            }
                        }
                    }


                }

                return returnmsg.ToString();
            }
            else
            {
                Console.WriteLine("Error with loading api context");
                return returnmsg.ToString();
            }
            
        }


        public static string createPayment(string iban, string uid,double amount)
        {
            
            if(LoadApiContext())
            {
                var receivedpayments = Payment.List().Value;
                var accounts = MonetaryAccount.List().Value;

                if (accounts.Count >= 1)// check if a bunq account already exists
                {
                    foreach (var item in accounts)
                    {

                        JToken acc = JToken.Parse(Convert.ToString(item));
                        string currency = acc["MonetaryAccountBank"]["daily_limit"]["currency"].ToString();
                        string active = acc["MonetaryAccountBank"]["status"].ToString();
                        string username = acc["MonetaryAccountBank"]["display_name"].ToString();
                        if (active == "ACTIVE")
                        {
                            if (currency == "EUR")
                            {

                                string balance = acc["MonetaryAccountBank"]["balance"]["value"].ToString();
                                string overdraft_limit = acc["MonetaryAccountBank"]["overdraft_limit"]["value"].ToString();

                                if (Convert.ToDouble(balance.Replace('.', ',')) >= amount || amount <= Convert.ToDouble(overdraft_limit.Replace('.', ',')))
                                {
                                    var returnmsg = BankingHelper.makePayment(amount.ToString(), username, iban);
                                    returnmsg.ToString();
                                }
                                else
                                {
                                    throw new BankingException("Not enough money to make the transaction");
                                }
                            }
                            else
                            {
                                throw new BankingException("Currency not supported. Please set Currency to Euro!");
                            }
                        }
                    }


                }

                return "success";
            }
            else
            {
                return "Error with loading api context";
            }

            
        }

        public static bool LoadApiContext()
        {
            try
            {
                var apiContext = ApiContext.Restore(@"C:\SimplyTrader\apicontext\context.txt");
                BunqContext.LoadApiContext(apiContext);
                return true;
            }
            catch
            {
                throw new BankingException("Unable to restore the api context");
            }
        }


    }
}
