using System;
using SharpCifs.Dcerpc.Ndr;
using SharpCifs.Smb;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F3 RID: 243
	public class MsrpcDfsRootEnum : Netdfs.NetrDfsEnumEx
	{
		// Token: 0x060007D5 RID: 2005 RVA: 0x0002A944 File Offset: 0x00028B44
		public MsrpcDfsRootEnum(string server) : base(server, 200, 65535, new Netdfs.DfsEnumStruct(), new NdrLong(0))
		{
			this.Info.Level = this.Level;
			this.Info.E = new Netdfs.DfsEnumArray200();
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0002A9A8 File Offset: 0x00028BA8
		public virtual IFileEntry[] GetEntries()
		{
			Netdfs.DfsEnumArray200 dfsEnumArray = (Netdfs.DfsEnumArray200)this.Info.E;
			SmbShareInfo[] array = new SmbShareInfo[dfsEnumArray.Count];
			for (int i = 0; i < dfsEnumArray.Count; i++)
			{
				array[i] = new SmbShareInfo(dfsEnumArray.S[i].DfsName, 0, null);
			}
			return array;
		}
	}
}
