using System;
using SharpCifs.Dcerpc.Ndr;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F1 RID: 241
	public class Lsarpc
	{
		// Token: 0x060007D2 RID: 2002 RVA: 0x0002A8CC File Offset: 0x00028ACC
		public static string GetSyntax()
		{
			return "12345778-1234-abcd-ef00-0123456789ab:0.0";
		}

		// Token: 0x040004FC RID: 1276
		public const int PolicyInfoAuditEvents = 2;

		// Token: 0x040004FD RID: 1277
		public const int PolicyInfoPrimaryDomain = 3;

		// Token: 0x040004FE RID: 1278
		public const int PolicyInfoAccountDomain = 5;

		// Token: 0x040004FF RID: 1279
		public const int PolicyInfoServerRole = 6;

		// Token: 0x04000500 RID: 1280
		public const int PolicyInfoModification = 9;

		// Token: 0x04000501 RID: 1281
		public const int PolicyInfoDnsDomain = 12;

		// Token: 0x04000502 RID: 1282
		public const int SidNameUseNone = 0;

		// Token: 0x04000503 RID: 1283
		public const int SidNameUser = 1;

		// Token: 0x04000504 RID: 1284
		public const int SidNameDomGrp = 2;

		// Token: 0x04000505 RID: 1285
		public const int SidNameDomain = 3;

		// Token: 0x04000506 RID: 1286
		public const int SidNameAlias = 4;

		// Token: 0x04000507 RID: 1287
		public const int SidNameWknGrp = 5;

		// Token: 0x04000508 RID: 1288
		public const int SidNameDeleted = 6;

		// Token: 0x04000509 RID: 1289
		public const int SidNameInvalid = 7;

		// Token: 0x0400050A RID: 1290
		public const int SidNameUnknown = 8;

		// Token: 0x0200012F RID: 303
		public class LsarQosInfo : NdrObject
		{
			// Token: 0x0600085E RID: 2142 RVA: 0x0002C4EA File Offset: 0x0002A6EA
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Length);
				dst.Enc_ndr_short((int)this.ImpersonationLevel);
				dst.Enc_ndr_small((int)this.ContextMode);
				dst.Enc_ndr_small((int)this.EffectiveOnly);
			}

			// Token: 0x0600085F RID: 2143 RVA: 0x0002C529 File Offset: 0x0002A729
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Length = src.Dec_ndr_long();
				this.ImpersonationLevel = (short)src.Dec_ndr_short();
				this.ContextMode = (byte)src.Dec_ndr_small();
				this.EffectiveOnly = (byte)src.Dec_ndr_small();
			}

			// Token: 0x040005D2 RID: 1490
			public int Length;

			// Token: 0x040005D3 RID: 1491
			public short ImpersonationLevel;

			// Token: 0x040005D4 RID: 1492
			public byte ContextMode;

			// Token: 0x040005D5 RID: 1493
			public byte EffectiveOnly;
		}

		// Token: 0x02000130 RID: 304
		public class LsarObjectAttributes : NdrObject
		{
			// Token: 0x06000861 RID: 2145 RVA: 0x0002C568 File Offset: 0x0002A768
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Length);
				dst.Enc_ndr_referent(this.RootDirectory, 1);
				dst.Enc_ndr_referent(this.ObjectName, 1);
				dst.Enc_ndr_long(this.Attributes);
				dst.Enc_ndr_long(this.SecurityDescriptor);
				dst.Enc_ndr_referent(this.SecurityQualityOfService, 1);
				bool flag = this.RootDirectory != null;
				if (flag)
				{
					dst = dst.Deferred;
					this.RootDirectory.Encode(dst);
				}
				bool flag2 = this.ObjectName != null;
				if (flag2)
				{
					dst = dst.Deferred;
					this.ObjectName.Encode(dst);
				}
				bool flag3 = this.SecurityQualityOfService != null;
				if (flag3)
				{
					dst = dst.Deferred;
					this.SecurityQualityOfService.Encode(dst);
				}
			}

			// Token: 0x06000862 RID: 2146 RVA: 0x0002C63C File Offset: 0x0002A83C
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Length = src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				int num2 = src.Dec_ndr_long();
				this.Attributes = src.Dec_ndr_long();
				this.SecurityDescriptor = src.Dec_ndr_long();
				int num3 = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.RootDirectory.Decode(src);
				}
				bool flag2 = num2 != 0;
				if (flag2)
				{
					bool flag3 = this.ObjectName == null;
					if (flag3)
					{
						this.ObjectName = new Rpc.Unicode_string();
					}
					src = src.Deferred;
					this.ObjectName.Decode(src);
				}
				bool flag4 = num3 != 0;
				if (flag4)
				{
					bool flag5 = this.SecurityQualityOfService == null;
					if (flag5)
					{
						this.SecurityQualityOfService = new Lsarpc.LsarQosInfo();
					}
					src = src.Deferred;
					this.SecurityQualityOfService.Decode(src);
				}
			}

			// Token: 0x040005D6 RID: 1494
			public int Length;

			// Token: 0x040005D7 RID: 1495
			public NdrSmall RootDirectory;

			// Token: 0x040005D8 RID: 1496
			public Rpc.Unicode_string ObjectName;

			// Token: 0x040005D9 RID: 1497
			public int Attributes;

			// Token: 0x040005DA RID: 1498
			public int SecurityDescriptor;

			// Token: 0x040005DB RID: 1499
			public Lsarpc.LsarQosInfo SecurityQualityOfService;
		}

		// Token: 0x02000131 RID: 305
		public class LsarDomainInfo : NdrObject
		{
			// Token: 0x06000864 RID: 2148 RVA: 0x0002C724 File Offset: 0x0002A924
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_short((int)this.Name.Length);
				dst.Enc_ndr_short((int)this.Name.MaximumLength);
				dst.Enc_ndr_referent(this.Name.Buffer, 1);
				dst.Enc_ndr_referent(this.Sid, 1);
				bool flag = this.Name.Buffer != null;
				if (flag)
				{
					dst = dst.Deferred;
					int num = (int)(this.Name.Length / 2);
					int l = (int)(this.Name.MaximumLength / 2);
					dst.Enc_ndr_long(l);
					dst.Enc_ndr_long(0);
					dst.Enc_ndr_long(num);
					int index = dst.Index;
					dst.Advance(2 * num);
					dst = dst.Derive(index);
					for (int i = 0; i < num; i++)
					{
						dst.Enc_ndr_short((int)this.Name.Buffer[i]);
					}
				}
				bool flag2 = this.Sid != null;
				if (flag2)
				{
					dst = dst.Deferred;
					this.Sid.Encode(dst);
				}
			}

			// Token: 0x06000865 RID: 2149 RVA: 0x0002C840 File Offset: 0x0002AA40
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				src.Align(4);
				bool flag = this.Name == null;
				if (flag)
				{
					this.Name = new Rpc.Unicode_string();
				}
				this.Name.Length = (short)src.Dec_ndr_short();
				this.Name.MaximumLength = (short)src.Dec_ndr_short();
				int num = src.Dec_ndr_long();
				int num2 = src.Dec_ndr_long();
				bool flag2 = num != 0;
				if (flag2)
				{
					src = src.Deferred;
					int num3 = src.Dec_ndr_long();
					src.Dec_ndr_long();
					int num4 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(2 * num4);
					bool flag3 = this.Name.Buffer == null;
					if (flag3)
					{
						bool flag4 = num3 < 0 || num3 > 65535;
						if (flag4)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Name.Buffer = new short[num3];
					}
					src = src.Derive(index);
					for (int i = 0; i < num4; i++)
					{
						this.Name.Buffer[i] = (short)src.Dec_ndr_short();
					}
				}
				bool flag5 = num2 != 0;
				if (flag5)
				{
					bool flag6 = this.Sid == null;
					if (flag6)
					{
						this.Sid = new Rpc.SidT();
					}
					src = src.Deferred;
					this.Sid.Decode(src);
				}
			}

			// Token: 0x040005DC RID: 1500
			public Rpc.Unicode_string Name;

			// Token: 0x040005DD RID: 1501
			public Rpc.SidT Sid;
		}

		// Token: 0x02000132 RID: 306
		public class LsarDnsDomainInfo : NdrObject
		{
			// Token: 0x06000867 RID: 2151 RVA: 0x0002C9AC File Offset: 0x0002ABAC
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_short((int)this.Name.Length);
				dst.Enc_ndr_short((int)this.Name.MaximumLength);
				dst.Enc_ndr_referent(this.Name.Buffer, 1);
				dst.Enc_ndr_short((int)this.DnsDomain.Length);
				dst.Enc_ndr_short((int)this.DnsDomain.MaximumLength);
				dst.Enc_ndr_referent(this.DnsDomain.Buffer, 1);
				dst.Enc_ndr_short((int)this.DnsForest.Length);
				dst.Enc_ndr_short((int)this.DnsForest.MaximumLength);
				dst.Enc_ndr_referent(this.DnsForest.Buffer, 1);
				dst.Enc_ndr_long(this.DomainGuid.TimeLow);
				dst.Enc_ndr_short((int)this.DomainGuid.TimeMid);
				dst.Enc_ndr_short((int)this.DomainGuid.TimeHiAndVersion);
				dst.Enc_ndr_small((int)this.DomainGuid.ClockSeqHiAndReserved);
				dst.Enc_ndr_small((int)this.DomainGuid.ClockSeqLow);
				int num = 6;
				int index = dst.Index;
				dst.Advance(num);
				dst.Enc_ndr_referent(this.Sid, 1);
				bool flag = this.Name.Buffer != null;
				if (flag)
				{
					dst = dst.Deferred;
					int num2 = (int)(this.Name.Length / 2);
					int l = (int)(this.Name.MaximumLength / 2);
					dst.Enc_ndr_long(l);
					dst.Enc_ndr_long(0);
					dst.Enc_ndr_long(num2);
					int index2 = dst.Index;
					dst.Advance(2 * num2);
					dst = dst.Derive(index2);
					for (int i = 0; i < num2; i++)
					{
						dst.Enc_ndr_short((int)this.Name.Buffer[i]);
					}
				}
				bool flag2 = this.DnsDomain.Buffer != null;
				if (flag2)
				{
					dst = dst.Deferred;
					int num3 = (int)(this.DnsDomain.Length / 2);
					int l2 = (int)(this.DnsDomain.MaximumLength / 2);
					dst.Enc_ndr_long(l2);
					dst.Enc_ndr_long(0);
					dst.Enc_ndr_long(num3);
					int index3 = dst.Index;
					dst.Advance(2 * num3);
					dst = dst.Derive(index3);
					for (int j = 0; j < num3; j++)
					{
						dst.Enc_ndr_short((int)this.DnsDomain.Buffer[j]);
					}
				}
				bool flag3 = this.DnsForest.Buffer != null;
				if (flag3)
				{
					dst = dst.Deferred;
					int num4 = (int)(this.DnsForest.Length / 2);
					int l3 = (int)(this.DnsForest.MaximumLength / 2);
					dst.Enc_ndr_long(l3);
					dst.Enc_ndr_long(0);
					dst.Enc_ndr_long(num4);
					int index4 = dst.Index;
					dst.Advance(2 * num4);
					dst = dst.Derive(index4);
					for (int k = 0; k < num4; k++)
					{
						dst.Enc_ndr_short((int)this.DnsForest.Buffer[k]);
					}
				}
				dst = dst.Derive(index);
				for (int m = 0; m < num; m++)
				{
					dst.Enc_ndr_small((int)this.DomainGuid.Node[m]);
				}
				bool flag4 = this.Sid != null;
				if (flag4)
				{
					dst = dst.Deferred;
					this.Sid.Encode(dst);
				}
			}

			// Token: 0x06000868 RID: 2152 RVA: 0x0002CD24 File Offset: 0x0002AF24
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				src.Align(4);
				bool flag = this.Name == null;
				if (flag)
				{
					this.Name = new Rpc.Unicode_string();
				}
				this.Name.Length = (short)src.Dec_ndr_short();
				this.Name.MaximumLength = (short)src.Dec_ndr_short();
				int num = src.Dec_ndr_long();
				src.Align(4);
				bool flag2 = this.DnsDomain == null;
				if (flag2)
				{
					this.DnsDomain = new Rpc.Unicode_string();
				}
				this.DnsDomain.Length = (short)src.Dec_ndr_short();
				this.DnsDomain.MaximumLength = (short)src.Dec_ndr_short();
				int num2 = src.Dec_ndr_long();
				src.Align(4);
				bool flag3 = this.DnsForest == null;
				if (flag3)
				{
					this.DnsForest = new Rpc.Unicode_string();
				}
				this.DnsForest.Length = (short)src.Dec_ndr_short();
				this.DnsForest.MaximumLength = (short)src.Dec_ndr_short();
				int num3 = src.Dec_ndr_long();
				src.Align(4);
				bool flag4 = this.DomainGuid == null;
				if (flag4)
				{
					this.DomainGuid = new Rpc.UuidT();
				}
				this.DomainGuid.TimeLow = src.Dec_ndr_long();
				this.DomainGuid.TimeMid = (short)src.Dec_ndr_short();
				this.DomainGuid.TimeHiAndVersion = (short)src.Dec_ndr_short();
				this.DomainGuid.ClockSeqHiAndReserved = (byte)src.Dec_ndr_small();
				this.DomainGuid.ClockSeqLow = (byte)src.Dec_ndr_small();
				int num4 = 6;
				int index = src.Index;
				src.Advance(num4);
				int num5 = src.Dec_ndr_long();
				bool flag5 = num != 0;
				if (flag5)
				{
					src = src.Deferred;
					int num6 = src.Dec_ndr_long();
					src.Dec_ndr_long();
					int num7 = src.Dec_ndr_long();
					int index2 = src.Index;
					src.Advance(2 * num7);
					bool flag6 = this.Name.Buffer == null;
					if (flag6)
					{
						bool flag7 = num6 < 0 || num6 > 65535;
						if (flag7)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Name.Buffer = new short[num6];
					}
					src = src.Derive(index2);
					for (int i = 0; i < num7; i++)
					{
						this.Name.Buffer[i] = (short)src.Dec_ndr_short();
					}
				}
				bool flag8 = num2 != 0;
				if (flag8)
				{
					src = src.Deferred;
					int num8 = src.Dec_ndr_long();
					src.Dec_ndr_long();
					int num9 = src.Dec_ndr_long();
					int index3 = src.Index;
					src.Advance(2 * num9);
					bool flag9 = this.DnsDomain.Buffer == null;
					if (flag9)
					{
						bool flag10 = num8 < 0 || num8 > 65535;
						if (flag10)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.DnsDomain.Buffer = new short[num8];
					}
					src = src.Derive(index3);
					for (int j = 0; j < num9; j++)
					{
						this.DnsDomain.Buffer[j] = (short)src.Dec_ndr_short();
					}
				}
				bool flag11 = num3 != 0;
				if (flag11)
				{
					src = src.Deferred;
					int num10 = src.Dec_ndr_long();
					src.Dec_ndr_long();
					int num11 = src.Dec_ndr_long();
					int index4 = src.Index;
					src.Advance(2 * num11);
					bool flag12 = this.DnsForest.Buffer == null;
					if (flag12)
					{
						bool flag13 = num10 < 0 || num10 > 65535;
						if (flag13)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.DnsForest.Buffer = new short[num10];
					}
					src = src.Derive(index4);
					for (int k = 0; k < num11; k++)
					{
						this.DnsForest.Buffer[k] = (short)src.Dec_ndr_short();
					}
				}
				bool flag14 = this.DomainGuid.Node == null;
				if (flag14)
				{
					bool flag15 = num4 < 0 || num4 > 65535;
					if (flag15)
					{
						throw new NdrException(NdrException.InvalidConformance);
					}
					this.DomainGuid.Node = new byte[num4];
				}
				src = src.Derive(index);
				for (int l = 0; l < num4; l++)
				{
					this.DomainGuid.Node[l] = (byte)src.Dec_ndr_small();
				}
				bool flag16 = num5 != 0;
				if (flag16)
				{
					bool flag17 = this.Sid == null;
					if (flag17)
					{
						this.Sid = new Rpc.SidT();
					}
					src = src.Deferred;
					this.Sid.Decode(src);
				}
			}

			// Token: 0x040005DE RID: 1502
			public Rpc.Unicode_string Name;

			// Token: 0x040005DF RID: 1503
			public Rpc.Unicode_string DnsDomain;

			// Token: 0x040005E0 RID: 1504
			public Rpc.Unicode_string DnsForest;

			// Token: 0x040005E1 RID: 1505
			public Rpc.UuidT DomainGuid;

			// Token: 0x040005E2 RID: 1506
			public Rpc.SidT Sid;
		}

		// Token: 0x02000133 RID: 307
		public class LsarSidPtr : NdrObject
		{
			// Token: 0x0600086A RID: 2154 RVA: 0x0002D1C8 File Offset: 0x0002B3C8
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_referent(this.Sid, 1);
				bool flag = this.Sid != null;
				if (flag)
				{
					dst = dst.Deferred;
					this.Sid.Encode(dst);
				}
			}

			// Token: 0x0600086B RID: 2155 RVA: 0x0002D210 File Offset: 0x0002B410
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					bool flag2 = this.Sid == null;
					if (flag2)
					{
						this.Sid = new Rpc.SidT();
					}
					src = src.Deferred;
					this.Sid.Decode(src);
				}
			}

			// Token: 0x040005E3 RID: 1507
			public Rpc.SidT Sid;
		}

		// Token: 0x02000134 RID: 308
		public class LsarSidArray : NdrObject
		{
			// Token: 0x0600086D RID: 2157 RVA: 0x0002D268 File Offset: 0x0002B468
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.NumSids);
				dst.Enc_ndr_referent(this.Sids, 1);
				bool flag = this.Sids != null;
				if (flag)
				{
					dst = dst.Deferred;
					int numSids = this.NumSids;
					dst.Enc_ndr_long(numSids);
					int index = dst.Index;
					dst.Advance(4 * numSids);
					dst = dst.Derive(index);
					for (int i = 0; i < numSids; i++)
					{
						this.Sids[i].Encode(dst);
					}
				}
			}

			// Token: 0x0600086E RID: 2158 RVA: 0x0002D2FC File Offset: 0x0002B4FC
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.NumSids = src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					int num2 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(4 * num2);
					bool flag2 = this.Sids == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Sids = new Lsarpc.LsarSidPtr[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.Sids[i] == null;
						if (flag4)
						{
							this.Sids[i] = new Lsarpc.LsarSidPtr();
						}
						this.Sids[i].Decode(src);
					}
				}
			}

			// Token: 0x040005E4 RID: 1508
			public int NumSids;

			// Token: 0x040005E5 RID: 1509
			public Lsarpc.LsarSidPtr[] Sids;
		}

		// Token: 0x02000135 RID: 309
		public class LsarTranslatedSid : NdrObject
		{
			// Token: 0x06000870 RID: 2160 RVA: 0x0002D3E3 File Offset: 0x0002B5E3
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_short(this.SidType);
				dst.Enc_ndr_long(this.Rid);
				dst.Enc_ndr_long(this.SidIndex);
			}

			// Token: 0x06000871 RID: 2161 RVA: 0x0002D415 File Offset: 0x0002B615
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.SidType = src.Dec_ndr_short();
				this.Rid = src.Dec_ndr_long();
				this.SidIndex = src.Dec_ndr_long();
			}

			// Token: 0x040005E6 RID: 1510
			public int SidType;

			// Token: 0x040005E7 RID: 1511
			public int Rid;

			// Token: 0x040005E8 RID: 1512
			public int SidIndex;
		}

		// Token: 0x02000136 RID: 310
		public class LsarTransSidArray : NdrObject
		{
			// Token: 0x06000873 RID: 2163 RVA: 0x0002D444 File Offset: 0x0002B644
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.Sids, 1);
				bool flag = this.Sids != null;
				if (flag)
				{
					dst = dst.Deferred;
					int count = this.Count;
					dst.Enc_ndr_long(count);
					int index = dst.Index;
					dst.Advance(12 * count);
					dst = dst.Derive(index);
					for (int i = 0; i < count; i++)
					{
						this.Sids[i].Encode(dst);
					}
				}
			}

			// Token: 0x06000874 RID: 2164 RVA: 0x0002D4DC File Offset: 0x0002B6DC
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Count = src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					int num2 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(12 * num2);
					bool flag2 = this.Sids == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Sids = new Lsarpc.LsarTranslatedSid[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.Sids[i] == null;
						if (flag4)
						{
							this.Sids[i] = new Lsarpc.LsarTranslatedSid();
						}
						this.Sids[i].Decode(src);
					}
				}
			}

			// Token: 0x040005E9 RID: 1513
			public int Count;

			// Token: 0x040005EA RID: 1514
			public Lsarpc.LsarTranslatedSid[] Sids;
		}

		// Token: 0x02000137 RID: 311
		public class LsarTrustInformation : NdrObject
		{
			// Token: 0x06000876 RID: 2166 RVA: 0x0002D5C4 File Offset: 0x0002B7C4
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_short((int)this.Name.Length);
				dst.Enc_ndr_short((int)this.Name.MaximumLength);
				dst.Enc_ndr_referent(this.Name.Buffer, 1);
				dst.Enc_ndr_referent(this.Sid, 1);
				bool flag = this.Name.Buffer != null;
				if (flag)
				{
					dst = dst.Deferred;
					int num = (int)(this.Name.Length / 2);
					int l = (int)(this.Name.MaximumLength / 2);
					dst.Enc_ndr_long(l);
					dst.Enc_ndr_long(0);
					dst.Enc_ndr_long(num);
					int index = dst.Index;
					dst.Advance(2 * num);
					dst = dst.Derive(index);
					for (int i = 0; i < num; i++)
					{
						dst.Enc_ndr_short((int)this.Name.Buffer[i]);
					}
				}
				bool flag2 = this.Sid != null;
				if (flag2)
				{
					dst = dst.Deferred;
					this.Sid.Encode(dst);
				}
			}

			// Token: 0x06000877 RID: 2167 RVA: 0x0002D6E0 File Offset: 0x0002B8E0
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				src.Align(4);
				bool flag = this.Name == null;
				if (flag)
				{
					this.Name = new Rpc.Unicode_string();
				}
				this.Name.Length = (short)src.Dec_ndr_short();
				this.Name.MaximumLength = (short)src.Dec_ndr_short();
				int num = src.Dec_ndr_long();
				int num2 = src.Dec_ndr_long();
				bool flag2 = num != 0;
				if (flag2)
				{
					src = src.Deferred;
					int num3 = src.Dec_ndr_long();
					src.Dec_ndr_long();
					int num4 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(2 * num4);
					bool flag3 = this.Name.Buffer == null;
					if (flag3)
					{
						bool flag4 = num3 < 0 || num3 > 65535;
						if (flag4)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Name.Buffer = new short[num3];
					}
					src = src.Derive(index);
					for (int i = 0; i < num4; i++)
					{
						this.Name.Buffer[i] = (short)src.Dec_ndr_short();
					}
				}
				bool flag5 = num2 != 0;
				if (flag5)
				{
					bool flag6 = this.Sid == null;
					if (flag6)
					{
						this.Sid = new Rpc.SidT();
					}
					src = src.Deferred;
					this.Sid.Decode(src);
				}
			}

			// Token: 0x040005EB RID: 1515
			public Rpc.Unicode_string Name;

			// Token: 0x040005EC RID: 1516
			public Rpc.SidT Sid;
		}

		// Token: 0x02000138 RID: 312
		public class LsarRefDomainList : NdrObject
		{
			// Token: 0x06000879 RID: 2169 RVA: 0x0002D84C File Offset: 0x0002BA4C
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.Domains, 1);
				dst.Enc_ndr_long(this.MaxCount);
				bool flag = this.Domains != null;
				if (flag)
				{
					dst = dst.Deferred;
					int count = this.Count;
					dst.Enc_ndr_long(count);
					int index = dst.Index;
					dst.Advance(12 * count);
					dst = dst.Derive(index);
					for (int i = 0; i < count; i++)
					{
						this.Domains[i].Encode(dst);
					}
				}
			}

			// Token: 0x0600087A RID: 2170 RVA: 0x0002D8F0 File Offset: 0x0002BAF0
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Count = src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				this.MaxCount = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					int num2 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(12 * num2);
					bool flag2 = this.Domains == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Domains = new Lsarpc.LsarTrustInformation[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.Domains[i] == null;
						if (flag4)
						{
							this.Domains[i] = new Lsarpc.LsarTrustInformation();
						}
						this.Domains[i].Decode(src);
					}
				}
			}

			// Token: 0x040005ED RID: 1517
			public int Count;

			// Token: 0x040005EE RID: 1518
			public Lsarpc.LsarTrustInformation[] Domains;

			// Token: 0x040005EF RID: 1519
			public int MaxCount;
		}

		// Token: 0x02000139 RID: 313
		public class LsarTranslatedName : NdrObject
		{
			// Token: 0x0600087C RID: 2172 RVA: 0x0002D9E4 File Offset: 0x0002BBE4
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_short((int)this.SidType);
				dst.Enc_ndr_short((int)this.Name.Length);
				dst.Enc_ndr_short((int)this.Name.MaximumLength);
				dst.Enc_ndr_referent(this.Name.Buffer, 1);
				dst.Enc_ndr_long(this.SidIndex);
				bool flag = this.Name.Buffer != null;
				if (flag)
				{
					dst = dst.Deferred;
					int num = (int)(this.Name.Length / 2);
					int l = (int)(this.Name.MaximumLength / 2);
					dst.Enc_ndr_long(l);
					dst.Enc_ndr_long(0);
					dst.Enc_ndr_long(num);
					int index = dst.Index;
					dst.Advance(2 * num);
					dst = dst.Derive(index);
					for (int i = 0; i < num; i++)
					{
						dst.Enc_ndr_short((int)this.Name.Buffer[i]);
					}
				}
			}

			// Token: 0x0600087D RID: 2173 RVA: 0x0002DAE8 File Offset: 0x0002BCE8
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.SidType = (short)src.Dec_ndr_short();
				src.Align(4);
				bool flag = this.Name == null;
				if (flag)
				{
					this.Name = new Rpc.Unicode_string();
				}
				this.Name.Length = (short)src.Dec_ndr_short();
				this.Name.MaximumLength = (short)src.Dec_ndr_short();
				int num = src.Dec_ndr_long();
				this.SidIndex = src.Dec_ndr_long();
				bool flag2 = num != 0;
				if (flag2)
				{
					src = src.Deferred;
					int num2 = src.Dec_ndr_long();
					src.Dec_ndr_long();
					int num3 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(2 * num3);
					bool flag3 = this.Name.Buffer == null;
					if (flag3)
					{
						bool flag4 = num2 < 0 || num2 > 65535;
						if (flag4)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Name.Buffer = new short[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num3; i++)
					{
						this.Name.Buffer[i] = (short)src.Dec_ndr_short();
					}
				}
			}

			// Token: 0x040005F0 RID: 1520
			public short SidType;

			// Token: 0x040005F1 RID: 1521
			public Rpc.Unicode_string Name;

			// Token: 0x040005F2 RID: 1522
			public int SidIndex;
		}

		// Token: 0x0200013A RID: 314
		public class LsarTransNameArray : NdrObject
		{
			// Token: 0x0600087F RID: 2175 RVA: 0x0002DC24 File Offset: 0x0002BE24
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.Names, 1);
				bool flag = this.Names != null;
				if (flag)
				{
					dst = dst.Deferred;
					int count = this.Count;
					dst.Enc_ndr_long(count);
					int index = dst.Index;
					dst.Advance(16 * count);
					dst = dst.Derive(index);
					for (int i = 0; i < count; i++)
					{
						this.Names[i].Encode(dst);
					}
				}
			}

			// Token: 0x06000880 RID: 2176 RVA: 0x0002DCBC File Offset: 0x0002BEBC
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Count = src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					int num2 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(16 * num2);
					bool flag2 = this.Names == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Names = new Lsarpc.LsarTranslatedName[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.Names[i] == null;
						if (flag4)
						{
							this.Names[i] = new Lsarpc.LsarTranslatedName();
						}
						this.Names[i].Decode(src);
					}
				}
			}

			// Token: 0x040005F3 RID: 1523
			public int Count;

			// Token: 0x040005F4 RID: 1524
			public Lsarpc.LsarTranslatedName[] Names;
		}

		// Token: 0x0200013B RID: 315
		public class LsarClose : DcerpcMessage
		{
			// Token: 0x06000882 RID: 2178 RVA: 0x0002DDA4 File Offset: 0x0002BFA4
			public override int GetOpnum()
			{
				return 0;
			}

			// Token: 0x06000883 RID: 2179 RVA: 0x0002DDB7 File Offset: 0x0002BFB7
			public LsarClose(Rpc.PolicyHandle handle)
			{
				this.Handle = handle;
			}

			// Token: 0x06000884 RID: 2180 RVA: 0x0002DDC8 File Offset: 0x0002BFC8
			public override void Encode_in(NdrBuffer dst)
			{
				this.Handle.Encode(dst);
			}

			// Token: 0x06000885 RID: 2181 RVA: 0x0002DDD8 File Offset: 0x0002BFD8
			public override void Decode_out(NdrBuffer src)
			{
				this.Handle.Decode(src);
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x040005F5 RID: 1525
			public int Retval;

			// Token: 0x040005F6 RID: 1526
			public Rpc.PolicyHandle Handle;
		}

		// Token: 0x0200013C RID: 316
		public class LsarQueryInformationPolicy : DcerpcMessage
		{
			// Token: 0x06000886 RID: 2182 RVA: 0x0002DDF4 File Offset: 0x0002BFF4
			public override int GetOpnum()
			{
				return 7;
			}

			// Token: 0x06000887 RID: 2183 RVA: 0x0002DE07 File Offset: 0x0002C007
			public LsarQueryInformationPolicy(Rpc.PolicyHandle handle, short level, NdrObject info)
			{
				this.Handle = handle;
				this.Level = level;
				this.Info = info;
			}

			// Token: 0x06000888 RID: 2184 RVA: 0x0002DE26 File Offset: 0x0002C026
			public override void Encode_in(NdrBuffer dst)
			{
				this.Handle.Encode(dst);
				dst.Enc_ndr_short((int)this.Level);
			}

			// Token: 0x06000889 RID: 2185 RVA: 0x0002DE44 File Offset: 0x0002C044
			public override void Decode_out(NdrBuffer src)
			{
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src.Dec_ndr_short();
					this.Info.Decode(src);
				}
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x040005F7 RID: 1527
			public int Retval;

			// Token: 0x040005F8 RID: 1528
			public Rpc.PolicyHandle Handle;

			// Token: 0x040005F9 RID: 1529
			public short Level;

			// Token: 0x040005FA RID: 1530
			public NdrObject Info;
		}

		// Token: 0x0200013D RID: 317
		public class LsarLookupSids : DcerpcMessage
		{
			// Token: 0x0600088A RID: 2186 RVA: 0x0002DE84 File Offset: 0x0002C084
			public override int GetOpnum()
			{
				return 15;
			}

			// Token: 0x0600088B RID: 2187 RVA: 0x0002DE98 File Offset: 0x0002C098
			public LsarLookupSids(Rpc.PolicyHandle handle, Lsarpc.LsarSidArray sids, Lsarpc.LsarRefDomainList domains, Lsarpc.LsarTransNameArray names, short level, int count)
			{
				this.Handle = handle;
				this.Sids = sids;
				this.Domains = domains;
				this.Names = names;
				this.Level = level;
				this.Count = count;
			}

			// Token: 0x0600088C RID: 2188 RVA: 0x0002DED0 File Offset: 0x0002C0D0
			public override void Encode_in(NdrBuffer dst)
			{
				this.Handle.Encode(dst);
				this.Sids.Encode(dst);
				this.Names.Encode(dst);
				dst.Enc_ndr_short((int)this.Level);
				dst.Enc_ndr_long(this.Count);
			}

			// Token: 0x0600088D RID: 2189 RVA: 0x0002DF20 File Offset: 0x0002C120
			public override void Decode_out(NdrBuffer src)
			{
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					bool flag2 = this.Domains == null;
					if (flag2)
					{
						this.Domains = new Lsarpc.LsarRefDomainList();
					}
					this.Domains.Decode(src);
				}
				this.Names.Decode(src);
				this.Count = src.Dec_ndr_long();
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x040005FB RID: 1531
			public int Retval;

			// Token: 0x040005FC RID: 1532
			public Rpc.PolicyHandle Handle;

			// Token: 0x040005FD RID: 1533
			public Lsarpc.LsarSidArray Sids;

			// Token: 0x040005FE RID: 1534
			public Lsarpc.LsarRefDomainList Domains;

			// Token: 0x040005FF RID: 1535
			public Lsarpc.LsarTransNameArray Names;

			// Token: 0x04000600 RID: 1536
			public short Level;

			// Token: 0x04000601 RID: 1537
			public int Count;
		}

		// Token: 0x0200013E RID: 318
		public class LsarOpenPolicy2 : DcerpcMessage
		{
			// Token: 0x0600088E RID: 2190 RVA: 0x0002DF8C File Offset: 0x0002C18C
			public override int GetOpnum()
			{
				return 44;
			}

			// Token: 0x0600088F RID: 2191 RVA: 0x0002DFA0 File Offset: 0x0002C1A0
			public LsarOpenPolicy2(string systemName, Lsarpc.LsarObjectAttributes objectAttributes, int desiredAccess, Rpc.PolicyHandle policyHandle)
			{
				this.SystemName = systemName;
				this.ObjectAttributes = objectAttributes;
				this.DesiredAccess = desiredAccess;
				this.PolicyHandle = policyHandle;
			}

			// Token: 0x06000890 RID: 2192 RVA: 0x0002DFC8 File Offset: 0x0002C1C8
			public override void Encode_in(NdrBuffer dst)
			{
				dst.Enc_ndr_referent(this.SystemName, 1);
				bool flag = this.SystemName != null;
				if (flag)
				{
					dst.Enc_ndr_string(this.SystemName);
				}
				this.ObjectAttributes.Encode(dst);
				dst.Enc_ndr_long(this.DesiredAccess);
			}

			// Token: 0x06000891 RID: 2193 RVA: 0x0002E01A File Offset: 0x0002C21A
			public override void Decode_out(NdrBuffer src)
			{
				this.PolicyHandle.Decode(src);
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000602 RID: 1538
			public int Retval;

			// Token: 0x04000603 RID: 1539
			public string SystemName;

			// Token: 0x04000604 RID: 1540
			public Lsarpc.LsarObjectAttributes ObjectAttributes;

			// Token: 0x04000605 RID: 1541
			public int DesiredAccess;

			// Token: 0x04000606 RID: 1542
			public Rpc.PolicyHandle PolicyHandle;
		}

		// Token: 0x0200013F RID: 319
		public class LsarQueryInformationPolicy2 : DcerpcMessage
		{
			// Token: 0x06000892 RID: 2194 RVA: 0x0002E038 File Offset: 0x0002C238
			public override int GetOpnum()
			{
				return 46;
			}

			// Token: 0x06000893 RID: 2195 RVA: 0x0002E04C File Offset: 0x0002C24C
			public LsarQueryInformationPolicy2(Rpc.PolicyHandle handle, short level, NdrObject info)
			{
				this.Handle = handle;
				this.Level = level;
				this.Info = info;
			}

			// Token: 0x06000894 RID: 2196 RVA: 0x0002E06B File Offset: 0x0002C26B
			public override void Encode_in(NdrBuffer dst)
			{
				this.Handle.Encode(dst);
				dst.Enc_ndr_short((int)this.Level);
			}

			// Token: 0x06000895 RID: 2197 RVA: 0x0002E088 File Offset: 0x0002C288
			public override void Decode_out(NdrBuffer src)
			{
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src.Dec_ndr_short();
					this.Info.Decode(src);
				}
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000607 RID: 1543
			public int Retval;

			// Token: 0x04000608 RID: 1544
			public Rpc.PolicyHandle Handle;

			// Token: 0x04000609 RID: 1545
			public short Level;

			// Token: 0x0400060A RID: 1546
			public NdrObject Info;
		}
	}
}
