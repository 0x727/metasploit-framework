using System;
using System.Globalization;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200001D RID: 29
	public abstract class DateFormat
	{
		// Token: 0x060000F9 RID: 249 RVA: 0x00007048 File Offset: 0x00005248
		public static DateFormat GetDateTimeInstance(int dateStyle, int timeStyle)
		{
			return DateFormat.GetDateTimeInstance(dateStyle, timeStyle, CultureInfo.CurrentCulture);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00007068 File Offset: 0x00005268
		public static DateFormat GetDateTimeInstance(int dateStyle, int timeStyle, CultureInfo aLocale)
		{
			return new SimpleDateFormat(aLocale.DateTimeFormat.FullDateTimePattern, aLocale);
		}

		// Token: 0x060000FB RID: 251
		public abstract DateTime Parse(string value);

		// Token: 0x060000FC RID: 252 RVA: 0x0000708C File Offset: 0x0000528C
		public TimeZoneInfo GetTimeZone()
		{
			return this._timeZone;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000070A4 File Offset: 0x000052A4
		public void SetTimeZone(TimeZoneInfo timeZone)
		{
			this._timeZone = timeZone;
		}

		// Token: 0x060000FE RID: 254
		public abstract string Format(DateTime time);

		// Token: 0x04000051 RID: 81
		public const int Default = 2;

		// Token: 0x04000052 RID: 82
		private TimeZoneInfo _timeZone;
	}
}
