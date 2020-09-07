using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000F5 RID: 245
	public class MIME_b_Image : MIME_b_SinglepartBase
	{
		// Token: 0x060009DA RID: 2522 RVA: 0x0003BF7A File Offset: 0x0003AF7A
		public MIME_b_Image(string mediaType) : base(new MIME_h_ContentType(mediaType))
		{
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x0003C328 File Offset: 0x0003B328
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
			MIME_b_Image mime_b_Image;
			if (flag4)
			{
				mime_b_Image = new MIME_b_Image(owner.ContentType.TypeWithSubtype);
			}
			else
			{
				mime_b_Image = new MIME_b_Image(defaultContentType.TypeWithSubtype);
			}
			Net_Utils.StreamCopy(stream, mime_b_Image.EncodedStream, stream.LineBufferSize);
			return mime_b_Image;
		}
	}
}
