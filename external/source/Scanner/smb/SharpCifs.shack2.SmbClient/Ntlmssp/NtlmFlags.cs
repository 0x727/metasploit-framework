using System;

namespace SharpCifs.Ntlmssp
{
	// Token: 0x020000CB RID: 203
	public abstract class NtlmFlags
	{
		// Token: 0x040003E7 RID: 999
		public const int NtlmsspNegotiateUnicode = 1;

		// Token: 0x040003E8 RID: 1000
		public const int NtlmsspNegotiateOem = 2;

		// Token: 0x040003E9 RID: 1001
		public const int NtlmsspRequestTarget = 4;

		// Token: 0x040003EA RID: 1002
		public const int NtlmsspNegotiateSign = 16;

		// Token: 0x040003EB RID: 1003
		public const int NtlmsspNegotiateSeal = 32;

		// Token: 0x040003EC RID: 1004
		public const int NtlmsspNegotiateDatagramStyle = 64;

		// Token: 0x040003ED RID: 1005
		public const int NtlmsspNegotiateLmKey = 128;

		// Token: 0x040003EE RID: 1006
		public const int NtlmsspNegotiateNetware = 256;

		// Token: 0x040003EF RID: 1007
		public const int NtlmsspNegotiateNtlm = 512;

		// Token: 0x040003F0 RID: 1008
		public const int NtlmsspNegotiateOemDomainSupplied = 4096;

		// Token: 0x040003F1 RID: 1009
		public const int NtlmsspNegotiateOemWorkstationSupplied = 8192;

		// Token: 0x040003F2 RID: 1010
		public const int NtlmsspNegotiateLocalCall = 16384;

		// Token: 0x040003F3 RID: 1011
		public const int NtlmsspNegotiateAlwaysSign = 32768;

		// Token: 0x040003F4 RID: 1012
		public const int NtlmsspTargetTypeDomain = 65536;

		// Token: 0x040003F5 RID: 1013
		public const int NtlmsspTargetTypeServer = 131072;

		// Token: 0x040003F6 RID: 1014
		public const int NtlmsspTargetTypeShare = 262144;

		// Token: 0x040003F7 RID: 1015
		public const int NtlmsspNegotiateNtlm2 = 524288;

		// Token: 0x040003F8 RID: 1016
		public const int NtlmsspRequestInitResponse = 1048576;

		// Token: 0x040003F9 RID: 1017
		public const int NtlmsspRequestAcceptResponse = 2097152;

		// Token: 0x040003FA RID: 1018
		public const int NtlmsspRequestNonNtSessionKey = 4194304;

		// Token: 0x040003FB RID: 1019
		public const int NtlmsspNegotiateTargetInfo = 8388608;

		// Token: 0x040003FC RID: 1020
		public const int NtlmsspNegotiate128 = 536870912;

		// Token: 0x040003FD RID: 1021
		public const int NtlmsspNegotiateKeyExch = 1073741824;

		// Token: 0x040003FE RID: 1022
		public const int NtlmsspNegotiate56 = -2147483648;
	}
}
