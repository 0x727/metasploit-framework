using System;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x02000103 RID: 259
	public class SamrPolicyHandle : Rpc.PolicyHandle
	{
		// Token: 0x060007EC RID: 2028 RVA: 0x0002AD90 File Offset: 0x00028F90
		public SamrPolicyHandle(DcerpcHandle handle, string server, int access)
		{
			bool flag = server == null;
			if (flag)
			{
				server = "\\\\";
			}
			MsrpcSamrConnect4 msg = new MsrpcSamrConnect4(server, access, this);
			try
			{
				handle.Sendrecv(msg);
			}
			catch (DcerpcException ex)
			{
				bool flag2 = ex.GetErrorCode() != DcerpcError.DcerpcFaultOpRngError;
				if (flag2)
				{
					throw;
				}
				MsrpcSamrConnect2 msg2 = new MsrpcSamrConnect2(server, access, this);
				handle.Sendrecv(msg2);
			}
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x00008663 File Offset: 0x00006863
		public virtual void Close()
		{
		}
	}
}
