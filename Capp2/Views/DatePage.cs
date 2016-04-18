using System;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using Plugin.Calendars.Abstractions;
using System.Threading.Tasks;
using Capp2.Helpers;

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

		}
		public async void SetAppointmentHandler(string whichCapp, ContactData personCalled){
            try {
                if (yescall)
                {
                    //text appointed person

                    switch (whichCapp)
                    {
                        case Values.NEXT:
                            Debug.WriteLine("ABOUT TO RESCHED: NEXTMEETINGID " + personCalled.NextMeetingID);
                            await CalendarService.ReschedAppointment(personCalled.NextMeetingID, datePicker.Date.AddHours(timePicker.Time.Hours));

                            App.Database.UpdateItem(personCalled);
                            await this.Navigation.PopModalAsync();
                            break;
                        case Values.APPOINTED:
                            personCalled.Appointed = datePicker.Date;

                            //always first time to give nextmeeting real values
                            personCalled.NextMeetingID = await CalendarService.CreateAppointment(personCalled.NextMeetingID, personCalled.Name, Values.APPOINTMENTDESCRIPTIONBOM, datePicker.Date.AddHours(timePicker.Time.Hours));
                            Debug.WriteLine("[DatePage - Appointed] NextMeetingID: " + personCalled.NextMeetingID);

                            App.Database.UpdateItem(personCalled);
                            //Navigation.PopModalAsync ();

                            await Navigation.PushModalAsync(new TextTemplatePage(personCalled, AutoCall));//or dependency call OS specific default messaging app

                            break;
                        case Values.PRESENTED:
                            personCalled.Presented = datePicker.Date;

                            App.Database.UpdateItem(personCalled);

                            //Navigation.PushModalAsync (new FollowUp(personCalled));//for follow ups
                            await this.Navigation.PopModalAsync();  //test
                            break;
                        case Values.PURCHASED:
                            //add date to contact's "purchased" property
                            personCalled.Purchased = datePicker.Date;
                            App.Database.UpdateItem(personCalled);
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
                        Navigation.PopModalAsync();
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

