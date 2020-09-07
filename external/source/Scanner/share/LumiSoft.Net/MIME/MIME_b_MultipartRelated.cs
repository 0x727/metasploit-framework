using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000100 RID: 256
	public class MIME_b_MultipartRelated : MIME_b_Multipart
	{
		// Token: 0x06000A06 RID: 2566 RVA: 0x0003D39C File Offset: 0x0003C39C
		public MIME_b_MultipartRelated(MIME_h_ContentType contentType) : base(contentType)
		{
			bool flag = !string.Equals(contentType.TypeWithSubtype, "multipart/related", StringComparison.CurrentCultureIgnoreCase);
			if (flag)
			{
				throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/related'.");
			}
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x0003D3D8 File Offset: 0x0003C3D8
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
			bool flag4 = owner.ContentType == null || owner.ContentType.Param_Boundary == null;
			if (flag4)
			{
				throw new ParseException("Multipart entity has not required 'boundary' paramter.");
			}
			MIME_b_MultipartRelated mime_b_MultipartRelated = new MIME_b_MultipartRelated(owner.ContentType);
			MIME_b_Multipart.ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, mime_b_MultipartRelated);
			return mime_b_MultipartRelated;
		}
	}
}
