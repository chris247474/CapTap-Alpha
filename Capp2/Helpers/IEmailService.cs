using System;

namespace Capp2
{
	public interface IEmailService
	{
		void SendEmail(string body = "");
		void SendDailyEmail();
	}
}

