using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000106 RID: 262
	public class MIME_b_Unknown : MIME_b_SinglepartBase
	{
		// Token: 0x06000A29 RID: 2601 RVA: 0x0003BF7A File Offset: 0x0003AF7A
		public MIME_b_Unknown(string mediaType) : base(new MIME_h_ContentType(mediaType))
		{
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x0003E624 File Offset: 0x0003D624
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
			string mediaType = null;
			try
			{
				mediaType = owner.ContentType.TypeWithSubtype;
			}
			catch
			{
				mediaType = "unparsable/unparsable";
			}
			MIME_b_Unknown mime_b_Unknown = new MIME_b_Unknown(mediaType);
			Net_Utils.StreamCopy(stream, mime_b_Unknown.EncodedStream, stream.LineBufferSize);
			return mime_b_Unknown;
		}
	}
}
