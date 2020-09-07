using System;
using System.Globalization;
using System.IO;
using System.Text;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200009E RID: 158
	public class SIP_Response : SIP_Message
	{
		// Token: 0x060005E4 RID: 1508 RVA: 0x0002128E File Offset: 0x0002028E
		public SIP_Response()
		{
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x000212C1 File Offset: 0x000202C1
		internal SIP_Response(SIP_Request request)
		{
			this.m_pRequest = request;
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x000212FC File Offset: 0x000202FC
		public SIP_Response Copy()
		{
			return SIP_Response.Parse(this.ToByteData());
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x0002131C File Offset: 0x0002031C
		public void Validate()
		{
			bool flag = base.Via.GetTopMostValue() == null;
			if (flag)
			{
				throw new SIP_ParseException("Via: header field is missing !");
			}
			bool flag2 = base.Via.GetTopMostValue().Branch == null;
			if (flag2)
			{
				throw new SIP_ParseException("Via: header fields branch parameter is missing !");
			}
			bool flag3 = base.To == null;
			if (flag3)
			{
				throw new SIP_ParseException("To: header field is missing !");
			}
			bool flag4 = base.From == null;
			if (flag4)
			{
				throw new SIP_ParseException("From: header field is missing !");
			}
			bool flag5 = base.CallID == null;
			if (flag5)
			{
				throw new SIP_ParseException("CallID: header field is missing !");
			}
			bool flag6 = base.CSeq == null;
			if (flag6)
			{
				throw new SIP_ParseException("CSeq: header field is missing !");
			}
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x000213D4 File Offset: 0x000203D4
		public static SIP_Response Parse(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			return SIP_Response.Parse(new MemoryStream(data));
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x00021408 File Offset: 0x00020408
		public static SIP_Response Parse(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			SIP_Response sip_Response = new SIP_Response();
			string[] array = new StreamLineReader(stream)
			{
				Encoding = "utf-8"
			}.ReadLineString().Split(new char[]
			{
				' '
			}, 3);
			bool flag2 = array.Length != 3;
			if (flag2)
			{
				throw new SIP_ParseException("Invalid SIP Status-Line syntax ! Syntax: {SIP-Version SP Status-Code SP Reason-Phrase}.");
			}
			try
			{
				sip_Response.SipVersion = Convert.ToDouble(array[0].Split(new char[]
				{
					'/'
				})[1], NumberFormatInfo.InvariantInfo);
			}
			catch
			{
				throw new SIP_ParseException("Invalid Status-Line SIP-Version value !");
			}
			try
			{
				sip_Response.StatusCode = Convert.ToInt32(array[1]);
			}
			catch
			{
				throw new SIP_ParseException("Invalid Status-Line Status-Code value !");
			}
			sip_Response.ReasonPhrase = array[2];
			sip_Response.InternalParse(stream);
			return sip_Response;
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00021504 File Offset: 0x00020504
		public void ToStream(Stream stream)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(string.Concat(new object[]
			{
				"SIP/",
				this.SipVersion.ToString("f1").Replace(',', '.'),
				" ",
				this.StatusCode,
				" ",
				this.ReasonPhrase,
				"\r\n"
			}));
			stream.Write(bytes, 0, bytes.Length);
			base.InternalToStream(stream);
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00021594 File Offset: 0x00020594
		public byte[] ToByteData()
		{
			MemoryStream memoryStream = new MemoryStream();
			this.ToStream(memoryStream);
			return memoryStream.ToArray();
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x000215BC File Offset: 0x000205BC
		public override string ToString()
		{
			return Encoding.UTF8.GetString(this.ToByteData());
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x000215E0 File Offset: 0x000205E0
		public SIP_Request Request
		{
			get
			{
				return this.m_pRequest;
			}
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x060005EE RID: 1518 RVA: 0x000215F8 File Offset: 0x000205F8
		// (set) Token: 0x060005EF RID: 1519 RVA: 0x00021610 File Offset: 0x00020610
		public double SipVersion
		{
			get
			{
				return this.m_SipVersion;
			}
			set
			{
				bool flag = value < 1.0;
				if (flag)
				{
					throw new ArgumentException("Property SIP version must be >= 1.0 !");
				}
				this.m_SipVersion = value;
			}
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x060005F0 RID: 1520 RVA: 0x00021644 File Offset: 0x00020644
		public SIP_StatusCodeType StatusCodeType
		{
			get
			{
				bool flag = this.m_StatusCode >= 100 && this.m_StatusCode < 200;
				SIP_StatusCodeType result;
				if (flag)
				{
					result = SIP_StatusCodeType.Provisional;
				}
				else
				{
					bool flag2 = this.m_StatusCode >= 200 && this.m_StatusCode < 300;
					if (flag2)
					{
						result = SIP_StatusCodeType.Success;
					}
					else
					{
						bool flag3 = this.m_StatusCode >= 300 && this.m_StatusCode < 400;
						if (flag3)
						{
							result = SIP_StatusCodeType.Redirection;
						}
						else
						{
							bool flag4 = this.m_StatusCode >= 400 && this.m_StatusCode < 500;
							if (flag4)
							{
								result = SIP_StatusCodeType.RequestFailure;
							}
							else
							{
								bool flag5 = this.m_StatusCode >= 500 && this.m_StatusCode < 600;
								if (flag5)
								{
									result = SIP_StatusCodeType.ServerFailure;
								}
								else
								{
									bool flag6 = this.m_StatusCode >= 600 && this.m_StatusCode < 700;
									if (!flag6)
									{
										throw new Exception("Unknown SIP StatusCodeType !");
									}
									result = SIP_StatusCodeType.GlobalFailure;
								}
							}
						}
					}
				}
				return result;
			}
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x00021750 File Offset: 0x00020750
		// (set) Token: 0x060005F2 RID: 1522 RVA: 0x00021768 File Offset: 0x00020768
		public int StatusCode
		{
			get
			{
				return this.m_StatusCode;
			}
			set
			{
				bool flag = value < 1 || value > 999;
				if (flag)
				{
					throw new ArgumentException("Property 'StatusCode' value must be >= 100 && <= 999 !");
				}
				this.m_StatusCode = value;
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x060005F3 RID: 1523 RVA: 0x0002179C File Offset: 0x0002079C
		// (set) Token: 0x060005F4 RID: 1524 RVA: 0x000217B4 File Offset: 0x000207B4
		public string ReasonPhrase
		{
			get
			{
				return this.m_ReasonPhrase;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("ReasonPhrase");
				}
				this.m_ReasonPhrase = value;
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x060005F5 RID: 1525 RVA: 0x000217E0 File Offset: 0x000207E0
		// (set) Token: 0x060005F6 RID: 1526 RVA: 0x00021810 File Offset: 0x00020810
		public string StatusCode_ReasonPhrase
		{
			get
			{
				return this.m_StatusCode + " " + this.m_ReasonPhrase;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("StatusCode_ReasonPhrase");
				}
				string[] array = value.Split(new char[]
				{
					' '
				}, 2);
				bool flag2 = array.Length != 2;
				if (flag2)
				{
					throw new ArgumentException("Invalid property 'StatusCode_ReasonPhrase' Reason-Phrase value !");
				}
				try
				{
					this.StatusCode = Convert.ToInt32(array[0]);
				}
				catch
				{
					throw new ArgumentException("Invalid property 'StatusCode_ReasonPhrase' Status-Code value !");
				}
				this.ReasonPhrase = array[1];
			}
		}

		// Token: 0x04000235 RID: 565
		private SIP_Request m_pRequest = null;

		// Token: 0x04000236 RID: 566
		private double m_SipVersion = 2.0;

		// Token: 0x04000237 RID: 567
		private int m_StatusCode = 100;

		// Token: 0x04000238 RID: 568
		private string m_ReasonPhrase = "";
	}
}
