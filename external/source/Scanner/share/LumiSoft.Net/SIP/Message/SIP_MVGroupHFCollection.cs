using System;
using System.Collections.Generic;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200005D RID: 93
	public class SIP_MVGroupHFCollection<T> where T : SIP_t_Value, new()
	{
		// Token: 0x06000309 RID: 777 RVA: 0x00010B50 File Offset: 0x0000FB50
		public SIP_MVGroupHFCollection(SIP_Message owner, string fieldName)
		{
			this.m_pMessage = owner;
			this.m_FieldName = fieldName;
			this.m_pFields = new List<SIP_MultiValueHF<T>>();
			this.Refresh();
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00010BA0 File Offset: 0x0000FBA0
		private void Refresh()
		{
			this.m_pFields.Clear();
			foreach (object obj in this.m_pMessage.Header)
			{
				SIP_HeaderField sip_HeaderField = (SIP_HeaderField)obj;
				bool flag = sip_HeaderField.Name.ToLower() == this.m_FieldName.ToLower();
				if (flag)
				{
					this.m_pFields.Add((SIP_MultiValueHF<T>)sip_HeaderField);
				}
			}
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00010C3C File Offset: 0x0000FC3C
		public void AddToTop(string value)
		{
			this.m_pMessage.Header.Insert(0, this.m_FieldName, value);
			this.Refresh();
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00010C5F File Offset: 0x0000FC5F
		public void Add(string value)
		{
			this.m_pMessage.Header.Add(this.m_FieldName, value);
			this.Refresh();
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00010C81 File Offset: 0x0000FC81
		public void RemoveAll()
		{
			this.m_pMessage.Header.RemoveAll(this.m_FieldName);
			this.m_pFields.Clear();
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00010CA8 File Offset: 0x0000FCA8
		public T GetTopMostValue()
		{
			bool flag = this.m_pFields.Count > 0;
			T result;
			if (flag)
			{
				result = this.m_pFields[0].Values[0];
			}
			else
			{
				result = default(T);
			}
			return result;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00010CF0 File Offset: 0x0000FCF0
		public void RemoveTopMostValue()
		{
			bool flag = this.m_pFields.Count > 0;
			if (flag)
			{
				SIP_MultiValueHF<T> sip_MultiValueHF = this.m_pFields[0];
				bool flag2 = sip_MultiValueHF.Count > 1;
				if (flag2)
				{
					sip_MultiValueHF.Remove(0);
				}
				else
				{
					this.m_pMessage.Header.Remove(this.m_pFields[0]);
					this.m_pFields.Remove(this.m_pFields[0]);
				}
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00010D70 File Offset: 0x0000FD70
		public void RemoveLastValue()
		{
			SIP_MultiValueHF<T> sip_MultiValueHF = this.m_pFields[this.m_pFields.Count - 1];
			bool flag = sip_MultiValueHF.Count > 1;
			if (flag)
			{
				sip_MultiValueHF.Remove(sip_MultiValueHF.Count - 1);
			}
			else
			{
				this.m_pMessage.Header.Remove(this.m_pFields[0]);
				this.m_pFields.Remove(sip_MultiValueHF);
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00010DE4 File Offset: 0x0000FDE4
		public T[] GetAllValues()
		{
			List<T> list = new List<T>();
			foreach (SIP_MultiValueHF<T> sip_MultiValueHF in this.m_pFields)
			{
				foreach (T t in sip_MultiValueHF.Values)
				{
					SIP_t_Value sip_t_Value = t;
					list.Add((T)((object)sip_t_Value));
				}
			}
			return list.ToArray();
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000312 RID: 786 RVA: 0x00010E9C File Offset: 0x0000FE9C
		public string FieldName
		{
			get
			{
				return this.m_FieldName;
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000313 RID: 787 RVA: 0x00010EB4 File Offset: 0x0000FEB4
		public int Count
		{
			get
			{
				return this.m_pFields.Count;
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000314 RID: 788 RVA: 0x00010ED4 File Offset: 0x0000FED4
		public SIP_MultiValueHF<T>[] HeaderFields
		{
			get
			{
				return this.m_pFields.ToArray();
			}
		}

		// Token: 0x04000127 RID: 295
		private SIP_Message m_pMessage = null;

		// Token: 0x04000128 RID: 296
		private string m_FieldName = "";

		// Token: 0x04000129 RID: 297
		private List<SIP_MultiValueHF<T>> m_pFields = null;
	}
}
