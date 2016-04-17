using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.Contacts;
using Plugin.Contacts.Abstractions;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using System.IO;
using Acr.UserDialogs;

namespace Capp2
{
	public class Util
	{
		public static bool contactPermission{ get; set;}
		public static IQueryable<Contact> deviceContacts{ get; set;}
		List<ContactData> contactErrors;
		List<ContactData> contactDuplicates;

		public Util ()
		{
		}
		/*public async Task<bool> CreateDefaultPlaylistsIfNoneExists(){
			Debug.WriteLine ("Checking playlists");
			PlaylistItemAzure[] playlists = (await App.AzurePlaylistDB.GetPlaylistItems ()).ToArray ();
			for(int c = 0;c < playlists.Length;c++){
				Debug.WriteLine ("Entered loop");
				if (string.Equals (playlists [c].PlaylistName, Values.ALLPLAYLISTPARAM) || string.Equals (playlists [c].PlaylistName, Values.TODAYSCALLS)) {
					Debug.WriteLine ("default playlists exist already");
				} else {
					Debug.WriteLine ("First run, default playlists dont exist yet");
					await App.AzurePlaylistDB.SaveItem (new PlaylistItemAzure{PlaylistName = Values.ALLPLAYLISTPARAM }, false);
					await App.AzurePlaylistDB.SaveItem (new PlaylistItemAzure{PlaylistName = Values.TODAYSCALLS}, false);
					await App.AzurePlaylistDB.SyncAsync ();
				}
			}

			return true;
		}*/
		public string ConnectStrings(string[] s){
			string result = "";
			if (s != null && s.Length > 0) {
				for(int c = 0;c < s.Length;c++){
					result += s [c];
				}
			}
			return result;
		}
		public async Task<bool> DeviceCalendarExistsAndInit(){
			//check if calendar app has at least one calendar account
			try{
				await DependencyService.Get<ICalendar> ().InitCalendar ();
				Debug.WriteLine ("CALENDAR ACCOUNT: "+ DependencyService.Get<ICalendar> ().PrimaryCalendar.Name);
				return true;//continue with program
			}catch(Exception){
				UserDialogs.Instance.ErrorToast ("Calendar error", "Please make sure your Calendar app has at least one account in it", 4000);
			}

			return false;
		}

		public string RemoveCountryCodeAndSpecialChar(string input, Regex countryCode, Regex special){
			return countryCode.Replace (special.Replace(input.Replace(Values.WEIRDCHARFL, "").Replace(Values.WEIRDQUOTE, "").Replace(Values.LONGDASH, "").Replace(Values.UNDERLINE, "").Replace(Values.WEIRDCHARFI, "").Replace(Values.OTHERWEIRDQUOTE, "").Trim(), ""), "0");
		}

		public string RemoveMatchingString(string input, Regex special){
			return special.Replace (input, "");
		}

		public string MakeDBContactCallable(string number, bool debug){
			if(debug) Debug.WriteLine ("DB NUMBER "+number);

			var completeSpecialReg = new Regex (Values.COMPLETESINGLESPECIALCHARREGEX);
			var strictNumReg = new Regex (Values.STRICTNUMREGEX);

			string callableNumber = RemoveMatchingString (number, completeSpecialReg);
			if(debug) Debug.WriteLine ("DB FILTERED NUMBER "+callableNumber);

			Match callableMatch = strictNumReg.Match(callableNumber);

			if (callableMatch.Success) {
				if(debug) Debug.WriteLine ("Made number callable - 0" + callableMatch.Groups [0].Value);
				return "0" + callableMatch.Groups [0].Value;
			} else {
				//add anyway
				return callableNumber;
			}
		}

		public List<ContactData> MakeDBContactListCallable(List<ContactData> list, bool debug){
			if (list == null)
				return null;

			var arr = list.ToArray ();
			for (int c = 0; c < arr.Length; c++) {
				arr [c].Number = MakeDBContactCallable (arr [c].Number, debug);
			}

			return arr.ToList ();
		}

		public async void loadContactsFromPic(string playlist, Stream s, CAPP page, bool saveProcessedImage){
			UserDialogs.Instance.ShowLoading ("Loading Contacts...", new MaskType?(MaskType.Clear));

			contactErrors = new List<ContactData> ();
			contactDuplicates = new List<ContactData> ();
			string error = null;
			string tempError = null;
			string firstname,lastname,number,aff;
			int errorStatus;
			List<ContactData> saveList = new List<ContactData> ();

			//user manual crop
			System.IO.Stream bwSharpenedStream = DependencyService.Get<IPics>().PreprocessImage(s, Values.GAUSSIANSIZEX, Values.GAUSSIANSIZEY);
			if(saveProcessedImage) DependencyService.Get<IPics>().SaveImageToDisk(bwSharpenedStream, System.DateTime.Now.Second+"bwsharp.png");
			await DependencyService.Get<IPics> ().loadFromPicDivideBy (Tesseract.PageIteratorLevel.Textline, bwSharpenedStream);
			IEnumerable<Tesseract.Result> imageResult = DependencyService.Get<IPics> ().imageResult;

			if (imageResult == null) {
				await page.DisplayAlert ("Unable to load contacts", "CapTap could not read the image", "OK");
			} else {

				foreach (Tesseract.Result result in imageResult) {
					errorStatus = 0;
					firstname = null;
					lastname = null;
					number = null;
					aff = null;

					try{

						var nameNumOrgNotesRegex = new Regex (Values.NAMENUMORGNOTESREGEX);
						var nameNumRegex = new Regex(Values.NAMENUMREGEX);
						var wordReg = new Regex(Values.WORDREGEX);
						var numReg = new Regex (Values.NUMREGEX);
						var strictNumReg = new Regex (Values.STRICTNUMREGEX);
						var anyReg = new Regex (Values.ANYSTRINGREGEX);
						var specialReg = new Regex(Values.SINGLESPECIALCHARREGEX);
						var inBetweenNumRegex = new Regex(Values.INVALIDINBETWEENNUMREGEX);

						Debug.WriteLine ("NEW LINE IN IMAGERESULT: "+result.Text);
						Debug.WriteLine ("MINUS SPECIAL CHARS: "+RemoveCountryCodeAndSpecialChar(result.Text, new Regex(Values.COUNTRYCODE), specialReg));

						Match nameNumOrgNotesMatch = nameNumOrgNotesRegex.Match (RemoveCountryCodeAndSpecialChar(result.Text, new Regex(Values.COUNTRYCODE), specialReg));
						Match nameNumMatch = nameNumRegex.Match (RemoveCountryCodeAndSpecialChar(result.Text, new Regex(Values.COUNTRYCODE), specialReg));
						Match wordMatch = wordReg.Match (RemoveCountryCodeAndSpecialChar(result.Text, new Regex(Values.COUNTRYCODE), specialReg));

						if (nameNumOrgNotesMatch.Success /*&& nameNumMatch.Success && wordMatch.Success*/) {//check for different types of name and number combinations?
							Debug.WriteLine ("OVERALL COMBINED MATCH: " + nameNumMatch.Groups [0].Value);
							Debug.WriteLine ("OVERALL NAME MATCH: " + wordMatch.Groups [0].Value);
							tempError = wordMatch.Groups [0].Value+"\n";

							//processing name
							string[] words = wordMatch.Groups [0].Value.Split (new char[] { ' ', '\t' });
							Debug.WriteLine ("words split by spaces count: " + words.Length);
							if (words.Length - 1 > 1) {
								firstname = words [0];
								for (int i = 1; i < words.Length; i++) {
									lastname += words [i];
								}
							} else {
								//usually happens in OCR misreads. add to error list for user to manually correct
								lastname = words [0];
								firstname = " ";
								errorStatus = 1;
							}
							Debug.WriteLine ("firstname: "+firstname+"lastname: "+lastname+" Field missing:"+errorStatus);

							//processing number
							Match numMatch = numReg.Match (RemoveMatchingString(subtractFromString(wordMatch.Groups[0].Value, nameNumMatch.Groups[0].Value), inBetweenNumRegex));//remove contact name from the name and number portion = number portion

							Debug.WriteLine ("OVERALL NUMBER MATCH: " + numMatch.Groups [0].Value);

							//remove spaces
							string[] num = numMatch.Groups [0].Value.Split (new char[] { ' ', '\t' });
							Debug.WriteLine ("number split by spaces count: " + num.Length);
							if (num.Length > 1) {
								for (int i = 0; i < num.Length; i++) {
									number += num [i];
								}
							} else {
								number = numMatch.Groups [0].Value;
							}

							if(!isValidMobile(number)){
								errorStatus = 1;
							}

							//processing org and notes
							Match orgNotesMatch = anyReg.Match(subtractFromString(nameNumMatch.Groups[0].Value, nameNumOrgNotesMatch.Groups[0].Value));
							Debug.WriteLine ("OVERALL ORG AND NOTES MATCH: "+ orgNotesMatch.Groups[0].Value);

							string[] orgNotes = orgNotesMatch.Groups [0].Value.Split (new char[] { ' ', '\t' });
							Debug.WriteLine ("orgNotes split by spaces count: " + orgNotes.Length);
							if (orgNotes.Length > 1) {

								for (int i = 0; i < orgNotes.Length; i++) {
									aff += orgNotes [i];
								}
							} else {
								aff = orgNotesMatch.Groups [0].Value;
							}
							Debug.WriteLine ("PROCESSING aff --------------------------- aff so far is " + aff);
						} else {
							//single word and number - horizontally, vertically
							//Debug.WriteLine ("PROBLEM IN WORD AND NUMBER MATCHING AFTER READING TEXTLINE FROM IMAGE");
						}

						Debug.WriteLine ("Storing contact Name:"+firstname+" ,Last Name:"+lastname+", Number:"+number);
						var contact = new ContactData{ FirstName = firstname, LastName = lastname, Number = number, Aff = aff, Playlist = playlist };
						saveList.Add (contact);
						/*if(!saveContactToDB(false, contact, playlist)){
							contactDuplicates.Add (contact);
						}*/
						//contactDuplicates += duplicates;

						if(errorStatus == 1){
							contactErrors.Add(contact);
							Debug.WriteLine("[CAPP Util.loadContactsFromPic() ] "+contact.Name+" added to error list");
						}
					}catch(ArgumentException ae){
						Debug.WriteLine (ae.Message + " - duplicatesAllowed?");
					}catch(Exception){
						//skip
						Debug.WriteLine ("ERROR SAVING CONTACT - SKIPPING: "+error);
					}
				}
				saveMultipleContactToDB (false, saveList, playlist);
			}

			UserDialogs.Instance.HideLoading ();
			page.refresh ();

			/////////////////////////// IMAGE PRE PROCESSING TESTS ////////////////////////////////////
		//	Tests.AccuracyTest(
		//		App.testDataLastName, App.testDataFirstName, App.testDataNumber, page, true, playlist);

			/////////////////////////// IMAGE PRE PROCESSING TESTS ////////////////////////////////////

			handleDuplicates (contactDuplicates, page);
			handleErrors (contactErrors, page);
			contactErrors = new List<ContactData> ();//reset error list
			contactDuplicates = new List<ContactData>();//reset duplciates list
		}

		public string[] SeparateFullNameIntoFirstAndLast(string rawName){
			string firstname = "", lastname = "";
			var wordReg = new Regex(Values.WORDREGEX);
			var specialReg = new Regex(Values.SINGLESPECIALCHARREGEX);
			Match wordMatch = wordReg.Match (RemoveCountryCodeAndSpecialChar(rawName, new Regex(Values.COUNTRYCODE), specialReg));
			//Match wordMatch = wordReg.Match(rawName);

			Debug.WriteLine ("[SeparateFullNameIntoFirstAndLast] ABOUT TO PROCESS NAME, strictnummatch w "+rawName+" is "+wordMatch.Success.ToString ());

			if(wordMatch.Success){
				Debug.WriteLine ("[SeparateFullNameIntoFirstAndLast] wordMatch: "+wordMatch.Groups [0].Value);

				//processing name
				string[] words = wordMatch.Groups [0].Value.Split (new char[] { ' ', '\t' });
				Debug.WriteLine ("words split by spaces count: " + words.Length);
				if (words.Length > 1) {
					firstname = words [0];
					for (int i = 1; i < words.Length; i++) {
						lastname += words [i];
					}
				} else {
					//usually happens in OCR misreads. add to error list for user to manually correct
					lastname = words [0];
					firstname = " ";
					//errorStatus = 1;
				}
				Debug.WriteLine ("[SeparateFullNameIntoFirstAndLast] firstname: "+firstname+"lastname: "+lastname);

				return new string[]{ firstname, lastname };
			}

			return null;
		}

		public bool isValidMobile(string number){
			Regex strictNumReg = new Regex (Values.STRICTNUMREGEX);
			Match strictNumMatch = strictNumReg.Match(number);
			if (!strictNumMatch.Success) {
				return false;
			}
			return true;
		}

		public void handleErrors(List<ContactData> errorList, CAPP page){
			var editIgnoreConfig = new ConfirmConfig () { 
				Title = "Some of these contacts' info was misread", 
				OkText = "Edit them",
				Message = errorList.Count+" contacts misread",
				CancelText = "Ignore",
			};
			editIgnoreConfig.OnConfirm = new Action<bool>(delegate(bool obj) {
				//correct/edit erroneous contact info
				foreach(ContactData error in errorList){
					page.Navigation.PushAsync(new EditContactPage(error, page));
				}
			});

			try{
				if (errorList == null || errorList.ElementAt(0) == null) {
				} else {
					Debug.WriteLine ("[Util loadContactsFromPic] errors so far "+errorList.Count);
					UserDialogs.Instance.Confirm (editIgnoreConfig);
				}
			}catch(ArgumentOutOfRangeException){
				Debug.WriteLine ("[CAPP Util.loadContactsFromPic() ] No errors");
			}
		}

		public async void handleDuplicates(List<ContactData> dList, CAPP page){
			string duplicates = "";

			foreach(ContactData c in dList){
				duplicates += c.Name;
			}
			if (!string.IsNullOrWhiteSpace (duplicates)) {
				UserDialogs.Instance.WarnToast ("Duplicates detected", "Some of these contacts already exist in this namelist. Try adding them to another namelist:\n" + duplicates, 8000);
			} else {
				string[] choices = new string[]{"Ignore", "Edit Manually"};
				var duplicateChoice = await page.DisplayActionSheet("Fixing Duplicates...", "OK", "Cancel", choices);
				if (string.Equals (duplicateChoice, "Edit Manually")) {
					foreach(ContactData x in dList){
						if(!string.IsNullOrWhiteSpace (x.LastName) && !string.IsNullOrWhiteSpace (x.Number)){
							await page.Navigation.PushAsync(new EditContactPage (x, page));
						}
					}
				}
			}
		}
		/// <summary>
		/// Saves the ContactData to the Contacts database, returns false if contact already exists in database
		/// </summary>
		/// <returns>Returns a string value that says function successful if contact name doesn't exist in the database yet. Otherwise, returns the contactToStore param as part of the duplicateList string param</returns>
		/// <param name="duplicatesAllowed">If set to <c>true</c> contacts with the same name in the database will be allowed. *not yet implemented</param>
		/// <param name="duplicateList">List of contacts who's names already exists in the db</param>
		/// <param name="contactToStore">The ContactData to save</param>
		public bool saveContactToDB(bool duplicatesAllowed, ContactData contactToStore, string playlist){
			if (!duplicatesAllowed) {
				if (!App.contactFuncs.duplicateExists (contactToStore, App.Database.GetItems (playlist).ToArray ())) {
					App.Database.SaveItem (contactToStore);
					return true;
				} else {
					return false;
				}
			} else {
				App.Database.SaveItem (contactToStore);
				return true;
			}
		}
		/*public async Task<bool> saveContactToDB(bool duplicatesAllowed, ContactDataItemAzure contactToStore, string playlist){
			if (!duplicatesAllowed) {
				if (!App.contactFuncs.duplicateExists (contactToStore, (await App.AzureDB.GetItems (playlist)).ToArray ())) {
					await App.AzureDB.SaveItem (contactToStore, true);
					return true;
				} else {
					return false;
				}
			} else {
				await App.AzureDB.SaveItem (contactToStore, true);
				return true;
			}
		}*/
		public bool saveMultipleContactToDB(bool duplicatesAllowed, List<ContactData> contactsToStore, string playlist){
			var DBListArr = App.Database.GetItems (playlist).ToArray ();
			var listArr = contactsToStore.ToArray ();
			List<ContactData> saveList = new List<ContactData>();

			if (!duplicatesAllowed) {
				for(int x = 0;x < listArr.Length;x++){
					if (!App.contactFuncs.duplicateExists (listArr[x], DBListArr)) {
						saveList.Add (listArr[x]);
					}
				}
				App.Database.SaveAll (saveList.AsEnumerable ());
				return true;
			} else {
				App.Database.SaveAll (contactsToStore.AsEnumerable ());
				return true;
			}
		}


		/*public string saveContactVMToDB(string duplicateList, ContactViewModel contactToStore){
			if (!App.contactFuncs.duplicateContactVMExists (contactToStore, loadContactsIntoViewModel(App.Database.GetItems ("All").ToList ()))) {
				App.Database.SaveItem (contactToStore);//not yet converted
				return Values.NODUPLICATES;
			} else {
				duplicateList += contactToStore.Contact.FirstName + " " + contactToStore.Contact.LastName + "\n";
				return duplicateList;
			}
		}*/

		/// <summary>
		/// returns a single string from the appended elements of a string array
		/// </summary>
		/// <returns>The string array to string.</returns>
		/// <param name="strArrayToConvert">The string array that you want to store in a single string</param>
		public string convertStringArrayToString(string[] strArrayToConvert){
			return null;
		}

		public string subtractFromString(string stringToRemove, string originalString){
			return originalString.Replace (stringToRemove, "");
		}

		public bool duplicateExists(ContactData contact, ContactData[] contactsArr){
			//var contactsArr = currentContacts.ToArray ();
			bool match = false;
			/*foreach(ContactData i in currentContacts){
				if ((contact.FirstName == i.FirstName) && (contact.LastName == i.LastName) && (contact.Playlist == i.Playlist)) {
					match = true;
				}
			}*/
			for(int c = 0;c < contactsArr.Length;c++){
				if ((contact.FirstName == contactsArr[c].FirstName) && (contact.LastName == contactsArr[c].LastName) && (contact.Playlist == contactsArr[c].Playlist)) {
					match = true;
				}
			}
			return match;
		}
		/*public bool duplicateExists(ContactDataItemAzure contact, ContactDataItemAzure[] contactsArr){
			//var contactsArr = currentContacts.ToArray ();
			bool match = false;
			for(int c = 0;c < contactsArr.Length;c++){
				if ((contact.FirstName == contactsArr[c].FirstName) && (contact.LastName == contactsArr[c].LastName) && (contact.Playlist == contactsArr[c].Playlist)) {
					match = true;
				}
			}
			return match;
		}*/
		/*public bool duplicateExists(PlaylistItemAzure playlist, PlaylistItemAzure[] contactsArr){
			bool match = false;
			for(int c = 0;c < contactsArr.Length;c++){
				if (playlist.PlaylistName == contactsArr[c].PlaylistName) {
					match = true;
				}
			}
			return match;
		}*/

		public bool duplicateContactVMExists(ContactViewModel contact, List<ContactViewModel> currentContacts){
			bool match = false;
			foreach(ContactViewModel i in currentContacts){
				if ((contact.Contact.FirstName == i.Contact.FirstName) && (contact.Contact.LastName == i.Contact.LastName)) {
					match = true;
				}
			}
			return match;
		}
		public async Task<bool> loadDeviceContactsIntoDBSingleTransaction(bool timeExecution){
			Debug.WriteLine ("In loadDeviceContactsIntoDBSingleTransaction");
			Stopwatch stopwatch = null;
			if (timeExecution) {
				stopwatch = Stopwatch.StartNew();
			}

			string aff = "";
			List<Plugin.Contacts.Abstractions.Phone> listp;

			IQueryable<Contact> list = await getDeviceContacts ();

			var listArr = list.ToArray ();
			List<ContactData> formattedList = new List<ContactData>();

			if (contactPermission) {
				Debug.WriteLine ("In contactPermission");
				for (int c = 0;c < listArr.Length;c++) {
					try {
						aff = listArr[c].Organizations.ElementAtOrDefault (0).Name;
					} catch (NullReferenceException) {
						aff = "";
					}
					try {
						listp = (List<Plugin.Contacts.Abstractions.Phone>)listArr[c].Phones;

						Debug.WriteLine ("Adding to list");
						formattedList.Add(new ContactData {
							FirstName = listArr[c].FirstName,
							LastName = listArr[c].LastName,
							Number = listp.ElementAtOrDefault(0).Number,
							Playlist = Values.ALLPLAYLISTPARAM,
							Aff = aff
						});

					} catch (NullReferenceException) {
						Debug.WriteLine (listArr[c].DisplayName + " doesn't have a phone number, skipping");
					}
				}

				Debug.WriteLine ("Saving to DB");
				saveMultipleContactToDB (false, formattedList, Values.ALLPLAYLISTPARAM);
			}

			if(timeExecution){
				stopwatch.Stop();
				UserDialogs.Instance.WarnToast("Phonebook load+save time", stopwatch.ElapsedMilliseconds+" ms", 5000);
			}
			return true;
		}
		public async Task<bool> loadDeviceContactsIntoDB(bool timeExecution){
			Stopwatch stopwatch = null;
			if (timeExecution) {
				stopwatch = Stopwatch.StartNew();
			}

			string aff = "";
			List<Plugin.Contacts.Abstractions.Phone> listp;
			IQueryable<Contact> list = await getDeviceContacts ();

			if (contactPermission) {
				foreach (Contact c in list.ToList()) {
					try {
						aff = c.Organizations.ElementAtOrDefault (0).Name;
					} catch (NullReferenceException) {
						aff = "";
					}
					try {
						listp = (List<Plugin.Contacts.Abstractions.Phone>)c.Phones;
						// load device contacts if not yet in db
						saveContactToDB (false, new ContactData {
							FirstName = c.FirstName,
							LastName = c.LastName,
							Number = listp.ElementAtOrDefault(0).Number,
							Playlist = Values.ALLPLAYLISTPARAM,
							Aff = aff
						}, Values.ALLPLAYLISTPARAM);
					} catch (NullReferenceException) {
						Debug.WriteLine (c.DisplayName + " doesn't have a phone number, skipping");
					}
				}
			}

			if(timeExecution){
				stopwatch.Stop();
				UserDialogs.Instance.WarnToast("Phonebook load+save time", stopwatch.ElapsedMilliseconds+" ms", 5000);
			}
			return true;
		}

		public List<ContactViewModel> loadContactsIntoViewModel(List<ContactData> contacts){
			List<ContactViewModel> temp = new List<ContactViewModel>();
			foreach(ContactData c in contacts){
				temp.Add (new ContactViewModel(c.FirstName, c.LastName, c.Number, c.Aff, c.Playlist/*, 
					new CAPPDates{Called = c.Called, Appointed = c.Appointed, Presented = c.Presented, Purchased = c.Purchased}*/));
			}
			return temp;
		}

		public async Task<IQueryable<Contact>> getDeviceContacts(){
			IQueryable<Contact> list = null;
			contactPermission = await CrossContacts.Current.RequestPermission ();
			if(contactPermission) 
			{
				CrossContacts.Current.PreferContactAggregation = false;//recommended

				if(CrossContacts.Current.Contacts == null)
					return null;

				list = CrossContacts.Current.Contacts
					.Where(c => !string.IsNullOrWhiteSpace(c.LastName) && c.Phones.Count > 0).OrderBy(c => c.DisplayName);
			}
			return list;
		}
	}
}