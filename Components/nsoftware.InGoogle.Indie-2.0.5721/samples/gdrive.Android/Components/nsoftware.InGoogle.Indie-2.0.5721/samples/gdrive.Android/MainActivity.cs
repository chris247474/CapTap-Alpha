using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using nsoftware.InGoogle;

namespace nsoftware.GdriveDemo
{
	[Activity (Label = "GDrive Demo", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private Gdrive gdrive1;
		private Oauth oauth1;
		string authString = "";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			gdrive1 = new Gdrive(this);
			oauth1 = new Oauth (this);
			gdrive1.OnSSLServerAuthentication += (object sender, GdriveSSLServerAuthenticationEventArgs e) => {
				e.Accept = true;
			};
			oauth1.OnSSLServerAuthentication += (object sender, OauthSSLServerAuthenticationEventArgs e) => {
				e.Accept = true;
			};
			oauth1.OnLaunchBrowser += (object sender, OauthLaunchBrowserEventArgs e) => {
				var uri = Android.Net.Uri.Parse(e.URL);
				var intent = new Intent(Intent.ActionView, uri);
				StartActivity (intent);
			};
			ShowMainView ();
		}

		private void ShowMainView()
		{
			// Set our view from the "Main" layout resource
			SetContentView (Resource.Layout.Main);

			EditText txtFilePath = FindViewById<EditText> (Resource.Id.txtFilePath);
			Button btnAuthorize = FindViewById<Button> (Resource.Id.btnAuthorize);
			Button btnList = FindViewById<Button> (Resource.Id.btnList);
			Button btnDelete = FindViewById<Button> (Resource.Id.btnDelete);
			Button btnDownload = FindViewById<Button> (Resource.Id.btnDownload);
			ListView Documents = FindViewById<ListView> (Resource.Id.listView1);

			if (authString != "") {
				ListDocuments ();
			}

			btnAuthorize.Click += (object sender, EventArgs e) => {
				ShowAuthorizeView();
			};

			btnList.Click += (object sender, EventArgs e) => {
				ListDocuments();
			};

			btnDelete.Click += (object sender, EventArgs e) => {
				if (gdrive1.ResourceIndex > -1) {
					try {
				  		gdrive1.DeleteResource();
				  		ListDocuments();
						Toast.MakeText(this, "Document deleted!", ToastLength.Short).Show();
					} catch (InGoogleException ex) {
						ShowMessage("Error", ex.Message);
					}
				} else {
					Toast.MakeText(this, "No document selected.", ToastLength.Short).Show();
				}
			};

			btnDownload.Click += (object sender, EventArgs e) => {
				if (gdrive1.ResourceIndex > -1) {
					string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

					// Specifying the following path will download to the public downloads folder, 
					// but this requires "WriteExternalStorage" permissions.
					// string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path;

					txtFilePath.Text = System.IO.Path.Combine(path, gdrive1.ResourceOriginalName);
					if (txtFilePath.Text == "") {
						ShowMessage("Error", "Filename Empty. Please select another document.");
					} else {
						gdrive1.LocalFile = txtFilePath.Text;
						try {
							gdrive1.DownloadFile("");
							Toast.MakeText(this, "Downloading " + gdrive1.ResourceOriginalName, ToastLength.Short).Show();
						} catch (InGoogleException ex) {
							ShowMessage("Error", ex.Message);
						}
					}
				} else {
					Toast.MakeText(this, "No document selected.", ToastLength.Short).Show();
			    }
			};

			Documents.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
				gdrive1.ResourceIndex = e.Position;
				Toast.MakeText(this, gdrive1.ResourceTitle + " selected.", ToastLength.Short).Show();
			};

		}

		private void ShowAuthorizeView()
		{
			// Set our view from the "Authorize" layout resource
			SetContentView (Resource.Layout.Authorize);

			EditText txtClientId = FindViewById<EditText> (Resource.Id.txtClientId);
			EditText txtClientSecret = FindViewById<EditText> (Resource.Id.txtClientSecret);
			EditText txtAuthString = FindViewById<EditText> (Resource.Id.txtAuthString);
			txtAuthString.Text = authString;

			Button btnDone = FindViewById<Button> (Resource.Id.btnDone);
			Button btnAuth = FindViewById<Button> (Resource.Id.btnAuth);

			// Save the auth string and return to the main view
			btnDone.Click += (object sender, EventArgs e) => {
				authString = txtAuthString.Text;
				ShowMainView();
			};

			// Attempt to authorize the application to access drive data.
			btnAuth.Click += (object sender, EventArgs e) => {
				try {
					oauth1.ClientProfile = OauthClientProfiles.cfMobile;
					oauth1.ClientId = txtClientId.Text;
					oauth1.ClientSecret = txtClientSecret.Text;
					oauth1.ServerAuthURL = "https://accounts.google.com/o/oauth2/auth";
					oauth1.ServerTokenURL = "https://accounts.google.com/o/oauth2/token";
					oauth1.AuthorizationScope = "https://www.googleapis.com/auth/drive";

					// When "GetAuthorization" is called, the "LaunchBrowser" event will fire.
					// Within this event, the authorization URL will be opened in the browser.
					txtAuthString.Text = oauth1.GetAuthorization();
				} catch (InGoogleException ex) {
					ShowMessage ("Error", ex.Message);
				}
			};
		}

		private void ListDocuments()
		{
			if (authString != "") {
				try {
					gdrive1.ResourceIndex = -1; // Clear the document properties.
					gdrive1.Authorization = authString;
					gdrive1.ListResources();
					FillDocumentsListView();
				} catch (InGoogleException ex) {
					ShowMessage ("Error", ex.Message);
				}
			} else {
				ShowMessage("Error", "Not authorized.");
			}
		}

		protected void FillDocumentsListView()
		{
			ListView Documents = FindViewById<ListView> (Resource.Id.listView1);
			string[] items = new string[gdrive1.ResourceCount];
			for (int i = 0; i < gdrive1.ResourceCount; ++i) {
				gdrive1.ResourceIndex = i;
				items [i] = gdrive1.ResourceTitle;
			}
			Documents.Adapter = new ArrayAdapter<string> (this, Android.Resource.Layout.SimpleListItem1, items);
			gdrive1.ResourceIndex = -1;
		}

		protected void ShowMessage (String title, String message)
		{
			AlertDialog dialog = new AlertDialog.Builder(this).Create();
			dialog.SetTitle(title);
			dialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
			dialog.SetMessage(message);
			dialog.SetButton("OK", (s, ev) => {});
			dialog.Show();
		}
	}
}


