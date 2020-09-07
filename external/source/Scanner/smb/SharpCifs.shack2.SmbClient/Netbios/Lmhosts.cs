using System;
using System.IO;
using SharpCifs.Smb;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D0 RID: 208
	public class Lmhosts
	{
		// Token: 0x060006D6 RID: 1750 RVA: 0x000248E8 File Offset: 0x00022AE8
		public static NbtAddress GetByName(string host)
		{
			Type typeFromHandle = typeof(Lmhosts);
			NbtAddress byName;
			lock (typeFromHandle)
			{
				byName = Lmhosts.GetByName(new Name(host, 32, null));
			}
			return byName;
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0002493C File Offset: 0x00022B3C
		internal static NbtAddress GetByName(Name name)
		{
			Type typeFromHandle = typeof(Lmhosts);
			NbtAddress result;
			lock (typeFromHandle)
			{
				NbtAddress nbtAddress = null;
				try
				{
					bool flag2 = Lmhosts.Filename != null;
					if (flag2)
					{
						FilePath filePath = new FilePath(Lmhosts.Filename);
						long lastModified;
						bool flag3 = (lastModified = filePath.LastModified()) > Lmhosts._lastModified;
						if (flag3)
						{
							Lmhosts._lastModified = lastModified;
							Lmhosts.Tab.Clear();
							Lmhosts._alt = 0;
							Lmhosts.Populate(new FileReader(filePath));
						}
						nbtAddress = (NbtAddress)Lmhosts.Tab[name];
					}
				}
				catch (FileNotFoundException ex)
				{
					bool flag4 = Lmhosts._log.Level > 1;
					if (flag4)
					{
						Lmhosts._log.WriteLine("lmhosts file: " + Lmhosts.Filename);
						Runtime.PrintStackTrace(ex, Lmhosts._log);
					}
				}
				catch (IOException ex2)
				{
					bool flag5 = Lmhosts._log.Level > 0;
					if (flag5)
					{
						Runtime.PrintStackTrace(ex2, Lmhosts._log);
					}
				}
				result = nbtAddress;
			}
			return result;
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x00024A74 File Offset: 0x00022C74
		internal static void Populate(StreamReader r)
		{
			BufferedReader bufferedReader = new BufferedReader((InputStreamReader)r);
			string text;
			while ((text = bufferedReader.ReadLine()) != null)
			{
				text = text.ToUpper().Trim();
				bool flag = text.Length == 0;
				if (!flag)
				{
					bool flag2 = text[0] == '#';
					if (flag2)
					{
						bool flag3 = text.StartsWith("#INCLUDE ");
						if (flag3)
						{
							text = Runtime.Substring(text, text.IndexOf('\\'));
							string text2 = "smb:" + text.Replace('\\', '/');
							bool flag4 = Lmhosts._alt > 0;
							if (flag4)
							{
								try
								{
									Lmhosts.Populate(new InputStreamReader(new SmbFileInputStream(text2)));
								}
								catch (IOException ex)
								{
									Lmhosts._log.WriteLine("lmhosts URL: " + text2);
									Runtime.PrintStackTrace(ex, Lmhosts._log);
									continue;
								}
								Lmhosts._alt--;
								while ((text = bufferedReader.ReadLine()) != null)
								{
									text = text.ToUpper().Trim();
									bool flag5 = text.StartsWith("#END_ALTERNATE");
									if (flag5)
									{
										break;
									}
								}
							}
							else
							{
								Lmhosts.Populate(new InputStreamReader(new SmbFileInputStream(text2)));
							}
						}
						else
						{
							bool flag6 = text.StartsWith("#BEGIN_ALTERNATE");
							if (flag6)
							{
								Lmhosts._alt++;
							}
							else
							{
								bool flag7 = text.StartsWith("#END_ALTERNATE") && Lmhosts._alt > 0;
								if (flag7)
								{
									Lmhosts._alt--;
									throw new IOException("no lmhosts alternate includes loaded");
								}
							}
						}
					}
					else
					{
						bool flag8 = char.IsDigit(text[0]);
						if (flag8)
						{
							char[] array = text.ToCharArray();
							char c = '.';
							int num2;
							int num = num2 = 0;
							while (num < array.Length && c == '.')
							{
								int num3 = 0;
								while (num < array.Length && (c = array[num]) >= '0' && c <= '9')
								{
									num3 = num3 * 10 + (int)c - 48;
									num++;
								}
								num2 = (num2 << 8) + num3;
								num++;
							}
							while (num < array.Length && char.IsWhiteSpace(array[num]))
							{
								num++;
							}
							int num4 = num;
							while (num4 < array.Length && !char.IsWhiteSpace(array[num4]))
							{
								num4++;
							}
							Name name = new Name(Runtime.Substring(text, num, num4), 32, null);
							NbtAddress value = new NbtAddress(name, num2, false, 0, false, false, true, true, NbtAddress.UnknownMacAddress);
							Lmhosts.Tab.Put(name, value);
						}
					}
				}
			}
		}

		// Token: 0x0400041D RID: 1053
		private static readonly string Filename = Config.GetProperty("jcifs.netbios.lmhosts");

		// Token: 0x0400041E RID: 1054
		private static readonly Hashtable Tab = new Hashtable();

		// Token: 0x0400041F RID: 1055
		private static long _lastModified = 1L;

		// Token: 0x04000420 RID: 1056
		private static int _alt;

		// Token: 0x04000421 RID: 1057
		private static LogStream _log = LogStream.GetInstance();
	}
}
