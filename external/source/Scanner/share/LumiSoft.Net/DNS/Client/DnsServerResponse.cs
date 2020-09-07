using System;
using System.Collections.Generic;

namespace LumiSoft.Net.DNS.Client
{
	// Token: 0x0200025E RID: 606
	[Serializable]
	public class DnsServerResponse
	{
		// Token: 0x0600159F RID: 5535 RVA: 0x000860E8 File Offset: 0x000850E8
		internal DnsServerResponse(bool connectionOk, int id, DNS_RCode rcode, List<DNS_rr> answers, List<DNS_rr> authoritiveAnswers, List<DNS_rr> additionalAnswers)
		{
			this.m_Success = connectionOk;
			this.m_ID = id;
			this.m_RCODE = rcode;
			this.m_pAnswers = answers;
			this.m_pAuthoritiveAnswers = authoritiveAnswers;
			this.m_pAdditionalAnswers = additionalAnswers;
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x00086154 File Offset: 0x00085154
		public DNS_rr_A[] GetARecords()
		{
			List<DNS_rr_A> list = new List<DNS_rr_A>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.A;
				if (flag)
				{
					list.Add((DNS_rr_A)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015A1 RID: 5537 RVA: 0x000861D4 File Offset: 0x000851D4
		public DNS_rr_NS[] GetNSRecords()
		{
			List<DNS_rr_NS> list = new List<DNS_rr_NS>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.NS;
				if (flag)
				{
					list.Add((DNS_rr_NS)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x00086254 File Offset: 0x00085254
		public DNS_rr_CNAME[] GetCNAMERecords()
		{
			List<DNS_rr_CNAME> list = new List<DNS_rr_CNAME>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.CNAME;
				if (flag)
				{
					list.Add((DNS_rr_CNAME)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015A3 RID: 5539 RVA: 0x000862D4 File Offset: 0x000852D4
		public DNS_rr_SOA[] GetSOARecords()
		{
			List<DNS_rr_SOA> list = new List<DNS_rr_SOA>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.SOA;
				if (flag)
				{
					list.Add((DNS_rr_SOA)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x00086354 File Offset: 0x00085354
		public DNS_rr_PTR[] GetPTRRecords()
		{
			List<DNS_rr_PTR> list = new List<DNS_rr_PTR>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.PTR;
				if (flag)
				{
					list.Add((DNS_rr_PTR)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x000863D4 File Offset: 0x000853D4
		public DNS_rr_HINFO[] GetHINFORecords()
		{
			List<DNS_rr_HINFO> list = new List<DNS_rr_HINFO>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.HINFO;
				if (flag)
				{
					list.Add((DNS_rr_HINFO)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015A6 RID: 5542 RVA: 0x00086454 File Offset: 0x00085454
		public DNS_rr_MX[] GetMXRecords()
		{
			List<DNS_rr_MX> list = new List<DNS_rr_MX>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.MX;
				if (flag)
				{
					list.Add((DNS_rr_MX)dns_rr);
				}
			}
			DNS_rr_MX[] array = list.ToArray();
			Array.Sort<DNS_rr_MX>(array);
			return array;
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x000864E0 File Offset: 0x000854E0
		public DNS_rr_TXT[] GetTXTRecords()
		{
			List<DNS_rr_TXT> list = new List<DNS_rr_TXT>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.TXT;
				if (flag)
				{
					list.Add((DNS_rr_TXT)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x00086560 File Offset: 0x00085560
		public DNS_rr_AAAA[] GetAAAARecords()
		{
			List<DNS_rr_AAAA> list = new List<DNS_rr_AAAA>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.AAAA;
				if (flag)
				{
					list.Add((DNS_rr_AAAA)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x000865E0 File Offset: 0x000855E0
		public DNS_rr_SRV[] GetSRVRecords()
		{
			List<DNS_rr_SRV> list = new List<DNS_rr_SRV>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.SRV;
				if (flag)
				{
					list.Add((DNS_rr_SRV)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x00086660 File Offset: 0x00085660
		public DNS_rr_NAPTR[] GetNAPTRRecords()
		{
			List<DNS_rr_NAPTR> list = new List<DNS_rr_NAPTR>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.NAPTR;
				if (flag)
				{
					list.Add((DNS_rr_NAPTR)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x000866E0 File Offset: 0x000856E0
		public DNS_rr_SPF[] GetSPFRecords()
		{
			List<DNS_rr_SPF> list = new List<DNS_rr_SPF>();
			foreach (DNS_rr dns_rr in this.m_pAnswers)
			{
				bool flag = dns_rr.RecordType == DNS_QType.SPF;
				if (flag)
				{
					list.Add((DNS_rr_SPF)dns_rr);
				}
			}
			return list.ToArray();
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x00086760 File Offset: 0x00085760
		private List<DNS_rr> FilterRecordsX(List<DNS_rr> answers, DNS_QType type)
		{
			List<DNS_rr> list = new List<DNS_rr>();
			foreach (DNS_rr dns_rr in answers)
			{
				bool flag = dns_rr.RecordType == type;
				if (flag)
				{
					list.Add(dns_rr);
				}
			}
			return list;
		}

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x060015AD RID: 5549 RVA: 0x000867D0 File Offset: 0x000857D0
		public bool ConnectionOk
		{
			get
			{
				return this.m_Success;
			}
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x060015AE RID: 5550 RVA: 0x000867E8 File Offset: 0x000857E8
		public int ID
		{
			get
			{
				return this.m_ID;
			}
		}

		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x060015AF RID: 5551 RVA: 0x00086800 File Offset: 0x00085800
		public DNS_RCode ResponseCode
		{
			get
			{
				return this.m_RCODE;
			}
		}

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x060015B0 RID: 5552 RVA: 0x00086818 File Offset: 0x00085818
		public DNS_rr[] AllAnswers
		{
			get
			{
				List<DNS_rr> list = new List<DNS_rr>();
				list.AddRange(this.m_pAnswers.ToArray());
				list.AddRange(this.m_pAuthoritiveAnswers.ToArray());
				list.AddRange(this.m_pAdditionalAnswers.ToArray());
				return list.ToArray();
			}
		}

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x060015B1 RID: 5553 RVA: 0x0008686C File Offset: 0x0008586C
		public DNS_rr[] Answers
		{
			get
			{
				return this.m_pAnswers.ToArray();
			}
		}

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x060015B2 RID: 5554 RVA: 0x0008688C File Offset: 0x0008588C
		public DNS_rr[] AuthoritiveAnswers
		{
			get
			{
				return this.m_pAuthoritiveAnswers.ToArray();
			}
		}

		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x060015B3 RID: 5555 RVA: 0x000868AC File Offset: 0x000858AC
		public DNS_rr[] AdditionalAnswers
		{
			get
			{
				return this.m_pAdditionalAnswers.ToArray();
			}
		}

		// Token: 0x0400089A RID: 2202
		private bool m_Success = true;

		// Token: 0x0400089B RID: 2203
		private int m_ID = 0;

		// Token: 0x0400089C RID: 2204
		private DNS_RCode m_RCODE = DNS_RCode.NO_ERROR;

		// Token: 0x0400089D RID: 2205
		private List<DNS_rr> m_pAnswers = null;

		// Token: 0x0400089E RID: 2206
		private List<DNS_rr> m_pAuthoritiveAnswers = null;

		// Token: 0x0400089F RID: 2207
		private List<DNS_rr> m_pAdditionalAnswers = null;
	}
}
