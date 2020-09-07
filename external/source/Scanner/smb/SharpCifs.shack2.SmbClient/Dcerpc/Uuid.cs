using System;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000E8 RID: 232
	public class Uuid : Rpc.UuidT
	{
		// Token: 0x0600079B RID: 1947 RVA: 0x00029CB4 File Offset: 0x00027EB4
		public static int Hex_to_bin(char[] arr, int offset, int length)
		{
			int num = 0;
			int num2 = 0;
			int num3 = offset;
			while (num3 < arr.Length && num2 < length)
			{
				num <<= 4;
				char c = arr[num3];
				switch (c)
				{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					num += (int)(arr[num3] - '0');
					break;
				case ':':
				case ';':
				case '<':
				case '=':
				case '>':
				case '?':
				case '@':
					goto IL_C9;
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
					num += (int)('\n' + (arr[num3] - 'A'));
					break;
				default:
					switch (c)
					{
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
						num += (int)('\n' + (arr[num3] - 'a'));
						break;
					default:
						goto IL_C9;
					}
					break;
				}
				num2++;
				num3++;
				continue;
				IL_C9:
				throw new ArgumentException(new string(arr, offset, length));
			}
			return num;
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00029DC0 File Offset: 0x00027FC0
		public static string Bin_to_hex(int value, int length)
		{
			char[] array = new char[length];
			int num = array.Length;
			while (num-- > 0)
			{
				array[num] = Uuid.Hexchars[value & 15];
				value = (int)((uint)value >> 4);
			}
			return new string(array);
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00029E08 File Offset: 0x00028008
		private static byte B(int i)
		{
			return (byte)(i & 255);
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x00029E24 File Offset: 0x00028024
		private static short S(int i)
		{
			return (short)(i & 65535);
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x00029E40 File Offset: 0x00028040
		public Uuid(Rpc.UuidT uuid)
		{
			this.TimeLow = uuid.TimeLow;
			this.TimeMid = uuid.TimeMid;
			this.TimeHiAndVersion = uuid.TimeHiAndVersion;
			this.ClockSeqHiAndReserved = uuid.ClockSeqHiAndReserved;
			this.ClockSeqLow = uuid.ClockSeqLow;
			this.Node = new byte[6];
			this.Node[0] = uuid.Node[0];
			this.Node[1] = uuid.Node[1];
			this.Node[2] = uuid.Node[2];
			this.Node[3] = uuid.Node[3];
			this.Node[4] = uuid.Node[4];
			this.Node[5] = uuid.Node[5];
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x00029F00 File Offset: 0x00028100
		public Uuid(string str)
		{
			char[] arr = str.ToCharArray();
			this.TimeLow = Uuid.Hex_to_bin(arr, 0, 8);
			this.TimeMid = Uuid.S(Uuid.Hex_to_bin(arr, 9, 4));
			this.TimeHiAndVersion = Uuid.S(Uuid.Hex_to_bin(arr, 14, 4));
			this.ClockSeqHiAndReserved = Uuid.B(Uuid.Hex_to_bin(arr, 19, 2));
			this.ClockSeqLow = Uuid.B(Uuid.Hex_to_bin(arr, 21, 2));
			this.Node = new byte[6];
			this.Node[0] = Uuid.B(Uuid.Hex_to_bin(arr, 24, 2));
			this.Node[1] = Uuid.B(Uuid.Hex_to_bin(arr, 26, 2));
			this.Node[2] = Uuid.B(Uuid.Hex_to_bin(arr, 28, 2));
			this.Node[3] = Uuid.B(Uuid.Hex_to_bin(arr, 30, 2));
			this.Node[4] = Uuid.B(Uuid.Hex_to_bin(arr, 32, 2));
			this.Node[5] = Uuid.B(Uuid.Hex_to_bin(arr, 34, 2));
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x0002A00C File Offset: 0x0002820C
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				Uuid.Bin_to_hex(this.TimeLow, 8),
				"-",
				Uuid.Bin_to_hex((int)this.TimeMid, 4),
				"-",
				Uuid.Bin_to_hex((int)this.TimeHiAndVersion, 4),
				"-",
				Uuid.Bin_to_hex((int)this.ClockSeqHiAndReserved, 2),
				Uuid.Bin_to_hex((int)this.ClockSeqLow, 2),
				"-",
				Uuid.Bin_to_hex((int)this.Node[0], 2),
				Uuid.Bin_to_hex((int)this.Node[1], 2),
				Uuid.Bin_to_hex((int)this.Node[2], 2),
				Uuid.Bin_to_hex((int)this.Node[3], 2),
				Uuid.Bin_to_hex((int)this.Node[4], 2),
				Uuid.Bin_to_hex((int)this.Node[5], 2)
			});
		}

		// Token: 0x040004EE RID: 1262
		internal static readonly char[] Hexchars = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};
	}
}
