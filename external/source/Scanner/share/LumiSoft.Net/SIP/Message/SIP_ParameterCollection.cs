using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000045 RID: 69
	public class SIP_ParameterCollection : IEnumerable
	{
		// Token: 0x06000246 RID: 582 RVA: 0x0000DFF2 File Offset: 0x0000CFF2
		public SIP_ParameterCollection()
		{
			this.m_pCollection = new List<SIP_Parameter>();
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000E010 File Offset: 0x0000D010
		public void Add(string name, string value)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = this.Contains(name);
			if (flag2)
			{
				throw new ArgumentException("Prameter '' with specified name already exists in the collection !");
			}
			this.m_pCollection.Add(new SIP_Parameter(name, value));
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000E05C File Offset: 0x0000D05C
		public void Set(string name, string value)
		{
			bool flag = this.Contains(name);
			if (flag)
			{
				this[name].Value = value;
			}
			else
			{
				this.Add(name, value);
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000E092 File Offset: 0x0000D092
		public void Clear()
		{
			this.m_pCollection.Clear();
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000E0A4 File Offset: 0x0000D0A4
		public void Remove(string name)
		{
			SIP_Parameter sip_Parameter = this[name];
			bool flag = sip_Parameter != null;
			if (flag)
			{
				this.m_pCollection.Remove(sip_Parameter);
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000E0D4 File Offset: 0x0000D0D4
		public bool Contains(string name)
		{
			SIP_Parameter sip_Parameter = this[name];
			return sip_Parameter != null;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000E100 File Offset: 0x0000D100
		public IEnumerator GetEnumerator()
		{
			return this.m_pCollection.GetEnumerator();
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600024D RID: 589 RVA: 0x0000E124 File Offset: 0x0000D124
		public int Count
		{
			get
			{
				return this.m_pCollection.Count;
			}
		}

		// Token: 0x170000B3 RID: 179
		public SIP_Parameter this[string name]
		{
			get
			{
				foreach (SIP_Parameter sip_Parameter in this.m_pCollection)
				{
					bool flag = sip_Parameter.Name.ToLower() == name.ToLower();
					if (flag)
					{
						return sip_Parameter;
					}
				}
				return null;
			}
		}

		// Token: 0x04000104 RID: 260
		private List<SIP_Parameter> m_pCollection = null;
	}
}
