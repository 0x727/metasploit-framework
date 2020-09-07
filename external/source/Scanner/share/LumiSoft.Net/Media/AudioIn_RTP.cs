using System;
using System.Collections.Generic;
using System.Diagnostics;
using LumiSoft.Net.Media.Codec.Audio;
using LumiSoft.Net.RTP;

namespace LumiSoft.Net.Media
{
	// Token: 0x0200014E RID: 334
	public class AudioIn_RTP : IDisposable
	{
		// Token: 0x06000D91 RID: 3473 RVA: 0x000550B0 File Offset: 0x000540B0
		public AudioIn_RTP(AudioInDevice audioInDevice, int audioFrameSize, Dictionary<int, AudioCodec> codecs, RTP_SendStream stream)
		{
			bool flag = audioInDevice == null;
			if (flag)
			{
				throw new ArgumentNullException("audioInDevice");
			}
			bool flag2 = codecs == null;
			if (flag2)
			{
				throw new ArgumentNullException("codecs");
			}
			bool flag3 = stream == null;
			if (flag3)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pAudioInDevice = audioInDevice;
			this.m_AudioFrameSize = audioFrameSize;
			this.m_pAudioCodecs = codecs;
			this.m_pRTP_Stream = stream;
			this.m_pRTP_Stream.Session.PayloadChanged += this.m_pRTP_Stream_PayloadChanged;
			this.m_pAudioCodecs.TryGetValue(this.m_pRTP_Stream.Session.Payload, out this.m_pActiveCodec);
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x000551A8 File Offset: 0x000541A8
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.Stop();
				this.m_IsDisposed = true;
				this.Error = null;
				this.m_pAudioInDevice = null;
				this.m_pAudioCodecs = null;
				this.m_pRTP_Stream.Session.PayloadChanged -= this.m_pRTP_Stream_PayloadChanged;
				this.m_pRTP_Stream = null;
				this.m_pActiveCodec = null;
			}
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x00055214 File Offset: 0x00054214
		private void m_pRTP_Stream_PayloadChanged(object sender, EventArgs e)
		{
			bool isRunning = this.m_IsRunning;
			if (isRunning)
			{
				this.Stop();
				this.m_pActiveCodec = null;
				this.m_pAudioCodecs.TryGetValue(this.m_pRTP_Stream.Session.Payload, out this.m_pActiveCodec);
				this.Start();
			}
		}

		// Token: 0x06000D94 RID: 3476 RVA: 0x00055268 File Offset: 0x00054268
		private void m_pWaveIn_AudioFrameReceived(object sender, EventArgs<byte[]> e)
		{
			try
			{
				bool flag = this.m_RtpTimeStamp == 0U || this.m_RtpTimeStamp > this.m_pRTP_Stream.Session.RtpClock.RtpTimestamp;
				if (flag)
				{
					this.m_RtpTimeStamp = this.m_pRTP_Stream.Session.RtpClock.RtpTimestamp;
				}
				else
				{
					this.m_RtpTimeStamp += (uint)this.m_pRTP_Stream.Session.RtpClock.MillisecondsToRtpTicks(this.m_AudioFrameSize);
				}
				bool flag2 = this.m_pActiveCodec != null;
				if (flag2)
				{
					RTP_Packet rtp_Packet = new RTP_Packet();
					rtp_Packet.Data = this.m_pActiveCodec.Encode(e.Value, 0, e.Value.Length);
					rtp_Packet.Timestamp = this.m_RtpTimeStamp;
					this.m_pRTP_Stream.Send(rtp_Packet);
				}
			}
			catch (Exception x)
			{
				bool flag3 = !this.IsDisposed;
				if (flag3)
				{
					this.OnError(x);
				}
			}
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x00055370 File Offset: 0x00054370
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
				bool flag = this.m_pActiveCodec != null;
				if (flag)
				{
					int bufferSize = this.m_pActiveCodec.AudioFormat.SamplesPerSecond / (1000 / this.m_AudioFrameSize) * (this.m_pActiveCodec.AudioFormat.BitsPerSample / 8);
					this.m_pWaveIn = new _WaveIn(this.m_pAudioInDevice, this.m_pActiveCodec.AudioFormat.SamplesPerSecond, this.m_pActiveCodec.AudioFormat.BitsPerSample, 1, bufferSize);
					this.m_pWaveIn.AudioFrameReceived += this.m_pWaveIn_AudioFrameReceived;
					this.m_pWaveIn.Start();
				}
			}
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x0005544C File Offset: 0x0005444C
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
				bool flag2 = this.m_pWaveIn != null;
				if (flag2)
				{
					this.m_pWaveIn.Dispose();
					this.m_pWaveIn = null;
				}
				this.m_IsRunning = false;
			}
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06000D97 RID: 3479 RVA: 0x000554B0 File Offset: 0x000544B0
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x06000D98 RID: 3480 RVA: 0x000554C8 File Offset: 0x000544C8
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

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06000D99 RID: 3481 RVA: 0x000554FC File Offset: 0x000544FC
		// (set) Token: 0x06000D9A RID: 3482 RVA: 0x00055530 File Offset: 0x00054530
		public AudioInDevice AudioInDevice
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAudioInDevice;
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
					throw new ArgumentNullException("AudioInDevice");
				}
				this.m_pAudioInDevice = value;
				bool isRunning = this.IsRunning;
				if (isRunning)
				{
					this.Stop();
					this.Start();
				}
			}
		}

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06000D9B RID: 3483 RVA: 0x00055590 File Offset: 0x00054590
		public RTP_SendStream RTP_Stream
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRTP_Stream;
			}
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06000D9C RID: 3484 RVA: 0x000555C4 File Offset: 0x000545C4
		public AudioCodec AudioCodec
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

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06000D9D RID: 3485 RVA: 0x000555F8 File Offset: 0x000545F8
		// (set) Token: 0x06000D9E RID: 3486 RVA: 0x0005562C File Offset: 0x0005462C
		public Dictionary<int, AudioCodec> AudioCodecs
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
					throw new ArgumentNullException("AudioCodecs");
				}
				this.m_pAudioCodecs = value;
			}
		}

		// Token: 0x1400005B RID: 91
		// (add) Token: 0x06000D9F RID: 3487 RVA: 0x00055674 File Offset: 0x00054674
		// (remove) Token: 0x06000DA0 RID: 3488 RVA: 0x000556AC File Offset: 0x000546AC
		
		public event EventHandler<ExceptionEventArgs> Error = null;

		// Token: 0x06000DA1 RID: 3489 RVA: 0x000556E4 File Offset: 0x000546E4
		private void OnError(Exception x)
		{
			bool flag = this.Error != null;
			if (flag)
			{
				this.Error(this, new ExceptionEventArgs(x));
			}
		}

		// Token: 0x040005AD RID: 1453
		private bool m_IsDisposed = false;

		// Token: 0x040005AE RID: 1454
		private bool m_IsRunning = false;

		// Token: 0x040005AF RID: 1455
		private AudioInDevice m_pAudioInDevice = null;

		// Token: 0x040005B0 RID: 1456
		private int m_AudioFrameSize = 20;

		// Token: 0x040005B1 RID: 1457
		private Dictionary<int, AudioCodec> m_pAudioCodecs = null;

		// Token: 0x040005B2 RID: 1458
		private RTP_SendStream m_pRTP_Stream = null;

		// Token: 0x040005B3 RID: 1459
		private AudioCodec m_pActiveCodec = null;

		// Token: 0x040005B4 RID: 1460
		private _WaveIn m_pWaveIn = null;

		// Token: 0x040005B5 RID: 1461
		private uint m_RtpTimeStamp = 0U;
	}
}
