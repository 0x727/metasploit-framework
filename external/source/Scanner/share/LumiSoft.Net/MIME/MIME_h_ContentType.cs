using System;
using System.Text;

namespace LumiSoft.Net.MIME
{
	// Token: 0x0200010C RID: 268
	public class MIME_h_ContentType : MIME_h
	{
		// Token: 0x06000A6D RID: 2669 RVA: 0x0003F928 File Offset: 0x0003E928
		public MIME_h_ContentType(string mediaType)
		{
			bool flag = mediaType == null;
			if (flag)
			{
				throw new ArgumentNullException(mediaType);
			}
			string[] array = mediaType.Split(new char[]
			{
				'/'
			}, 2);
			bool flag2 = array.Length == 2;
			if (!flag2)
			{
				throw new ArgumentException("Invalid argument 'mediaType' value '" + mediaType + "'.");
			}
			bool flag3 = array[0] == "" || !MIME_Reader.IsToken(array[0]);
			if (flag3)
			{
				throw new ArgumentException("Invalid argument 'mediaType' value '" + mediaType + "', value must be token.");
			}
			bool flag4 = array[1] == "" || !MIME_Reader.IsToken(array[1]);
			if (flag4)
			{
				throw new ArgumentException("Invalid argument 'mediaType' value '" + mediaType + "', value must be token.");
			}
			this.m_Type = array[0];
			this.m_SubType = array[1];
			this.m_pParameters = new MIME_h_ParameterCollection(this);
			this.m_IsModified = true;
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x0003FA4C File Offset: 0x0003EA4C
		private MIME_h_ContentType()
		{
			this.m_pParameters = new MIME_h_ParameterCollection(this);
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x0003FA98 File Offset: 0x0003EA98
		public static MIME_h_ContentType Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			string text = MIME_Encoding_EncodedWord.DecodeS(value);
			MIME_h_ContentType mime_h_ContentType = new MIME_h_ContentType();
			string[] array = text.Split(new char[]
			{
				':'
			}, 2);
			bool flag2 = array.Length != 2;
			if (flag2)
			{
				throw new ParseException("Invalid Content-Type: header field value '" + value + "'.");
			}
			MIME_Reader mime_Reader = new MIME_Reader(array[1]);
			string text2 = mime_Reader.Token();
			bool flag3 = text2 == null;
			if (flag3)
			{
				throw new ParseException("Invalid Content-Type: header field value '" + value + "'.");
			}
			mime_h_ContentType.m_Type = text2;
			bool flag4 = mime_Reader.Char(false) != 47;
			if (flag4)
			{
				throw new ParseException("Invalid Content-Type: header field value '" + value + "'.");
			}
			string text3 = mime_Reader.Token();
			bool flag5 = text3 == null;
			if (flag5)
			{
				throw new ParseException("Invalid Content-Type: header field value '" + value + "'.");
			}
			mime_h_ContentType.m_SubType = text3;
			bool flag6 = mime_Reader.Available > 0;
			if (flag6)
			{
				mime_h_ContentType.m_pParameters.Parse(mime_Reader);
			}
			mime_h_ContentType.m_ParseValue = value;
			mime_h_ContentType.m_IsModified = false;
			return mime_h_ContentType;
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0003FBD0 File Offset: 0x0003EBD0
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			bool flag = !reEncode && !this.IsModified;
			string result;
			if (flag)
			{
				result = this.m_ParseValue;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Content-Type: " + this.m_Type + "/" + this.m_SubType);
				stringBuilder.Append(this.m_pParameters.ToString(parmetersCharset));
				stringBuilder.Append("\r\n");
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06000A71 RID: 2673 RVA: 0x0003FC50 File Offset: 0x0003EC50
		public override bool IsModified
		{
			get
			{
				return this.m_IsModified || this.m_pParameters.IsModified;
			}
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06000A72 RID: 2674 RVA: 0x0003FC78 File Offset: 0x0003EC78
		public override string Name
		{
			get
			{
				return "Content-Type";
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06000A73 RID: 2675 RVA: 0x0003FC90 File Offset: 0x0003EC90
		public string Type
		{
			get
			{
				return this.m_Type;
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06000A74 RID: 2676 RVA: 0x0003FCA8 File Offset: 0x0003ECA8
		public string SubType
		{
			get
			{
				return this.m_SubType;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x06000A75 RID: 2677 RVA: 0x0003FCC0 File Offset: 0x0003ECC0
		[Obsolete("Mispelled 'TypeWithSubype', use TypeWithSubtype instead !")]
		public string TypeWithSubype
		{
			get
			{
				return this.m_Type + "/" + this.m_SubType;
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06000A76 RID: 2678 RVA: 0x0003FCE8 File Offset: 0x0003ECE8
		public string TypeWithSubtype
		{
			get
			{
				return this.m_Type + "/" + this.m_SubType;
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06000A77 RID: 2679 RVA: 0x0003FD10 File Offset: 0x0003ED10
		public MIME_h_ParameterCollection Parameters
		{
			get
			{
				return this.m_pParameters;
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06000A78 RID: 2680 RVA: 0x0003FD28 File Offset: 0x0003ED28
		// (set) Token: 0x06000A79 RID: 2681 RVA: 0x0003FD4A File Offset: 0x0003ED4A
		public string Param_Name
		{
			get
			{
				return this.m_pParameters["name"];
			}
			set
			{
				this.m_pParameters["name"] = value;
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0003FD60 File Offset: 0x0003ED60
		// (set) Token: 0x06000A7B RID: 2683 RVA: 0x0003FD82 File Offset: 0x0003ED82
		public string Param_Charset
		{
			get
			{
				return this.m_pParameters["charset"];
			}
			set
			{
				this.m_pParameters["charset"] = value;
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06000A7C RID: 2684 RVA: 0x0003FD98 File Offset: 0x0003ED98
		// (set) Token: 0x06000A7D RID: 2685 RVA: 0x0003FDBA File Offset: 0x0003EDBA
		public string Param_Boundary
		{
			get
			{
				return this.m_pParameters["boundary"];
			}
			set
			{
				this.m_pParameters["boundary"] = value;
			}
		}

		// Token: 0x04000466 RID: 1126
		private bool m_IsModified = false;

		// Token: 0x04000467 RID: 1127
		private string m_ParseValue = null;

		// Token: 0x04000468 RID: 1128
		private string m_Type = "";

		// Token: 0x04000469 RID: 1129
		private string m_SubType = "";

		// Token: 0x0400046A RID: 1130
		private MIME_h_ParameterCollection m_pParameters = null;
	}
}
