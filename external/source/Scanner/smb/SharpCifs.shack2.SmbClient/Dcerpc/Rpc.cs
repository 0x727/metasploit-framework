using System;
using SharpCifs.Dcerpc.Ndr;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000E6 RID: 230
	public class Rpc
	{
		// Token: 0x0200012A RID: 298
		public class UuidT : NdrObject
		{
			// Token: 0x06000851 RID: 2129 RVA: 0x0002BDEC File Offset: 0x00029FEC
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.TimeLow);
				dst.Enc_ndr_short((int)this.TimeMid);
				dst.Enc_ndr_short((int)this.TimeHiAndVersion);
				dst.Enc_ndr_small((int)this.ClockSeqHiAndReserved);
				dst.Enc_ndr_small((int)this.ClockSeqLow);
				int num = 6;
				int index = dst.Index;
				dst.Advance(num);
				dst = dst.Derive(index);
				for (int i = 0; i < num; i++)
				{
					dst.Enc_ndr_small((int)this.Node[i]);
				}
			}

			// Token: 0x06000852 RID: 2130 RVA: 0x0002BE80 File Offset: 0x0002A080
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.TimeLow = src.Dec_ndr_long();
				this.TimeMid = (short)src.Dec_ndr_short();
				this.TimeHiAndVersion = (short)src.Dec_ndr_short();
				this.ClockSeqHiAndReserved = (byte)src.Dec_ndr_small();
				this.ClockSeqLow = (byte)src.Dec_ndr_small();
				int num = 6;
				int index = src.Index;
				src.Advance(num);
				bool flag = this.Node == null;
				if (flag)
				{
					bool flag2 = num < 0 || num > 65535;
					if (flag2)
					{
						throw new NdrException(NdrException.InvalidConformance);
					}
					this.Node = new byte[num];
				}
				src = src.Derive(index);
				for (int i = 0; i < num; i++)
				{
					this.Node[i] = (byte)src.Dec_ndr_small();
				}
			}

			// Token: 0x040005C1 RID: 1473
			public int TimeLow;

			// Token: 0x040005C2 RID: 1474
			public short TimeMid;

			// Token: 0x040005C3 RID: 1475
			public short TimeHiAndVersion;

			// Token: 0x040005C4 RID: 1476
			public byte ClockSeqHiAndReserved;

			// Token: 0x040005C5 RID: 1477
			public byte ClockSeqLow;

			// Token: 0x040005C6 RID: 1478
			public byte[] Node;
		}

		// Token: 0x0200012B RID: 299
		public class PolicyHandle : NdrObject
		{
			// Token: 0x06000854 RID: 2132 RVA: 0x0002BF5C File Offset: 0x0002A15C
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_long(this.Type);
				dst.Enc_ndr_long(this.Uuid.TimeLow);
				dst.Enc_ndr_short((int)this.Uuid.TimeMid);
				dst.Enc_ndr_short((int)this.Uuid.TimeHiAndVersion);
				dst.Enc_ndr_small((int)this.Uuid.ClockSeqHiAndReserved);
				dst.Enc_ndr_small((int)this.Uuid.ClockSeqLow);
				int num = 6;
				int index = dst.Index;
				dst.Advance(num);
				dst = dst.Derive(index);
				for (int i = 0; i < num; i++)
				{
					dst.Enc_ndr_small((int)this.Uuid.Node[i]);
				}
			}

			// Token: 0x06000855 RID: 2133 RVA: 0x0002C01C File Offset: 0x0002A21C
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Type = src.Dec_ndr_long();
				src.Align(4);
				bool flag = this.Uuid == null;
				if (flag)
				{
					this.Uuid = new Rpc.UuidT();
				}
				this.Uuid.TimeLow = src.Dec_ndr_long();
				this.Uuid.TimeMid = (short)src.Dec_ndr_short();
				this.Uuid.TimeHiAndVersion = (short)src.Dec_ndr_short();
				this.Uuid.ClockSeqHiAndReserved = (byte)src.Dec_ndr_small();
				this.Uuid.ClockSeqLow = (byte)src.Dec_ndr_small();
				int num = 6;
				int index = src.Index;
				src.Advance(num);
				bool flag2 = this.Uuid.Node == null;
				if (flag2)
				{
					bool flag3 = num < 0 || num > 65535;
					if (flag3)
					{
						throw new NdrException(NdrException.InvalidConformance);
					}
					this.Uuid.Node = new byte[num];
				}
				src = src.Derive(index);
				for (int i = 0; i < num; i++)
				{
					this.Uuid.Node[i] = (byte)src.Dec_ndr_small();
				}
			}

			// Token: 0x040005C7 RID: 1479
			public int Type;

			// Token: 0x040005C8 RID: 1480
			public Rpc.UuidT Uuid;
		}

		// Token: 0x0200012C RID: 300
		public class Unicode_string : NdrObject
		{
			// Token: 0x06000857 RID: 2135 RVA: 0x0002C148 File Offset: 0x0002A348
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				dst.Enc_ndr_short((int)this.Length);
				dst.Enc_ndr_short((int)this.MaximumLength);
				dst.Enc_ndr_referent(this.Buffer, 1);
				bool flag = this.Buffer != null;
				if (flag)
				{
					dst = dst.Deferred;
					int num = (int)(this.Length / 2);
					int l = (int)(this.MaximumLength / 2);
					dst.Enc_ndr_long(l);
					dst.Enc_ndr_long(0);
					dst.Enc_ndr_long(num);
					int index = dst.Index;
					dst.Advance(2 * num);
					dst = dst.Derive(index);
					for (int i = 0; i < num; i++)
					{
						dst.Enc_ndr_short((int)this.Buffer[i]);
					}
				}
			}

			// Token: 0x06000858 RID: 2136 RVA: 0x0002C20C File Offset: 0x0002A40C
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				this.Length = (short)src.Dec_ndr_short();
				this.MaximumLength = (short)src.Dec_ndr_short();
				int num = src.Dec_ndr_long();
				bool flag = num != 0;
				if (flag)
				{
					src = src.Deferred;
					int num2 = src.Dec_ndr_long();
					src.Dec_ndr_long();
					int num3 = src.Dec_ndr_long();
					int index = src.Index;
					src.Advance(2 * num3);
					bool flag2 = this.Buffer == null;
					if (flag2)
					{
						bool flag3 = num2 < 0 || num2 > 65535;
						if (flag3)
						{
							throw new NdrException(NdrException.InvalidConformance);
						}
						this.Buffer = new short[num2];
					}
					src = src.Derive(index);
					for (int i = 0; i < num3; i++)
					{
						this.Buffer[i] = (short)src.Dec_ndr_short();
					}
				}
			}

			// Token: 0x040005C9 RID: 1481
			public short Length;

			// Token: 0x040005CA RID: 1482
			public short MaximumLength;

			// Token: 0x040005CB RID: 1483
			public short[] Buffer;
		}

		// Token: 0x0200012D RID: 301
		public class SidT : NdrObject
		{
			// Token: 0x0600085A RID: 2138 RVA: 0x0002C2F0 File Offset: 0x0002A4F0
			public override void Encode(NdrBuffer dst)
			{
				dst.Align(4);
				int subAuthorityCount = (int)this.SubAuthorityCount;
				dst.Enc_ndr_long(subAuthorityCount);
				dst.Enc_ndr_small((int)this.Revision);
				dst.Enc_ndr_small((int)this.SubAuthorityCount);
				int num = 6;
				int index = dst.Index;
				dst.Advance(num);
				int index2 = dst.Index;
				dst.Advance(4 * subAuthorityCount);
				dst = dst.Derive(index);
				for (int i = 0; i < num; i++)
				{
					dst.Enc_ndr_small((int)this.IdentifierAuthority[i]);
				}
				dst = dst.Derive(index2);
				for (int j = 0; j < subAuthorityCount; j++)
				{
					dst.Enc_ndr_long(this.SubAuthority[j]);
				}
			}

			// Token: 0x0600085B RID: 2139 RVA: 0x0002C3B4 File Offset: 0x0002A5B4
			public override void Decode(NdrBuffer src)
			{
				src.Align(4);
				int num = src.Dec_ndr_long();
				this.Revision = (byte)src.Dec_ndr_small();
				this.SubAuthorityCount = (byte)src.Dec_ndr_small();
				int num2 = 6;
				int index = src.Index;
				src.Advance(num2);
				int index2 = src.Index;
				src.Advance(4 * num);
				bool flag = this.IdentifierAuthority == null;
				if (flag)
				{
					bool flag2 = num2 < 0 || num2 > 65535;
					if (flag2)
					{
						throw new NdrException(NdrException.InvalidConformance);
					}
					this.IdentifierAuthority = new byte[num2];
				}
				src = src.Derive(index);
				for (int i = 0; i < num2; i++)
				{
					this.IdentifierAuthority[i] = (byte)src.Dec_ndr_small();
				}
				bool flag3 = this.SubAuthority == null;
				if (flag3)
				{
					bool flag4 = num < 0 || num > 65535;
					if (flag4)
					{
						throw new NdrException(NdrException.InvalidConformance);
					}
					this.SubAuthority = new int[num];
				}
				src = src.Derive(index2);
				for (int j = 0; j < num; j++)
				{
					this.SubAuthority[j] = src.Dec_ndr_long();
				}
			}

			// Token: 0x040005CC RID: 1484
			public byte Revision;

			// Token: 0x040005CD RID: 1485
			public byte SubAuthorityCount;

			// Token: 0x040005CE RID: 1486
			public byte[] IdentifierAuthority;

			// Token: 0x040005CF RID: 1487
			public int[] SubAuthority;
		}
	}
}
