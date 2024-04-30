using Demo.DAL.Models;
using Demo.PL.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Demo.PL.Helpers
{
	public class SMS : Isms
	{
		private readonly TwillioSettings _options;

		public SMS(IOptions<TwillioSettings> options)
		{
			_options = options.Value;
		}
		public MessageResource Send(SmsMessage sms)
		{
			TwilioClient.Init(_options.AccountSID, _options.AuthToken);

			var result = MessageResource.Create(
				body: sms.Body,
				from: new Twilio.Types.PhoneNumber(_options.TwilioPhoneNumber),
				to: sms.phoneNumber);

			return result;
		}
	}
}
