using System;
using System.Diagnostics;
using System.Text;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000C9 RID: 201
	public class RTP_Participant_Remote : RTP_Participant
	{
		// Token: 0x060007AD RID: 1965 RVA: 0x0002E5A8 File Offset: 0x0002D5A8
		internal RTP_Participant_Remote(string cname) : base(cname)
		{
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x0002E5E4 File Offset: 0x0002D5E4
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("CNAME: " + base.CNAME);
			bool flag = !string.IsNullOrEmpty(this.m_Name);
			if (flag)
			{
				stringBuilder.AppendLine("Name: " + this.m_Name);
			}
			bool flag2 = !string.IsNullOrEmpty(this.m_Email);
			if (flag2)
			{
				stringBuilder.AppendLine("Email: " + this.m_Email);
			}
			bool flag3 = !string.IsNullOrEmpty(this.m_Phone);
			if (flag3)
			{
				stringBuilder.AppendLine("Phone: " + this.m_Phone);
			}
			bool flag4 = !string.IsNullOrEmpty(this.m_Location);
			if (flag4)
			{
				stringBuilder.AppendLine("Location: " + this.m_Location);
			}
			bool flag5 = !string.IsNullOrEmpty(this.m_Tool);
			if (flag5)
			{
				stringBuilder.AppendLine("Tool: " + this.m_Tool);
			}
			bool flag6 = !string.IsNullOrEmpty(this.m_Note);
			if (flag6)
			{
				stringBuilder.AppendLine("Note: " + this.m_Note);
			}
			return stringBuilder.ToString().TrimEnd(new char[0]);
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x0002E730 File Offset: 0x0002D730
		internal void Update(RTCP_Packet_SDES_Chunk sdes)
		{
			bool flag = sdes == null;
			if (flag)
			{
				throw new ArgumentNullException("sdes");
			}
			bool flag2 = false;
			bool flag3 = !string.IsNullOrEmpty(sdes.Name) && !string.Equals(this.m_Name, sdes.Name);
			if (flag3)
			{
				this.m_Name = sdes.Name;
				flag2 = true;
			}
			bool flag4 = !string.IsNullOrEmpty(sdes.Email) && !string.Equals(this.m_Email, sdes.Email);
			if (flag4)
			{
				this.m_Email = sdes.Email;
				flag2 = true;
			}
			bool flag5 = !string.IsNullOrEmpty(sdes.Phone) && !string.Equals(this.Phone, sdes.Phone);
			if (flag5)
			{
				this.m_Phone = sdes.Phone;
				flag2 = true;
			}
			bool flag6 = !string.IsNullOrEmpty(sdes.Location) && !string.Equals(this.m_Location, sdes.Location);
			if (flag6)
			{
				this.m_Location = sdes.Location;
				flag2 = true;
			}
			bool flag7 = !string.IsNullOrEmpty(sdes.Tool) && !string.Equals(this.m_Tool, sdes.Tool);
			if (flag7)
			{
				this.m_Tool = sdes.Tool;
				flag2 = true;
			}
			bool flag8 = !string.IsNullOrEmpty(sdes.Note) && !string.Equals(this.m_Note, sdes.Note);
			if (flag8)
			{
				this.m_Note = sdes.Note;
				flag2 = true;
			}
			bool flag9 = flag2;
			if (flag9)
			{
				this.OnChanged();
			}
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x060007B0 RID: 1968 RVA: 0x0002E8BC File Offset: 0x0002D8BC
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x060007B1 RID: 1969 RVA: 0x0002E8D4 File Offset: 0x0002D8D4
		public string Email
		{
			get
			{
				return this.m_Email;
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x060007B2 RID: 1970 RVA: 0x0002E8EC File Offset: 0x0002D8EC
		public string Phone
		{
			get
			{
				return this.m_Phone;
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x0002E904 File Offset: 0x0002D904
		public string Location
		{
			get
			{
				return this.m_Location;
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x060007B4 RID: 1972 RVA: 0x0002E91C File Offset: 0x0002D91C
		public string Tool
		{
			get
			{
				return this.m_Tool;
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x060007B5 RID: 1973 RVA: 0x0002E934 File Offset: 0x0002D934
		public string Note
		{
			get
			{
				return this.m_Note;
			}
		}

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x060007B6 RID: 1974 RVA: 0x0002E94C File Offset: 0x0002D94C
		// (remove) Token: 0x060007B7 RID: 1975 RVA: 0x0002E984 File Offset: 0x0002D984
		
		public event EventHandler<RTP_ParticipantEventArgs> Changed = null;

		// Token: 0x060007B8 RID: 1976 RVA: 0x0002E9BC File Offset: 0x0002D9BC
		private void OnChanged()
		{
			bool flag = this.Changed != null;
			if (flag)
			{
				this.Changed(this, new RTP_ParticipantEventArgs(this));
			}
		}

		// Token: 0x04000352 RID: 850
		private string m_Name = null;

		// Token: 0x04000353 RID: 851
		private string m_Email = null;

		// Token: 0x04000354 RID: 852
		private string m_Phone = null;

		// Token: 0x04000355 RID: 853
		private string m_Location = null;

		// Token: 0x04000356 RID: 854
		private string m_Tool = null;

		// Token: 0x04000357 RID: 855
		private string m_Note = null;
	}
}
