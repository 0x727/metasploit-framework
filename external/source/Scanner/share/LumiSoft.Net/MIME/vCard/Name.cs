using System;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x02000173 RID: 371
	public class Name
	{
		// Token: 0x06000F0E RID: 3854 RVA: 0x0005DBA8 File Offset: 0x0005CBA8
		public Name(string lastName, string firstName, string additionalNames, string honorificPrefix, string honorificSuffix)
		{
			this.m_LastName = lastName;
			this.m_FirstName = firstName;
			this.m_AdditionalNames = additionalNames;
			this.m_HonorificPrefix = honorificPrefix;
			this.m_HonorificSuffix = honorificSuffix;
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x0005DC1C File Offset: 0x0005CC1C
		internal Name()
		{
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x0005DC68 File Offset: 0x0005CC68
		public string ToValueString()
		{
			return string.Concat(new string[]
			{
				this.m_LastName,
				";",
				this.m_FirstName,
				";",
				this.m_AdditionalNames,
				";",
				this.m_HonorificPrefix,
				";",
				this.m_HonorificSuffix
			});
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x0005DCD4 File Offset: 0x0005CCD4
		internal static Name Parse(Item item)
		{
			string[] array = item.DecodedValue.Split(new char[]
			{
				';'
			});
			Name name = new Name();
			bool flag = array.Length >= 1;
			if (flag)
			{
				name.m_LastName = array[0];
			}
			bool flag2 = array.Length >= 2;
			if (flag2)
			{
				name.m_FirstName = array[1];
			}
			bool flag3 = array.Length >= 3;
			if (flag3)
			{
				name.m_AdditionalNames = array[2];
			}
			bool flag4 = array.Length >= 4;
			if (flag4)
			{
				name.m_HonorificPrefix = array[3];
			}
			bool flag5 = array.Length >= 5;
			if (flag5)
			{
				name.m_HonorificSuffix = array[4];
			}
			return name;
		}

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06000F12 RID: 3858 RVA: 0x0005DD84 File Offset: 0x0005CD84
		public string LastName
		{
			get
			{
				return this.m_LastName;
			}
		}

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06000F13 RID: 3859 RVA: 0x0005DD9C File Offset: 0x0005CD9C
		public string FirstName
		{
			get
			{
				return this.m_FirstName;
			}
		}

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06000F14 RID: 3860 RVA: 0x0005DDB4 File Offset: 0x0005CDB4
		public string AdditionalNames
		{
			get
			{
				return this.m_AdditionalNames;
			}
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06000F15 RID: 3861 RVA: 0x0005DDCC File Offset: 0x0005CDCC
		public string HonorificPerfix
		{
			get
			{
				return this.m_HonorificPrefix;
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x0005DDE4 File Offset: 0x0005CDE4
		public string HonorificSuffix
		{
			get
			{
				return this.m_HonorificSuffix;
			}
		}

		// Token: 0x0400063E RID: 1598
		private string m_LastName = "";

		// Token: 0x0400063F RID: 1599
		private string m_FirstName = "";

		// Token: 0x04000640 RID: 1600
		private string m_AdditionalNames = "";

		// Token: 0x04000641 RID: 1601
		private string m_HonorificPrefix = "";

		// Token: 0x04000642 RID: 1602
		private string m_HonorificSuffix = "";
	}
}
