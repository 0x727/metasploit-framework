using System;

namespace LumiSoft.Net.Media.Codec.Audio
{
	// Token: 0x02000156 RID: 342
	public abstract class AudioCodec : Codec
	{
		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06000DE1 RID: 3553
		public abstract AudioFormat AudioFormat { get; }

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06000DE2 RID: 3554
		public abstract AudioFormat CompressedAudioFormat { get; }
	}
}
