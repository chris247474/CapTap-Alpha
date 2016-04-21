﻿using System;

namespace Capp2
{
	public static class Values
	{
		public const int CALLTOTEXTDELAY = 4000;

		public const string BACKGROUNDLIGHTSILVER = "#F5F6F7";
		public const string GOOGLEBLUE = "#4285F4";
		public const string TESTCOLOR = "#512DA8";
		public const string PIVOTALNAVBLUE = "#1C4D76";
		public const string PURPLE = "#512DA8";
		public const string CYAN = "#00BCD4";
		public const string TEAL = "#009688";
		public const string ORANGE = "#FF9800";

		public static string ApplicationURL = @"https://secretfiles.azurewebsites.net";

		public static string ISEDITING = "isediting";
		public static string DONEEDITING = "doneediting";

		public static string TODAY = "today";
		public static string TOMORROW = "tomorrow";
		public static string MEETINGSREMINDED = "reminded";
		public static string MEETINGSNOTYETREMINDED = "notreminded";
		public static string CONFIRM = "confirm";
		public static string BOM = "bom";

		public static string DONEWITHCALL = "Done";
        public static string iOSDONEWITHCALL = "iOSDone";
        public static string DONEWITHNOCALL = "DoneNoCall";

		public const string TODAYSCALLS = "Today's Calls";
		public const int TOMORROWMEETINGREMINDTIME = 3;

		public const bool MEASUREEXECUTION = true;

		public const string PURCHASED = "purchased";
		public const string PRESENTED = "presented";
		public const string APPOINTED = "appointed";
		public const string CALLED = "called";
		public const string NEXT = "next";
		public const string APPOINTMENTDESCRIPTIONBOM = "Appointed for BOM";
		public const string FOLLOWUP = "Follow up";
		public const double MEETINGLENGTH = 1.5;
		public const string NEXTMEETINGDEFAULT = "-1";
		public const int _5PMBOM = 17;

		public const string NAMENUMORGNOTESREGEX = "(([A-Z]*[a-z]*)(\\s*))*((\\()*(\\))*(\\d+)*(\\s)*(-)*(\\d+)*(-)*)*(\\w*\\s*)*";
		public const string OLDNAMENUMREGEX = "(([A-Z]*[a-z]*)(\\s*))*((\\()*(\\))*(\\d+)*(\\s)*(-)*(\\d+)*(-)*)*";
		public const string NAMENUMREGEX = "(([A-Z]*[a-z]*)(\\s*))*((\\()*(\\))*(\\d+)*([A-Z]*[a-z]*)(\\s)*(-)*)*(\\d)";
		public const string NUMREGEX = "((\\()*(\\))*(\\d+)*(\\s)*(-)*(\\d+)*(-)*)*";
		public const string CALLABLENUMREGEX = "9((\\()*(\\))*(\\d+)*(\\s)*(-)*(\\d+)*(-)*)*";
		public const string STRICTNUMREGEX = "9\\d\\d\\d\\d\\d\\d\\d\\d\\d";
		public const string WORDREGEX = "(([A-Z]*[a-z]*)(\\s*))*";
		public const string ANYSTRINGREGEX = "(\\w*\\s*)*";
		public const string INVALIDINBETWEENNUMREGEX = "(([A-Z]*(\\(*\\)*)[a-z]*)(\\s*)\\-*)*";
		//SINGLESPECIALCHARREGEX doesnt detect "_" char cause of xamarin studio compiler confusion (doesnt detect, causes error) 
		public const string SINGLESPECIALCHARREGEX = "(\\**\\'*\\`*\\~*\\.*\\,*\\?*\\:*\\;*\\\\*\\/*\\[*\\]*\\{*\\}*\\<*\\>*\\+*\\!*\\@*\\&*\\^*\\&*\\%*\\$*\\#*\\|*\\-*\\=*\\\"*)";
		public const string COMPLETESINGLESPECIALCHARREGEX = "\\(*\\)*\\**\\'*\\`*\\~*\\.*\\,*\\?*\\:*\\;*\\\\*\\/*\\[*\\]*\\{*\\}*\\<*\\>*\\+*\\!*\\@*\\&*\\^*\\&*\\%*\\$*\\#*\\|*\\-*\\=*\\\"*\\ *";
		public const string COUNTRYCODE = "\\+63";
		public const string WEIRDCHARFL = "ﬂ";
		public const string LONGDASH = "—";
		public const string WEIRDQUOTE = "‘";
		public const string OTHERWEIRDQUOTE = "’";
		public const string UNDERLINE = "_";
		public const string WEIRDCHARFI = "ﬁ";

		public const string FNAMEPARAM = "fname";
		public const string LNAMEPARAM = "lname";
		public const string NUMBERPARAM = "num";
		public const string AFFPARAM = "aff";
		public const string NOTESPARAM = "notes";

		public const string NODUPLICATES = "";

		public const string ALLPLAYLISTPARAM = "All";

		//image preprocessing values. adjust to change image OCR readability. so far 28/30 perfect reads w current values
		public const double GAUSSIANSIGMA = 0;// 0 so far best value for OCR to read edges of font accurately
		public const double GAUSSIANSIZEX = 3;//3 usually //must be odd no diff when using 51
		public const double GAUSSIANSIZEY = 3;//3 usually //must be odd no diff when using 51
		public const double SHARPENMASKWEIGHT = 1;//1 gives better OCR readability then 2, 1.5, 0.8, 0.9

		public const double ADDWEIGHT = 1;

		public const int ADAPTIVETHRESHBLOCKSIZE = 13;//Must be odd
		public const int ADAPTIVETHRESHPARAM = 10;
	}
}