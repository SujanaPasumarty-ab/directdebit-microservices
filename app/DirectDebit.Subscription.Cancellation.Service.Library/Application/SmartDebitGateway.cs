using RestSharp;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using DirectDebit.Subscription.Cancellation.Service.Library.Databases;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Application
{
	public class SmartDebitGateway
	{
		private readonly IDbContext _context;

		public string base_url { get; set; }
		public string username { get; set; }
		public string password { get; set; }

		private RestClient Client { get; set; }
		private RestRequest? Request { get; set; }
		public RestResponse? Response { get; set; }

		public XmlDocument ContentXml
		{
			get
			{
				if (Response?.Content != null)
				{
					XmlDocument x = new XmlDocument();
					x.LoadXml(Response.Content);
					return x;
				}
				else
					return null;
			}
		}

		public SmartDebitGateway(IDbContext context)
		{
			_context = context;

			//Get smart debit settings from the database
			base_url ??= _context.DirectDebitSettings.First(s => s.SettingName == "URL").SettingValue;
			username ??= _context.DirectDebitSettings.First(s => s.SettingName == "USERNAME").SettingValue;
			password ??= _context.DirectDebitSettings.First(s => s.SettingName == "PASSWORD").SettingValue;

			Client = new RestClient(new RestClientOptions(base_url)
			{
				Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(username, password)
			});
		}

		public RestResponse GetResponse(string Path, XmlDocument Xml)
		{
			if (Xml != null)
			{
				Request = new RestRequest(Path, Method.Post);
				Request.RequestFormat = DataFormat.Xml;

				Request.AddParameter("application/xml", Xml.OuterXml, ParameterType.RequestBody);

				Response = (RestResponse)Client.Execute(Request);
			}
			else
			{
				Request = new RestRequest(Path, Method.Get);
				Request.RequestFormat = DataFormat.Xml;
				Response = (RestResponse)Client.Get(Request);
			}

			return Response;
		}

		[System.Serializable]
		public class variable_ddi
		{
			public string reference_number { get; set; }
			public string first_name { get; set; }
			public string last_name { get; set; }
			public string address_1 { get; set; }
			public string address_2 { get; set; }
			public string town { get; set; }
			public string county { get; set; }
			public string postcode { get; set; }
			public string country { get; set; }
			public string account_name { get; set; }
			public string sort_code { get; set; }
			public string account_number { get; set; }
			public string frequency_type { get; set; }  //W M Q Y
			public string start_date { get; set; } //YYYY-MM-DD
			public string end_date { get; set; } //YYYY-MM-DD
			public string default_amount { get; set; }
			public string first_amount { get; set; }
			public string frequency_factor { get; set; } //1 - 4 (1 only with Y)
			public string email_address { get; set; }
			public string payer_reference { get; set; }
			public service_user service_user { get; set; }
			public List<debit> debits { get; set; }

			public XmlDocument xml
			{
				get
				{
					XmlSerializer ser = new XmlSerializer(this.GetType());
					string result = string.Empty;

					using (MemoryStream memStm = new MemoryStream())
					{
						ser.Serialize(memStm, this);

						memStm.Position = 0;
						result = new StreamReader(memStm).ReadToEnd();
					}
					XmlDocument xml = new XmlDocument();
					xml.LoadXml(result);
					return xml;
				}
			}
		}

		public class debit
		{
			public string amount { get; set; } //in pence
			public string date { get; set; }    //yyyy-MM-dd
		}

		[System.Serializable]
		public class service_user
		{
			public service_user(IDbContext context)
			{
				pslid = context.DirectDebitSettings.First(s => s.SettingName == "SUID").SettingValue;
			}

			public string pslid;
		}

		[System.Serializable]
		public class adhoc_ddi
		{
			public string reference_number { get; set; }
			public string first_name { get; set; }
			public string last_name { get; set; }
			public string address_1 { get; set; }
			public string address_2 { get; set; }
			public string town { get; set; }
			public string county { get; set; }
			public string postcode { get; set; }
			public string country { get; set; }
			public string account_name { get; set; }
			public string sort_code { get; set; }
			public string account_number { get; set; }
			public string email_address { get; set; }
			public string payer_reference { get; set; }
			public service_user service_user { get; set; }

			public XmlDocument xml
			{
				get
				{
					XmlSerializer ser = new XmlSerializer(this.GetType());
					string result = string.Empty;

					using (MemoryStream memStm = new MemoryStream())
					{
						ser.Serialize(memStm, this);

						memStm.Position = 0;
						result = new StreamReader(memStm).ReadToEnd();
					}
					XmlDocument xml = new XmlDocument();
					xml.LoadXml(result);
					return xml;
				}
			}
		}

		[System.Serializable]
		public class query
		{
			public string reference_number { get; set; }
			public service_user service_user { get; set; }

			public XmlDocument xml
			{
				get
				{
					XmlSerializer ser = new XmlSerializer(this.GetType());
					string result = string.Empty;

					using (MemoryStream memStm = new MemoryStream())
					{
						ser.Serialize(memStm, this);

						memStm.Position = 0;
						result = new StreamReader(memStm).ReadToEnd();
					}
					XmlDocument xml = new XmlDocument();
					xml.LoadXml(result);
					return xml;
				}
			}
		}
	}
}