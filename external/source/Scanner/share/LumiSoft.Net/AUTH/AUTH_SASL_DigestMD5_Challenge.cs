using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000269 RID: 617
	public class AUTH_SASL_DigestMD5_Challenge
	{
		// Token: 0x06001611 RID: 5649 RVA: 0x00089AAC File Offset: 0x00088AAC
		public AUTH_SASL_DigestMD5_Challenge(string[] realm, string nonce, string[] qopOptions, bool stale)
		{
			bool flag = realm == null;
			if (flag)
			{
				throw new ArgumentNullException("realm");
			}
			bool flag2 = nonce == null;
			if (flag2)
			{
				throw new ArgumentNullException("nonce");
			}
			bool flag3 = qopOptions == null;
			if (flag3)
			{
				throw new ArgumentNullException("qopOptions");
			}
			this.m_Realm = realm;
			this.m_Nonce = nonce;
			this.m_QopOptions = qopOptions;
			this.m_Stale = stale;
			this.m_Charset = "utf-8";
			this.m_Algorithm = "md5-sess";
		}

		// Token: 0x06001612 RID: 5650 RVA: 0x00089B68 File Offset: 0x00088B68
		private AUTH_SASL_DigestMD5_Challenge()
		{
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x00089BB8 File Offset: 0x00088BB8
		public static AUTH_SASL_DigestMD5_Challenge Parse(string challenge)
		{
			bool flag = challenge == null;
			if (flag)
			{
				throw new ArgumentNullException("challenge");
			}
			AUTH_SASL_DigestMD5_Challenge auth_SASL_DigestMD5_Challenge = new AUTH_SASL_DigestMD5_Challenge();
			string[] array = TextUtils.SplitQuotedString(challenge, ',');
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					'='
				}, 2);
				string text2 = array3[0].Trim();
				bool flag2 = array3.Length == 2;
				if (flag2)
				{
					bool flag3 = text2.ToLower() == "realm";
					if (flag3)
					{
						auth_SASL_DigestMD5_Challenge.m_Realm = TextUtils.UnQuoteString(array3[1]).Split(new char[]
						{
							','
						});
					}
					else
					{
						bool flag4 = text2.ToLower() == "nonce";
						if (flag4)
						{
							auth_SASL_DigestMD5_Challenge.m_Nonce = TextUtils.UnQuoteString(array3[1]);
						}
						else
						{
							bool flag5 = text2.ToLower() == "qop";
							if (flag5)
							{
								auth_SASL_DigestMD5_Challenge.m_QopOptions = TextUtils.UnQuoteString(array3[1]).Split(new char[]
								{
									','
								});
							}
							else
							{
								bool flag6 = text2.ToLower() == "stale";
								if (flag6)
								{
									auth_SASL_DigestMD5_Challenge.m_Stale = Convert.ToBoolean(TextUtils.UnQuoteString(array3[1]));
								}
								else
								{
									bool flag7 = text2.ToLower() == "maxbuf";
									if (flag7)
									{
										auth_SASL_DigestMD5_Challenge.m_Maxbuf = Convert.ToInt32(TextUtils.UnQuoteString(array3[1]));
									}
									else
									{
										bool flag8 = text2.ToLower() == "charset";
										if (flag8)
										{
											auth_SASL_DigestMD5_Challenge.m_Charset = TextUtils.UnQuoteString(array3[1]);
										}
										else
										{
											bool flag9 = text2.ToLower() == "algorithm";
											if (flag9)
											{
												auth_SASL_DigestMD5_Challenge.m_Algorithm = TextUtils.UnQuoteString(array3[1]);
											}
											else
											{
												bool flag10 = text2.ToLower() == "cipher-opts";
												if (flag10)
												{
													auth_SASL_DigestMD5_Challenge.m_CipherOpts = TextUtils.UnQuoteString(array3[1]);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool flag11 = string.IsNullOrEmpty(auth_SASL_DigestMD5_Challenge.Nonce);
			if (flag11)
			{
				throw new ParseException("The challenge-string doesn't contain required parameter 'nonce' value.");
			}
			bool flag12 = string.IsNullOrEmpty(auth_SASL_DigestMD5_Challenge.Algorithm);
			if (flag12)
			{
				throw new ParseException("The challenge-string doesn't contain required parameter 'algorithm' value.");
			}
			return auth_SASL_DigestMD5_Challenge;
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x00089E04 File Offset: 0x00088E04
		public string ToChallenge()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("realm=\"" + Net_Utils.ArrayToString(this.Realm, ",") + "\"");
			stringBuilder.Append(",nonce=\"" + this.Nonce + "\"");
			bool flag = this.QopOptions != null;
			if (flag)
			{
				stringBuilder.Append(",qop=\"" + Net_Utils.ArrayToString(this.QopOptions, ",") + "\"");
			}
			bool stale = this.Stale;
			if (stale)
			{
				stringBuilder.Append(",stale=true");
			}
			bool flag2 = this.Maxbuf > 0;
			if (flag2)
			{
				stringBuilder.Append(",maxbuf=" + this.Maxbuf);
			}
			bool flag3 = !string.IsNullOrEmpty(this.Charset);
			if (flag3)
			{
				stringBuilder.Append(",charset=" + this.Charset);
			}
			stringBuilder.Append(",algorithm=" + this.Algorithm);
			bool flag4 = !string.IsNullOrEmpty(this.CipherOpts);
			if (flag4)
			{
				stringBuilder.Append(",cipher-opts=\"" + this.CipherOpts + "\"");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000734 RID: 1844
		// (get) Token: 0x06001615 RID: 5653 RVA: 0x00089F54 File Offset: 0x00088F54
		public string[] Realm
		{
			get
			{
				return this.m_Realm;
			}
		}

		// Token: 0x17000735 RID: 1845
		// (get) Token: 0x06001616 RID: 5654 RVA: 0x00089F6C File Offset: 0x00088F6C
		public string Nonce
		{
			get
			{
				return this.m_Nonce;
			}
		}

		// Token: 0x17000736 RID: 1846
		// (get) Token: 0x06001617 RID: 5655 RVA: 0x00089F84 File Offset: 0x00088F84
		public string[] QopOptions
		{
			get
			{
				return this.m_QopOptions;
			}
		}

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x06001618 RID: 5656 RVA: 0x00089F9C File Offset: 0x00088F9C
		public bool Stale
		{
			get
			{
				return this.m_Stale;
			}
		}

		// Token: 0x17000738 RID: 1848
		// (get) Token: 0x06001619 RID: 5657 RVA: 0x00089FB4 File Offset: 0x00088FB4
		public int Maxbuf
		{
			get
			{
				return this.m_Maxbuf;
			}
		}

		// Token: 0x17000739 RID: 1849
		// (get) Token: 0x0600161A RID: 5658 RVA: 0x00089FCC File Offset: 0x00088FCC
		public string Charset
		{
			get
			{
				return this.m_Charset;
			}
		}

		// Token: 0x1700073A RID: 1850
		// (get) Token: 0x0600161B RID: 5659 RVA: 0x00089FE4 File Offset: 0x00088FE4
		public string Algorithm
		{
			get
			{
				return this.m_Algorithm;
			}
		}

		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x0600161C RID: 5660 RVA: 0x00089FFC File Offset: 0x00088FFC
		public string CipherOpts
		{
			get
			{
				return this.m_CipherOpts;
			}
		}

		// Token: 0x040008D3 RID: 2259
		private string[] m_Realm = null;

		// Token: 0x040008D4 RID: 2260
		private string m_Nonce = null;

		// Token: 0x040008D5 RID: 2261
		private string[] m_QopOptions = null;

		// Token: 0x040008D6 RID: 2262
		private bool m_Stale = false;

		// Token: 0x040008D7 RID: 2263
		private int m_Maxbuf = 0;

		// Token: 0x040008D8 RID: 2264
		private string m_Charset = null;

		// Token: 0x040008D9 RID: 2265
		private string m_Algorithm = null;

		// Token: 0x040008DA RID: 2266
		private string m_CipherOpts = null;
	}
}
