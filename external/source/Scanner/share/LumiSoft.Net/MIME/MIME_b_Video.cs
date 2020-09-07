using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000107 RID: 263
	public class MIME_b_Video : MIME_b_SinglepartBase
	{
		// Token: 0x06000A2B RID: 2603 RVA: 0x0003BF7A File Offset: 0x0003AF7A
		public MIME_b_Video(string mediaType) : base(new MIME_h_ContentType(mediaType))
		{
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x0003E6C0 File Offset: 0x0003D6C0
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
			MIME_b_Video mime_b_Video;
			if (flag4)
			{
				mime_b_Video = new MIME_b_Video(owner.ContentType.TypeWithSubtype);
			}
			else
			{
				mime_b_Video = new MIME_b_Video(defaultContentType.TypeWithSubtype);
			}
			Net_Utils.StreamCopy(stream, mime_b_Video.EncodedStream, stream.LineBufferSize);
			return mime_b_Video;
		}
	}
}
