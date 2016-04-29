using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using Plugin.Calendars.Abstractions;
using System.Threading.Tasks;
using Capp2.Helpers;
using Acr.UserDialogs;
using System.Globalization;

namespace Capp2
{
	public class DatePage:ContentPage
	{
		bool yescall = true;
		public DatePicker datePicker{ get; set;}
		TimePicker timePicker{get;set;}
		bool AutoCall;
		string Name;

		public DatePage (string whichCapp, ContactData personCalled, bool autocall)
		{
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
			datePicker.DateSelected += (dateSender, de) => {
				if(!yescall && timePicker.IsEnabled == false){
					SetAppointmentHandler (whichCapp, personCalled);
				}
				else if(yescall){
					timePicker.IsEnabled = true;
					timePicker.Unfocused += (sender,e) =>
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

		}
		string LabelForPartOfCall(string whichcapp, ContactData personCalled){
			switch (whichcapp) {
			case Values.APPOINTED:
				return "When did we book " + personCalled.Name + "?";
				break;
			case Values.PRESENTED:
				return "When did we present " + personCalled.Name + " ?";
				break;
			case Values.PURCHASED:
				return "When did " + personCalled.Name + " signup/purchase?";
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
							await CalendarService.ReschedAppointment(personCalled.NextMeetingID, datePicker.Date.AddHours(timePicker.Time.Hours));

                            App.Database.UpdateItem(personCalled);

                            await this.Navigation.PopModalAsync();
                            break;
                        case Values.APPOINTED:
							personCalled.Appointed = datePicker.Date.AddHours(timePicker.Time.Hours).AddMinutes(timePicker.Time.Minutes);

                            personCalled.NextMeetingID = await CalendarService.CreateAppointment(personCalled.NextMeetingID, personCalled.Name, Values.APPOINTMENTDESCRIPTIONBOM, datePicker.Date.AddHours(timePicker.Time.Hours));
                            Debug.WriteLine("[DatePage - Appointed] NextMeetingID: " + personCalled.NextMeetingID);

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

                            App.Database.UpdateItem(personCalled);
							await DisplayAlert ("Presented", "Noted in virtual CapSheet!", "Great!");

                            //Navigation.PushModalAsync (new FollowUp(personCalled));//for follow ups
                            await this.Navigation.PopModalAsync();  //test
                            break;
                        case Values.PURCHASED:
                            //add date to contact's "purchased" property
                            personCalled.Purchased = datePicker.Date;
                            App.Database.UpdateItem(personCalled);
							await DisplayAlert ("Purchased", "Signup recorded in virtual CapSheet!", "Great!");
                            await this.Navigation.PopModalAsync();
                            break;
                    }
                }
                else {//if no call
                    if (AutoCall)
                    {
                        MessagingCenter.Send(this, Values.DONEWITHNOCALL);
                    }
                    else {
                        personCalled.NextCall = datePicker.Date;
                        App.Database.UpdateItem(personCalled);
						NavigationHelper.ClearModals(this);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in setting date: {0}", e.Message);
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

	public class FollowUp:ContentPage{
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
				await CalendarService.ReschedAppointment (personCalled.NextMeetingID, personCalled.Name, Values.FOLLOWUP, datePicker2.Date.AddHours (Values._5PMBOM));

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
	}
}

