using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000071 RID: 113
	public class SIP_MultiValueHF<T> : SIP_HeaderField where T : SIP_t_Value, new()
	{
		// Token: 0x060003AC RID: 940 RVA: 0x0001308B File Offset: 0x0001208B
		public SIP_MultiValueHF(string name, string value) : base(name, value)
		{
			this.m_pValues = new List<T>();
			base.SetMultiValue(true);
			this.Parse(value);
		}

		// Token: 0x060003AD RID: 941 RVA: 0x000130BC File Offset: 0x000120BC
		private void Parse(string value)
		{
			this.m_pValues.Clear();
			StringReader stringReader = new StringReader(value);
			while (stringReader.Available > 0L)
			{
				stringReader.ReadToFirstChar();
				bool flag = stringReader.StartsWith(",");
				if (flag)
				{
					stringReader.ReadSpecifiedLength(1);
				}
				T t = Activator.CreateInstance<T>();
				t.Parse(stringReader);
				this.m_pValues.Add(t);
			}
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00013130 File Offset: 0x00012130
		private string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.m_pValues.Count; i++)
			{
				stringBuilder.Append(this.m_pValues[i].ToStringValue());
				bool flag = i < this.m_pValues.Count - 1;
				if (flag)
				{
					stringBuilder.Append(',');
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060003AF RID: 943 RVA: 0x000131A8 File Offset: 0x000121A8
		public object[] GetValues()
		{
			return this.m_pValues.ToArray();
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x000131C8 File Offset: 0x000121C8
		public void Remove(int index)
		{
			bool flag = index > -1 && index < this.m_pValues.Count;
			if (flag)
			{
				this.m_pValues.RemoveAt(index);
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060003B1 RID: 945 RVA: 0x00013200 File Offset: 0x00012200
		// (set) Token: 0x060003B2 RID: 946 RVA: 0x00013218 File Offset: 0x00012218
		public override string Value
		{
			get
			{
				return this.ToStringValue();
			}
			set
			{
				bool flag = value != null;
				if (flag)
				{
					throw new ArgumentNullException("Property Value value may not be null !");
				}
				this.Parse(value);
				base.Value = value;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060003B3 RID: 947 RVA: 0x0001324C File Offset: 0x0001224C
		public List<T> Values
		{
			get
			{
				return this.m_pValues;
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x00013264 File Offset: 0x00012264
		public int Count
		{
			get
			{
				return this.m_pValues.Count;
			}
		}

		// Token: 0x0400014A RID: 330
		private List<T> m_pValues = null;
	}
}
