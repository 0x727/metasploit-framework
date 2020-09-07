using System;
using System.Net;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000E2 RID: 226
	public class RTP_Utils
	{
		// Token: 0x06000915 RID: 2325 RVA: 0x000366BC File Offset: 0x000356BC
		public static uint GenerateSSRC()
		{
			return (uint)new Random().Next(100000, int.MaxValue);
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x000366E4 File Offset: 0x000356E4
		public static string GenerateCNAME()
		{
			return string.Concat(new string[]
			{
				Environment.UserName,
				"@",
				Dns.GetHostName(),
				".",
				Guid.NewGuid().ToString().Substring(0, 8)
			});
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x00036740 File Offset: 0x00035740
		public static uint DateTimeToNTP32(DateTime value)
		{
			return (uint)(RTP_Utils.DateTimeToNTP64(value) >> 16 & (ulong)-1);
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x00036760 File Offset: 0x00035760
		public static ulong DateTimeToNTP64(DateTime value)
		{
			TimeSpan timeSpan = value.ToUniversalTime() - new DateTime(1900, 1, 1, 0, 0, 0);
			return (ulong)(timeSpan.TotalMilliseconds % 1000.0) << 32 | (ulong)((ulong)timeSpan.Milliseconds << 22);
		}
	}
}
