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

namespace Budget
{
    class MoviesAPI
    {
        public async static Task<string> GetMovies()
        {
            string URL = Constants.MOVIES_API + "/all";
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

        public async static Task<Dictionary<string, string>> CreateMovie(string data)
        {
            if (data != "" && data != null)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(Constants.MOVIES_API, httpContent);
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

        public async static Task<Dictionary<string, string>> UpdateMovie(object data, string id)
        {
            if (data.ToString() != "" && data != null && id != "" && id != null)
            {
                string URL = Constants.MOVIES_API + "/" + id;
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

        public async static Task<Dictionary<string, string>> DeleteMovie(string id)
        {
            if (id != "" && id != null)
            {
                string URL = Constants.MOVIES_API + "/" + id;
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
    }
}