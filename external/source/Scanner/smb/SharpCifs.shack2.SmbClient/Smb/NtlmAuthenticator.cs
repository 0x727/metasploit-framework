using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200007B RID: 123
	public abstract class NtlmAuthenticator
	{
		// Token: 0x06000376 RID: 886 RVA: 0x0000EC3D File Offset: 0x0000CE3D
		private void Reset()
		{
			this._url = null;
			this._sae = null;
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0000EC50 File Offset: 0x0000CE50
		public static void SetDefault(NtlmAuthenticator a)
		{
			Type typeFromHandle = typeof(NtlmAuthenticator);
			lock (typeFromHandle)
			{
				bool flag2 = NtlmAuthenticator._auth != null;
				if (!flag2)
				{
					NtlmAuthenticator._auth = a;
				}
			}
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0000ECA8 File Offset: 0x0000CEA8
		protected internal string GetRequestingUrl()
		{
			return this._url;
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0000ECC0 File Offset: 0x0000CEC0
		protected internal SmbAuthException GetRequestingException()
		{
			return this._sae;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0000ECD8 File Offset: 0x0000CED8
		public static NtlmPasswordAuthentication RequestNtlmPasswordAuthentication(string url, SmbAuthException sae)
		{
			bool flag = NtlmAuthenticator._auth == null;
			NtlmPasswordAuthentication result;
			if (flag)
			{
				result = null;
			}
			else
			{
				NtlmAuthenticator auth = NtlmAuthenticator._auth;
				lock (auth)
				{
					NtlmAuthenticator._auth._url = url;
					NtlmAuthenticator._auth._sae = sae;
					result = NtlmAuthenticator._auth.GetNtlmPasswordAuthentication();
				}
			}
			return result;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x0000ED48 File Offset: 0x0000CF48
		protected internal virtual NtlmPasswordAuthentication GetNtlmPasswordAuthentication()
		{
			return null;
		}

		// Token: 0x040000E1 RID: 225
		private static NtlmAuthenticator _auth;

		// Token: 0x040000E2 RID: 226
		private string _url;

		// Token: 0x040000E3 RID: 227
		private SmbAuthException _sae;
	}
}
