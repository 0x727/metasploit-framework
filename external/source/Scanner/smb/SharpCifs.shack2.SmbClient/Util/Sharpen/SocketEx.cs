using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000064 RID: 100
	public class SocketEx : Socket
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x0000B5F4 File Offset: 0x000097F4
		// (set) Token: 0x060002C6 RID: 710 RVA: 0x0000B60C File Offset: 0x0000980C
		public int SoTimeOut
		{
			get
			{
				return this._soTimeOut;
			}
			set
			{
				bool flag = value > 0;
				if (flag)
				{
					this._soTimeOut = value;
				}
				else
				{
					this._soTimeOut = -1;
				}
			}
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000B636 File Offset: 0x00009836
		public SocketEx(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
		{
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000B64C File Offset: 0x0000984C
		public void Connect(IPEndPoint endPoint, int timeOut)
		{
			AutoResetEvent autoReset = new AutoResetEvent(false);
			SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs
			{
				RemoteEndPoint = endPoint
			};
			socketAsyncEventArgs.Completed += delegate(object s, SocketAsyncEventArgs e)
			{
				autoReset.Set();
			};
			base.ConnectAsync(socketAsyncEventArgs);
			bool flag = !autoReset.WaitOne(timeOut);
			if (flag)
			{
				Socket.CancelConnectAsync(socketAsyncEventArgs);
				throw new ConnectException("Can't connect to end point.");
			}
			bool flag2 = socketAsyncEventArgs.SocketError > SocketError.Success;
			if (flag2)
			{
				throw new ConnectException("Can't connect to end point.");
			}
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000B6D4 File Offset: 0x000098D4
		public void Bind2(EndPoint ep)
		{
			bool flag = ep == null;
			if (flag)
			{
				base.Bind(new IPEndPoint(IPAddress.Any, 0));
			}
			else
			{
				base.Bind(ep);
			}
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0000B708 File Offset: 0x00009908
		public int Receive(byte[] buffer, int offset, int count)
		{
			AutoResetEvent autoReset = new AutoResetEvent(false);
			SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
			socketAsyncEventArgs.UserToken = this;
			socketAsyncEventArgs.SetBuffer(buffer, offset, count);
			socketAsyncEventArgs.Completed += delegate(object s, SocketAsyncEventArgs e)
			{
				autoReset.Set();
			};
			bool flag = base.ReceiveAsync(socketAsyncEventArgs);
			if (flag)
			{
				bool flag2 = !autoReset.WaitOne(this._soTimeOut);
				if (flag2)
				{
					throw new TimeoutException("No data received.");
				}
			}
			return socketAsyncEventArgs.BytesTransferred;
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000B790 File Offset: 0x00009990
		public void Send(byte[] buffer, int offset, int length, EndPoint destination = null)
		{
			AutoResetEvent autoReset = new AutoResetEvent(false);
			SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
			socketAsyncEventArgs.UserToken = this;
			socketAsyncEventArgs.SetBuffer(buffer, offset, length);
			socketAsyncEventArgs.Completed += delegate(object s, SocketAsyncEventArgs e)
			{
				autoReset.Set();
			};
			socketAsyncEventArgs.RemoteEndPoint = (destination ?? base.RemoteEndPoint);
			base.SendToAsync(socketAsyncEventArgs);
			bool flag = !autoReset.WaitOne(this._soTimeOut);
			if (flag)
			{
				throw new TimeoutException("No data sent.");
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000B81C File Offset: 0x00009A1C
		public InputStream GetInputStream()
		{
			return new NetworkStream(this);
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000B83C File Offset: 0x00009A3C
		public OutputStream GetOutputStream()
		{
			return new NetworkStream(this);
		}

		// Token: 0x04000083 RID: 131
		private int _soTimeOut = -1;
	}
}
