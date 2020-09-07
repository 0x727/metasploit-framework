using System;

namespace LumiSoft.Net.Media
{
	// Token: 0x02000154 RID: 340
	public class AudioOutDevice
	{
		// Token: 0x06000DD9 RID: 3545 RVA: 0x0005667D File Offset: 0x0005567D
		internal AudioOutDevice(int index, string name, int channels)
		{
			this.m_Index = index;
			this.m_Name = name;
			this.m_Channels = channels;
		}

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06000DDA RID: 3546 RVA: 0x000566B8 File Offset: 0x000556B8
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06000DDB RID: 3547 RVA: 0x000566D0 File Offset: 0x000556D0
		public int Channels
		{
			get
			{
				return this.m_Channels;
			}
		}

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06000DDC RID: 3548 RVA: 0x000566E8 File Offset: 0x000556E8
		internal int Index
		{
			get
			{
				return this.m_Index;
			}
		}

		// Token: 0x040005CF RID: 1487
		private int m_Index = 0;

		// Token: 0x040005D0 RID: 1488
		private string m_Name = "";

		// Token: 0x040005D1 RID: 1489
		private int m_Channels = 1;
	}
}
