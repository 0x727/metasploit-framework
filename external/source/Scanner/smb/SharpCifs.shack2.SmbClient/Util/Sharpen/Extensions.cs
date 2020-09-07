using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200003B RID: 59
	public static class Extensions
	{
		// Token: 0x0600012A RID: 298 RVA: 0x00007278 File Offset: 0x00005478
		static Extensions()
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			Extensions.EpochTicks = dateTime.Ticks;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000072A4 File Offset: 0x000054A4
		public static void Add<T>(this IList<T> list, int index, T item)
		{
			list.Insert(index, item);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000072B0 File Offset: 0x000054B0
		public static void AddFirst<T>(this IList<T> list, T item)
		{
			list.Insert(0, item);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000072BC File Offset: 0x000054BC
		public static void AddLast<T>(this IList<T> list, T item)
		{
			list.Add(item);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x000072C8 File Offset: 0x000054C8
		public static void RemoveLast<T>(this IList<T> list)
		{
			bool flag = list.Count > 0;
			if (flag)
			{
				list.Remove(list.Count - 1);
			}
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000072F4 File Offset: 0x000054F4
		public static StringBuilder AppendRange(this StringBuilder sb, string str, int start, int end)
		{
			return sb.Append(str, start, end - start);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00007314 File Offset: 0x00005514
		public static StringBuilder Delete(this StringBuilder sb, int start, int end)
		{
			return sb.Remove(start, end - start);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00007330 File Offset: 0x00005530
		public static void SetCharAt(this StringBuilder sb, int index, char c)
		{
			sb[index] = c;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000733C File Offset: 0x0000553C
		public static int IndexOf(this StringBuilder sb, string str)
		{
			return sb.ToString().IndexOf(str);
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000735C File Offset: 0x0000555C
		public static int BitCount(int val)
		{
			uint num = (uint)val;
			int num2 = 0;
			for (int i = 0; i < 32; i++)
			{
				bool flag = (num & 1U) > 0U;
				if (flag)
				{
					num2++;
				}
				num >>= 1;
			}
			return num2;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000073A0 File Offset: 0x000055A0
		public static IndexOutOfRangeException CreateIndexOutOfRangeException(int index)
		{
			return new IndexOutOfRangeException("Index: " + index);
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000073C8 File Offset: 0x000055C8
		public static string Decode(this Encoding e, byte[] chars, int start, int len)
		{
			string @string;
			try
			{
				byte[] preamble = e.GetPreamble();
				bool flag = preamble != null && preamble.Length != 0;
				if (flag)
				{
					bool flag2 = len >= preamble.Length;
					if (flag2)
					{
						int num = start;
						bool flag3 = true;
						int num2 = 0;
						while (num2 < preamble.Length && flag3)
						{
							bool flag4 = preamble[num2] != chars[num++];
							if (flag4)
							{
								flag3 = false;
							}
							num2++;
						}
						bool flag5 = flag3;
						if (flag5)
						{
							len -= num - start;
							start = num;
						}
					}
				}
				@string = e.GetString(chars, start, len);
			}
			catch (DecoderFallbackException)
			{
				throw new CharacterCodingException();
			}
			return @string;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007474 File Offset: 0x00005674
		public static Encoding GetEncoding(string name)
		{
			Encoding result;
			try
			{
				Encoding encoding = Encoding.GetEncoding(name.Replace('_', '-'));
				bool flag = encoding is UTF8Encoding;
				if (flag)
				{
					result = new UTF8Encoding(false, true);
				}
				else
				{
					result = encoding;
				}
			}
			catch (ArgumentException)
			{
				throw new UnsupportedCharsetException(name);
			}
			return result;
		}

		// Token: 0x06000137 RID: 311 RVA: 0x000074C8 File Offset: 0x000056C8
		public static ICollection<KeyValuePair<T, TU>> EntrySet<T, TU>(this IDictionary<T, TU> s)
		{
			return s;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x000074DC File Offset: 0x000056DC
		public static bool AddItem<T>(this IList<T> list, T item)
		{
			list.Add(item);
			return true;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x000074F8 File Offset: 0x000056F8
		public static bool AddItem<T>(this ICollection<T> list, T item)
		{
			list.Add(item);
			return true;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00007514 File Offset: 0x00005714
		public static TU Get<T, TU>(this IDictionary<T, TU> d, T key)
		{
			TU result;
			d.TryGetValue(key, out result);
			return result;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00007534 File Offset: 0x00005734
		public static TU Put<T, TU>(this IDictionary<T, TU> d, T key, TU value)
		{
			TU result;
			d.TryGetValue(key, out result);
			d[key] = value;
			return result;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000755C File Offset: 0x0000575C
		public static void PutAll<T, TU>(this IDictionary<T, TU> d, IDictionary<T, TU> values)
		{
			foreach (KeyValuePair<T, TU> keyValuePair in values)
			{
				d[keyValuePair.Key] = keyValuePair.Value;
			}
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000075B8 File Offset: 0x000057B8
		public static CultureInfo GetEnglishCulture()
		{
			return new CultureInfo("en-US");
		}

		// Token: 0x0600013E RID: 318 RVA: 0x000075D4 File Offset: 0x000057D4
		public static T GetFirst<T>(this IList<T> list)
		{
			return (list.Count == 0) ? default(T) : list[0];
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00007600 File Offset: 0x00005800
		public static CultureInfo GetGermanCulture()
		{
			return new CultureInfo("de-DE");
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00007620 File Offset: 0x00005820
		public static T GetLast<T>(this IList<T> list)
		{
			return (list.Count == 0) ? default(T) : list[list.Count - 1];
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00007654 File Offset: 0x00005854
		public static int GetOffset(this TimeZoneInfo tzone, long date)
		{
			return (int)tzone.GetUtcOffset(Extensions.MillisToDateTimeOffset(date, 0L).DateTime).TotalMilliseconds;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00007688 File Offset: 0x00005888
		public static InputStream GetResourceAsStream(this Type type, string name)
		{
			string text = type.Assembly.GetName().Name + ".resources";
			string[] values = new string[]
			{
				text,
				".",
				type.Namespace,
				".",
				name
			};
			string name2 = string.Concat(values);
			Stream manifestResourceStream = type.Assembly.GetManifestResourceStream(name2);
			bool flag = manifestResourceStream == null;
			InputStream result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = InputStream.Wrap(manifestResourceStream);
			}
			return result;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000770C File Offset: 0x0000590C
		public static long GetTime(this DateTime dateTime)
		{
			return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero).ToMillisecondsSinceEpoch();
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00007734 File Offset: 0x00005934
		public static void InitCause(this Exception ex, Exception cause)
		{
			Console.WriteLine(cause);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00007740 File Offset: 0x00005940
		public static bool IsEmpty<T>(this ICollection<T> col)
		{
			return col.Count == 0;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000775C File Offset: 0x0000595C
		public static bool IsEmpty<T>(this Stack<T> col)
		{
			return col.Count == 0;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00007778 File Offset: 0x00005978
		public static bool IsLower(this char c)
		{
			return char.IsLower(c);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00007790 File Offset: 0x00005990
		public static bool IsUpper(this char c)
		{
			return char.IsUpper(c);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000077A8 File Offset: 0x000059A8
		public static Iterator<T> Iterator<T>(this ICollection<T> col)
		{
			return new EnumeratorWrapper<T>(col, col.GetEnumerator());
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000077C8 File Offset: 0x000059C8
		public static Iterator<T> Iterator<T>(this IEnumerable<T> col)
		{
			return new EnumeratorWrapper<T>(col, col.GetEnumerator());
		}

		// Token: 0x0600014B RID: 331 RVA: 0x000077E8 File Offset: 0x000059E8
		public static T Last<T>(this ICollection<T> col)
		{
			IList<T> list = col as IList<T>;
			bool flag = list != null;
			T result;
			if (flag)
			{
				result = list[list.Count - 1];
			}
			else
			{
				result = col.Last<T>();
			}
			return result;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00007824 File Offset: 0x00005A24
		public static int LowestOneBit(int val)
		{
			return 1 << Extensions.NumberOfTrailingZeros(val);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00007844 File Offset: 0x00005A44
		public static bool Matches(this string str, string regex)
		{
			Regex regex2 = new Regex(regex);
			return regex2.IsMatch(str);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00007864 File Offset: 0x00005A64
		public static DateTime CreateDate(long milliSecondsSinceEpoch)
		{
			long ticks = Extensions.EpochTicks + milliSecondsSinceEpoch * 10000L;
			return new DateTime(ticks);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000788C File Offset: 0x00005A8C
		public static DateTime CreateDateFromUTC(long milliSecondsSinceEpoch)
		{
			long ticks = Extensions.EpochTicks + milliSecondsSinceEpoch * 10000L;
			return new DateTime(ticks, DateTimeKind.Utc);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x000078B4 File Offset: 0x00005AB4
		public static DateTimeOffset MillisToDateTimeOffset(long milliSecondsSinceEpoch, long offsetMinutes)
		{
			TimeSpan offset = TimeSpan.FromMinutes((double)offsetMinutes);
			long num = Extensions.EpochTicks + milliSecondsSinceEpoch * 10000L;
			return new DateTimeOffset(num + offset.Ticks, offset);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x000078EC File Offset: 0x00005AEC
		public static int NumberOfLeadingZeros(int val)
		{
			uint num = (uint)val;
			int num2 = 0;
			while ((num & 2147483648U) == 0U)
			{
				num <<= 1;
				num2++;
			}
			return num2;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00007920 File Offset: 0x00005B20
		public static int NumberOfTrailingZeros(int val)
		{
			uint num = (uint)val;
			int num2 = 0;
			while ((num & 1U) == 0U)
			{
				num >>= 1;
				num2++;
			}
			return num2;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00007950 File Offset: 0x00005B50
		public static int Read(this StreamReader reader, char[] data)
		{
			return reader.Read(data, 0, data.Length);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00007970 File Offset: 0x00005B70
		public static T Remove<T>(this IList<T> list, T item)
		{
			int num = list.IndexOf(item);
			bool flag = num == -1;
			T result;
			if (flag)
			{
				result = default(T);
			}
			else
			{
				T t = list[num];
				list.RemoveAt(num);
				result = t;
			}
			return result;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x000079B4 File Offset: 0x00005BB4
		public static T Remove<T>(this IList<T> list, int i)
		{
			T result;
			try
			{
				result = list[i];
				list.RemoveAt(i);
			}
			catch (IndexOutOfRangeException)
			{
				throw new NoSuchElementException();
			}
			return result;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x000079F4 File Offset: 0x00005BF4
		public static T RemoveFirst<T>(this IList<T> list)
		{
			return list.Remove(0);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00007A10 File Offset: 0x00005C10
		public static string ReplaceAll(this string str, string regex, string replacement)
		{
			Regex regex2 = new Regex(regex);
			bool flag = replacement.IndexOfAny(new char[]
			{
				'\\',
				'$'
			}) != -1;
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < replacement.Length; i++)
				{
					char c = replacement[i];
					bool flag2 = c == '$';
					if (flag2)
					{
						throw new NotSupportedException("Back references not supported");
					}
					bool flag3 = c == '\\';
					if (flag3)
					{
						c = replacement[++i];
					}
					stringBuilder.Append(c);
				}
				replacement = stringBuilder.ToString();
			}
			return regex2.Replace(str, replacement);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00007AC0 File Offset: 0x00005CC0
		public static bool RegionMatches(this string str, bool ignoreCase, int toOffset, string other, int ooffset, int len)
		{
			bool flag = toOffset < 0 || ooffset < 0 || toOffset + len > str.Length || ooffset + len > other.Length;
			return !flag && string.Compare(str, toOffset, other, ooffset, len) == 0;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00007B10 File Offset: 0x00005D10
		public static T Set<T>(this IList<T> list, int index, T item)
		{
			T result = list[index];
			list[index] = item;
			return result;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00007B34 File Offset: 0x00005D34
		public static int Signum(long val)
		{
			bool flag = val < 0L;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = val > 0L;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00007B64 File Offset: 0x00005D64
		public static void RemoveAll<T, TU>(this ICollection<T> col, ICollection<TU> items) where TU : T
		{
			foreach (TU tu in items)
			{
				col.Remove((T)((object)tu));
			}
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00007BBC File Offset: 0x00005DBC
		public static bool ContainsAll<T, TU>(this ICollection<T> col, ICollection<TU> items) where TU : T
		{
			using (IEnumerator<TU> enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TU u = enumerator.Current;
					bool flag = !col.Any((T n) =>/* n == u ||*/ n.Equals(u));
					if (flag)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00007C30 File Offset: 0x00005E30
		public static bool Contains<T>(this ICollection<T> col, object item)
		{
			bool flag = !(item is T);
			return !flag && col.Any((T n) => /*n == item ||*/ n.Equals(item));
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00007C7C File Offset: 0x00005E7C
		public static void Sort<T>(this IList<T> list)
		{
			List<T> list2 = new List<T>(list);
			list2.Sort();
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = list2[i];
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00007CC0 File Offset: 0x00005EC0
		public static void Sort<T>(this IList<T> list, IComparer<T> comparer)
		{
			List<T> list2 = new List<T>(list);
			list2.Sort(comparer);
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = list2[i];
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00007D04 File Offset: 0x00005F04
		public static string[] Split(this string str, string regex)
		{
			return str.Split(regex, 0);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00007D20 File Offset: 0x00005F20
		public static string[] Split(this string str, string regex, int limit)
		{
			Regex regex2 = new Regex(regex);
			List<string> list = new List<string>();
			int num = 0;
			bool flag = limit != 1;
			if (flag)
			{
				int num2 = 1;
				foreach (object obj in regex2.Matches(str))
				{
					Match match = (Match)obj;
					list.Add(str.Substring(num, match.Index - num));
					num = match.Index + match.Length;
					bool flag2 = limit > 0 && ++num2 == limit;
					if (flag2)
					{
						break;
					}
				}
			}
			bool flag3 = num < str.Length;
			if (flag3)
			{
				list.Add(str.Substring(num));
			}
			bool flag4 = limit >= 0;
			if (flag4)
			{
				int num3 = list.Count - 1;
				while (num3 >= 0 && list[num3].Length == 0)
				{
					num3--;
				}
				list.RemoveRange(num3 + 1, list.Count - num3 - 1);
			}
			return list.ToArray();
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00007E64 File Offset: 0x00006064
		public static IList<T> SubList<T>(this IList<T> list, int start, int len)
		{
			List<T> list2 = new List<T>(len);
			for (int i = start; i < start + len; i++)
			{
				list2.Add(list[i]);
			}
			return list2;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00007EA0 File Offset: 0x000060A0
		public static char[] ToCharArray(this string str)
		{
			char[] array = new char[str.Length];
			str.CopyTo(0, array, 0, str.Length);
			return array;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00007ED0 File Offset: 0x000060D0
		public static long ToMillisecondsSinceEpoch(this DateTime dateTime)
		{
			bool flag = dateTime.Kind != DateTimeKind.Utc;
			if (flag)
			{
				throw new ArgumentException("dateTime is expected to be expressed as a UTC DateTime", "dateTime");
			}
			return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero).ToMillisecondsSinceEpoch();
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00007F1C File Offset: 0x0000611C
		public static long ToMillisecondsSinceEpoch(this DateTimeOffset dateTimeOffset)
		{
			return (dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks - Extensions.EpochTicks) / 10000L;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00007F54 File Offset: 0x00006154
		public static string ToOctalString(int val)
		{
			return Convert.ToString(val, 8);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00007F70 File Offset: 0x00006170
		public static string ToHexString(int val)
		{
			return Convert.ToString(val, 16);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00007F8C File Offset: 0x0000618C
		public static string ToString(object val)
		{
			return val.ToString();
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00007FA4 File Offset: 0x000061A4
		public static string ToString(int val, int bas)
		{
			return Convert.ToString(val, bas);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00007FC0 File Offset: 0x000061C0
		public static IList<TU> UpcastTo<T, TU>(this IList<T> s) where T : TU
		{
			List<TU> list = new List<TU>(s.Count);
			for (int i = 0; i < s.Count; i++)
			{
				list.Add((TU)((object)s[i]));
			}
			return list;
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00008010 File Offset: 0x00006210
		public static ICollection<TU> UpcastTo<T, TU>(this ICollection<T> s) where T : TU
		{
			List<TU> list = new List<TU>(s.Count);
			foreach (T t in s)
			{
				list.Add((TU)((object)t));
			}
			return list;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000807C File Offset: 0x0000627C
		public static T ValueOf<T>(T val)
		{
			return val;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00008090 File Offset: 0x00006290
		public static string GetTestName(object obj)
		{
			return Extensions.GetTestName();
		}

		// Token: 0x0600016E RID: 366 RVA: 0x000080A8 File Offset: 0x000062A8
		public static string GetTestName()
		{
			int num = 0;
			MethodBase method;
			for (;;)
			{
				method = new StackFrame(num).GetMethod();
				bool flag = method != null;
				if (flag)
				{
					foreach (Attribute attribute in method.GetCustomAttributes(true))
					{
						bool flag2 = attribute.GetType().FullName == "NUnit.Framework.TestAttribute";
						if (flag2)
						{
							goto Block_2;
						}
					}
				}
				num++;
				if (!(method != null))
				{
					goto Block_4;
				}
			}
			Block_2:
			string text = method.Name;
			bool flag3 = char.IsUpper(text[0]);
			if (flag3)
			{
				text = char.ToLower(text[0]).ToString() + text.Substring(1);
			}
			return text;
			Block_4:
			return "";
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00008180 File Offset: 0x00006380
		public static string GetHostAddress(this IPAddress addr)
		{
			return addr.ToString();
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00008198 File Offset: 0x00006398
		public static IPAddress GetAddressByName(string host)
		{
			bool flag = host == "0.0.0.0";
			IPAddress result;
			if (flag)
			{
				result = IPAddress.Any;
			}
			else
			{
				try
				{
					result = IPAddress.Parse(host);
				}
				catch (Exception ex)
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x000081E0 File Offset: 0x000063E0
		public static IPAddress[] GetAddressesByName(string host)
		{
			return Dns.GetHostAddresses(host);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000081F8 File Offset: 0x000063F8
		public static string GetImplementationVersion(this Assembly asm)
		{
			return asm.GetName().Version.ToString();
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000821C File Offset: 0x0000641C
		public static string GetHost(this Uri uri)
		{
			return string.IsNullOrEmpty(uri.Host) ? "" : uri.Host;
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00008248 File Offset: 0x00006448
		public static string GetUserInfo(this Uri uri)
		{
			return string.IsNullOrEmpty(uri.UserInfo) ? null : uri.UserInfo;
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00008270 File Offset: 0x00006470
		public static string GetQuery(this Uri uri)
		{
			return string.IsNullOrEmpty(uri.Query) ? null : uri.Query;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00008298 File Offset: 0x00006498
		public static int GetLocalPort(this Socket socket)
		{
			return ((IPEndPoint)socket.LocalEndPoint).Port;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x000082BC File Offset: 0x000064BC
		public static int GetPort(this Socket socket)
		{
			return ((IPEndPoint)socket.RemoteEndPoint).Port;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000082E0 File Offset: 0x000064E0
		public static IPAddress GetInetAddress(this Socket socket)
		{
			return ((IPEndPoint)socket.RemoteEndPoint).Address;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00008304 File Offset: 0x00006504
		public static Semaphore CreateSemaphore(int count)
		{
			return new Semaphore(count, int.MaxValue);
		}

		// Token: 0x04000058 RID: 88
		private static readonly long EpochTicks;
	}
}
