using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x0200007C RID: 124
	public sealed class NtlmChallenge
	{
		// Token: 0x0600037D RID: 893 RVA: 0x0000ED5B File Offset: 0x0000CF5B
		internal NtlmChallenge(byte[] challenge, UniAddress dc)
		{
			this.Challenge = challenge;
			this.Dc = dc;
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0000ED74 File Offset: 0x0000CF74
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"NtlmChallenge[challenge=0x",
				Hexdump.ToHexString(this.Challenge, 0, this.Challenge.Length * 2),
				",dc=",
				this.Dc,
				"]"
			});
		}

		// Token: 0x040000E4 RID: 228
		public byte[] Challenge;

		// Token: 0x040000E5 RID: 229
		public UniAddress Dc;
	}
}
