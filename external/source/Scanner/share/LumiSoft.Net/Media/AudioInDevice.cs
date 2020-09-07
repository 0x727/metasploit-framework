using System;

namespace LumiSoft.Net.Media
{
	// Token: 0x02000152 RID: 338
	public class AudioInDevice
	{
		// Token: 0x06000DC7 RID: 3527 RVA: 0x000561AE File Offset: 0x000551AE
		internal AudioInDevice(int index, string name, int channels)
		{
			this.m_Index = index;
			this.m_Name = name;
			this.m_Channels = channels;
		}

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06000DC8 RID: 3528 RVA: 0x000561E8 File Offset: 0x000551E8
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06000DC9 RID: 3529 RVA: 0x00056200 File Offset: 0x00055200
		public int Channels
		{
			get
			{
				return this.m_Channels;
			}
		}

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06000DCA RID: 3530 RVA: 0x00056218 File Offset: 0x00055218
		internal int Index
		{
			get
			{
				return this.m_Index;
			}
		}

		// Token: 0x040005C8 RID: 1480
		private int m_Index = 0;

		// Token: 0x040005C9 RID: 1481
		private string m_Name = "";

		// Token: 0x040005CA RID: 1482
		private int m_Channels = 1;
	}
}
