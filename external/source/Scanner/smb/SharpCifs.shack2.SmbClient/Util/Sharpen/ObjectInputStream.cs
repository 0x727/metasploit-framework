using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000057 RID: 87
	internal class ObjectInputStream : InputStream
	{
		// Token: 0x0600022B RID: 555 RVA: 0x0000A033 File Offset: 0x00008233
		public ObjectInputStream(InputStream s)
		{
			this._reader = new BinaryReader(s.GetWrappedStream());
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000A050 File Offset: 0x00008250
		public int ReadInt()
		{
			return this._reader.ReadInt32();
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00002380 File Offset: 0x00000580
		public object ReadObject()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0400006D RID: 109
		private BinaryReader _reader;
	}
}
