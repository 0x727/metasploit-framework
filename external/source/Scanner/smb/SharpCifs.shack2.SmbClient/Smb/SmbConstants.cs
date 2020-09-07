using System;
using System.Collections.Generic;
using System.Net;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000A7 RID: 167
	public static class SmbConstants
	{
		// Token: 0x04000274 RID: 628
		public static readonly int DefaultPort = 445;

		// Token: 0x04000275 RID: 629
		public static readonly int DefaultMaxMpxCount = 10;

		// Token: 0x04000276 RID: 630
		public static int DefaultResponseTimeout = 30000;

		// Token: 0x04000277 RID: 631
		public static int DefaultSoTimeout = 35000;

		// Token: 0x04000278 RID: 632
		public static readonly int DefaultRcvBufSize = 60416;

		// Token: 0x04000279 RID: 633
		public static readonly int DefaultSndBufSize = 16644;

		// Token: 0x0400027A RID: 634
		public static readonly int DefaultSsnLimit = 250;

		// Token: 0x0400027B RID: 635
		public static int DefaultConnTimeout = 35000;

		// Token: 0x0400027C RID: 636
		public static readonly IPAddress Laddr = Config.GetLocalHost();

		// Token: 0x0400027D RID: 637
		public static readonly int Lport = Config.GetInt("jcifs.smb.client.lport", 0);

		// Token: 0x0400027E RID: 638
		public static readonly int MaxMpxCount = Config.GetInt("jcifs.smb.client.maxMpxCount", SmbConstants.DefaultMaxMpxCount);

		// Token: 0x0400027F RID: 639
		public static readonly int SndBufSize = Config.GetInt("jcifs.smb.client.snd_buf_size", SmbConstants.DefaultSndBufSize);

		// Token: 0x04000280 RID: 640
		public static readonly int RcvBufSize = Config.GetInt("jcifs.smb.client.rcv_buf_size", SmbConstants.DefaultRcvBufSize);

		// Token: 0x04000281 RID: 641
		public static readonly bool UseUnicode = Config.GetBoolean("jcifs.smb.client.useUnicode", true);

		// Token: 0x04000282 RID: 642
		public static readonly bool ForceUnicode = Config.GetBoolean("jcifs.smb.client.useUnicode", false);

		// Token: 0x04000283 RID: 643
		public static readonly bool UseNtstatus = Config.GetBoolean("jcifs.smb.client.useNtStatus", true);

		// Token: 0x04000284 RID: 644
		public static readonly bool Signpref = Config.GetBoolean("jcifs.smb.client.signingPreferred", false);

		// Token: 0x04000285 RID: 645
		public static readonly bool UseNtsmbs = Config.GetBoolean("jcifs.smb.client.useNTSmbs", true);

		// Token: 0x04000286 RID: 646
		public static readonly bool UseExtsec = Config.GetBoolean("jcifs.smb.client.useExtendedSecurity", true);

		// Token: 0x04000287 RID: 647
		public static readonly string NetbiosHostname = Config.GetProperty("jcifs.netbios.hostname", null);

		// Token: 0x04000288 RID: 648
		public static readonly int LmCompatibility = Config.GetInt("jcifs.smb.lmCompatibility", 3);

		// Token: 0x04000289 RID: 649
		public static readonly int FlagsNone = 0;

		// Token: 0x0400028A RID: 650
		public static readonly int FlagsLockAndReadWriteAndUnlock = 1;

		// Token: 0x0400028B RID: 651
		public static readonly int FlagsReceiveBufferPosted = 2;

		// Token: 0x0400028C RID: 652
		public static readonly int FlagsPathNamesCaseless = 8;

		// Token: 0x0400028D RID: 653
		public static readonly int FlagsPathNamesCanonicalized = 16;

		// Token: 0x0400028E RID: 654
		public static readonly int FlagsOplockRequestedOrGranted = 32;

		// Token: 0x0400028F RID: 655
		public static readonly int FlagsNotifyOfModifyAction = 64;

		// Token: 0x04000290 RID: 656
		public static readonly int FlagsResponse = 128;

		// Token: 0x04000291 RID: 657
		public static readonly int Flags2None = 0;

		// Token: 0x04000292 RID: 658
		public static readonly int Flags2LongFilenames = 1;

		// Token: 0x04000293 RID: 659
		public static readonly int Flags2ExtendedAttributes = 2;

		// Token: 0x04000294 RID: 660
		public static readonly int Flags2SecuritySignatures = 4;

		// Token: 0x04000295 RID: 661
		public static readonly int Flags2ExtendedSecurityNegotiation = 2048;

		// Token: 0x04000296 RID: 662
		public static readonly int Flags2ResolvePathsInDfs = 4096;

		// Token: 0x04000297 RID: 663
		public static readonly int Flags2PermitReadIfExecutePerm = 8192;

		// Token: 0x04000298 RID: 664
		public static readonly int Flags2Status32 = 16384;

		// Token: 0x04000299 RID: 665
		public static readonly int Flags2Unicode = 32768;

		// Token: 0x0400029A RID: 666
		public static readonly int CapNone = 0;

		// Token: 0x0400029B RID: 667
		public static readonly int CapRawMode = 1;

		// Token: 0x0400029C RID: 668
		public static readonly int CapMpxMode = 2;

		// Token: 0x0400029D RID: 669
		public static readonly int CapUnicode = 4;

		// Token: 0x0400029E RID: 670
		public static readonly int CapLargeFiles = 8;

		// Token: 0x0400029F RID: 671
		public static readonly int CapNtSmbs = 16;

		// Token: 0x040002A0 RID: 672
		public static readonly int CapRpcRemoteApis = 32;

		// Token: 0x040002A1 RID: 673
		public static readonly int CapStatus32 = 64;

		// Token: 0x040002A2 RID: 674
		public static readonly int CapLevelIiOplocks = 128;

		// Token: 0x040002A3 RID: 675
		public static readonly int CapLockAndRead = 256;

		// Token: 0x040002A4 RID: 676
		public static readonly int CapNtFind = 512;

		// Token: 0x040002A5 RID: 677
		public static readonly int CapDfs = 4096;

		// Token: 0x040002A6 RID: 678
		public static readonly int CapExtendedSecurity = int.MinValue;

		// Token: 0x040002A7 RID: 679
		public static readonly int AttrReadonly = 1;

		// Token: 0x040002A8 RID: 680
		public static readonly int AttrHidden = 2;

		// Token: 0x040002A9 RID: 681
		public static readonly int AttrSystem = 4;

		// Token: 0x040002AA RID: 682
		public static readonly int AttrVolume = 8;

		// Token: 0x040002AB RID: 683
		public static readonly int AttrDirectory = 16;

		// Token: 0x040002AC RID: 684
		public static readonly int AttrArchive = 32;

		// Token: 0x040002AD RID: 685
		public static readonly int AttrCompressed = 2048;

		// Token: 0x040002AE RID: 686
		public static readonly int AttrNormal = 128;

		// Token: 0x040002AF RID: 687
		public static readonly int AttrTemporary = 256;

		// Token: 0x040002B0 RID: 688
		public static readonly int FileReadData = 1;

		// Token: 0x040002B1 RID: 689
		public static readonly int FileWriteData = 2;

		// Token: 0x040002B2 RID: 690
		public static readonly int FileAppendData = 4;

		// Token: 0x040002B3 RID: 691
		public static readonly int FileReadEa = 8;

		// Token: 0x040002B4 RID: 692
		public static readonly int FileWriteEa = 16;

		// Token: 0x040002B5 RID: 693
		public static readonly int FileExecute = 32;

		// Token: 0x040002B6 RID: 694
		public static readonly int FileDelete = 64;

		// Token: 0x040002B7 RID: 695
		public static readonly int FileReadAttributes = 128;

		// Token: 0x040002B8 RID: 696
		public static readonly int FileWriteAttributes = 256;

		// Token: 0x040002B9 RID: 697
		public static readonly int Delete = 65536;

		// Token: 0x040002BA RID: 698
		public static readonly int ReadControl = 131072;

		// Token: 0x040002BB RID: 699
		public static readonly int WriteDac = 262144;

		// Token: 0x040002BC RID: 700
		public static readonly int WriteOwner = 524288;

		// Token: 0x040002BD RID: 701
		public static readonly int Synchronize = 1048576;

		// Token: 0x040002BE RID: 702
		public static readonly int GenericAll = 268435456;

		// Token: 0x040002BF RID: 703
		public static readonly int GenericExecute = 536870912;

		// Token: 0x040002C0 RID: 704
		public static readonly int GenericWrite = 1073741824;

		// Token: 0x040002C1 RID: 705
		public static readonly int GenericRead = int.MinValue;

		// Token: 0x040002C2 RID: 706
		public static readonly int FlagsTargetMustBeFile = 1;

		// Token: 0x040002C3 RID: 707
		public static readonly int FlagsTargetMustBeDirectory = 2;

		// Token: 0x040002C4 RID: 708
		public static readonly int FlagsCopyTargetModeAscii = 4;

		// Token: 0x040002C5 RID: 709
		public static readonly int FlagsCopySourceModeAscii = 8;

		// Token: 0x040002C6 RID: 710
		public static readonly int FlagsVerifyAllWrites = 16;

		// Token: 0x040002C7 RID: 711
		public static readonly int FlagsTreeCopy = 32;

		// Token: 0x040002C8 RID: 712
		public static readonly int OpenFunctionFailIfExists = 0;

		// Token: 0x040002C9 RID: 713
		public static readonly int OpenFunctionOverwriteIfExists = 32;

		// Token: 0x040002CA RID: 714
		public static readonly int Pid = (int)(new Random().NextDouble() * 65536.0);

		// Token: 0x040002CB RID: 715
		public static readonly int SecurityShare = 0;

		// Token: 0x040002CC RID: 716
		public static readonly int SecurityUser = 1;

		// Token: 0x040002CD RID: 717
		public static readonly int CmdOffset = 4;

		// Token: 0x040002CE RID: 718
		public static readonly int ErrorCodeOffset = 5;

		// Token: 0x040002CF RID: 719
		public static readonly int FlagsOffset = 9;

		// Token: 0x040002D0 RID: 720
		public static readonly int SignatureOffset = 14;

		// Token: 0x040002D1 RID: 721
		public static readonly int TidOffset = 24;

		// Token: 0x040002D2 RID: 722
		public static readonly int HeaderLength = 32;

		// Token: 0x040002D3 RID: 723
		public static readonly long MillisecondsBetween1970And1601 = 11644473600000L;

		// Token: 0x040002D4 RID: 724
		public static readonly TimeZoneInfo Tz = TimeZoneInfo.Local;

		// Token: 0x040002D5 RID: 725
		public static readonly bool UseBatching = Config.GetBoolean("jcifs.smb.client.useBatching", true);

		// Token: 0x040002D6 RID: 726
		public static readonly string OemEncoding = Config.GetProperty("jcifs.encoding", Config.DefaultOemEncoding);

		// Token: 0x040002D7 RID: 727
		public static readonly string UniEncoding = "UTF-16LE";

		// Token: 0x040002D8 RID: 728
		public static readonly int DefaultFlags2 = SmbConstants.Flags2LongFilenames | SmbConstants.Flags2ExtendedAttributes | (SmbConstants.UseExtsec ? SmbConstants.Flags2ExtendedSecurityNegotiation : 0) | (SmbConstants.Signpref ? SmbConstants.Flags2SecuritySignatures : 0) | (SmbConstants.UseNtstatus ? SmbConstants.Flags2Status32 : 0) | (SmbConstants.UseUnicode ? SmbConstants.Flags2Unicode : 0);

		// Token: 0x040002D9 RID: 729
		public static readonly int DefaultCapabilities = (SmbConstants.UseNtsmbs ? SmbConstants.CapNtSmbs : 0) | (SmbConstants.UseNtstatus ? SmbConstants.CapStatus32 : 0) | (SmbConstants.UseUnicode ? SmbConstants.CapUnicode : 0) | SmbConstants.CapDfs;

		// Token: 0x040002DA RID: 730
		public static readonly int Flags2 = Config.GetInt("jcifs.smb.client.flags2", SmbConstants.DefaultFlags2);

		// Token: 0x040002DB RID: 731
		public static readonly int Capabilities = Config.GetInt("jcifs.smb.client.capabilities", SmbConstants.DefaultCapabilities);

		// Token: 0x040002DC RID: 732
		public static readonly bool TcpNodelay = Config.GetBoolean("jcifs.smb.client.tcpNoDelay", false);

		// Token: 0x040002DD RID: 733
		public static readonly int ResponseTimeout = Config.GetInt("jcifs.smb.client.responseTimeout", SmbConstants.DefaultResponseTimeout);

		// Token: 0x040002DE RID: 734
		public static readonly List<SmbTransport> Connections = new List<SmbTransport>();

		// Token: 0x040002DF RID: 735
		public static readonly int SsnLimit = Config.GetInt("jcifs.smb.client.ssnLimit", SmbConstants.DefaultSsnLimit);

		// Token: 0x040002E0 RID: 736
		public static readonly int SoTimeout = Config.GetInt("jcifs.smb.client.soTimeout", SmbConstants.DefaultSoTimeout);

		// Token: 0x040002E1 RID: 737
		public static readonly int ConnTimeout = Config.GetInt("jcifs.smb.client.connTimeout", SmbConstants.DefaultConnTimeout);

		// Token: 0x040002E2 RID: 738
		public static readonly string NativeOs = Config.GetProperty("jcifs.smb.client.nativeOs", Runtime.GetProperty("os.name"));

		// Token: 0x040002E3 RID: 739
		public static readonly string NativeLanman = Config.GetProperty("jcifs.smb.client.nativeLanMan", "jCIFS");

		// Token: 0x040002E4 RID: 740
		public static readonly int VcNumber = 1;

		// Token: 0x040002E5 RID: 741
		public static SmbTransport NullTransport = new SmbTransport(null, 0, null, 0);
	}
}
