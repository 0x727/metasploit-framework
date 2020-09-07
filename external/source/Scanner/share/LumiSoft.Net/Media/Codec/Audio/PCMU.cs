using System;

namespace LumiSoft.Net.Media.Codec.Audio
{
	// Token: 0x02000158 RID: 344
	public class PCMU : AudioCodec
	{
		// Token: 0x06000DED RID: 3565 RVA: 0x000569F8 File Offset: 0x000559F8
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
				throw new ArgumentException("Invalid buffer value, it doesn't contain 16-bit boundaries.");
			}
			int i = 0;
			byte[] array = new byte[count / 2];
			while (i < array.Length)
			{
				short sample = (short)((int)buffer[offset + 1] << 8 | (int)buffer[offset]);
				offset += 2;
				array[i++] = PCMU.LinearToMuLawSample(sample);
			}
			return array;
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x00056AB4 File Offset: 0x00055AB4
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
				throw new ArgumentException("Argument 'offset' is out of range.");
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
				short num2 = PCMU.MuLawDecompressTable[(int)buffer[i]];
				array[num++] = (byte)(num2 & 255);
				array[num++] = (byte)(num2 >> 8 & 255);
			}
			return array;
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x00056B70 File Offset: 0x00055B70
		private static byte LinearToMuLawSample(short sample)
		{
			int num = 132;
			int num2 = 32635;
			int num3 = sample >> 8 & 128;
			bool flag = num3 != 0;
			if (flag)
			{
				sample = -sample;
			}
			bool flag2 = (int)sample > num2;
			if (flag2)
			{
				sample = (short)num2;
			}
			sample = (short)((int)sample + num);
			int num4 = (int)PCMU.MuLawCompressTable[sample >> 7 & 255];
			int num5 = sample >> num4 + 3 & 15;
			int num6 = ~(num3 | num4 << 4 | num5);
			return (byte)num6;
		}

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x00056BEC File Offset: 0x00055BEC
		public override string Name
		{
			get
			{
				return "PCMU";
			}
		}

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06000DF1 RID: 3569 RVA: 0x00056C04 File Offset: 0x00055C04
		public override AudioFormat AudioFormat
		{
			get
			{
				return this.m_pAudioFormat;
			}
		}

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x00056C1C File Offset: 0x00055C1C
		public override AudioFormat CompressedAudioFormat
		{
			get
			{
				return this.m_pCompressedAudioFormat;
			}
		}

		// Token: 0x040005D6 RID: 1494
		private static readonly byte[] MuLawCompressTable = new byte[]
		{
			0,
			0,
			1,
			1,
			2,
			2,
			2,
			2,
			3,
			3,
			3,
			3,
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
			7,
			7
		};

		// Token: 0x040005D7 RID: 1495
		private static readonly short[] MuLawDecompressTable = new short[]
		{
			-32124,
			-31100,
			-30076,
			-29052,
			-28028,
			-27004,
			-25980,
			-24956,
			-23932,
			-22908,
			-21884,
			-20860,
			-19836,
			-18812,
			-17788,
			-16764,
			-15996,
			-15484,
			-14972,
			-14460,
			-13948,
			-13436,
			-12924,
			-12412,
			-11900,
			-11388,
			-10876,
			-10364,
			-9852,
			-9340,
			-8828,
			-8316,
			-7932,
			-7676,
			-7420,
			-7164,
			-6908,
			-6652,
			-6396,
			-6140,
			-5884,
			-5628,
			-5372,
			-5116,
			-4860,
			-4604,
			-4348,
			-4092,
			-3900,
			-3772,
			-3644,
			-3516,
			-3388,
			-3260,
			-3132,
			-3004,
			-2876,
			-2748,
			-2620,
			-2492,
			-2364,
			-2236,
			-2108,
			-1980,
			-1884,
			-1820,
			-1756,
			-1692,
			-1628,
			-1564,
			-1500,
			-1436,
			-1372,
			-1308,
			-1244,
			-1180,
			-1116,
			-1052,
			-988,
			-924,
			-876,
			-844,
			-812,
			-780,
			-748,
			-716,
			-684,
			-652,
			-620,
			-588,
			-556,
			-524,
			-492,
			-460,
			-428,
			-396,
			-372,
			-356,
			-340,
			-324,
			-308,
			-292,
			-276,
			-260,
			-244,
			-228,
			-212,
			-196,
			-180,
			-164,
			-148,
			-132,
			-120,
			-112,
			-104,
			-96,
			-88,
			-80,
			-72,
			-64,
			-56,
			-48,
			-40,
			-32,
			-24,
			-16,
			-8,
			0,
			32124,
			31100,
			30076,
			29052,
			28028,
			27004,
			25980,
			24956,
			23932,
			22908,
			21884,
			20860,
			19836,
			18812,
			17788,
			16764,
			15996,
			15484,
			14972,
			14460,
			13948,
			13436,
			12924,
			12412,
			11900,
			11388,
			10876,
			10364,
			9852,
			9340,
			8828,
			8316,
			7932,
			7676,
			7420,
			7164,
			6908,
			6652,
			6396,
			6140,
			5884,
			5628,
			5372,
			5116,
			4860,
			4604,
			4348,
			4092,
			3900,
			3772,
			3644,
			3516,
			3388,
			3260,
			3132,
			3004,
			2876,
			2748,
			2620,
			2492,
			2364,
			2236,
			2108,
			1980,
			1884,
			1820,
			1756,
			1692,
			1628,
			1564,
			1500,
			1436,
			1372,
			1308,
			1244,
			1180,
			1116,
			1052,
			988,
			924,
			876,
			844,
			812,
			780,
			748,
			716,
			684,
			652,
			620,
			588,
			556,
			524,
			492,
			460,
			428,
			396,
			372,
			356,
			340,
			324,
			308,
			292,
			276,
			260,
			244,
			228,
			212,
			196,
			180,
			164,
			148,
			132,
			120,
			112,
			104,
			96,
			88,
			80,
			72,
			64,
			56,
			48,
			40,
			32,
			24,
			16,
			8,
			0
		};

		// Token: 0x040005D8 RID: 1496
		private AudioFormat m_pAudioFormat = new AudioFormat(8000, 16, 1);

		// Token: 0x040005D9 RID: 1497
		private AudioFormat m_pCompressedAudioFormat = new AudioFormat(8000, 8, 1);
	}
}
