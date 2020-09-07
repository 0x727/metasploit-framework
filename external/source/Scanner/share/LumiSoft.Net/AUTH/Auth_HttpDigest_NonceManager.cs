using System;
using System.Collections.Generic;
using System.Timers;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x0200026E RID: 622
	public class Auth_HttpDigest_NonceManager : IDisposable
	{
		// Token: 0x06001669 RID: 5737 RVA: 0x0008C0D4 File Offset: 0x0008B0D4
		public Auth_HttpDigest_NonceManager()
		{
			this.m_pNonces = new List<Auth_HttpDigest_NonceManager.NonceEntry>();
			this.m_pTimer = new Timer(15000.0);
			this.m_pTimer.Elapsed += this.m_pTimer_Elapsed;
			this.m_pTimer.Enabled = true;
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x0008C144 File Offset: 0x0008B144
		public void Dispose()
		{
			bool flag = this.m_pNonces == null;
			if (flag)
			{
				this.m_pNonces.Clear();
				this.m_pNonces = null;
			}
			bool flag2 = this.m_pTimer != null;
			if (flag2)
			{
				this.m_pTimer.Dispose();
				this.m_pTimer = null;
			}
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x0008C196 File Offset: 0x0008B196
		private void m_pTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			this.RemoveExpiredNonces();
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x0008C1A0 File Offset: 0x0008B1A0
		public string CreateNonce()
		{
			string text = Guid.NewGuid().ToString().Replace("-", "");
			this.m_pNonces.Add(new Auth_HttpDigest_NonceManager.NonceEntry(text));
			return text;
		}

		// Token: 0x0600166D RID: 5741 RVA: 0x0008C1E8 File Offset: 0x0008B1E8
		public bool NonceExists(string nonce)
		{
			List<Auth_HttpDigest_NonceManager.NonceEntry> pNonces = this.m_pNonces;
			lock (pNonces)
			{
				foreach (Auth_HttpDigest_NonceManager.NonceEntry nonceEntry in this.m_pNonces)
				{
					bool flag2 = nonceEntry.Nonce == nonce;
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x0008C288 File Offset: 0x0008B288
		public void RemoveNonce(string nonce)
		{
			List<Auth_HttpDigest_NonceManager.NonceEntry> pNonces = this.m_pNonces;
			lock (pNonces)
			{
				for (int i = 0; i < this.m_pNonces.Count; i++)
				{
					bool flag2 = this.m_pNonces[i].Nonce == nonce;
					if (flag2)
					{
						this.m_pNonces.RemoveAt(i);
						i--;
					}
				}
			}
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x0008C314 File Offset: 0x0008B314
		private void RemoveExpiredNonces()
		{
			List<Auth_HttpDigest_NonceManager.NonceEntry> pNonces = this.m_pNonces;
			lock (pNonces)
			{
				for (int i = 0; i < this.m_pNonces.Count; i++)
				{
					bool flag2 = this.m_pNonces[i].CreateTime.AddSeconds((double)this.m_ExpireTime) < DateTime.Now;
					if (flag2)
					{
						this.m_pNonces.RemoveAt(i);
						i--;
					}
				}
			}
		}

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x06001670 RID: 5744 RVA: 0x0008C3B4 File Offset: 0x0008B3B4
		// (set) Token: 0x06001671 RID: 5745 RVA: 0x0008C3CC File Offset: 0x0008B3CC
		public int ExpireTime
		{
			get
			{
				return this.m_ExpireTime;
			}
			set
			{
				bool flag = value < 5;
				if (flag)
				{
					throw new ArgumentException("Property ExpireTime value must be >= 5 !");
				}
				this.m_ExpireTime = value;
			}
		}

		// Token: 0x040008FC RID: 2300
		private List<Auth_HttpDigest_NonceManager.NonceEntry> m_pNonces = null;

		// Token: 0x040008FD RID: 2301
		private int m_ExpireTime = 30;

		// Token: 0x040008FE RID: 2302
		private Timer m_pTimer = null;

		// Token: 0x0200038D RID: 909
		private class NonceEntry
		{
			// Token: 0x06001BE9 RID: 7145 RVA: 0x000ACAE7 File Offset: 0x000ABAE7
			public NonceEntry(string nonce)
			{
				this.m_Nonce = nonce;
				this.m_CreateTime = DateTime.Now;
			}

			// Token: 0x170008A1 RID: 2209
			// (get) Token: 0x06001BEA RID: 7146 RVA: 0x000ACB10 File Offset: 0x000ABB10
			public string Nonce
			{
				get
				{
					return this.m_Nonce;
				}
			}

			// Token: 0x170008A2 RID: 2210
			// (get) Token: 0x06001BEB RID: 7147 RVA: 0x000ACB28 File Offset: 0x000ABB28
			public DateTime CreateTime
			{
				get
				{
					return this.m_CreateTime;
				}
			}

			// Token: 0x04000D04 RID: 3332
			private string m_Nonce = "";

			// Token: 0x04000D05 RID: 3333
			private DateTime m_CreateTime;
		}
	}
}
