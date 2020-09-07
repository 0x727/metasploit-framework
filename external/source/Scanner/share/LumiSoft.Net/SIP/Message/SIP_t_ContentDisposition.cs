using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000047 RID: 71
	public class SIP_t_ContentDisposition : SIP_t_ValueWithParams
	{
		// Token: 0x06000258 RID: 600 RVA: 0x0000E36F File Offset: 0x0000D36F
		public SIP_t_ContentDisposition(string value)
		{
			this.Parse(value);
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000E38C File Offset: 0x0000D38C
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000E3BC File Offset: 0x0000D3BC
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
				throw new SIP_ParseException("SIP Content-Disposition 'disp-type' value is missing !");
			}
			this.m_DispositionType = text;
			base.ParseParameters(reader);
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000E408 File Offset: 0x0000D408
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_DispositionType);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600025C RID: 604 RVA: 0x0000E440 File Offset: 0x0000D440
		// (set) Token: 0x0600025D RID: 605 RVA: 0x0000E458 File Offset: 0x0000D458
		public string DispositionType
		{
			get
			{
				return this.m_DispositionType;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("DispositionType");
				}
				bool flag2 = !TextUtils.IsToken(value);
				if (flag2)
				{
					throw new ArgumentException("Invalid DispositionType value, value must be 'token' !");
				}
				this.m_DispositionType = value;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x0600025E RID: 606 RVA: 0x0000E49C File Offset: 0x0000D49C
		// (set) Token: 0x0600025F RID: 607 RVA: 0x0000E4D4 File Offset: 0x0000D4D4
		public string Handling
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["handling"];
				bool flag = sip_Parameter != null;
				string result;
				if (flag)
				{
					result = sip_Parameter.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					base.Parameters.Remove("handling");
				}
				else
				{
					base.Parameters.Set("handling", value);
				}
			}
		}

		// Token: 0x04000105 RID: 261
		private string m_DispositionType = "";
	}
}
