using System;
using Xamarin.Forms;
using System.Diagnostics;
using XLabs.Forms.Controls;
using System.Collections.Generic;
using System.Linq;
using Acr.UserDialogs;

namespace Capp2
{
	public class ContactViewCell:NativeCell
	{
		public ContactData personCalled{ set; get;}
		TapGestureRecognizer tapGestureRecognizer;
		Label nameLabel;
		Label playlistLabel;
		Image phone;
		CheckBox checkbox;
		CircleImage circleImage;

		public ContactViewCell (CAPP page)
		{
			this.Height = 56;
			this.Height *= 1.3;
			nameLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				VerticalOptions = LayoutOptions.StartAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
			};
			nameLabel.SetBinding(Label.TextProperty, "Name");//"Name" links directly to the ContactData.Name property

			circleImage = new CircleImage{
				HorizontalOptions = LayoutOptions.Fill,
				Aspect = Aspect.AspectFit,
			};
			circleImage.SetBinding (CircleImage.SourceProperty, "PicStringBase64");

			playlistLabel = new Label{
				FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)), 
				TextColor = Color.Green,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Start,
			};
			playlistLabel.SetBinding(Label.TextProperty, "Playlist");//"Playlist" links directly to the ContactData.Name property

			checkbox = new CheckBox{ 
				HorizontalOptions = LayoutOptions.Center
			};
            checkbox.IsEnabled = true;
            checkbox.IsVisible = true;
            
			checkbox.SetBinding (CheckBox.CheckedProperty, "IsSelected");
			checkbox.CheckedChanged += (sender, e) => {
				personCalled = (sender as CheckBox).Parent.Parent.Parent.BindingContext as ContactData;
				Debug.WriteLine (personCalled.Name+"' selected value is "+personCalled.IsSelected.ToString ());
				App.Database.UpdateItem(personCalled);

				var person = (from x in (App.Database.GetItems (Values.ALLPLAYLISTPARAM).Where(x => x.Name == personCalled.Name))
					select x);
				Debug.WriteLine (person.ElementAtOrDefault (0).Name+"' selected value is "+person.ElementAtOrDefault (0).IsSelected.ToString ()); 
			};

			phone = new Image {
				Aspect = Aspect.AspectFit,
				Source = FileImageSource.FromFile ("Phone"),
				HorizontalOptions = LayoutOptions.End,
				HeightRequest = nameLabel.FontSize *1.5,
				WidthRequest = nameLabel.FontSize *1.5,
			};

			tapGestureRecognizer = new TapGestureRecognizer ();
			tapGestureRecognizer.Tapped += async (s, e) => {
				UIAnimationHelper.ZoomUnZoomElement(phone);
				personCalled = (s as Image).Parent.Parent.Parent.BindingContext as ContactData;
				CallHelper.call(personCalled, false);
			};
			phone.GestureRecognizers.Add (tapGestureRecognizer);
			tapGestureRecognizer.NumberOfTapsRequired = 1;

			var nextAction = new MenuItem { Text = "Next Meeting", IsDestructive = true };
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

			View = createView (page.playlist);

			// add context actions to the cell
			ContextActions.Add(nextAction);
			ContextActions.Add (appointedAction);
			ContextActions.Add (presentedAction);
			ContextActions.Add (purchasedAction);
		}

		public View createView (string playlist)
		{
			if (App.IsEditing) {
				if (string.Equals (playlist, Values.ALLPLAYLISTPARAM)) {
					return UIBuilder.AddElementToObjectDependingOniOSAndAndroidListViewShortNameBinding (
						new StackLayout{
							Orientation = StackOrientation.Horizontal,
							HorizontalOptions = LayoutOptions.StartAndExpand,
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
							HorizontalOptions = LayoutOptions.StartAndExpand,
							Children = {
								circleImage, 
								new StackLayout{
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
							HorizontalOptions = LayoutOptions.StartAndExpand,
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
							HorizontalOptions = LayoutOptions.StartAndExpand,
							Children = {
								circleImage, 
								new StackLayout{
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

	}
}

