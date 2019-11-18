using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using System.Threading.Tasks;

namespace Budget
{
    class Constants
    {
        public static string SERIES_API = "https://download-tracker.herokuapp.com/series";
        public static string MOVIES_API = "https://download-tracker.herokuapp.com/movies";
        public static string MUSIC_API = "https://download-tracker.herokuapp.com/music";
        public static string SEARCH_SERIES_API = "https://download-tracker.herokuapp.com/searches";

        public static string SERVER_ERROR = "InternalServerError";

        public static void ShowAlert(string alerTitle, string alertMessage, Context context)
        {
            Android.App.AlertDialog.Builder showDialog = new Android.App.AlertDialog.Builder(context);
            Android.App.AlertDialog Alert = showDialog.Create();
            Alert.SetTitle(alerTitle);
            Alert.SetMessage(alertMessage);
            Alert.SetButton2("OK", delegate
            {
                Alert.Dismiss();
                Alert.Dispose();
                return;
            });
            Alert.Show();
        }

        public static void ShowToast(Context context, string msg)
        {
            Toast.MakeText(context, msg, ToastLength.Short).Show();
        }

        public static Dictionary<string, string> ShowError(string title, string type)
        {
            if (title.Contains("duplicate key value"))
            {
                return new Dictionary<string, string>
                {
                    ["Formatted"] = type + " already exists",
                };
            }
            else if (title.Contains("invalid input syntax for type real"))
            {
                return new Dictionary<string, string>
                {
                    ["Formatted"] = "Field(s) cannot be empty",
                };
            }
            return new Dictionary<string, string>
            {
                ["Formatted"] = title,
            };
        }
    }
}