using System;
using System.Text;

namespace LumiSoft.Net.MIME
{
	// Token: 0x0200010B RID: 267
	public class MIME_h_ContentDisposition : MIME_h
	{
		// Token: 0x06000A5B RID: 2651 RVA: 0x0003F49C File Offset: 0x0003E49C
		public MIME_h_ContentDisposition(string dispositionType)
		{
			bool flag = dispositionType == null;
			if (flag)
			{
				throw new ArgumentNullException("dispositionType");
			}
			bool flag2 = dispositionType == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'dispositionType' value must be specified.");
			}
			this.m_DispositionType = dispositionType;
			this.m_pParameters = new MIME_h_ParameterCollection(this);
			this.m_IsModified = true;
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x0003F51A File Offset: 0x0003E51A
		private MIME_h_ContentDisposition()
		{
			this.m_pParameters = new MIME_h_ParameterCollection(this);
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0003F550 File Offset: 0x0003E550
		public static MIME_h_ContentDisposition Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			string text = MIME_Encoding_EncodedWord.DecodeS(value);
			MIME_h_ContentDisposition mime_h_ContentDisposition = new MIME_h_ContentDisposition();
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
				throw new ParseException("Invalid Content-Disposition: header field value '" + value + "'.");
			}
			mime_h_ContentDisposition.m_DispositionType = text2.Trim();
			mime_h_ContentDisposition.m_pParameters.Parse(mime_Reader);
			mime_h_ContentDisposition.m_ParseValue = value;
			return mime_h_ContentDisposition;
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0003F618 File Offset: 0x0003E618
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			bool flag = reEncode || this.IsModified;
			string result;
			if (flag)
			{
				result = "Content-Disposition: " + this.m_DispositionType + this.m_pParameters.ToString(parmetersCharset) + "\r\n";
			}
			else
			{
				result = this.m_ParseValue;
			}
			return result;
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06000A5F RID: 2655 RVA: 0x0003F668 File Offset: 0x0003E668
		public override bool IsModified
		{
			get
			{
				return this.m_IsModified || this.m_pParameters.IsModified;
			}
		}

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06000A60 RID: 2656 RVA: 0x0003F690 File Offset: 0x0003E690
		public override string Name
		{
			get
			{
				return "Content-Disposition";
			}
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06000A61 RID: 2657 RVA: 0x0003F6A8 File Offset: 0x0003E6A8
		public string DispositionType
		{
			get
			{
				return this.m_DispositionType;
			}
		}

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06000A62 RID: 2658 RVA: 0x0003F6C0 File Offset: 0x0003E6C0
		public MIME_h_ParameterCollection Parameters
		{
			get
			{
				return this.m_pParameters;
			}
		}

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06000A63 RID: 2659 RVA: 0x0003F6D8 File Offset: 0x0003E6D8
		// (set) Token: 0x06000A64 RID: 2660 RVA: 0x0003F6FA File Offset: 0x0003E6FA
		public string Param_FileName
		{
			get
			{
				return this.Parameters["filename"];
			}
			set
			{
				this.m_pParameters["filename"] = value;
			}
		}

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06000A65 RID: 2661 RVA: 0x0003F710 File Offset: 0x0003E710
		// (set) Token: 0x06000A66 RID: 2662 RVA: 0x0003F74C File Offset: 0x0003E74C
		public DateTime Param_CreationDate
		{
			get
			{
				string text = this.Parameters["creation-date"];
				bool flag = text == null;
				DateTime result;
				if (flag)
				{
					result = DateTime.MinValue;
				}
				else
				{
					result = MIME_Utils.ParseRfc2822DateTime(text);
				}
				return result;
			}
			set
			{
				bool flag = value == DateTime.MinValue;
				if (flag)
				{
					this.Parameters.Remove("creation-date");
				}
				else
				{
					this.Parameters["creation-date"] = MIME_Utils.DateTimeToRfc2822(value);
				}
			}
		}

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06000A67 RID: 2663 RVA: 0x0003F798 File Offset: 0x0003E798
		// (set) Token: 0x06000A68 RID: 2664 RVA: 0x0003F7D4 File Offset: 0x0003E7D4
		public DateTime Param_ModificationDate
		{
			get
			{
				string text = this.Parameters["modification-date"];
				bool flag = text == null;
				DateTime result;
				if (flag)
				{
					result = DateTime.MinValue;
				}
				else
				{
					result = MIME_Utils.ParseRfc2822DateTime(text);
				}
				return result;
			}
			set
			{
				bool flag = value == DateTime.MinValue;
				if (flag)
				{
					this.Parameters.Remove("modification-date");
				}
				else
				{
					this.Parameters["modification-date"] = MIME_Utils.DateTimeToRfc2822(value);
				}
			}
		}

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06000A69 RID: 2665 RVA: 0x0003F820 File Offset: 0x0003E820
		// (set) Token: 0x06000A6A RID: 2666 RVA: 0x0003F85C File Offset: 0x0003E85C
		public DateTime Param_ReadDate
		{
			get
			{
				string text = this.Parameters["read-date"];
				bool flag = text == null;
				DateTime result;
				if (flag)
				{
					result = DateTime.MinValue;
				}
				else
				{
					result = MIME_Utils.ParseRfc2822DateTime(text);
				}
				return result;
			}
			set
			{
				bool flag = value == DateTime.MinValue;
				if (flag)
				{
					this.Parameters.Remove("read-date");
				}
				else
				{
					this.Parameters["read-date"] = MIME_Utils.DateTimeToRfc2822(value);
				}
			}
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06000A6B RID: 2667 RVA: 0x0003F8A8 File Offset: 0x0003E8A8
		// (set) Token: 0x06000A6C RID: 2668 RVA: 0x0003F8E0 File Offset: 0x0003E8E0
		public long Param_Size
		{
			get
			{
				string text = this.Parameters["size"];
				bool flag = text == null;
				long result;
				if (flag)
				{
					result = -1L;
				}
				else
				{
					result = Convert.ToInt64(text);
				}
				return result;
			}
			set
			{
				bool flag = value < 0L;
				if (flag)
				{
					this.Parameters.Remove("size");
				}
				else
				{
					this.Parameters["size"] = value.ToString();
				}
			}
		}

		// Token: 0x04000462 RID: 1122
		private bool m_IsModified = false;

		// Token: 0x04000463 RID: 1123
		private string m_ParseValue = null;

		// Token: 0x04000464 RID: 1124
		private string m_DispositionType = "";

		// Token: 0x04000465 RID: 1125
		private MIME_h_ParameterCollection m_pParameters = null;
	}
}
