using System;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000105 RID: 261
	public class MIME_b_Text : MIME_b_SinglepartBase
	{
		// Token: 0x06000A24 RID: 2596 RVA: 0x0003BF7A File Offset: 0x0003AF7A
		public MIME_b_Text(string mediaType) : base(new MIME_h_ContentType(mediaType))
		{
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x0003E3A0 File Offset: 0x0003D3A0
		protected new static MIME_b Parse(MIME_Entity owner, MIME_h_ContentType defaultContentType, SmartStream stream)
		{
			bool flag = owner == null;
			if (flag)
			{
				throw new ArgumentNullException("owner");
			}
			bool flag2 = defaultContentType == null;
			if (flag2)
			{
				throw new ArgumentNullException("defaultContentType");
			}
			bool flag3 = stream == null;
			if (flag3)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag4 = owner.ContentType != null;
			MIME_b_Text mime_b_Text;
			if (flag4)
			{
				mime_b_Text = new MIME_b_Text(owner.ContentType.TypeWithSubtype);
			}
			else
			{
				mime_b_Text = new MIME_b_Text(defaultContentType.TypeWithSubtype);
			}
			Net_Utils.StreamCopy(stream, mime_b_Text.EncodedStream, stream.LineBufferSize);
			mime_b_Text.SetModified(false);
			return mime_b_Text;
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x0003E440 File Offset: 0x0003D440
		public void SetText(string transferEncoding, Encoding charset, string text)
		{
			bool flag = transferEncoding == null;
			if (flag)
			{
				throw new ArgumentNullException("transferEncoding");
			}
			bool flag2 = charset == null;
			if (flag2)
			{
				throw new ArgumentNullException("charset");
			}
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ArgumentNullException("text");
			}
			bool flag4 = base.Entity == null;
			if (flag4)
			{
				throw new InvalidOperationException("Body must be bounded to some entity first.");
			}
			base.SetData(new MemoryStream(charset.GetBytes(text)), transferEncoding);
			base.Entity.ContentType.Param_Charset = charset.WebName;
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x0003E4D0 File Offset: 0x0003D4D0
		public Encoding GetCharset()
		{
			bool flag = base.Entity.ContentType == null || string.IsNullOrEmpty(base.Entity.ContentType.Param_Charset);
			Encoding result;
			if (flag)
			{
				result = Encoding.ASCII;
			}
			else
			{
				bool flag2 = base.Entity.ContentType.Param_Charset.ToLower().StartsWith("x-");
				if (flag2)
				{
					result = Encoding.GetEncoding(base.Entity.ContentType.Param_Charset.Substring(2));
				}
				else
				{
					bool flag3 = string.Equals(base.Entity.ContentType.Param_Charset, "cp1252", StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						result = Encoding.GetEncoding("windows-1252");
					}
					else
					{
						bool flag4 = string.Equals(base.Entity.ContentType.Param_Charset, "utf8", StringComparison.InvariantCultureIgnoreCase);
						if (flag4)
						{
							result = Encoding.GetEncoding("utf-8");
						}
						else
						{
							bool flag5 = string.Equals(base.Entity.ContentType.Param_Charset, "iso8859_1", StringComparison.InvariantCultureIgnoreCase);
							if (flag5)
							{
								result = Encoding.GetEncoding("iso-8859-1");
							}
							else
							{
								result = Encoding.GetEncoding(base.Entity.ContentType.Param_Charset);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06000A28 RID: 2600 RVA: 0x0003E600 File Offset: 0x0003D600
		public string Text
		{
			get
			{
				return this.GetCharset().GetString(base.Data);
			}
		}
	}
}
