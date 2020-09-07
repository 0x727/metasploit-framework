using System;
using SharpCifs.Smb;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000FD RID: 253
	public class MsrpcShareEnum : Srvsvc.ShareEnumAll
	{
		// Token: 0x060007E0 RID: 2016 RVA: 0x0002ABCF File Offset: 0x00028DCF
		public MsrpcShareEnum(string server) : base("\\\\" + server, 1, new Srvsvc.ShareInfoCtr1(), -1, 0, 0)
		{
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x0002AC08 File Offset: 0x00028E08
		public virtual IFileEntry[] GetEntries()
		{
			Srvsvc.ShareInfoCtr1 shareInfoCtr = (Srvsvc.ShareInfoCtr1)this.Info;
			MsrpcShareEnum.MsrpcShareInfo1[] array = new MsrpcShareEnum.MsrpcShareInfo1[shareInfoCtr.Count];
			for (int i = 0; i < shareInfoCtr.Count; i++)
			{
				array[i] = new MsrpcShareEnum.MsrpcShareInfo1(this, shareInfoCtr.Array[i]);
			}
			return array;
		}

		// Token: 0x02000140 RID: 320
		internal class MsrpcShareInfo1 : SmbShareInfo
		{
			// Token: 0x06000896 RID: 2198 RVA: 0x0002E0C7 File Offset: 0x0002C2C7
			internal MsrpcShareInfo1(MsrpcShareEnum enclosing, Srvsvc.ShareInfo1 info1)
			{
				this._enclosing = enclosing;
				this.NetName = info1.Netname;
				this.Type = info1.Type;
				this.Remark = info1.Remark;
			}

			// Token: 0x0400060B RID: 1547
			private readonly MsrpcShareEnum _enclosing;
		}
	}
}
