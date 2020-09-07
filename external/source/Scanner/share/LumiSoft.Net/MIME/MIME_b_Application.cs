using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000F2 RID: 242
	public class MIME_b_Application : MIME_b_SinglepartBase
	{
		// Token: 0x060009D0 RID: 2512 RVA: 0x0003BF7A File Offset: 0x0003AF7A
		public MIME_b_Application(string mediaType) : base(new MIME_h_ContentType(mediaType))
		{
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x0003BF8C File Offset: 0x0003AF8C
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
			MIME_b_Application mime_b_Application;
			if (flag4)
			{
				mime_b_Application = new MIME_b_Application(owner.ContentType.TypeWithSubtype);
			}
			else
			{
				mime_b_Application = new MIME_b_Application(defaultContentType.TypeWithSubtype);
			}
			Net_Utils.StreamCopy(stream, mime_b_Application.EncodedStream, stream.LineBufferSize);
			return mime_b_Application;
		}
	}
}
