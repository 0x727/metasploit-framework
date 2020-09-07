using System;

namespace LumiSoft.Net.STUN.Message
{
	// Token: 0x02000025 RID: 37
	public enum STUN_MessageType
	{
		// Token: 0x04000082 RID: 130
		BindingRequest = 1,
		// Token: 0x04000083 RID: 131
		BindingResponse = 257,
		// Token: 0x04000084 RID: 132
		BindingErrorResponse = 273,
		// Token: 0x04000085 RID: 133
		SharedSecretRequest = 2,
		// Token: 0x04000086 RID: 134
		SharedSecretResponse = 258,
		// Token: 0x04000087 RID: 135
		SharedSecretErrorResponse = 274
	}
}
