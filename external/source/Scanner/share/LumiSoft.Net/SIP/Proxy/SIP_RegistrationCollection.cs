using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000B3 RID: 179
	public class SIP_RegistrationCollection : IEnumerable
	{
		// Token: 0x060006E0 RID: 1760 RVA: 0x00029428 File Offset: 0x00028428
		public SIP_RegistrationCollection()
		{
			this.m_pRegistrations = new List<SIP_Registration>();
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x00029444 File Offset: 0x00028444
		public void Add(SIP_Registration registration)
		{
			List<SIP_Registration> pRegistrations = this.m_pRegistrations;
			lock (pRegistrations)
			{
				bool flag2 = this.Contains(registration.AOR);
				if (flag2)
				{
					throw new ArgumentException("Registration with specified registration name already exists !");
				}
				this.m_pRegistrations.Add(registration);
			}
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x000294AC File Offset: 0x000284AC
		public void Remove(string addressOfRecord)
		{
			List<SIP_Registration> pRegistrations = this.m_pRegistrations;
			lock (pRegistrations)
			{
				foreach (SIP_Registration sip_Registration in this.m_pRegistrations)
				{
					bool flag2 = sip_Registration.AOR.ToLower() == addressOfRecord.ToLower();
					if (flag2)
					{
						this.m_pRegistrations.Remove(sip_Registration);
						break;
					}
				}
			}
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x00029558 File Offset: 0x00028558
		public bool Contains(string addressOfRecord)
		{
			List<SIP_Registration> pRegistrations = this.m_pRegistrations;
			lock (pRegistrations)
			{
				foreach (SIP_Registration sip_Registration in this.m_pRegistrations)
				{
					bool flag2 = sip_Registration.AOR.ToLower() == addressOfRecord.ToLower();
					if (flag2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x00029600 File Offset: 0x00028600
		public void RemoveExpired()
		{
			List<SIP_Registration> pRegistrations = this.m_pRegistrations;
			lock (pRegistrations)
			{
				for (int i = 0; i < this.m_pRegistrations.Count; i++)
				{
					SIP_Registration sip_Registration = this.m_pRegistrations[i];
					sip_Registration.RemoveExpiredBindings();
					bool flag2 = sip_Registration.Bindings.Length == 0;
					if (flag2)
					{
						this.m_pRegistrations.Remove(sip_Registration);
						i--;
					}
				}
			}
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00029694 File Offset: 0x00028694
		public IEnumerator GetEnumerator()
		{
			return this.m_pRegistrations.GetEnumerator();
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x000296B8 File Offset: 0x000286B8
		public int Count
		{
			get
			{
				return this.m_pRegistrations.Count;
			}
		}

		// Token: 0x1700023A RID: 570
		public SIP_Registration this[string addressOfRecord]
		{
			get
			{
				List<SIP_Registration> pRegistrations = this.m_pRegistrations;
				SIP_Registration result;
				lock (pRegistrations)
				{
					foreach (SIP_Registration sip_Registration in this.m_pRegistrations)
					{
						bool flag2 = sip_Registration.AOR.ToLower() == addressOfRecord.ToLower();
						if (flag2)
						{
							return sip_Registration;
						}
					}
					result = null;
				}
				return result;
			}
		}

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x0002977C File Offset: 0x0002877C
		public SIP_Registration[] Values
		{
			get
			{
				return this.m_pRegistrations.ToArray();
			}
		}

		// Token: 0x040002ED RID: 749
		private List<SIP_Registration> m_pRegistrations = null;
	}
}
