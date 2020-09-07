using System;
using System.Collections.Generic;
using System.Text;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.MIME;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP
{
	// Token: 0x02000042 RID: 66
	public class SIP_Utils
	{
		// Token: 0x06000234 RID: 564 RVA: 0x0000DAB4 File Offset: 0x0000CAB4
		public static string ParseAddress(string to)
		{
			string result;
			try
			{
				string text = to;
				bool flag = to.IndexOf('<') > -1 && to.IndexOf('<') < to.IndexOf('>');
				if (flag)
				{
					text = to.Substring(to.IndexOf('<') + 1, to.IndexOf('>') - to.IndexOf('<') - 1);
				}
				bool flag2 = text.IndexOf(':') > -1;
				if (flag2)
				{
					text = text.Substring(text.IndexOf(':') + 1).Split(new char[]
					{
						':'
					})[0];
				}
				result = text;
			}
			catch
			{
				throw new ArgumentException("Invalid SIP header To: '" + to + "' value !");
			}
			return result;
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000DB70 File Offset: 0x0000CB70
		public static AbsoluteUri UriToRequestUri(AbsoluteUri uri)
		{
			bool flag = uri == null;
			if (flag)
			{
				throw new ArgumentNullException("uri");
			}
			bool flag2 = uri is SIP_Uri;
			AbsoluteUri result;
			if (flag2)
			{
				SIP_Uri sip_Uri = (SIP_Uri)uri;
				sip_Uri.Parameters.Remove("method");
				sip_Uri.Header = null;
				result = sip_Uri;
			}
			else
			{
				result = uri;
			}
			return result;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000DBCC File Offset: 0x0000CBCC
		public static bool IsSipOrSipsUri(string value)
		{
			try
			{
				SIP_Uri.Parse(value);
				return true;
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000DC00 File Offset: 0x0000CC00
		public static bool IsTelUri(string uri)
		{
			uri = uri.ToLower();
			try
			{
				bool flag = uri.StartsWith("tel:");
				if (flag)
				{
					return true;
				}
				bool flag2 = SIP_Utils.IsSipOrSipsUri(uri);
				if (flag2)
				{
					SIP_Uri sip_Uri = SIP_Uri.Parse(uri);
					bool flag3 = sip_Uri.User.StartsWith("+");
					if (flag3)
					{
						return true;
					}
					bool flag4 = sip_Uri.Param_User != null && sip_Uri.Param_User.ToLower() == "phone";
					if (flag4)
					{
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000DCA4 File Offset: 0x0000CCA4
		public static SIP_t_Credentials GetCredentials(SIP_Request request, string realm)
		{
			foreach (SIP_SingleValueHF<SIP_t_Credentials> sip_SingleValueHF in request.ProxyAuthorization.HeaderFields)
			{
				bool flag = sip_SingleValueHF.ValueX.Method.ToLower() == "digest";
				if (flag)
				{
					Auth_HttpDigest auth_HttpDigest = new Auth_HttpDigest(sip_SingleValueHF.ValueX.AuthData, request.RequestLine.Method);
					bool flag2 = auth_HttpDigest.Realm.ToLower() == realm.ToLower();
					if (flag2)
					{
						return sip_SingleValueHF.ValueX;
					}
				}
			}
			return null;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000DD40 File Offset: 0x0000CD40
		public static bool ContainsOptionTag(SIP_t_OptionTag[] tags, string tag)
		{
			foreach (SIP_t_OptionTag sip_t_OptionTag in tags)
			{
				bool flag = sip_t_OptionTag.OptionTag.ToLower() == tag;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000DD88 File Offset: 0x0000CD88
		public static bool MethodCanEstablishDialog(string method)
		{
			bool flag = string.IsNullOrEmpty(method);
			if (flag)
			{
				throw new ArgumentException("Argument 'method' value can't be null or empty !");
			}
			method = method.ToUpper();
			bool flag2 = method == "INVITE";
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = method == "SUBSCRIBE";
				if (flag3)
				{
					result = true;
				}
				else
				{
					bool flag4 = method == "REFER";
					result = flag4;
				}
			}
			return result;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000DDF8 File Offset: 0x0000CDF8
		public static string CreateTag()
		{
			return Guid.NewGuid().ToString().Replace("-", "").Substring(8);
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000DE34 File Offset: 0x0000CE34
		public static bool IsReliableTransport(string transport)
		{
			bool flag = transport == null;
			if (flag)
			{
				throw new ArgumentNullException("transport");
			}
			bool flag2 = transport.ToUpper() == "TCP";
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = transport.ToUpper() == "TLS";
				result = flag3;
			}
			return result;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000DE90 File Offset: 0x0000CE90
		public static bool IsToken(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			return MIME_Reader.IsToken(value);
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000DEBC File Offset: 0x0000CEBC
		public static string ListToString(List<string> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Count; i++)
			{
				bool flag2 = i == 0;
				if (flag2)
				{
					stringBuilder.Append(list[i]);
				}
				else
				{
					stringBuilder.Append("," + list[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
