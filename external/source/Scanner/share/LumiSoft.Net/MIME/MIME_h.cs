using System;
using System.Text;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000109 RID: 265
	public abstract class MIME_h
	{
		// Token: 0x06000A37 RID: 2615 RVA: 0x00009954 File Offset: 0x00008954
		public MIME_h()
		{
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x0003EB3C File Offset: 0x0003DB3C
		public override string ToString()
		{
			return this.ToString(null, null, false);
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x0003EB58 File Offset: 0x0003DB58
		public string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset)
		{
			return this.ToString(wordEncoder, parmetersCharset, false);
		}

		// Token: 0x06000A3A RID: 2618
		public abstract string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode);

		// Token: 0x06000A3B RID: 2619 RVA: 0x0003EB74 File Offset: 0x0003DB74
		public string ValueToString()
		{
			return this.ValueToString(null, null);
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x0003EB90 File Offset: 0x0003DB90
		public string ValueToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset)
		{
			return this.ToString(wordEncoder, parmetersCharset).Split(new char[]
			{
				':'
			}, 2)[1].TrimStart(new char[0]);
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06000A3D RID: 2621
		public abstract bool IsModified { get; }

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06000A3E RID: 2622
		public abstract string Name { get; }
	}
}
