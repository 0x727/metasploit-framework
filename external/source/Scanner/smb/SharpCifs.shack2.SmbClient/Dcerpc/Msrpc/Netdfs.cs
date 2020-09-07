using System;
using SharpCifs.Dcerpc.Ndr;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000FF RID: 255
	public class Netdfs
	{
		// Token: 0x060007E4 RID: 2020 RVA: 0x0002ACD8 File Offset: 0x00028ED8
		public static string GetSyntax()
		{
			return "4fc742e0-4a10-11cf-8273-00aa004ae673:3.0";
		}

		// Token: 0x0400050C RID: 1292
		public const int DfsVolumeFlavorStandalone = 256;

		// Token: 0x0400050D RID: 1293
		public const int DfsVolumeFlavorAdBlob = 512;

		// Token: 0x0400050E RID: 1294
		public const int DfsStorageStateOffline = 1;

		// Token: 0x0400050F RID: 1295
		public const int DfsStorageStateOnline = 2;

		// Token: 0x04000510 RID: 1296
		public const int DfsStorageStateActive = 4;

		// Token: 0x02000141 RID: 321
		public class DfsInfo1 : NdrObject
		{
			// Token: 0x06000897 RID: 2199 RVA: 0x0002E0FC File Offset: 0x0002C2FC
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_referent(this.EntryPath, 1);
				bool flag = this.EntryPath != null;
				if (flag)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.EntryPath);
				}
			}

			// Token: 0x06000898 RID: 2200 RVA: 0x0002E144 File Offset: 0x0002C344
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.EntryPath = src.Dec_ndr_string();
				}
			}

			// Token: 0x0400060C RID: 1548
			public string EntryPath;
		}

		// Token: 0x02000142 RID: 322
		public class DfsEnumArray1 : NdrObject
		{
			// Token: 0x0600089A RID: 2202 RVA: 0x0002E180 File Offset: 0x0002C380
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.S, 1);
				bool flag = this.S != null;
				if (flag)
				{
					dst = dst.Deferred;
					int count = this.Count;
					dst.Enc_ndr_long(count);
					int index = dst.Index;
					dst.Advance(4 * count);
					dst = dst.Derive(index);
					for (int i = 0; i < count; i++)
					{
						this.S[i].Encode(dst);
					}
				}
			}

			// Token: 0x0600089B RID: 2203 RVA: 0x0002E214 File Offset: 0x0002C414
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
					src.Advance(4 * num2);
					bool flag2 = this.S == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.S = new Netdfs.DfsInfo1[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.S[i] == null;
						if (flag4)
						{
							this.S[i] = new Netdfs.DfsInfo1();
						}
						this.S[i].Decode(src);
					}
				}
			}

			// Token: 0x0400060D RID: 1549
			public int Count;

			// Token: 0x0400060E RID: 1550
			public Netdfs.DfsInfo1[] S;
		}

		// Token: 0x02000143 RID: 323
		public class DfsStorageInfo : NdrObject
		{
			// Token: 0x0600089D RID: 2205 RVA: 0x0002E2FC File Offset: 0x0002C4FC
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.State);
				dst.Enc_ndr_referent(this.ServerName, 1);
				dst.Enc_ndr_referent(this.ShareName, 1);
				bool flag = this.ServerName != null;
				if (flag)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.ServerName);
				}
				bool flag2 = this.ShareName != null;
				if (flag2)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.ShareName);
				}
			}

			// Token: 0x0600089E RID: 2206 RVA: 0x0002E384 File Offset: 0x0002C584
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.State = src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				int num2 = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.ServerName = src.Dec_ndr_string();
				}
				bool flag2 = num2 != 0;
				if (flag2)
				{
					src = src.Deferred;
					this.ShareName = src.Dec_ndr_string();
				}
			}

			// Token: 0x0400060F RID: 1551
			public int State;

			// Token: 0x04000610 RID: 1552
			public string ServerName;

			// Token: 0x04000611 RID: 1553
			public string ShareName;
		}

		// Token: 0x02000144 RID: 324
		public class DfsInfo3 : NdrObject
		{
			// Token: 0x060008A0 RID: 2208 RVA: 0x0002E3F0 File Offset: 0x0002C5F0
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_referent(this.Path, 1);
				dst.Enc_ndr_referent(this.Comment, 1);
				dst.Enc_ndr_long(this.State);
				dst.Enc_ndr_long(this.NumStores);
				dst.Enc_ndr_referent(this.Stores, 1);
				bool flag = this.Path != null;
				if (flag)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Path);
				}
				bool flag2 = this.Comment != null;
				if (flag2)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Comment);
				}
				bool flag3 = this.Stores != null;
				if (flag3)
				{
					dst = dst.Deferred;
					int numStores = this.NumStores;
					dst.Enc_ndr_long(numStores);
					int index = dst.Index;
					dst.Advance(12 * numStores);
					dst = dst.Derive(index);
					for (int i = 0; i < numStores; i++)
					{
						this.Stores[i].Encode(dst);
					}
				}
			}

			// Token: 0x060008A1 RID: 2209 RVA: 0x0002E500 File Offset: 0x0002C700
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				int num = src.Dec_ndr_long();
				int num2 = src.Dec_ndr_long();
				this.State = src.Dec_ndr_long();
				this.NumStores = src.Dec_ndr_long();
				int num3 = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.Path = src.Dec_ndr_string();
				}
				bool flag2 = num2 != 0;
				if (flag2)
				{
					src = src.Deferred;
					this.Comment = src.Dec_ndr_string();
				}
				bool flag3 = num3 != 0;
				if (flag3)
				{
					src = src.Deferred;
					int num4 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(12 * num4);
					bool flag4 = this.Stores == null;
					if (flag4)
					{
						bool flag5 = num4 < 0 || num4 > 65535;
						if (flag5)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Stores = new Netdfs.DfsStorageInfo[num4];
					}
					src = src.Derive(index);
					for (int i = 0; i < num4; i++)
					{
						bool flag6 = this.Stores[i] == null;
						if (flag6)
						{
							this.Stores[i] = new Netdfs.DfsStorageInfo();
						}
						this.Stores[i].Decode(src);
					}
				}
			}

			// Token: 0x04000612 RID: 1554
			public string Path;

			// Token: 0x04000613 RID: 1555
			public string Comment;

			// Token: 0x04000614 RID: 1556
			public int State;

			// Token: 0x04000615 RID: 1557
			public int NumStores;

			// Token: 0x04000616 RID: 1558
			public Netdfs.DfsStorageInfo[] Stores;
		}

		// Token: 0x02000145 RID: 325
		public class DfsEnumArray3 : NdrObject
		{
			// Token: 0x060008A3 RID: 2211 RVA: 0x0002E64C File Offset: 0x0002C84C
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.S, 1);
				bool flag = this.S != null;
				if (flag)
				{
					dst = dst.Deferred;
					int count = this.Count;
					dst.Enc_ndr_long(count);
					int index = dst.Index;
					dst.Advance(20 * count);
					dst = dst.Derive(index);
					for (int i = 0; i < count; i++)
					{
						this.S[i].Encode(dst);
					}
				}
			}

			// Token: 0x060008A4 RID: 2212 RVA: 0x0002E6E4 File Offset: 0x0002C8E4
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
					src.Advance(20 * num2);
					bool flag2 = this.S == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.S = new Netdfs.DfsInfo3[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.S[i] == null;
						if (flag4)
						{
							this.S[i] = new Netdfs.DfsInfo3();
						}
						this.S[i].Decode(src);
					}
				}
			}

			// Token: 0x04000617 RID: 1559
			public int Count;

			// Token: 0x04000618 RID: 1560
			public Netdfs.DfsInfo3[] S;
		}

		// Token: 0x02000146 RID: 326
		public class DfsInfo200 : NdrObject
		{
			// Token: 0x060008A6 RID: 2214 RVA: 0x0002E7CC File Offset: 0x0002C9CC
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_referent(this.DfsName, 1);
				bool flag = this.DfsName != null;
				if (flag)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.DfsName);
				}
			}

			// Token: 0x060008A7 RID: 2215 RVA: 0x0002E814 File Offset: 0x0002CA14
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.DfsName = src.Dec_ndr_string();
				}
			}

			// Token: 0x04000619 RID: 1561
			public string DfsName;
		}

		// Token: 0x02000147 RID: 327
		public class DfsEnumArray200 : NdrObject
		{
			// Token: 0x060008A9 RID: 2217 RVA: 0x0002E850 File Offset: 0x0002CA50
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.S, 1);
				bool flag = this.S != null;
				if (flag)
				{
					dst = dst.Deferred;
					int count = this.Count;
					dst.Enc_ndr_long(count);
					int index = dst.Index;
					dst.Advance(4 * count);
					dst = dst.Derive(index);
					for (int i = 0; i < count; i++)
					{
						this.S[i].Encode(dst);
					}
				}
			}

			// Token: 0x060008AA RID: 2218 RVA: 0x0002E8E4 File Offset: 0x0002CAE4
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
					src.Advance(4 * num2);
					bool flag2 = this.S == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.S = new Netdfs.DfsInfo200[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.S[i] == null;
						if (flag4)
						{
							this.S[i] = new Netdfs.DfsInfo200();
						}
						this.S[i].Decode(src);
					}
				}
			}

			// Token: 0x0400061A RID: 1562
			public int Count;

			// Token: 0x0400061B RID: 1563
			public Netdfs.DfsInfo200[] S;
		}

		// Token: 0x02000148 RID: 328
		public class DfsInfo300 : NdrObject
		{
			// Token: 0x060008AC RID: 2220 RVA: 0x0002E9CC File Offset: 0x0002CBCC
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Flags);
				dst.Enc_ndr_referent(this.DfsName, 1);
				bool flag = this.DfsName != null;
				if (flag)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.DfsName);
				}
			}

			// Token: 0x060008AD RID: 2221 RVA: 0x0002EA24 File Offset: 0x0002CC24
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Flags = src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.DfsName = src.Dec_ndr_string();
				}
			}

			// Token: 0x0400061C RID: 1564
			public int Flags;

			// Token: 0x0400061D RID: 1565
			public string DfsName;
		}

		// Token: 0x02000149 RID: 329
		public class DfsEnumArray300 : NdrObject
		{
			// Token: 0x060008AF RID: 2223 RVA: 0x0002EA6C File Offset: 0x0002CC6C
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.S, 1);
				bool flag = this.S != null;
				if (flag)
				{
					dst = dst.Deferred;
					int count = this.Count;
					dst.Enc_ndr_long(count);
					int index = dst.Index;
					dst.Advance(8 * count);
					dst = dst.Derive(index);
					for (int i = 0; i < count; i++)
					{
						this.S[i].Encode(dst);
					}
				}
			}

			// Token: 0x060008B0 RID: 2224 RVA: 0x0002EB00 File Offset: 0x0002CD00
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
					src.Advance(8 * num2);
					bool flag2 = this.S == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.S = new Netdfs.DfsInfo300[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.S[i] == null;
						if (flag4)
						{
							this.S[i] = new Netdfs.DfsInfo300();
						}
						this.S[i].Decode(src);
					}
				}
			}

			// Token: 0x0400061E RID: 1566
			public int Count;

			// Token: 0x0400061F RID: 1567
			public Netdfs.DfsInfo300[] S;
		}

		// Token: 0x0200014A RID: 330
		public class DfsEnumStruct : NdrObject
		{
			// Token: 0x060008B2 RID: 2226 RVA: 0x0002EBE8 File Offset: 0x0002CDE8
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Level);
				int level = this.Level;
				dst.Enc_ndr_long(level);
				dst.Enc_ndr_referent(this.E, 1);
				bool flag = this.E != null;
				if (flag)
				{
					dst = dst.Deferred;
					this.E.Encode(dst);
				}
			}

			// Token: 0x060008B3 RID: 2227 RVA: 0x0002EC4C File Offset: 0x0002CE4C
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Level = src.Dec_ndr_long();
				src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					bool flag2 = this.E == null;
					if (flag2)
					{
						this.E = new Netdfs.DfsEnumArray1();
					}
					src = src.Deferred;
					this.E.Decode(src);
				}
			}

			// Token: 0x04000620 RID: 1568
			public int Level;

			// Token: 0x04000621 RID: 1569
			public NdrObject E;
		}

		// Token: 0x0200014B RID: 331
		public class NetrDfsEnumEx : DcerpcMessage
		{
			// Token: 0x060008B5 RID: 2229 RVA: 0x0002ECB8 File Offset: 0x0002CEB8
			public override int GetOpnum()
			{
				return 21;
			}

			// Token: 0x060008B6 RID: 2230 RVA: 0x0002ECCC File Offset: 0x0002CECC
			public NetrDfsEnumEx(string dfsName, int level, int prefmaxlen, Netdfs.DfsEnumStruct info, NdrLong totalentries)
			{
				this.DfsName = dfsName;
				this.Level = level;
				this.Prefmaxlen = prefmaxlen;
				this.Info = info;
				this.Totalentries = totalentries;
			}

			// Token: 0x060008B7 RID: 2231 RVA: 0x0002ECFC File Offset: 0x0002CEFC
			public override void Encode_in(NdrBuffer dst)
			{
				dst.Enc_ndr_string(this.DfsName);
				dst.Enc_ndr_long(this.Level);
				dst.Enc_ndr_long(this.Prefmaxlen);
				dst.Enc_ndr_referent(this.Info, 1);
				bool flag = this.Info != null;
				if (flag)
				{
					this.Info.Encode(dst);
				}
				dst.Enc_ndr_referent(this.Totalentries, 1);
				bool flag2 = this.Totalentries != null;
				if (flag2)
				{
					this.Totalentries.Encode(dst);
				}
			}

			// Token: 0x060008B8 RID: 2232 RVA: 0x0002ED88 File Offset: 0x0002CF88
			public override void Decode_out(NdrBuffer src)
			{
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					bool flag2 = this.Info == null;
					if (flag2)
					{
						this.Info = new Netdfs.DfsEnumStruct();
					}
					this.Info.Decode(src);
				}
				int num2 = src.Dec_ndr_long();
				bool flag3 = num2 != 0;
				if (flag3)
				{
					this.Totalentries.Decode(src);
				}
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000622 RID: 1570
			public int Retval;

			// Token: 0x04000623 RID: 1571
			public string DfsName;

			// Token: 0x04000624 RID: 1572
			public int Level;

			// Token: 0x04000625 RID: 1573
			public int Prefmaxlen;

			// Token: 0x04000626 RID: 1574
			public Netdfs.DfsEnumStruct Info;

			// Token: 0x04000627 RID: 1575
			public NdrLong Totalentries;
		}
	}
}
