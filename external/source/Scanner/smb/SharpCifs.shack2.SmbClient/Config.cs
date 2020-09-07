using System;
using System.IO;
using System.Net;
using System.Security;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs
{
	// Token: 0x02000003 RID: 3
	public class Config
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002210 File Offset: 0x00000410
		static Config()
		{
			FileInputStream fileInputStream = null;
			Config._log = LogStream.GetInstance();
			try
			{
				string property = Runtime.GetProperty("jcifs.properties");
				bool flag = property != null && property.Length > 1;
				if (flag)
				{
					fileInputStream = new FileInputStream(property);
				}
				Config.Load(fileInputStream);
				bool flag2 = fileInputStream != null;
				if (flag2)
				{
					fileInputStream.Close();
				}
			}
			catch (IOException ex)
			{
				bool flag3 = Config._log.Level > 0;
				if (flag3)
				{
					Runtime.PrintStackTrace(ex, Config._log);
				}
			}
			int @int;
			bool flag4 = (@int = Config.GetInt("jcifs.util.loglevel", -1)) != -1;
			if (flag4)
			{
				Config._log.SetLevel(@int);
			}
			try
			{
				Runtime.GetBytesForString(string.Empty, Config.DefaultOemEncoding);
			}
			catch (Exception ex2)
			{
				bool flag5 = Config._log.Level >= 2;
				if (flag5)
				{
					Config._log.WriteLine("WARNING: The default OEM encoding " + Config.DefaultOemEncoding + " does not appear to be supported by this JRE. The default encoding will be US-ASCII.");
				}
			}
			bool flag6 = Config._log.Level >= 4;
			if (flag6)
			{
				try
				{
					Config._prp.Store(Config._log);
				}
				catch (IOException)
				{
				}
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002380 File Offset: 0x00000580
		public static void RegisterSmbURLHandler()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002388 File Offset: 0x00000588
		public static void SetProperties(Properties prp)
		{
			Config._prp = new Properties(prp);
			try
			{
				Config._prp.PutAll(Runtime.GetProperties());
			}
			catch (SecurityException)
			{
				bool flag = Config._log.Level > 1;
				if (flag)
				{
					Config._log.WriteLine("SecurityException: jcifs will ignore System properties");
				}
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000023F0 File Offset: 0x000005F0
		public static void Load(InputStream input)
		{
			bool flag = input != null;
			if (flag)
			{
				Config._prp.Load(input);
			}
			try
			{
				Config._prp.PutAll(Runtime.GetProperties());
			}
			catch (SecurityException)
			{
				bool flag2 = Config._log.Level > 1;
				if (flag2)
				{
					Config._log.WriteLine("SecurityException: jcifs will ignore System properties");
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002464 File Offset: 0x00000664
		public static void Store(OutputStream output, string header)
		{
			Config._prp.Store(output);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002473 File Offset: 0x00000673
		public static void SetProperty(string key, string value)
		{
			Config._prp.SetProperty(key, value);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002484 File Offset: 0x00000684
		public static object Get(string key)
		{
			return Config._prp.GetProperty(key);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000024A4 File Offset: 0x000006A4
		public static string GetProperty(string key, string def)
		{
			return (string)Config._prp.GetProperty(key, def);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000024C8 File Offset: 0x000006C8
		public static string GetProperty(string key)
		{
			return (string)Config._prp.GetProperty(key);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000024EC File Offset: 0x000006EC
		public static int GetInt(string key, int def)
		{
			string text = (string)Config._prp.GetProperty(key);
			bool flag = text != null;
			if (flag)
			{
				try
				{
					def = Convert.ToInt32(text);
				}
				catch (FormatException ex)
				{
					bool flag2 = Config._log.Level > 0;
					if (flag2)
					{
						Runtime.PrintStackTrace(ex, Config._log);
					}
				}
			}
			return def;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000255C File Offset: 0x0000075C
		public static int GetInt(string key)
		{
			string text = (string)Config._prp.GetProperty(key);
			int result = -1;
			bool flag = text != null;
			if (flag)
			{
				try
				{
					result = Convert.ToInt32(text);
				}
				catch (FormatException ex)
				{
					bool flag2 = Config._log.Level > 0;
					if (flag2)
					{
						Runtime.PrintStackTrace(ex, Config._log);
					}
				}
			}
			return result;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000025D0 File Offset: 0x000007D0
		public static long GetLong(string key, long def)
		{
			string text = (string)Config._prp.GetProperty(key);
			bool flag = text != null;
			if (flag)
			{
				try
				{
					def = long.Parse(text);
				}
				catch (FormatException ex)
				{
					bool flag2 = Config._log.Level > 0;
					if (flag2)
					{
						Runtime.PrintStackTrace(ex, Config._log);
					}
				}
			}
			return def;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002640 File Offset: 0x00000840
		public static IPAddress GetInetAddress(string key, IPAddress def)
		{
			string text = (string)Config._prp.GetProperty(key);
			bool flag = text != null;
			if (flag)
			{
				try
				{
					def = Extensions.GetAddressByName(text);
				}
				catch (UnknownHostException ex)
				{
					bool flag2 = Config._log.Level > 0;
					if (flag2)
					{
						Config._log.WriteLine(text);
						Runtime.PrintStackTrace(ex, Config._log);
					}
				}
			}
			return def;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000026BC File Offset: 0x000008BC
		public static IPAddress GetLocalHost()
		{
			string text = (string)Config._prp.GetProperty("jcifs.smb.client.laddr");
			bool flag = text != null;
			if (flag)
			{
				try
				{
					return Extensions.GetAddressByName(text);
				}
				catch (UnknownHostException ex)
				{
					bool flag2 = Config._log.Level > 0;
					if (flag2)
					{
						Config._log.WriteLine("Ignoring jcifs.smb.client.laddr address: " + text);
						Runtime.PrintStackTrace(ex, Config._log);
					}
				}
			}
			return null;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002744 File Offset: 0x00000944
		public static bool GetBoolean(string key, bool def)
		{
			string property = Config.GetProperty(key);
			bool flag = property != null;
			if (flag)
			{
				def = property.ToLower().Equals("true");
			}
			return def;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000277C File Offset: 0x0000097C
		public static IPAddress[] GetInetAddressArray(string key, string delim, IPAddress[] def)
		{
			string property = Config.GetProperty(key);
			bool flag = property != null;
			IPAddress[] result;
			if (flag)
			{
				StringTokenizer stringTokenizer = new StringTokenizer(property, delim);
				int num = stringTokenizer.CountTokens();
				IPAddress[] array = new IPAddress[num];
				for (int i = 0; i < num; i++)
				{
					string text = stringTokenizer.NextToken();
					try
					{
						array[i] = Extensions.GetAddressByName(text);
					}
					catch (UnknownHostException ex)
					{
						bool flag2 = Config._log.Level > 0;
						if (flag2)
						{
							Config._log.WriteLine(text);
							Runtime.PrintStackTrace(ex, Config._log);
						}
						return def;
					}
				}
				result = array;
			}
			else
			{
				result = def;
			}
			return result;
		}

		// Token: 0x04000005 RID: 5
		private static Properties _prp = new Properties();

		// Token: 0x04000006 RID: 6
		private static LogStream _log;

		// Token: 0x04000007 RID: 7
		public static string DefaultOemEncoding = "UTF-8";
	}
}
