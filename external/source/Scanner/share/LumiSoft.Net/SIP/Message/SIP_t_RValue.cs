using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000056 RID: 86
	public class SIP_t_RValue : SIP_t_Value
	{
		// Token: 0x060002D3 RID: 723 RVA: 0x0000FEE4 File Offset: 0x0000EEE4
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0000FF14 File Offset: 0x0000EF14
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			string text = reader.ReadWord();
			bool flag2 = text == null;
			if (flag2)
			{
				throw new SIP_ParseException("Invalid 'r-value' value, 'namespace \".\" r-priority' is missing !");
			}
			string[] array = text.Split(new char[]
			{
				'.'
			});
			bool flag3 = array.Length != 2;
			if (flag3)
			{
				throw new SIP_ParseException("Invalid r-value !");
			}
			this.m_Namespace = array[0];
			this.m_Priority = array[1];
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0000FF90 File Offset: 0x0000EF90
		public override string ToStringValue()
		{
			return this.m_Namespace + "." + this.m_Priority;
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0000FFB8 File Offset: 0x0000EFB8
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x0000FFD0 File Offset: 0x0000EFD0
		public string Namespace
		{
			get
			{
				return this.m_Namespace;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Namespace");
				}
				bool flag2 = value == "";
				if (flag2)
				{
					throw new ArgumentException("Property Namespace value may not be '' !");
				}
				bool flag3 = !TextUtils.IsToken(value);
				if (flag3)
				{
					throw new ArgumentException("Property Namespace value must be 'token' !");
				}
				this.m_Namespace = value;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x00010030 File Offset: 0x0000F030
		// (set) Token: 0x060002D9 RID: 729 RVA: 0x00010048 File Offset: 0x0000F048
		public string Priority
		{
			get
			{
				return this.m_Priority;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Priority");
				}
				bool flag2 = value == "";
				if (flag2)
				{
					throw new ArgumentException("Property Priority value may not be '' !");
				}
				bool flag3 = !TextUtils.IsToken(value);
				if (flag3)
				{
					throw new ArgumentException("Property Priority value must be 'token' !");
				}
				this.m_Priority = value;
			}
		}

		// Token: 0x04000115 RID: 277
		private string m_Namespace = "";

		// Token: 0x04000116 RID: 278
		private string m_Priority = "";
	}
}
