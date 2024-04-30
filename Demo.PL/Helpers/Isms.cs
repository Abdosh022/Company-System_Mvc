using Demo.DAL.Models;
using System.Net.Mail;
using Twilio.Rest.Api.V2010.Account;

namespace Demo.PL.Helpers
{
	public interface Isms
	{
		public MessageResource Send(SmsMessage sms);
	}
}
