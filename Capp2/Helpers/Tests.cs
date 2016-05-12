using System;
using System.Collections.Generic;
using Plugin.Calendars.Abstractions;
using Xamarin.Forms;
using System.Diagnostics;
using System.Linq;
using Acr.UserDialogs;
using Capp2.Helpers;

namespace Capp2
{
	public static class Tests
	{
		/*public static async void testAzure(){
			try{
				var contacts = await App.AzureDB.GetItems (Values.ALLPLAYLISTPARAM, true);
				Debug.WriteLine ("GOT AZURE DATA");
				var arr = contacts.ToArray ();
				for (int c = 0; c < arr.Length; c++) {
					Debug.WriteLine ("From Azure: "+c+". "+arr[c].Name + " " + arr[c].Number);
				}
			}catch(Exception e){
				Debug.WriteLine ("Azure retrieve error: "+e.Message);
			}
		}*/
		public static void AccuracyTest(string[] lastname, string[] firstname, string[] number, CAPP page, bool printComparisons, string playlist){
			List<ContactData> DBList = App.Database.GetItems (playlist).ToList();
			List<ContactData> control = new List<ContactData> ();
			List<ContactData> temp = new List<ContactData>();
			float d = (float)lastname.Length, numerator = 0.00f;
			float accuracy = 0.00f;
			int similar = 15, similarNumber = 0;
			string mismatch = "";
			int fNameD = 0, lNameD = 0, numD = 0;

			if (lastname.Length != DBList.Count || number.Length != DBList.Count || firstname.Length != DBList.Count) {
				Debug.WriteLine ("[Util.AccuracyTest] "+lastname.Length+","+number.Length+","+firstname.Length+","+DBList.Count);
				UserDialogs.Instance.WarnToast("Image reading error", "not all contacts were loaded from image", 4000);
			} else if (lastname == null || number == null || firstname == null) {
				//page.DisplayAlert ("", "test data null", "Cancel");
				UserDialogs.Instance.WarnToast("Test Data not populated", "test data null", 4000);
			} else {
				for(int c = 0; c < lastname.Length; c++){
					temp.Add (new ContactData{
						LastName = lastname[c],
						FirstName = firstname[c],
						Name = firstname[c]+" "+lastname[c],
						Number = number[c]
					});
				}
				control = (from x in temp.OrderBy (x => x.LastName)
					select x).ToList ();

				if (printComparisons) {	
					//print out namelist
					foreach (ContactData i in DBList) {
						Debug.WriteLine ("[Util.AccuracyTest - DB contacts] " + DBList.IndexOf (i) + ". " + i.Name + " " + i.Number);
					}
					//print out perfect namelist to compare to
					foreach (ContactData i in control) {
						Debug.WriteLine ("[Util.AccuracyTest - Perfect test contacts] " + control.IndexOf (i) + ". " + i.Name + " " + i.Number);
					}
				}

				Levenshtein ld = new Levenshtein ();

				foreach (ContactData x in control) {
					int lastCount = 0;
					float numeratorBefore = numerator;

					int c = 0;
					for (; c < DBList.Count; c++) {
						lastCount = c;
						fNameD = ld.LD (x.FirstName, DBList.ElementAt (c).FirstName);
						lNameD = ld.LD (x.LastName, DBList.ElementAt (c).LastName);
						numD = ld.LD (x.Number, DBList.ElementAt (c).Number);
						if (fNameD <= similar && lNameD <= similar && numD == similarNumber) {
							numerator++;
						}
					}

					if (numerator == numeratorBefore) {
						mismatch += control.IndexOf(x)+": "+"("+fNameD+","+lNameD+")"+x.Name+" "+"("+numD+")"+x.Number+" no match within "+similar+","+similarNumber+" Lev Distance for name,number found \n";
					}
				}

				accuracy = (numerator / d)*100;
				//page.DisplayAlert("Accuracry", "Accuracy is "+numerator+"/"+d+" = "+accuracy+"%", "OK");
				UserDialogs.Instance.WarnToast("Accuracy", "Accuracy is "+numerator+"/"+d+" = "+accuracy+"%", 4000);
				//page.DisplayAlert("Misreads", "Error matching: "+mismatch+"\n", "OK");
				UserDialogs.Instance.WarnToast("Misreads", "Error matching: "+mismatch+"\n", 4000);
			}
		}
		public static void MeasureLoadDatabaseTime(){
			/*var watch = Stopwatch.StartNew (); 
			var list = App.Database.GetItemsAsync(Values.ALLPLAYLISTPARAM).Result;
			watch.Stop ();

			var first = watch.ElapsedMilliseconds;

			watch = Stopwatch.StartNew ();
			list = App.Database.GetItems (Values.ALLPLAYLISTPARAM);
			watch.Stop ();

			var second = watch.ElapsedMilliseconds;

			Debug.WriteLine ("[CAPP Loading Time Test] GetItemsAsync Time:" + first+", GetItems Time:" + second);


			watch = Stopwatch.StartNew (); 
			App.Database.GetGroupedItemsFasterAsync(Values.ALLPLAYLISTPARAM);
			watch.Stop ();

			first = watch.ElapsedMilliseconds;

			watch = Stopwatch.StartNew ();
			App.Database.GetGroupedItems (Values.ALLPLAYLISTPARAM);
			watch.Stop ();

			second = watch.ElapsedMilliseconds;

			Debug.WriteLine ("[CAPP Loading Time Test] GetGroupedItemsFaster Time:" + first+", GetGroupedItems Time:" + second);
*/
			}
		public static void testCalendarForTomorrow(){
			//check events tomorrow
			IList<CalendarEvent> meetingsTomorrow = CalendarService.GetAppointmentsTomorrow ();
			List<ContactData> peopleTomorrow = new List<ContactData> ();
			if (meetingsTomorrow == null) {
				Debug.WriteLine ("NO MEETINGS TOMORROW");
			} else {
				foreach(CalendarEvent e in meetingsTomorrow){
					foreach(ContactData c in App.Database.GetItems (Values.ALLPLAYLISTPARAM)){
						if(e.ExternalID == c.NextMeetingID){
							peopleTomorrow.Add (c);
						}
					}
				}
				Debug.WriteLine ("DONE LOOKING FOR PEOPLE IM MEETING TOMORROW, number of meetings: "+peopleTomorrow.Count);
			}

			foreach(ContactData x in peopleTomorrow){
				Debug.WriteLine ("Im meeting "+x.Name+" tomorrow");
			}
		}
	}
}

