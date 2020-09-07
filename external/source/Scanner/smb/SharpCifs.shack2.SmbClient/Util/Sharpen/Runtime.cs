using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000062 RID: 98
	public class Runtime
	{
		// Token: 0x06000299 RID: 665 RVA: 0x0000B064 File Offset: 0x00009264
		internal void AddShutdownHook(IRunnable r)
		{
			Runtime.ShutdownHook shutdownHook = new Runtime.ShutdownHook();
			shutdownHook.Runnable = r;
			this._shutdownHooks.Add(shutdownHook);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000B08C File Offset: 0x0000928C
		internal int AvailableProcessors()
		{
			return Environment.ProcessorCount;
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000B0A4 File Offset: 0x000092A4
		public static long CurrentTimeMillis()
		{
			return DateTime.UtcNow.ToMillisecondsSinceEpoch();
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000B0C0 File Offset: 0x000092C0
		public static Hashtable GetProperties()
		{
			bool flag = Runtime._properties == null;
			if (flag)
			{
				Runtime._properties = new Hashtable();
				Runtime._properties["jgit.fs.debug"] = "false";
				Runtime._properties["file.encoding"] = "UTF-8";
				bool flag2 = Path.DirectorySeparatorChar != '\\';
				if (flag2)
				{
					Runtime._properties["os.name"] = "Unix";
				}
				else
				{
					Runtime._properties["os.name"] = "Windows";
				}
			}
			return Runtime._properties;
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000B158 File Offset: 0x00009358
		public static string GetProperty(string key)
		{
			bool flag = Runtime.GetProperties().Keys.Contains(key);
			string result;
			if (flag)
			{
				result = (string)Runtime.GetProperties()[key];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000B193 File Offset: 0x00009393
		public static void SetProperty(string key, string value)
		{
			Runtime.GetProperties()[key] = value;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000B1A4 File Offset: 0x000093A4
		public static Runtime GetRuntime()
		{
			bool flag = Runtime._instance == null;
			if (flag)
			{
				Runtime._instance = new Runtime();
			}
			return Runtime._instance;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000B1D4 File Offset: 0x000093D4
		public static int IdentityHashCode(object ob)
		{
			return RuntimeHelpers.GetHashCode(ob);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0000B1EC File Offset: 0x000093EC
		internal long MaxMemory()
		{
			return 2147483647L;
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000B204 File Offset: 0x00009404
		public static void DeleteCharAt(StringBuilder sb, int index)
		{
			sb.Remove(index, 1);
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000B210 File Offset: 0x00009410
		public static byte[] GetBytesForString(string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000B230 File Offset: 0x00009430
		public static byte[] GetBytesForString(string str, string encoding)
		{
			return Encoding.GetEncoding(encoding).GetBytes(str);
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000B250 File Offset: 0x00009450
		public static FieldInfo[] GetDeclaredFields(Type t)
		{
			return t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000B26A File Offset: 0x0000946A
		public static void NotifyAll(object ob)
		{
			Monitor.PulseAll(ob);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0000B274 File Offset: 0x00009474
		public static void Notify(object obj)
		{
			Monitor.Pulse(obj);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000B27E File Offset: 0x0000947E
		public static void PrintStackTrace(Exception ex)
		{
			Console.WriteLine(ex);
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0000B288 File Offset: 0x00009488
		public static void PrintStackTrace(Exception ex, TextWriter tw)
		{
			tw.WriteLine(ex);
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000B294 File Offset: 0x00009494
		public static string Substring(string str, int index)
		{
			return str.Substring(index);
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000B2B0 File Offset: 0x000094B0
		public static string Substring(string str, int index, int endIndex)
		{
			return str.Substring(index, endIndex - index);
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0000B2CC File Offset: 0x000094CC
		public static void Wait(object ob)
		{
			Monitor.Wait(ob);
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0000B2D8 File Offset: 0x000094D8
		public static bool Wait(object ob, long milis)
		{
			return Monitor.Wait(ob, (int)milis);
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0000B2F4 File Offset: 0x000094F4
		public static Type GetType(string name)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type type = assembly.GetType(name);
				bool flag = type != null;
				if (flag)
				{
					return type;
				}
			}
			throw new InvalidOperationException("Type not found: " + name);
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00007330 File Offset: 0x00005530
		public static void SetCharAt(StringBuilder sb, int index, char c)
		{
			sb[index] = c;
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000B350 File Offset: 0x00009550
		public static bool EqualsIgnoreCase(string s1, string s2)
		{
			return s1.Equals(s2, StringComparison.CurrentCultureIgnoreCase);
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0000B36C File Offset: 0x0000956C
		internal static long NanoTime()
		{
			return (long)(Environment.TickCount * 1000 * 1000);
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0000B390 File Offset: 0x00009590
		internal static int CompareOrdinal(string s1, string s2)
		{
			return string.CompareOrdinal(s1, s2);
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0000B3AC File Offset: 0x000095AC
		public static string GetStringForBytes(byte[] chars)
		{
			return Encoding.UTF8.GetString(chars, 0, chars.Length);
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0000B3D0 File Offset: 0x000095D0
		public static string GetStringForBytes(byte[] chars, string encoding)
		{
			return Runtime.GetEncoding(encoding).GetString(chars, 0, chars.Length);
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0000B3F4 File Offset: 0x000095F4
		public static string GetStringForBytes(byte[] chars, int start, int len)
		{
			return Encoding.UTF8.GetString(chars, start, len);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000B414 File Offset: 0x00009614
		public static string GetStringForBytes(byte[] chars, int start, int len, string encoding)
		{
			return Runtime.GetEncoding(encoding).Decode(chars, start, len);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000B434 File Offset: 0x00009634
		public static Encoding GetEncoding(string name)
		{
			Encoding encoding = Encoding.GetEncoding(name.Replace('_', '-'));
			bool flag = encoding is UTF8Encoding;
			Encoding result;
			if (flag)
			{
				result = new UTF8Encoding(false, true);
			}
			else
			{
				result = encoding;
			}
			return result;
		}

		// Token: 0x0400007D RID: 125
		private static Runtime _instance;

		// Token: 0x0400007E RID: 126
		private List<Runtime.ShutdownHook> _shutdownHooks = new List<Runtime.ShutdownHook>();

		// Token: 0x0400007F RID: 127
		private static Hashtable _properties;

		// Token: 0x02000111 RID: 273
		private class ShutdownHook
		{
			// Token: 0x0600080A RID: 2058 RVA: 0x0002B06C File Offset: 0x0002926C
			~ShutdownHook()
			{
				this.Runnable.Run();
			}

			// Token: 0x04000553 RID: 1363
			public IRunnable Runnable;
		}
	}
}
