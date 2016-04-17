using System;
using Xamarin.Forms;
using System.Diagnostics;
using XLabs.Platform.Services;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace Capp2
{
	public class TextTemplatePageCloudSync : ContentPage
	{
		public string BOMTextTemplate{ get; set;}
		public string MeetupLoc{get;set;}
		public string MeetupDate{get;set;}
		public string Smiley{ get; set;}
		public bool useSmiley{get;set;}
		Editor SMSEntry;
		Button cmdSMS;
		Label lbl;
		string Name;
		bool AutoCall;
		ContactDataItemAzure person;

		public TextTemplatePageCloudSync (string name, string meetupLoc, string meetupDate, bool smiley)
		{
			if(smiley) Smiley = ":)";
			BOMTextTemplate = " we can meet at "+meetupLoc+". We introduce our guests to the speaker then all go up together. " +
				"See you "+meetupDate+" "+Smiley+". Please reply if recieved ";

			Content = createUI ();
		}
		public TextTemplatePageCloudSync (ContactDataItemAzure person, bool autocall)
		{
			AutoCall = autocall;
			this.Name = person.Name;
			this.person = person;

			Content = createUI ();
		}
		StackLayout createUI(){
			BindingContext = new SettingsViewModel();

			SMSEntry = new Editor ();
			SMSEntry.SetBinding<SettingsViewModel> (Editor.TextProperty, vm => vm.BOMTemplateSettings);

			cmdSMS = new Button {Text = "Send"};
			cmdSMS.Clicked += (sender, e) => {
				/*if(DependencyService.Get<IPhoneService>().CanSendSMS){
					DependencyService.Get<IPhoneService>().SendSMS ("09163247357", "TESTING AUTO TEXT");
				}*/
				DependencyService.Get<IPhoneContacts>().SendSMS (person.Number, "Hi " + this.Name + " ," + SMSEntry.Text, person.Name, Values.BOM);

				if(AutoCall){
					Debug.WriteLine ("SENDING DONEWITHCALL MESSAGE");
					MessagingCenter.Send(this, Values.DONEWITHCALL);
				}else{
					Navigation.PopModalAsync (); 
					Navigation.PopModalAsync (); 
				}
			};

			lbl = new Label{ 
				Text = "Send Text",
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
			};
			return new StackLayout{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Padding = new Thickness(20),
				Children = {
					lbl, new StackLayout{
						VerticalOptions = LayoutOptions.CenterAndExpand,
						Children = { SMSEntry,cmdSMS }
					}
				}
			};

		}
	}
}


