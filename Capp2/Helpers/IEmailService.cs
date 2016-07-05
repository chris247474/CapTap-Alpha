using System;

namespace Capp2
{
	public interface IEmailService
	{
		void SendEmail(string recipient = "", string body = "", string subject = "");
		void SendDailyEmail();
	}
}

