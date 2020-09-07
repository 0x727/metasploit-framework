using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LumiSoft.Net.SDP
{
	// Token: 0x020000BE RID: 190
	public class SDP_Message
	{
		// Token: 0x0600074C RID: 1868 RVA: 0x0002CAFC File Offset: 0x0002BAFC
		public SDP_Message()
		{
			this.m_pTimes = new List<SDP_Time>();
			this.m_pAttributes = new List<SDP_Attribute>();
			this.m_pMediaDescriptions = new List<SDP_MediaDescription>();
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x0002CB8C File Offset: 0x0002BB8C
		public static SDP_Message Parse(string data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			SDP_Message sdp_Message = new SDP_Message();
			StringReader stringReader = new StringReader(data);
			string text = stringReader.ReadLine();
			while (text != null)
			{
				text = text.Trim();
				bool flag2 = text.ToLower().StartsWith("m");
				if (flag2)
				{
					SDP_MediaDescription sdp_MediaDescription = SDP_MediaDescription.Parse(text);
					sdp_Message.m_pMediaDescriptions.Add(sdp_MediaDescription);
					for (text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
					{
						text = text.Trim();
						bool flag3 = text.ToLower().StartsWith("m");
						if (flag3)
						{
							break;
						}
						bool flag4 = text.ToLower().StartsWith("i");
						if (flag4)
						{
							sdp_MediaDescription.Information = text.Split(new char[]
							{
								'='
							}, 2)[1].Trim();
						}
						else
						{
							bool flag5 = text.ToLower().StartsWith("c");
							if (flag5)
							{
								sdp_MediaDescription.Connection = SDP_Connection.Parse(text);
							}
							else
							{
								bool flag6 = text.ToLower().StartsWith("a");
								if (flag6)
								{
									sdp_MediaDescription.Attributes.Add(SDP_Attribute.Parse(text));
								}
							}
						}
					}
					bool flag7 = text == null;
					if (flag7)
					{
						break;
					}
				}
				else
				{
					bool flag8 = text.ToLower().StartsWith("v");
					if (flag8)
					{
						sdp_Message.Version = text.Split(new char[]
						{
							'='
						}, 2)[1].Trim();
					}
					else
					{
						bool flag9 = text.ToLower().StartsWith("o");
						if (flag9)
						{
							sdp_Message.Origin = SDP_Origin.Parse(text);
						}
						else
						{
							bool flag10 = text.ToLower().StartsWith("s");
							if (flag10)
							{
								sdp_Message.SessionName = text.Split(new char[]
								{
									'='
								}, 2)[1].Trim();
							}
							else
							{
								bool flag11 = text.ToLower().StartsWith("i");
								if (flag11)
								{
									sdp_Message.SessionDescription = text.Split(new char[]
									{
										'='
									}, 2)[1].Trim();
								}
								else
								{
									bool flag12 = text.ToLower().StartsWith("u");
									if (flag12)
									{
										sdp_Message.Uri = text.Split(new char[]
										{
											'='
										}, 2)[1].Trim();
									}
									else
									{
										bool flag13 = text.ToLower().StartsWith("c");
										if (flag13)
										{
											sdp_Message.Connection = SDP_Connection.Parse(text);
										}
										else
										{
											bool flag14 = text.ToLower().StartsWith("t");
											if (flag14)
											{
												sdp_Message.Times.Add(SDP_Time.Parse(text));
											}
											else
											{
												bool flag15 = text.ToLower().StartsWith("a");
												if (flag15)
												{
													sdp_Message.Attributes.Add(SDP_Attribute.Parse(text));
												}
											}
										}
									}
								}
							}
						}
					}
					text = stringReader.ReadLine().Trim();
				}
			}
			return sdp_Message;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0002CEA4 File Offset: 0x0002BEA4
		public SDP_Message Clone()
		{
			return (SDP_Message)base.MemberwiseClone();
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x0002CEC1 File Offset: 0x0002BEC1
		public void ToFile(string fileName)
		{
			File.WriteAllText(fileName, this.ToStringData());
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x0002CED4 File Offset: 0x0002BED4
		public string ToStringData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("v=" + this.Version);
			bool flag = this.Origin != null;
			if (flag)
			{
				stringBuilder.Append(this.Origin.ToString());
			}
			bool flag2 = !string.IsNullOrEmpty(this.SessionName);
			if (flag2)
			{
				stringBuilder.AppendLine("s=" + this.SessionName);
			}
			bool flag3 = !string.IsNullOrEmpty(this.SessionDescription);
			if (flag3)
			{
				stringBuilder.AppendLine("i=" + this.SessionDescription);
			}
			bool flag4 = !string.IsNullOrEmpty(this.Uri);
			if (flag4)
			{
				stringBuilder.AppendLine("u=" + this.Uri);
			}
			bool flag5 = this.Connection != null;
			if (flag5)
			{
				stringBuilder.Append(this.Connection.ToValue());
			}
			foreach (SDP_Time sdp_Time in this.Times)
			{
				stringBuilder.Append(sdp_Time.ToValue());
			}
			foreach (SDP_Attribute sdp_Attribute in this.Attributes)
			{
				stringBuilder.Append(sdp_Attribute.ToValue());
			}
			foreach (SDP_MediaDescription sdp_MediaDescription in this.MediaDescriptions)
			{
				stringBuilder.Append(sdp_MediaDescription.ToValue());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x0002D0C4 File Offset: 0x0002C0C4
		public byte[] ToByte()
		{
			return Encoding.UTF8.GetBytes(this.ToStringData());
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000752 RID: 1874 RVA: 0x0002D0E8 File Offset: 0x0002C0E8
		// (set) Token: 0x06000753 RID: 1875 RVA: 0x0002D100 File Offset: 0x0002C100
		public string Version
		{
			get
			{
				return this.m_Version;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Version can't be null or empty !");
				}
				this.m_Version = value;
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x0002D12C File Offset: 0x0002C12C
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x0002D144 File Offset: 0x0002C144
		public SDP_Origin Origin
		{
			get
			{
				return this.m_pOrigin;
			}
			set
			{
				this.m_pOrigin = value;
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000756 RID: 1878 RVA: 0x0002D150 File Offset: 0x0002C150
		// (set) Token: 0x06000757 RID: 1879 RVA: 0x0002D168 File Offset: 0x0002C168
		public string SessionName
		{
			get
			{
				return this.m_SessionName;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property SessionName can't be null or empty !");
				}
				this.m_SessionName = value;
			}
		}

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000758 RID: 1880 RVA: 0x0002D194 File Offset: 0x0002C194
		// (set) Token: 0x06000759 RID: 1881 RVA: 0x0002D1AC File Offset: 0x0002C1AC
		public string SessionDescription
		{
			get
			{
				return this.m_SessionDescription;
			}
			set
			{
				this.m_SessionDescription = value;
			}
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x0600075A RID: 1882 RVA: 0x0002D1B8 File Offset: 0x0002C1B8
		// (set) Token: 0x0600075B RID: 1883 RVA: 0x0002D1D0 File Offset: 0x0002C1D0
		public string Uri
		{
			get
			{
				return this.m_Uri;
			}
			set
			{
				this.m_Uri = value;
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x0600075C RID: 1884 RVA: 0x0002D1DC File Offset: 0x0002C1DC
		// (set) Token: 0x0600075D RID: 1885 RVA: 0x0002D1F4 File Offset: 0x0002C1F4
		public SDP_Connection Connection
		{
			get
			{
				return this.m_pConnectionData;
			}
			set
			{
				this.m_pConnectionData = value;
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x0002D200 File Offset: 0x0002C200
		public List<SDP_Time> Times
		{
			get
			{
				return this.m_pTimes;
			}
		}

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x0600075F RID: 1887 RVA: 0x0002D218 File Offset: 0x0002C218
		// (set) Token: 0x06000760 RID: 1888 RVA: 0x0002D230 File Offset: 0x0002C230
		public string RepeatTimes
		{
			get
			{
				return this.m_RepeatTimes;
			}
			set
			{
				this.m_RepeatTimes = value;
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000761 RID: 1889 RVA: 0x0002D23C File Offset: 0x0002C23C
		public List<SDP_Attribute> Attributes
		{
			get
			{
				return this.m_pAttributes;
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x0002D254 File Offset: 0x0002C254
		public List<SDP_MediaDescription> MediaDescriptions
		{
			get
			{
				return this.m_pMediaDescriptions;
			}
		}

		// Token: 0x0400031F RID: 799
		private string m_Version = "0";

		// Token: 0x04000320 RID: 800
		private SDP_Origin m_pOrigin = null;

		// Token: 0x04000321 RID: 801
		private string m_SessionName = "";

		// Token: 0x04000322 RID: 802
		private string m_SessionDescription = "";

		// Token: 0x04000323 RID: 803
		private string m_Uri = "";

		// Token: 0x04000324 RID: 804
		private SDP_Connection m_pConnectionData = null;

		// Token: 0x04000325 RID: 805
		private List<SDP_Time> m_pTimes = null;

		// Token: 0x04000326 RID: 806
		private string m_RepeatTimes = "";

		// Token: 0x04000327 RID: 807
		private List<SDP_Attribute> m_pAttributes = null;

		// Token: 0x04000328 RID: 808
		private List<SDP_MediaDescription> m_pMediaDescriptions = null;
	}
}
