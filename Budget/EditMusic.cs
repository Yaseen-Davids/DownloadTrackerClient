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
using Newtonsoft.Json.Linq;

namespace Budget
{
    [Activity(Label = "Edit song")]
    public class EditMusic : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.music_edit);

            string music = Intent.GetStringExtra("Music");

            Button CancelEditMusic = FindViewById<Button>(Resource.Id.cancel_edit_music_button);

            CancelEditMusic.Click += delegate (object sender, EventArgs e)
            {
                Finish();
            };

            SetMusic(music);
        }

        public void SetMusic(string music)
        {
            dynamic jsonData = JsonConvert.DeserializeObject(music);

            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.edit_music_title);
            TextInputEditText artist = FindViewById<TextInputEditText>(Resource.Id.edit_music_artist);
            Button Submit = FindViewById<Button>(Resource.Id.submit_edit_music_btn);
            Button DeleteMusicButton = FindViewById<Button>(Resource.Id.delete_music_btn);

            title.Text = jsonData.title;
            artist.Text = jsonData.artist;

            string id = jsonData.id.ToString();

            Submit.Click += delegate (object sender, EventArgs e)
            {
                UpdateMusic(id);
            };

            DeleteMusicButton.Click += delegate (object sender, EventArgs e)
            {
                ShowConfirmDelete("Confirm delete", "Are you sure you want to delete " + jsonData.title + " ?", id);
            };
        }

        public async void UpdateMusic(string id)
        {
            TextInputEditText title = FindViewById<TextInputEditText>(Resource.Id.edit_music_title);
            TextInputEditText artist = FindViewById<TextInputEditText>(Resource.Id.edit_music_artist);
            Button Submit = FindViewById<Button>(Resource.Id.submit_edit_music_btn);

            JObject UpdatedMusic = new JObject
            {
                ["title"] = title.Text,
                ["artist"] = artist.Text,
            };
            try
            {
                var response = await MusicAPI.UpdateMusic(UpdatedMusic, id);
                var formatted = Constants.ShowError(response["Contents"], title.Text);
                if (response["StatusCode"] == Constants.SERVER_ERROR || response["Status"] == "error")
                {
                    Constants.ShowAlert("Error", formatted["Formatted"], this);
                    return;

                }
                else if (response["Status"] == "complete" && response["StatusCode"] == "OK")
                {
                    Finish();
                    Constants.ShowToast(this, "Successfully updated!");
                }
            }
            catch (Exception e)
            {
                Constants.ShowAlert("Error", e.Message, this);
                return;
            }
        }

        public async void DeleteMusic(string id)
        {
            try
            {
                var response = await MusicAPI.DeleteMusic(id);
                var formatted = Constants.ShowError(response["Contents"], "Music");
                if (response["StatusCode"] == Constants.SERVER_ERROR || response["Status"] == "error")
                {
                    Constants.ShowAlert("Error", formatted["Formatted"], this);
                    return;

                }
                else if (response["Status"] == "complete" && response["StatusCode"] == "OK")
                {
                    Finish();
                    Constants.ShowToast(this, "Successfully deleted!");
                }
            }
            catch (Exception e)
            {
                Constants.ShowAlert("Error", e.Message, this);
                return;
            }
        }

        public void ShowConfirmDelete(string alerTitle, string alertMessage, string id)
        {
            Android.App.AlertDialog.Builder showDialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog Alert = showDialog.Create();
            Alert.SetTitle(alerTitle);
            Alert.SetMessage(alertMessage);
            Alert.SetButton("YES", delegate
            {
                try
                {
                    DeleteMusic(id);
                }
                catch (Exception e)
                {
                    Constants.ShowAlert("Error", e.Message, this);
                    return;
                }
            });
            Alert.SetButton2("NO", delegate
            {
                Alert.Dismiss();
                Alert.Dispose();
                return;
            });
            Alert.Show();
        }
    }
}