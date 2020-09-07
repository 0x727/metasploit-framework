using System;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000189 RID: 393
	public abstract class IMAP_t_Fetch_r_i_BodyStructure_e
	{
		// Token: 0x06001024 RID: 4132 RVA: 0x00064932 File Offset: 0x00063932
		internal void SetParent(IMAP_t_Fetch_r_i_BodyStructure_e_Multipart parent)
		{
			this.m_pParent = parent;
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06001025 RID: 4133
		public abstract MIME_h_ContentType ContentType { get; }

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06001026 RID: 4134
		public abstract MIME_h_ContentDisposition ContentDisposition { get; }

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06001027 RID: 4135
		public abstract string Language { get; }

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06001028 RID: 4136
		public abstract string Location { get; }

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06001029 RID: 4137 RVA: 0x0006493C File Offset: 0x0006393C
		public string PartSpecifier
		{
			get
			{
				string text = "";
				bool flag = this.m_pParent == null;
				if (flag)
				{
					text = "";
				}
				else
				{
					IMAP_t_Fetch_r_i_BodyStructure_e bodyPart = this;
					for (IMAP_t_Fetch_r_i_BodyStructure_e_Multipart pParent = this.m_pParent; pParent != null; pParent = pParent.m_pParent)
					{
						int num = pParent.IndexOfBodyPart(bodyPart) + 1;
						bool flag2 = string.IsNullOrEmpty(text);
						if (flag2)
						{
							text = num.ToString();
						}
						else
						{
							text = num.ToString() + "." + text;
						}
						bodyPart = pParent;
					}
				}
				return text;
			}
		}

		// Token: 0x04000683 RID: 1667
		private IMAP_t_Fetch_r_i_BodyStructure_e_Multipart m_pParent = null;
	}
}
