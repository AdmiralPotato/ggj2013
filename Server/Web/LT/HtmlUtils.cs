using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Security.Cryptography;

namespace LT
{
	public class HtmlUtils
	{
		public static string SiteName
		{
			get { return ConfigurationManager.AppSettings["SiteName"]; }
		}	

        public static string ServerAddress
        {
            get { return ConfigurationManager.AppSettings["ServerAddress"]; }
        }

        public static int Now
        {
            get { return Convert.ToInt32((DateTime.Now - new DateTime(2000, 1, 1)).TotalSeconds); }
        }

		static Random randomGenerator = new Random();
		public static Random RandomGenerator
		{
			get { return HtmlUtils.randomGenerator; }
		}

		// http://www.regexlib.com/REDetails.aspx?regexp_id=711
		static Regex emailRegex =
			new Regex(@"^((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*""\x20*)*(?<angle><))?((?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*"")@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>)$");

		public static bool IsValidEmailAddress(string emailAddress)
		{
			// verify the address
			if (!emailRegex.IsMatch(emailAddress))
				return false;

			// lookup the hostname
			string host = emailAddress.Split('@')[1];
			try
			{
				Dns.GetHostEntry(host);
			}
			catch(SocketException) 
			{
				return false;
			}
			return true;
		}

		// http://weblogs.asp.net/rosherove/archive/2003/05/13/6963.aspx
		private static Regex stripHtmlRegex = new Regex(@"<(.|\n)*?>");
		public static string StripHtml(string html)
		{
			//This pattern Matches everything found inside html tags;
			//(.|\n) - > Look for any character or a new line
			// *?  -> 0 or more occurences, and make a non-greedy search meaning
			//That the match will stop at the first available '>' it sees, and not at the last one
			//(if it stopped at the last one we could have overlooked 
			//nested HTML tags inside a bigger HTML tag..)
			// Thanks to Oisin and Hugh Brown for helping on this one... 
			return stripHtmlRegex.Replace(html, String.Empty).Replace("&nbsp;", String.Empty);
		}

        public static uint IpAddressToInteger(string ipAddress)
        {
            uint r = 0;
            foreach (var s in ipAddress.Split('.'))
                r = (r << 8) ^ UInt32.Parse(s);
            return r;
        } 

		public static bool HandleException(Exception exception, HttpContext context)
		{
			var httpException = exception as HttpException;
            if (httpException != null && (httpException.GetHttpCode() == 405 || httpException.GetHttpCode() == 404 || httpException.GetHttpCode() == 403))
			{
				context.Response.StatusCode = httpException.GetHttpCode();
				context.Response.SuppressContent = true;
				context.Response.End();
				return false;
			}

			var mailClient = new SmtpClient(ConfigurationManager.AppSettings["MailServer"]);

			var errorMessage = new StringBuilder();

			if (exception is HttpUnhandledException)
				exception = exception.InnerException;

			errorMessage.Append(exception.ToString());

			if (httpException != null)
				errorMessage.Append("\n\nHTTP EXCEPTION CODE: " + httpException.GetHttpCode());

			if (exception.InnerException != null)
			{
				errorMessage.Append("\n\n ***INNER EXCEPTION*** \n");
				errorMessage.Append(exception.InnerException.ToString());
			}

			if (context != null)
			{
                //if (context.Request.IsLocal)
                //    return;
                errorMessage.AppendFormat("\n\nRequest Path = {0}\n", context.Request.Url);

				errorMessage.Append("\n\n ***REQUEST PARAMETERS*** \n");
				foreach (string name in context.Request.Params.Keys)
				{
					errorMessage.AppendFormat("\n{0} = {1};", name, context.Request[name]);
				}

                if (context.Request.RequestType == "POST")
                {
                    errorMessage.Append("\n\n ***POST DATA*** \n");

                    using (var reader = new StreamReader(context.Request.InputStream))
                    {
                        errorMessage.Append(reader.ReadToEnd());
                    }
                }

                if (context.Session != null)
                {
                    errorMessage.Append("\n\n ***SESSION VARIABLES*** \n");
                    foreach (string key in context.Session.Keys)
                    {
                        errorMessage.AppendFormat("\n{0} = {1};", key, context.Session[key]);
                    }
                }
			}

            System.Diagnostics.Debug.Print(errorMessage.ToString());            

            try
			{
				mailClient.Send
				(
					SiteName + " Web Server <server@cooltext.com>",
					ConfigurationManager.AppSettings["ErrorEmail"],
					SiteName + " Error " + Guid.NewGuid().ToString(),
					errorMessage.ToString()
				);
			}
			catch (SmtpException)
			{
			}

            return true;
		}

        static SHA512 hasher = System.Security.Cryptography.SHA512.Create();

        public static string CalculateHash(string input)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes("$!D*c3X;`}|{" + input);
            byte[] hash = hasher.ComputeHash(inputBytes);
            return Convert.ToBase64String(hash);
        }

        // generate random password
        static char[] pwdCharArray = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        static Random Random = new Random();

        public static string GeneratePassword(int length)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < length; i++)
                result.Append(pwdCharArray[Random.Next(pwdCharArray.Length)]);

            return result.ToString();
        }
	}
}