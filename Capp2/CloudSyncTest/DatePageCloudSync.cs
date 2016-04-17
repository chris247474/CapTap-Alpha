using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using Plugin.Calendars.Abstractions;
using System.Threading.Tasks;

namespace Capp2
{
	public class DatePageCloudSync : ContentPage
	{
		bool yescall = true;
		public DatePicker datePicker{ get; set;}
		TimePicker timePicker{get;set;}
		bool AutoCall;
		string Name;

		public DatePageCloudSync (string whichCapp, ContactDataItemAzure personCalled, bool autocall)
		{
			Debug.WriteLine ("Entered DatePageCloudSync");

			//BackgroundColor = Color.Transparent;
			AutoCall = autocall;
			//this.SetBinding (ContentPage.TitleProperty, "Name");
			Name = personCalled.Name;

			//NavigationPage.SetHasNavigationBar (this, true);

			Label lbl = new Label{
				Text = "When did we book " + personCalled.Name + "?",
				FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)),
				FontAttributes = FontAttributes.Bold,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
			};

			timePicker = new TimePicker {
				Format = "t",
				IsEnabled = false
			};

			datePicker = new DatePicker
			{
				Format = "D",
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};
			datePicker.DateSelected += (dateSender, de) => {
				if(!yescall){
					SetAppointmentHandler (whichCapp, personCalled);
				}
				else if(yescall){
					timePicker.IsEnabled = true;
					timePicker.PropertyChanged += (sender,e) =>
					{
						if(e.PropertyName == TimePicker.TimeProperty.PropertyName)
						{
							SetAppointmentHandler (whichCapp, personCalled);
						}

					};
				}
			};


			var cancelButton = new Button { 
				Text = personCalled.Name+" hasn't said yes yet" 
			};
			cancelButton.Clicked += (sender, e) => {
				if(cancelButton.Text.Contains(" hasn't said yes yet")){
					lbl.Text = "When should we call "+personCalled.Name+" again?";
					yescall = false;
					cancelButton.Text = "I mean I booked "+personCalled.Name;
					DisableTimePicker (1);
				}else {
					yescall = true;
					lbl.Text = "When did we book "+personCalled.Name+"?";
					cancelButton.Text = personCalled.Name+" hasn't said yes yet";
				}
			};

			Content = new StackLayout {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Padding = new Thickness(20),
				Children = {
					lbl, new StackLayout{
						VerticalOptions = LayoutOptions.CenterAndExpand,
						Children = { datePicker, timePicker,}
					}, cancelButton
				}
			};

			Debug.WriteLine ("Finished DatePageCloudSync()");
		}
		public async void SetAppointmentHandler(string whichCapp, ContactDataItemAzure personCalled){
			if(yescall){	
				//text appointed person

				switch (whichCapp) {
				case Values.NEXT:
					Debug.WriteLine ("ABOUT TO RESCHED: NEXTMEETINGID "+personCalled.NextMeetingID);
					await DependencyService.Get<ICalendar>().ReschedAppointment(personCalled.NextMeetingID, datePicker.Date.AddHours (timePicker.Time.Hours));

					App.AzureDB.UpdateItem (personCalled);
					await this.Navigation.PopModalAsync();	
					break;
				case Values.APPOINTED:
					personCalled.Appointed = datePicker.Date;

					//always first time to give nextmeeting real values
					personCalled.NextMeetingID = await DependencyService.Get<ICalendar> ().CreateAppointment (personCalled.NextMeetingID, personCalled.Name, Values.APPOINTMENTDESCRIPTIONBOM, datePicker.Date.AddHours (timePicker.Time.Hours));
					Debug.WriteLine ("[DatePage - Appointed] NextMeetingID: " + personCalled.NextMeetingID);

					App.AzureDB.UpdateItem (personCalled);
					//Navigation.PopModalAsync ();

					await Navigation.PushModalAsync (new TextTemplatePageCloudSync(personCalled, AutoCall));//or dependency call OS specific default messaging app

					break;
				case Values.PRESENTED:
					personCalled.Presented = datePicker.Date;

					App.AzureDB.UpdateItem(personCalled);

					//Navigation.PushModalAsync (new FollowUp(personCalled));//for follow ups
					await this.Navigation.PopModalAsync();	//test
					break;
				case Values.PURCHASED:
					//add date to contact's "purchased" property
					personCalled.Purchased = datePicker.Date;
					App.AzureDB.UpdateItem(personCalled);
					await this.Navigation.PopModalAsync();	
					break;
				}


			}else{//if no call
				if (AutoCall) {
					MessagingCenter.Send(this, Values.DONEWITHNOCALL);
				} else {
					personCalled.NextCall = datePicker.Date;
					App.AzureDB.UpdateItem (personCalled);
					//App.AzureDB.SyncAsync ();
					Debug.WriteLine ("NextCall is "+personCalled.NextCall.Day.ToString ());
					Navigation.PopModalAsync ();
				}
				//backup DB to cloud?
			}


			//ReadyForNext = true;

		}
		public void DisableTimePicker(int defaultHour){
			timePicker.Time = TimeSpan.FromHours (defaultHour);
			timePicker.IsEnabled = false;
		}
		public void SubscribeForPopSelf(){
			MessagingCenter.Subscribe<TextTemplatePage>(this, Values.DONEWITHCALL, async (args) =>{ 
				Debug.WriteLine ("POPPING DATEPAGE OF "+this.Name);
				await Navigation.PopModalAsync ();
			});
		}
	}
}


