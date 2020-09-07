using System;
using System.IO;
using SharpCifs.Smb;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Util.Transport
{
	// Token: 0x02000011 RID: 17
	public abstract class Transport : IRunnable
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600008E RID: 142 RVA: 0x000059F0 File Offset: 0x00003BF0
		public LogStream Log
		{
			get
			{
				return LogStream.GetInstance();
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005A08 File Offset: 0x00003C08
		public static int Readn(InputStream @in, byte[] b, int off, int len)
		{
			int i;
			int num;
			for (i = 0; i < len; i += num)
			{
				num = @in.Read(b, off + i, len - i);
				bool flag = num <= 0;
				if (flag)
				{
					break;
				}
			}
			return i;
		}

		// Token: 0x06000090 RID: 144
		protected internal abstract void MakeKey(ServerMessageBlock request);

		// Token: 0x06000091 RID: 145
		protected internal abstract ServerMessageBlock PeekKey();

		// Token: 0x06000092 RID: 146
		protected internal abstract void DoSend(ServerMessageBlock request);

		// Token: 0x06000093 RID: 147
		protected internal abstract void DoRecv(Response response);

		// Token: 0x06000094 RID: 148
		protected internal abstract void DoSkip();

		// Token: 0x06000095 RID: 149 RVA: 0x00005A50 File Offset: 0x00003C50
		public virtual void Sendrecv(ServerMessageBlock request, Response response, long timeout)
		{
			lock (this)
			{
				this.MakeKey(request);
				response.IsReceived = false;
				try
				{
					this.ResponseMap.Put(request, response);
					this.DoSend(request);
					response.Expiration = Runtime.CurrentTimeMillis() + timeout;
					while (!response.IsReceived)
					{
						Runtime.Wait(this, timeout);
						timeout = response.Expiration - Runtime.CurrentTimeMillis();
						bool flag2 = timeout <= 0L;
						if (flag2)
						{
							throw new TransportException(this.Name + " timedout waiting for response to " + request);
						}
					}
				}
				catch (IOException ex)
				{
					bool flag3 = this.Log.Level > 2;
					if (flag3)
					{
						Runtime.PrintStackTrace(ex, this.Log);
					}
					try
					{
						this.Disconnect(true);
					}
					catch (IOException ex2)
					{
						Runtime.PrintStackTrace(ex2, this.Log);
					}
					throw;
				}
				catch (Exception rootCause)
				{
					throw new TransportException(rootCause);
				}
				finally
				{
					this.ResponseMap.Remove(request);
				}
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005BA4 File Offset: 0x00003DA4
		private void Loop()
		{
			while (this.Thread == Thread.CurrentThread())
			{
				try
				{
					ServerMessageBlock serverMessageBlock = this.PeekKey();
					bool flag = serverMessageBlock == null;
					if (flag)
					{
						throw new IOException("end of stream");
					}
					lock (this)
					{
						Response response = (Response)this.ResponseMap.Get(serverMessageBlock);
						bool flag3 = response == null;
						if (flag3)
						{
							bool flag4 = this.Log.Level >= 4;
							if (flag4)
							{
								this.Log.WriteLine("Invalid key, skipping message");
							}
							this.DoSkip();
						}
						else
						{
							this.DoRecv(response);
							response.IsReceived = true;
							Runtime.NotifyAll(this);
						}
					}
				}
				catch (Exception ex)
				{
					string message = ex.Message;
					bool flag5 = message != null && message.Equals("Read timed out");
					bool hard = !flag5;
					bool flag6 = !flag5 && this.Log.Level >= 3;
					if (flag6)
					{
						Runtime.PrintStackTrace(ex, this.Log);
					}
					try
					{
						this.Disconnect(hard);
					}
					catch (IOException ex2)
					{
						Runtime.PrintStackTrace(ex2, this.Log);
					}
				}
			}
		}

		// Token: 0x06000097 RID: 151
		protected internal abstract void DoConnect();

		// Token: 0x06000098 RID: 152
		protected internal abstract void DoDisconnect(bool hard);

		// Token: 0x06000099 RID: 153 RVA: 0x00005D1C File Offset: 0x00003F1C
		public virtual void Connect(long timeout)
		{
			lock (this)
			{
				try
				{
					try
					{
						switch (this.State)
						{
						case 0:
						{
							this.State = 1;
							this.Te = null;
							this.Thread = new Thread(this);
							this.Thread.SetDaemon(true);
							Thread thread = this.Thread;
							lock (thread)
							{
								this.Thread.Start();
								Runtime.Wait(this.Thread, timeout);
								int state = this.State;
								if (state == 1)
								{
									this.State = 0;
									this.Thread = null;
									throw new TransportException("Connection timeout");
								}
								if (state == 2)
								{
									bool flag3 = this.Te != null;
									if (flag3)
									{
										this.State = 4;
										this.Thread = null;
										throw this.Te;
									}
									this.State = 3;
								}
							}
							goto IL_159;
						}
						case 3:
							return;
						case 4:
							this.State = 0;
							throw new TransportException("Connection in error", this.Te);
						}
						this.State = 0;
						throw new TransportException("Invalid state: " + this.State);
					}
					catch (Exception rootCause)
					{
						this.State = 0;
						this.Thread = null;
						throw new TransportException(rootCause);
					}
					IL_159:;
				}
				finally
				{
					bool flag4 = this.State != 0 && this.State != 3 && this.State != 4;
					if (flag4)
					{
						bool flag5 = this.Log.Level >= 1;
						if (flag5)
						{
							this.Log.WriteLine("Invalid state: " + this.State);
						}
						this.State = 0;
						this.Thread = null;
					}
				}
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005F6C File Offset: 0x0000416C
		public virtual void Disconnect(bool hard)
		{
			bool flag = hard;
			if (flag)
			{
				IOException ex = null;
				switch (this.State)
				{
				case 0:
					return;
				case 1:
					goto IL_80;
				case 2:
					hard = true;
					break;
				case 3:
					break;
				case 4:
					goto IL_6F;
				default:
					goto IL_80;
				}
				bool flag2 = this.ResponseMap.Count != 0 && !hard;
				if (flag2)
				{
					goto IL_CB;
				}
				try
				{
					this.DoDisconnect(hard);
				}
				catch (IOException ex2)
				{
					ex = ex2;
				}
				IL_6F:
				this.Thread = null;
				this.State = 0;
				goto IL_CB;
				IL_80:
				bool flag3 = this.Log.Level >= 1;
				if (flag3)
				{
					this.Log.WriteLine("Invalid state: " + this.State);
				}
				this.Thread = null;
				this.State = 0;
				IL_CB:
				bool flag4 = ex != null;
				if (flag4)
				{
					throw ex;
				}
			}
			else
			{
				lock (this)
				{
					IOException ex3 = null;
					switch (this.State)
					{
					case 0:
						return;
					case 1:
						goto IL_16A;
					case 2:
						hard = true;
						break;
					case 3:
						break;
					case 4:
						goto IL_159;
					default:
						goto IL_16A;
					}
					bool flag6 = this.ResponseMap.Count != 0 && !hard;
					if (flag6)
					{
						goto IL_1B5;
					}
					try
					{
						this.DoDisconnect(hard);
					}
					catch (IOException ex4)
					{
						ex3 = ex4;
					}
					IL_159:
					this.Thread = null;
					this.State = 0;
					goto IL_1B5;
					IL_16A:
					bool flag7 = this.Log.Level >= 1;
					if (flag7)
					{
						this.Log.WriteLine("Invalid state: " + this.State);
					}
					this.Thread = null;
					this.State = 0;
					IL_1B5:
					bool flag8 = ex3 != null;
					if (flag8)
					{
						throw ex3;
					}
				}
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006178 File Offset: 0x00004378
		public virtual void Run()
		{
			Thread thread = Thread.CurrentThread();
			Exception ex = null;
			try
			{
				this.DoConnect();
			}
			catch (Exception ex2)
			{
				ex = ex2;
				return;
			}
			finally
			{
				Thread obj = thread;
				lock (obj)
				{
					bool flag2 = thread != this.Thread;
					if (flag2)
					{
						bool flag3 = ex != null;
						if (flag3)
						{
							bool flag4 = this.Log.Level >= 2;
							if (flag4)
							{
								Runtime.PrintStackTrace(ex, this.Log);
							}
						}
					}
					bool flag5 = ex != null;
					if (flag5)
					{
						this.Te = new TransportException(ex);
					}
					this.State = 2;
					Runtime.Notify(thread);
				}
			}
			this.Loop();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006264 File Offset: 0x00004464
		public override string ToString()
		{
			return this.Name;
		}

		// Token: 0x04000045 RID: 69
		internal static int Id;

		// Token: 0x04000046 RID: 70
		internal int State;

		// Token: 0x04000047 RID: 71
		internal string Name = "Transport" + Transport.Id++;

		// Token: 0x04000048 RID: 72
		internal Thread Thread;

		// Token: 0x04000049 RID: 73
		internal TransportException Te;

		// Token: 0x0400004A RID: 74
		protected internal Hashtable ResponseMap = new Hashtable();
	}
}
