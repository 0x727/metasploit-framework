using System;
using System.Text;

namespace LumiSoft.Net
{
	// Token: 0x02000017 RID: 23
	internal class BitDebuger
	{
		// Token: 0x0600006E RID: 110 RVA: 0x000043F4 File Offset: 0x000033F4
		public static string ToBit(byte[] buffer, int count, int bytesPerLine)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			int num = 1;
			while (i < count)
			{
				byte b = buffer[i];
				char[] array = new char[8];
				for (int j = 7; j >= 0; j--)
				{
					array[j] = (b >> 7 - j & 1).ToString()[0];
				}
				stringBuilder.Append(array);
				bool flag2 = num == bytesPerLine;
				if (flag2)
				{
					stringBuilder.AppendLine();
					num = 0;
				}
				else
				{
					stringBuilder.Append(" ");
				}
				num++;
				i++;
			}
			return stringBuilder.ToString();
		}
	}
}
