using System;
using SharpCifs.Dcerpc.Ndr;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x02000104 RID: 260
	public class Srvsvc
	{
		// Token: 0x060007EE RID: 2030 RVA: 0x0002AE0C File Offset: 0x0002900C
		public static string GetSyntax()
		{
			return "4b324fc8-1670-01d3-1278-5a47bf6ee188:3.0";
		}

		// Token: 0x02000157 RID: 343
		public class ShareInfo0 : NdrObject
		{
			// Token: 0x060008E1 RID: 2273 RVA: 0x0002F724 File Offset: 0x0002D924
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_referent(this.Netname, 1);
				bool flag = this.Netname != null;
				if (flag)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Netname);
				}
			}

			// Token: 0x060008E2 RID: 2274 RVA: 0x0002F76C File Offset: 0x0002D96C
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.Netname = src.Dec_ndr_string();
				}
			}

			// Token: 0x0400064E RID: 1614
			public string Netname;
		}

		// Token: 0x02000158 RID: 344
		public class ShareInfoCtr0 : NdrObject
		{
			// Token: 0x060008E4 RID: 2276 RVA: 0x0002F7A8 File Offset: 0x0002D9A8
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.Array, 1);
				bool flag = this.Array != null;
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
						this.Array[i].Encode(dst);
					}
				}
			}

			// Token: 0x060008E5 RID: 2277 RVA: 0x0002F83C File Offset: 0x0002DA3C
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
					bool flag2 = this.Array == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Array = new Srvsvc.ShareInfo0[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.Array[i] == null;
						if (flag4)
						{
							this.Array[i] = new Srvsvc.ShareInfo0();
						}
						this.Array[i].Decode(src);
					}
				}
			}

			// Token: 0x0400064F RID: 1615
			public int Count;

			// Token: 0x04000650 RID: 1616
			public Srvsvc.ShareInfo0[] Array;
		}

		// Token: 0x02000159 RID: 345
		public class ShareInfo1 : NdrObject
		{
			// Token: 0x060008E7 RID: 2279 RVA: 0x0002F924 File Offset: 0x0002DB24
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_referent(this.Netname, 1);
				dst.Enc_ndr_long(this.Type);
				dst.Enc_ndr_referent(this.Remark, 1);
				bool flag = this.Netname != null;
				if (flag)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Netname);
				}
				bool flag2 = this.Remark != null;
				if (flag2)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Remark);
				}
			}

			// Token: 0x060008E8 RID: 2280 RVA: 0x0002F9AC File Offset: 0x0002DBAC
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				int num = src.Dec_ndr_long();
				this.Type = src.Dec_ndr_long();
				int num2 = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.Netname = src.Dec_ndr_string();
				}
				bool flag2 = num2 != 0;
				if (flag2)
				{
					src = src.Deferred;
					this.Remark = src.Dec_ndr_string();
				}
			}

			// Token: 0x04000651 RID: 1617
			public string Netname;

			// Token: 0x04000652 RID: 1618
			public int Type;

			// Token: 0x04000653 RID: 1619
			public string Remark;
		}

		// Token: 0x0200015A RID: 346
		public class ShareInfoCtr1 : NdrObject
		{
			// Token: 0x060008EA RID: 2282 RVA: 0x0002FA18 File Offset: 0x0002DC18
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.Array, 1);
				bool flag = this.Array != null;
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
						this.Array[i].Encode(dst);
					}
				}
			}

			// Token: 0x060008EB RID: 2283 RVA: 0x0002FAB0 File Offset: 0x0002DCB0
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
					bool flag2 = this.Array == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Array = new Srvsvc.ShareInfo1[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.Array[i] == null;
						if (flag4)
						{
							this.Array[i] = new Srvsvc.ShareInfo1();
						}
						this.Array[i].Decode(src);
					}
				}
			}

			// Token: 0x04000654 RID: 1620
			public int Count;

			// Token: 0x04000655 RID: 1621
			public Srvsvc.ShareInfo1[] Array;
		}

		// Token: 0x0200015B RID: 347
		public class ShareInfo502 : NdrObject
		{
			// Token: 0x060008ED RID: 2285 RVA: 0x0002FB98 File Offset: 0x0002DD98
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_referent(this.Netname, 1);
				dst.Enc_ndr_long(this.Type);
				dst.Enc_ndr_referent(this.Remark, 1);
				dst.Enc_ndr_long(this.Permissions);
				dst.Enc_ndr_long(this.MaxUses);
				dst.Enc_ndr_long(this.CurrentUses);
				dst.Enc_ndr_referent(this.Path, 1);
				dst.Enc_ndr_referent(this.Password, 1);
				dst.Enc_ndr_long(this.SdSize);
				dst.Enc_ndr_referent(this.SecurityDescriptor, 1);
				bool flag = this.Netname != null;
				if (flag)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Netname);
				}
				bool flag2 = this.Remark != null;
				if (flag2)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Remark);
				}
				bool flag3 = this.Path != null;
				if (flag3)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Path);
				}
				bool flag4 = this.Password != null;
				if (flag4)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Password);
				}
				bool flag5 = this.SecurityDescriptor != null;
				if (flag5)
				{
					dst = dst.Deferred;
					int sdSize = this.SdSize;
					dst.Enc_ndr_long(sdSize);
					int index = dst.Index;
					dst.Advance(sdSize);
					dst = dst.Derive(index);
					for (int i = 0; i < sdSize; i++)
					{
						dst.Enc_ndr_small((int)this.SecurityDescriptor[i]);
					}
				}
			}

			// Token: 0x060008EE RID: 2286 RVA: 0x0002FD34 File Offset: 0x0002DF34
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				int num = src.Dec_ndr_long();
				this.Type = src.Dec_ndr_long();
				int num2 = src.Dec_ndr_long();
				this.Permissions = src.Dec_ndr_long();
				this.MaxUses = src.Dec_ndr_long();
				this.CurrentUses = src.Dec_ndr_long();
				int num3 = src.Dec_ndr_long();
				int num4 = src.Dec_ndr_long();
				this.SdSize = src.Dec_ndr_long();
				int num5 = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.Netname = src.Dec_ndr_string();
				}
				bool flag2 = num2 != 0;
				if (flag2)
				{
					src = src.Deferred;
					this.Remark = src.Dec_ndr_string();
				}
				bool flag3 = num3 != 0;
				if (flag3)
				{
					src = src.Deferred;
					this.Path = src.Dec_ndr_string();
				}
				bool flag4 = num4 != 0;
				if (flag4)
				{
					src = src.Deferred;
					this.Password = src.Dec_ndr_string();
				}
				bool flag5 = num5 != 0;
				if (flag5)
				{
					src = src.Deferred;
					int num6 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(num6);
					bool flag6 = this.SecurityDescriptor == null;
					if (flag6)
					{
						bool flag7 = num6 < 0 || num6 > 65535;
						if (flag7)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.SecurityDescriptor = new byte[num6];
					}
					src = src.Derive(index);
					for (int i = 0; i < num6; i++)
					{
						this.SecurityDescriptor[i] = (byte)src.Dec_ndr_small();
					}
				}
			}

			// Token: 0x04000656 RID: 1622
			public string Netname;

			// Token: 0x04000657 RID: 1623
			public int Type;

			// Token: 0x04000658 RID: 1624
			public string Remark;

			// Token: 0x04000659 RID: 1625
			public int Permissions;

			// Token: 0x0400065A RID: 1626
			public int MaxUses;

			// Token: 0x0400065B RID: 1627
			public int CurrentUses;

			// Token: 0x0400065C RID: 1628
			public string Path;

			// Token: 0x0400065D RID: 1629
			public string Password;

			// Token: 0x0400065E RID: 1630
			public int SdSize;

			// Token: 0x0400065F RID: 1631
			public byte[] SecurityDescriptor;
		}

		// Token: 0x0200015C RID: 348
		public class ShareInfoCtr502 : NdrObject
		{
			// Token: 0x060008F0 RID: 2288 RVA: 0x0002FED0 File Offset: 0x0002E0D0
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.Array, 1);
				bool flag = this.Array != null;
				if (flag)
				{
					dst = dst.Deferred;
					int count = this.Count;
					dst.Enc_ndr_long(count);
					int index = dst.Index;
					dst.Advance(40 * count);
					dst = dst.Derive(index);
					for (int i = 0; i < count; i++)
					{
						this.Array[i].Encode(dst);
					}
				}
			}

			// Token: 0x060008F1 RID: 2289 RVA: 0x0002FF68 File Offset: 0x0002E168
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
					src.Advance(40 * num2);
					bool flag2 = this.Array == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Array = new Srvsvc.ShareInfo502[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.Array[i] == null;
						if (flag4)
						{
							this.Array[i] = new Srvsvc.ShareInfo502();
						}
						this.Array[i].Decode(src);
					}
				}
			}

			// Token: 0x04000660 RID: 1632
			public int Count;

			// Token: 0x04000661 RID: 1633
			public Srvsvc.ShareInfo502[] Array;
		}

		// Token: 0x0200015D RID: 349
		public class ShareEnumAll : DcerpcMessage
		{
			// Token: 0x060008F3 RID: 2291 RVA: 0x00030050 File Offset: 0x0002E250
			public override int GetOpnum()
			{
				return 15;
			}

			// Token: 0x060008F4 RID: 2292 RVA: 0x00030064 File Offset: 0x0002E264
			public ShareEnumAll(string servername, int level, NdrObject info, int prefmaxlen, int totalentries, int resumeHandle)
			{
				this.Servername = servername;
				this.Level = level;
				this.Info = info;
				this.Prefmaxlen = prefmaxlen;
				this.Totalentries = totalentries;
				this.ResumeHandle = resumeHandle;
			}

			// Token: 0x060008F5 RID: 2293 RVA: 0x0003009C File Offset: 0x0002E29C
			public override void Encode_in(NdrBuffer dst)
			{
				dst.Enc_ndr_referent(this.Servername, 1);
				bool flag = this.Servername != null;
				if (flag)
				{
					dst.Enc_ndr_string(this.Servername);
				}
				dst.Enc_ndr_long(this.Level);
				int level = this.Level;
				dst.Enc_ndr_long(level);
				dst.Enc_ndr_referent(this.Info, 1);
				bool flag2 = this.Info != null;
				if (flag2)
				{
					dst = dst.Deferred;
					this.Info.Encode(dst);
				}
				dst.Enc_ndr_long(this.Prefmaxlen);
				dst.Enc_ndr_long(this.ResumeHandle);
			}

			// Token: 0x060008F6 RID: 2294 RVA: 0x0003013C File Offset: 0x0002E33C
			public override void Decode_out(NdrBuffer src)
			{
				this.Level = src.Dec_ndr_long();
				src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					bool flag2 = this.Info == null;
					if (flag2)
					{
						this.Info = new Srvsvc.ShareInfoCtr0();
					}
					src = src.Deferred;
					this.Info.Decode(src);
				}
				this.Totalentries = src.Dec_ndr_long();
				this.ResumeHandle = src.Dec_ndr_long();
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000662 RID: 1634
			public int Retval;

			// Token: 0x04000663 RID: 1635
			public string Servername;

			// Token: 0x04000664 RID: 1636
			public int Level;

			// Token: 0x04000665 RID: 1637
			public NdrObject Info;

			// Token: 0x04000666 RID: 1638
			public int Prefmaxlen;

			// Token: 0x04000667 RID: 1639
			public int Totalentries;

			// Token: 0x04000668 RID: 1640
			public int ResumeHandle;
		}

		// Token: 0x0200015E RID: 350
		public class ShareGetInfo : DcerpcMessage
		{
			// Token: 0x060008F7 RID: 2295 RVA: 0x000301C4 File Offset: 0x0002E3C4
			public override int GetOpnum()
			{
				return 16;
			}

			// Token: 0x060008F8 RID: 2296 RVA: 0x000301D8 File Offset: 0x0002E3D8
			public ShareGetInfo(string servername, string sharename, int level, NdrObject info)
			{
				this.Servername = servername;
				this.Sharename = sharename;
				this.Level = level;
				this.Info = info;
			}

			// Token: 0x060008F9 RID: 2297 RVA: 0x00030200 File Offset: 0x0002E400
			public override void Encode_in(NdrBuffer dst)
			{
				dst.Enc_ndr_referent(this.Servername, 1);
				bool flag = this.Servername != null;
				if (flag)
				{
					dst.Enc_ndr_string(this.Servername);
				}
				dst.Enc_ndr_string(this.Sharename);
				dst.Enc_ndr_long(this.Level);
			}

			// Token: 0x060008FA RID: 2298 RVA: 0x00030254 File Offset: 0x0002E454
			public override void Decode_out(NdrBuffer src)
			{
				src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					bool flag2 = this.Info == null;
					if (flag2)
					{
						this.Info = new Srvsvc.ShareInfo0();
					}
					src = src.Deferred;
					this.Info.Decode(src);
				}
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000669 RID: 1641
			public int Retval;

			// Token: 0x0400066A RID: 1642
			public string Servername;

			// Token: 0x0400066B RID: 1643
			public string Sharename;

			// Token: 0x0400066C RID: 1644
			public int Level;

			// Token: 0x0400066D RID: 1645
			public NdrObject Info;
		}

		// Token: 0x0200015F RID: 351
		public class ServerInfo100 : NdrObject
		{
			// Token: 0x060008FB RID: 2299 RVA: 0x000302B8 File Offset: 0x0002E4B8
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.PlatformId);
				dst.Enc_ndr_referent(this.Name, 1);
				bool flag = this.Name != null;
				if (flag)
				{
					dst = dst.Deferred;
					dst.Enc_ndr_string(this.Name);
				}
			}

			// Token: 0x060008FC RID: 2300 RVA: 0x00030310 File Offset: 0x0002E510
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.PlatformId = src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					this.Name = src.Dec_ndr_string();
				}
			}

			// Token: 0x0400066E RID: 1646
			public int PlatformId;

			// Token: 0x0400066F RID: 1647
			public string Name;
		}

		// Token: 0x02000160 RID: 352
		public class ServerGetInfo : DcerpcMessage
		{
			// Token: 0x060008FE RID: 2302 RVA: 0x00030358 File Offset: 0x0002E558
			public override int GetOpnum()
			{
				return 21;
			}

			// Token: 0x060008FF RID: 2303 RVA: 0x0003036C File Offset: 0x0002E56C
			public ServerGetInfo(string servername, int level, NdrObject info)
			{
				this.Servername = servername;
				this.Level = level;
				this.Info = info;
			}

			// Token: 0x06000900 RID: 2304 RVA: 0x0003038C File Offset: 0x0002E58C
			public override void Encode_in(NdrBuffer dst)
			{
				dst.Enc_ndr_referent(this.Servername, 1);
				bool flag = this.Servername != null;
				if (flag)
				{
					dst.Enc_ndr_string(this.Servername);
				}
				dst.Enc_ndr_long(this.Level);
			}

			// Token: 0x06000901 RID: 2305 RVA: 0x000303D4 File Offset: 0x0002E5D4
			public override void Decode_out(NdrBuffer src)
			{
				src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					bool flag2 = this.Info == null;
					if (flag2)
					{
						this.Info = new Srvsvc.ServerInfo100();
					}
					src = src.Deferred;
					this.Info.Decode(src);
				}
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000670 RID: 1648
			public int Retval;

			// Token: 0x04000671 RID: 1649
			public string Servername;

			// Token: 0x04000672 RID: 1650
			public int Level;

			// Token: 0x04000673 RID: 1651
			public NdrObject Info;
		}

		// Token: 0x02000161 RID: 353
		public class TimeOfDayInfo : NdrObject
		{
			// Token: 0x06000902 RID: 2306 RVA: 0x00030438 File Offset: 0x0002E638
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Elapsedt);
				dst.Enc_ndr_long(this.Msecs);
				dst.Enc_ndr_long(this.Hours);
				dst.Enc_ndr_long(this.Mins);
				dst.Enc_ndr_long(this.Secs);
				dst.Enc_ndr_long(this.Hunds);
				dst.Enc_ndr_long(this.Timezone);
				dst.Enc_ndr_long(this.Tinterval);
				dst.Enc_ndr_long(this.Day);
				dst.Enc_ndr_long(this.Month);
				dst.Enc_ndr_long(this.Year);
				dst.Enc_ndr_long(this.Weekday);
			}

			// Token: 0x06000903 RID: 2307 RVA: 0x000304EC File Offset: 0x0002E6EC
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Elapsedt = src.Dec_ndr_long();
				this.Msecs = src.Dec_ndr_long();
				this.Hours = src.Dec_ndr_long();
				this.Mins = src.Dec_ndr_long();
				this.Secs = src.Dec_ndr_long();
				this.Hunds = src.Dec_ndr_long();
				this.Timezone = src.Dec_ndr_long();
				this.Tinterval = src.Dec_ndr_long();
				this.Day = src.Dec_ndr_long();
				this.Month = src.Dec_ndr_long();
				this.Year = src.Dec_ndr_long();
				this.Weekday = src.Dec_ndr_long();
			}

			// Token: 0x04000674 RID: 1652
			public int Elapsedt;

			// Token: 0x04000675 RID: 1653
			public int Msecs;

			// Token: 0x04000676 RID: 1654
			public int Hours;

			// Token: 0x04000677 RID: 1655
			public int Mins;

			// Token: 0x04000678 RID: 1656
			public int Secs;

			// Token: 0x04000679 RID: 1657
			public int Hunds;

			// Token: 0x0400067A RID: 1658
			public int Timezone;

			// Token: 0x0400067B RID: 1659
			public int Tinterval;

			// Token: 0x0400067C RID: 1660
			public int Day;

			// Token: 0x0400067D RID: 1661
			public int Month;

			// Token: 0x0400067E RID: 1662
			public int Year;

			// Token: 0x0400067F RID: 1663
			public int Weekday;
		}

		// Token: 0x02000162 RID: 354
		public class RemoteTod : DcerpcMessage
		{
			// Token: 0x06000905 RID: 2309 RVA: 0x00030594 File Offset: 0x0002E794
			public override int GetOpnum()
			{
				return 28;
			}

			// Token: 0x06000906 RID: 2310 RVA: 0x000305A8 File Offset: 0x0002E7A8
			public RemoteTod(string servername, Srvsvc.TimeOfDayInfo info)
			{
				this.Servername = servername;
				this.Info = info;
			}

			// Token: 0x06000907 RID: 2311 RVA: 0x000305C0 File Offset: 0x0002E7C0
			public override void Encode_in(NdrBuffer dst)
			{
				dst.Enc_ndr_referent(this.Servername, 1);
				bool flag = this.Servername != null;
				if (flag)
				{
					dst.Enc_ndr_string(this.Servername);
				}
			}

			// Token: 0x06000908 RID: 2312 RVA: 0x000305F8 File Offset: 0x0002E7F8
			public override void Decode_out(NdrBuffer src)
			{
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					bool flag2 = this.Info == null;
					if (flag2)
					{
						this.Info = new Srvsvc.TimeOfDayInfo();
					}
					this.Info.Decode(src);
				}
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000680 RID: 1664
			public int Retval;

			// Token: 0x04000681 RID: 1665
			public string Servername;

			// Token: 0x04000682 RID: 1666
			public Srvsvc.TimeOfDayInfo Info;
		}
	}
}
