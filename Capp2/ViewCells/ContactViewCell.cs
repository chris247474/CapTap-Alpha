using System;
using Xamarin.Forms;
using System.Diagnostics;
using XLabs.Forms.Controls;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;
using FFImageLoading.Forms;
using FFImageLoading.Work;
using FFImageLoading.Transformations;

namespace Capp2
{
	public class ContactViewCell:NativeCell
	{
		public ContactData personCalled{ set; get;}
		TapGestureRecognizer tapGestureRecognizer;
		Label nameLabel;//, firstnameLabel, lastnameLabel;
		Label playlistLabel, initials;
		Image phone;
		CheckBox checkbox;
		CircleImage circleImage;
		//CachedImage circleImage;
		string playlist;
		RelativeLayout layout = new RelativeLayout();

		//commenting in this class causes significant performance degradation in debug mode
		public ContactViewCell (CAPP page):base()
		{
			this.Height = 70;
			this.playlist = page.playlist;

			nameLabel = new Label{
				FontSize = Values.NAMEFONTSIZE,//Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				HorizontalOptions = LayoutOptions.Start,
				HorizontalTextAlignment = TextAlignment.Start,
				FontFamily = Font.Default.FontFamily,
			};
			nameLabel.SetBinding(Label.TextProperty, "Name");

			circleImage = new CircleImage{
				HorizontalOptions = LayoutOptions.Start,
				Aspect = Aspect.AspectFit,
				BackgroundColor = Color.Transparent,
			};
			circleImage.SetBinding (CircleImage.SourceProperty, "PicStringBase64");

			playlistLabel = new Label{
				FontSize = Values.NAMELISTFONTSIZE,//Device.GetNamedSize (NamedSize.Small, typeof(Label)), 
				TextColor = Color.Green,
				VerticalOptions = LayoutOptions.Start,
				HorizontalTextAlignment = TextAlignment.Start,
				LineBreakMode = LineBreakMode.WordWrap,
				FontFamily = Font.Default.FontFamily,
			};
			playlistLabel.SetBinding(Label.TextProperty, "Playlist");//"Playlist" links directly to the ContactData.Name property

			checkbox = new CheckBox{ 
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
			checkbox.MinimumWidthRequest = checkbox.Height;
            checkbox.IsEnabled = true;
            checkbox.IsVisible = true;
            
			checkbox.SetBinding (CheckBox.CheckedProperty, "IsSelected");
			checkbox.CheckedChanged += (sender, e) => {
				personCalled = (sender as CheckBox).Parent.Parent/*.Parent*/.BindingContext as ContactData;
				Debug.WriteLine (personCalled.Name+"' selected value is "+personCalled.IsSelected.ToString ());
				Debug.WriteLine("checkbox value is {0}", checkbox.Checked);
				App.Database.UpdateItem(personCalled);

				var person = (from x in (App.Database.GetItems (Values.ALLPLAYLISTPARAM).Where(x => x.Name == personCalled.Name && x.Playlist == personCalled.Playlist))
					select x);
				Debug.WriteLine (person.ElementAtOrDefault (0).Name+"' selected value is "+person.ElementAtOrDefault (0).IsSelected.ToString ()); 
			};

			phone = new Image {
				Aspect = Aspect.AspectFit,
				Source = FileImageSource.FromFile ("Phone"),
				HorizontalOptions = LayoutOptions.End,
				//HeightRequest = nameLabel.FontSize *1.5,
				//WidthRequest = nameLabel.FontSize *1.5,
			};

			tapGestureRecognizer = new TapGestureRecognizer ();
			tapGestureRecognizer.Tapped += async (s, e) => {
				UIAnimationHelper.ZoomUnZoomElement(phone);
				personCalled = (s as Image).Parent./*Parent.*/Parent.BindingContext as ContactData;
				CallHelper.call(personCalled, false);
			};
			phone.GestureRecognizers.Add (tapGestureRecognizer);
			tapGestureRecognizer.NumberOfTapsRequired = 1;

			var nextAction = new MenuItem { Text = "Resched", IsDestructive = true };
			nextAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			nextAction.Clicked += (sender, e) => {
				var mi = ((MenuItem)sender);
				personCalled = (ContactData)mi.BindingContext;

                if (!personCalled.Appointed.ToString().Contains("1/1/0001"))
                {
                    page.Navigation.PushModalAsync(new DatePage(Values.NEXT, personCalled, false));//pass Contact.ID of listview row selected via contextmenu event listener
                }
                else {
                    Debug.WriteLine("Set an appointment for {0} first", personCalled.FirstName);
                    UserDialogs.Instance.WarnToast("Pls book "+ personCalled.FirstName + " at a BOM before we can move on to the follow up!", null, 2000);
                }
            };

			var appointedAction = new MenuItem { Text = "Appointed" };
			appointedAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			appointedAction.Clicked += (sender, e) => {
				var mi = ((MenuItem)sender);
				personCalled = (ContactData)mi.BindingContext;
				page.Navigation.PushModalAsync(new DatePage(Values.APPOINTED, personCalled, false));//pass Contact.ID of listview row selected via contextmenu event listener
			};

			var presentedAction = new MenuItem { Text = "Presented" };
			presentedAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			presentedAction.Clicked += (sender, e) => {
				var mi = ((MenuItem)sender);
				personCalled = (ContactData)mi.BindingContext;
				page.Navigation.PushModalAsync(new DatePage(Values.PRESENTED, personCalled, false));
			};

			var purchasedAction = new MenuItem { Text = "Purchased" }; 
			purchasedAction.SetBinding (MenuItem.CommandParameterProperty, new Binding ("."));
			purchasedAction.Clicked += (sender, e) => {
				var mi = ((MenuItem)sender);
				personCalled = (ContactData)mi.BindingContext;
				page.Navigation.PushModalAsync(new DatePage(Values.PURCHASED, personCalled, false));
			};

			var sched = new MenuItem { Text = "Sched" };
			sched.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
			sched.Clicked += async (sender, e) =>
			{
				var mi = ((MenuItem)sender);
				personCalled = (ContactData)mi.BindingContext;
				var result = (await UserDialogs.Instance.ActionSheetAsync(null, null, null,
				                                                          new string[] {Values.APPOINTEDCAPITALIZED,
					                                                                          Values.PRESENTEDCAPITALIZED, 
					                                                                          Values.PURCHASEDCAPITALIZED}));
				await page.Navigation.PushModalAsync(new DatePage(result.ToLower(), personCalled, false));
			};

			View = CreateLayout(page.playlist);//createLayoutView(page.playlist);

			SubscribeToEditingListeners ();

			// add context actions to the cell
			ContextActions.Add(nextAction);
			ContextActions.Add(sched);
			//ContextActions.Add (appointedAction);
			//ContextActions.Add (presentedAction);
			//ContextActions.Add (purchasedAction);
		}
		protected override void OnBindingContextChanged ()
		{
			base.OnBindingContextChanged ();
			//Debug.WriteLine ("OnBindingContextChanged");

			var item = BindingContext as ContactData;
			if (item != null) {
				nameLabel.Text = item.Name;
				playlistLabel.Text = item.Playlist;
				circleImage.Source = item.PicStringBase64;
				initials.Text = item.Initials;
			}
		}
		void SwitchToEditingOrNotEditingMode(){
			Debug.WriteLine ("In SwitchToEditOrNotEditingMode()");
			if (App.IsEditing) {
				Debug.WriteLine ("Editing in CustomViewCell - OnBindingContextChanged");
				layout.Children/*.RemoveAt (layout.Children.Count - 1);*/.Remove (phone);
				AddCheckboxToLayout ();
			} else {
				Debug.WriteLine ("Not editing in CustomViewCell - OnBindingContextChanged");
				//layout.Children.RemoveAt (layout.Children.Count - 1);
				layout.Children.Remove (checkbox);
				AddPhoneToLayout ();
			}
		}
		public RelativeLayout CreateLayout(string playlist){
			layout.HorizontalOptions = LayoutOptions.FillAndExpand;

			createLayoutView (playlist);

			initials = UIBuilder.CreateInitialsLabel (/*this.RenderHeight * 0.45*/Values.INITIALSFONTSIZE, "Initials");
			UIBuilder.AddInitialsToContactListItem (layout, initials, 0.039, circleImage, 0.45);

			return layout;
		}
		public View createView (string playlist)
		{
			if (App.IsEditing) {
				if (string.Equals (playlist, Values.ALLPLAYLISTPARAM)) {
					return UIBuilder.AddElementToObjectDependingOniOSAndAndroidListViewShortNameBinding (
						new StackLayout{
							Orientation = StackOrientation.Horizontal,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							Children = {
								circleImage,
								new StackLayout {
									Orientation = StackOrientation.Vertical,
									Children = { nameLabel, playlistLabel }
								}
							}
						},
						new StackLayout{
							HorizontalOptions = LayoutOptions.End,
							VerticalOptions = LayoutOptions.Center,
							Children = {checkbox}
						}
					);
				} else {
					return UIBuilder.AddElementToObjectDependingOniOSAndAndroidListViewShortNameBinding (
						new StackLayout{
							Orientation = StackOrientation.Horizontal,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							Children = {
								circleImage,
								new StackLayout{
									//VerticalOptions = LayoutOptions.Center,
									Children = {
										nameLabel
									}
								}
							}
						},
						new StackLayout{
							HorizontalOptions = LayoutOptions.End,
							VerticalOptions = LayoutOptions.Center,
							Children = {checkbox}
						}
					);
				}
			} else {
				if (string.Equals (playlist, Values.ALLPLAYLISTPARAM)) {
					return UIBuilder.AddElementToObjectDependingOniOSAndAndroidListViewShortNameBinding (
						new StackLayout{
							Orientation = StackOrientation.Horizontal,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							Children = {
								circleImage,
								new StackLayout {
									Orientation = StackOrientation.Vertical,
									Children = { nameLabel, playlistLabel }
								}
							}
						},
						new StackLayout{
							HorizontalOptions = LayoutOptions.End,
							VerticalOptions = LayoutOptions.Center,
							Children = {phone}
						}
					);
				} else {
					return UIBuilder.AddElementToObjectDependingOniOSAndAndroidListViewShortNameBinding( 
						new StackLayout{
							Orientation = StackOrientation.Horizontal,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							Children = {
								circleImage,
								new StackLayout{
									//VerticalOptions = LayoutOptions.Center,
									Children = {
										nameLabel
									}
								}
							}
						},
						new StackLayout{
							HorizontalOptions = LayoutOptions.End,
							VerticalOptions = LayoutOptions.Center,
							Children = {phone}
						}
					);
				}
			}
		}
		RelativeLayout CreateEditingLayout(){
			//Debug.WriteLine ("In CreateEditingLayout");
			layout.HorizontalOptions = LayoutOptions.FillAndExpand;
			layout.Padding = new Thickness (15, 5, 5, 15);

			layout.Children.Add(
				circleImage,
				Constraint.RelativeToParent(parent => parent.Width*0.039),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent => parent.Width*0.15)),
				Constraint.RelativeToParent((parent => parent.Height))
			); 

			layout.Children.Add(nameLabel,
				Constraint.RelativeToParent((parent => parent.X + circleImage.Width*1.5)),
				Constraint.RelativeToParent((parent => parent.Height*0.18))
			);
			layout.Children.Add(playlistLabel,
				Constraint.RelativeToParent((parent => parent.X + circleImage.Width*1.5)),
				Constraint.RelativeToParent((parent => parent.Height*0.22+nameLabel.Height)),
			                    Constraint.RelativeToParent((parent => parent.Width*0.8))
			);

			AddCheckboxToLayout ();

			return layout;
		}
		void AddCheckboxToLayout(){
			layout.Children.Add(checkbox,
				Constraint.RelativeToParent((parent => parent.Width*0.85)),
				Constraint.RelativeToParent((parent => parent.Height*0.3))
			);
		}
		void AddPhoneToLayout(){
			layout.Children.Add(phone,
				Constraint.RelativeToParent((parent => parent.Width*0.85)),
				Constraint.RelativeToParent((parent => parent.Height*0.3))
			);
		}
		RelativeLayout CreateNotEditingLayout(){
			//Debug.WriteLine ("In CreateNotEditingLayout");
			layout.HorizontalOptions = LayoutOptions.FillAndExpand;
			layout.Padding = new Thickness (15, 5, 5, 15);

			layout.Children.Add(
				circleImage,
				Constraint.RelativeToParent(parent => parent.Width*0.039),
				Constraint.Constant(0),
				Constraint.RelativeToParent((parent => parent.Width*0.15)),
				Constraint.RelativeToParent((parent => parent.Height))
			); 

			layout.Children.Add(nameLabel,
				Constraint.RelativeToParent((parent => parent.X + circleImage.Width*1.5)),
				Constraint.RelativeToParent((parent => parent.Height*0.18))
			);
			layout.Children.Add(playlistLabel,
				Constraint.RelativeToParent((parent => parent.X + circleImage.Width*1.5)),
				Constraint.RelativeToParent((parent => parent.Height*0.22+nameLabel.Height)),
								Constraint.RelativeToParent((parent => parent.Width * 0.5))
			);

			AddPhoneToLayout ();

			return layout;
		}
		public View createLayoutView (string playlist)
		{
			//Debug.WriteLine ("In createLayoutView");

			if (App.IsEditing) {
				return CreateEditingLayout ();
			} else {
				return CreateNotEditingLayout ();
			}
		}

		void SubscribeToEditingListeners(){
			MessagingCenter.Subscribe<CAPP> (this, Values.ISEDITING, (args) => { 
				Debug.WriteLine ("ContactViewCell - ISEDITING MESSAGE RECEIVED");
				SwitchToEditingOrNotEditingMode ();
			});
			MessagingCenter.Subscribe<CAPP> (this, Values.DONEEDITING, (args) => { 
				Debug.WriteLine ("ContactViewCell - DONEEDITING MESSAGE RECEIVED");
				SwitchToEditingOrNotEditingMode ();
			});
		}
	}
}

