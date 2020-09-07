using System;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000FB RID: 251
	public class MIME_b_MultipartDigest : MIME_b_Multipart
	{
		// Token: 0x060009FA RID: 2554 RVA: 0x0003CEF8 File Offset: 0x0003BEF8
		public MIME_b_MultipartDigest(MIME_h_ContentType contentType) : base(contentType)
		{
			bool flag = !string.Equals(contentType.TypeWithSubtype, "multipart/digest", StringComparison.CurrentCultureIgnoreCase);
			if (flag)
			{
				throw new ArgumentException("Argument 'contentType.TypeWithSubype' value must be 'multipart/digest'.");
			}
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x0003CF34 File Offset: 0x0003BF34
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
			MIME_b_MultipartDigest mime_b_MultipartDigest = new MIME_b_MultipartDigest(owner.ContentType);
			MIME_b_Multipart.ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, mime_b_MultipartDigest);
			return mime_b_MultipartDigest;
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060009FC RID: 2556 RVA: 0x0003CFD0 File Offset: 0x0003BFD0
		public override MIME_h_ContentType DefaultBodyPartContentType
		{
			get
			{
				return new MIME_h_ContentType("message/rfc822");
			}
		}
	}
}
