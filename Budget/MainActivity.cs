using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Budget.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace Budget
{
    [Activity(Label = "Download Tracker", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            LinearLayout SeriesMain = FindViewById<LinearLayout>(Resource.Id.series_main_layout);
            Button AddSeriesBtn = FindViewById<Button>(Resource.Id.add_series_btn);

            AddSeriesBtn.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    Intent AddSeriesActivity = new Intent(this, typeof(AddSeries));
                    StartActivity(AddSeriesActivity);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            };

            SetSeriesView();
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);
            LinearLayout SeriesMain = FindViewById<LinearLayout>(Resource.Id.series_main_layout);

            loading.Visibility = ViewStates.Visible;
            SeriesMain.RemoveAllViews();

            SetSeriesView();
        }

        public async void SetSeriesView()
        {
            LinearLayout SeriesMain = FindViewById<LinearLayout>(Resource.Id.series_main_layout);
            TextView textMessage = FindViewById<TextView>(Resource.Id.message);
            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);

            string URL = "https://download-tracker.herokuapp.com/series/all";

            using (var client = new HttpClient())
            {
                try
                {
                    string data = await client.GetStringAsync(URL);
                    dynamic jsonData = JsonConvert.DeserializeObject(data);
                    loading.Visibility = ViewStates.Gone;

                    foreach (var series in jsonData)
                    {
                        View SeriesView = LayoutInflater.From(this).Inflate(Resource.Layout.Series_layout, null);
                        SeriesMain.AddView(SeriesView);

                        TextView Title = SeriesView.FindViewById<TextView>(Resource.Id.series_name);
                        TextView Season = SeriesView.FindViewById<TextView>(Resource.Id.series_season);
                        TextView Episode = SeriesView.FindViewById<TextView>(Resource.Id.series_episode);
                        TextView EditBtn = SeriesView.FindViewById<TextView>(Resource.Id.edit_series_btn);

                        Title.Text = series.title;
                        Season.Text = "Season " + series.season;
                        Episode.Text = "Episode " + series.episode;

                        JObject SeriesObj = new JObject
                        {
                            ["id"] = series.id,
                            ["title"] = series.title,
                            ["season"] = series.season,
                            ["episode"] = series.episode
                        };

                        EditBtn.Click += delegate (object sender, EventArgs e)
                        {
                            try
                            {
                                Intent EditSeriesActivity = new Intent(this, typeof(EditSeries));
                                EditSeriesActivity.PutExtra("Series", SeriesObj.ToString());
                                StartActivity(EditSeriesActivity);
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine(err.Message);
                            }
                        };

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void ShowToast(string toastMsg)
        {
            Toast.MakeText(this, toastMsg, ToastLength.Short).Show();
        }
    }
}

