using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using LumiSoft.Net.Media.Codec;
using LumiSoft.Net.STUN.Client;
using LumiSoft.Net.UDP;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000DE RID: 222
	public class RTP_Session : IDisposable
	{
		// Token: 0x060008B3 RID: 2227 RVA: 0x000336D0 File Offset: 0x000326D0
		internal RTP_Session(RTP_MultimediaSession session, RTP_Address localEP, RTP_Clock clock)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			bool flag2 = localEP == null;
			if (flag2)
			{
				throw new ArgumentNullException("localEP");
			}
			bool flag3 = clock == null;
			if (flag3)
			{
				throw new ArgumentNullException("clock");
			}
			this.m_pSession = session;
			this.m_pLocalEP = localEP;
			this.m_pRtpClock = clock;
			this.m_pLocalSources = new List<RTP_Source_Local>();
			this.m_pTargets = new List<RTP_Address>();
			this.m_pMembers = new Dictionary<uint, RTP_Source>();
			this.m_pSenders = new Dictionary<uint, RTP_Source>();
			this.m_pConflictingEPs = new Dictionary<string, DateTime>();
			this.m_pPayloads = new KeyValueCollection<int, Codec>();
			this.m_pUdpDataReceivers = new List<UDP_DataReceiver>();
			this.m_pRtpSocket = new Socket(localEP.IP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			this.m_pRtpSocket.Bind(localEP.RtpEP);
			this.m_pRtcpSocket = new Socket(localEP.IP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			this.m_pRtcpSocket.Bind(localEP.RtcpEP);
			this.m_pRtcpTimer = new TimerEx();
			this.m_pRtcpTimer.Elapsed += delegate(object sender, ElapsedEventArgs e)
			{
				this.SendRtcp();
			};
			this.m_pRtcpTimer.AutoReset = false;
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x00033964 File Offset: 0x00032964
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				foreach (UDP_DataReceiver udp_DataReceiver in this.m_pUdpDataReceivers)
				{
					udp_DataReceiver.Dispose();
				}
				this.m_pUdpDataReceivers = null;
				bool flag = this.m_pRtcpTimer != null;
				if (flag)
				{
					this.m_pRtcpTimer.Dispose();
					this.m_pRtcpTimer = null;
				}
				this.m_pSession = null;
				this.m_pLocalEP = null;
				this.m_pTargets = null;
				foreach (RTP_Source_Local rtp_Source_Local in this.m_pLocalSources.ToArray())
				{
					rtp_Source_Local.Dispose();
				}
				this.m_pLocalSources = null;
				this.m_pRtcpSource = null;
				foreach (RTP_Source rtp_Source in this.m_pMembers.Values)
				{
					rtp_Source.Dispose();
				}
				this.m_pMembers = null;
				this.m_pSenders = null;
				this.m_pConflictingEPs = null;
				this.m_pRtpSocket.Close();
				this.m_pRtpSocket = null;
				this.m_pRtcpSocket.Close();
				this.m_pRtcpSocket = null;
				this.m_pUdpDataReceivers = null;
				this.OnDisposed();
				this.Disposed = null;
				this.Closed = null;
				this.NewSendStream = null;
				this.NewReceiveStream = null;
			}
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x00033B04 File Offset: 0x00032B04
		public void Close(string closeReason)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			RTCP_CompoundPacket rtcp_CompoundPacket = new RTCP_CompoundPacket();
			RTCP_Packet_RR rtcp_Packet_RR = new RTCP_Packet_RR();
			rtcp_Packet_RR.SSRC = this.m_pRtcpSource.SSRC;
			rtcp_CompoundPacket.Packets.Add(rtcp_Packet_RR);
			RTCP_Packet_SDES rtcp_Packet_SDES = new RTCP_Packet_SDES();
			rtcp_Packet_SDES.Chunks.Add(new RTCP_Packet_SDES_Chunk(this.m_pRtcpSource.SSRC, this.m_pSession.LocalParticipant.CNAME));
			rtcp_CompoundPacket.Packets.Add(rtcp_Packet_SDES);
			int i = 0;
			while (i < this.m_pLocalSources.Count)
			{
				uint[] array = new uint[Math.Min(this.m_pLocalSources.Count - i, 31)];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = this.m_pLocalSources[i].SSRC;
					i++;
				}
				RTCP_Packet_BYE rtcp_Packet_BYE = new RTCP_Packet_BYE();
				rtcp_Packet_BYE.Sources = array;
				rtcp_CompoundPacket.Packets.Add(rtcp_Packet_BYE);
			}
			this.SendRtcpPacket(rtcp_CompoundPacket);
			this.OnClosed();
			this.Dispose();
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x00033C3C File Offset: 0x00032C3C
		public void Start()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isStarted = this.m_IsStarted;
			if (!isStarted)
			{
				this.m_IsStarted = true;
				this.m_PMembersCount = 1;
				this.m_RtcpAvgPacketSize = 100.0;
				this.m_pRtcpSource = this.CreateLocalSource();
				this.m_pMembers.Add(this.m_pRtcpSource.SSRC, this.m_pRtcpSource);
				UDP_DataReceiver udp_DataReceiver = new UDP_DataReceiver(this.m_pRtpSocket);
				udp_DataReceiver.PacketReceived += delegate(object s1, UDP_e_PacketReceived e1)
				{
					this.ProcessRtp(e1.Buffer, e1.Count, e1.RemoteEP);
				};
				this.m_pUdpDataReceivers.Add(udp_DataReceiver);
				udp_DataReceiver.Start();
				UDP_DataReceiver udp_DataReceiver2 = new UDP_DataReceiver(this.m_pRtcpSocket);
				udp_DataReceiver2.PacketReceived += delegate(object s1, UDP_e_PacketReceived e1)
				{
					this.ProcessRtcp(e1.Buffer, e1.Count, e1.RemoteEP);
				};
				this.m_pUdpDataReceivers.Add(udp_DataReceiver2);
				udp_DataReceiver2.Start();
				this.Schedule(this.ComputeRtcpTransmissionInterval(this.m_pMembers.Count, this.m_pSenders.Count, (double)this.m_Bandwidth * 0.25, false, this.m_RtcpAvgPacketSize, true));
			}
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x00033D60 File Offset: 0x00032D60
		public void Stop()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			throw new NotImplementedException();
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x00033D90 File Offset: 0x00032D90
		public RTP_SendStream CreateSendStream()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			RTP_Source_Local rtp_Source_Local = this.CreateLocalSource();
			rtp_Source_Local.CreateStream();
			this.OnNewSendStream(rtp_Source_Local.Stream);
			return rtp_Source_Local.Stream;
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x00033DE0 File Offset: 0x00032DE0
		public void AddTarget(RTP_Address target)
		{
			bool flag = target == null;
			if (flag)
			{
				throw new ArgumentNullException("target");
			}
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag2 = this.m_pLocalEP.Equals(target);
			if (flag2)
			{
				throw new ArgumentException("Argument 'target' value collapses with property 'LocalEP'.", "target");
			}
			foreach (RTP_Address rtp_Address in this.Targets)
			{
				bool flag3 = rtp_Address.Equals(target);
				if (flag3)
				{
					throw new ArgumentException("Specified target already exists.", "target");
				}
			}
			this.m_pTargets.Add(target);
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x00033E90 File Offset: 0x00032E90
		public void RemoveTarget(RTP_Address target)
		{
			bool flag = target == null;
			if (flag)
			{
				throw new ArgumentNullException("target");
			}
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			this.m_pTargets.Remove(target);
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x00033EDC File Offset: 0x00032EDC
		public void RemoveTargets()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			this.m_pTargets.Clear();
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x00033F14 File Offset: 0x00032F14
		public bool StunPublicEndPoints(string server, int port, out IPEndPoint rtpEP, out IPEndPoint rtcpEP)
		{
			bool flag = server == null;
			if (flag)
			{
				throw new ArgumentNullException("server");
			}
			bool isStarted = this.m_IsStarted;
			if (isStarted)
			{
				throw new InvalidOperationException("Method 'StunPublicEndPoints' may be called only if RTP session has not started.");
			}
			rtpEP = null;
			rtcpEP = null;
			try
			{
				STUN_Result stun_Result = STUN_Client.Query(server, port, this.m_pRtpSocket);
				bool flag2 = stun_Result.NetType == STUN_NetType.FullCone || stun_Result.NetType == STUN_NetType.PortRestrictedCone || stun_Result.NetType == STUN_NetType.RestrictedCone;
				if (flag2)
				{
					rtpEP = stun_Result.PublicEndPoint;
					rtcpEP = STUN_Client.GetPublicEP(server, port, this.m_pRtcpSocket);
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x00033FC4 File Offset: 0x00032FC4
		internal int SendRtcpPacket(RTCP_CompoundPacket packet)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = packet == null;
			if (flag)
			{
				throw new ArgumentNullException("packet");
			}
			byte[] array = packet.ToByte();
			foreach (RTP_Address rtp_Address in this.Targets)
			{
				try
				{
					this.m_pRtcpSocket.SendTo(array, array.Length, SocketFlags.None, rtp_Address.RtcpEP);
					this.m_RtcpPacketsSent += 1L;
					this.m_RtcpBytesSent += (long)array.Length;
					this.m_RtcpAvgPacketSize = 0.0 + 0.0 * this.m_RtcpAvgPacketSize;
				}
				catch
				{
					this.m_RtcpFailedTransmissions += 1L;
				}
			}
			return array.Length;
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x000340B8 File Offset: 0x000330B8
		internal int SendRtpPacket(RTP_SendStream stream, RTP_Packet packet)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = packet == null;
			if (flag2)
			{
				throw new ArgumentNullException("packet");
			}
			Dictionary<uint, RTP_Source> pMembers = this.m_pMembers;
			lock (pMembers)
			{
				bool flag4 = !this.m_pMembers.ContainsKey(stream.Source.SSRC);
				if (flag4)
				{
					this.m_pMembers.Add(stream.Source.SSRC, stream.Source);
				}
			}
			Dictionary<uint, RTP_Source> pSenders = this.m_pSenders;
			lock (pSenders)
			{
				bool flag6 = !this.m_pSenders.ContainsKey(stream.Source.SSRC);
				if (flag6)
				{
					this.m_pSenders.Add(stream.Source.SSRC, stream.Source);
				}
			}
			byte[] buffer = new byte[this.m_MTU];
			int num = 0;
			packet.ToByte(buffer, ref num);
			foreach (RTP_Address rtp_Address in this.Targets)
			{
				try
				{
					this.m_pRtpSocket.BeginSendTo(buffer, 0, num, SocketFlags.None, rtp_Address.RtpEP, new AsyncCallback(this.RtpAsyncSocketSendCompleted), null);
				}
				catch
				{
					this.m_RtpFailedTransmissions += 1L;
				}
			}
			return num;
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x0003427C File Offset: 0x0003327C
		private void ProcessRtcp(byte[] buffer, int count, IPEndPoint remoteEP)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = remoteEP == null;
			if (flag2)
			{
				throw new ArgumentNullException("remoteEP");
			}
			this.m_RtcpPacketsReceived += 1L;
			this.m_RtcpBytesReceived += (long)count;
			this.m_RtcpAvgPacketSize = 0.0 + 0.0 * this.m_RtcpAvgPacketSize;
			try
			{
				RTCP_CompoundPacket rtcp_CompoundPacket = RTCP_CompoundPacket.Parse(buffer, count);
				foreach (RTCP_Packet rtcp_Packet in rtcp_CompoundPacket.Packets)
				{
					bool flag3 = rtcp_Packet.Type == 204;
					if (flag3)
					{
						RTCP_Packet_APP rtcp_Packet_APP = (RTCP_Packet_APP)rtcp_Packet;
						RTP_Source_Remote orCreateSource = this.GetOrCreateSource(true, rtcp_Packet_APP.Source, null, remoteEP);
						bool flag4 = orCreateSource != null;
						if (flag4)
						{
							orCreateSource.SetLastRtcpPacket(DateTime.Now);
							orCreateSource.OnAppPacket(rtcp_Packet_APP);
						}
					}
					else
					{
						bool flag5 = rtcp_Packet.Type == 203;
						if (flag5)
						{
							RTCP_Packet_BYE rtcp_Packet_BYE = (RTCP_Packet_BYE)rtcp_Packet;
							bool flag6 = false;
							foreach (uint num in rtcp_Packet_BYE.Sources)
							{
								RTP_Source orCreateSource2 = this.GetOrCreateSource(true, num, null, remoteEP);
								bool flag7 = orCreateSource2 != null;
								if (flag7)
								{
									flag6 = true;
									this.m_pMembers.Remove(num);
									orCreateSource2.Close(rtcp_Packet_BYE.LeavingReason);
								}
								this.m_pSenders.Remove(num);
							}
							bool flag8 = flag6;
							if (flag8)
							{
								this.DoReverseReconsideration();
							}
						}
						else
						{
							bool flag9 = rtcp_Packet.Type == 201;
							if (flag9)
							{
								RTCP_Packet_RR rtcp_Packet_RR = (RTCP_Packet_RR)rtcp_Packet;
								RTP_Source orCreateSource3 = this.GetOrCreateSource(true, rtcp_Packet_RR.SSRC, null, remoteEP);
								bool flag10 = orCreateSource3 != null;
								if (flag10)
								{
									orCreateSource3.SetLastRtcpPacket(DateTime.Now);
									foreach (RTCP_Packet_ReportBlock rr in rtcp_Packet_RR.ReportBlocks)
									{
										orCreateSource3 = this.GetOrCreateSource(true, rtcp_Packet_RR.SSRC, null, remoteEP);
										bool flag11 = orCreateSource3 != null;
										if (flag11)
										{
											orCreateSource3.SetLastRtcpPacket(DateTime.Now);
											orCreateSource3.SetRR(rr);
										}
									}
								}
							}
							else
							{
								bool flag12 = rtcp_Packet.Type == 202;
								if (flag12)
								{
									foreach (RTCP_Packet_SDES_Chunk rtcp_Packet_SDES_Chunk in ((RTCP_Packet_SDES)rtcp_Packet).Chunks)
									{
										RTP_Source orCreateSource4 = this.GetOrCreateSource(true, rtcp_Packet_SDES_Chunk.Source, rtcp_Packet_SDES_Chunk.CName, remoteEP);
										bool flag13 = orCreateSource4 != null;
										if (flag13)
										{
											orCreateSource4.SetLastRtcpPacket(DateTime.Now);
											RTP_Participant_Remote orCreateParticipant = this.m_pSession.GetOrCreateParticipant(string.IsNullOrEmpty(rtcp_Packet_SDES_Chunk.CName) ? "null" : rtcp_Packet_SDES_Chunk.CName);
											((RTP_Source_Remote)orCreateSource4).SetParticipant(orCreateParticipant);
											orCreateParticipant.EnsureSource(orCreateSource4);
											orCreateParticipant.Update(rtcp_Packet_SDES_Chunk);
										}
									}
								}
								else
								{
									bool flag14 = rtcp_Packet.Type == 200;
									if (flag14)
									{
										RTCP_Packet_SR rtcp_Packet_SR = (RTCP_Packet_SR)rtcp_Packet;
										RTP_Source_Remote orCreateSource5 = this.GetOrCreateSource(true, rtcp_Packet_SR.SSRC, null, remoteEP);
										bool flag15 = orCreateSource5 != null;
										if (flag15)
										{
											orCreateSource5.SetLastRtcpPacket(DateTime.Now);
											orCreateSource5.OnSenderReport(new RTCP_Report_Sender(rtcp_Packet_SR));
											foreach (RTCP_Packet_ReportBlock rr2 in rtcp_Packet_SR.ReportBlocks)
											{
												orCreateSource5 = this.GetOrCreateSource(true, rtcp_Packet_SR.SSRC, null, remoteEP);
												bool flag16 = orCreateSource5 != null;
												if (flag16)
												{
													orCreateSource5.SetLastRtcpPacket(DateTime.Now);
													orCreateSource5.SetRR(rr2);
												}
											}
										}
									}
									else
									{
										this.m_RtcpUnknownPacketsReceived += 1L;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				this.m_pSession.OnError(exception);
			}
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x00034744 File Offset: 0x00033744
		private void ProcessRtp(byte[] buffer, int count, IPEndPoint remoteEP)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = remoteEP == null;
			if (flag2)
			{
				throw new ArgumentNullException("remoteEP");
			}
			this.m_RtpPacketsReceived += 1L;
			this.m_RtpBytesReceived += (long)count;
			try
			{
				RTP_Packet rtp_Packet = RTP_Packet.Parse(buffer, count);
				RTP_Source orCreateSource = this.GetOrCreateSource(false, rtp_Packet.SSRC, null, remoteEP);
				bool flag3 = orCreateSource != null;
				if (flag3)
				{
					foreach (uint num in rtp_Packet.CSRC)
					{
						RTP_Source orCreateSource2 = this.GetOrCreateSource(false, rtp_Packet.SSRC, null, remoteEP);
					}
					Dictionary<uint, RTP_Source> pSenders = this.m_pSenders;
					lock (pSenders)
					{
						bool flag5 = !this.m_pSenders.ContainsKey(orCreateSource.SSRC);
						if (flag5)
						{
							this.m_pSenders.Add(orCreateSource.SSRC, orCreateSource);
						}
					}
					((RTP_Source_Remote)orCreateSource).OnRtpPacketReceived(rtp_Packet, count);
				}
			}
			catch (Exception exception)
			{
				this.m_pSession.OnError(exception);
			}
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x0003488C File Offset: 0x0003388C
		internal RTP_Source_Local CreateLocalSource()
		{
			uint num = RTP_Utils.GenerateSSRC();
			while (this.m_pMembers.ContainsKey(num))
			{
				num = RTP_Utils.GenerateSSRC();
			}
			RTP_Source_Local source = new RTP_Source_Local(this, num, this.m_pLocalEP.RtcpEP, this.m_pLocalEP.RtpEP);
			source.Disposing += delegate(object s, EventArgs e)
			{
				this.m_pSenders.Remove(source.SSRC);
				this.m_pMembers.Remove(source.SSRC);
				this.m_pLocalSources.Remove(source);
			};
			this.m_pLocalSources.Add(source);
			this.m_pSession.LocalParticipant.EnsureSource(source);
			return source;
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x00034938 File Offset: 0x00033938
		private RTP_Source_Remote GetOrCreateSource(bool rtcp_rtp, uint src, string cname, IPEndPoint packetEP)
		{
			bool flag = packetEP == null;
			if (flag)
			{
				throw new ArgumentNullException("packetEP");
			}
			RTP_Source rtp_Source = null;
			Dictionary<uint, RTP_Source> pMembers = this.m_pMembers;
			lock (pMembers)
			{
				this.m_pMembers.TryGetValue(src, out rtp_Source);
				bool flag3 = rtp_Source == null;
				if (flag3)
				{
					rtp_Source = new RTP_Source_Remote(this, src);
					if (rtcp_rtp)
					{
						rtp_Source.SetRtcpEP(packetEP);
					}
					else
					{
						rtp_Source.SetRtpEP(packetEP);
					}
					this.m_pMembers.Add(src, rtp_Source);
				}
				else
				{
					bool flag4 = (rtcp_rtp ? rtp_Source.RtcpEP : rtp_Source.RtpEP) == null;
					if (flag4)
					{
						if (rtcp_rtp)
						{
							rtp_Source.SetRtcpEP(packetEP);
						}
						else
						{
							rtp_Source.SetRtpEP(packetEP);
						}
					}
					else
					{
						bool flag5 = !packetEP.Equals(rtcp_rtp ? rtp_Source.RtcpEP : rtp_Source.RtpEP);
						if (flag5)
						{
							bool flag6 = !rtp_Source.IsLocal;
							if (flag6)
							{
								bool flag7 = cname != null && cname != rtp_Source.CName;
								if (flag7)
								{
									this.m_RemoteCollisions += 1L;
								}
								else
								{
									this.m_RemotePacketsLooped += 1L;
								}
								return null;
							}
							bool flag8 = this.m_pConflictingEPs.ContainsKey(packetEP.ToString());
							if (flag8)
							{
								bool flag9 = cname == null || cname == rtp_Source.CName;
								if (flag9)
								{
									this.m_LocalPacketsLooped += 1L;
								}
								this.m_pConflictingEPs[packetEP.ToString()] = DateTime.Now;
								return null;
							}
							this.m_LocalCollisions += 1L;
							this.m_pConflictingEPs.Add(packetEP.ToString(), DateTime.Now);
							this.m_pMembers.Remove(rtp_Source.SSRC);
							this.m_pSenders.Remove(rtp_Source.SSRC);
							uint ssrc = rtp_Source.SSRC;
							rtp_Source.GenerateNewSSRC();
							while (this.m_pMembers.ContainsKey(rtp_Source.SSRC))
							{
								rtp_Source.GenerateNewSSRC();
							}
							this.m_pMembers.Add(rtp_Source.SSRC, rtp_Source);
							RTCP_CompoundPacket rtcp_CompoundPacket = new RTCP_CompoundPacket();
							RTCP_Packet_RR rtcp_Packet_RR = new RTCP_Packet_RR();
							rtcp_Packet_RR.SSRC = this.m_pRtcpSource.SSRC;
							rtcp_CompoundPacket.Packets.Add(rtcp_Packet_RR);
							RTCP_Packet_SDES rtcp_Packet_SDES = new RTCP_Packet_SDES();
							RTCP_Packet_SDES_Chunk item = new RTCP_Packet_SDES_Chunk(rtp_Source.SSRC, this.m_pSession.LocalParticipant.CNAME);
							rtcp_Packet_SDES.Chunks.Add(item);
							rtcp_CompoundPacket.Packets.Add(rtcp_Packet_SDES);
							RTCP_Packet_BYE rtcp_Packet_BYE = new RTCP_Packet_BYE();
							rtcp_Packet_BYE.Sources = new uint[]
							{
								ssrc
							};
							rtcp_Packet_BYE.LeavingReason = "Collision, changing SSRC.";
							rtcp_CompoundPacket.Packets.Add(rtcp_Packet_BYE);
							this.SendRtcpPacket(rtcp_CompoundPacket);
							rtp_Source = new RTP_Source_Remote(this, src);
							if (rtcp_rtp)
							{
								rtp_Source.SetRtcpEP(packetEP);
							}
							else
							{
								rtp_Source.SetRtpEP(packetEP);
							}
							this.m_pMembers.Add(src, rtp_Source);
						}
					}
				}
			}
			return (RTP_Source_Remote)rtp_Source;
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x00034C90 File Offset: 0x00033C90
		private void Schedule(int seconds)
		{
			this.m_pRtcpTimer.Stop();
			this.m_pRtcpTimer.Interval = (double)(seconds * 1000);
			this.m_pRtcpTimer.Enabled = true;
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x00034CC0 File Offset: 0x00033CC0
		private int ComputeRtcpTransmissionInterval(int members, int senders, double rtcp_bw, bool we_sent, double avg_rtcp_size, bool initial)
		{
			double num = 5.0;
			double num2 = 0.25;
			double num3 = 1.0 - num2;
			double num4 = 1.21828;
			double num5 = num;
			if (initial)
			{
				num5 /= 2.0;
			}
			int num6 = members;
			bool flag = (double)senders <= (double)members * num2;
			if (flag)
			{
				if (we_sent)
				{
					rtcp_bw *= num2;
					num6 = senders;
				}
				else
				{
					rtcp_bw *= num2;
					num6 -= senders;
				}
			}
			double num7 = avg_rtcp_size * (double)num6 / rtcp_bw;
			bool flag2 = num7 < num5;
			if (flag2)
			{
				num7 = num5;
			}
			num7 *= (double)new Random().Next(5, 15) / 10.0;
			num7 /= num4;
			return (int)Math.Max(num7, 2.0);
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x00034DA0 File Offset: 0x00033DA0
		private void DoReverseReconsideration()
		{
			DateTime d = (this.m_RtcpLastTransmission == DateTime.MinValue) ? DateTime.Now : this.m_RtcpLastTransmission.AddMilliseconds(this.m_pRtcpTimer.Interval);
			this.Schedule((int)Math.Max((double)(this.m_pMembers.Count / this.m_PMembersCount) * (d - DateTime.Now).TotalSeconds, 2.0));
			this.m_PMembersCount = this.m_pMembers.Count;
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x00034E2C File Offset: 0x00033E2C
		private void TimeOutSsrc()
		{
			bool flag = false;
			RTP_Source[] array = new RTP_Source[this.m_pSenders.Count];
			this.m_pSenders.Values.CopyTo(array, 0);
			foreach (RTP_Source rtp_Source in array)
			{
				bool flag2 = rtp_Source.LastRtpPacket.AddMilliseconds(2.0 * this.m_pRtcpTimer.Interval) < DateTime.Now;
				if (flag2)
				{
					this.m_pSenders.Remove(rtp_Source.SSRC);
					rtp_Source.SetActivePassive(false);
				}
			}
			int num = this.ComputeRtcpTransmissionInterval(this.m_pMembers.Count, this.m_pSenders.Count, (double)this.m_Bandwidth * 0.25, false, this.m_RtcpAvgPacketSize, false);
			foreach (RTP_Source rtp_Source2 in this.Members)
			{
				bool flag3 = rtp_Source2.LastActivity.AddSeconds((double)(5 * num)) < DateTime.Now;
				if (flag3)
				{
					this.m_pMembers.Remove(rtp_Source2.SSRC);
					bool flag4 = !rtp_Source2.IsLocal;
					if (flag4)
					{
						rtp_Source2.Dispose();
					}
					flag = true;
				}
			}
			bool flag5 = flag;
			if (flag5)
			{
				this.DoReverseReconsideration();
			}
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00034F90 File Offset: 0x00033F90
		private void SendRtcp()
		{
			bool flag = false;
			try
			{
				this.m_pRtcpSource.SetLastRtcpPacket(DateTime.Now);
				RTCP_CompoundPacket rtcp_CompoundPacket = new RTCP_CompoundPacket();
				RTCP_Packet_RR rtcp_Packet_RR = null;
				List<RTP_SendStream> list = new List<RTP_SendStream>();
				foreach (RTP_SendStream rtp_SendStream in this.SendStreams)
				{
					bool flag2 = rtp_SendStream.RtcpCyclesSinceWeSent < 2;
					if (flag2)
					{
						list.Add(rtp_SendStream);
						flag = true;
					}
					rtp_SendStream.RtcpCycle();
				}
				bool flag3 = flag;
				if (flag3)
				{
					for (int j = 0; j < list.Count; j++)
					{
						RTP_SendStream rtp_SendStream2 = list[j];
						RTCP_Packet_SR rtcp_Packet_SR = new RTCP_Packet_SR(rtp_SendStream2.Source.SSRC);
						rtcp_Packet_SR.NtpTimestamp = RTP_Utils.DateTimeToNTP64(DateTime.Now);
						rtcp_Packet_SR.RtpTimestamp = this.m_pRtpClock.RtpTimestamp;
						rtcp_Packet_SR.SenderPacketCount = (uint)rtp_SendStream2.RtpPacketsSent;
						rtcp_Packet_SR.SenderOctetCount = (uint)rtp_SendStream2.RtpBytesSent;
						rtcp_CompoundPacket.Packets.Add(rtcp_Packet_SR);
					}
				}
				else
				{
					rtcp_Packet_RR = new RTCP_Packet_RR();
					rtcp_Packet_RR.SSRC = this.m_pRtcpSource.SSRC;
					rtcp_CompoundPacket.Packets.Add(rtcp_Packet_RR);
				}
				RTCP_Packet_SDES rtcp_Packet_SDES = new RTCP_Packet_SDES();
				RTCP_Packet_SDES_Chunk rtcp_Packet_SDES_Chunk = new RTCP_Packet_SDES_Chunk(this.m_pRtcpSource.SSRC, this.m_pSession.LocalParticipant.CNAME);
				this.m_pSession.LocalParticipant.AddNextOptionalSdesItem(rtcp_Packet_SDES_Chunk);
				rtcp_Packet_SDES.Chunks.Add(rtcp_Packet_SDES_Chunk);
				foreach (RTP_SendStream rtp_SendStream3 in list)
				{
					rtcp_Packet_SDES.Chunks.Add(new RTCP_Packet_SDES_Chunk(rtp_SendStream3.Source.SSRC, this.m_pSession.LocalParticipant.CNAME));
				}
				rtcp_CompoundPacket.Packets.Add(rtcp_Packet_SDES);
				RTP_Source[] senders = this.Senders;
				DateTime[] array = new DateTime[senders.Length];
				RTP_ReceiveStream[] array2 = new RTP_ReceiveStream[senders.Length];
				int num = 0;
				foreach (RTP_Source rtp_Source in senders)
				{
					bool flag4 = !rtp_Source.IsLocal && rtp_Source.LastRtpPacket > this.m_RtcpLastTransmission;
					if (flag4)
					{
						array[num] = rtp_Source.LastRRTime;
						array2[num] = ((RTP_Source_Remote)rtp_Source).Stream;
						num++;
					}
				}
				bool flag5 = rtcp_Packet_RR == null;
				if (flag5)
				{
					rtcp_Packet_RR = new RTCP_Packet_RR();
					rtcp_Packet_RR.SSRC = this.m_pRtcpSource.SSRC;
					rtcp_CompoundPacket.Packets.Add(rtcp_Packet_RR);
				}
				Array.Sort<DateTime, RTP_ReceiveStream>(array, array2, 0, num);
				for (int l = 1; l < 31; l++)
				{
					bool flag6 = num - l < 0;
					if (flag6)
					{
						break;
					}
					rtcp_Packet_RR.ReportBlocks.Add(array2[num - l].CreateReceiverReport());
				}
				this.SendRtcpPacket(rtcp_CompoundPacket);
				Dictionary<string, DateTime> pConflictingEPs = this.m_pConflictingEPs;
				lock (pConflictingEPs)
				{
					string[] array4 = new string[this.m_pConflictingEPs.Count];
					this.m_pConflictingEPs.Keys.CopyTo(array4, 0);
					foreach (string key in array4)
					{
						bool flag8 = this.m_pConflictingEPs[key].AddMinutes(3.0) < DateTime.Now;
						if (flag8)
						{
							this.m_pConflictingEPs.Remove(key);
						}
					}
				}
				this.TimeOutSsrc();
			}
			catch (Exception exception)
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					return;
				}
				this.m_pSession.OnError(exception);
			}
			this.m_RtcpLastTransmission = DateTime.Now;
			this.Schedule(this.ComputeRtcpTransmissionInterval(this.m_pMembers.Count, this.m_pSenders.Count, (double)this.m_Bandwidth * 0.25, flag, this.m_RtcpAvgPacketSize, false));
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x0003540C File Offset: 0x0003440C
		private void RtpAsyncSocketSendCompleted(IAsyncResult ar)
		{
			try
			{
				this.m_RtpBytesSent += (long)this.m_pRtpSocket.EndSendTo(ar);
				this.m_RtpPacketsSent += 1L;
			}
			catch
			{
				this.m_RtpFailedTransmissions += 1L;
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060008C9 RID: 2249 RVA: 0x0003546C File Offset: 0x0003446C
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060008CA RID: 2250 RVA: 0x00035484 File Offset: 0x00034484
		public RTP_MultimediaSession Session
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSession;
			}
		}

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060008CB RID: 2251 RVA: 0x000354B8 File Offset: 0x000344B8
		public RTP_Address LocalEP
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pLocalEP;
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x060008CC RID: 2252 RVA: 0x000354EC File Offset: 0x000344EC
		public RTP_Clock RtpClock
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRtpClock;
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x060008CD RID: 2253 RVA: 0x00035520 File Offset: 0x00034520
		// (set) Token: 0x060008CE RID: 2254 RVA: 0x00035554 File Offset: 0x00034554
		public RTP_StreamMode StreamMode
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_StreamMode;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_StreamMode = value;
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x060008CF RID: 2255 RVA: 0x00035588 File Offset: 0x00034588
		public RTP_Address[] Targets
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pTargets.ToArray();
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x060008D0 RID: 2256 RVA: 0x000355C4 File Offset: 0x000345C4
		public int MTU
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MTU;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x060008D1 RID: 2257 RVA: 0x000355F8 File Offset: 0x000345F8
		// (set) Token: 0x060008D2 RID: 2258 RVA: 0x0003562C File Offset: 0x0003462C
		public int Payload
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Payload;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = this.m_Payload != value;
				if (flag)
				{
					this.m_Payload = value;
					this.OnPayloadChanged();
				}
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x060008D3 RID: 2259 RVA: 0x00035678 File Offset: 0x00034678
		// (set) Token: 0x060008D4 RID: 2260 RVA: 0x000356AC File Offset: 0x000346AC
		public int Bandwidth
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Bandwidth;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value < 8;
				if (flag)
				{
					throw new ArgumentException("Property 'Bandwidth' value must be >= 8.");
				}
				this.m_Bandwidth = value;
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x060008D5 RID: 2261 RVA: 0x000356F4 File Offset: 0x000346F4
		public RTP_Source[] Members
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				Dictionary<uint, RTP_Source> pMembers = this.m_pMembers;
				RTP_Source[] result;
				lock (pMembers)
				{
					RTP_Source[] array = new RTP_Source[this.m_pMembers.Count];
					this.m_pMembers.Values.CopyTo(array, 0);
					result = array;
				}
				return result;
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x060008D6 RID: 2262 RVA: 0x00035778 File Offset: 0x00034778
		public RTP_Source[] Senders
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				Dictionary<uint, RTP_Source> pSenders = this.m_pSenders;
				RTP_Source[] result;
				lock (pSenders)
				{
					RTP_Source[] array = new RTP_Source[this.m_pSenders.Count];
					this.m_pSenders.Values.CopyTo(array, 0);
					result = array;
				}
				return result;
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x000357FC File Offset: 0x000347FC
		public RTP_SendStream[] SendStreams
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				List<RTP_Source_Local> pLocalSources = this.m_pLocalSources;
				RTP_SendStream[] result;
				lock (pLocalSources)
				{
					List<RTP_SendStream> list = new List<RTP_SendStream>();
					foreach (RTP_Source_Local rtp_Source_Local in this.m_pLocalSources)
					{
						bool flag2 = rtp_Source_Local.Stream != null;
						if (flag2)
						{
							list.Add(rtp_Source_Local.Stream);
						}
					}
					result = list.ToArray();
				}
				return result;
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x060008D8 RID: 2264 RVA: 0x000358C8 File Offset: 0x000348C8
		public RTP_ReceiveStream[] ReceiveStreams
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				Dictionary<uint, RTP_Source> pSenders = this.m_pSenders;
				RTP_ReceiveStream[] result;
				lock (pSenders)
				{
					List<RTP_ReceiveStream> list = new List<RTP_ReceiveStream>();
					foreach (RTP_Source rtp_Source in this.m_pSenders.Values)
					{
						bool flag2 = !rtp_Source.IsLocal;
						if (flag2)
						{
							list.Add(((RTP_Source_Remote)rtp_Source).Stream);
						}
					}
					result = list.ToArray();
				}
				return result;
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x060008D9 RID: 2265 RVA: 0x000359A0 File Offset: 0x000349A0
		public long RtpPacketsSent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtpPacketsSent;
			}
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x060008DA RID: 2266 RVA: 0x000359D4 File Offset: 0x000349D4
		public long RtpBytesSent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtpBytesSent;
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x060008DB RID: 2267 RVA: 0x00035A08 File Offset: 0x00034A08
		public long RtpPacketsReceived
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtpPacketsReceived;
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x060008DC RID: 2268 RVA: 0x00035A3C File Offset: 0x00034A3C
		public long RtpBytesReceived
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtpBytesReceived;
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060008DD RID: 2269 RVA: 0x00035A70 File Offset: 0x00034A70
		public long RtpFailedTransmissions
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtpFailedTransmissions;
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060008DE RID: 2270 RVA: 0x00035AA4 File Offset: 0x00034AA4
		public long RtcpPacketsSent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtcpPacketsSent;
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060008DF RID: 2271 RVA: 0x00035AD8 File Offset: 0x00034AD8
		public long RtcpBytesSent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtcpBytesSent;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060008E0 RID: 2272 RVA: 0x00035B0C File Offset: 0x00034B0C
		public long RtcpPacketsReceived
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtcpPacketsReceived;
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x060008E1 RID: 2273 RVA: 0x00035B40 File Offset: 0x00034B40
		public long RtcpBytesReceived
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtcpBytesReceived;
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x060008E2 RID: 2274 RVA: 0x00035B74 File Offset: 0x00034B74
		public long RtcpFailedTransmissions
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtcpFailedTransmissions;
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x060008E3 RID: 2275 RVA: 0x00035BA8 File Offset: 0x00034BA8
		public int RtcpInterval
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (int)(this.m_pRtcpTimer.Interval / 1000.0);
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x060008E4 RID: 2276 RVA: 0x00035BEC File Offset: 0x00034BEC
		public DateTime RtcpLastTransmission
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RtcpLastTransmission;
			}
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060008E5 RID: 2277 RVA: 0x00035C20 File Offset: 0x00034C20
		public long LocalCollisions
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LocalCollisions;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060008E6 RID: 2278 RVA: 0x00035C54 File Offset: 0x00034C54
		public long RemoteCollisions
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RemoteCollisions;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060008E7 RID: 2279 RVA: 0x00035C88 File Offset: 0x00034C88
		public long LocalPacketsLooped
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LocalPacketsLooped;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060008E8 RID: 2280 RVA: 0x00035CBC File Offset: 0x00034CBC
		public long RemotePacketsLooped
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RemotePacketsLooped;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060008E9 RID: 2281 RVA: 0x00035CF0 File Offset: 0x00034CF0
		public KeyValueCollection<int, Codec> Payloads
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pPayloads;
			}
		}

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x060008EA RID: 2282 RVA: 0x00035D24 File Offset: 0x00034D24
		// (remove) Token: 0x060008EB RID: 2283 RVA: 0x00035D5C File Offset: 0x00034D5C
		
		public event EventHandler Disposed = null;

		// Token: 0x060008EC RID: 2284 RVA: 0x00035D94 File Offset: 0x00034D94
		private void OnDisposed()
		{
			bool flag = this.Disposed != null;
			if (flag)
			{
				this.Disposed(this, new EventArgs());
			}
		}

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x060008ED RID: 2285 RVA: 0x00035DC4 File Offset: 0x00034DC4
		// (remove) Token: 0x060008EE RID: 2286 RVA: 0x00035DFC File Offset: 0x00034DFC
		
		public event EventHandler Closed = null;

		// Token: 0x060008EF RID: 2287 RVA: 0x00035E34 File Offset: 0x00034E34
		private void OnClosed()
		{
			bool flag = this.Closed != null;
			if (flag)
			{
				this.Closed(this, new EventArgs());
			}
		}

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x060008F0 RID: 2288 RVA: 0x00035E64 File Offset: 0x00034E64
		// (remove) Token: 0x060008F1 RID: 2289 RVA: 0x00035E9C File Offset: 0x00034E9C
		
		public event EventHandler<RTP_SendStreamEventArgs> NewSendStream = null;

		// Token: 0x060008F2 RID: 2290 RVA: 0x00035ED4 File Offset: 0x00034ED4
		private void OnNewSendStream(RTP_SendStream stream)
		{
			bool flag = this.NewSendStream != null;
			if (flag)
			{
				this.NewSendStream(this, new RTP_SendStreamEventArgs(stream));
			}
		}

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x060008F3 RID: 2291 RVA: 0x00035F04 File Offset: 0x00034F04
		// (remove) Token: 0x060008F4 RID: 2292 RVA: 0x00035F3C File Offset: 0x00034F3C
		
		public event EventHandler<RTP_ReceiveStreamEventArgs> NewReceiveStream = null;

		// Token: 0x060008F5 RID: 2293 RVA: 0x00035F74 File Offset: 0x00034F74
		internal void OnNewReceiveStream(RTP_ReceiveStream stream)
		{
			bool flag = this.NewReceiveStream != null;
			if (flag)
			{
				this.NewReceiveStream(this, new RTP_ReceiveStreamEventArgs(stream));
			}
		}

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x060008F6 RID: 2294 RVA: 0x00035FA4 File Offset: 0x00034FA4
		// (remove) Token: 0x060008F7 RID: 2295 RVA: 0x00035FDC File Offset: 0x00034FDC
		
		public event EventHandler PayloadChanged = null;

		// Token: 0x060008F8 RID: 2296 RVA: 0x00036014 File Offset: 0x00035014
		private void OnPayloadChanged()
		{
			bool flag = this.PayloadChanged != null;
			if (flag)
			{
				this.PayloadChanged(this, new EventArgs());
			}
		}

		// Token: 0x040003E3 RID: 995
		private object m_pLock = new object();

		// Token: 0x040003E4 RID: 996
		private bool m_IsDisposed = false;

		// Token: 0x040003E5 RID: 997
		private bool m_IsStarted = false;

		// Token: 0x040003E6 RID: 998
		private RTP_MultimediaSession m_pSession = null;

		// Token: 0x040003E7 RID: 999
		private RTP_Address m_pLocalEP = null;

		// Token: 0x040003E8 RID: 1000
		private RTP_Clock m_pRtpClock = null;

		// Token: 0x040003E9 RID: 1001
		private RTP_StreamMode m_StreamMode = RTP_StreamMode.SendReceive;

		// Token: 0x040003EA RID: 1002
		private List<RTP_Address> m_pTargets = null;

		// Token: 0x040003EB RID: 1003
		private int m_Payload = 0;

		// Token: 0x040003EC RID: 1004
		private int m_Bandwidth = 64000;

		// Token: 0x040003ED RID: 1005
		private List<RTP_Source_Local> m_pLocalSources = null;

		// Token: 0x040003EE RID: 1006
		private RTP_Source m_pRtcpSource = null;

		// Token: 0x040003EF RID: 1007
		private Dictionary<uint, RTP_Source> m_pMembers = null;

		// Token: 0x040003F0 RID: 1008
		private int m_PMembersCount = 0;

		// Token: 0x040003F1 RID: 1009
		private Dictionary<uint, RTP_Source> m_pSenders = null;

		// Token: 0x040003F2 RID: 1010
		private Dictionary<string, DateTime> m_pConflictingEPs = null;

		// Token: 0x040003F3 RID: 1011
		private List<UDP_DataReceiver> m_pUdpDataReceivers = null;

		// Token: 0x040003F4 RID: 1012
		private Socket m_pRtpSocket = null;

		// Token: 0x040003F5 RID: 1013
		private Socket m_pRtcpSocket = null;

		// Token: 0x040003F6 RID: 1014
		private long m_RtpPacketsSent = 0L;

		// Token: 0x040003F7 RID: 1015
		private long m_RtpBytesSent = 0L;

		// Token: 0x040003F8 RID: 1016
		private long m_RtpPacketsReceived = 0L;

		// Token: 0x040003F9 RID: 1017
		private long m_RtpBytesReceived = 0L;

		// Token: 0x040003FA RID: 1018
		private long m_RtpFailedTransmissions = 0L;

		// Token: 0x040003FB RID: 1019
		private long m_RtcpPacketsSent = 0L;

		// Token: 0x040003FC RID: 1020
		private long m_RtcpBytesSent = 0L;

		// Token: 0x040003FD RID: 1021
		private long m_RtcpPacketsReceived = 0L;

		// Token: 0x040003FE RID: 1022
		private long m_RtcpBytesReceived = 0L;

		// Token: 0x040003FF RID: 1023
		private double m_RtcpAvgPacketSize = 0.0;

		// Token: 0x04000400 RID: 1024
		private long m_RtcpFailedTransmissions = 0L;

		// Token: 0x04000401 RID: 1025
		private long m_RtcpUnknownPacketsReceived = 0L;

		// Token: 0x04000402 RID: 1026
		private DateTime m_RtcpLastTransmission = DateTime.MinValue;

		// Token: 0x04000403 RID: 1027
		private long m_LocalCollisions = 0L;

		// Token: 0x04000404 RID: 1028
		private long m_RemoteCollisions = 0L;

		// Token: 0x04000405 RID: 1029
		private long m_LocalPacketsLooped = 0L;

		// Token: 0x04000406 RID: 1030
		private long m_RemotePacketsLooped = 0L;

		// Token: 0x04000407 RID: 1031
		private int m_MTU = 1400;

		// Token: 0x04000408 RID: 1032
		private TimerEx m_pRtcpTimer = null;

		// Token: 0x04000409 RID: 1033
		private KeyValueCollection<int, Codec> m_pPayloads = null;
	}
}
