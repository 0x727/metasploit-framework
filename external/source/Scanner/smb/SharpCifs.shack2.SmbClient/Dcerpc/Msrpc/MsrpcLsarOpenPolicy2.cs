using System;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F7 RID: 247
	public class MsrpcLsarOpenPolicy2 : Lsarpc.LsarOpenPolicy2
	{
		// Token: 0x060007DA RID: 2010 RVA: 0x0002AAA0 File Offset: 0x00028CA0
		public MsrpcLsarOpenPolicy2(string server, int access, LsaPolicyHandle policyHandle) : base(server, new Lsarpc.LsarObjectAttributes(), access, policyHandle)
		{
			this.ObjectAttributes.Length = 24;
			Lsarpc.LsarQosInfo lsarQosInfo = new Lsarpc.LsarQosInfo();
			lsarQosInfo.Length = 12;
			lsarQosInfo.ImpersonationLevel = 2;
			lsarQosInfo.ContextMode = 1;
			lsarQosInfo.EffectiveOnly = 0;
			this.ObjectAttributes.SecurityQualityOfService = lsarQosInfo;
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}
	}
}
