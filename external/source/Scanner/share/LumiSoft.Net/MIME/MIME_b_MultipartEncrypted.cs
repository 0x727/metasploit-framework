using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000FC RID: 252
	public class MIME_b_MultipartEncrypted : MIME_b_Multipart
	{
		// Token: 0x060009FD RID: 2557 RVA: 0x0003CFF0 File Offset: 0x0003BFF0
		public MIME_b_MultipartEncrypted(MIME_h_ContentType contentType) : base(contentType)
		{
			bool flag = !string.Equals(contentType.TypeWithSubtype, "multipart/encrypted", StringComparison.CurrentCultureIgnoreCase);
			if (flag)
			{
				throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/encrypted'.");
			}
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x0003D02C File Offset: 0x0003C02C
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
			MIME_b_MultipartEncrypted mime_b_MultipartEncrypted = new MIME_b_MultipartEncrypted(owner.ContentType);
			MIME_b_Multipart.ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, mime_b_MultipartEncrypted);
			return mime_b_MultipartEncrypted;
		}
	}
}
