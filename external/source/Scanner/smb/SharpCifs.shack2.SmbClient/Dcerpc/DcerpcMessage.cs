using System;
using SharpCifs.Dcerpc.Ndr;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000E3 RID: 227
	public abstract class DcerpcMessage : NdrObject
	{
		// Token: 0x06000784 RID: 1924 RVA: 0x000295C4 File Offset: 0x000277C4
		public virtual bool IsFlagSet(int flag)
		{
			return (this.Flags & flag) == flag;
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x000295E1 File Offset: 0x000277E1
		public virtual void UnsetFlag(int flag)
		{
			this.Flags &= ~flag;
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x000295F3 File Offset: 0x000277F3
		public virtual void SetFlag(int flag)
		{
			this.Flags |= flag;
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x00029604 File Offset: 0x00027804
		public virtual DcerpcException GetResult()
		{
			bool flag = this.Result != 0;
			DcerpcException result;
			if (flag)
			{
				result = new DcerpcException(this.Result);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x00029634 File Offset: 0x00027834
		internal virtual void Encode_header(NdrBuffer buf)
		{
			buf.Enc_ndr_small(5);
			buf.Enc_ndr_small(0);
			buf.Enc_ndr_small(this.Ptype);
			buf.Enc_ndr_small(this.Flags);
			buf.Enc_ndr_long(16);
			buf.Enc_ndr_short(this.Length);
			buf.Enc_ndr_short(0);
			buf.Enc_ndr_long(this.CallId);
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x00029698 File Offset: 0x00027898
		internal virtual void Decode_header(NdrBuffer buf)
		{
			bool flag = buf.Dec_ndr_small() != 5 || buf.Dec_ndr_small() != 0;
			if (flag)
			{
				throw new NdrException("DCERPC version not supported");
			}
			this.Ptype = buf.Dec_ndr_small();
			this.Flags = buf.Dec_ndr_small();
			bool flag2 = buf.Dec_ndr_long() != 16;
			if (flag2)
			{
				throw new NdrException("Data representation not supported");
			}
			this.Length = buf.Dec_ndr_short();
			bool flag3 = buf.Dec_ndr_short() != 0;
			if (flag3)
			{
				throw new NdrException("DCERPC authentication not supported");
			}
			this.CallId = buf.Dec_ndr_long();
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x00029734 File Offset: 0x00027934
		public override void Encode(NdrBuffer buf)
		{
			int index = buf.GetIndex();
			int num = 0;
			buf.Advance(16);
			bool flag = this.Ptype == 0;
			if (flag)
			{
				num = buf.GetIndex();
				buf.Enc_ndr_long(0);
				buf.Enc_ndr_short(0);
				buf.Enc_ndr_short(this.GetOpnum());
			}
			this.Encode_in(buf);
			this.Length = buf.GetIndex() - index;
			bool flag2 = this.Ptype == 0;
			if (flag2)
			{
				buf.SetIndex(num);
				this.AllocHint = this.Length - num;
				buf.Enc_ndr_long(this.AllocHint);
			}
			buf.SetIndex(index);
			this.Encode_header(buf);
			buf.SetIndex(index + this.Length);
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x000297F0 File Offset: 0x000279F0
		public override void Decode(NdrBuffer buf)
		{
			this.Decode_header(buf);
			bool flag = this.Ptype != 12 && this.Ptype != 2 && this.Ptype != 3 && this.Ptype != 13;
			if (flag)
			{
				throw new NdrException("Unexpected ptype: " + this.Ptype);
			}
			bool flag2 = this.Ptype == 2 || this.Ptype == 3;
			if (flag2)
			{
				this.AllocHint = buf.Dec_ndr_long();
				buf.Dec_ndr_short();
				buf.Dec_ndr_short();
			}
			bool flag3 = this.Ptype == 3 || this.Ptype == 13;
			if (flag3)
			{
				this.Result = buf.Dec_ndr_long();
			}
			else
			{
				this.Decode_out(buf);
			}
		}

		// Token: 0x0600078C RID: 1932
		public abstract int GetOpnum();

		// Token: 0x0600078D RID: 1933
		public abstract void Encode_in(NdrBuffer dst);

		// Token: 0x0600078E RID: 1934
		public abstract void Decode_out(NdrBuffer src);

		// Token: 0x040004E3 RID: 1251
		protected internal int Ptype = -1;

		// Token: 0x040004E4 RID: 1252
		protected internal int Flags;

		// Token: 0x040004E5 RID: 1253
		protected internal int Length;

		// Token: 0x040004E6 RID: 1254
		protected internal int CallId;

		// Token: 0x040004E7 RID: 1255
		protected internal int AllocHint;

		// Token: 0x040004E8 RID: 1256
		protected internal int Result;
	}
}
