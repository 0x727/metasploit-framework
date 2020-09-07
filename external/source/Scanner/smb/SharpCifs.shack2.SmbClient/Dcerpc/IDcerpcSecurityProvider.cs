using System;
using SharpCifs.Dcerpc.Ndr;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000E5 RID: 229
	public interface IDcerpcSecurityProvider
	{
		// Token: 0x06000794 RID: 1940
		void Wrap(NdrBuffer outgoing);

		// Token: 0x06000795 RID: 1941
		void Unwrap(NdrBuffer incoming);
	}
}
