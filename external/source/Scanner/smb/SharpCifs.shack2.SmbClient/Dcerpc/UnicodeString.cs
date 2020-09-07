using System;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000E7 RID: 231
	public class UnicodeString : Rpc.Unicode_string
	{
		// Token: 0x06000797 RID: 1943 RVA: 0x00029B90 File Offset: 0x00027D90
		public UnicodeString(bool zterm)
		{
			this.Zterm = zterm;
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x00029BA1 File Offset: 0x00027DA1
		public UnicodeString(Rpc.Unicode_string rus, bool zterm)
		{
			this.Length = rus.Length;
			this.MaximumLength = rus.MaximumLength;
			this.Buffer = rus.Buffer;
			this.Zterm = zterm;
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x00029BD8 File Offset: 0x00027DD8
		public UnicodeString(string str, bool zterm)
		{
			this.Zterm = zterm;
			int length = str.Length;
			int num = zterm ? 1 : 0;
			this.Length = (this.MaximumLength = (short)((length + num) * 2));
			this.Buffer = new short[length + num];
			int i;
			for (i = 0; i < length; i++)
			{
				this.Buffer[i] = (short)str[i];
			}
			if (zterm)
			{
				this.Buffer[i] = 0;
			}
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00029C5C File Offset: 0x00027E5C
		public override string ToString()
		{
			int num = (int)(this.Length / 2 - (this.Zterm ? 1 : 0));
			char[] array = new char[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (char)this.Buffer[i];
			}
			return new string(array, 0, num);
		}

		// Token: 0x040004ED RID: 1261
		internal bool Zterm;
	}
}
