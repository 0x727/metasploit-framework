using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000070 RID: 112
	public class BufferCache
	{
		// Token: 0x06000336 RID: 822 RVA: 0x0000D1AC File Offset: 0x0000B3AC
		public static byte[] GetBuffer()
		{
			object[] cache = BufferCache.Cache;
			byte[] result;
			lock (cache)
			{
				bool flag2 = BufferCache._freeBuffers > 0;
				byte[] array;
				if (flag2)
				{
					for (int i = 0; i < BufferCache.MaxBuffers; i++)
					{
						bool flag3 = BufferCache.Cache[i] != null;
						if (flag3)
						{
							array = (byte[])BufferCache.Cache[i];
							BufferCache.Cache[i] = null;
							BufferCache._freeBuffers--;
							return array;
						}
					}
				}
				array = new byte[65535];
				result = array;
			}
			return result;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0000D260 File Offset: 0x0000B460
		internal static void GetBuffers(SmbComTransaction req, SmbComTransactionResponse rsp)
		{
			object[] cache = BufferCache.Cache;
			lock (cache)
			{
				req.TxnBuf = BufferCache.GetBuffer();
				rsp.TxnBuf = BufferCache.GetBuffer();
			}
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0000D2B4 File Offset: 0x0000B4B4
		public static void ReleaseBuffer(byte[] buf)
		{
			object[] cache = BufferCache.Cache;
			lock (cache)
			{
				bool flag2 = BufferCache._freeBuffers < BufferCache.MaxBuffers;
				if (flag2)
				{
					for (int i = 0; i < BufferCache.MaxBuffers; i++)
					{
						bool flag3 = BufferCache.Cache[i] == null;
						if (flag3)
						{
							BufferCache.Cache[i] = buf;
							BufferCache._freeBuffers++;
							break;
						}
					}
				}
			}
		}

		// Token: 0x040000BC RID: 188
		private static readonly int MaxBuffers = Config.GetInt("jcifs.smb.maxBuffers", 16);

		// Token: 0x040000BD RID: 189
		internal static object[] Cache = new object[BufferCache.MaxBuffers];

		// Token: 0x040000BE RID: 190
		private static int _freeBuffers;
	}
}
