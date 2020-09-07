using System;

namespace LumiSoft.Net.Media.Codec
{
	// Token: 0x02000155 RID: 341
	public abstract class Codec
	{
		// Token: 0x06000DDD RID: 3549
		public abstract byte[] Encode(byte[] buffer, int offset, int count);

		// Token: 0x06000DDE RID: 3550
		public abstract byte[] Decode(byte[] buffer, int offset, int count);

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06000DDF RID: 3551
		public abstract string Name { get; }
	}
}
