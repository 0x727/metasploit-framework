using System;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000DF RID: 223
	public static class DcerpcConstants
	{
		// Token: 0x040004C7 RID: 1223
		public static Uuid DcerpcUuidSyntaxNdr = new Uuid("8a885d04-1ceb-11c9-9fe8-08002b104860");

		// Token: 0x040004C8 RID: 1224
		public static int DcerpcFirstFrag = 1;

		// Token: 0x040004C9 RID: 1225
		public static int DcerpcLastFrag = 2;

		// Token: 0x040004CA RID: 1226
		public static int DcerpcPendingCancel = 4;

		// Token: 0x040004CB RID: 1227
		public static int DcerpcReserved1 = 8;

		// Token: 0x040004CC RID: 1228
		public static int DcerpcConcMpx = 16;

		// Token: 0x040004CD RID: 1229
		public static int DcerpcDidNotExecute = 32;

		// Token: 0x040004CE RID: 1230
		public static int DcerpcMaybe = 64;

		// Token: 0x040004CF RID: 1231
		public static int DcerpcObjectUuid = 128;
	}
}
