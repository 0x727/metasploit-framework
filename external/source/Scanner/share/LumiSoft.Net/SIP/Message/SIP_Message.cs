using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000078 RID: 120
	public abstract class SIP_Message
	{
		// Token: 0x060003EC RID: 1004 RVA: 0x00013FC1 File Offset: 0x00012FC1
		public SIP_Message()
		{
			this.m_pHeader = new SIP_HeaderFieldCollection();
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00013FE4 File Offset: 0x00012FE4
		protected void InternalParse(byte[] data)
		{
			this.InternalParse(new MemoryStream(data));
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00013FF4 File Offset: 0x00012FF4
		protected void InternalParse(Stream stream)
		{
			this.Header.Parse(stream);
			int num = 0;
			try
			{
				num = Convert.ToInt32(this.m_pHeader.GetFirst("Content-Length:").Value);
			}
			catch
			{
			}
			bool flag = num > 0;
			if (flag)
			{
				byte[] array = new byte[num];
				stream.Read(array, 0, array.Length);
				this.Data = array;
			}
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0001406C File Offset: 0x0001306C
		protected void InternalToStream(Stream stream)
		{
			this.m_pHeader.RemoveAll("Content-Length:");
			bool flag = this.m_Data != null;
			if (flag)
			{
				this.m_pHeader.Add("Content-Length:", Convert.ToString(this.m_Data.Length));
			}
			else
			{
				this.m_pHeader.Add("Content-Length:", Convert.ToString(0));
			}
			byte[] bytes = Encoding.UTF8.GetBytes(this.m_pHeader.ToHeaderString());
			stream.Write(bytes, 0, bytes.Length);
			bool flag2 = this.m_Data != null && this.m_Data.Length != 0;
			if (flag2)
			{
				stream.Write(this.m_Data, 0, this.m_Data.Length);
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00014128 File Offset: 0x00013128
		public SIP_HeaderFieldCollection Header
		{
			get
			{
				return this.m_pHeader;
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060003F1 RID: 1009 RVA: 0x00014140 File Offset: 0x00013140
		public SIP_MVGroupHFCollection<SIP_t_AcceptRange> Accept
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_AcceptRange>(this, "Accept:");
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00014160 File Offset: 0x00013160
		public SIP_MVGroupHFCollection<SIP_t_ACValue> AcceptContact
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_ACValue>(this, "Accept-Contact:");
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060003F3 RID: 1011 RVA: 0x00014180 File Offset: 0x00013180
		public SIP_MVGroupHFCollection<SIP_t_Encoding> AcceptEncoding
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_Encoding>(this, "Accept-Encoding:");
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x000141A0 File Offset: 0x000131A0
		public SIP_MVGroupHFCollection<SIP_t_Language> AcceptLanguage
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_Language>(this, "Accept-Language:");
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060003F5 RID: 1013 RVA: 0x000141C0 File Offset: 0x000131C0
		public SIP_MVGroupHFCollection<SIP_t_RValue> AcceptResourcePriority
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_RValue>(this, "Accept-Resource-Priority:");
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x000141E0 File Offset: 0x000131E0
		public SIP_MVGroupHFCollection<SIP_t_AlertParam> AlertInfo
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_AlertParam>(this, "Alert-Info:");
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060003F7 RID: 1015 RVA: 0x00014200 File Offset: 0x00013200
		public SIP_MVGroupHFCollection<SIP_t_Method> Allow
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_Method>(this, "Allow:");
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00014220 File Offset: 0x00013220
		public SIP_MVGroupHFCollection<SIP_t_EventType> AllowEvents
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_EventType>(this, "Allow-Events:");
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00014240 File Offset: 0x00013240
		public SIP_SVGroupHFCollection<SIP_t_AuthenticationInfo> AuthenticationInfo
		{
			get
			{
				return new SIP_SVGroupHFCollection<SIP_t_AuthenticationInfo>(this, "Authentication-Info:");
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x00014260 File Offset: 0x00013260
		public SIP_SVGroupHFCollection<SIP_t_Credentials> Authorization
		{
			get
			{
				return new SIP_SVGroupHFCollection<SIP_t_Credentials>(this, "Authorization:");
			}
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060003FB RID: 1019 RVA: 0x00014280 File Offset: 0x00013280
		// (set) Token: 0x060003FC RID: 1020 RVA: 0x000142B8 File Offset: 0x000132B8
		public string CallID
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Call-ID:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Call-ID:");
				}
				else
				{
					this.m_pHeader.Set("Call-ID:", value);
				}
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060003FD RID: 1021 RVA: 0x000142F8 File Offset: 0x000132F8
		public SIP_MVGroupHFCollection<SIP_t_Info> CallInfo
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_Info>(this, "Call-Info:");
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060003FE RID: 1022 RVA: 0x00014318 File Offset: 0x00013318
		public SIP_MVGroupHFCollection<SIP_t_ContactParam> Contact
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_ContactParam>(this, "Contact:");
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x00014338 File Offset: 0x00013338
		// (set) Token: 0x06000400 RID: 1024 RVA: 0x00014374 File Offset: 0x00013374
		public SIP_t_ContentDisposition ContentDisposition
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Content-Disposition:");
				bool flag = first != null;
				SIP_t_ContentDisposition result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_ContentDisposition>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Content-Disposition:");
				}
				else
				{
					this.m_pHeader.Set("Content-Disposition:", value.ToStringValue());
				}
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x000143B8 File Offset: 0x000133B8
		public SIP_MVGroupHFCollection<SIP_t_ContentCoding> ContentEncoding
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_ContentCoding>(this, "Content-Encoding:");
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000402 RID: 1026 RVA: 0x000143D8 File Offset: 0x000133D8
		public SIP_MVGroupHFCollection<SIP_t_LanguageTag> ContentLanguage
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_LanguageTag>(this, "Content-Language:");
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x000143F8 File Offset: 0x000133F8
		public int ContentLength
		{
			get
			{
				bool flag = this.m_Data == null;
				int result;
				if (flag)
				{
					result = 0;
				}
				else
				{
					result = this.m_Data.Length;
				}
				return result;
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x00014428 File Offset: 0x00013428
		// (set) Token: 0x06000405 RID: 1029 RVA: 0x00014460 File Offset: 0x00013460
		public string ContentType
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Content-Type:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Content-Type:");
				}
				else
				{
					this.m_pHeader.Set("Content-Type:", value);
				}
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000406 RID: 1030 RVA: 0x000144A0 File Offset: 0x000134A0
		// (set) Token: 0x06000407 RID: 1031 RVA: 0x000144DC File Offset: 0x000134DC
		public SIP_t_CSeq CSeq
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("CSeq:");
				bool flag = first != null;
				SIP_t_CSeq result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_CSeq>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("CSeq:");
				}
				else
				{
					this.m_pHeader.Set("CSeq:", value.ToStringValue());
				}
			}
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x00014520 File Offset: 0x00013520
		// (set) Token: 0x06000409 RID: 1033 RVA: 0x0001456C File Offset: 0x0001356C
		public DateTime Date
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Date:");
				bool flag = first != null;
				DateTime result;
				if (flag)
				{
					result = DateTime.ParseExact(first.Value, "r", DateTimeFormatInfo.InvariantInfo);
				}
				else
				{
					result = DateTime.MinValue;
				}
				return result;
			}
			set
			{
				bool flag = value == DateTime.MinValue;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Date:");
				}
				else
				{
					this.m_pHeader.Set("Date:", value.ToString("r"));
				}
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600040A RID: 1034 RVA: 0x000145C0 File Offset: 0x000135C0
		public SIP_MVGroupHFCollection<SIP_t_ErrorUri> ErrorInfo
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_ErrorUri>(this, "Error-Info:");
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600040B RID: 1035 RVA: 0x000145E0 File Offset: 0x000135E0
		// (set) Token: 0x0600040C RID: 1036 RVA: 0x0001461C File Offset: 0x0001361C
		public SIP_t_Event Event
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Event:");
				bool flag = first != null;
				SIP_t_Event result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_Event>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Event:");
				}
				else
				{
					this.m_pHeader.Set("Event:", value.ToStringValue());
				}
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x00014660 File Offset: 0x00013660
		// (set) Token: 0x0600040E RID: 1038 RVA: 0x0001469C File Offset: 0x0001369C
		public int Expires
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Expires:");
				bool flag = first != null;
				int result;
				if (flag)
				{
					result = Convert.ToInt32(first.Value);
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Expires:");
				}
				else
				{
					this.m_pHeader.Set("Expires:", value.ToString());
				}
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x0600040F RID: 1039 RVA: 0x000146E4 File Offset: 0x000136E4
		// (set) Token: 0x06000410 RID: 1040 RVA: 0x00014720 File Offset: 0x00013720
		public SIP_t_From From
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("From:");
				bool flag = first != null;
				SIP_t_From result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_From>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("From:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_From>("From:", value));
				}
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000411 RID: 1041 RVA: 0x00014764 File Offset: 0x00013764
		public SIP_MVGroupHFCollection<SIP_t_HiEntry> HistoryInfo
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_HiEntry>(this, "History-Info:");
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x00014784 File Offset: 0x00013784
		// (set) Token: 0x06000413 RID: 1043 RVA: 0x000147BC File Offset: 0x000137BC
		public string Identity
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Identity:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Identity:");
				}
				else
				{
					this.m_pHeader.Set("Identity:", value);
				}
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000414 RID: 1044 RVA: 0x000147FC File Offset: 0x000137FC
		// (set) Token: 0x06000415 RID: 1045 RVA: 0x00014838 File Offset: 0x00013838
		public SIP_t_IdentityInfo IdentityInfo
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Identity-Info:");
				bool flag = first != null;
				SIP_t_IdentityInfo result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_IdentityInfo>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Identity-Info:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_IdentityInfo>("Identity-Info:", value));
				}
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x0001487C File Offset: 0x0001387C
		public SIP_MVGroupHFCollection<SIP_t_CallID> InReplyTo
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_CallID>(this, "In-Reply-To:");
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x0001489C File Offset: 0x0001389C
		// (set) Token: 0x06000418 RID: 1048 RVA: 0x000148D8 File Offset: 0x000138D8
		public SIP_t_Join Join
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Join:");
				bool flag = first != null;
				SIP_t_Join result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_Join>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Join:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_Join>("Join:", value));
				}
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x0001491C File Offset: 0x0001391C
		// (set) Token: 0x0600041A RID: 1050 RVA: 0x00014958 File Offset: 0x00013958
		public int MaxForwards
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Max-Forwards:");
				bool flag = first != null;
				int result;
				if (flag)
				{
					result = Convert.ToInt32(first.Value);
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Max-Forwards:");
				}
				else
				{
					this.m_pHeader.Set("Max-Forwards:", value.ToString());
				}
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600041B RID: 1051 RVA: 0x000149A0 File Offset: 0x000139A0
		// (set) Token: 0x0600041C RID: 1052 RVA: 0x000149D8 File Offset: 0x000139D8
		public string MimeVersion
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Mime-Version:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Mime-Version:");
				}
				else
				{
					this.m_pHeader.Set("Mime-Version:", value);
				}
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x00014A18 File Offset: 0x00013A18
		// (set) Token: 0x0600041E RID: 1054 RVA: 0x00014A54 File Offset: 0x00013A54
		public int MinExpires
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Min-Expires:");
				bool flag = first != null;
				int result;
				if (flag)
				{
					result = Convert.ToInt32(first.Value);
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Min-Expires:");
				}
				else
				{
					this.m_pHeader.Set("Min-Expires:", value.ToString());
				}
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600041F RID: 1055 RVA: 0x00014A9C File Offset: 0x00013A9C
		// (set) Token: 0x06000420 RID: 1056 RVA: 0x00014AD8 File Offset: 0x00013AD8
		public SIP_t_MinSE MinSE
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Min-SE:");
				bool flag = first != null;
				SIP_t_MinSE result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_MinSE>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Min-SE:");
				}
				else
				{
					this.m_pHeader.Set("Min-SE:", value.ToStringValue());
				}
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000421 RID: 1057 RVA: 0x00014B1C File Offset: 0x00013B1C
		// (set) Token: 0x06000422 RID: 1058 RVA: 0x00014B54 File Offset: 0x00013B54
		public string Organization
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Organization:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Organization:");
				}
				else
				{
					this.m_pHeader.Set("Organization:", value);
				}
			}
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x00014B94 File Offset: 0x00013B94
		public SIP_SVGroupHFCollection<SIP_t_AddressParam> Path
		{
			get
			{
				return new SIP_SVGroupHFCollection<SIP_t_AddressParam>(this, "Path:");
			}
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000424 RID: 1060 RVA: 0x00014BB4 File Offset: 0x00013BB4
		// (set) Token: 0x06000425 RID: 1061 RVA: 0x00014BEC File Offset: 0x00013BEC
		public string Priority
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Priority:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Priority:");
				}
				else
				{
					this.m_pHeader.Set("Priority:", value);
				}
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x00014C2C File Offset: 0x00013C2C
		public SIP_SVGroupHFCollection<SIP_t_Challenge> ProxyAuthenticate
		{
			get
			{
				return new SIP_SVGroupHFCollection<SIP_t_Challenge>(this, "Proxy-Authenticate:");
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x00014C4C File Offset: 0x00013C4C
		public SIP_SVGroupHFCollection<SIP_t_Credentials> ProxyAuthorization
		{
			get
			{
				return new SIP_SVGroupHFCollection<SIP_t_Credentials>(this, "Proxy-Authorization:");
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000428 RID: 1064 RVA: 0x00014C6C File Offset: 0x00013C6C
		public SIP_MVGroupHFCollection<SIP_t_OptionTag> ProxyRequire
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_OptionTag>(this, "Proxy-Require:");
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x00014C8C File Offset: 0x00013C8C
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x00014CC8 File Offset: 0x00013CC8
		public SIP_t_RAck RAck
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("RAck:");
				bool flag = first != null;
				SIP_t_RAck result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_RAck>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("RAck:");
				}
				else
				{
					this.m_pHeader.Set("RAck:", value.ToStringValue());
				}
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x0600042B RID: 1067 RVA: 0x00014D0C File Offset: 0x00013D0C
		public SIP_MVGroupHFCollection<SIP_t_ReasonValue> Reason
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_ReasonValue>(this, "Reason:");
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x0600042C RID: 1068 RVA: 0x00014D2C File Offset: 0x00013D2C
		public SIP_MVGroupHFCollection<SIP_t_AddressParam> RecordRoute
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_AddressParam>(this, "Record-Route:");
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x0600042D RID: 1069 RVA: 0x00014D4C File Offset: 0x00013D4C
		// (set) Token: 0x0600042E RID: 1070 RVA: 0x00014D88 File Offset: 0x00013D88
		public SIP_t_ReferSub ReferSub
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Refer-Sub:");
				bool flag = first != null;
				SIP_t_ReferSub result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_ReferSub>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Refer-Sub:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_ReferSub>("Refer-Sub:", value));
				}
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x00014DCC File Offset: 0x00013DCC
		// (set) Token: 0x06000430 RID: 1072 RVA: 0x00014E08 File Offset: 0x00013E08
		public SIP_t_AddressParam ReferTo
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Refer-To:");
				bool flag = first != null;
				SIP_t_AddressParam result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_AddressParam>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Refer-To:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_AddressParam>("Refer-To:", value));
				}
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x00014E4C File Offset: 0x00013E4C
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x00014E88 File Offset: 0x00013E88
		public SIP_t_ReferredBy ReferredBy
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Referred-By:");
				bool flag = first != null;
				SIP_t_ReferredBy result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_ReferredBy>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Referred-By:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_ReferredBy>("Referred-By:", value));
				}
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x00014ECC File Offset: 0x00013ECC
		public SIP_MVGroupHFCollection<SIP_t_RCValue> RejectContact
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_RCValue>(this, "Reject-Contact:");
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x00014EEC File Offset: 0x00013EEC
		// (set) Token: 0x06000435 RID: 1077 RVA: 0x00014F28 File Offset: 0x00013F28
		public SIP_t_Replaces Replaces
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Replaces:");
				bool flag = first != null;
				SIP_t_Replaces result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_Replaces>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Replaces:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_Replaces>("Replaces:", value));
				}
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x00014F6C File Offset: 0x00013F6C
		public SIP_MVGroupHFCollection<SIP_t_AddressParam> ReplyTo
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_AddressParam>(this, "Reply-To:");
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000437 RID: 1079 RVA: 0x00014F8C File Offset: 0x00013F8C
		public SIP_MVGroupHFCollection<SIP_t_Directive> RequestDisposition
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_Directive>(this, "Request-Disposition:");
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x00014FAC File Offset: 0x00013FAC
		public SIP_MVGroupHFCollection<SIP_t_OptionTag> Require
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_OptionTag>(this, "Require:");
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000439 RID: 1081 RVA: 0x00014FCC File Offset: 0x00013FCC
		public SIP_MVGroupHFCollection<SIP_t_RValue> ResourcePriority
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_RValue>(this, "Resource-Priority:");
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x00014FEC File Offset: 0x00013FEC
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x00015028 File Offset: 0x00014028
		public SIP_t_RetryAfter RetryAfter
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Retry-After:");
				bool flag = first != null;
				SIP_t_RetryAfter result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_RetryAfter>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Retry-After:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_RetryAfter>("Retry-After:", value));
				}
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x0001506C File Offset: 0x0001406C
		public SIP_MVGroupHFCollection<SIP_t_AddressParam> Route
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_AddressParam>(this, "Route:");
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x0001508C File Offset: 0x0001408C
		// (set) Token: 0x0600043E RID: 1086 RVA: 0x000150C8 File Offset: 0x000140C8
		public int RSeq
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("RSeq:");
				bool flag = first != null;
				int result;
				if (flag)
				{
					result = Convert.ToInt32(first.Value);
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("RSeq:");
				}
				else
				{
					this.m_pHeader.Set("RSeq:", value.ToString());
				}
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600043F RID: 1087 RVA: 0x00015110 File Offset: 0x00014110
		public SIP_MVGroupHFCollection<SIP_t_SecMechanism> SecurityClient
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_SecMechanism>(this, "Security-Client:");
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00015130 File Offset: 0x00014130
		public SIP_MVGroupHFCollection<SIP_t_SecMechanism> SecurityServer
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_SecMechanism>(this, "Security-Server:");
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000441 RID: 1089 RVA: 0x00015150 File Offset: 0x00014150
		public SIP_MVGroupHFCollection<SIP_t_SecMechanism> SecurityVerify
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_SecMechanism>(this, "Security-Verify:");
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00015170 File Offset: 0x00014170
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x000151A8 File Offset: 0x000141A8
		public string Server
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Server:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Server:");
				}
				else
				{
					this.m_pHeader.Set("Server:", value);
				}
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x000151E8 File Offset: 0x000141E8
		public SIP_MVGroupHFCollection<SIP_t_AddressParam> ServiceRoute
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_AddressParam>(this, "Service-Route:");
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x00015208 File Offset: 0x00014208
		// (set) Token: 0x06000446 RID: 1094 RVA: 0x00015244 File Offset: 0x00014244
		public SIP_t_SessionExpires SessionExpires
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Session-Expires:");
				bool flag = first != null;
				SIP_t_SessionExpires result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_SessionExpires>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Session-Expires:");
				}
				else
				{
					this.m_pHeader.Set("Session-Expires:", value.ToStringValue());
				}
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000447 RID: 1095 RVA: 0x00015288 File Offset: 0x00014288
		// (set) Token: 0x06000448 RID: 1096 RVA: 0x000152C0 File Offset: 0x000142C0
		public string SIPETag
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("SIP-ETag:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("SIP-ETag:");
				}
				else
				{
					this.m_pHeader.Set("SIP-ETag:", value);
				}
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x00015300 File Offset: 0x00014300
		// (set) Token: 0x0600044A RID: 1098 RVA: 0x00015338 File Offset: 0x00014338
		public string SIPIfMatch
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("SIP-If-Match:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("SIP-If-Match:");
				}
				else
				{
					this.m_pHeader.Set("SIP-If-Match:", value);
				}
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x00015378 File Offset: 0x00014378
		// (set) Token: 0x0600044C RID: 1100 RVA: 0x000153B0 File Offset: 0x000143B0
		public string Subject
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Subject:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Subject:");
				}
				else
				{
					this.m_pHeader.Set("Subject:", value);
				}
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x0600044D RID: 1101 RVA: 0x000153F0 File Offset: 0x000143F0
		// (set) Token: 0x0600044E RID: 1102 RVA: 0x0001542C File Offset: 0x0001442C
		public SIP_t_SubscriptionState SubscriptionState
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Subscription-State:");
				bool flag = first != null;
				SIP_t_SubscriptionState result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_SubscriptionState>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Subscription-State:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_SubscriptionState>("Subscription-State:", value));
				}
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600044F RID: 1103 RVA: 0x00015470 File Offset: 0x00014470
		public SIP_MVGroupHFCollection<SIP_t_OptionTag> Supported
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_OptionTag>(this, "Supported:");
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000450 RID: 1104 RVA: 0x00015490 File Offset: 0x00014490
		// (set) Token: 0x06000451 RID: 1105 RVA: 0x000154CC File Offset: 0x000144CC
		public SIP_t_TargetDialog TargetDialog
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Target-Dialog:");
				bool flag = first != null;
				SIP_t_TargetDialog result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_TargetDialog>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Target-Dialog:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_TargetDialog>("Target-Dialog:", value));
				}
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000452 RID: 1106 RVA: 0x00015510 File Offset: 0x00014510
		// (set) Token: 0x06000453 RID: 1107 RVA: 0x0001554C File Offset: 0x0001454C
		public SIP_t_Timestamp Timestamp
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("Timestamp:");
				bool flag = first != null;
				SIP_t_Timestamp result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_Timestamp>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("Timestamp:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_Timestamp>("Timestamp:", value));
				}
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x00015590 File Offset: 0x00014590
		// (set) Token: 0x06000455 RID: 1109 RVA: 0x000155CC File Offset: 0x000145CC
		public SIP_t_To To
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("To:");
				bool flag = first != null;
				SIP_t_To result;
				if (flag)
				{
					result = ((SIP_SingleValueHF<SIP_t_To>)first).ValueX;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("To:");
				}
				else
				{
					this.m_pHeader.Add(new SIP_SingleValueHF<SIP_t_To>("To:", value));
				}
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000456 RID: 1110 RVA: 0x00015610 File Offset: 0x00014610
		public SIP_MVGroupHFCollection<SIP_t_OptionTag> Unsupported
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_OptionTag>(this, "Unsupported:");
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000457 RID: 1111 RVA: 0x00015630 File Offset: 0x00014630
		// (set) Token: 0x06000458 RID: 1112 RVA: 0x00015668 File Offset: 0x00014668
		public string UserAgent
		{
			get
			{
				SIP_HeaderField first = this.m_pHeader.GetFirst("User-Agent:");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveFirst("User-Agent:");
				}
				else
				{
					this.m_pHeader.Set("User-Agent:", value);
				}
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000459 RID: 1113 RVA: 0x000156A8 File Offset: 0x000146A8
		public SIP_MVGroupHFCollection<SIP_t_ViaParm> Via
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_ViaParm>(this, "Via:");
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600045A RID: 1114 RVA: 0x000156C8 File Offset: 0x000146C8
		public SIP_MVGroupHFCollection<SIP_t_WarningValue> Warning
		{
			get
			{
				return new SIP_MVGroupHFCollection<SIP_t_WarningValue>(this, "Warning:");
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600045B RID: 1115 RVA: 0x000156E8 File Offset: 0x000146E8
		public SIP_SVGroupHFCollection<SIP_t_Challenge> WWWAuthenticate
		{
			get
			{
				return new SIP_SVGroupHFCollection<SIP_t_Challenge>(this, "WWW-Authenticate:");
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600045C RID: 1116 RVA: 0x00015708 File Offset: 0x00014708
		// (set) Token: 0x0600045D RID: 1117 RVA: 0x00015720 File Offset: 0x00014720
		public byte[] Data
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		// Token: 0x04000154 RID: 340
		private SIP_HeaderFieldCollection m_pHeader = null;

		// Token: 0x04000155 RID: 341
		private byte[] m_Data = null;
	}
}
