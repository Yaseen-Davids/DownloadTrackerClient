using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Budget
{
    [Activity(Label = "Series Info")]
    public class SeriesMain : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SeriesMainLayout);

            // Get data from intent
            string series_data = Intent.GetStringExtra("Series");
            SetData(series_data);
        }

        public async void SetData(string data)
        {
            dynamic jsonData = JsonConvert.DeserializeObject(data);

            TextView title = FindViewById<TextView>(Resource.Id.series_main_name);
            TextView season_watched = FindViewById<TextView>(Resource.Id.series_main_watched_season);
            TextView episode_watched = FindViewById<TextView>(Resource.Id.series_main_watched_episode);
            TextView season_downloaded = FindViewById<TextView>(Resource.Id.series_main_downloaded_season);
            TextView episode_downloaded = FindViewById<TextView>(Resource.Id.series_main_downloaded_episode);
            LinearLayout loading_series = FindViewById<LinearLayout>(Resource.Id.loading_series_main_data);

            string seriesTitle = jsonData.title;
            string seriesSeason = jsonData.season;

            title.Text = seriesTitle;
            season_downloaded.Text = "Season " + seriesSeason;
            episode_downloaded.Text = "Episode "+ jsonData.episode;
            season_watched.Text = "Season " + seriesSeason;
            episode_watched.Text = "Episode " + jsonData.episode;

            Dictionary<string, string> nextEpisodeData = await SeriesAPI.NextEpisode(seriesTitle, seriesSeason);
            string objData = nextEpisodeData["Contents"];
            dynamic respData = JsonConvert.DeserializeObject(objData.ToString());

            Console.WriteLine(respData);
            loading_series.Visibility = ViewStates.Gone;
        }

    }
}