using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SharpCifs.Netbios;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;
using SharpCifs.Util.Transport;

namespace SharpCifs.Smb
{
	// Token: 0x020000B3 RID: 179
	public class SmbTransport : Transport
	{
		// Token: 0x060005B6 RID: 1462 RVA: 0x0001E604 File Offset: 0x0001C804
		internal static SmbTransport GetSmbTransport(UniAddress address, int port)
		{
			Type typeFromHandle = typeof(SmbTransport);
			SmbTransport smbTransport;
			lock (typeFromHandle)
			{
				smbTransport = SmbTransport.GetSmbTransport(address, port, SmbConstants.Laddr, SmbConstants.Lport, null);
			}
			return smbTransport;
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x0001E65C File Offset: 0x0001C85C
		internal static SmbTransport GetSmbTransport(UniAddress address, int port, IPAddress localAddr, int localPort, string hostName)
		{
			Type typeFromHandle = typeof(SmbTransport);
			SmbTransport result;
			lock (typeFromHandle)
			{
				List<SmbTransport> connections = SmbConstants.Connections;
				SmbTransport smbTransport;
				lock (connections)
				{
					bool flag3 = SmbConstants.SsnLimit != 1;
					if (flag3)
					{
						smbTransport = SmbConstants.Connections.FirstOrDefault((SmbTransport c) => c.Matches(address, port, localAddr, localPort, hostName) && (SmbConstants.SsnLimit == 0 || c.Sessions.Count < SmbConstants.SsnLimit));
						bool flag4 = smbTransport != null;
						if (flag4)
						{
							return smbTransport;
						}
					}
					smbTransport = new SmbTransport(address, port, localAddr, localPort);
					SmbConstants.Connections.Insert(0, smbTransport);
				}
				result = smbTransport;
			}
			return result;
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x0001E764 File Offset: 0x0001C964
		internal SmbTransport(UniAddress address, int port, IPAddress localAddr, int localPort)
		{
			this.Server = new SmbTransport.ServerData(this);
			this.Address = address;
			this.Port = port;
			this.LocalAddr = localAddr;
			this.LocalPort = localPort;
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x0001E830 File Offset: 0x0001CA30
		internal virtual SmbSession GetSmbSession()
		{
			SmbSession smbSession;
			lock (this)
			{
				smbSession = this.GetSmbSession(new NtlmPasswordAuthentication(null, null, null));
			}
			return smbSession;
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x0001E87C File Offset: 0x0001CA7C
		internal virtual SmbSession GetSmbSession(NtlmPasswordAuthentication auth)
		{
			bool flag = false;
			SmbSession result;
			try
			{
				Monitor.Enter(this, ref flag);
				SmbSession smbSession = this.Sessions.FirstOrDefault((SmbSession s) => s.Matches(auth));
				bool flag2 = smbSession != null;
				if (flag2)
				{
					smbSession.Auth = auth;
					result = smbSession;
				}
				else
				{
					long now = Runtime.CurrentTimeMillis();
					bool flag3 = SmbConstants.SoTimeout > 0 && this.SessionExpiration < (now);
					if (flag3)
					{
						this.SessionExpiration = now + (long)SmbConstants.SoTimeout;
						IEnumerable<SmbSession> sessions = this.Sessions;
						Func<SmbSession, bool> predicate;
						Func<SmbSession, bool> predicate2=null;
						predicate = predicate2;
						if (predicate == null)
						{
							predicate = (predicate2 = ((SmbSession s) => s.Expiration < now));
						}
						foreach (SmbSession smbSession2 in sessions.Where(predicate))
						{
							smbSession2.Logoff(false);
						}
					}
					smbSession = new SmbSession(this.Address, this.Port, this.LocalAddr, this.LocalPort, auth);
					smbSession.transport = this;
					this.Sessions.Add(smbSession);
					result = smbSession;
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this);
				}
			}
			return result;
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x0001EA08 File Offset: 0x0001CC08
		internal virtual bool Matches(UniAddress address, int port, IPAddress localAddr, int localPort, string hostName)
		{
			bool flag = hostName == null;
			if (flag)
			{
				hostName = address.GetHostName();
			}
			return (this.TconHostName == null || Runtime.EqualsIgnoreCase(hostName, this.TconHostName)) && address.Equals(this.Address) && (port == -1 || port == this.Port || (port == 445 && this.Port == 139)) && (localAddr == this.LocalAddr || (localAddr != null && localAddr.Equals(this.LocalAddr))) && localPort == this.LocalPort;
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x0001EA9C File Offset: 0x0001CC9C
		internal virtual bool HasCapability(int cap)
		{
			try
			{
				this.Connect((long)SmbConstants.ResponseTimeout);
			}
			catch (IOException ex)
			{
				throw new SmbException(ex.Message, ex);
			}
			return (this.Capabilities & cap) == cap;
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x0001EAEC File Offset: 0x0001CCEC
		internal virtual bool IsSignatureSetupRequired(NtlmPasswordAuthentication auth)
		{
			return (this.Flags2 & SmbConstants.Flags2SecuritySignatures) != 0 && this.Digest == null && auth != NtlmPasswordAuthentication.Null && !NtlmPasswordAuthentication.Null.Equals(auth);
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x0001EB30 File Offset: 0x0001CD30
		internal virtual void Ssn139()
		{
			Name name = new Name(this.Address.FirstCalledName(), 32, null);
			int num2;
			for (;;)
			{
				this.Socket = new SocketEx(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				bool flag = this.LocalAddr != null;
				if (flag)
				{
					this.Socket.Bind2(new IPEndPoint(this.LocalAddr, this.LocalPort));
				}
				this.Socket.Connect(new IPEndPoint(IPAddress.Parse(this.Address.GetHostAddress()), 139), SmbConstants.ConnTimeout);
				this.Socket.SoTimeOut = SmbConstants.SoTimeout;
				this.Out = this.Socket.GetOutputStream();
				this.In = this.Socket.GetInputStream();
				SessionServicePacket sessionServicePacket = new SessionRequestPacket(name, NbtAddress.GetLocalName());
				this.Out.Write(this.Sbuf, 0, sessionServicePacket.WriteWireFormat(this.Sbuf, 0));
				bool flag2 = Transport.Readn(this.In, this.Sbuf, 0, 4) < 4;
				if (flag2)
				{
					break;
				}
				int num = (int)(this.Sbuf[0] & byte.MaxValue);
				if (num == -1)
				{
					goto IL_1D4;
				}
				if (num == 130)
				{
					goto IL_148;
				}
				if (num != 131)
				{
					goto Block_5;
				}
				num2 = (this.In.Read() & 255);
				int num3 = num2;
				if (num3 != 128 && num3 != 130)
				{
					goto Block_8;
				}
				this.Socket.Close();
				if ((name.name = this.Address.NextCalledName()) == null)
				{
					goto Block_9;
				}
			}
			try
			{
				this.Socket.Close();
			}
			catch (IOException)
			{
			}
			throw new SmbException("EOF during NetBIOS session request");
			Block_5:
			this.Disconnect(true);
			throw new NbtException(2, 0);
			IL_148:
			bool flag3 = base.Log.Level >= 4;
			if (flag3)
			{
				base.Log.WriteLine("session established ok with " + this.Address);
			}
			return;
			Block_8:
			this.Disconnect(true);
			throw new NbtException(2, num2);
			IL_1D4:
			this.Disconnect(true);
			throw new NbtException(2, -1);
			Block_9:
			throw new IOException("Failed to establish session with " + this.Address);
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x0001ED7C File Offset: 0x0001CF7C
		private void Negotiate(int port, ServerMessageBlock resp)
		{
			byte[] sbuf = this.Sbuf;
			lock (sbuf)
			{
				bool flag2 = port == 139;
				if (flag2)
				{
					this.Ssn139();
				}
				else
				{
					bool flag3 = port == -1;
					if (flag3)
					{
						port = SmbConstants.DefaultPort;
					}
					this.Socket = new SocketEx(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					bool flag4 = this.LocalAddr != null;
					if (flag4)
					{
						this.Socket.Bind2(new IPEndPoint(this.LocalAddr, this.LocalPort));
					}
					this.Socket.Connect(new IPEndPoint(IPAddress.Parse(this.Address.GetHostAddress()), port), SmbConstants.ConnTimeout);
					this.Socket.SoTimeOut = SmbConstants.SoTimeout;
					this.Out = this.Socket.GetOutputStream();
					this.In = this.Socket.GetInputStream();
				}
				int num = this.Mid + 1;
				this.Mid = num;
				bool flag5 = num == 32000;
				if (flag5)
				{
					this.Mid = 1;
				}
				SmbTransport.NegotiateRequest.Mid = this.Mid;
				int num2 = SmbTransport.NegotiateRequest.Encode(this.Sbuf, 4);
				Encdec.Enc_uint32be(num2 & 65535, this.Sbuf, 0);
				bool flag6 = base.Log.Level >= 4;
				if (flag6)
				{
					base.Log.WriteLine(SmbTransport.NegotiateRequest);
					bool flag7 = base.Log.Level >= 6;
					if (flag7)
					{
						Hexdump.ToHexdump(base.Log, this.Sbuf, 4, num2);
					}
				}
				this.Out.Write(this.Sbuf, 0, 4 + num2);
				this.Out.Flush();
				bool flag8 = this.PeekKey() == null;
				if (flag8)
				{
					throw new IOException("transport closed in negotiate");
				}
				int num3 = (int)Encdec.Dec_uint16be(this.Sbuf, 2) & 65535;
				bool flag9 = num3 < 33 || 4 + num3 > this.Sbuf.Length;
				if (flag9)
				{
					throw new IOException("Invalid payload size: " + num3);
				}
				Transport.Readn(this.In, this.Sbuf, 36, num3 - 32);
				resp.Decode(this.Sbuf, 4);
				bool flag10 = base.Log.Level >= 4;
				if (flag10)
				{
					base.Log.WriteLine(resp);
					bool flag11 = base.Log.Level >= 6;
					if (flag11)
					{
						Hexdump.ToHexdump(base.Log, this.Sbuf, 4, num2);
					}
				}
			}
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x0001F040 File Offset: 0x0001D240
		public virtual void Connect()
		{
			try
			{
				base.Connect((long)SmbConstants.ResponseTimeout);
			}
			catch (TransportException rootCause)
			{
				throw new SmbException("Failed to connect: " + this.Address, rootCause);
			}
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x0001F088 File Offset: 0x0001D288
		protected internal override void DoConnect()
		{
			SmbComNegotiateResponse smbComNegotiateResponse = new SmbComNegotiateResponse(this.Server);
			try
			{
				this.Negotiate(this.Port, smbComNegotiateResponse);
			}
			catch (ConnectException)
			{
				this.Port = ((this.Port == -1 || this.Port == SmbConstants.DefaultPort) ? 139 : SmbConstants.DefaultPort);
				this.Negotiate(this.Port, smbComNegotiateResponse);
			}
			bool flag = smbComNegotiateResponse.DialectIndex > 10;
			if (flag)
			{
				throw new SmbException("This client does not support the negotiated dialect.");
			}
			bool flag2 = (this.Server.Capabilities & SmbConstants.CapExtendedSecurity) != SmbConstants.CapExtendedSecurity && this.Server.EncryptionKeyLength != 8 && SmbConstants.LmCompatibility == 0;
			if (flag2)
			{
				throw new SmbException("Unexpected encryption key length: " + this.Server.EncryptionKeyLength);
			}
			this.TconHostName = this.Address.GetHostName();
			bool flag3 = this.Server.SignaturesRequired || (this.Server.SignaturesEnabled && SmbConstants.Signpref);
			if (flag3)
			{
				this.Flags2 |= SmbConstants.Flags2SecuritySignatures;
			}
			else
			{
				this.Flags2 &= (65535 ^ SmbConstants.Flags2SecuritySignatures);
			}
			this.MaxMpxCount = Math.Min(this.MaxMpxCount, this.Server.MaxMpxCount);
			bool flag4 = this.MaxMpxCount < 1;
			if (flag4)
			{
				this.MaxMpxCount = 1;
			}
			this.SndBufSize = Math.Min(this.SndBufSize, this.Server.MaxBufferSize);
			this.Capabilities &= this.Server.Capabilities;
			bool flag5 = (this.Server.Capabilities & SmbConstants.CapExtendedSecurity) == SmbConstants.CapExtendedSecurity;
			if (flag5)
			{
				this.Capabilities |= SmbConstants.CapExtendedSecurity;
			}
			bool flag6 = (this.Capabilities & SmbConstants.CapUnicode) == 0;
			if (flag6)
			{
				bool forceUnicode = SmbConstants.ForceUnicode;
				if (forceUnicode)
				{
					this.Capabilities |= SmbConstants.CapUnicode;
				}
				else
				{
					this.UseUnicode = false;
					this.Flags2 &= (65535 ^ SmbConstants.Flags2Unicode);
				}
			}
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x0001F2CC File Offset: 0x0001D4CC
		protected internal override void DoDisconnect(bool hard)
		{
			try
			{
				foreach (SmbSession smbSession in this.Sessions)
				{
					smbSession.Logoff(hard);
				}
				this.Out.Close();
				this.In.Close();
				this.Socket.Close();
			}
			finally
			{
				this.Digest = null;
				this.Socket = null;
				this.TconHostName = null;
			}
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x0001F374 File Offset: 0x0001D574
		protected internal override void MakeKey(ServerMessageBlock request)
		{
			int num = this.Mid + 1;
			this.Mid = num;
			bool flag = num == 32000;
			if (flag)
			{
				this.Mid = 1;
			}
			request.Mid = this.Mid;
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x0001F3B4 File Offset: 0x0001D5B4
		protected internal override ServerMessageBlock PeekKey()
		{
			for (;;)
			{
				bool flag = Transport.Readn(this.In, this.Sbuf, 0, 4) < 4;
				if (flag)
				{
					break;
				}
				if (this.Sbuf[0] != 133)
				{
					goto Block_1;
				}
			}
			return null;
			Block_1:
			bool flag2 = Transport.Readn(this.In, this.Sbuf, 4, 32) < 32;
			ServerMessageBlock result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				bool flag3 = base.Log.Level >= 4;
				if (flag3)
				{
					base.Log.WriteLine("New data read: " + this);
					Hexdump.ToHexdump(base.Log, this.Sbuf, 4, 32);
				}
				for (;;)
				{
					bool flag4 = this.Sbuf[0] == 0 && this.Sbuf[1] == 0 && this.Sbuf[4] == byte.MaxValue && this.Sbuf[5] == 83 && this.Sbuf[6] == 77 && this.Sbuf[7] == 66;
					if (flag4)
					{
						break;
					}
					for (int i = 0; i < 35; i++)
					{
						this.Sbuf[i] = this.Sbuf[i + 1];
					}
					int num;
					bool flag5 = (num = this.In.Read()) == -1;
					if (flag5)
					{
						goto Block_11;
					}
					this.Sbuf[35] = (byte)num;
				}
				this.Key.Mid = ((int)Encdec.Dec_uint16le(this.Sbuf, 34) & 65535);
				return this.Key;
				Block_11:
				result = null;
			}
			return result;
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x0001F544 File Offset: 0x0001D744
		protected internal override void DoSend(ServerMessageBlock request)
		{
			byte[] buf = SmbTransport.Buf;
			lock (buf)
			{
				ServerMessageBlock serverMessageBlock = request;
				int num = serverMessageBlock.Encode(SmbTransport.Buf, 4);
				Encdec.Enc_uint32be(num & 65535, SmbTransport.Buf, 0);
				bool flag2 = base.Log.Level >= 4;
				if (flag2)
				{
					do
					{
						base.Log.WriteLine(serverMessageBlock);
					}
					while (serverMessageBlock is AndXServerMessageBlock && (serverMessageBlock = ((AndXServerMessageBlock)serverMessageBlock).Andx) != null);
					bool flag3 = base.Log.Level >= 6;
					if (flag3)
					{
						Hexdump.ToHexdump(base.Log, SmbTransport.Buf, 4, num);
					}
				}
				this.Out.Write(SmbTransport.Buf, 0, 4 + num);
			}
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x0001F630 File Offset: 0x0001D830
		protected internal virtual void DoSend0(ServerMessageBlock request)
		{
			try
			{
				this.DoSend(request);
			}
			catch (IOException ex)
			{
				bool flag = base.Log.Level > 2;
				if (flag)
				{
					Runtime.PrintStackTrace(ex, base.Log);
				}
				try
				{
					this.Disconnect(true);
				}
				catch (IOException ex2)
				{
					Runtime.PrintStackTrace(ex2, base.Log);
				}
				throw;
			}
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x0001F6AC File Offset: 0x0001D8AC
		protected internal override void DoRecv(Response response)
		{
			ServerMessageBlock serverMessageBlock = (ServerMessageBlock)response;
			serverMessageBlock.UseUnicode = this.UseUnicode;
			serverMessageBlock.ExtendedSecurity = ((this.Capabilities & SmbConstants.CapExtendedSecurity) == SmbConstants.CapExtendedSecurity);
			byte[] buf = SmbTransport.Buf;
			lock (buf)
			{
				Array.Copy(this.Sbuf, 0, SmbTransport.Buf, 0, 4 + SmbConstants.HeaderLength);
				int num = (int)Encdec.Dec_uint16be(SmbTransport.Buf, 2) & 65535;
				bool flag2 = num < SmbConstants.HeaderLength + 1 || 4 + num > this.RcvBufSize;
				if (flag2)
				{
					throw new IOException("Invalid payload size: " + num);
				}
				int num2 = Encdec.Dec_uint32le(SmbTransport.Buf, 9) & -1;
				bool flag3 = serverMessageBlock.Command == 46 && (num2 == 0 || num2 == -2147483643);
				if (flag3)
				{
					SmbComReadAndXResponse smbComReadAndXResponse = (SmbComReadAndXResponse)serverMessageBlock;
					int num3 = SmbConstants.HeaderLength;
					Transport.Readn(this.In, SmbTransport.Buf, 4 + num3, 27);
					num3 += 27;
					serverMessageBlock.Decode(SmbTransport.Buf, 4);
					int num4 = smbComReadAndXResponse.DataOffset - num3;
					bool flag4 = smbComReadAndXResponse.ByteCount > 0 && num4 > 0 && num4 < 4;
					if (flag4)
					{
						Transport.Readn(this.In, SmbTransport.Buf, 4 + num3, num4);
					}
					bool flag5 = smbComReadAndXResponse.DataLength > 0;
					if (flag5)
					{
						Transport.Readn(this.In, smbComReadAndXResponse.B, smbComReadAndXResponse.Off, smbComReadAndXResponse.DataLength);
					}
				}
				else
				{
					Transport.Readn(this.In, SmbTransport.Buf, 36, num - 32);
					serverMessageBlock.Decode(SmbTransport.Buf, 4);
					bool flag6 = serverMessageBlock is SmbComTransactionResponse;
					if (flag6)
					{
						((SmbComTransactionResponse)serverMessageBlock).Current();
					}
				}
				bool flag7 = this.Digest != null && serverMessageBlock.ErrorCode == 0;
				if (flag7)
				{
					this.Digest.Verify(SmbTransport.Buf, 4, serverMessageBlock);
				}
				bool flag8 = base.Log.Level >= 4;
				if (flag8)
				{
					base.Log.WriteLine(response);
					bool flag9 = base.Log.Level >= 6;
					if (flag9)
					{
						Hexdump.ToHexdump(base.Log, SmbTransport.Buf, 4, num);
					}
				}
			}
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x0001F930 File Offset: 0x0001DB30
		protected internal override void DoSkip()
		{
			int num = (int)Encdec.Dec_uint16be(this.Sbuf, 2) & 65535;
			bool flag = num < 33 || 4 + num > this.RcvBufSize;
			if (flag)
			{
				this.In.Skip((long)this.In.Available());
			}
			else
			{
				this.In.Skip((long)(num - 32));
			}
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x0001F998 File Offset: 0x0001DB98
		internal virtual void CheckStatus(ServerMessageBlock req, ServerMessageBlock resp)
		{
			resp.ErrorCode = SmbException.GetStatusByCode(resp.ErrorCode);
			int errorCode = resp.ErrorCode;
			if (errorCode <= -1073741710)
			{
				if (errorCode <= -1073741802)
				{
					if (errorCode == -2147483643)
					{
						goto IL_12E;
					}
					if (errorCode != -1073741802)
					{
						goto IL_120;
					}
					goto IL_12E;
				}
				else if (errorCode != -1073741790)
				{
					switch (errorCode)
					{
					case -1073741718:
					case -1073741715:
					case -1073741714:
					case -1073741713:
					case -1073741712:
					case -1073741711:
					case -1073741710:
						break;
					case -1073741717:
					case -1073741716:
						goto IL_120;
					default:
						goto IL_120;
					}
				}
			}
			else if (errorCode <= -1073741260)
			{
				if (errorCode != -1073741428 && errorCode != -1073741260)
				{
					goto IL_120;
				}
			}
			else if (errorCode != -1073741225)
			{
				if (errorCode != 0)
				{
					goto IL_120;
				}
				goto IL_12E;
			}
			else
			{
				bool flag = req.Auth == null;
				if (flag)
				{
					throw new SmbException(resp.ErrorCode, null);
				}
				DfsReferral dfsReferrals = this.GetDfsReferrals(req.Auth, req.Path, 1);
				bool flag2 = dfsReferrals == null;
				if (flag2)
				{
					throw new SmbException(resp.ErrorCode, null);
				}

				// modify 0901
				//SmbFile.Dfs.Insert(req.Path, dfsReferrals);
				// end modify 0901
				throw dfsReferrals;
			}
			throw new SmbAuthException(resp.ErrorCode);
			IL_120:
			throw new SmbException(resp.ErrorCode, null);
			IL_12E:
			bool verifyFailed = resp.VerifyFailed;
			if (verifyFailed)
			{
				throw new SmbException("Signature verification failed.");
			}
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x0001FAEC File Offset: 0x0001DCEC
		internal virtual void Send(ServerMessageBlock request, ServerMessageBlock response)
		{
			this.Connect();
			request.Flags2 |= this.Flags2;
			request.UseUnicode = this.UseUnicode;
			request.Response = response;
			bool flag = request.Digest == null;
			if (flag)
			{
				request.Digest = this.Digest;
			}
			try
			{
				bool flag2 = response == null;
				if (flag2)
				{
					this.DoSend0(request);
					return;
				}
				bool flag3 = request is SmbComTransaction;
				if (flag3)
				{
					response.Command = request.Command;
					SmbComTransaction smbComTransaction = (SmbComTransaction)request;
					SmbComTransactionResponse smbComTransactionResponse = (SmbComTransactionResponse)response;
					smbComTransaction.MaxBufferSize = this.SndBufSize;
					smbComTransactionResponse.Reset();
					try
					{
						BufferCache.GetBuffers(smbComTransaction, smbComTransactionResponse);
						smbComTransaction.Current();
						bool flag4 = smbComTransaction.MoveNext();
						if (flag4)
						{
							SmbComBlankResponse smbComBlankResponse = new SmbComBlankResponse();
							this.Sendrecv(smbComTransaction, smbComBlankResponse, (long)SmbConstants.ResponseTimeout);
							bool flag5 = smbComBlankResponse.ErrorCode != 0;
							if (flag5)
							{
								this.CheckStatus(smbComTransaction, smbComBlankResponse);
							}
							smbComTransaction.Current();
						}
						else
						{
							this.MakeKey(smbComTransaction);
						}
						lock (this)
						{
							response.Received = false;
							smbComTransactionResponse.IsReceived = false;
							try
							{
								this.ResponseMap.Put(smbComTransaction, smbComTransactionResponse);
								do
								{
									this.DoSend0(smbComTransaction);
								}
								while (smbComTransaction.MoveNext() && smbComTransaction.Current() != null);
								long num = (long)SmbConstants.ResponseTimeout;
								smbComTransactionResponse.Expiration = Runtime.CurrentTimeMillis() + num;
								while (smbComTransactionResponse.MoveNext())
								{
									Runtime.Wait(this, num);
									num = smbComTransactionResponse.Expiration - Runtime.CurrentTimeMillis();
									bool flag7 = num <= 0L;
									if (flag7)
									{
										throw new TransportException(this + " timedout waiting for response to " + smbComTransaction);
									}
								}
								bool flag8 = response.ErrorCode != 0;
								if (flag8)
								{
									this.CheckStatus(smbComTransaction, smbComTransactionResponse);
								}
							}
							catch (Exception ex)
							{
								bool flag9 = ex is SmbException;
								if (flag9)
								{
									throw;
								}
								throw new TransportException(ex);
							}
							finally
							{
								this.ResponseMap.Remove(smbComTransaction);
							}
						}
					}
					finally
					{
						BufferCache.ReleaseBuffer(smbComTransaction.TxnBuf);
						BufferCache.ReleaseBuffer(smbComTransactionResponse.TxnBuf);
					}
				}
				else
				{
					response.Command = request.Command;
					this.Sendrecv(request, response, (long)SmbConstants.ResponseTimeout);
				}
			}
			catch (SmbException ex2)
			{
				throw;
			}
			catch (IOException ex3)
			{
				throw new SmbException(ex3.Message, ex3);
			}
			this.CheckStatus(request, response);
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x0001FE0C File Offset: 0x0001E00C
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				"[",
				this.Address,
				":",
				this.Port,
				"]"
			});
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x0001FE64 File Offset: 0x0001E064
		internal virtual void DfsPathSplit(string path, string[] result)
		{
			int i = 0;
			int num = result.Length - 1;
			int num2 = 0;
			int index = 0;
			int length = path.Length;
			for (;;)
			{
				bool flag = i == num;
				if (flag)
				{
					break;
				}
				bool flag2 = num2 == length || path[num2] == '\\';
				if (flag2)
				{
					result[i++] = Runtime.Substring(path, index, num2);
					index = num2 + 1;
				}
				if (num2++ >= length)
				{
					goto Block_4;
				}
			}
			result[num] = Runtime.Substring(path, index);
			return;
			Block_4:
			while (i < result.Length)
			{
				result[i++] = string.Empty;
			}
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x0001FEF8 File Offset: 0x0001E0F8
		internal virtual DfsReferral GetDfsReferrals(NtlmPasswordAuthentication auth, string path, int rn)
		{
			SmbTree smbTree = this.GetSmbSession(auth).GetSmbTree("IPC$", null);
			Trans2GetDfsReferralResponse trans2GetDfsReferralResponse = new Trans2GetDfsReferralResponse();
			smbTree.Send(new Trans2GetDfsReferral(path), trans2GetDfsReferralResponse);
			bool flag = trans2GetDfsReferralResponse.NumReferrals == 0;
			DfsReferral result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = rn == 0 || trans2GetDfsReferralResponse.NumReferrals < rn;
				if (flag2)
				{
					rn = trans2GetDfsReferralResponse.NumReferrals;
				}
				DfsReferral dfsReferral = new DfsReferral();
				string[] array = new string[4];
				long expiration = Runtime.CurrentTimeMillis() + Dfs.Ttl * 1000L;
				int num = 0;
				for (;;)
				{
					dfsReferral.ResolveHashes = auth.HashesExternal;
					dfsReferral.Ttl = (long)trans2GetDfsReferralResponse.Referrals[num].Ttl;
					dfsReferral.Expiration = expiration;
					bool flag3 = path.Equals(string.Empty);
					if (flag3)
					{
						dfsReferral.Server = Runtime.Substring(trans2GetDfsReferralResponse.Referrals[num].Path, 1).ToLower();
					}
					else
					{
						this.DfsPathSplit(trans2GetDfsReferralResponse.Referrals[num].Node, array);
						dfsReferral.Server = array[1];
						dfsReferral.Share = array[2];
						dfsReferral.Path = array[3];
					}
					dfsReferral.PathConsumed = trans2GetDfsReferralResponse.PathConsumed;
					num++;
					bool flag4 = num == rn;
					if (flag4)
					{
						break;
					}
					dfsReferral.Append(new DfsReferral());
					dfsReferral = dfsReferral.Next;
				}
				result = dfsReferral.Next;
			}
			return result;
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x00020064 File Offset: 0x0001E264
		internal virtual DfsReferral[] __getDfsReferrals(NtlmPasswordAuthentication auth, string path, int rn)
		{
			SmbTree smbTree = this.GetSmbSession(auth).GetSmbTree("IPC$", null);
			Trans2GetDfsReferralResponse trans2GetDfsReferralResponse = new Trans2GetDfsReferralResponse();
			smbTree.Send(new Trans2GetDfsReferral(path), trans2GetDfsReferralResponse);
			bool flag = rn == 0 || trans2GetDfsReferralResponse.NumReferrals < rn;
			if (flag)
			{
				rn = trans2GetDfsReferralResponse.NumReferrals;
			}
			DfsReferral[] array = new DfsReferral[rn];
			string[] array2 = new string[4];
			long expiration = Runtime.CurrentTimeMillis() + Dfs.Ttl * 1000L;
			for (int i = 0; i < array.Length; i++)
			{
				DfsReferral dfsReferral = new DfsReferral();
				dfsReferral.ResolveHashes = auth.HashesExternal;
				dfsReferral.Ttl = (long)trans2GetDfsReferralResponse.Referrals[i].Ttl;
				dfsReferral.Expiration = expiration;
				bool flag2 = path.Equals(string.Empty);
				if (flag2)
				{
					dfsReferral.Server = Runtime.Substring(trans2GetDfsReferralResponse.Referrals[i].Path, 1).ToLower();
				}
				else
				{
					this.DfsPathSplit(trans2GetDfsReferralResponse.Referrals[i].Node, array2);
					dfsReferral.Server = array2[1];
					dfsReferral.Share = array2[2];
					dfsReferral.Path = array2[3];
				}
				dfsReferral.PathConsumed = trans2GetDfsReferralResponse.PathConsumed;
				array[i] = dfsReferral;
			}
			return array;
		}

		// Token: 0x0400035D RID: 861
		internal static readonly byte[] Buf = new byte[65535];

		// Token: 0x0400035E RID: 862
		internal static readonly SmbComNegotiate NegotiateRequest = new SmbComNegotiate();

		// Token: 0x0400035F RID: 863
		internal static LogStream LogStatic = LogStream.GetInstance();

		// Token: 0x04000360 RID: 864
		internal static Hashtable DfsRoots = null;

		// Token: 0x04000361 RID: 865
		internal IPAddress LocalAddr;

		// Token: 0x04000362 RID: 866
		internal int LocalPort;

		// Token: 0x04000363 RID: 867
		internal UniAddress Address;

		// Token: 0x04000364 RID: 868
		internal SocketEx Socket;

		// Token: 0x04000365 RID: 869
		internal int Port;

		// Token: 0x04000366 RID: 870
		internal int Mid;

		// Token: 0x04000367 RID: 871
		internal OutputStream Out;

		// Token: 0x04000368 RID: 872
		internal InputStream In;

		// Token: 0x04000369 RID: 873
		internal byte[] Sbuf = new byte[512];

		// Token: 0x0400036A RID: 874
		internal SmbComBlankResponse Key = new SmbComBlankResponse();

		// Token: 0x0400036B RID: 875
		internal long SessionExpiration = Runtime.CurrentTimeMillis() + (long)SmbConstants.SoTimeout;

		// Token: 0x0400036C RID: 876
		internal List<object> Referrals = new List<object>();

		// Token: 0x0400036D RID: 877
		internal SigningDigest Digest;

		// Token: 0x0400036E RID: 878
		internal List<SmbSession> Sessions = new List<SmbSession>();

		// Token: 0x0400036F RID: 879
		internal SmbTransport.ServerData Server;

		// Token: 0x04000370 RID: 880
		internal int Flags2 = SmbConstants.Flags2;

		// Token: 0x04000371 RID: 881
		internal int MaxMpxCount = SmbConstants.MaxMpxCount;

		// Token: 0x04000372 RID: 882
		internal int SndBufSize = SmbConstants.SndBufSize;

		// Token: 0x04000373 RID: 883
		internal int RcvBufSize = SmbConstants.RcvBufSize;

		// Token: 0x04000374 RID: 884
		internal int Capabilities = SmbConstants.Capabilities;

		// Token: 0x04000375 RID: 885
		internal int SessionKey = 0;

		// Token: 0x04000376 RID: 886
		internal bool UseUnicode = SmbConstants.UseUnicode;

		// Token: 0x04000377 RID: 887
		internal string TconHostName;

		// Token: 0x02000120 RID: 288
		internal class ServerData
		{
			// Token: 0x0600082E RID: 2094 RVA: 0x0002B546 File Offset: 0x00029746
			internal ServerData(SmbTransport enclosing)
			{
				this._enclosing = enclosing;
			}

			// Token: 0x04000575 RID: 1397
			internal byte Flags;

			// Token: 0x04000576 RID: 1398
			internal int Flags2;

			// Token: 0x04000577 RID: 1399
			internal int MaxMpxCount;

			// Token: 0x04000578 RID: 1400
			internal int MaxBufferSize;

			// Token: 0x04000579 RID: 1401
			internal int SessionKey;

			// Token: 0x0400057A RID: 1402
			internal int Capabilities;

			// Token: 0x0400057B RID: 1403
			internal string OemDomainName;

			// Token: 0x0400057C RID: 1404
			internal int SecurityMode;

			// Token: 0x0400057D RID: 1405
			internal int Security;

			// Token: 0x0400057E RID: 1406
			internal bool EncryptedPasswords;

			// Token: 0x0400057F RID: 1407
			internal bool SignaturesEnabled;

			// Token: 0x04000580 RID: 1408
			internal bool SignaturesRequired;

			// Token: 0x04000581 RID: 1409
			internal int MaxNumberVcs;

			// Token: 0x04000582 RID: 1410
			internal int MaxRawSize;

			// Token: 0x04000583 RID: 1411
			internal long ServerTime;

			// Token: 0x04000584 RID: 1412
			internal int ServerTimeZone;

			// Token: 0x04000585 RID: 1413
			internal int EncryptionKeyLength;

			// Token: 0x04000586 RID: 1414
			internal byte[] EncryptionKey;

			// Token: 0x04000587 RID: 1415
			internal byte[] Guid;

			// Token: 0x04000588 RID: 1416
			private readonly SmbTransport _enclosing;
		}
	}
}
