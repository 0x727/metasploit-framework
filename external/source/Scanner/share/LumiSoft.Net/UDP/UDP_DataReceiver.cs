using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LumiSoft.Net.UDP
{
	// Token: 0x0200012E RID: 302
	public class UDP_DataReceiver : IDisposable
	{
		// Token: 0x06000BF7 RID: 3063 RVA: 0x00049770 File Offset: 0x00048770
		public UDP_DataReceiver(Socket socket)
		{
			bool flag = socket == null;
			if (flag)
			{
				throw new ArgumentNullException("socket");
			}
			this.m_pSocket = socket;
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x000497E4 File Offset: 0x000487E4
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.m_pSocket = null;
				this.m_pBuffer = null;
				bool flag = this.m_pSocketArgs != null;
				if (flag)
				{
					this.m_pSocketArgs.Dispose();
					this.m_pSocketArgs = null;
				}
				this.m_pEventArgs = null;
				this.PacketReceived = null;
				this.Error = null;
			}
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x0004984C File Offset: 0x0004884C
		public void Start()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isRunning = this.m_IsRunning;
			if (!isRunning)
			{
				this.m_IsRunning = true;
				bool isIoCompletionSupported = Net_Utils.IsSocketAsyncSupported();
				this.m_pEventArgs = new UDP_e_PacketReceived();
				this.m_pBuffer = new byte[this.m_BufferSize];
				bool isIoCompletionSupported3 = isIoCompletionSupported;
				if (isIoCompletionSupported3)
				{
					this.m_pSocketArgs = new SocketAsyncEventArgs();
					this.m_pSocketArgs.SetBuffer(this.m_pBuffer, 0, this.m_BufferSize);
					this.m_pSocketArgs.RemoteEndPoint = new IPEndPoint((this.m_pSocket.AddressFamily == AddressFamily.InterNetwork) ? IPAddress.Any : IPAddress.IPv6Any, 0);
					this.m_pSocketArgs.Completed += delegate(object s1, SocketAsyncEventArgs e1)
					{
						bool isDisposed2 = this.m_IsDisposed;
						if (!isDisposed2)
						{
							try
							{
								bool flag = this.m_pSocketArgs.SocketError == SocketError.Success;
								if (flag)
								{
									this.OnPacketReceived(this.m_pBuffer, this.m_pSocketArgs.BytesTransferred, (IPEndPoint)this.m_pSocketArgs.RemoteEndPoint);
								}
								else
								{
									this.OnError(new Exception("Socket error '" + this.m_pSocketArgs.SocketError + "'."));
								}
								this.IOCompletionReceive();
							}
							catch (Exception x)
							{
								this.OnError(x);
							}
						}
					};
				}
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					bool isDisposed2 = this.m_IsDisposed;
					if (!isDisposed2)
					{
						try
						{
							bool isIoCompletionSupported2 = isIoCompletionSupported;
							if (isIoCompletionSupported2)
							{
								this.IOCompletionReceive();
							}
							else
							{
								EndPoint endPoint = new IPEndPoint((this.m_pSocket.AddressFamily == AddressFamily.InterNetwork) ? IPAddress.Any : IPAddress.IPv6Any, 0);
								this.m_pSocket.BeginReceiveFrom(this.m_pBuffer, 0, this.m_BufferSize, SocketFlags.None, ref endPoint, new AsyncCallback(this.AsyncSocketReceive), null);
							}
						}
						catch (Exception x)
						{
							this.OnError(x);
						}
					}
				});
			}
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x00049948 File Offset: 0x00048948
		private void IOCompletionReceive()
		{
			try
			{
				while (!this.m_IsDisposed && !this.m_pSocket.ReceiveFromAsync(this.m_pSocketArgs))
				{
					bool flag = this.m_pSocketArgs.SocketError == SocketError.Success;
					if (flag)
					{
						try
						{
							this.OnPacketReceived(this.m_pBuffer, this.m_pSocketArgs.BytesTransferred, (IPEndPoint)this.m_pSocketArgs.RemoteEndPoint);
						}
						catch (Exception x)
						{
							this.OnError(x);
						}
					}
					else
					{
						this.OnError(new Exception("Socket error '" + this.m_pSocketArgs.SocketError + "'."));
					}
					this.m_pSocketArgs.RemoteEndPoint = new IPEndPoint((this.m_pSocket.AddressFamily == AddressFamily.InterNetwork) ? IPAddress.Any : IPAddress.IPv6Any, 0);
				}
			}
			catch (Exception x2)
			{
				this.OnError(x2);
			}
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x00049A58 File Offset: 0x00048A58
		private void AsyncSocketReceive(IAsyncResult ar)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				try
				{
					EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
					int count = this.m_pSocket.EndReceiveFrom(ar, ref endPoint);
					this.OnPacketReceived(this.m_pBuffer, count, (IPEndPoint)endPoint);
				}
				catch (Exception x)
				{
					this.OnError(x);
				}
				try
				{
					EndPoint endPoint2 = new IPEndPoint((this.m_pSocket.AddressFamily == AddressFamily.InterNetwork) ? IPAddress.Any : IPAddress.IPv6Any, 0);
					this.m_pSocket.BeginReceiveFrom(this.m_pBuffer, 0, this.m_BufferSize, SocketFlags.None, ref endPoint2, new AsyncCallback(this.AsyncSocketReceive), null);
				}
				catch (Exception x2)
				{
					this.OnError(x2);
				}
			}
		}

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06000BFC RID: 3068 RVA: 0x00049B34 File Offset: 0x00048B34
		// (remove) Token: 0x06000BFD RID: 3069 RVA: 0x00049B6C File Offset: 0x00048B6C
		
		public event EventHandler<UDP_e_PacketReceived> PacketReceived = null;

		// Token: 0x06000BFE RID: 3070 RVA: 0x00049BA4 File Offset: 0x00048BA4
		private void OnPacketReceived(byte[] buffer, int count, IPEndPoint remoteEP)
		{
			bool flag = this.PacketReceived != null;
			if (flag)
			{
				this.m_pEventArgs.Reuse(this.m_pSocket, buffer, count, remoteEP);
				this.PacketReceived(this, this.m_pEventArgs);
			}
		}

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x06000BFF RID: 3071 RVA: 0x00049BEC File Offset: 0x00048BEC
		// (remove) Token: 0x06000C00 RID: 3072 RVA: 0x00049C24 File Offset: 0x00048C24
		
		public event EventHandler<ExceptionEventArgs> Error = null;

		// Token: 0x06000C01 RID: 3073 RVA: 0x00049C5C File Offset: 0x00048C5C
		private void OnError(Exception x)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				bool flag = this.Error != null;
				if (flag)
				{
					this.Error(this, new ExceptionEventArgs(x));
				}
			}
		}

		// Token: 0x040004E8 RID: 1256
		private bool m_IsDisposed = false;

		// Token: 0x040004E9 RID: 1257
		private bool m_IsRunning = false;

		// Token: 0x040004EA RID: 1258
		private Socket m_pSocket = null;

		// Token: 0x040004EB RID: 1259
		private byte[] m_pBuffer = null;

		// Token: 0x040004EC RID: 1260
		private int m_BufferSize = 8000;

		// Token: 0x040004ED RID: 1261
		private SocketAsyncEventArgs m_pSocketArgs = null;

		// Token: 0x040004EE RID: 1262
		private UDP_e_PacketReceived m_pEventArgs = null;
	}
}
