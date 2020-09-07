using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000F6 RID: 246
	public class MIME_b_Message : MIME_b_SinglepartBase
	{
		// Token: 0x060009DC RID: 2524 RVA: 0x0003BF7A File Offset: 0x0003AF7A
		public MIME_b_Message(string mediaType) : base(new MIME_h_ContentType(mediaType))
		{
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x0003C3C0 File Offset: 0x0003B3C0
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
			MIME_b_Message mime_b_Message;
			if (flag4)
			{
				mime_b_Message = new MIME_b_Message(owner.ContentType.TypeWithSubtype);
			}
			else
			{
				mime_b_Message = new MIME_b_Message(defaultContentType.TypeWithSubtype);
			}
			Net_Utils.StreamCopy(stream, mime_b_Message.EncodedStream, stream.LineBufferSize);
			return mime_b_Message;
		}
	}
}
