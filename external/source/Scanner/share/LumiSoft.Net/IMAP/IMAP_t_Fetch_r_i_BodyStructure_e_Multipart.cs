using System;
using System.Collections.Generic;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200018A RID: 394
	public class IMAP_t_Fetch_r_i_BodyStructure_e_Multipart : IMAP_t_Fetch_r_i_BodyStructure_e
	{
		// Token: 0x0600102A RID: 4138 RVA: 0x000649C8 File Offset: 0x000639C8
		private IMAP_t_Fetch_r_i_BodyStructure_e_Multipart()
		{
			this.m_pBodyParts = new List<IMAP_t_Fetch_r_i_BodyStructure_e>();
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x00064A00 File Offset: 0x00063A00
		public static IMAP_t_Fetch_r_i_BodyStructure_e_Multipart Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			IMAP_t_Fetch_r_i_BodyStructure_e_Multipart imap_t_Fetch_r_i_BodyStructure_e_Multipart = new IMAP_t_Fetch_r_i_BodyStructure_e_Multipart();
			while (r.Available > 0L)
			{
				r.ReadToFirstChar();
				bool flag2 = r.StartsWith("(");
				if (!flag2)
				{
					break;
				}
				StringReader stringReader = new StringReader(r.ReadParenthesized());
				stringReader.ReadToFirstChar();
				bool flag3 = stringReader.StartsWith("(");
				IMAP_t_Fetch_r_i_BodyStructure_e imap_t_Fetch_r_i_BodyStructure_e;
				if (flag3)
				{
					imap_t_Fetch_r_i_BodyStructure_e = IMAP_t_Fetch_r_i_BodyStructure_e_Multipart.Parse(stringReader);
				}
				else
				{
					imap_t_Fetch_r_i_BodyStructure_e = IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart.Parse(stringReader);
				}
				imap_t_Fetch_r_i_BodyStructure_e.SetParent(imap_t_Fetch_r_i_BodyStructure_e_Multipart);
				imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_pBodyParts.Add(imap_t_Fetch_r_i_BodyStructure_e);
			}
			string text = IMAP_Utils.ReadString(r);
			bool flag4 = !string.IsNullOrEmpty(text);
			if (flag4)
			{
				imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_pContentType = new MIME_h_ContentType("multipart/" + text);
			}
			r.ReadToFirstChar();
			bool flag5 = r.StartsWith("(");
			if (flag5)
			{
				StringReader stringReader2 = new StringReader(r.ReadParenthesized());
				bool flag6 = imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_pContentType != null;
				if (flag6)
				{
					while (stringReader2.Available > 0L)
					{
						string text2 = IMAP_Utils.ReadString(stringReader2);
						bool flag7 = string.IsNullOrEmpty(text2);
						if (flag7)
						{
							break;
						}
						string text3 = IMAP_Utils.ReadString(stringReader2);
						bool flag8 = text3 == null;
						if (flag8)
						{
							text3 = "";
						}
						imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_pContentType.Parameters[text2] = MIME_Encoding_EncodedWord.DecodeTextS(text3);
					}
				}
			}
			else
			{
				IMAP_Utils.ReadString(r);
			}
			bool flag9 = r.StartsWith("(");
			if (flag9)
			{
				string text4 = IMAP_Utils.ReadString(r);
				bool flag10 = !string.IsNullOrEmpty(text4);
				if (flag10)
				{
					imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_pContentDisposition = new MIME_h_ContentDisposition(text4);
				}
				r.ReadToFirstChar();
				bool flag11 = r.StartsWith("(");
				if (flag11)
				{
					StringReader stringReader3 = new StringReader(r.ReadParenthesized());
					bool flag12 = imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_pContentDisposition != null;
					if (flag12)
					{
						while (stringReader3.Available > 0L)
						{
							string text5 = IMAP_Utils.ReadString(stringReader3);
							bool flag13 = string.IsNullOrEmpty(text5);
							if (flag13)
							{
								break;
							}
							string text6 = IMAP_Utils.ReadString(stringReader3);
							bool flag14 = text6 == null;
							if (flag14)
							{
								text6 = "";
							}
							imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_pContentDisposition.Parameters[text5] = MIME_Encoding_EncodedWord.DecodeTextS(text6);
						}
					}
				}
				else
				{
					IMAP_Utils.ReadString(r);
				}
			}
			else
			{
				IMAP_Utils.ReadString(r);
			}
			r.ReadToFirstChar();
			bool flag15 = r.StartsWith("(");
			if (flag15)
			{
				imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_Language = r.ReadParenthesized();
			}
			else
			{
				imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_Language = IMAP_Utils.ReadString(r);
			}
			imap_t_Fetch_r_i_BodyStructure_e_Multipart.m_Location = IMAP_Utils.ReadString(r);
			return imap_t_Fetch_r_i_BodyStructure_e_Multipart;
		}

		// Token: 0x0600102C RID: 4140 RVA: 0x00064CCC File Offset: 0x00063CCC
		internal int IndexOfBodyPart(IMAP_t_Fetch_r_i_BodyStructure_e bodyPart)
		{
			return this.m_pBodyParts.IndexOf(bodyPart);
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x0600102D RID: 4141 RVA: 0x00064CEC File Offset: 0x00063CEC
		public override MIME_h_ContentType ContentType
		{
			get
			{
				return this.m_pContentType;
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x0600102E RID: 4142 RVA: 0x00064D04 File Offset: 0x00063D04
		public override MIME_h_ContentDisposition ContentDisposition
		{
			get
			{
				return this.m_pContentDisposition;
			}
		}

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x0600102F RID: 4143 RVA: 0x00064D1C File Offset: 0x00063D1C
		public override string Language
		{
			get
			{
				return this.m_Language;
			}
		}

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06001030 RID: 4144 RVA: 0x00064D34 File Offset: 0x00063D34
		public override string Location
		{
			get
			{
				return this.m_Location;
			}
		}

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06001031 RID: 4145 RVA: 0x00064D4C File Offset: 0x00063D4C
		public IMAP_t_Fetch_r_i_BodyStructure_e[] BodyParts
		{
			get
			{
				return this.m_pBodyParts.ToArray();
			}
		}

		// Token: 0x04000684 RID: 1668
		private MIME_h_ContentType m_pContentType = null;

		// Token: 0x04000685 RID: 1669
		private MIME_h_ContentDisposition m_pContentDisposition = null;

		// Token: 0x04000686 RID: 1670
		private string m_Language = null;

		// Token: 0x04000687 RID: 1671
		private string m_Location = null;

		// Token: 0x04000688 RID: 1672
		private List<IMAP_t_Fetch_r_i_BodyStructure_e> m_pBodyParts = null;
	}
}
