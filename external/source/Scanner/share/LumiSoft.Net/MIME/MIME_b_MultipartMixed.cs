using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000FE RID: 254
	public class MIME_b_MultipartMixed : MIME_b_Multipart
	{
		// Token: 0x06000A01 RID: 2561 RVA: 0x0003D1A0 File Offset: 0x0003C1A0
		public MIME_b_MultipartMixed()
		{
			base.ContentType = new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed)
			{
				Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.')
			};
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x0003D1EC File Offset: 0x0003C1EC
		public MIME_b_MultipartMixed(MIME_h_ContentType contentType) : base(contentType)
		{
			bool flag = !string.Equals(contentType.TypeWithSubtype, "multipart/mixed", StringComparison.CurrentCultureIgnoreCase);
			if (flag)
			{
				throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/mixed'.");
			}
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x0003D228 File Offset: 0x0003C228
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
			MIME_b_MultipartMixed mime_b_MultipartMixed = new MIME_b_MultipartMixed(owner.ContentType);
			MIME_b_Multipart.ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, mime_b_MultipartMixed);
			return mime_b_MultipartMixed;
		}
	}
}
