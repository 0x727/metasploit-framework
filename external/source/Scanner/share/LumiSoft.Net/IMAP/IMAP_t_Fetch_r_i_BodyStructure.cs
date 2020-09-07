using System;
using System.Collections.Generic;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001B6 RID: 438
	public class IMAP_t_Fetch_r_i_BodyStructure : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010D2 RID: 4306 RVA: 0x00067DC2 File Offset: 0x00066DC2
		private IMAP_t_Fetch_r_i_BodyStructure()
		{
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x00067DD4 File Offset: 0x00066DD4
		public static IMAP_t_Fetch_r_i_BodyStructure Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			IMAP_t_Fetch_r_i_BodyStructure imap_t_Fetch_r_i_BodyStructure = new IMAP_t_Fetch_r_i_BodyStructure();
			r.ReadToFirstChar();
			bool flag2 = r.StartsWith("(");
			if (flag2)
			{
				imap_t_Fetch_r_i_BodyStructure.m_pMessage = IMAP_t_Fetch_r_i_BodyStructure_e_Multipart.Parse(r);
			}
			else
			{
				imap_t_Fetch_r_i_BodyStructure.m_pMessage = IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart.Parse(r);
			}
			return imap_t_Fetch_r_i_BodyStructure;
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x00067E38 File Offset: 0x00066E38
		public IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart[] GetAttachments(bool includeInline)
		{
			List<IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart> list = new List<IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart>();
			foreach (IMAP_t_Fetch_r_i_BodyStructure_e imap_t_Fetch_r_i_BodyStructure_e in this.AllEntities)
			{
				MIME_h_ContentType contentType = imap_t_Fetch_r_i_BodyStructure_e.ContentType;
				MIME_h_ContentDisposition contentDisposition = imap_t_Fetch_r_i_BodyStructure_e.ContentDisposition;
				bool flag = imap_t_Fetch_r_i_BodyStructure_e is IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart;
				if (flag)
				{
					bool flag2 = contentDisposition != null && string.Equals(contentDisposition.DispositionType, "attachment", StringComparison.InvariantCultureIgnoreCase);
					if (flag2)
					{
						list.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)imap_t_Fetch_r_i_BodyStructure_e);
					}
					else
					{
						bool flag3 = contentDisposition != null && string.Equals(contentDisposition.DispositionType, "inline", StringComparison.InvariantCultureIgnoreCase);
						if (flag3)
						{
							if (includeInline)
							{
								list.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)imap_t_Fetch_r_i_BodyStructure_e);
							}
						}
						else
						{
							bool flag4 = contentType != null && string.Equals(contentType.Type, "application", StringComparison.InvariantCultureIgnoreCase);
							if (flag4)
							{
								list.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)imap_t_Fetch_r_i_BodyStructure_e);
							}
							else
							{
								bool flag5 = contentType != null && string.Equals(contentType.Type, "image", StringComparison.InvariantCultureIgnoreCase);
								if (flag5)
								{
									list.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)imap_t_Fetch_r_i_BodyStructure_e);
								}
								else
								{
									bool flag6 = contentType != null && string.Equals(contentType.Type, "video", StringComparison.InvariantCultureIgnoreCase);
									if (flag6)
									{
										list.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)imap_t_Fetch_r_i_BodyStructure_e);
									}
									else
									{
										bool flag7 = contentType != null && string.Equals(contentType.Type, "audio", StringComparison.InvariantCultureIgnoreCase);
										if (flag7)
										{
											list.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)imap_t_Fetch_r_i_BodyStructure_e);
										}
										else
										{
											bool flag8 = contentType != null && string.Equals(contentType.Type, "message", StringComparison.InvariantCultureIgnoreCase);
											if (flag8)
											{
												list.Add((IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)imap_t_Fetch_r_i_BodyStructure_e);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x060010D5 RID: 4309 RVA: 0x00068004 File Offset: 0x00067004
		public bool IsSigned
		{
			get
			{
				IMAP_t_Fetch_r_i_BodyStructure_e[] allEntities = this.AllEntities;
				int i = 0;
				while (i < allEntities.Length)
				{
					IMAP_t_Fetch_r_i_BodyStructure_e imap_t_Fetch_r_i_BodyStructure_e = allEntities[i];
					bool flag = string.Equals(imap_t_Fetch_r_i_BodyStructure_e.ContentType.TypeWithSubtype, MIME_MediaTypes.Application.pkcs7_mime, StringComparison.InvariantCultureIgnoreCase);
					bool result;
					if (flag)
					{
						result = true;
					}
					else
					{
						bool flag2 = string.Equals(imap_t_Fetch_r_i_BodyStructure_e.ContentType.TypeWithSubtype, MIME_MediaTypes.Multipart.signed, StringComparison.InvariantCultureIgnoreCase);
						if (!flag2)
						{
							i++;
							continue;
						}
						result = true;
					}
					return result;
				}
				return false;
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x060010D6 RID: 4310 RVA: 0x00068078 File Offset: 0x00067078
		public IMAP_t_Fetch_r_i_BodyStructure_e Message
		{
			get
			{
				return this.m_pMessage;
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x060010D7 RID: 4311 RVA: 0x00068090 File Offset: 0x00067090
		public IMAP_t_Fetch_r_i_BodyStructure_e[] AllEntities
		{
			get
			{
				List<IMAP_t_Fetch_r_i_BodyStructure_e> list = new List<IMAP_t_Fetch_r_i_BodyStructure_e>();
				List<IMAP_t_Fetch_r_i_BodyStructure_e> list2 = new List<IMAP_t_Fetch_r_i_BodyStructure_e>();
				list2.Add(this.m_pMessage);
				while (list2.Count > 0)
				{
					IMAP_t_Fetch_r_i_BodyStructure_e imap_t_Fetch_r_i_BodyStructure_e = list2[0];
					list2.RemoveAt(0);
					list.Add(imap_t_Fetch_r_i_BodyStructure_e);
					bool flag = imap_t_Fetch_r_i_BodyStructure_e is IMAP_t_Fetch_r_i_BodyStructure_e_Multipart;
					if (flag)
					{
						IMAP_t_Fetch_r_i_BodyStructure_e[] bodyParts = ((IMAP_t_Fetch_r_i_BodyStructure_e_Multipart)imap_t_Fetch_r_i_BodyStructure_e).BodyParts;
						for (int i = 0; i < bodyParts.Length; i++)
						{
							list2.Insert(i, bodyParts[i]);
						}
					}
				}
				return list.ToArray();
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x060010D8 RID: 4312 RVA: 0x00068134 File Offset: 0x00067134
		public IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart[] Attachments
		{
			get
			{
				return this.GetAttachments(false);
			}
		}

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x060010D9 RID: 4313 RVA: 0x00068150 File Offset: 0x00067150
		public IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart BodyTextEntity
		{
			get
			{
				foreach (IMAP_t_Fetch_r_i_BodyStructure_e imap_t_Fetch_r_i_BodyStructure_e in this.AllEntities)
				{
					bool flag = string.Equals(imap_t_Fetch_r_i_BodyStructure_e.ContentType.TypeWithSubtype, MIME_MediaTypes.Text.plain, StringComparison.InvariantCultureIgnoreCase);
					if (flag)
					{
						return (IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)imap_t_Fetch_r_i_BodyStructure_e;
					}
				}
				return null;
			}
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x060010DA RID: 4314 RVA: 0x000681A8 File Offset: 0x000671A8
		public IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart BodyTextHtmlEntity
		{
			get
			{
				foreach (IMAP_t_Fetch_r_i_BodyStructure_e imap_t_Fetch_r_i_BodyStructure_e in this.AllEntities)
				{
					bool flag = string.Equals(imap_t_Fetch_r_i_BodyStructure_e.ContentType.TypeWithSubtype, MIME_MediaTypes.Text.html, StringComparison.InvariantCultureIgnoreCase);
					if (flag)
					{
						return (IMAP_t_Fetch_r_i_BodyStructure_e_SinglePart)imap_t_Fetch_r_i_BodyStructure_e;
					}
				}
				return null;
			}
		}

		// Token: 0x040006B5 RID: 1717
		private IMAP_t_Fetch_r_i_BodyStructure_e m_pMessage = null;
	}
}
