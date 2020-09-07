using System;
using SharpCifs.Smb;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F0 RID: 240
	public class LsaPolicyHandle : Rpc.PolicyHandle
	{
		// Token: 0x060007D0 RID: 2000 RVA: 0x0002A878 File Offset: 0x00028A78
		public LsaPolicyHandle(DcerpcHandle handle, string server, int access)
		{
			bool flag = server == null;
			if (flag)
			{
				server = "\\\\";
			}
			MsrpcLsarOpenPolicy2 msrpcLsarOpenPolicy = new MsrpcLsarOpenPolicy2(server, access, this);
			handle.Sendrecv(msrpcLsarOpenPolicy);
			bool flag2 = msrpcLsarOpenPolicy.Retval != 0;
			if (flag2)
			{
				throw new SmbException(msrpcLsarOpenPolicy.Retval, false);
			}
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x00008663 File Offset: 0x00006863
		public virtual void Close()
		{
		}
	}
}
