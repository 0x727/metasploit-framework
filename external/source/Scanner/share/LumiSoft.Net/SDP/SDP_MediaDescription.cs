using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.SDP
{
	// Token: 0x020000BD RID: 189
	public class SDP_MediaDescription
	{
		// Token: 0x06000737 RID: 1847 RVA: 0x0002C364 File Offset: 0x0002B364
		public SDP_MediaDescription(string mediaType, int port, int ports, string protocol, string[] mediaFormats)
		{
			bool flag = mediaType == null;
			if (flag)
			{
				throw new ArgumentNullException("mediaType");
			}
			bool flag2 = mediaType == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'mediaType' value must be specified.");
			}
			bool flag3 = port < 0;
			if (flag3)
			{
				throw new ArgumentException("Argument 'port' value must be >= 0.");
			}
			bool flag4 = ports < 0;
			if (flag4)
			{
				throw new ArgumentException("Argument 'ports' value must be >= 0.");
			}
			bool flag5 = protocol == null;
			if (flag5)
			{
				throw new ArgumentNullException("protocol");
			}
			bool flag6 = protocol == string.Empty;
			if (flag6)
			{
				throw new ArgumentException("Argument 'protocol' value msut be specified.");
			}
			this.m_MediaType = mediaType;
			this.m_Port = port;
			this.m_NumberOfPorts = ports;
			this.m_Protocol = protocol;
			this.m_pMediaFormats = new List<string>();
			this.m_pAttributes = new List<SDP_Attribute>();
			this.m_pTags = new Dictionary<string, object>();
			bool flag7 = mediaFormats != null;
			if (flag7)
			{
				this.m_pMediaFormats.AddRange(mediaFormats);
			}
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x0002C4AC File Offset: 0x0002B4AC
		private SDP_MediaDescription()
		{
			this.m_pMediaFormats = new List<string>();
			this.m_pAttributes = new List<SDP_Attribute>();
			this.m_pTags = new Dictionary<string, object>();
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x0002C530 File Offset: 0x0002B530
		public static SDP_MediaDescription Parse(string mValue)
		{
			SDP_MediaDescription sdp_MediaDescription = new SDP_MediaDescription();
			StringReader stringReader = new StringReader(mValue);
			stringReader.QuotedReadToDelimiter('=');
			string text = stringReader.ReadWord();
			bool flag = text == null;
			if (flag)
			{
				throw new Exception("SDP message \"m\" field <media> value is missing !");
			}
			sdp_MediaDescription.m_MediaType = text;
			text = stringReader.ReadWord();
			bool flag2 = text == null;
			if (flag2)
			{
				throw new Exception("SDP message \"m\" field <port> value is missing !");
			}
			bool flag3 = text.IndexOf('/') > -1;
			if (flag3)
			{
				string[] array = text.Split(new char[]
				{
					'/'
				});
				sdp_MediaDescription.m_Port = Convert.ToInt32(array[0]);
				sdp_MediaDescription.m_NumberOfPorts = Convert.ToInt32(array[1]);
			}
			else
			{
				sdp_MediaDescription.m_Port = Convert.ToInt32(text);
				sdp_MediaDescription.m_NumberOfPorts = 1;
			}
			text = stringReader.ReadWord();
			bool flag4 = text == null;
			if (flag4)
			{
				throw new Exception("SDP message \"m\" field <proto> value is missing !");
			}
			sdp_MediaDescription.m_Protocol = text;
			for (text = stringReader.ReadWord(); text != null; text = stringReader.ReadWord())
			{
				sdp_MediaDescription.MediaFormats.Add(text);
			}
			return sdp_MediaDescription;
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x0002C644 File Offset: 0x0002B644
		public void SetStreamMode(string streamMode)
		{
			bool flag = streamMode == null;
			if (flag)
			{
				throw new ArgumentNullException("streamMode");
			}
			for (int i = 0; i < this.m_pAttributes.Count; i++)
			{
				SDP_Attribute sdp_Attribute = this.m_pAttributes[i];
				bool flag2 = string.Equals(sdp_Attribute.Name, "sendrecv", StringComparison.InvariantCultureIgnoreCase);
				if (flag2)
				{
					this.m_pAttributes.RemoveAt(i);
					i--;
				}
				else
				{
					bool flag3 = string.Equals(sdp_Attribute.Name, "sendonly", StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						this.m_pAttributes.RemoveAt(i);
						i--;
					}
					else
					{
						bool flag4 = string.Equals(sdp_Attribute.Name, "recvonly", StringComparison.InvariantCultureIgnoreCase);
						if (flag4)
						{
							this.m_pAttributes.RemoveAt(i);
							i--;
						}
						else
						{
							bool flag5 = string.Equals(sdp_Attribute.Name, "inactive", StringComparison.InvariantCultureIgnoreCase);
							if (flag5)
							{
								this.m_pAttributes.RemoveAt(i);
								i--;
							}
						}
					}
				}
			}
			bool flag6 = streamMode != "";
			if (flag6)
			{
				this.m_pAttributes.Add(new SDP_Attribute(streamMode, ""));
			}
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x0002C770 File Offset: 0x0002B770
		public string ToValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = this.NumberOfPorts > 1;
			if (flag)
			{
				stringBuilder.Append(string.Concat(new object[]
				{
					"m=",
					this.MediaType,
					" ",
					this.Port,
					"/",
					this.NumberOfPorts,
					" ",
					this.Protocol
				}));
			}
			else
			{
				stringBuilder.Append(string.Concat(new object[]
				{
					"m=",
					this.MediaType,
					" ",
					this.Port,
					" ",
					this.Protocol
				}));
			}
			foreach (string str in this.MediaFormats)
			{
				stringBuilder.Append(" " + str);
			}
			stringBuilder.Append("\r\n");
			bool flag2 = !string.IsNullOrEmpty(this.m_Information);
			if (flag2)
			{
				stringBuilder.Append("i=" + this.m_Information + "\r\n");
			}
			bool flag3 = !string.IsNullOrEmpty(this.m_Bandwidth);
			if (flag3)
			{
				stringBuilder.Append("b=" + this.m_Bandwidth + "\r\n");
			}
			bool flag4 = this.m_pConnection != null;
			if (flag4)
			{
				stringBuilder.Append(this.m_pConnection.ToValue());
			}
			foreach (SDP_Attribute sdp_Attribute in this.Attributes)
			{
				stringBuilder.Append(sdp_Attribute.ToValue());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x0600073C RID: 1852 RVA: 0x0002C984 File Offset: 0x0002B984
		public string MediaType
		{
			get
			{
				return this.m_MediaType;
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x0600073D RID: 1853 RVA: 0x0002C99C File Offset: 0x0002B99C
		// (set) Token: 0x0600073E RID: 1854 RVA: 0x0002C9B4 File Offset: 0x0002B9B4
		public int Port
		{
			get
			{
				return this.m_Port;
			}
			set
			{
				this.m_Port = value;
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x0600073F RID: 1855 RVA: 0x0002C9C0 File Offset: 0x0002B9C0
		// (set) Token: 0x06000740 RID: 1856 RVA: 0x0002C9D8 File Offset: 0x0002B9D8
		public int NumberOfPorts
		{
			get
			{
				return this.m_NumberOfPorts;
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentException("Property NumberOfPorts must be >= 1 !");
				}
				this.m_NumberOfPorts = value;
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x0002CA04 File Offset: 0x0002BA04
		// (set) Token: 0x06000742 RID: 1858 RVA: 0x0002CA1C File Offset: 0x0002BA1C
		public string Protocol
		{
			get
			{
				return this.m_Protocol;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Protocol cant be null or empty !");
				}
				this.m_Protocol = value;
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000743 RID: 1859 RVA: 0x0002CA48 File Offset: 0x0002BA48
		public List<string> MediaFormats
		{
			get
			{
				return this.m_pMediaFormats;
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x0002CA60 File Offset: 0x0002BA60
		// (set) Token: 0x06000745 RID: 1861 RVA: 0x0002CA78 File Offset: 0x0002BA78
		public string Information
		{
			get
			{
				return this.m_Information;
			}
			set
			{
				this.m_Information = value;
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0002CA84 File Offset: 0x0002BA84
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x0002CA9C File Offset: 0x0002BA9C
		public SDP_Connection Connection
		{
			get
			{
				return this.m_pConnection;
			}
			set
			{
				this.m_pConnection = value;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x0002CAA8 File Offset: 0x0002BAA8
		// (set) Token: 0x06000749 RID: 1865 RVA: 0x0002CAC0 File Offset: 0x0002BAC0
		public string Bandwidth
		{
			get
			{
				return this.m_Bandwidth;
			}
			set
			{
				this.m_Bandwidth = value;
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x0600074A RID: 1866 RVA: 0x0002CACC File Offset: 0x0002BACC
		public List<SDP_Attribute> Attributes
		{
			get
			{
				return this.m_pAttributes;
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x0600074B RID: 1867 RVA: 0x0002CAE4 File Offset: 0x0002BAE4
		public Dictionary<string, object> Tags
		{
			get
			{
				return this.m_pTags;
			}
		}

		// Token: 0x04000315 RID: 789
		private string m_MediaType = "";

		// Token: 0x04000316 RID: 790
		private int m_Port = 0;

		// Token: 0x04000317 RID: 791
		private int m_NumberOfPorts = 1;

		// Token: 0x04000318 RID: 792
		private string m_Protocol = "";

		// Token: 0x04000319 RID: 793
		private List<string> m_pMediaFormats = null;

		// Token: 0x0400031A RID: 794
		private string m_Information = null;

		// Token: 0x0400031B RID: 795
		private SDP_Connection m_pConnection = null;

		// Token: 0x0400031C RID: 796
		private string m_Bandwidth = null;

		// Token: 0x0400031D RID: 797
		private List<SDP_Attribute> m_pAttributes = null;

		// Token: 0x0400031E RID: 798
		private Dictionary<string, object> m_pTags = null;
	}
}
