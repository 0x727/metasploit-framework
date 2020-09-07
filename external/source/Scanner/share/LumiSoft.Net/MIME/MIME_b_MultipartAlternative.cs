using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000FA RID: 250
	public class MIME_b_MultipartAlternative : MIME_b_Multipart
	{
		// Token: 0x060009F7 RID: 2551 RVA: 0x0003CDD4 File Offset: 0x0003BDD4
		public MIME_b_MultipartAlternative()
		{
			base.ContentType = new MIME_h_ContentType(MIME_MediaTypes.Multipart.alternative)
			{
				Param_Boundary = Guid.NewGuid().ToString().Replace('-', '.')
			};
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x0003CE20 File Offset: 0x0003BE20
		public MIME_b_MultipartAlternative(MIME_h_ContentType contentType) : base(contentType)
		{
			bool flag = !string.Equals(contentType.TypeWithSubtype, "multipart/alternative", StringComparison.CurrentCultureIgnoreCase);
			if (flag)
			{
				throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/alternative'.");
			}
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x0003CE5C File Offset: 0x0003BE5C
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
			MIME_b_MultipartAlternative mime_b_MultipartAlternative = new MIME_b_MultipartAlternative(owner.ContentType);
			MIME_b_Multipart.ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, mime_b_MultipartAlternative);
			return mime_b_MultipartAlternative;
		}
	}
}
