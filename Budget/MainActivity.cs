using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace Budget
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.CanScrollHorizontally(1);

            try
            {
                OnNavigationItemSelected(navigation.Menu.GetItem(0));
            }
            catch (Exception e)
            {
                Constants.ShowAlert("Error", e.Message, this);
                return;
            }

            ImageButton AddSeriesBtn = FindViewById<ImageButton>(Resource.Id.add_series_btn);
            ImageButton AddMoviesBtn = FindViewById<ImageButton>(Resource.Id.add_movies_button);
            ImageButton AddMusicBtn = FindViewById<ImageButton>(Resource.Id.add_music_button);

            AddSeriesBtn.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    SetReturnActivity("Series");
                    Intent AddSeriesActivity = new Intent(this, typeof(AddSeries));
                    StartActivity(AddSeriesActivity);
                }
                catch (Exception err)
                {
                    Constants.ShowAlert("Error", err.Message, this);
                    return;
                }
            };
            AddMoviesBtn.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    SetReturnActivity("Movies");
                    Intent AddMovieActivity = new Intent(this, typeof(AddMovie));
                    StartActivity(AddMovieActivity);
                }
                catch (Exception err)
                {
                    Constants.ShowAlert("Error", err.Message, this);
                    return;
                }
            };
            AddMusicBtn.Click += delegate (object sender, EventArgs e)
            {
                try
                {
                    SetReturnActivity("Music");
                    Intent AddMusicActivity = new Intent(this, typeof(AddMusic));
                    StartActivity(AddMusicActivity);
                }
                catch (Exception err)
                {
                    Constants.ShowAlert("Error", err.Message, this);
                    return;
                }
            };
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            HideButtons();
            switch (item.ItemId)
            {
                case Resource.Id.navigation_series:
                    SetSeriesView();
                    return true;
                case Resource.Id.navigation_movies:
                    SetMoviesView();
                    return true;
                case Resource.Id.navigation_music:
                    SetMusicView();
                    return true;
            }
            return false;
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);
            LinearLayout MainLayout = FindViewById<LinearLayout>(Resource.Id.main_layout);
            ImageButton AddSeriesBtn = FindViewById<ImageButton>(Resource.Id.add_series_btn);
            var activityHistory = Application.Context.GetSharedPreferences("ActivtyHistory", FileCreationMode.Private);
            string returnValue = activityHistory.GetString("ReturnValue", null);

            // Show loading
            loading.Visibility = ViewStates.Visible;
            // Remove all views
            MainLayout.RemoveAllViews();
            // Hide all Add Buttons
            HideButtons();

            if (returnValue == "Series")
            {
                SetSeriesView();
                return;
            }
            else if (returnValue == "Movies")
            {
                SetMoviesView();
                return;
            }
            else if (returnValue == "Music")
            {
                SetMusicView();
                return;
            }
        }

        public async void SetSeriesView()
        {
            LinearLayout MainLayout = FindViewById<LinearLayout>(Resource.Id.main_layout);
            TextView textMessage = FindViewById<TextView>(Resource.Id.message);
            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);
            ImageButton AddSeriesBtn = FindViewById<ImageButton>(Resource.Id.add_series_btn);

            ResetMain();

            try
            {
                string data = await SeriesAPI.GetSeries();
                dynamic jsonData = JsonConvert.DeserializeObject(data);
                loading.Visibility = ViewStates.Gone;

                foreach (var series in jsonData)
                {
                    View SeriesView = LayoutInflater.From(this).Inflate(Resource.Layout.Series_layout, null);
                    MainLayout.AddView(SeriesView);

                    TextView Title = SeriesView.FindViewById<TextView>(Resource.Id.series_name);
                    TextView SeasonDownloaded = SeriesView.FindViewById<TextView>(Resource.Id.series_season_downloaded);
                    TextView EpisodeDownloaded = SeriesView.FindViewById<TextView>(Resource.Id.series_episode_downloaded);
                    TextView SeasonWatched = SeriesView.FindViewById<TextView>(Resource.Id.series_season_watched);
                    TextView EpisodeWatched = SeriesView.FindViewById<TextView>(Resource.Id.series_episode_watched);
                    TextView EditBtn = SeriesView.FindViewById<TextView>(Resource.Id.edit_series_btn);
                    LinearLayout SeriesMainSelector = SeriesView.FindViewById<LinearLayout>(Resource.Id.series_main_selector);

                    Title.Text = series.title;

                    SeasonDownloaded.Text = "Season " + series.season;
                    SeasonWatched.Text = "Season " + series.season;

                    EpisodeWatched.Text = "Episode " + series.episode;
                    EpisodeDownloaded.Text = "Episode " + series.episode;

                    JObject SeriesObj = new JObject
                    {
                        ["id"] = series.id,
                        ["title"] = series.title,
                        ["season"] = series.season,
                        ["episode"] = series.episode
                    };

                    SeriesMainSelector.Click += delegate (object sender, EventArgs e)
                    {
                        Intent EditSeriesActivity = new Intent(this, typeof(SeriesMain));
                        EditSeriesActivity.PutExtra("Series", SeriesObj.ToString());
                        SetReturnActivity("Series");
                        StartActivity(EditSeriesActivity);
                    };

                    EditBtn.Click += delegate (object sender, EventArgs e)
                    {
                        try
                        {
                            Intent EditSeriesActivity = new Intent(this, typeof(EditSeries));
                            EditSeriesActivity.PutExtra("Series", SeriesObj.ToString());
                            SetReturnActivity("Series");
                            StartActivity(EditSeriesActivity);
                        }
                        catch (Exception err)
                        {
                            Constants.ShowAlert("Error", err.Message, this);
                            return;
                        }
                    };

                }
                AddSeriesBtn.Visibility = ViewStates.Visible;

            }
            catch (Exception e)
            {
                Constants.ShowAlert("Error", e.Message, this);
                return;
            }
        }

        public async void SetMoviesView()
        {
            LinearLayout MainLayout = FindViewById<LinearLayout>(Resource.Id.main_layout);
            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);
            ImageButton AddMoviesBtn = FindViewById<ImageButton>(Resource.Id.add_movies_button);

            ResetMain();

            string URL = "https://download-tracker.herokuapp.com/movies/all";

            using (var client = new HttpClient())
            {
                try
                {
                    string data = await client.GetStringAsync(URL);
                    dynamic jsonData = JsonConvert.DeserializeObject(data);
                    loading.Visibility = ViewStates.Gone;

                    foreach (var movie in jsonData)
                    {
                        View MoviesView = LayoutInflater.From(this).Inflate(Resource.Layout.Movies_layout, null);
                        MainLayout.AddView(MoviesView);

                        TextView Title = MoviesView.FindViewById<TextView>(Resource.Id.movies_name);
                        TextView EditBtn = MoviesView.FindViewById<TextView>(Resource.Id.edit_movies_btn);

                        Title.Text = movie.title;

                        JObject MovieObj = new JObject
                        {
                            ["id"] = movie.id,
                            ["title"] = movie.title,
                        };

                        EditBtn.Click += delegate (object sender, EventArgs e)
                        {
                            try
                            {
                                Intent EditMovieActivity = new Intent(this, typeof(EditMovie));
                                EditMovieActivity.PutExtra("Movie", MovieObj.ToString());
                                SetReturnActivity("Movies");
                                StartActivity(EditMovieActivity);
                            }
                            catch (Exception err)
                            {
                                Constants.ShowAlert("Error", err.Message, this);
                                return;
                            }
                        };

                    }
                    AddMoviesBtn.Visibility = ViewStates.Visible;
                }
                catch (Exception e)
                {
                    Constants.ShowAlert("Error", e.Message, this);
                    return;
                }
            }
        }

        public async void SetMusicView()
        {
            LinearLayout MainLayout = FindViewById<LinearLayout>(Resource.Id.main_layout);
            TextView textMessage = FindViewById<TextView>(Resource.Id.message);
            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);
            ImageButton AddMusicBtn = FindViewById<ImageButton>(Resource.Id.add_music_button);

            ResetMain();

            try
            {
                string data = await MusicAPI.GetMusic();
                dynamic jsonData = JsonConvert.DeserializeObject(data);
                loading.Visibility = ViewStates.Gone;

                foreach (var music in jsonData)
                {
                    View MusicView = LayoutInflater.From(this).Inflate(Resource.Layout.Music_layout, null);
                    MainLayout.AddView(MusicView);

                    TextView Title = MusicView.FindViewById<TextView>(Resource.Id.music_name);
                    TextView Artist = MusicView.FindViewById<TextView>(Resource.Id.music_artist);
                    Button EditBtn = MusicView.FindViewById<Button>(Resource.Id.edit_music_btn);

                    Title.Text = music.title;
                    Artist.Text = "By " + music.artist;

                    JObject MusicObj = new JObject
                    {
                        ["id"] = music.id,
                        ["title"] = music.title,
                        ["artist"] = music.artist,
                    };

                    EditBtn.Click += delegate (object sender, EventArgs e)
                    {

                        try
                        {
                            Intent EditMusicActivity = new Intent(this, typeof(EditMusic));
                            EditMusicActivity.PutExtra("Music", MusicObj.ToString());
                            SetReturnActivity("Music");
                            StartActivity(EditMusicActivity);
                        }
                        catch (Exception err)
                        {
                            Constants.ShowAlert("Error", err.Message, this);
                            return;
                        }
                    };

                }
                AddMusicBtn.Visibility = ViewStates.Visible;
            }
            catch (Exception e)
            {
                Constants.ShowAlert("Error", e.Message, this);
                return;
            }
        }

        public void HideButtons()
        {
            ImageButton AddMoviesBtn = FindViewById<ImageButton>(Resource.Id.add_movies_button);
            ImageButton AddSeriesBtn = FindViewById<ImageButton>(Resource.Id.add_series_btn);
            ImageButton AddMusicBtn = FindViewById<ImageButton>(Resource.Id.add_music_button);
            AddMoviesBtn.Visibility = ViewStates.Gone;
            AddSeriesBtn.Visibility = ViewStates.Gone;
            AddMusicBtn.Visibility = ViewStates.Gone;
        }

        public void ResetMain()
        {
            LinearLayout MainLayout = FindViewById<LinearLayout>(Resource.Id.main_layout);
            LinearLayout loading = FindViewById<LinearLayout>(Resource.Id.loading_data);
            loading.Visibility = ViewStates.Visible;
            MainLayout.RemoveAllViews();
        }
 
        public void SetReturnActivity(string returnValue)
        {
            var activityHistory = Application.Context.GetSharedPreferences("ActivtyHistory", FileCreationMode.Private);
            var activityHistoryEdit = activityHistory.Edit();

            activityHistoryEdit.Clear();
            activityHistoryEdit.Commit();

            activityHistoryEdit.PutString("ReturnValue", returnValue);
            activityHistoryEdit.Commit();
        }
    }
}

