using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000260 RID: 608
	public class AUTH_Gmail_OAuth1_3leg
	{
		// Token: 0x060015D4 RID: 5588 RVA: 0x00087D3F File Offset: 0x00086D3F
		public AUTH_Gmail_OAuth1_3leg() : this("anonymous", "anonymous")
		{
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x00087D54 File Offset: 0x00086D54
		public AUTH_Gmail_OAuth1_3leg(string consumerKey, string consumerSecret)
		{
			bool flag = consumerKey == null;
			if (flag)
			{
				throw new ArgumentNullException("consumerKey");
			}
			bool flag2 = consumerKey == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'consumerKey' value must be specified.", "consumerKey");
			}
			bool flag3 = consumerSecret == null;
			if (flag3)
			{
				throw new ArgumentNullException("consumerSecret");
			}
			bool flag4 = consumerSecret == "";
			if (flag4)
			{
				throw new ArgumentException("Argument 'consumerSecret' value must be specified.", "consumerSecret");
			}
			this.m_ConsumerKey = consumerKey;
			this.m_ConsumerSecret = consumerSecret;
			this.m_pRandom = new Random();
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x00087E2D File Offset: 0x00086E2D
		public void GetRequestToken()
		{
			this.GetRequestToken("oob");
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x00087E3C File Offset: 0x00086E3C
		public void GetRequestToken(string callback)
		{
			bool flag = callback == null;
			if (flag)
			{
				throw new ArgumentNullException("callback");
			}
			bool flag2 = !string.IsNullOrEmpty(this.m_RequestToken);
			if (flag2)
			{
				throw new InvalidOperationException("Invalid state, you have already called this 'GetRequestToken' method.");
			}
			string text = this.GenerateTimeStamp();
			string text2 = this.GenerateNonce();
			string requestUriString = "https://www.google.com/accounts/OAuthGetRequestToken?scope=" + this.UrlEncode(this.m_Scope);
			string value = "https://www.google.com/accounts/OAuthGetRequestToken";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("oauth_callback=" + this.UrlEncode(callback));
			stringBuilder.Append("&oauth_consumer_key=" + this.UrlEncode(this.m_ConsumerKey));
			stringBuilder.Append("&oauth_nonce=" + this.UrlEncode(text2));
			stringBuilder.Append("&oauth_signature_method=" + this.UrlEncode("HMAC-SHA1"));
			stringBuilder.Append("&oauth_timestamp=" + this.UrlEncode(text));
			stringBuilder.Append("&oauth_version=" + this.UrlEncode("1.0"));
			stringBuilder.Append("&scope=" + this.UrlEncode(this.m_Scope));
			string signatureBase = "GET&" + this.UrlEncode(value) + "&" + this.UrlEncode(stringBuilder.ToString());
			string value2 = this.ComputeHmacSha1Signature(signatureBase, this.m_ConsumerSecret, null);
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("Authorization: OAuth ");
			stringBuilder2.Append("oauth_version=\"1.0\", ");
			stringBuilder2.Append("oauth_nonce=\"" + text2 + "\", ");
			stringBuilder2.Append("oauth_timestamp=\"" + text + "\", ");
			stringBuilder2.Append("oauth_consumer_key=\"" + this.m_ConsumerKey + "\", ");
			stringBuilder2.Append("oauth_callback=\"" + this.UrlEncode(callback) + "\", ");
			stringBuilder2.Append("oauth_signature_method=\"HMAC-SHA1\", ");
			stringBuilder2.Append("oauth_signature=\"" + this.UrlEncode(value2) + "\"");
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers.Add(stringBuilder2.ToString());
			using (WebResponse response = httpWebRequest.GetResponse())
			{
				using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
				{
					foreach (string text3 in HttpUtility.UrlDecode(streamReader.ReadToEnd()).Split(new char[]
					{
						'&'
					}))
					{
						string[] array2 = text3.Split(new char[]
						{
							'='
						});
						bool flag3 = string.Equals(array2[0], "oauth_token", StringComparison.InvariantCultureIgnoreCase);
						if (flag3)
						{
							this.m_RequestToken = array2[1];
						}
						else
						{
							bool flag4 = string.Equals(array2[0], "oauth_token_secret", StringComparison.InvariantCultureIgnoreCase);
							if (flag4)
							{
								this.m_RequestTokenSecret = array2[1];
							}
						}
					}
				}
			}
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x00088178 File Offset: 0x00087178
		public string GetAuthorizationUrl()
		{
			bool flag = this.m_RequestToken == null;
			if (flag)
			{
				throw new InvalidOperationException("You need call method 'GetRequestToken' before.");
			}
			return "https://accounts.google.com/OAuthAuthorizeToken?oauth_token=" + this.UrlEncode(this.m_RequestToken) + "&hd=default";
		}

		// Token: 0x060015D9 RID: 5593 RVA: 0x000881C0 File Offset: 0x000871C0
		public void GetAccessToken(string verificationCode)
		{
			bool flag = verificationCode == null;
			if (flag)
			{
				throw new ArgumentNullException("verificationCode");
			}
			bool flag2 = verificationCode == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'verificationCode' value must be specified.", "verificationCode");
			}
			bool flag3 = string.IsNullOrEmpty(this.m_RequestToken);
			if (flag3)
			{
				throw new InvalidOperationException("Invalid state, you need to call 'GetRequestToken' method first.");
			}
			bool flag4 = !string.IsNullOrEmpty(this.m_AccessToken);
			if (flag4)
			{
				throw new InvalidOperationException("Invalid state, you have already called this 'GetAccessToken' method.");
			}
			string text = "https://www.google.com/accounts/OAuthGetAccessToken";
			string text2 = this.GenerateTimeStamp();
			string text3 = this.GenerateNonce();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("oauth_consumer_key=" + this.UrlEncode(this.m_ConsumerKey));
			stringBuilder.Append("&oauth_nonce=" + this.UrlEncode(text3));
			stringBuilder.Append("&oauth_signature_method=" + this.UrlEncode("HMAC-SHA1"));
			stringBuilder.Append("&oauth_timestamp=" + this.UrlEncode(text2));
			stringBuilder.Append("&oauth_token=" + this.UrlEncode(this.m_RequestToken));
			stringBuilder.Append("&oauth_verifier=" + this.UrlEncode(verificationCode));
			stringBuilder.Append("&oauth_version=" + this.UrlEncode("1.0"));
			string signatureBase = "GET&" + this.UrlEncode(text) + "&" + this.UrlEncode(stringBuilder.ToString());
			string value = this.ComputeHmacSha1Signature(signatureBase, this.m_ConsumerSecret, this.m_RequestTokenSecret);
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("Authorization: OAuth ");
			stringBuilder2.Append("oauth_version=\"1.0\", ");
			stringBuilder2.Append("oauth_nonce=\"" + text3 + "\", ");
			stringBuilder2.Append("oauth_timestamp=\"" + text2 + "\", ");
			stringBuilder2.Append("oauth_consumer_key=\"" + this.m_ConsumerKey + "\", ");
			stringBuilder2.Append("oauth_verifier=\"" + this.UrlEncode(verificationCode) + "\", ");
			stringBuilder2.Append("oauth_token=\"" + this.UrlEncode(this.m_RequestToken) + "\", ");
			stringBuilder2.Append("oauth_signature_method=\"HMAC-SHA1\", ");
			stringBuilder2.Append("oauth_signature=\"" + this.UrlEncode(value) + "\"");
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers.Add(stringBuilder2.ToString());
			using (WebResponse response = httpWebRequest.GetResponse())
			{
				using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
				{
					foreach (string text4 in HttpUtility.UrlDecode(streamReader.ReadToEnd()).Split(new char[]
					{
						'&'
					}))
					{
						string[] array2 = text4.Split(new char[]
						{
							'='
						});
						bool flag5 = string.Equals(array2[0], "oauth_token", StringComparison.InvariantCultureIgnoreCase);
						if (flag5)
						{
							this.m_AccessToken = array2[1];
						}
						else
						{
							bool flag6 = string.Equals(array2[0], "oauth_token_secret", StringComparison.InvariantCultureIgnoreCase);
							if (flag6)
							{
								this.m_AccessTokenSecret = array2[1];
							}
						}
					}
				}
			}
		}

		// Token: 0x060015DA RID: 5594 RVA: 0x00088544 File Offset: 0x00087544
		public string GetXOAuthStringForSmtp()
		{
			return this.GetXOAuthStringForSmtp((this.m_Email == null) ? this.GetUserEmail() : this.m_Email);
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x00088574 File Offset: 0x00087574
		public string GetXOAuthStringForSmtp(string email)
		{
			bool flag = email == null;
			if (flag)
			{
				throw new ArgumentNullException("email");
			}
			bool flag2 = email == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'email' value must be specified.", "email");
			}
			bool flag3 = string.IsNullOrEmpty(this.m_AccessToken);
			if (flag3)
			{
				throw new InvalidOperationException("Invalid state, you need to call 'GetAccessToken' method first.");
			}
			string value = "https://mail.google.com/mail/b/" + email + "/smtp/";
			string value2 = this.GenerateTimeStamp();
			string value3 = this.GenerateNonce();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("oauth_consumer_key=" + this.UrlEncode(this.m_ConsumerKey));
			stringBuilder.Append("&oauth_nonce=" + this.UrlEncode(value3));
			stringBuilder.Append("&oauth_signature_method=" + this.UrlEncode("HMAC-SHA1"));
			stringBuilder.Append("&oauth_timestamp=" + this.UrlEncode(value2));
			stringBuilder.Append("&oauth_token=" + this.UrlEncode(this.m_AccessToken));
			stringBuilder.Append("&oauth_version=" + this.UrlEncode("1.0"));
			string signatureBase = "GET&" + this.UrlEncode(value) + "&" + this.UrlEncode(stringBuilder.ToString());
			string value4 = this.ComputeHmacSha1Signature(signatureBase, this.m_ConsumerSecret, this.m_AccessTokenSecret);
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("GET ");
			stringBuilder2.Append(value);
			stringBuilder2.Append(" oauth_consumer_key=\"" + this.UrlEncode(this.m_ConsumerKey) + "\"");
			stringBuilder2.Append(",oauth_nonce=\"" + this.UrlEncode(value3) + "\"");
			stringBuilder2.Append(",oauth_signature=\"" + this.UrlEncode(value4) + "\"");
			stringBuilder2.Append(",oauth_signature_method=\"HMAC-SHA1\"");
			stringBuilder2.Append(",oauth_timestamp=\"" + this.UrlEncode(value2) + "\"");
			stringBuilder2.Append(",oauth_token=\"" + this.UrlEncode(this.m_AccessToken) + "\"");
			stringBuilder2.Append(",oauth_version=\"1.0\"");
			return stringBuilder2.ToString();
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x000887C0 File Offset: 0x000877C0
		public string GetXOAuthStringForImap()
		{
			return this.GetXOAuthStringForImap((this.m_Email == null) ? this.GetUserEmail() : this.m_Email);
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x000887F0 File Offset: 0x000877F0
		public string GetXOAuthStringForImap(string email)
		{
			bool flag = email == null;
			if (flag)
			{
				throw new ArgumentNullException("email");
			}
			bool flag2 = email == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'email' value must be specified.", "email");
			}
			bool flag3 = string.IsNullOrEmpty(this.m_AccessToken);
			if (flag3)
			{
				throw new InvalidOperationException("Invalid state, you need to call 'GetAccessToken' method first.");
			}
			string value = "https://mail.google.com/mail/b/" + email + "/imap/";
			string value2 = this.GenerateTimeStamp();
			string value3 = this.GenerateNonce();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("oauth_consumer_key=" + this.UrlEncode(this.m_ConsumerKey));
			stringBuilder.Append("&oauth_nonce=" + this.UrlEncode(value3));
			stringBuilder.Append("&oauth_signature_method=" + this.UrlEncode("HMAC-SHA1"));
			stringBuilder.Append("&oauth_timestamp=" + this.UrlEncode(value2));
			stringBuilder.Append("&oauth_token=" + this.UrlEncode(this.m_AccessToken));
			stringBuilder.Append("&oauth_version=" + this.UrlEncode("1.0"));
			string signatureBase = "GET&" + this.UrlEncode(value) + "&" + this.UrlEncode(stringBuilder.ToString());
			string value4 = this.ComputeHmacSha1Signature(signatureBase, this.m_ConsumerSecret, this.m_AccessTokenSecret);
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("GET ");
			stringBuilder2.Append(value);
			stringBuilder2.Append(" oauth_consumer_key=\"" + this.UrlEncode(this.m_ConsumerKey) + "\"");
			stringBuilder2.Append(",oauth_nonce=\"" + this.UrlEncode(value3) + "\"");
			stringBuilder2.Append(",oauth_signature=\"" + this.UrlEncode(value4) + "\"");
			stringBuilder2.Append(",oauth_signature_method=\"HMAC-SHA1\"");
			stringBuilder2.Append(",oauth_timestamp=\"" + this.UrlEncode(value2) + "\"");
			stringBuilder2.Append(",oauth_token=\"" + this.UrlEncode(this.m_AccessToken) + "\"");
			stringBuilder2.Append(",oauth_version=\"1.0\"");
			return stringBuilder2.ToString();
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x00088A3C File Offset: 0x00087A3C
		public string GetUserEmail()
		{
			bool flag = string.IsNullOrEmpty(this.m_AccessToken);
			if (flag)
			{
				throw new InvalidOperationException("Invalid state, you need to call 'GetAccessToken' method first.");
			}
			string text = "https://www.googleapis.com/userinfo/email";
			string text2 = this.GenerateTimeStamp();
			string text3 = this.GenerateNonce();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("oauth_consumer_key=" + this.UrlEncode(this.m_ConsumerKey));
			stringBuilder.Append("&oauth_nonce=" + this.UrlEncode(text3));
			stringBuilder.Append("&oauth_signature_method=" + this.UrlEncode("HMAC-SHA1"));
			stringBuilder.Append("&oauth_timestamp=" + this.UrlEncode(text2));
			stringBuilder.Append("&oauth_token=" + this.UrlEncode(this.m_AccessToken));
			stringBuilder.Append("&oauth_version=" + this.UrlEncode("1.0"));
			string signatureBase = "GET&" + this.UrlEncode(text) + "&" + this.UrlEncode(stringBuilder.ToString());
			string value = this.ComputeHmacSha1Signature(signatureBase, this.m_ConsumerSecret, this.m_AccessTokenSecret);
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("Authorization: OAuth ");
			stringBuilder2.Append("oauth_version=\"1.0\", ");
			stringBuilder2.Append("oauth_nonce=\"" + text3 + "\", ");
			stringBuilder2.Append("oauth_timestamp=\"" + text2 + "\", ");
			stringBuilder2.Append("oauth_consumer_key=\"" + this.m_ConsumerKey + "\", ");
			stringBuilder2.Append("oauth_token=\"" + this.UrlEncode(this.m_AccessToken) + "\", ");
			stringBuilder2.Append("oauth_signature_method=\"HMAC-SHA1\", ");
			stringBuilder2.Append("oauth_signature=\"" + this.UrlEncode(value) + "\"");
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers.Add(stringBuilder2.ToString());
			using (WebResponse response = httpWebRequest.GetResponse())
			{
				using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
				{
					foreach (string text4 in HttpUtility.UrlDecode(streamReader.ReadToEnd()).Split(new char[]
					{
						'&'
					}))
					{
						string[] array2 = text4.Split(new char[]
						{
							'='
						});
						bool flag2 = string.Equals(array2[0], "email", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							this.m_Email = array2[1];
						}
					}
				}
			}
			return this.m_Email;
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x00088D1C File Offset: 0x00087D1C
		private string UrlEncode(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in value)
			{
				bool flag2 = text.IndexOf(c) != -1;
				if (flag2)
				{
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.Append("%" + string.Format("{0:X2}", (int)c));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x00088DC0 File Offset: 0x00087DC0
		private string ComputeHmacSha1Signature(string signatureBase, string consumerSecret, string tokenSecret)
		{
			bool flag = signatureBase == null;
			if (flag)
			{
				throw new ArgumentNullException("signatureBase");
			}
			bool flag2 = consumerSecret == null;
			if (flag2)
			{
				throw new ArgumentNullException("consumerSecret");
			}
			return Convert.ToBase64String(new HMACSHA1
			{
				Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", this.UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : this.UrlEncode(tokenSecret)))
			}.ComputeHash(Encoding.ASCII.GetBytes(signatureBase)));
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x00088E50 File Offset: 0x00087E50
		private string GenerateTimeStamp()
		{
			return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
		}

		// Token: 0x060015E2 RID: 5602 RVA: 0x00088E94 File Offset: 0x00087E94
		private string GenerateNonce()
		{
			return this.m_pRandom.Next(123400, 9999999).ToString();
		}

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x060015E3 RID: 5603 RVA: 0x00088EC4 File Offset: 0x00087EC4
		public string Email
		{
			get
			{
				return this.m_Email;
			}
		}

		// Token: 0x040008AA RID: 2218
		private string m_ConsumerKey = null;

		// Token: 0x040008AB RID: 2219
		private string m_ConsumerSecret = null;

		// Token: 0x040008AC RID: 2220
		private string m_Scope = "https://mail.google.com/ https://www.googleapis.com/auth/userinfo.email";

		// Token: 0x040008AD RID: 2221
		private string m_RequestToken = null;

		// Token: 0x040008AE RID: 2222
		private string m_RequestTokenSecret = null;

		// Token: 0x040008AF RID: 2223
		private string m_AccessToken = null;

		// Token: 0x040008B0 RID: 2224
		private string m_AccessTokenSecret = null;

		// Token: 0x040008B1 RID: 2225
		private string m_Email = null;

		// Token: 0x040008B2 RID: 2226
		private Random m_pRandom = null;
	}
}
