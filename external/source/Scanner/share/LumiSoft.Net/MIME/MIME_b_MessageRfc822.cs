using System;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;
using LumiSoft.Net.Mail;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000F8 RID: 248
	public class MIME_b_MessageRfc822 : MIME_b
	{
		// Token: 0x060009E4 RID: 2532 RVA: 0x0003C6C8 File Offset: 0x0003B6C8
		public MIME_b_MessageRfc822() : base(new MIME_h_ContentType("message/rfc822"))
		{
			this.m_pMessage = new Mail_Message();
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0003C6F0 File Offset: 0x0003B6F0
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
			return new MIME_b_MessageRfc822
			{
				m_pMessage = Mail_Message.ParseFromStream(stream)
			};
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x0003C754 File Offset: 0x0003B754
		protected internal override void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pMessage.ToStream(stream, headerWordEncoder, headerParmetersCharset, headerReencode);
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x060009E7 RID: 2535 RVA: 0x0003C788 File Offset: 0x0003B788
		public override bool IsModified
		{
			get
			{
				return this.m_pMessage.IsModified;
			}
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x060009E8 RID: 2536 RVA: 0x0003C7A8 File Offset: 0x0003B7A8
		// (set) Token: 0x060009E9 RID: 2537 RVA: 0x0003C7C0 File Offset: 0x0003B7C0
		public Mail_Message Message
		{
			get
			{
				return this.m_pMessage;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				bool flag2 = base.Entity == null;
				if (flag2)
				{
					throw new InvalidOperationException("Body must be bounded to some entity first.");
				}
				bool flag3 = base.Entity.ContentType == null || !string.Equals(base.Entity.ContentType.TypeWithSubtype, base.MediaType, StringComparison.InvariantCultureIgnoreCase);
				if (flag3)
				{
					base.Entity.ContentType = new MIME_h_ContentType(base.MediaType);
				}
				this.m_pMessage = value;
			}
		}

		// Token: 0x04000453 RID: 1107
		private Mail_Message m_pMessage = null;
	}
}
