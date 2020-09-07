using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000FD RID: 253
	public class MIME_b_MultipartFormData : MIME_b_Multipart
	{
		// Token: 0x060009FF RID: 2559 RVA: 0x0003D0C8 File Offset: 0x0003C0C8
		public MIME_b_MultipartFormData(MIME_h_ContentType contentType) : base(contentType)
		{
			bool flag = !string.Equals(contentType.TypeWithSubtype, "multipart/form-data", StringComparison.CurrentCultureIgnoreCase);
			if (flag)
			{
				throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/form-data'.");
			}
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x0003D104 File Offset: 0x0003C104
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
			MIME_b_MultipartFormData mime_b_MultipartFormData = new MIME_b_MultipartFormData(owner.ContentType);
			MIME_b_Multipart.ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, mime_b_MultipartFormData);
			return mime_b_MultipartFormData;
		}
	}
}
