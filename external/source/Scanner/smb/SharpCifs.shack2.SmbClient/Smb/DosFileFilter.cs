using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000074 RID: 116
	public class DosFileFilter : ISmbFileFilter
	{
		// Token: 0x06000347 RID: 839 RVA: 0x0000E39B File Offset: 0x0000C59B
		public DosFileFilter(string wildcard, int attributes)
		{
			this.Wildcard = wildcard;
			this.Attributes = attributes;
		}

		// Token: 0x06000348 RID: 840 RVA: 0x0000E3B4 File Offset: 0x0000C5B4
		public virtual bool Accept(SmbFile file)
		{
			return (file.GetAttributes() & this.Attributes) != 0;
		}

		// Token: 0x040000D3 RID: 211
		protected internal string Wildcard;

		// Token: 0x040000D4 RID: 212
		protected internal int Attributes;
	}
}
