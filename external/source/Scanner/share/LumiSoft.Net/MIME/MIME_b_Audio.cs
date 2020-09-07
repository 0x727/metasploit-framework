using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000F4 RID: 244
	public class MIME_b_Audio : MIME_b_SinglepartBase
	{
		// Token: 0x060009D8 RID: 2520 RVA: 0x0003BF7A File Offset: 0x0003AF7A
		public MIME_b_Audio(string mediaType) : base(new MIME_h_ContentType(mediaType))
		{
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x0003C290 File Offset: 0x0003B290
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
			MIME_b_Audio mime_b_Audio;
			if (flag4)
			{
				mime_b_Audio = new MIME_b_Audio(owner.ContentType.TypeWithSubtype);
			}
			else
			{
				mime_b_Audio = new MIME_b_Audio(defaultContentType.TypeWithSubtype);
			}
			Net_Utils.StreamCopy(stream, mime_b_Audio.EncodedStream, stream.LineBufferSize);
			return mime_b_Audio;
		}
	}
}
