using System;
using System.IO;
using System.Net;
using System.Text;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200009D RID: 157
	public class SIP_Request : SIP_Message
	{
		// Token: 0x060005D5 RID: 1493 RVA: 0x00020E74 File Offset: 0x0001FE74
		public SIP_Request(string method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new ArgumentNullException("method");
			}
			this.m_pRequestLine = new SIP_RequestLine(method, new AbsoluteUri());
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x00020ECC File Offset: 0x0001FECC
		public SIP_Request Copy()
		{
			SIP_Request sip_Request = SIP_Request.Parse(this.ToByteData());
			sip_Request.Flow = this.m_pFlow;
			sip_Request.LocalEndPoint = this.m_pLocalEP;
			sip_Request.RemoteEndPoint = this.m_pRemoteEP;
			return sip_Request;
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x00020F14 File Offset: 0x0001FF14
		public void Validate()
		{
			bool flag = !this.RequestLine.Version.ToUpper().StartsWith("SIP/2.0");
			if (flag)
			{
				throw new SIP_ParseException("Not supported SIP version '" + this.RequestLine.Version + "' !");
			}
			bool flag2 = base.Via.GetTopMostValue() == null;
			if (flag2)
			{
				throw new SIP_ParseException("Via: header field is missing !");
			}
			bool flag3 = base.Via.GetTopMostValue().Branch == null;
			if (flag3)
			{
				throw new SIP_ParseException("Via: header field branch parameter is missing !");
			}
			bool flag4 = base.To == null;
			if (flag4)
			{
				throw new SIP_ParseException("To: header field is missing !");
			}
			bool flag5 = base.From == null;
			if (flag5)
			{
				throw new SIP_ParseException("From: header field is missing !");
			}
			bool flag6 = base.CallID == null;
			if (flag6)
			{
				throw new SIP_ParseException("CallID: header field is missing !");
			}
			bool flag7 = base.CSeq == null;
			if (flag7)
			{
				throw new SIP_ParseException("CSeq: header field is missing !");
			}
			bool flag8 = base.MaxForwards == -1;
			if (flag8)
			{
				base.MaxForwards = 70;
			}
			bool flag9 = SIP_Utils.MethodCanEstablishDialog(this.RequestLine.Method);
			if (flag9)
			{
				bool flag10 = base.Contact.GetAllValues().Length == 0;
				if (flag10)
				{
					throw new SIP_ParseException("Contact: header field is missing, method that can establish a dialog MUST provide a SIP or SIPS URI !");
				}
				bool flag11 = base.Contact.GetAllValues().Length > 1;
				if (flag11)
				{
					throw new SIP_ParseException("There may be only 1 Contact: header for the method that can establish a dialog !");
				}
				bool flag12 = !base.Contact.GetTopMostValue().Address.IsSipOrSipsUri;
				if (flag12)
				{
					throw new SIP_ParseException("Method that can establish a dialog MUST have SIP or SIPS uri in Contact: header !");
				}
			}
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x000210B0 File Offset: 0x000200B0
		public static SIP_Request Parse(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			return SIP_Request.Parse(new MemoryStream(data));
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x000210E4 File Offset: 0x000200E4
		public static SIP_Request Parse(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			string[] array = new StreamLineReader(stream)
			{
				Encoding = "utf-8"
			}.ReadLineString().Split(new char[]
			{
				' '
			});
			bool flag2 = array.Length != 3;
			if (flag2)
			{
				throw new Exception("Invalid SIP request data ! Method line doesn't contain: SIP-Method SIP-URI SIP-Version.");
			}
			SIP_Request sip_Request = new SIP_Request(array[0]);
			sip_Request.RequestLine.Uri = AbsoluteUri.Parse(array[1]);
			sip_Request.RequestLine.Version = array[2];
			sip_Request.InternalParse(stream);
			return sip_Request;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x00021188 File Offset: 0x00020188
		public void ToStream(Stream stream)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(this.m_pRequestLine.ToString());
			stream.Write(bytes, 0, bytes.Length);
			base.InternalToStream(stream);
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x000211C0 File Offset: 0x000201C0
		public byte[] ToByteData()
		{
			MemoryStream memoryStream = new MemoryStream();
			this.ToStream(memoryStream);
			return memoryStream.ToArray();
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x000211E8 File Offset: 0x000201E8
		public override string ToString()
		{
			return Encoding.UTF8.GetString(this.ToByteData());
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x060005DD RID: 1501 RVA: 0x0002120C File Offset: 0x0002020C
		public SIP_RequestLine RequestLine
		{
			get
			{
				return this.m_pRequestLine;
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x060005DE RID: 1502 RVA: 0x00021224 File Offset: 0x00020224
		// (set) Token: 0x060005DF RID: 1503 RVA: 0x0002123C File Offset: 0x0002023C
		internal SIP_Flow Flow
		{
			get
			{
				return this.m_pFlow;
			}
			set
			{
				this.m_pFlow = value;
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x00021248 File Offset: 0x00020248
		// (set) Token: 0x060005E1 RID: 1505 RVA: 0x00021260 File Offset: 0x00020260
		internal IPEndPoint LocalEndPoint
		{
			get
			{
				return this.m_pLocalEP;
			}
			set
			{
				this.m_pLocalEP = value;
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060005E2 RID: 1506 RVA: 0x0002126C File Offset: 0x0002026C
		// (set) Token: 0x060005E3 RID: 1507 RVA: 0x00021284 File Offset: 0x00020284
		internal IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.m_pRemoteEP;
			}
			set
			{
				this.m_pRemoteEP = value;
			}
		}

		// Token: 0x04000231 RID: 561
		private SIP_RequestLine m_pRequestLine = null;

		// Token: 0x04000232 RID: 562
		private SIP_Flow m_pFlow = null;

		// Token: 0x04000233 RID: 563
		private IPEndPoint m_pLocalEP = null;

		// Token: 0x04000234 RID: 564
		private IPEndPoint m_pRemoteEP = null;
	}
}
