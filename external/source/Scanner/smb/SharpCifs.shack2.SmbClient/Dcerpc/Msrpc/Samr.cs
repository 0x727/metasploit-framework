using System;
using SharpCifs.Dcerpc.Ndr;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x02000100 RID: 256
	public class Samr
	{
		// Token: 0x060007E6 RID: 2022 RVA: 0x0002ACF0 File Offset: 0x00028EF0
		public static string GetSyntax()
		{
			return "12345778-1234-abcd-ef00-0123456789ac:1.0";
		}

		// Token: 0x04000511 RID: 1297
		public const int AcbDisabled = 1;

		// Token: 0x04000512 RID: 1298
		public const int AcbHomdirreq = 2;

		// Token: 0x04000513 RID: 1299
		public const int AcbPwnotreq = 4;

		// Token: 0x04000514 RID: 1300
		public const int AcbTempdup = 8;

		// Token: 0x04000515 RID: 1301
		public const int AcbNormal = 16;

		// Token: 0x04000516 RID: 1302
		public const int AcbMns = 32;

		// Token: 0x04000517 RID: 1303
		public const int AcbDomtrust = 64;

		// Token: 0x04000518 RID: 1304
		public const int AcbWstrust = 128;

		// Token: 0x04000519 RID: 1305
		public const int AcbSvrtrust = 256;

		// Token: 0x0400051A RID: 1306
		public const int AcbPwnoexp = 512;

		// Token: 0x0400051B RID: 1307
		public const int AcbAutolock = 1024;

		// Token: 0x0400051C RID: 1308
		public const int AcbEncTxtPwdAllowed = 2048;

		// Token: 0x0400051D RID: 1309
		public const int AcbSmartcardRequired = 4096;

		// Token: 0x0400051E RID: 1310
		public const int AcbTrustedForDelegation = 8192;

		// Token: 0x0400051F RID: 1311
		public const int AcbNotDelegated = 16384;

		// Token: 0x04000520 RID: 1312
		public const int AcbUseDesKeyOnly = 32768;

		// Token: 0x04000521 RID: 1313
		public const int AcbDontRequirePreauth = 65536;

		// Token: 0x04000522 RID: 1314
		public const int SeGroupMandatory = 1;

		// Token: 0x04000523 RID: 1315
		public const int SeGroupEnabledByDefault = 2;

		// Token: 0x04000524 RID: 1316
		public const int SeGroupEnabled = 4;

		// Token: 0x04000525 RID: 1317
		public const int SeGroupOwner = 8;

		// Token: 0x04000526 RID: 1318
		public const int SeGroupUseForDenyOnly = 16;

		// Token: 0x04000527 RID: 1319
		public const int SeGroupResource = 536870912;

		// Token: 0x04000528 RID: 1320
		public const int SeGroupLogonId = -1073741824;

		// Token: 0x0200014C RID: 332
		public class SamrCloseHandle : DcerpcMessage
		{
			// Token: 0x060008B9 RID: 2233 RVA: 0x0002EDFC File Offset: 0x0002CFFC
			public override int GetOpnum()
			{
				return 1;
			}

			// Token: 0x060008BA RID: 2234 RVA: 0x0002EE0F File Offset: 0x0002D00F
			public SamrCloseHandle(Rpc.PolicyHandle handle)
			{
				this.Handle = handle;
			}

			// Token: 0x060008BB RID: 2235 RVA: 0x0002EE20 File Offset: 0x0002D020
			public override void Encode_in(NdrBuffer dst)
			{
				this.Handle.Encode(dst);
			}

			// Token: 0x060008BC RID: 2236 RVA: 0x0002EE30 File Offset: 0x0002D030
			public override void Decode_out(NdrBuffer src)
			{
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000628 RID: 1576
			public int Retval;

			// Token: 0x04000629 RID: 1577
			public Rpc.PolicyHandle Handle;
		}

		// Token: 0x0200014D RID: 333
		public class SamrConnect2 : DcerpcMessage
		{
			// Token: 0x060008BD RID: 2237 RVA: 0x0002EE40 File Offset: 0x0002D040
			public override int GetOpnum()
			{
				return 57;
			}

			// Token: 0x060008BE RID: 2238 RVA: 0x0002EE54 File Offset: 0x0002D054
			public SamrConnect2(string systemName, int accessMask, Rpc.PolicyHandle handle)
			{
				this.SystemName = systemName;
				this.AccessMask = accessMask;
				this.Handle = handle;
			}

			// Token: 0x060008BF RID: 2239 RVA: 0x0002EE74 File Offset: 0x0002D074
			public override void Encode_in(NdrBuffer dst)
			{
				dst.Enc_ndr_referent(this.SystemName, 1);
				bool flag = this.SystemName != null;
				if (flag)
				{
					dst.Enc_ndr_string(this.SystemName);
				}
				dst.Enc_ndr_long(this.AccessMask);
			}

			// Token: 0x060008C0 RID: 2240 RVA: 0x0002EEB9 File Offset: 0x0002D0B9
			public override void Decode_out(NdrBuffer src)
			{
				this.Handle.Decode(src);
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x0400062A RID: 1578
			public int Retval;

			// Token: 0x0400062B RID: 1579
			public string SystemName;

			// Token: 0x0400062C RID: 1580
			public int AccessMask;

			// Token: 0x0400062D RID: 1581
			public Rpc.PolicyHandle Handle;
		}

		// Token: 0x0200014E RID: 334
		public class SamrConnect4 : DcerpcMessage
		{
			// Token: 0x060008C1 RID: 2241 RVA: 0x0002EED8 File Offset: 0x0002D0D8
			public override int GetOpnum()
			{
				return 62;
			}

			// Token: 0x060008C2 RID: 2242 RVA: 0x0002EEEC File Offset: 0x0002D0EC
			public SamrConnect4(string systemName, int unknown, int accessMask, Rpc.PolicyHandle handle)
			{
				this.SystemName = systemName;
				this.Unknown = unknown;
				this.AccessMask = accessMask;
				this.Handle = handle;
			}

			// Token: 0x060008C3 RID: 2243 RVA: 0x0002EF14 File Offset: 0x0002D114
			public override void Encode_in(NdrBuffer dst)
			{
				dst.Enc_ndr_referent(this.SystemName, 1);
				bool flag = this.SystemName != null;
				if (flag)
				{
					dst.Enc_ndr_string(this.SystemName);
				}
				dst.Enc_ndr_long(this.Unknown);
				dst.Enc_ndr_long(this.AccessMask);
			}

			// Token: 0x060008C4 RID: 2244 RVA: 0x0002EF66 File Offset: 0x0002D166
			public override void Decode_out(NdrBuffer src)
			{
				this.Handle.Decode(src);
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x0400062E RID: 1582
			public int Retval;

			// Token: 0x0400062F RID: 1583
			public string SystemName;

			// Token: 0x04000630 RID: 1584
			public int Unknown;

			// Token: 0x04000631 RID: 1585
			public int AccessMask;

			// Token: 0x04000632 RID: 1586
			public Rpc.PolicyHandle Handle;
		}

		// Token: 0x0200014F RID: 335
		public class SamrOpenDomain : DcerpcMessage
		{
			// Token: 0x060008C5 RID: 2245 RVA: 0x0002EF84 File Offset: 0x0002D184
			public override int GetOpnum()
			{
				return 7;
			}

			// Token: 0x060008C6 RID: 2246 RVA: 0x0002EF97 File Offset: 0x0002D197
			public SamrOpenDomain(Rpc.PolicyHandle handle, int accessMask, Rpc.SidT sid, Rpc.PolicyHandle domainHandle)
			{
				this.Handle = handle;
				this.AccessMask = accessMask;
				this.Sid = sid;
				this.DomainHandle = domainHandle;
			}

			// Token: 0x060008C7 RID: 2247 RVA: 0x0002EFBE File Offset: 0x0002D1BE
			public override void Encode_in(NdrBuffer dst)
			{
				this.Handle.Encode(dst);
				dst.Enc_ndr_long(this.AccessMask);
				this.Sid.Encode(dst);
			}

			// Token: 0x060008C8 RID: 2248 RVA: 0x0002EFE8 File Offset: 0x0002D1E8
			public override void Decode_out(NdrBuffer src)
			{
				this.DomainHandle.Decode(src);
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000633 RID: 1587
			public int Retval;

			// Token: 0x04000634 RID: 1588
			public Rpc.PolicyHandle Handle;

			// Token: 0x04000635 RID: 1589
			public int AccessMask;

			// Token: 0x04000636 RID: 1590
			public Rpc.SidT Sid;

			// Token: 0x04000637 RID: 1591
			public Rpc.PolicyHandle DomainHandle;
		}

		// Token: 0x02000150 RID: 336
		public class SamrSamEntry : NdrObject
		{
			// Token: 0x060008C9 RID: 2249 RVA: 0x0002F004 File Offset: 0x0002D204
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Idx);
				dst.Enc_ndr_short((int)this.Name.Length);
				dst.Enc_ndr_short((int)this.Name.MaximumLength);
				dst.Enc_ndr_referent(this.Name.Buffer, 1);
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

			// Token: 0x060008CA RID: 2250 RVA: 0x0002F0F8 File Offset: 0x0002D2F8
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Idx = src.Dec_ndr_long();
				src.Align(4);
				bool flag = this.Name == null;
				if (flag)
				{
					this.Name = new Rpc.Unicode_string();
				}
				this.Name.Length = (short)src.Dec_ndr_short();
				this.Name.MaximumLength = (short)src.Dec_ndr_short();
				int num = src.Dec_ndr_long();
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

			// Token: 0x04000638 RID: 1592
			public int Idx;

			// Token: 0x04000639 RID: 1593
			public Rpc.Unicode_string Name;
		}

		// Token: 0x02000151 RID: 337
		public class SamrSamArray : NdrObject
		{
			// Token: 0x060008CC RID: 2252 RVA: 0x0002F228 File Offset: 0x0002D428
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.Entries, 1);
				bool flag = this.Entries != null;
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
						this.Entries[i].Encode(dst);
					}
				}
			}

			// Token: 0x060008CD RID: 2253 RVA: 0x0002F2C0 File Offset: 0x0002D4C0
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
					bool flag2 = this.Entries == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Entries = new Samr.SamrSamEntry[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.Entries[i] == null;
						if (flag4)
						{
							this.Entries[i] = new Samr.SamrSamEntry();
						}
						this.Entries[i].Decode(src);
					}
				}
			}

			// Token: 0x0400063A RID: 1594
			public int Count;

			// Token: 0x0400063B RID: 1595
			public Samr.SamrSamEntry[] Entries;
		}

		// Token: 0x02000152 RID: 338
		public class SamrEnumerateAliasesInDomain : DcerpcMessage
		{
			// Token: 0x060008CF RID: 2255 RVA: 0x0002F3A8 File Offset: 0x0002D5A8
			public override int GetOpnum()
			{
				return 15;
			}

			// Token: 0x060008D0 RID: 2256 RVA: 0x0002F3BC File Offset: 0x0002D5BC
			public SamrEnumerateAliasesInDomain(Rpc.PolicyHandle domainHandle, int resumeHandle, int acctFlags, Samr.SamrSamArray sam, int numEntries)
			{
				this.DomainHandle = domainHandle;
				this.ResumeHandle = resumeHandle;
				this.AcctFlags = acctFlags;
				this.Sam = sam;
				this.NumEntries = numEntries;
			}

			// Token: 0x060008D1 RID: 2257 RVA: 0x0002F3EB File Offset: 0x0002D5EB
			public override void Encode_in(NdrBuffer dst)
			{
				this.DomainHandle.Encode(dst);
				dst.Enc_ndr_long(this.ResumeHandle);
				dst.Enc_ndr_long(this.AcctFlags);
			}

			// Token: 0x060008D2 RID: 2258 RVA: 0x0002F418 File Offset: 0x0002D618
			public override void Decode_out(NdrBuffer src)
			{
				this.ResumeHandle = src.Dec_ndr_long();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					bool flag2 = this.Sam == null;
					if (flag2)
					{
						this.Sam = new Samr.SamrSamArray();
					}
					this.Sam.Decode(src);
				}
				this.NumEntries = src.Dec_ndr_long();
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x0400063C RID: 1596
			public int Retval;

			// Token: 0x0400063D RID: 1597
			public Rpc.PolicyHandle DomainHandle;

			// Token: 0x0400063E RID: 1598
			public int ResumeHandle;

			// Token: 0x0400063F RID: 1599
			public int AcctFlags;

			// Token: 0x04000640 RID: 1600
			public Samr.SamrSamArray Sam;

			// Token: 0x04000641 RID: 1601
			public int NumEntries;
		}

		// Token: 0x02000153 RID: 339
		public class SamrOpenAlias : DcerpcMessage
		{
			// Token: 0x060008D3 RID: 2259 RVA: 0x0002F484 File Offset: 0x0002D684
			public override int GetOpnum()
			{
				return 27;
			}

			// Token: 0x060008D4 RID: 2260 RVA: 0x0002F498 File Offset: 0x0002D698
			public SamrOpenAlias(Rpc.PolicyHandle domainHandle, int accessMask, int rid, Rpc.PolicyHandle aliasHandle)
			{
				this.DomainHandle = domainHandle;
				this.AccessMask = accessMask;
				this.Rid = rid;
				this.AliasHandle = aliasHandle;
			}

			// Token: 0x060008D5 RID: 2261 RVA: 0x0002F4BF File Offset: 0x0002D6BF
			public override void Encode_in(NdrBuffer dst)
			{
				this.DomainHandle.Encode(dst);
				dst.Enc_ndr_long(this.AccessMask);
				dst.Enc_ndr_long(this.Rid);
			}

			// Token: 0x060008D6 RID: 2262 RVA: 0x0002F4E9 File Offset: 0x0002D6E9
			public override void Decode_out(NdrBuffer src)
			{
				this.AliasHandle.Decode(src);
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000642 RID: 1602
			public int Retval;

			// Token: 0x04000643 RID: 1603
			public Rpc.PolicyHandle DomainHandle;

			// Token: 0x04000644 RID: 1604
			public int AccessMask;

			// Token: 0x04000645 RID: 1605
			public int Rid;

			// Token: 0x04000646 RID: 1606
			public Rpc.PolicyHandle AliasHandle;
		}

		// Token: 0x02000154 RID: 340
		public class SamrGetMembersInAlias : DcerpcMessage
		{
			// Token: 0x060008D7 RID: 2263 RVA: 0x0002F508 File Offset: 0x0002D708
			public override int GetOpnum()
			{
				return 33;
			}

			// Token: 0x060008D8 RID: 2264 RVA: 0x0002F51C File Offset: 0x0002D71C
			public SamrGetMembersInAlias(Rpc.PolicyHandle aliasHandle, Lsarpc.LsarSidArray sids)
			{
				this.AliasHandle = aliasHandle;
				this.Sids = sids;
			}

			// Token: 0x060008D9 RID: 2265 RVA: 0x0002F534 File Offset: 0x0002D734
			public override void Encode_in(NdrBuffer dst)
			{
				this.AliasHandle.Encode(dst);
			}

			// Token: 0x060008DA RID: 2266 RVA: 0x0002F544 File Offset: 0x0002D744
			public override void Decode_out(NdrBuffer src)
			{
				this.Sids.Decode(src);
				this.Retval = src.Dec_ndr_long();
			}

			// Token: 0x04000647 RID: 1607
			public int Retval;

			// Token: 0x04000648 RID: 1608
			public Rpc.PolicyHandle AliasHandle;

			// Token: 0x04000649 RID: 1609
			public Lsarpc.LsarSidArray Sids;
		}

		// Token: 0x02000155 RID: 341
		public class SamrRidWithAttribute : NdrObject
		{
			// Token: 0x060008DB RID: 2267 RVA: 0x0002F560 File Offset: 0x0002D760
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Rid);
				dst.Enc_ndr_long(this.Attributes);
			}

			// Token: 0x060008DC RID: 2268 RVA: 0x0002F585 File Offset: 0x0002D785
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Rid = src.Dec_ndr_long();
				this.Attributes = src.Dec_ndr_long();
			}

			// Token: 0x0400064A RID: 1610
			public int Rid;

			// Token: 0x0400064B RID: 1611
			public int Attributes;
		}

		// Token: 0x02000156 RID: 342
		public class SamrRidWithAttributeArray : NdrObject
		{
			// Token: 0x060008DE RID: 2270 RVA: 0x0002F5A8 File Offset: 0x0002D7A8
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Count);
				dst.Enc_ndr_referent(this.Rids, 1);
				bool flag = this.Rids != null;
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
						this.Rids[i].Encode(dst);
					}
				}
			}

			// Token: 0x060008DF RID: 2271 RVA: 0x0002F63C File Offset: 0x0002D83C
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
					bool flag2 = this.Rids == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Rids = new Samr.SamrRidWithAttribute[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num2; i++)
					{
						bool flag4 = this.Rids[i] == null;
						if (flag4)
						{
							this.Rids[i] = new Samr.SamrRidWithAttribute();
						}
						this.Rids[i].Decode(src);
					}
				}
			}

			// Token: 0x0400064C RID: 1612
			public int Count;

			// Token: 0x0400064D RID: 1613
			public Samr.SamrRidWithAttribute[] Rids;
		}
	}
}
