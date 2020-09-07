using System;
using System.Collections.Generic;
using System.Diagnostics;
using LumiSoft.Net.Media.Codec.Audio;
using LumiSoft.Net.RTP;

namespace LumiSoft.Net.Media
{
	// Token: 0x0200014F RID: 335
	public class AudioOut_RTP : IDisposable
	{
		// Token: 0x06000DA2 RID: 3490 RVA: 0x00055714 File Offset: 0x00054714
		public AudioOut_RTP(AudioOutDevice audioOutDevice, RTP_ReceiveStream stream, Dictionary<int, AudioCodec> codecs)
		{
			bool flag = audioOutDevice == null;
			if (flag)
			{
				throw new ArgumentNullException("audioOutDevice");
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag3 = codecs == null;
			if (flag3)
			{
				throw new ArgumentNullException("codecs");
			}
			this.m_pAudioOutDevice = audioOutDevice;
			this.m_pRTP_Stream = stream;
			this.m_pAudioCodecs = codecs;
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x000557B4 File Offset: 0x000547B4
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.Stop();
				this.Error = null;
				this.m_pAudioOutDevice = null;
				this.m_pRTP_Stream = null;
				this.m_pAudioCodecs = null;
				this.m_pActiveCodec = null;
			}
		}

		// Token: 0x06000DA4 RID: 3492 RVA: 0x000557FC File Offset: 0x000547FC
		private void m_pRTP_Stream_PacketReceived(object sender, RTP_PacketEventArgs e)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				try
				{
					AudioCodec audioCodec = null;
					bool flag = !this.m_pAudioCodecs.TryGetValue(e.Packet.PayloadType, out audioCodec);
					if (!flag)
					{
						this.m_pActiveCodec = audioCodec;
						bool flag2 = this.m_pAudioOut == null;
						if (flag2)
						{
							this.m_pAudioOut = new AudioOut(this.m_pAudioOutDevice, audioCodec.AudioFormat);
						}
						else
						{
							bool flag3 = !this.m_pAudioOut.AudioFormat.Equals(audioCodec.AudioFormat);
							if (flag3)
							{
								this.m_pAudioOut.Dispose();
								this.m_pAudioOut = new AudioOut(this.m_pAudioOutDevice, audioCodec.AudioFormat);
							}
						}
						byte[] array = audioCodec.Decode(e.Packet.Data, 0, e.Packet.Data.Length);
						this.m_pAudioOut.Write(array, 0, array.Length);
					}
				}
				catch (Exception x)
				{
					bool flag4 = !this.IsDisposed;
					if (flag4)
					{
						this.OnError(x);
					}
				}
			}
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x0005591C File Offset: 0x0005491C
		public void Start()
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isRunning = this.m_IsRunning;
			if (!isRunning)
			{
				this.m_IsRunning = true;
				this.m_pRTP_Stream.PacketReceived += this.m_pRTP_Stream_PacketReceived;
			}
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x00055974 File Offset: 0x00054974
		public void Stop()
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.m_IsRunning;
			if (!flag)
			{
				this.m_IsRunning = false;
				this.m_pRTP_Stream.PacketReceived -= this.m_pRTP_Stream_PacketReceived;
				bool flag2 = this.m_pAudioOut != null;
				if (flag2)
				{
					this.m_pAudioOut.Dispose();
					this.m_pAudioOut = null;
				}
			}
		}

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06000DA7 RID: 3495 RVA: 0x000559F0 File Offset: 0x000549F0
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x00055A08 File Offset: 0x00054A08
		public bool IsRunning
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_IsRunning;
			}
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06000DA9 RID: 3497 RVA: 0x00055A3C File Offset: 0x00054A3C
		// (set) Token: 0x06000DAA RID: 3498 RVA: 0x00055A70 File Offset: 0x00054A70
		public AudioOutDevice AudioOutDevice
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAudioOutDevice;
			}
			set
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("AudioOutDevice");
				}
				this.m_pAudioOutDevice = value;
				bool isRunning = this.IsRunning;
				if (isRunning)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06000DAB RID: 3499 RVA: 0x00055AD0 File Offset: 0x00054AD0
		public Dictionary<int, AudioCodec> Codecs
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAudioCodecs;
			}
		}

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x00055B04 File Offset: 0x00054B04
		public AudioCodec ActiveCodec
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pActiveCodec;
			}
		}

		// Token: 0x1400005C RID: 92
		// (add) Token: 0x06000DAD RID: 3501 RVA: 0x00055B38 File Offset: 0x00054B38
		// (remove) Token: 0x06000DAE RID: 3502 RVA: 0x00055B70 File Offset: 0x00054B70
		
		public event EventHandler<ExceptionEventArgs> Error = null;

		// Token: 0x06000DAF RID: 3503 RVA: 0x00055BA8 File Offset: 0x00054BA8
		private void OnError(Exception x)
		{
			bool flag = this.Error != null;
			if (flag)
			{
				this.Error(this, new ExceptionEventArgs(x));
			}
		}

		// Token: 0x040005B7 RID: 1463
		private bool m_IsDisposed = false;

		// Token: 0x040005B8 RID: 1464
		private bool m_IsRunning = false;

		// Token: 0x040005B9 RID: 1465
		private AudioOutDevice m_pAudioOutDevice = null;

		// Token: 0x040005BA RID: 1466
		private RTP_ReceiveStream m_pRTP_Stream = null;

		// Token: 0x040005BB RID: 1467
		private Dictionary<int, AudioCodec> m_pAudioCodecs = null;

		// Token: 0x040005BC RID: 1468
		private AudioOut m_pAudioOut = null;

		// Token: 0x040005BD RID: 1469
		private AudioCodec m_pActiveCodec = null;
	}
}
