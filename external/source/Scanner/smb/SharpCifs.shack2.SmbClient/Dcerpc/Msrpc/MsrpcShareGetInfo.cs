using System;
using SharpCifs.Smb;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000FE RID: 254
	public class MsrpcShareGetInfo : Srvsvc.ShareGetInfo
	{
		// Token: 0x060007E2 RID: 2018 RVA: 0x0002AC5D File Offset: 0x00028E5D
		public MsrpcShareGetInfo(string server, string sharename) : base(server, sharename, 502, new Srvsvc.ShareInfo502())
		{
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x0002AC8C File Offset: 0x00028E8C
		public virtual Ace[] GetSecurity()
		{
			Srvsvc.ShareInfo502 shareInfo = (Srvsvc.ShareInfo502)this.Info;
			bool flag = shareInfo.SecurityDescriptor != null;
			Ace[] result;
			if (flag)
			{
				SecurityDescriptor securityDescriptor = new SecurityDescriptor(shareInfo.SecurityDescriptor, 0, shareInfo.SdSize);
				result = securityDescriptor.Aces;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
