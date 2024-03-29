﻿using LifeCalculator.Framework.Accounts;
using LifeCalculator.Framework.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace LifeCalculator.Framework.Services.PlaidService
{
    public static class PlaidService
    {
        public static void StartPlaidLink(string initToken = null)
        {
            if (AppSettings.Instance.PlaidApiSettings.Environment == Enums.Environment.Sandbox)
            {
                string message = "You are running Plaid in the Sandbox environment. Credentials for all institutions are as follows:\n\n" +
                                 "username: user_good\n" +
                                 "password: pass_good";
                MessageBox.Show(message);
            }

            string plaidLinkHTML;
            if (!string.IsNullOrEmpty(initToken))
            {
                plaidLinkHTML = "<script src=\"https://cdn.plaid.com/link/v2/stable/link-initialize.js\"></script>" +
                       "<script type=\"text/javascript\">" +
                           "function myFunction() {" +
                               "var handler = Plaid.create({" +
                                   $"token: '{initToken}'," +
                                   "onSuccess: function(public_token, metadata) {" +
                                       "alert('Successfully updated the access token!');" +
                                       "window.close();" +
                                   "}," +
                                   "onExit: function(err, metadata) {}," +
                               "});" +

                               "handler.open();" +
                           "};" +

                           "window.onload = myFunction;" +
                       "</script>";
            }
            else
            {
                plaidLinkHTML = "<script src=\"https://cdn.plaid.com/link/v2/stable/link-initialize.js\"></script>" +
                       "<script type=\"text/javascript\">" +
                           "function myFunction() {" +
                               "var handler = Plaid.create({" +
                                   "clientName: 'Plaid Demo'," +
                                   "env: '" + AppSettings.Instance.PlaidApiSettings.Environment.ToString().ToLower() + "'," +
                                   "key: '" + AppSettings.Instance.PlaidApiSettings.PublicKey + "'," +
                                   "product: ['transactions']," +
                                   "onLoad: function() {}," +
                                   "onSuccess: function(public_token, metadata) {" +
                                       "prompt('Please copy the institution id below and insert it into the application:', metadata.institution.institution_id);" +
                                       "prompt('Please copy the public token below and insert it into the application:', public_token);" +
                                       "window.close();" +
                                   "}," +
                                   "onExit: function(err, metadata) {}," +
                                   "onEvent: function(eventName, metadata) {}" +
                               "});" +

                               "handler.open();" +
                           "};" +

                           "window.onload = myFunction;" +
                       "</script>";
            }

            string tempPath = Path.GetTempFileName().Replace(".tmp", ".html");
            File.WriteAllText(tempPath, plaidLinkHTML);
            Process.Start(tempPath);
        }

        public static Credentials AuthorizeInstitution(Institution institution, string public_token)
        {
            JObject json = JObject.FromObject(new { client_id = AppSettings.Instance.PlaidApiSettings.ClientId, secret = GetSecret(), public_token });
            JObject result = HttpRequestToPlaid("item/public_token/exchange", json);
            string access_token = result["access_token"].ToString();
            string item_id = result["item_id"].ToString();

            return new Credentials() { AccessToken = access_token, Item = item_id };
        }

        public static void RefreshToken(string currentToken)
        {
            JObject json = JObject.FromObject(new
            {
                user = new
                {
                    client_user_id = Guid.NewGuid(),
                },
                client_id = AppSettings.Instance.PlaidApiSettings.ClientId,
                client_name = "Plaid Demo",
                country_codes = new[] { "US" },
                language = "en",
                secret = GetSecret(),
                access_token = currentToken
            });

            JObject response = HttpRequestToPlaid("link/token/create", json);
            StartPlaidLink(response["link_token"].Value<string>());
        }

        public static List<Institution> GetAllInstitutions()
        {
            List<Institution> allInstitutions = new List<Institution>();
            int offset = 0;
            int count = 500;
            int total = int.MaxValue;
            while (offset < total)
            {
                JObject result = GetInstitutions(count, offset);
                List<JToken> jsonInstitutions = result["institutions"].ToList();
                allInstitutions.AddRange(jsonInstitutions.Select(p => ConvertToInstitution(p)));

                total = (int)result["total"];
                offset += count;
            }

            return allInstitutions;
        }

        public static JObject GetInstitutions(int count, int offset)
        {
            JObject json = JObject.FromObject(new { client_id = AppSettings.Instance.PlaidApiSettings.ClientId, secret = GetSecret(), count, offset });
            return HttpRequestToPlaid("institutions/get", json);
        }

        public static Institution GetInstitutionById(string institution_id)
        {
            JObject json = JObject.FromObject(new { public_key = AppSettings.Instance.PlaidApiSettings.PublicKey, institution_id });
            JObject result = HttpRequestToPlaid("institutions/get_by_id", json);
            JToken jsonInstitution = result["institution"];

            return ConvertToInstitution(jsonInstitution);
        }

        public static List<Institution> GetInstitutionBySearch(string query, string[] products = null)
        {
            List<Institution> institutions = new List<Institution>();

            JObject json = JObject.FromObject(new { public_key = AppSettings.Instance.PlaidApiSettings.PublicKey, query, products });
            JObject result = HttpRequestToPlaid("institutions/search", json);
            List<JToken> jsonInstitutions = result["institutions"].ToList();
            institutions.AddRange(jsonInstitutions.Select(p => ConvertToInstitution(p)));

            return institutions;
        }

        private static Institution ConvertToInstitution(JToken jsonInstitution)
        {
            Institution result = new Institution();
            result.Name = jsonInstitution["name"].ToString();
            result.PlaidId = jsonInstitution["institution_id"].ToString();

            return result;
        }

        private static Account ConvertToAccount(JToken jsonAccount)
        {
            Account result = new Account();
            result.Name = jsonAccount["name"].ToString();
            result.PlaidID = jsonAccount["account_id"].ToString();

            Enums.PlaidAccountType accountType;
            bool success = Enum.TryParse(jsonAccount["subtype"].ToString(), true, out accountType);
            result.Type = accountType;

            if (accountType == Enums.PlaidAccountType.CreditCard || accountType == Enums.PlaidAccountType.Other)
                result.AvailableBalance = (double)jsonAccount["balances"]["current"];
            else
                result.AvailableBalance = (double)jsonAccount["balances"]["available"];

            return result;
        }

        private static TransactionItem ConvertToTransaction(JToken jsonTransaction)
        {
            TransactionItem result = new TransactionItem();
            result.Name = jsonTransaction["name"].ToString();
            result.TransactionId = jsonTransaction["transaction_id"].ToString();

            result.Amount = (double)jsonTransaction["amount"];

            result.Date = DateTime.Parse(jsonTransaction["date"].ToString());
            result.AccountId = jsonTransaction["account_id"].ToString();
            result.BudgetCategory = jsonTransaction["category"].ToString();
            result.BudgetCategoryPlaidDefault = jsonTransaction["category"].ToString();

            return result;
        }

        public static List<Account> GetInstitutionAccounts(Institution institution)
        {
            List<Account> accounts = new List<Account>();

            JObject json = JObject.FromObject(new
            {
                client_id = AppSettings.Instance.PlaidApiSettings.ClientId,
                secret = GetSecret(),
                access_token = institution.Credentials.AccessToken
            });
            JObject result = HttpRequestToPlaid("accounts/get", json);
            List<JToken> jsonAccounts = result["accounts"].ToList();
            accounts.AddRange(jsonAccounts.Select(p => ConvertToAccount(p)));

            return accounts;
        }

        public static List<TransactionItem> GetTransactions(Institution institution, DateTime startDate, DateTime endDate)
        {
            List<TransactionItem> transactions = new List<TransactionItem>();

            string start_date = startDate.ToString("yyyy-MM-dd");
            string end_date = endDate.ToString("yyyy-MM-dd");

            Dictionary<string,int> properties = new Dictionary<string,int>();
            properties.Add("count", 250);

            JObject json = JObject.FromObject(new
            {
                client_id = AppSettings.Instance.PlaidApiSettings.ClientId,
                secret = GetSecret(),
                access_token = institution.Credentials.AccessToken,
                start_date,
                end_date,
                options = properties
            });
            JObject result = HttpRequestToPlaid("transactions/get", json);
            List<JToken> jsonTransactions = result["transactions"].ToList();
            transactions.AddRange(jsonTransactions.Select(p => ConvertToTransaction(p)));

            return transactions;
        }

        public static List<Account> AddTransactionsToAccounts(List<Account> accounts, List<TransactionItem> transactions)
        {
            foreach (TransactionItem transaction in transactions)
            {
                Account matchingAccount = accounts.Find(p => p.PlaidID == transaction.AccountId);
                if (matchingAccount != null && matchingAccount.RecentTransactions.Find(p => p.TransactionId == transaction.TransactionId) == null)
                    matchingAccount.RecentTransactions.Add(transaction);
                else
                    Console.WriteLine("Account not found or transaction already exists in list");
            }

            return accounts;
        }

        

        public static void DeauthorizeInstitution(Institution institution)
        {
            JObject json = JObject.FromObject(new
            {
                client_id = AppSettings.Instance.PlaidApiSettings.ClientId,
                secret = GetSecret(),
                access_token = institution.Credentials.AccessToken
            });
            JObject result = HttpRequestToPlaid("item/remove", json);
        }

        private static string GetBaseUrl()
        {
            switch (AppSettings.Instance.PlaidApiSettings.Environment)
            {
                case Enums.Environment.Development:
                    return "https://development.plaid.com/";
                case Enums.Environment.Sandbox:
                default:
                    return "https://sandbox.plaid.com/";
            }
        }

        private static string GetSecret()
        {
            switch (AppSettings.Instance.PlaidApiSettings.Environment)
            {
                case Enums.Environment.Development:
                    return AppSettings.Instance.PlaidApiSettings.SecretKey;
                case Enums.Environment.Sandbox:
                default:
                    return AppSettings.Instance.PlaidApiSettings.SandboxSecret;
            }
        }

        private static JObject HttpRequestToPlaid(string apiEndpoint, JObject payload)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetBaseUrl() + apiEndpoint);
            request.ContentType = "application/json";
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(payload.ToString());
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            try
            {
                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string Response = reader.ReadToEnd();
                JObject resultJson = JObject.Parse(Response);

                return resultJson;
            }
            catch (WebException ex)
            {
                string errorMessage = string.Format("Error Status Code : {1} {0}", ((HttpWebResponse)ex.Response).StatusCode,
                                                                                   (int)((HttpWebResponse)ex.Response).StatusCode);
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    errorMessage += "\nError Response: " + reader.ReadToEnd();
                }

                if (errorMessage.Contains("ITEM_LOGIN_REQUIRED"))
                {
                    MessageBox.Show("Credentials need to be updated. Please go to Settings, right-click on the institution and select \"Update\"");
                }
                else
                {
                    MessageBox.Show("Error contacting Plaid: \n\n" + errorMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception thrown:\n\n" + ex.Message);
            }

            return null;
        }
    }
}
