using System;
using System.Collections.Generic;
using System.Reflection;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000103 RID: 259
	public class MIME_b_Provider
	{
		// Token: 0x06000A11 RID: 2577 RVA: 0x0003D908 File Offset: 0x0003C908
		public MIME_b_Provider()
		{
			this.m_pBodyTypes = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);
			this.m_pBodyTypes.Add("application/pkcs7-mime", typeof(MIME_b_ApplicationPkcs7Mime));
			this.m_pBodyTypes.Add("message/rfc822", typeof(MIME_b_MessageRfc822));
			this.m_pBodyTypes.Add("message/delivery-status", typeof(MIME_b_MessageDeliveryStatus));
			this.m_pBodyTypes.Add("multipart/alternative", typeof(MIME_b_MultipartAlternative));
			this.m_pBodyTypes.Add("multipart/digest", typeof(MIME_b_MultipartDigest));
			this.m_pBodyTypes.Add("multipart/encrypted", typeof(MIME_b_MultipartEncrypted));
			this.m_pBodyTypes.Add("multipart/form-data", typeof(MIME_b_MultipartFormData));
			this.m_pBodyTypes.Add("multipart/mixed", typeof(MIME_b_MultipartMixed));
			this.m_pBodyTypes.Add("multipart/parallel", typeof(MIME_b_MultipartParallel));
			this.m_pBodyTypes.Add("multipart/related", typeof(MIME_b_MultipartRelated));
			this.m_pBodyTypes.Add("multipart/report", typeof(MIME_b_MultipartReport));
			this.m_pBodyTypes.Add("multipart/signed", typeof(MIME_b_MultipartSigned));
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x0003DA78 File Offset: 0x0003CA78
		public MIME_b Parse(MIME_Entity owner, SmartStream stream, MIME_h_ContentType defaultContentType)
		{
			bool flag = owner == null;
			if (flag)
			{
				throw new ArgumentNullException("owner");
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag3 = defaultContentType == null;
			if (flag3)
			{
				throw new ArgumentNullException("defaultContentType");
			}
			string text = defaultContentType.TypeWithSubtype;
			try
			{
				bool flag4 = owner.ContentType != null;
				if (flag4)
				{
					text = owner.ContentType.TypeWithSubtype;
				}
			}
			catch
			{
				text = "unknown/unknown";
			}
			bool flag5 = this.m_pBodyTypes.ContainsKey(text);
			Type type;
			if (flag5)
			{
				type = this.m_pBodyTypes[text];
			}
			else
			{
				string a = text.Split(new char[]
				{
					'/'
				})[0].ToLowerInvariant();
				bool flag6 = a == "application";
				if (flag6)
				{
					type = typeof(MIME_b_Application);
				}
				else
				{
					bool flag7 = a == "audio";
					if (flag7)
					{
						type = typeof(MIME_b_Audio);
					}
					else
					{
						bool flag8 = a == "image";
						if (flag8)
						{
							type = typeof(MIME_b_Image);
						}
						else
						{
							bool flag9 = a == "message";
							if (flag9)
							{
								type = typeof(MIME_b_Message);
							}
							else
							{
								bool flag10 = a == "multipart";
								if (flag10)
								{
									type = typeof(MIME_b_Multipart);
								}
								else
								{
									bool flag11 = a == "text";
									if (flag11)
									{
										type = typeof(MIME_b_Text);
									}
									else
									{
										bool flag12 = a == "video";
										if (flag12)
										{
											type = typeof(MIME_b_Video);
										}
										else
										{
											type = typeof(MIME_b_Unknown);
										}
									}
								}
							}
						}
					}
				}
			}
			return (MIME_b)type.GetMethod("Parse", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Invoke(null, new object[]
			{
				owner,
				defaultContentType,
				stream
			});
		}

		// Token: 0x04000458 RID: 1112
		private Dictionary<string, Type> m_pBodyTypes = null;
	}
}
