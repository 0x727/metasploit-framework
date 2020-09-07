using System;
using System.Globalization;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000063 RID: 99
	public class SimpleDateFormat : DateFormat
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060002B9 RID: 697 RVA: 0x0000B482 File Offset: 0x00009682
		// (set) Token: 0x060002BA RID: 698 RVA: 0x0000B48A File Offset: 0x0000968A
		private CultureInfo Culture { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060002BB RID: 699 RVA: 0x0000B493 File Offset: 0x00009693
		// (set) Token: 0x060002BC RID: 700 RVA: 0x0000B49B File Offset: 0x0000969B
		private bool Lenient { get; set; }

		// Token: 0x060002BD RID: 701 RVA: 0x0000B4A4 File Offset: 0x000096A4
		public SimpleDateFormat() : this("g")
		{
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000B4B3 File Offset: 0x000096B3
		public SimpleDateFormat(string format) : this(format, CultureInfo.CurrentCulture)
		{
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000B4C4 File Offset: 0x000096C4
		public SimpleDateFormat(string format, CultureInfo c)
		{
			this.Culture = c;
			this._format = format.Replace("EEE", "ddd");
			this._format = this._format.Replace("Z", "zzz");
			base.SetTimeZone(TimeZoneInfo.Local);
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000B520 File Offset: 0x00009720
		public bool IsLenient()
		{
			return this.Lenient;
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0000B538 File Offset: 0x00009738
		public void SetLenient(bool lenient)
		{
			this.Lenient = lenient;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000B544 File Offset: 0x00009744
		public override DateTime Parse(string value)
		{
			bool flag = this.IsLenient();
			DateTime result;
			if (flag)
			{
				result = DateTime.Parse(value);
			}
			else
			{
				result = DateTime.ParseExact(value, this._format, this.Culture);
			}
			return result;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000B57C File Offset: 0x0000977C
		public override string Format(DateTime date)
		{
			date += base.GetTimeZone().BaseUtcOffset;
			return date.ToString(this._format);
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0000B5B0 File Offset: 0x000097B0
		public string Format(long date)
		{
			return Extensions.MillisToDateTimeOffset(date, (long)((int)base.GetTimeZone().BaseUtcOffset.TotalMinutes)).DateTime.ToString(this._format);
		}

		// Token: 0x04000080 RID: 128
		private string _format;
	}
}
