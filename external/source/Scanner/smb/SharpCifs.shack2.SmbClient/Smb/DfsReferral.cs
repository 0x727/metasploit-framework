using System;
using System.Collections.Generic;

namespace SharpCifs.Smb
{
	// Token: 0x02000072 RID: 114
	public class DfsReferral : SmbException
	{
		// Token: 0x06000343 RID: 835 RVA: 0x0000DD24 File Offset: 0x0000BF24
		public DfsReferral()
		{
			this.Next = this;
		}

		// Token: 0x06000344 RID: 836 RVA: 0x0000DD3C File Offset: 0x0000BF3C
		internal virtual void Append(DfsReferral dr)
		{
			dr.Next = this.Next;
			this.Next = dr;
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0000DD54 File Offset: 0x0000BF54
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"DfsReferral[pathConsumed=",
				this.PathConsumed,
				",server=",
				this.Server,
				",share=",
				this.Share,
				",link=",
				this.Link,
				",path=",
				this.Path,
				",ttl=",
				this.Ttl,
				",expiration=",
				this.Expiration,
				",resolveHashes=",
				this.ResolveHashes.ToString(),
				"]"
			});
		}

		// Token: 0x040000C6 RID: 198
		public int PathConsumed;

		// Token: 0x040000C7 RID: 199
		public long Ttl;

		// Token: 0x040000C8 RID: 200
		public string Server;

		// Token: 0x040000C9 RID: 201
		public string Share;

		// Token: 0x040000CA RID: 202
		public string Link;

		// Token: 0x040000CB RID: 203
		public string Path;

		// Token: 0x040000CC RID: 204
		public bool ResolveHashes;

		// Token: 0x040000CD RID: 205
		public long Expiration;

		// Token: 0x040000CE RID: 206
		internal DfsReferral Next;

		// Token: 0x040000CF RID: 207
		internal IDictionary<string, DfsReferral> Map;

		// Token: 0x040000D0 RID: 208
		internal string Key = null;
	}
}
