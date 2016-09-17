using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using Plugin.Calendars.Abstractions;
using System.Threading.Tasks;
using Capp2.Helpers;
using Acr.UserDialogs;
using System.Globalization;
using FAB.Forms;

namespace Capp2
{
	public class DatePage:ContentPage
	{
		bool yescall = true;
		public DatePicker datePicker{ get; set;}
		TimePicker timePicker{get;set;}
		bool AutoCall;
		string Name;
		SettingsViewModel settings = new SettingsViewModel ();
		FloatingActionButton fab;
		double UnfocusedPosition = 0;
		StackLayout stack;

		public DatePage (string whichCapp, ContactData personCalled, bool autocall)
		{
			var content = (this.Content as RelativeLayout);
			BackgroundColor = Color.White;
			AutoCall = autocall;
			this.SetBinding (ContentPage.TitleProperty, "Name");
			Name = personCalled.Name;

			Label lbl = new Label{
				Text = LabelForPartOfCall(whichCapp, personCalled),
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

			datePicker.Unfocused += async (dateSender, de) => {
				if(!yescall && timePicker.IsEnabled == false){
					App.Database.MarkForTodaysCalls(personCalled, false, datePicker.Date);
					//App.CapPage.ContactsVM.MarkForTodaysCalls(personCalled, false, datePicker.Date);
					App.CapPage.refresh();
					SetAppointmentHandler(whichCapp, personCalled);
				}
				else if(yescall){
					timePicker.IsEnabled = true;
					timePicker.Unfocused += async (sender,e) =>
					{
						SetAppointmentHandler (whichCapp, personCalled);
					};
				}
			};

			var cancelButton = new Button { 
				Text = personCalled.Name+" hasn't said yes yet" 
			};
			cancelButton.Clicked += (sender, e) => {
				UIAnimationHelper.ShrinkUnshrinkElement(lbl);
				UIAnimationHelper.ZoomUnZoomElement(datePicker);
				UIAnimationHelper.ShrinkUnshrinkElement(timePicker);

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
			if (!string.Equals(whichCapp, Values.APPOINTED)) {
				cancelButton.IsVisible = false;
				cancelButton.IsEnabled = false;
			}

			stack = new StackLayout {
				VerticalOptions = LayoutOptions.Center,
				Padding = new Thickness(20),
				Children = {
					lbl, new StackLayout{
						VerticalOptions = LayoutOptions.Center,
						Children = { datePicker, timePicker,}
					}, cancelButton,
					UIBuilder.CreateEmptyStackSpace(),
				}
			};

			fab = UIBuilder.CreateFAB ("", FabSize.Normal, Color.FromHex (Values.RED), 
				Color.FromHex (Values.GOOGLEBLUE));

			Content = UIBuilder.AddFABToViewWrapRelativeLayout(stack, fab, "phone_end", new Command(() => {
				NavigationHelper.ClearModals(this);
				if(App.CapPage.AutoCalling){
					App.CapPage.SetupNotAutoCalling();
				}

				MessagingCenter.Send("", Values.READYFOREXTRATIPS);
				/*if (App.InTutorialMode// && TutorialHelper.ReadyForExtraTips
				    && TutorialHelper.HowToAddContactsDone)
				{
					Debug.WriteLine("finishing tutorial mode. about to show extra tips");
					MessagingCenter.Send("", Values.READYFOREXTRATIPS);
				}*/
			}));

			content = this.Content as RelativeLayout;
			datePicker.Focused += (object sender, FocusEventArgs e) => {
				Debug.WriteLine("fab at {0}, {1}", fab.X, fab.Y);
			};

			UnfocusedPosition = fab.Y;
		}
		string LabelForPartOfCall(string whichcapp, ContactData personCalled){
			switch (whichcapp) {
			case Values.APPOINTED:
				return "When did we book " + personCalled.Name + "?";
				break;
			case Values.PRESENTED:
				return "When did we present to " + personCalled.Name + "?";
				break;
			case Values.PURCHASED:
					return "When did we close the deal with " + personCalled.Name + "?";// + " signup/purchase?";
				break;
			case Values.NEXT:
				return "When are following up " + personCalled.Name + "?";
				break;
			}
			return string.Empty;
		}
		public async void SetAppointmentHandler(string whichCapp, ContactData personCalled){
            try {
                if (yescall)
                {
                    switch (whichCapp)
                    {
                        case Values.NEXT:
                            Debug.WriteLine("ABOUT TO RESCHED: NEXTMEETINGID " + personCalled.NextMeetingID);
							await App.CalendarHelper.ReschedAppointment(personCalled.NextMeetingID, datePicker.Date.AddHours(timePicker.Time.Hours));

                            App.Database.UpdateItem(personCalled);

                            await this.Navigation.PopModalAsync();
							
                            break;
                        case Values.APPOINTED:
							personCalled.Appointed = datePicker.Date.AddHours(timePicker.Time.Hours).AddMinutes(timePicker.Time.Minutes);
							
							ResolveCAPP(personCalled, Values.APPOINTED);

							personCalled.NextMeetingID = await App.CalendarHelper.CreateReschedAppointment(
								personCalled,
								Values.APPOINTMENTDESCRIPTIONBOM, 
								datePicker.Date.AddHours(timePicker.Time.Hours));
                            Debug.WriteLine("[DatePage - Appointed] NextMeetingID: " + personCalled.NextMeetingID);

							App.Database.MarkForTodaysCalls (personCalled, true, DateTime.MaxValue);
							//App.CapPage.ContactsVM.MarkForTodaysCalls(personCalled, true, DateTime.MaxValue);
							App.CapPage.refresh();	

                            App.Database.UpdateItem(personCalled);

							if(Device.OS == TargetPlatform.Android) {
								await Navigation.PushModalAsync(new TextTemplatePage(personCalled, AutoCall));
							}
							else if(Device.OS == TargetPlatform.iOS){
								await TextTemplateHelper.BookProspectOrMarkForCallBackDate(personCalled, AutoCall);
							}

                            break;
                        case Values.PRESENTED:
                            personCalled.Presented = datePicker.Date;

							ResolveCAPP(personCalled, Values.PRESENTED);

                            App.Database.UpdateItem(personCalled);
							await DisplayAlert ("Presented", "Noted!", "Great!");

                            await this.Navigation.PopModalAsync();  //test
                            break;
                        case Values.PURCHASED:
                            //add date to contact's "purchased" property
                            personCalled.Purchased = datePicker.Date;
							ResolveCAPP(personCalled, Values.PURCHASED);
                            App.Database.UpdateItem(personCalled);
							await DisplayAlert ("Purchased", "Closed deal recorded!", "Great!");
                            await this.Navigation.PopModalAsync();
                            break;
                    }
                }
                else {//if no call
					Debug.WriteLine("recieved No Call. checking for autocall status to send DONEWITHNOCALL message");
                    if (AutoCall)
                    {
                        MessagingCenter.Send(this, Values.DONEWITHNOCALL);
                    }
                    else {
						NavigationHelper.ClearModals(this);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in setting date: {0}", e.Message);
            }

		}

		//assign placeholder values to any blank but necessary CAPP functions to maintain CapStats page graphs accuracy
		public void ResolveCAPP(ContactData personCalled, string cappstrings){
			switch (cappstrings) {
			case Values.APPOINTED:
				if (personCalled.Called == DateTime.MinValue) {
					personCalled.Called = DateTime.Now;
				}
				break;
			case Values.PRESENTED:
				if(personCalled.Called == DateTime.MinValue){
					personCalled.Called = DateTime.Now;
				}
				if(personCalled.Appointed == DateTime.MinValue){
					personCalled.Appointed = DateTime.Now;
				}
				break;
			case Values.PURCHASED:
				if(personCalled.Called == DateTime.MinValue){
					personCalled.Called = DateTime.Now;
				}
				if(personCalled.Appointed == DateTime.MinValue){
					personCalled.Appointed = DateTime.Now;
				}
				if(personCalled.Presented == DateTime.MinValue){
					personCalled.Presented = DateTime.Now;
				}
				break;
			}
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

	/*public class FollowUp:ContentPage{
		 /// <summary>
		/// Date Picker page for follow ups/next meetings after being presented or after signing up, only accessible within the DatePage class
		/// </summary>
		/// <param name="personCalled">ContactData personCalled.</param>
		public FollowUp(ContactData personCalled){
			Debug.WriteLine ("FOLLOWUP()");
			Label lbl2 = new Label{ Text = "When do we follow up "+personCalled.Name+" ?" };

			var cancelButton2 = new Button { Text = "No next meeting" };
			cancelButton2.Clicked += (sender, e) => {
				Navigation.PopModalAsync ();
			};

			var datePicker2 = new DatePicker
			{
				Format = "D",
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			datePicker2.DateSelected += async (dateSender, de) => {
				await App.CalendarHelper.ReschedAppointment (personCalled.NextMeetingID, personCalled.Name, Values.FOLLOWUP, datePicker2.Date.AddHours (Values._5PMBOM));

				App.Database.UpdateItem (personCalled);

				await Navigation.PopModalAsync ();
			};
			Content = new StackLayout {
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Padding = new Thickness(20),
				Children = {
					lbl2, datePicker2, cancelButton2
				}
			};
		}
	}*/
}

