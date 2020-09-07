using System;

namespace LumiSoft.Net.Media
{
	// Token: 0x0200014D RID: 333
	public class AudioFormat
	{
		// Token: 0x06000D8B RID: 3467 RVA: 0x00054F8B File Offset: 0x00053F8B
		public AudioFormat(int samplesPerSecond, int bitsPerSample, int channels)
		{
			this.m_SamplesPerSecond = samplesPerSecond;
			this.m_BitsPerSample = bitsPerSample;
			this.m_Channels = channels;
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x00054FC0 File Offset: 0x00053FC0
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !(obj is AudioFormat);
				if (flag2)
				{
					result = false;
				}
				else
				{
					AudioFormat audioFormat = (AudioFormat)obj;
					bool flag3 = audioFormat.SamplesPerSecond != this.SamplesPerSecond;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = audioFormat.BitsPerSample != this.BitsPerSample;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = audioFormat.Channels != this.Channels;
							result = !flag5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x00055050 File Offset: 0x00054050
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06000D8E RID: 3470 RVA: 0x00055068 File Offset: 0x00054068
		public int SamplesPerSecond
		{
			get
			{
				return this.m_SamplesPerSecond;
			}
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06000D8F RID: 3471 RVA: 0x00055080 File Offset: 0x00054080
		public int BitsPerSample
		{
			get
			{
				return this.m_BitsPerSample;
			}
		}

		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06000D90 RID: 3472 RVA: 0x00055098 File Offset: 0x00054098
		public int Channels
		{
			get
			{
				return this.m_Channels;
			}
		}

		// Token: 0x040005AA RID: 1450
		private int m_SamplesPerSecond = 0;

		// Token: 0x040005AB RID: 1451
		private int m_BitsPerSample = 0;

		// Token: 0x040005AC RID: 1452
		private int m_Channels = 0;
	}
}
