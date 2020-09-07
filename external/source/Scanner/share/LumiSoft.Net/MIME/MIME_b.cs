using System;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000F1 RID: 241
	public abstract class MIME_b
	{
		// Token: 0x060009C6 RID: 2502 RVA: 0x0003BE20 File Offset: 0x0003AE20
		public MIME_b(MIME_h_ContentType contentType)
		{
			bool flag = contentType == null;
			if (flag)
			{
				throw new ArgumentNullException("contentType");
			}
			this.m_pContentType = contentType;
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x0003BE5E File Offset: 0x0003AE5E
		internal MIME_b()
		{
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x0003BE78 File Offset: 0x0003AE78
		protected static MIME_b Parse(MIME_Entity owner, MIME_h_ContentType defaultContentType, SmartStream stream)
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
			throw new NotImplementedException("Body provider class does not implement required Parse method.");
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x0003BECC File Offset: 0x0003AECC
		internal virtual void SetParent(MIME_Entity entity, bool setContentType)
		{
			this.m_pEntity = entity;
			bool flag = setContentType && (entity.ContentType == null || !string.Equals(entity.ContentType.TypeWithSubtype, this.MediaType, StringComparison.InvariantCultureIgnoreCase));
			if (flag)
			{
				entity.ContentType = this.m_pContentType;
			}
		}

		// Token: 0x060009CA RID: 2506
		protected internal abstract void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode);

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x060009CB RID: 2507
		public abstract bool IsModified { get; }

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x060009CC RID: 2508 RVA: 0x0003BF20 File Offset: 0x0003AF20
		public MIME_Entity Entity
		{
			get
			{
				return this.m_pEntity;
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x060009CD RID: 2509 RVA: 0x0003BF38 File Offset: 0x0003AF38
		public string MediaType
		{
			get
			{
				return this.m_pContentType.TypeWithSubtype;
			}
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x060009CE RID: 2510 RVA: 0x0003BF58 File Offset: 0x0003AF58
		// (set) Token: 0x060009CF RID: 2511 RVA: 0x0003BF70 File Offset: 0x0003AF70
		internal MIME_h_ContentType ContentType
		{
			get
			{
				return this.m_pContentType;
			}
			set
			{
				this.m_pContentType = value;
			}
		}

		// Token: 0x0400044F RID: 1103
		private MIME_Entity m_pEntity = null;

		// Token: 0x04000450 RID: 1104
		private MIME_h_ContentType m_pContentType = null;
	}
}
