using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000CA RID: 202
	public static class WinError
	{
		// Token: 0x040003DC RID: 988
		public static int ErrorSuccess = 0;

		// Token: 0x040003DD RID: 989
		public static int ErrorAccessDenied = 5;

		// Token: 0x040003DE RID: 990
		public static int ErrorReqNotAccep = 71;

		// Token: 0x040003DF RID: 991
		public static int ErrorBadPipe = 230;

		// Token: 0x040003E0 RID: 992
		public static int ErrorPipeBusy = 231;

		// Token: 0x040003E1 RID: 993
		public static int ErrorNoData = 232;

		// Token: 0x040003E2 RID: 994
		public static int ErrorPipeNotConnected = 233;

		// Token: 0x040003E3 RID: 995
		public static int ErrorMoreData = 234;

		// Token: 0x040003E4 RID: 996
		public static int ErrorNoBrowserServersFound = 6118;

		// Token: 0x040003E5 RID: 997
		public static int[] WinerrCodes = new int[]
		{
			WinError.ErrorSuccess,
			WinError.ErrorAccessDenied,
			WinError.ErrorReqNotAccep,
			WinError.ErrorBadPipe,
			WinError.ErrorPipeBusy,
			WinError.ErrorNoData,
			WinError.ErrorPipeNotConnected,
			WinError.ErrorMoreData,
			WinError.ErrorNoBrowserServersFound
		};

		// Token: 0x040003E6 RID: 998
		public static string[] WinerrMessages = new string[]
		{
			"The operation completed successfully.",
			"Access is denied.",
			"No more connections can be made to this remote computer at this time because there are already as many connections as the computer can accept.",
			"The pipe state is invalid.",
			"All pipe instances are busy.",
			"The pipe is being closed.",
			"No process is on the other end of the pipe.",
			"More data is available.",
			"The list of servers for this workgroup is not currently available."
		};
	}
}
