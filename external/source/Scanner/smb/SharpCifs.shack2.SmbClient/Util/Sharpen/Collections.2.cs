using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200001B RID: 27
	public static class Collections
	{
		// Token: 0x060000D9 RID: 217 RVA: 0x00006924 File Offset: 0x00004B24
		public static bool AddAll<T>(ICollection<T> list, IEnumerable toAdd)
		{
			foreach (object obj in toAdd)
			{
				T item = (T)((object)obj);
				list.Add(item);
			}
			return true;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006984 File Offset: 0x00004B84
		public static TV Remove<TK, TV>(IDictionary<TK, TV> map, TK toRemove) where TK : class
		{
			TV tv;
			bool flag = map.TryGetValue(toRemove, out tv);
			TV result;
			if (flag)
			{
				map.Remove(toRemove);
				result = tv;
			}
			else
			{
				result = default(TV);
			}
			return result;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000069BC File Offset: 0x00004BBC
		public static T[] ToArray<T>(ICollection<T> list)
		{
			T[] array = new T[list.Count];
			list.CopyTo(array, 0);
			return array;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000069E4 File Offset: 0x00004BE4
		public static T[] ToArray<T>(List<object> list)
		{
			T[] array = new T[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = (T)((object)list[i]);
			}
			return array;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00006A30 File Offset: 0x00004C30
		public static TU[] ToArray<T, TU>(ICollection<T> list, TU[] res) where T : TU
		{
			bool flag = res.Length < list.Count;
			if (flag)
			{
				res = new TU[list.Count];
			}
			int num = 0;
			foreach (T t in list)
			{
				res[num++] = (TU)((object)t);
			}
			bool flag2 = res.Length > list.Count;
			if (flag2)
			{
				res[list.Count] = (TU)((object)default(T));
			}
			return res;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00006AE0 File Offset: 0x00004CE0
		public static IDictionary<TK, TV> EmptyMap<TK, TV>()
		{
			return new Dictionary<TK, TV>();
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00006AF8 File Offset: 0x00004CF8
		public static IList<T> EmptyList<T>()
		{
			return Collections<T>.EmptySet;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00006B10 File Offset: 0x00004D10
		public static ICollection<T> EmptySet<T>()
		{
			return Collections<T>.EmptySet;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00006B28 File Offset: 0x00004D28
		public static IList<T> NCopies<T>(int n, T elem)
		{
			List<T> list = new List<T>(n);
			while (n-- > 0)
			{
				list.Add(elem);
			}
			return list;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00006B5C File Offset: 0x00004D5C
		public static void Reverse<T>(IList<T> list)
		{
			int num = list.Count - 1;
			int i = 0;
			while (i < num)
			{
				T value = list[i];
				list[i] = list[num];
				list[num] = value;
				i++;
				num--;
			}
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00006BAC File Offset: 0x00004DAC
		public static ICollection<T> Singleton<T>(T item)
		{
			return new List<T>(1)
			{
				item
			};
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00006BD0 File Offset: 0x00004DD0
		public static IList<T> SingletonList<T>(T item)
		{
			return new List<T>(1)
			{
				item
			};
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006BF4 File Offset: 0x00004DF4
		public static IList<T> SynchronizedList<T>(IList<T> list)
		{
			return new SynchronizedList<T>(list);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006C0C File Offset: 0x00004E0C
		public static ICollection<T> UnmodifiableCollection<T>(ICollection<T> list)
		{
			return list;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006C20 File Offset: 0x00004E20
		public static IList<T> UnmodifiableList<T>(IList<T> list)
		{
			return new ReadOnlyCollection<T>(list);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00006C38 File Offset: 0x00004E38
		public static ICollection<T> UnmodifiableSet<T>(ICollection<T> list)
		{
			return list;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00006C4C File Offset: 0x00004E4C
		public static IDictionary<TK, TV> UnmodifiableMap<TK, TV>(IDictionary<TK, TV> dict)
		{
			return dict;
		}
	}
}
