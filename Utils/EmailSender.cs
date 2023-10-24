using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public class EmailSender : IEmailSender
	{
		/*private readonly MailSettings _settings;
		private readonly IWebHostEnvironment _environment;
*/
		public EmailSender()//IOptions<MailSettings> settings)
		{
			//this._settings = settings.Value;

		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			/*CancellationToken ct = default;
			try
			{
				// Initialize a new instance of the MimeKit.MimeMessage class
				var mail = new MimeMessage();


				// Sender
				mail.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
				mail.Sender = new MailboxAddress(_settings.DisplayName, _settings.From);


				mail.To.Add(MailboxAddress.Parse(email));

				// Add Content to Mime Message
				var body = new BodyBuilder();
				mail.Subject = subject;
				body.HtmlBody = htmlMessage;
				mail.Body = body.ToMessageBody();




				using var smtp = new SmtpClient();

				if (_settings.UseSSL)
				{
					await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
				}
				else if (_settings.UseStartTls)
				{
					await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
				}

				await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
				await smtp.SendAsync(mail, ct);
				await smtp.DisconnectAsync(true, ct);




			}
			catch (Exception)
			{

			}*/

		}
	}
	}
