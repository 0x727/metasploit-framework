using System;
using System.Collections.Generic;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x0200013F RID: 319
	public class SMTP_Server : TCP_Server<SMTP_Session>
	{
		// Token: 0x06000C5C RID: 3164 RVA: 0x0004B71C File Offset: 0x0004A71C
		public SMTP_Server()
		{
			this.m_pServiceExtentions = new List<string>();
			this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.PIPELINING);
			this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.SIZE);
			this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.STARTTLS);
			this.m_pServiceExtentions.Add(SMTP_ServiceExtensions._8BITMIME);
			this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.BINARYMIME);
			this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.CHUNKING);
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x0004B7D7 File Offset: 0x0004A7D7
		protected override void OnMaxConnectionsExceeded(SMTP_Session session)
		{
			session.TcpStream.WriteLine("421 Client host rejected: too many connections, please try again later.");
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x0004B7EB File Offset: 0x0004A7EB
		protected override void OnMaxConnectionsPerIPExceeded(SMTP_Session session)
		{
			session.TcpStream.WriteLine("421 Client host rejected: too many connections from your IP(" + session.RemoteEndPoint.Address + "), please try again later.");
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06000C5F RID: 3167 RVA: 0x0004B814 File Offset: 0x0004A814
		// (set) Token: 0x06000C60 RID: 3168 RVA: 0x0004B850 File Offset: 0x0004A850
		public string[] ServiceExtentions
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pServiceExtentions.ToArray();
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("ServiceExtentions");
				}
				this.m_pServiceExtentions.Clear();
				for (int i = 0; i < value.Length; i++)
				{
					string text = value[i];
					bool flag2 = text.ToUpper() == SMTP_ServiceExtensions.PIPELINING;
					if (flag2)
					{
						this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.PIPELINING);
					}
					else
					{
						bool flag3 = text.ToUpper() == SMTP_ServiceExtensions.SIZE;
						if (flag3)
						{
							this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.SIZE);
						}
						else
						{
							bool flag4 = text.ToUpper() == SMTP_ServiceExtensions.STARTTLS;
							if (flag4)
							{
								this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.STARTTLS);
							}
							else
							{
								bool flag5 = text.ToUpper() == SMTP_ServiceExtensions._8BITMIME;
								if (flag5)
								{
									this.m_pServiceExtentions.Add(SMTP_ServiceExtensions._8BITMIME);
								}
								else
								{
									bool flag6 = text.ToUpper() == SMTP_ServiceExtensions.BINARYMIME;
									if (flag6)
									{
										this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.BINARYMIME);
									}
									else
									{
										bool flag7 = text.ToUpper() == SMTP_ServiceExtensions.CHUNKING;
										if (flag7)
										{
											this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.CHUNKING);
										}
										else
										{
											bool flag8 = text.ToUpper() == SMTP_ServiceExtensions.DSN;
											if (flag8)
											{
												this.m_pServiceExtentions.Add(SMTP_ServiceExtensions.DSN);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06000C61 RID: 3169 RVA: 0x0004B9D0 File Offset: 0x0004A9D0
		// (set) Token: 0x06000C62 RID: 3170 RVA: 0x0004BA04 File Offset: 0x0004AA04
		public string GreetingText
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_GreetingText;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_GreetingText = value;
			}
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06000C63 RID: 3171 RVA: 0x0004BA38 File Offset: 0x0004AA38
		// (set) Token: 0x06000C64 RID: 3172 RVA: 0x0004BA6C File Offset: 0x0004AA6C
		public int MaxBadCommands
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaxBadCommands;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value < 0;
				if (flag)
				{
					throw new ArgumentException("Property 'MaxBadCommands' value must be >= 0.");
				}
				this.m_MaxBadCommands = value;
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06000C65 RID: 3173 RVA: 0x0004BAB4 File Offset: 0x0004AAB4
		// (set) Token: 0x06000C66 RID: 3174 RVA: 0x0004BAE8 File Offset: 0x0004AAE8
		public int MaxTransactions
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaxTransactions;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value < 0;
				if (flag)
				{
					throw new ArgumentException("Property 'MaxTransactions' value must be >= 0.");
				}
				this.m_MaxTransactions = value;
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06000C67 RID: 3175 RVA: 0x0004BB30 File Offset: 0x0004AB30
		// (set) Token: 0x06000C68 RID: 3176 RVA: 0x0004BB64 File Offset: 0x0004AB64
		public int MaxMessageSize
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaxMessageSize;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value < 500;
				if (flag)
				{
					throw new ArgumentException("Property 'MaxMessageSize' value must be >= 500.");
				}
				this.m_MaxMessageSize = value;
			}
		}

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06000C69 RID: 3177 RVA: 0x0004BBB0 File Offset: 0x0004ABB0
		// (set) Token: 0x06000C6A RID: 3178 RVA: 0x0004BBE4 File Offset: 0x0004ABE4
		public int MaxRecipients
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaxRecipients;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentException("Property 'MaxRecipients' value must be >= 1.");
				}
				this.m_MaxRecipients = value;
			}
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06000C6B RID: 3179 RVA: 0x0004BC2C File Offset: 0x0004AC2C
		internal List<string> Extentions
		{
			get
			{
				return this.m_pServiceExtentions;
			}
		}

		// Token: 0x04000543 RID: 1347
		private List<string> m_pServiceExtentions = null;

		// Token: 0x04000544 RID: 1348
		private string m_GreetingText = "";

		// Token: 0x04000545 RID: 1349
		private int m_MaxBadCommands = 30;

		// Token: 0x04000546 RID: 1350
		private int m_MaxTransactions = 10;

		// Token: 0x04000547 RID: 1351
		private int m_MaxMessageSize = 10000000;

		// Token: 0x04000548 RID: 1352
		private int m_MaxRecipients = 100;
	}
}
