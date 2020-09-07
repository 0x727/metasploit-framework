using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000F7 RID: 247
	public class MIME_b_MessageDeliveryStatus : MIME_b
	{
		// Token: 0x060009DE RID: 2526 RVA: 0x0003C458 File Offset: 0x0003B458
		public MIME_b_MessageDeliveryStatus() : base(new MIME_h_ContentType("message/delivery-status"))
		{
			this.m_pMessageFields = new MIME_h_Collection(new MIME_h_Provider());
			this.m_pRecipientBlocks = new List<MIME_h_Collection>();
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x0003C498 File Offset: 0x0003B498
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
			MemoryStream memoryStream = new MemoryStream();
			Net_Utils.StreamCopy(stream, memoryStream, stream.LineBufferSize);
			memoryStream.Position = 0L;
			SmartStream smartStream = new SmartStream(memoryStream, true);
			MIME_b_MessageDeliveryStatus mime_b_MessageDeliveryStatus = new MIME_b_MessageDeliveryStatus();
			mime_b_MessageDeliveryStatus.m_pMessageFields.Parse(smartStream);
			while (smartStream.Position - (long)smartStream.BytesInReadBuffer < smartStream.Length)
			{
				MIME_h_Collection mime_h_Collection = new MIME_h_Collection(new MIME_h_Provider());
				mime_h_Collection.Parse(smartStream);
				mime_b_MessageDeliveryStatus.m_pRecipientBlocks.Add(mime_h_Collection);
			}
			return mime_b_MessageDeliveryStatus;
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x0003C568 File Offset: 0x0003B568
		protected internal override void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pMessageFields.ToStream(stream, headerWordEncoder, headerParmetersCharset, headerReencode);
			stream.Write(new byte[]
			{
				13,
				10
			}, 0, 2);
			foreach (MIME_h_Collection mime_h_Collection in this.m_pRecipientBlocks)
			{
				mime_h_Collection.ToStream(stream, headerWordEncoder, headerParmetersCharset, headerReencode);
				stream.Write(new byte[]
				{
					13,
					10
				}, 0, 2);
			}
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x060009E1 RID: 2529 RVA: 0x0003C61C File Offset: 0x0003B61C
		public override bool IsModified
		{
			get
			{
				bool isModified = this.m_pMessageFields.IsModified;
				bool result;
				if (isModified)
				{
					result = true;
				}
				else
				{
					foreach (MIME_h_Collection mime_h_Collection in this.m_pRecipientBlocks)
					{
						bool isModified2 = mime_h_Collection.IsModified;
						if (isModified2)
						{
							return true;
						}
					}
					result = false;
				}
				return result;
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x060009E2 RID: 2530 RVA: 0x0003C698 File Offset: 0x0003B698
		public MIME_h_Collection MessageFields
		{
			get
			{
				return this.m_pMessageFields;
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x060009E3 RID: 2531 RVA: 0x0003C6B0 File Offset: 0x0003B6B0
		public List<MIME_h_Collection> RecipientBlocks
		{
			get
			{
				return this.m_pRecipientBlocks;
			}
		}

		// Token: 0x04000451 RID: 1105
		private MIME_h_Collection m_pMessageFields = null;

		// Token: 0x04000452 RID: 1106
		private List<MIME_h_Collection> m_pRecipientBlocks = null;
	}
}
