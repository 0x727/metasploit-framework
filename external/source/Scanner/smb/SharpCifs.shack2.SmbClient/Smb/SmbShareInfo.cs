using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x020000B2 RID: 178
	public class SmbShareInfo : IFileEntry
	{
		// Token: 0x060005AB RID: 1451 RVA: 0x00003195 File Offset: 0x00001395
		public SmbShareInfo()
		{
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x0001E488 File Offset: 0x0001C688
		public SmbShareInfo(string netName, int type, string remark)
		{
			this.NetName = netName;
			this.Type = type;
			this.Remark = remark;
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x0001E4A8 File Offset: 0x0001C6A8
		public virtual string GetName()
		{
			return this.NetName;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x0001E4C0 File Offset: 0x0001C6C0
		public new virtual int GetType()
		{
			int num = this.Type & 65535;
			int result;
			if (num != 1)
			{
				if (num != 3)
				{
					result = 8;
				}
				else
				{
					result = 16;
				}
			}
			else
			{
				result = 32;
			}
			return result;
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x0001E4F8 File Offset: 0x0001C6F8
		public virtual int GetAttributes()
		{
			return 17;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x0001E50C File Offset: 0x0001C70C
		public virtual long CreateTime()
		{
			return 0L;
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x0001E520 File Offset: 0x0001C720
		public virtual long LastModified()
		{
			return 0L;
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x0001E534 File Offset: 0x0001C734
		public virtual long Length()
		{
			return 0L;
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x0001E548 File Offset: 0x0001C748
		public override bool Equals(object obj)
		{
			bool flag = obj is SmbShareInfo;
			bool result;
			if (flag)
			{
				SmbShareInfo smbShareInfo = (SmbShareInfo)obj;
				result = this.NetName.Equals(smbShareInfo.NetName);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x0001E584 File Offset: 0x0001C784
		public override int GetHashCode()
		{
			return this.NetName.GetHashCode();
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x0001E5A4 File Offset: 0x0001C7A4
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"SmbShareInfo[netName=",
				this.NetName,
				",type=0x",
				Hexdump.ToHexString(this.Type, 8),
				",remark=",
				this.Remark,
				"]"
			});
		}

		// Token: 0x0400035A RID: 858
		protected internal string NetName;

		// Token: 0x0400035B RID: 859
		protected internal int Type;

		// Token: 0x0400035C RID: 860
		protected internal string Remark;
	}
}
