using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Budget
{
    class SeriesAPI
    {
        public async static Task<string> GetSeries()
        {
            string URL = Constants.SERIES_API + "/all";
            using (var client = new HttpClient())
            {
                try
                {
                    string data = await client.GetStringAsync(URL);
                    return data;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        public async static Task<Dictionary<string, string>> CreateSeries(string data)
        {
            if (data != "" && data != null)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(Constants.SERIES_API, httpContent);
                        return new Dictionary<string, string>
                        {
                            ["StatusCode"] = response.StatusCode.ToString(),
                            ["Contents"] = response.Content.ReadAsStringAsync().Result.ToString(),
                            ["Status"] = "complete",
                        };
                    }
                    catch (Exception e)
                    {
                        return new Dictionary<string, string>
                        {
                            ["Status"] = "error",
                            ["Contents"] = e.Message
                        };
                    }
                }
            }
            return new Dictionary<string, string>
            {
                ["Status"] = "error",
                ["Contents"] = "Data cannot be empty"
            };
        }

        public async static Task<Dictionary<string, string>> UpdateSeries(object data, string id)
        {
            if (data.ToString() != "" && data != null && id != "" && id != null)
            {
                string URL = Constants.SERIES_API + "/" + id;
                using (var client = new HttpClient())
                {
                    try
                    {
                        var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                        var response = await client.PutAsync(URL, httpContent);
                        return new Dictionary<string, string>
                        {
                            ["Status"] = "complete",
                            ["StatusCode"] = response.StatusCode.ToString(),
                            ["Contents"] = response.Content.ReadAsStringAsync().Result.ToString(),
                        };
                    }
                    catch (Exception e)
                    {
                        return new Dictionary<string, string>
                        {
                            ["Status"] = "error",
                            ["Contents"] = e.Message.ToString(),
                        };
                    }
                }
            }
            return new Dictionary<string, string>
            {
                ["Status"] = "error",
                ["Contents"] = "Data cannot be empty",
            };
        }

        public async static Task<Dictionary<string, string>> DeleteSeries(string id)
        {
            if (id != "" && id != null)
            {
                string URL = Constants.SERIES_API + "/" + id;
                using (var client = new HttpClient())
                {
                    try
                    {
                        var response = await client.DeleteAsync(URL);
                        return new Dictionary<string, string>
                        {
                            ["Status"] = "complete",
                            ["StatusCode"] = response.StatusCode.ToString(),
                            ["Contents"] = response.Content.ReadAsStringAsync().Result.ToString(),
                        };
                    }
                    catch (Exception e)
                    {
                        return new Dictionary<string, string>
                        {
                            ["Status"] = "error",
                            ["Contents"] = e.Message.ToString(),
                        };
                    }
                }
            }
            return new Dictionary<string, string>
            {
                ["Status"] = "error",
                ["Contents"] = "Data cannot be empty",
            };
        }

        public async static Task<Dictionary<string, string>> NextEpisode(string name, string season)
        {
            if ((name != "" && name != null) || (season != "" && season != null))
            {
                string URL = Constants.SEARCH_SERIES_API + "/search/" + name + "/" + season;
                using (var client = new HttpClient())
                {
                    try
                    {
                        var response = await client.GetAsync(URL);
                        return new Dictionary<string, string>
                        {
                            ["Status"] = "complete",
                            ["StatusCode"] = response.StatusCode.ToString(),
                            ["Contents"] = response.Content.ReadAsStringAsync().Result.ToString(),
                        };
                    }
                    catch (Exception e)
                    {
                        return new Dictionary<string, string>
                        {
                            ["Status"] = "error",
                            ["Contents"] = e.Message.ToString(),
                        };
                    }
                }
            }
            return new Dictionary<string, string>
            {
                ["Status"] = "error",
                ["Contents"] = "Data cannot be empty",
            };
        }
    }
}