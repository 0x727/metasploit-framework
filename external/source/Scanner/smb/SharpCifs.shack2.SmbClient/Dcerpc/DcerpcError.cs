using System;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000E0 RID: 224
	public static class DcerpcError
	{
		// Token: 0x040004D0 RID: 1232
		public static int DcerpcFaultOther = 1;

		// Token: 0x040004D1 RID: 1233
		public static int DcerpcFaultAccessDenied = 5;

		// Token: 0x040004D2 RID: 1234
		public static int DcerpcFaultCantPerform = 1752;

		// Token: 0x040004D3 RID: 1235
		public static int DcerpcFaultNdr = 1783;

		// Token: 0x040004D4 RID: 1236
		public static int DcerpcFaultInvalidTag = 469762054;

		// Token: 0x040004D5 RID: 1237
		public static int DcerpcFaultContextMismatch = 469762074;

		// Token: 0x040004D6 RID: 1238
		public static int DcerpcFaultOpRngError = 469827586;

		// Token: 0x040004D7 RID: 1239
		public static int DcerpcFaultUnkIf = 469827587;

		// Token: 0x040004D8 RID: 1240
		public static int DcerpcFaultProtoError = 469827595;

		// Token: 0x040004D9 RID: 1241
		public static int[] DcerpcFaultCodes = new int[]
		{
			DcerpcError.DcerpcFaultOther,
			DcerpcError.DcerpcFaultAccessDenied,
			DcerpcError.DcerpcFaultCantPerform,
			DcerpcError.DcerpcFaultNdr,
			DcerpcError.DcerpcFaultInvalidTag,
			DcerpcError.DcerpcFaultContextMismatch,
			DcerpcError.DcerpcFaultOpRngError,
			DcerpcError.DcerpcFaultUnkIf,
			DcerpcError.DcerpcFaultProtoError
		};

		// Token: 0x040004DA RID: 1242
		public static string[] DcerpcFaultMessages = new string[]
		{
			"DCERPC_FAULT_OTHER",
			"DCERPC_FAULT_ACCESS_DENIED",
			"DCERPC_FAULT_CANT_PERFORM",
			"DCERPC_FAULT_NDR",
			"DCERPC_FAULT_INVALID_TAG",
			"DCERPC_FAULT_CONTEXT_MISMATCH",
			"DCERPC_FAULT_OP_RNG_ERROR",
			"DCERPC_FAULT_UNK_IF",
			"DCERPC_FAULT_PROTO_ERROR"
		};
	}
}
