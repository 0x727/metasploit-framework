using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000058 RID: 88
	internal class ObjectOutputStream : OutputStream
	{
		// Token: 0x0600022E RID: 558 RVA: 0x0000A06D File Offset: 0x0000826D
		public ObjectOutputStream(OutputStream os)
		{
			this._bw = new BinaryWriter(os.GetWrappedStream());
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000A088 File Offset: 0x00008288
		public virtual void WriteInt(int i)
		{
			this._bw.Write(i);
		}

		// Token: 0x0400006E RID: 110
		private BinaryWriter _bw;
	}
}
