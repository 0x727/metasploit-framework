using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000014 RID: 20
	public class Arrays
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x00006710 File Offset: 0x00004910
		public static List<T> AsList<T>(params T[] array)
		{
			return array.ToList<T>();
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006728 File Offset: 0x00004928
		public static bool Equals<T>(T[] a1, T[] a2)
		{
			bool flag = a1.Length != a2.Length;
			return !flag && !a1.Where((T t, int i) => !t.Equals(a2[i])).Any<T>();
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000677A File Offset: 0x0000497A
		public static void Fill<T>(T[] array, T val)
		{
			Arrays.Fill<T>(array, 0, array.Length, val);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000678C File Offset: 0x0000498C
		public static void Fill<T>(T[] array, int start, int end, T val)
		{
			for (int i = start; i < end; i++)
			{
				array[i] = val;
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000067B4 File Offset: 0x000049B4
		public static void Sort(string[] array)
		{
			Array.Sort<string>(array, (string s1, string s2) => string.CompareOrdinal(s1, s2));
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000067DD File Offset: 0x000049DD
		public static void Sort<T>(T[] array)
		{
			Array.Sort<T>(array);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000067E7 File Offset: 0x000049E7
		public static void Sort<T>(T[] array, IComparer<T> c)
		{
			Array.Sort<T>(array, c);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000067F2 File Offset: 0x000049F2
		public static void Sort<T>(T[] array, int start, int count)
		{
			Array.Sort<T>(array, start, count);
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000067FE File Offset: 0x000049FE
		public static void Sort<T>(T[] array, int start, int count, IComparer<T> c)
		{
			Array.Sort<T>(array, start, count, c);
		}
	}
}
