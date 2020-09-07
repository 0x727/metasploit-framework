using System;
using System.IO;
using System.Text;
using System.Threading;

namespace LumiSoft.Net.Media
{
	// Token: 0x02000150 RID: 336
	public class WavePlayer
	{
		// Token: 0x06000DB0 RID: 3504 RVA: 0x00055BD8 File Offset: 0x00054BD8
		public WavePlayer(AudioOutDevice device)
		{
			bool flag = device == null;
			if (flag)
			{
				throw new ArgumentNullException("device");
			}
			this.m_pOutputDevice = device;
		}

		// Token: 0x06000DB1 RID: 3505 RVA: 0x00055C20 File Offset: 0x00054C20
		public void Play(string file, int count)
		{
			bool flag = file == null;
			if (flag)
			{
				throw new ArgumentNullException("file");
			}
			this.Play(File.OpenRead(file), count);
		}

		// Token: 0x06000DB2 RID: 3506 RVA: 0x00055C50 File Offset: 0x00054C50
		public void Play(Stream stream, int count)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool isPlaying = this.m_IsPlaying;
			if (isPlaying)
			{
				this.Stop();
			}
			this.m_IsPlaying = true;
			this.m_Stop = false;
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				using (BinaryReader binaryReader = new BinaryReader(stream))
				{
					WavePlayer.WavReader wavReader = new WavePlayer.WavReader(binaryReader);
					bool flag2 = !string.Equals(wavReader.Read_ChunkID(), "riff", StringComparison.InvariantCultureIgnoreCase);
					if (flag2)
					{
						throw new ArgumentNullException("Invalid wave file, RIFF header missing.");
					}
					WavePlayer.RIFF_Chunk riff_Chunk = wavReader.Read_RIFF();
					wavReader.Read_ChunkID();
					WavePlayer.fmt_Chunk fmt_Chunk = wavReader.Read_fmt();
					using (AudioOut audioOut = new AudioOut(this.m_pOutputDevice, fmt_Chunk.SampleRate, fmt_Chunk.BitsPerSample, fmt_Chunk.NumberOfChannels))
					{
						long position = binaryReader.BaseStream.Position;
						int i = 0;
						while (i < count)
						{
							binaryReader.BaseStream.Position = position;
							for (;;)
							{
								string text = wavReader.Read_ChunkID();
								bool flag3 = text == null || binaryReader.BaseStream.Length - binaryReader.BaseStream.Position < 4L;
								if (flag3)
								{
									break;
								}
								bool flag4 = string.Equals(text, "data", StringComparison.InvariantCultureIgnoreCase);
								if (flag4)
								{
									WavePlayer.data_Chunk data_Chunk = wavReader.Read_data();
									int num = 0;
									byte[] array = new byte[8000];
									while ((long)num < (long)((ulong)data_Chunk.ChunkSize))
									{
										bool stop = this.m_Stop;
										if (stop)
										{
											goto Block_9;
										}
										int num2 = binaryReader.Read(array, 0, (int)Math.Min((long)array.Length, (long)((ulong)data_Chunk.ChunkSize - (ulong)((long)num))));
										audioOut.Write(array, 0, num2);
										while (this.m_IsPlaying && audioOut.BytesBuffered >= array.Length * 2)
										{
											Thread.Sleep(10);
										}
										num += num2;
									}
								}
								else
								{
									wavReader.SkipChunk();
								}
							}
							i++;
							continue;
							Block_9:
							this.m_IsPlaying = false;
							return;
						}
						while (this.m_IsPlaying && audioOut.BytesBuffered > 0)
						{
							Thread.Sleep(10);
						}
					}
				}
				this.m_IsPlaying = false;
			});
		}

		// Token: 0x06000DB3 RID: 3507 RVA: 0x00055CC8 File Offset: 0x00054CC8
		public void Stop()
		{
			this.m_Stop = true;
			while (this.m_IsPlaying)
			{
				Thread.Sleep(5);
			}
		}

		// Token: 0x040005BF RID: 1471
		private bool m_IsPlaying = false;

		// Token: 0x040005C0 RID: 1472
		private bool m_Stop = false;

		// Token: 0x040005C1 RID: 1473
		private AudioOutDevice m_pOutputDevice = null;

		// Token: 0x0200030A RID: 778
		private class RIFF_Chunk
		{
			// Token: 0x060019D9 RID: 6617 RVA: 0x0009F180 File Offset: 0x0009E180
			public void Parse(BinaryReader reader)
			{
				bool flag = reader == null;
				if (flag)
				{
					throw new ArgumentNullException("reader");
				}
				this.m_ChunkSize = reader.ReadUInt32();
				this.m_Format = new string(reader.ReadChars(4)).Trim();
			}

			// Token: 0x17000842 RID: 2114
			// (get) Token: 0x060019DA RID: 6618 RVA: 0x0009F1C8 File Offset: 0x0009E1C8
			public string ChunkID
			{
				get
				{
					return "RIFF";
				}
			}

			// Token: 0x17000843 RID: 2115
			// (get) Token: 0x060019DB RID: 6619 RVA: 0x0009F1E0 File Offset: 0x0009E1E0
			public uint ChunkSize
			{
				get
				{
					return this.m_ChunkSize;
				}
			}

			// Token: 0x17000844 RID: 2116
			// (get) Token: 0x060019DC RID: 6620 RVA: 0x0009F1F8 File Offset: 0x0009E1F8
			public string Format
			{
				get
				{
					return this.m_Format;
				}
			}

			// Token: 0x04000B96 RID: 2966
			private uint m_ChunkSize = 0U;

			// Token: 0x04000B97 RID: 2967
			private string m_Format = "";
		}

		// Token: 0x0200030B RID: 779
		private class fmt_Chunk
		{
			// Token: 0x060019DE RID: 6622 RVA: 0x0009F24C File Offset: 0x0009E24C
			public void Parse(BinaryReader reader)
			{
				bool flag = reader == null;
				if (flag)
				{
					throw new ArgumentNullException("reader");
				}
				this.m_ChunkSize = reader.ReadUInt32();
				this.m_AudioFormat = (int)reader.ReadInt16();
				this.m_NumberOfChannels = (int)reader.ReadInt16();
				this.m_SampleRate = reader.ReadInt32();
				this.m_AvgBytesPerSec = reader.ReadInt32();
				this.m_BlockAlign = (int)reader.ReadInt16();
				this.m_BitsPerSample = (int)reader.ReadInt16();
				int num = 0;
				while ((long)num < (long)((ulong)(this.m_ChunkSize - 16U)))
				{
					reader.ReadByte();
					num++;
				}
			}

			// Token: 0x060019DF RID: 6623 RVA: 0x0009F2E8 File Offset: 0x0009E2E8
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("ChunkSize: " + this.m_ChunkSize);
				stringBuilder.AppendLine("AudioFormat: " + this.m_AudioFormat);
				stringBuilder.AppendLine("Channels: " + this.m_NumberOfChannels);
				stringBuilder.AppendLine("SampleRate: " + this.m_SampleRate);
				stringBuilder.AppendLine("AvgBytesPerSec: " + this.m_AvgBytesPerSec);
				stringBuilder.AppendLine("BlockAlign: " + this.m_BlockAlign);
				stringBuilder.AppendLine("BitsPerSample: " + this.m_BitsPerSample);
				return stringBuilder.ToString();
			}

			// Token: 0x17000845 RID: 2117
			// (get) Token: 0x060019E0 RID: 6624 RVA: 0x0009F3CC File Offset: 0x0009E3CC
			public string ChunkID
			{
				get
				{
					return "fmt";
				}
			}

			// Token: 0x17000846 RID: 2118
			// (get) Token: 0x060019E1 RID: 6625 RVA: 0x0009F3E4 File Offset: 0x0009E3E4
			public uint ChunkSize
			{
				get
				{
					return this.m_ChunkSize;
				}
			}

			// Token: 0x17000847 RID: 2119
			// (get) Token: 0x060019E2 RID: 6626 RVA: 0x0009F3FC File Offset: 0x0009E3FC
			public int AudioFormat
			{
				get
				{
					return this.m_AudioFormat;
				}
			}

			// Token: 0x17000848 RID: 2120
			// (get) Token: 0x060019E3 RID: 6627 RVA: 0x0009F414 File Offset: 0x0009E414
			public int NumberOfChannels
			{
				get
				{
					return this.m_NumberOfChannels;
				}
			}

			// Token: 0x17000849 RID: 2121
			// (get) Token: 0x060019E4 RID: 6628 RVA: 0x0009F42C File Offset: 0x0009E42C
			public int SampleRate
			{
				get
				{
					return this.m_SampleRate;
				}
			}

			// Token: 0x1700084A RID: 2122
			// (get) Token: 0x060019E5 RID: 6629 RVA: 0x0009F444 File Offset: 0x0009E444
			public int AvgBytesPerSec
			{
				get
				{
					return this.m_AvgBytesPerSec;
				}
			}

			// Token: 0x1700084B RID: 2123
			// (get) Token: 0x060019E6 RID: 6630 RVA: 0x0009F45C File Offset: 0x0009E45C
			public int BlockAlign
			{
				get
				{
					return this.m_BlockAlign;
				}
			}

			// Token: 0x1700084C RID: 2124
			// (get) Token: 0x060019E7 RID: 6631 RVA: 0x0009F474 File Offset: 0x0009E474
			public int BitsPerSample
			{
				get
				{
					return this.m_BitsPerSample;
				}
			}

			// Token: 0x04000B98 RID: 2968
			private uint m_ChunkSize = 0U;

			// Token: 0x04000B99 RID: 2969
			private int m_AudioFormat = 0;

			// Token: 0x04000B9A RID: 2970
			private int m_NumberOfChannels = 0;

			// Token: 0x04000B9B RID: 2971
			private int m_SampleRate = 0;

			// Token: 0x04000B9C RID: 2972
			private int m_AvgBytesPerSec = 0;

			// Token: 0x04000B9D RID: 2973
			private int m_BlockAlign = 0;

			// Token: 0x04000B9E RID: 2974
			private int m_BitsPerSample = 0;
		}

		// Token: 0x0200030C RID: 780
		private class data_Chunk
		{
			// Token: 0x060019E9 RID: 6633 RVA: 0x0009F4A0 File Offset: 0x0009E4A0
			public void Parse(BinaryReader reader)
			{
				bool flag = reader == null;
				if (flag)
				{
					throw new ArgumentNullException("reader");
				}
				this.m_ChunkSize = reader.ReadUInt32();
			}

			// Token: 0x1700084D RID: 2125
			// (get) Token: 0x060019EA RID: 6634 RVA: 0x0009F4D0 File Offset: 0x0009E4D0
			public string ChunkID
			{
				get
				{
					return "data";
				}
			}

			// Token: 0x1700084E RID: 2126
			// (get) Token: 0x060019EB RID: 6635 RVA: 0x0009F4E8 File Offset: 0x0009E4E8
			public uint ChunkSize
			{
				get
				{
					return this.m_ChunkSize;
				}
			}

			// Token: 0x04000B9F RID: 2975
			private uint m_ChunkSize = 0U;
		}

		// Token: 0x0200030D RID: 781
		private class WavReader
		{
			// Token: 0x060019EC RID: 6636 RVA: 0x0009F500 File Offset: 0x0009E500
			public WavReader(BinaryReader reader)
			{
				bool flag = reader == null;
				if (flag)
				{
					throw new ArgumentNullException("reader");
				}
				this.m_pBinaryReader = reader;
			}

			// Token: 0x060019ED RID: 6637 RVA: 0x0009F538 File Offset: 0x0009E538
			public string Read_ChunkID()
			{
				char[] array = this.m_pBinaryReader.ReadChars(4);
				bool flag = array.Length == 0;
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = new string(array).Trim();
				}
				return result;
			}

			// Token: 0x060019EE RID: 6638 RVA: 0x0009F574 File Offset: 0x0009E574
			public WavePlayer.RIFF_Chunk Read_RIFF()
			{
				WavePlayer.RIFF_Chunk riff_Chunk = new WavePlayer.RIFF_Chunk();
				riff_Chunk.Parse(this.m_pBinaryReader);
				return riff_Chunk;
			}

			// Token: 0x060019EF RID: 6639 RVA: 0x0009F59C File Offset: 0x0009E59C
			public WavePlayer.fmt_Chunk Read_fmt()
			{
				WavePlayer.fmt_Chunk fmt_Chunk = new WavePlayer.fmt_Chunk();
				fmt_Chunk.Parse(this.m_pBinaryReader);
				return fmt_Chunk;
			}

			// Token: 0x060019F0 RID: 6640 RVA: 0x0009F5C4 File Offset: 0x0009E5C4
			public WavePlayer.data_Chunk Read_data()
			{
				WavePlayer.data_Chunk data_Chunk = new WavePlayer.data_Chunk();
				data_Chunk.Parse(this.m_pBinaryReader);
				return data_Chunk;
			}

			// Token: 0x060019F1 RID: 6641 RVA: 0x0009F5EC File Offset: 0x0009E5EC
			public void SkipChunk()
			{
				uint num = this.m_pBinaryReader.ReadUInt32();
				this.m_pBinaryReader.BaseStream.Position += (long)((ulong)num);
			}

			// Token: 0x04000BA0 RID: 2976
			private BinaryReader m_pBinaryReader = null;
		}
	}
}
