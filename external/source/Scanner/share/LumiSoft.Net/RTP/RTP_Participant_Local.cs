using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000C6 RID: 198
	public class RTP_Participant_Local : RTP_Participant
	{
		// Token: 0x06000787 RID: 1927 RVA: 0x0002DB14 File Offset: 0x0002CB14
		public RTP_Participant_Local(string cname) : base(cname)
		{
			this.m_pOtionalItemsRoundRobin = new CircleCollection<string>();
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x0002DB68 File Offset: 0x0002CB68
		internal void AddNextOptionalSdesItem(RTCP_Packet_SDES_Chunk sdes)
		{
			bool flag = sdes == null;
			if (flag)
			{
				throw new ArgumentNullException("sdes");
			}
			CircleCollection<string> pOtionalItemsRoundRobin = this.m_pOtionalItemsRoundRobin;
			lock (pOtionalItemsRoundRobin)
			{
				bool flag3 = this.m_pOtionalItemsRoundRobin.Count > 0;
				if (flag3)
				{
					string a = this.m_pOtionalItemsRoundRobin.Next();
					bool flag4 = a == "name";
					if (flag4)
					{
						sdes.Name = this.m_Name;
					}
					else
					{
						bool flag5 = a == "email";
						if (flag5)
						{
							sdes.Email = this.m_Email;
						}
						else
						{
							bool flag6 = a == "phone";
							if (flag6)
							{
								sdes.Phone = this.m_Phone;
							}
							else
							{
								bool flag7 = a == "location";
								if (flag7)
								{
									sdes.Location = this.m_Location;
								}
								else
								{
									bool flag8 = a == "tool";
									if (flag8)
									{
										sdes.Tool = this.m_Tool;
									}
									else
									{
										bool flag9 = a == "note";
										if (flag9)
										{
											sdes.Note = this.m_Note;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x0002DCC4 File Offset: 0x0002CCC4
		private void ConstructOptionalItems()
		{
			CircleCollection<string> pOtionalItemsRoundRobin = this.m_pOtionalItemsRoundRobin;
			lock (pOtionalItemsRoundRobin)
			{
				this.m_pOtionalItemsRoundRobin.Clear();
				bool flag2 = !string.IsNullOrEmpty(this.m_Note);
				if (flag2)
				{
					this.m_pOtionalItemsRoundRobin.Add("note");
				}
				bool flag3 = !string.IsNullOrEmpty(this.m_Name);
				if (flag3)
				{
					this.m_pOtionalItemsRoundRobin.Add("name");
				}
				bool flag4 = !string.IsNullOrEmpty(this.m_Email);
				if (flag4)
				{
					this.m_pOtionalItemsRoundRobin.Add("email");
				}
				bool flag5 = !string.IsNullOrEmpty(this.m_Phone);
				if (flag5)
				{
					this.m_pOtionalItemsRoundRobin.Add("phone");
				}
				bool flag6 = !string.IsNullOrEmpty(this.m_Location);
				if (flag6)
				{
					this.m_pOtionalItemsRoundRobin.Add("location");
				}
				bool flag7 = !string.IsNullOrEmpty(this.m_Tool);
				if (flag7)
				{
					this.m_pOtionalItemsRoundRobin.Add("tool");
				}
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x0600078A RID: 1930 RVA: 0x0002DDF8 File Offset: 0x0002CDF8
		// (set) Token: 0x0600078B RID: 1931 RVA: 0x0002DE10 File Offset: 0x0002CE10
		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
				this.ConstructOptionalItems();
			}
		}

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x0600078C RID: 1932 RVA: 0x0002DE24 File Offset: 0x0002CE24
		// (set) Token: 0x0600078D RID: 1933 RVA: 0x0002DE3C File Offset: 0x0002CE3C
		public string Email
		{
			get
			{
				return this.m_Email;
			}
			set
			{
				this.m_Email = value;
				this.ConstructOptionalItems();
			}
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x0600078E RID: 1934 RVA: 0x0002DE50 File Offset: 0x0002CE50
		// (set) Token: 0x0600078F RID: 1935 RVA: 0x0002DE68 File Offset: 0x0002CE68
		public string Phone
		{
			get
			{
				return this.m_Phone;
			}
			set
			{
				this.m_Phone = value;
				this.ConstructOptionalItems();
			}
		}

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000790 RID: 1936 RVA: 0x0002DE7C File Offset: 0x0002CE7C
		// (set) Token: 0x06000791 RID: 1937 RVA: 0x0002DE94 File Offset: 0x0002CE94
		public string Location
		{
			get
			{
				return this.m_Location;
			}
			set
			{
				this.m_Location = value;
				this.ConstructOptionalItems();
			}
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000792 RID: 1938 RVA: 0x0002DEA8 File Offset: 0x0002CEA8
		// (set) Token: 0x06000793 RID: 1939 RVA: 0x0002DEC0 File Offset: 0x0002CEC0
		public string Tool
		{
			get
			{
				return this.m_Tool;
			}
			set
			{
				this.m_Tool = value;
				this.ConstructOptionalItems();
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000794 RID: 1940 RVA: 0x0002DED4 File Offset: 0x0002CED4
		// (set) Token: 0x06000795 RID: 1941 RVA: 0x0002DEEC File Offset: 0x0002CEEC
		public string Note
		{
			get
			{
				return this.m_Note;
			}
			set
			{
				this.m_Note = value;
				this.ConstructOptionalItems();
			}
		}

		// Token: 0x04000343 RID: 835
		private string m_Name = null;

		// Token: 0x04000344 RID: 836
		private string m_Email = null;

		// Token: 0x04000345 RID: 837
		private string m_Phone = null;

		// Token: 0x04000346 RID: 838
		private string m_Location = null;

		// Token: 0x04000347 RID: 839
		private string m_Tool = null;

		// Token: 0x04000348 RID: 840
		private string m_Note = null;

		// Token: 0x04000349 RID: 841
		private CircleCollection<string> m_pOtionalItemsRoundRobin = null;
	}
}
