using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Dcerpc.Ndr
{
	// Token: 0x020000E9 RID: 233
	public class NdrBuffer
	{
		// Token: 0x060007A3 RID: 1955 RVA: 0x0002A11C File Offset: 0x0002831C
		public NdrBuffer(byte[] buf, int start)
		{
			this.Buf = buf;
			this.Index = start;
			this.Start = start;
			this.Length = 0;
			this.Deferred = this;
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x0002A158 File Offset: 0x00028358
		public virtual NdrBuffer Derive(int idx)
		{
			return new NdrBuffer(this.Buf, this.Start)
			{
				Index = idx,
				Deferred = this.Deferred
			};
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x0002A190 File Offset: 0x00028390
		public virtual void Reset()
		{
			this.Index = this.Start;
			this.Length = 0;
			this.Deferred = this;
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x0002A1B0 File Offset: 0x000283B0
		public virtual int GetIndex()
		{
			return this.Index;
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x0002A1C8 File Offset: 0x000283C8
		public virtual void SetIndex(int index)
		{
			this.Index = index;
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x0002A1D4 File Offset: 0x000283D4
		public virtual int GetCapacity()
		{
			return this.Buf.Length - this.Start;
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x0002A1F8 File Offset: 0x000283F8
		public virtual int GetTailSpace()
		{
			return this.Buf.Length - this.Index;
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x0002A21C File Offset: 0x0002841C
		public virtual byte[] GetBuffer()
		{
			return this.Buf;
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x0002A234 File Offset: 0x00028434
		public virtual int Align(int boundary, byte value)
		{
			int num = this.Align(boundary);
			for (int i = num; i > 0; i--)
			{
				this.Buf[this.Index - i] = value;
			}
			return num;
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x0002A271 File Offset: 0x00028471
		public virtual void WriteOctetArray(byte[] b, int i, int l)
		{
			Array.Copy(b, i, this.Buf, this.Index, l);
			this.Advance(l);
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x0002A291 File Offset: 0x00028491
		public virtual void ReadOctetArray(byte[] b, int i, int l)
		{
			Array.Copy(this.Buf, this.Index, b, i, l);
			this.Advance(l);
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x0002A2B4 File Offset: 0x000284B4
		public virtual int GetLength()
		{
			return this.Deferred.Length;
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x0002A2D1 File Offset: 0x000284D1
		public virtual void SetLength(int length)
		{
			this.Deferred.Length = length;
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x0002A2E0 File Offset: 0x000284E0
		public virtual void Advance(int n)
		{
			this.Index += n;
			bool flag = this.Index - this.Start > this.Deferred.Length;
			if (flag)
			{
				this.Deferred.Length = this.Index - this.Start;
			}
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x0002A334 File Offset: 0x00028534
		public virtual int Align(int boundary)
		{
			int num = boundary - 1;
			int num2 = this.Index - this.Start;
			int num3 = (num2 + num & ~num) - num2;
			this.Advance(num3);
			return num3;
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x0002A36A File Offset: 0x0002856A
		public virtual void Enc_ndr_small(int s)
		{
			this.Buf[this.Index] = (byte)(s & 255);
			this.Advance(1);
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x0002A38C File Offset: 0x0002858C
		public virtual int Dec_ndr_small()
		{
			int result = (int)(this.Buf[this.Index] & byte.MaxValue);
			this.Advance(1);
			return result;
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x0002A3BB File Offset: 0x000285BB
		public virtual void Enc_ndr_short(int s)
		{
			this.Align(2);
			Encdec.Enc_uint16le((short)s, this.Buf, this.Index);
			this.Advance(2);
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x0002A3E4 File Offset: 0x000285E4
		public virtual int Dec_ndr_short()
		{
			this.Align(2);
			int result = (int)Encdec.Dec_uint16le(this.Buf, this.Index);
			this.Advance(2);
			return result;
		}

		// Token: 0x060007B6 RID: 1974 RVA: 0x0002A419 File Offset: 0x00028619
		public virtual void Enc_ndr_long(int l)
		{
			this.Align(4);
			Encdec.Enc_uint32le(l, this.Buf, this.Index);
			this.Advance(4);
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x0002A440 File Offset: 0x00028640
		public virtual int Dec_ndr_long()
		{
			this.Align(4);
			int result = Encdec.Dec_uint32le(this.Buf, this.Index);
			this.Advance(4);
			return result;
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x0002A475 File Offset: 0x00028675
		public virtual void Enc_ndr_hyper(long h)
		{
			this.Align(8);
			Encdec.Enc_uint64le(h, this.Buf, this.Index);
			this.Advance(8);
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x0002A49C File Offset: 0x0002869C
		public virtual long Dec_ndr_hyper()
		{
			this.Align(8);
			long result = Encdec.Dec_uint64le(this.Buf, this.Index);
			this.Advance(8);
			return result;
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x0002A4D4 File Offset: 0x000286D4
		public virtual void Enc_ndr_string(string s)
		{
			this.Align(4);
			int num = this.Index;
			int length = s.Length;
			Encdec.Enc_uint32le(length + 1, this.Buf, num);
			num += 4;
			Encdec.Enc_uint32le(0, this.Buf, num);
			num += 4;
			Encdec.Enc_uint32le(length + 1, this.Buf, num);
			num += 4;
			try
			{
				Array.Copy(Runtime.GetBytesForString(s, "UTF-16LE"), 0, this.Buf, num, length * 2);
			}
			catch (UnsupportedEncodingException)
			{
			}
			num += length * 2;
			this.Buf[num++] = 0;
			this.Buf[num++] = 0;
			this.Advance(num - this.Index);
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x0002A598 File Offset: 0x00028798
		public virtual string Dec_ndr_string()
		{
			this.Align(4);
			int num = this.Index;
			string result = null;
			int num2 = Encdec.Dec_uint32le(this.Buf, num);
			num += 12;
			bool flag = num2 != 0;
			if (flag)
			{
				num2--;
				int num3 = num2 * 2;
				try
				{
					bool flag2 = num3 < 0 || num3 > 65535;
					if (flag2)
					{
						throw new NdrException(NdrException.InvalidConformance);
					}
					result = Runtime.GetStringForBytes(this.Buf, num, num3, "UTF-16LE");
					num += num3 + 2;
				}
				catch (UnsupportedEncodingException)
				{
				}
			}
			this.Advance(num - this.Index);
			return result;
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x0002A64C File Offset: 0x0002884C
		private int GetDceReferent(object obj)
		{
			bool flag = this.Referents == null;
			if (flag)
			{
				this.Referents = new Hashtable();
				this.Referent = 1;
			}
			NdrBuffer.Entry entry;
			bool flag2 = (entry = (NdrBuffer.Entry)this.Referents.Get(obj)) == null;
			if (flag2)
			{
				entry = new NdrBuffer.Entry();
				NdrBuffer.Entry entry2 = entry;
				int referent = this.Referent;
				this.Referent = referent + 1;
				entry2.Referent = referent;
				entry.Obj = obj;
				this.Referents.Put(obj, entry);
			}
			return entry.Referent;
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x0002A6D8 File Offset: 0x000288D8
		public virtual void Enc_ndr_referent(object obj, int type)
		{
			bool flag = obj == null;
			if (flag)
			{
				this.Enc_ndr_long(0);
			}
			else
			{
				switch (type)
				{
				case 1:
				case 3:
					this.Enc_ndr_long(Runtime.IdentityHashCode(obj));
					break;
				case 2:
					this.Enc_ndr_long(this.GetDceReferent(obj));
					break;
				}
			}
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0002A734 File Offset: 0x00028934
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"start=",
				this.Start,
				",index=",
				this.Index,
				",length=",
				this.GetLength()
			});
		}

		// Token: 0x040004EF RID: 1263
		internal int Referent;

		// Token: 0x040004F0 RID: 1264
		internal Hashtable Referents;

		// Token: 0x040004F1 RID: 1265
		public byte[] Buf;

		// Token: 0x040004F2 RID: 1266
		public int Start;

		// Token: 0x040004F3 RID: 1267
		public int Index;

		// Token: 0x040004F4 RID: 1268
		public int Length;

		// Token: 0x040004F5 RID: 1269
		public NdrBuffer Deferred;

		// Token: 0x0200012E RID: 302
		internal class Entry
		{
			// Token: 0x040005D0 RID: 1488
			internal int Referent;

			// Token: 0x040005D1 RID: 1489
			internal object Obj;
		}
	}
}
