using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;

namespace Tools
{
	public class Interval
	{
		public Interval()
		{
			this.st = 0L;
			this.ed = 0L;
		}

		public Interval(long s, long e)
		{
			this.st = s;
			this.ed = e;
		}

		public long st;
		public long ed;
	}

	class HostTool
    {
		public static uint IpToLong(string ip)
		{
			IPAddress ipaddress = null;
			IPAddress.TryParse(ip, out ipaddress);
			uint result;
			if (ipaddress == null)
			{
				result = 0U;
			}
			else
			{
				byte[] addressBytes = ipaddress.GetAddressBytes();
				Array.Reverse(addressBytes);
				result = BitConverter.ToUInt32(addressBytes, 0);
			}
			return result;
		}

		public static string LongToIP(long longIP)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(longIP >> 24);
			stringBuilder.Append(".");
			stringBuilder.Append((longIP & 16777215L) >> 16);
			stringBuilder.Append(".");
			stringBuilder.Append((longIP & 65535L) >> 8);
			stringBuilder.Append(".");
			stringBuilder.Append(longIP & 255L);
			return stringBuilder.ToString();
		}

		public static List<Interval> Merge(List<Interval> intervals)
		{
			List<Interval> list = new List<Interval>();
			List<Interval> result;
			if (intervals.Count == 0)
			{
				result = list;
			}
			else
			{
				intervals.OrderBy(i => i.st);
				list.Add(intervals[0]);
				long num = (long)(intervals.Count - 1);
				for (long num2 = 1L; num2 <= num; num2 += 1L)
				{
					if (intervals[(int)num2].st <= list[list.Count - 1].ed)
					{
						list[list.Count - 1].ed = Math.Max(intervals[(int)num2].ed, list[list.Count - 1].ed);
					}
					else
					{
						list.Add(intervals[(int)num2]);
					}
				}
				result = list;
			}
			return result;
		}

		public static List<Interval> GetHosts(string strHost)
		{
			Random r1 = new Random();
			int v = r1.Next(1000000);
			string tmpPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetTempPath()) + "\\t" + v.ToString() + ".tmp";

			string TaskTempFile = "";
			if (File.Exists(strHost))
			{
				TaskTempFile = strHost;
			}
			else
			{
				TaskTempFile = tmpPath;
				File.WriteAllText(TaskTempFile, strHost.Replace(",", "\r\n"));
			}
			List<Interval> list = new List<Interval>();
			int[] array = new int[4];
			IPAddress ipaddress = null;
			try
			{
				foreach (string text in File.ReadLines(TaskTempFile))
				{
					if (text.Length != 0)
					{
						if (text.Contains("-"))
						{
							string[] array2 = text.Split(new char[]
							{
								'-'
							});
							if (array2.Length == 2)
							{
								long s = (long)((ulong)HostTool.IpToLong(array2[0]));
								long e = (long)((ulong)HostTool.IpToLong(array2[1]));
								list.Add(new Interval(s, e));
							}
						}
						else if (text.Contains("/") && !text.Contains("://"))
						{
							string[] array2 = text.Split(new char[]
							{
								'/'
							});
							if (array2.Length == 2)
							{
								string text2 = array2[0];
								string text3 = array2[1];
								array2 = text2.Split(new char[]
								{
									'.'
								});
								int num = 0;
								do
								{
									array[num] = int.Parse(array2[num]);
									array2[num] = Convert.ToString(array[num], 2).PadLeft(8, '0');
									num++;
								}
								while (num <= 3);
								string text4 = string.Join("", array2);
								text4 = text4.Substring(0, Convert.ToInt32(text3)).PadRight(32, '1');
								int num2 = 0;
								int num3 = 0;
								do
								{
									array2[num2] = text4.Substring(num3, 8);
									array[num2] = Convert.ToInt32(array2[num2], 2);
									num2++;
									num3 += 8;
								}
								while (num3 <= 31);
								text3 = string.Join<int>(".", array);
								long s = (long)((ulong)HostTool.IpToLong(text2));
								long e = (long)((ulong)HostTool.IpToLong(text3));
								list.Add(new Interval(s, e));
							}
						}
						else
						{
							string text_tmp = text;
							try
							{
								if (text.Contains("://"))
								{
									text_tmp = new Uri(text).Host;
								}
							}
							catch
							{
							}
							if (IPAddress.TryParse(text_tmp, out ipaddress))
							{
								list.Add(new Interval((long)((ulong)HostTool.IpToLong(text_tmp)), (long)((ulong)HostTool.IpToLong(text_tmp))));
							}
							else
							{
								try
								{
									text_tmp = Dns.GetHostEntry(text_tmp).AddressList[0].ToString();
									list.Add(new Interval((long)((ulong)HostTool.IpToLong(text_tmp)), (long)((ulong)HostTool.IpToLong(text_tmp))));
								}
								catch
								{
								}
							}
						}
					}
				}
				if (File.Exists(tmpPath))
					File.Delete(tmpPath);
			}
			catch
			{
			}

			return list;
		}
	}
}
