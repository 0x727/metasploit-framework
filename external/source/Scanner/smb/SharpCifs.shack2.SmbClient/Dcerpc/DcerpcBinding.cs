using System;
using SharpCifs.Dcerpc.Msrpc;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Dcerpc
{
	// Token: 0x020000DE RID: 222
	public class DcerpcBinding
	{
		// Token: 0x06000768 RID: 1896 RVA: 0x00028AC8 File Offset: 0x00026CC8
		static DcerpcBinding()
		{
			DcerpcBinding._interfaces.Put("srvsvc", Srvsvc.GetSyntax());
			DcerpcBinding._interfaces.Put("lsarpc", Lsarpc.GetSyntax());
			DcerpcBinding._interfaces.Put("samr", Samr.GetSyntax());
			DcerpcBinding._interfaces.Put("netdfs", Netdfs.GetSyntax());
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x00028B34 File Offset: 0x00026D34
		public static void AddInterface(string name, string syntax)
		{
			DcerpcBinding._interfaces.Put(name, syntax);
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x00028B44 File Offset: 0x00026D44
		internal DcerpcBinding(string proto, string server)
		{
			this.Proto = proto;
			this.Server = server;
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x00028B5C File Offset: 0x00026D5C
		internal virtual void SetOption(string key, object val)
		{
			bool flag = key.Equals("endpoint");
			if (flag)
			{
				this.Endpoint = val.ToString().ToLower();
				bool flag2 = this.Endpoint.StartsWith("\\pipe\\");
				if (flag2)
				{
					string text = (string)DcerpcBinding._interfaces.Get(Runtime.Substring(this.Endpoint, 6));
					bool flag3 = text != null;
					if (flag3)
					{
						int num = text.IndexOf(':');
						int num2 = text.IndexOf('.', num + 1);
						this.Uuid = new Uuid(Runtime.Substring(text, 0, num));
						this.Major = Convert.ToInt32(Runtime.Substring(text, num + 1, num2));
						this.Minor = Convert.ToInt32(Runtime.Substring(text, num2 + 1));
						return;
					}
				}
				throw new DcerpcException("Bad endpoint: " + this.Endpoint);
			}
			bool flag4 = this.Options == null;
			if (flag4)
			{
				this.Options = new Hashtable();
			}
			this.Options.Put(key, val);
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x00028C68 File Offset: 0x00026E68
		internal virtual object GetOption(string key)
		{
			bool flag = key.Equals("endpoint");
			object result;
			if (flag)
			{
				result = this.Endpoint;
			}
			else
			{
				bool flag2 = this.Options != null;
				if (flag2)
				{
					result = this.Options.Get(key);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x00028CB4 File Offset: 0x00026EB4
		public override string ToString()
		{
			return null;
		}

		// Token: 0x040004BF RID: 1215
		private static Hashtable _interfaces = new Hashtable();

		// Token: 0x040004C0 RID: 1216
		internal string Proto;

		// Token: 0x040004C1 RID: 1217
		internal string Server;

		// Token: 0x040004C2 RID: 1218
		internal string Endpoint;

		// Token: 0x040004C3 RID: 1219
		internal Hashtable Options;

		// Token: 0x040004C4 RID: 1220
		internal Uuid Uuid;

		// Token: 0x040004C5 RID: 1221
		internal int Major;

		// Token: 0x040004C6 RID: 1222
		internal int Minor;
	}
}
