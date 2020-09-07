using System;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200018B RID: 395
	public class IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart : IMAP_t_Fetch_r_i_BodyStructure_e
	{
		// Token: 0x06001032 RID: 4146 RVA: 0x00064D6C File Offset: 0x00063D6C
		private IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart()
		{
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x00064DC8 File Offset: 0x00063DC8
		public static IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart imap_t_Fetch_r_i_BodyStructure_e_SinglePart = new IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart();
			string text = IMAP_Utils.ReadString(r);
			string text2 = IMAP_Utils.ReadString(r);
			bool flag2 = !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2);
			if (flag2)
			{
				imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_pContentType = new MIME_h_ContentType(text + "/" + text2);
			}
			r.ReadToFirstChar();
			bool flag3 = r.StartsWith("(");
			if (flag3)
			{
				StringReader stringReader = new StringReader(r.ReadParenthesized());
				bool flag4 = imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_pContentType != null;
				if (flag4)
				{
					while (stringReader.Available > 0L)
					{
						string text3 = IMAP_Utils.ReadString(stringReader);
						bool flag5 = string.IsNullOrEmpty(text3);
						if (flag5)
						{
							break;
						}
						string text4 = IMAP_Utils.ReadString(stringReader);
						bool flag6 = text4 == null;
						if (flag6)
						{
							text4 = "";
						}
						imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_pContentType.Parameters[text3] = MIME_Encoding_EncodedWord.DecodeTextS(text4);
					}
				}
			}
			else
			{
				IMAP_Utils.ReadString(r);
			}
			imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_ContentID = IMAP_Utils.ReadString(r);
			imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_ContentDescription = IMAP_Utils.ReadString(r);
			imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_ContentTransferEncoding = IMAP_Utils.ReadString(r);
			string value = IMAP_Utils.ReadString(r);
			bool flag7 = string.IsNullOrEmpty(value);
			if (flag7)
			{
				imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_ContentSize = -1L;
			}
			else
			{
				imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_ContentSize = Convert.ToInt64(value);
			}
			bool flag8 = string.Equals("text", text, StringComparison.InvariantCultureIgnoreCase);
			if (flag8)
			{
				string value2 = IMAP_Utils.ReadString(r);
				bool flag9 = string.IsNullOrEmpty(value);
				if (flag9)
				{
					imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_LinesCount = -1;
				}
				else
				{
					imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_LinesCount = Convert.ToInt32(value2);
				}
			}
			bool flag10 = string.Equals("message", text, StringComparison.InvariantCultureIgnoreCase);
			if (flag10)
			{
				r.ReadToFirstChar();
				bool flag11 = r.StartsWith("(");
				if (flag11)
				{
					string text5 = r.ReadParenthesized();
				}
				else
				{
					IMAP_Utils.ReadString(r);
				}
				r.ReadToFirstChar();
				bool flag12 = r.StartsWith("(");
				if (flag12)
				{
					string text6 = r.ReadParenthesized();
				}
				else
				{
					IMAP_Utils.ReadString(r);
				}
				string value3 = IMAP_Utils.ReadString(r);
				bool flag13 = string.IsNullOrEmpty(value);
				if (flag13)
				{
					imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_LinesCount = -1;
				}
				else
				{
					imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_LinesCount = Convert.ToInt32(value3);
				}
			}
			imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_Md5 = IMAP_Utils.ReadString(r);
			bool flag14 = r.StartsWith("(");
			if (flag14)
			{
				string text7 = IMAP_Utils.ReadString(r);
				bool flag15 = !string.IsNullOrEmpty(text7);
				if (flag15)
				{
					imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_pContentDisposition = new MIME_h_ContentDisposition(text7);
				}
				r.ReadToFirstChar();
				bool flag16 = r.StartsWith("(");
				if (flag16)
				{
					StringReader stringReader2 = new StringReader(r.ReadParenthesized());
					bool flag17 = imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_pContentDisposition != null;
					if (flag17)
					{
						while (stringReader2.Available > 0L)
						{
							string text8 = IMAP_Utils.ReadString(stringReader2);
							bool flag18 = string.IsNullOrEmpty(text8);
							if (flag18)
							{
								break;
							}
							string text9 = IMAP_Utils.ReadString(stringReader2);
							bool flag19 = text9 == null;
							if (flag19)
							{
								text9 = "";
							}
							imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_pContentDisposition.Parameters[text8] = MIME_Encoding_EncodedWord.DecodeTextS(text9);
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
			bool flag20 = r.StartsWith("(");
			if (flag20)
			{
				imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_Language = r.ReadParenthesized();
			}
			else
			{
				imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_Language = IMAP_Utils.ReadString(r);
			}
			imap_t_Fetch_r_i_BodyStructure_e_SinglePart.m_Location = IMAP_Utils.ReadString(r);
			return imap_t_Fetch_r_i_BodyStructure_e_SinglePart;
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x06001034 RID: 4148 RVA: 0x00065158 File Offset: 0x00064158
		public override MIME_h_ContentType ContentType
		{
			get
			{
				return this.m_pContentType;
			}
		}

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06001035 RID: 4149 RVA: 0x00065170 File Offset: 0x00064170
		public string ContentID
		{
			get
			{
				return this.m_ContentID;
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06001036 RID: 4150 RVA: 0x00065188 File Offset: 0x00064188
		public string ContentDescription
		{
			get
			{
				return this.m_ContentDescription;
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06001037 RID: 4151 RVA: 0x000651A0 File Offset: 0x000641A0
		public string ContentTransferEncoding
		{
			get
			{
				return this.m_ContentTransferEncoding;
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06001038 RID: 4152 RVA: 0x000651B8 File Offset: 0x000641B8
		public long ContentSize
		{
			get
			{
				return this.m_ContentSize;
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x06001039 RID: 4153 RVA: 0x000651D0 File Offset: 0x000641D0
		public int LinesCount
		{
			get
			{
				return this.m_LinesCount;
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x0600103A RID: 4154 RVA: 0x000651E8 File Offset: 0x000641E8
		public string Md5
		{
			get
			{
				return this.m_Md5;
			}
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x0600103B RID: 4155 RVA: 0x00065200 File Offset: 0x00064200
		public override MIME_h_ContentDisposition ContentDisposition
		{
			get
			{
				return this.m_pContentDisposition;
			}
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x0600103C RID: 4156 RVA: 0x00065218 File Offset: 0x00064218
		public override string Language
		{
			get
			{
				return this.m_Language;
			}
		}

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x0600103D RID: 4157 RVA: 0x00065230 File Offset: 0x00064230
		public override string Location
		{
			get
			{
				return this.m_Location;
			}
		}

		// Token: 0x04000689 RID: 1673
		private MIME_h_ContentType m_pContentType = null;

		// Token: 0x0400068A RID: 1674
		private string m_ContentID = null;

		// Token: 0x0400068B RID: 1675
		private string m_ContentDescription = null;

		// Token: 0x0400068C RID: 1676
		private string m_ContentTransferEncoding = null;

		// Token: 0x0400068D RID: 1677
		private long m_ContentSize = -1L;

		// Token: 0x0400068E RID: 1678
		private int m_LinesCount = -1;

		// Token: 0x0400068F RID: 1679
		private string m_Md5 = null;

		// Token: 0x04000690 RID: 1680
		private MIME_h_ContentDisposition m_pContentDisposition = null;

		// Token: 0x04000691 RID: 1681
		private string m_Language = null;

		// Token: 0x04000692 RID: 1682
		private string m_Location = null;
	}
}
