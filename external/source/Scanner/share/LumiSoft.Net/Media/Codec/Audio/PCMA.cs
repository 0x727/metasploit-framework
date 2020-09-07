using System;

namespace LumiSoft.Net.Media.Codec.Audio
{
	// Token: 0x02000157 RID: 343
	public class PCMA : AudioCodec
	{
		// Token: 0x06000DE5 RID: 3557 RVA: 0x00056738 File Offset: 0x00055738
		public override byte[] Encode(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0 || offset > buffer.Length;
			if (flag2)
			{
				throw new ArgumentException("Argument 'offset' is out of range.");
			}
			bool flag3 = count < 1 || count + offset > buffer.Length;
			if (flag3)
			{
				throw new ArgumentException("Argument 'count' is out of range.");
			}
			bool flag4 = count % 2 != 0;
			if (flag4)
			{
				throw new ArgumentException("Invalid 'count' value, it doesn't contain 16-bit boundaries.");
			}
			int i = 0;
			byte[] array = new byte[count / 2];
			while (i < array.Length)
			{
				short sample = (short)((int)buffer[offset + 1] << 8 | (int)buffer[offset]);
				offset += 2;
				array[i++] = PCMA.LinearToALawSample(sample);
			}
			return array;
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x000567F4 File Offset: 0x000557F4
		public override byte[] Decode(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0 || offset > buffer.Length;
			if (flag2)
			{
				throw new ArgumentException("Argument 'offse't is out of range.");
			}
			bool flag3 = count < 1 || count + offset > buffer.Length;
			if (flag3)
			{
				throw new ArgumentException("Argument 'count' is out of range.");
			}
			int num = 0;
			byte[] array = new byte[count * 2];
			for (int i = offset; i < buffer.Length; i++)
			{
				short num2 = PCMA.ALawDecompressTable[(int)buffer[i]];
				array[num++] = (byte)(num2 & 255);
				array[num++] = (byte)(num2 >> 8 & 255);
			}
			return array;
		}

		// Token: 0x06000DE7 RID: 3559 RVA: 0x000568B0 File Offset: 0x000558B0
		private static byte LinearToALawSample(short sample)
		{
			int num = ~sample >> 8 & 128;
			bool flag = num == 0;
			if (flag)
			{
				sample = -sample;
			}
			bool flag2 = sample > 32635;
			if (flag2)
			{
				sample = 32635;
			}
			bool flag3 = sample >= 256;
			byte b;
			if (flag3)
			{
				int num2 = (int)PCMA.ALawCompressTable[sample >> 8 & 127];
				int num3 = sample >> num2 + 3 & 15;
				b = (byte)(num2 << 4 | num3);
			}
			else
			{
				b = (byte)(sample >> 4);
			}
			return b ^ (byte)(num ^ 85);
		}

		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06000DE8 RID: 3560 RVA: 0x00056948 File Offset: 0x00055948
		public override string Name
		{
			get
			{
				return "PCMA";
			}
		}

		// Token: 0x1700049F RID: 1183
		// (get) Token: 0x06000DE9 RID: 3561 RVA: 0x00056960 File Offset: 0x00055960
		public override AudioFormat AudioFormat
		{
			get
			{
				return this.m_pAudioFormat;
			}
		}

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06000DEA RID: 3562 RVA: 0x00056978 File Offset: 0x00055978
		public override AudioFormat CompressedAudioFormat
		{
			get
			{
				return this.m_pCompressedAudioFormat;
			}
		}

		// Token: 0x040005D2 RID: 1490
		private static readonly byte[] ALawCompressTable = new byte[]
		{
			1,
			1,
			2,
			2,
			3,
			3,
			3,
			3,
			4,
			4,
			4,
			4,
			4,
			4,
			4,
			4,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			6,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7,
			7
		};

		// Token: 0x040005D3 RID: 1491
		private static readonly short[] ALawDecompressTable = new short[]
		{
			-5504,
			-5248,
			-6016,
			-5760,
			-4480,
			-4224,
			-4992,
			-4736,
			-7552,
			-7296,
			-8064,
			-7808,
			-6528,
			-6272,
			-7040,
			-6784,
			-2752,
			-2624,
			-3008,
			-2880,
			-2240,
			-2112,
			-2496,
			-2368,
			-3776,
			-3648,
			-4032,
			-3904,
			-3264,
			-3136,
			-3520,
			-3392,
			-22016,
			-20992,
			-24064,
			-23040,
			-17920,
			-16896,
			-19968,
			-18944,
			-30208,
			-29184,
			-32256,
			-31232,
			-26112,
			-25088,
			-28160,
			-27136,
			-11008,
			-10496,
			-12032,
			-11520,
			-8960,
			-8448,
			-9984,
			-9472,
			-15104,
			-14592,
			-16128,
			-15616,
			-13056,
			-12544,
			-14080,
			-13568,
			-344,
			-328,
			-376,
			-360,
			-280,
			-264,
			-312,
			-296,
			-472,
			-456,
			-504,
			-488,
			-408,
			-392,
			-440,
			-424,
			-88,
			-72,
			-120,
			-104,
			-24,
			-8,
			-56,
			-40,
			-216,
			-200,
			-248,
			-232,
			-152,
			-136,
			-184,
			-168,
			-1376,
			-1312,
			-1504,
			-1440,
			-1120,
			-1056,
			-1248,
			-1184,
			-1888,
			-1824,
			-2016,
			-1952,
			-1632,
			-1568,
			-1760,
			-1696,
			-688,
			-656,
			-752,
			-720,
			-560,
			-528,
			-624,
			-592,
			-944,
			-912,
			-1008,
			-976,
			-816,
			-784,
			-880,
			-848,
			5504,
			5248,
			6016,
			5760,
			4480,
			4224,
			4992,
			4736,
			7552,
			7296,
			8064,
			7808,
			6528,
			6272,
			7040,
			6784,
			2752,
			2624,
			3008,
			2880,
			2240,
			2112,
			2496,
			2368,
			3776,
			3648,
			4032,
			3904,
			3264,
			3136,
			3520,
			3392,
			22016,
			20992,
			24064,
			23040,
			17920,
			16896,
			19968,
			18944,
			30208,
			29184,
			32256,
			31232,
			26112,
			25088,
			28160,
			27136,
			11008,
			10496,
			12032,
			11520,
			8960,
			8448,
			9984,
			9472,
			15104,
			14592,
			16128,
			15616,
			13056,
			12544,
			14080,
			13568,
			344,
			328,
			376,
			360,
			280,
			264,
			312,
			296,
			472,
			456,
			504,
			488,
			408,
			392,
			440,
			424,
			88,
			72,
			120,
			104,
			24,
			8,
			56,
			40,
			216,
			200,
			248,
			232,
			152,
			136,
			184,
			168,
			1376,
			1312,
			1504,
			1440,
			1120,
			1056,
			1248,
			1184,
			1888,
			1824,
			2016,
			1952,
			1632,
			1568,
			1760,
			1696,
			688,
			656,
			752,
			720,
			560,
			528,
			624,
			592,
			944,
			912,
			1008,
			976,
			816,
			784,
			880,
			848
		};

		// Token: 0x040005D4 RID: 1492
		private AudioFormat m_pAudioFormat = new AudioFormat(8000, 16, 1);

		// Token: 0x040005D5 RID: 1493
		private AudioFormat m_pCompressedAudioFormat = new AudioFormat(8000, 8, 1);
	}
}
