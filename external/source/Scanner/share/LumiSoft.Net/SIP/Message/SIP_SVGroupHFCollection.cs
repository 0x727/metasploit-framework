using System;
using System.Collections.Generic;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200005E RID: 94
	public class SIP_SVGroupHFCollection<T> where T : SIP_t_Value
	{
		// Token: 0x06000315 RID: 789 RVA: 0x00010EF4 File Offset: 0x0000FEF4
		public SIP_SVGroupHFCollection(SIP_Message owner, string fieldName)
		{
			this.m_pMessage = owner;
			this.m_FieldName = fieldName;
			this.m_pFields = new List<SIP_SingleValueHF<T>>();
			this.Refresh();
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00010F44 File Offset: 0x0000FF44
		private void Refresh()
		{
			this.m_pFields.Clear();
			foreach (object obj in this.m_pMessage.Header)
			{
				SIP_HeaderField sip_HeaderField = (SIP_HeaderField)obj;
				bool flag = sip_HeaderField.Name.ToLower() == this.m_FieldName.ToLower();
				if (flag)
				{
					this.m_pFields.Add((SIP_SingleValueHF<T>)sip_HeaderField);
				}
			}
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00010FE0 File Offset: 0x0000FFE0
		public void Add(string value)
		{
			this.m_pMessage.Header.Add(this.m_FieldName, value);
			this.Refresh();
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00011002 File Offset: 0x00010002
		public void Remove(int index)
		{
			this.m_pMessage.Header.Remove(this.m_pFields[index]);
			this.m_pFields.RemoveAt(index);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0001102F File Offset: 0x0001002F
		public void Remove(SIP_SingleValueHF<T> field)
		{
			this.m_pMessage.Header.Remove(field);
			this.m_pFields.Remove(field);
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00011054 File Offset: 0x00010054
		public void RemoveAll()
		{
			foreach (SIP_SingleValueHF<T> field in this.m_pFields)
			{
				this.m_pMessage.Header.Remove(field);
			}
			this.m_pFields.Clear();
		}

		// Token: 0x0600031B RID: 795 RVA: 0x000110C4 File Offset: 0x000100C4
		public SIP_SingleValueHF<T> GetFirst()
		{
			bool flag = this.m_pFields.Count > 0;
			SIP_SingleValueHF<T> result;
			if (flag)
			{
				result = this.m_pFields[0];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600031C RID: 796 RVA: 0x000110FC File Offset: 0x000100FC
		public T[] GetAllValues()
		{
			List<T> list = new List<T>();
			foreach (SIP_SingleValueHF<T> sip_SingleValueHF in this.m_pFields)
			{
				list.Add(sip_SingleValueHF.ValueX);
			}
			return list.ToArray();
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600031D RID: 797 RVA: 0x0001116C File Offset: 0x0001016C
		public string FieldName
		{
			get
			{
				return this.m_FieldName;
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600031E RID: 798 RVA: 0x00011184 File Offset: 0x00010184
		public int Count
		{
			get
			{
				return this.m_pFields.Count;
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600031F RID: 799 RVA: 0x000111A4 File Offset: 0x000101A4
		public SIP_SingleValueHF<T>[] HeaderFields
		{
			get
			{
				return this.m_pFields.ToArray();
			}
		}

		// Token: 0x0400012A RID: 298
		private SIP_Message m_pMessage = null;

		// Token: 0x0400012B RID: 299
		private string m_FieldName = "";

		// Token: 0x0400012C RID: 300
		private List<SIP_SingleValueHF<T>> m_pFields = null;
	}
}
