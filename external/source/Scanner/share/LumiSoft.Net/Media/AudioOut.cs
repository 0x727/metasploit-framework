using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace LumiSoft.Net.Media
{
	// Token: 0x02000153 RID: 339
	public class AudioOut : IDisposable
	{
		// Token: 0x06000DCB RID: 3531 RVA: 0x00056230 File Offset: 0x00055230
		public AudioOut(AudioOutDevice device, AudioFormat format)
		{
			bool flag = device == null;
			if (flag)
			{
				throw new ArgumentNullException("device");
			}
			bool flag2 = format == null;
			if (flag2)
			{
				throw new ArgumentNullException("format");
			}
			this.m_pDevice = device;
			this.m_pAudioFormat = format;
			this.m_pWaveOut = new AudioOut.WaveOut(device, format.SamplesPerSecond, format.BitsPerSample, format.Channels);
		}

		// Token: 0x06000DCC RID: 3532 RVA: 0x000562B8 File Offset: 0x000552B8
		public AudioOut(AudioOutDevice device, int samplesPerSec, int bitsPerSample, int channels)
		{
			bool flag = device == null;
			if (flag)
			{
				throw new ArgumentNullException("device");
			}
			bool flag2 = samplesPerSec < 1;
			if (flag2)
			{
				throw new ArgumentException("Argument 'samplesPerSec' value must be >= 1.", "samplesPerSec");
			}
			bool flag3 = bitsPerSample < 8;
			if (flag3)
			{
				throw new ArgumentException("Argument 'bitsPerSample' value must be >= 8.", "bitsPerSample");
			}
			bool flag4 = channels < 1;
			if (flag4)
			{
				throw new ArgumentException("Argument 'channels' value must be >= 1.", "channels");
			}
			this.m_pDevice = device;
			this.m_pAudioFormat = new AudioFormat(samplesPerSec, bitsPerSample, channels);
			this.m_pWaveOut = new AudioOut.WaveOut(device, samplesPerSec, bitsPerSample, channels);
		}

		// Token: 0x06000DCD RID: 3533 RVA: 0x00056370 File Offset: 0x00055370
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.m_pWaveOut.Dispose();
				this.m_pWaveOut = null;
			}
		}

		// Token: 0x06000DCE RID: 3534 RVA: 0x000563A8 File Offset: 0x000553A8
		public void Write(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0 || offset > buffer.Length;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			bool flag3 = count < 0 || count > buffer.Length + offset;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			bool flag4 = count % this.BlockSize != 0;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' is not n * BlockSize.");
			}
			this.m_pWaveOut.Play(buffer, offset, count);
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06000DCF RID: 3535 RVA: 0x00056438 File Offset: 0x00055438
		public static AudioOutDevice[] Devices
		{
			get
			{
				return AudioOut.WaveOut.Devices;
			}
		}

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06000DD0 RID: 3536 RVA: 0x00056450 File Offset: 0x00055450
		public AudioOutDevice OutputDevice
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pDevice;
			}
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x06000DD1 RID: 3537 RVA: 0x00056484 File Offset: 0x00055484
		public AudioFormat AudioFormat
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAudioFormat;
			}
		}

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06000DD2 RID: 3538 RVA: 0x000564B8 File Offset: 0x000554B8
		public int SamplesPerSec
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAudioFormat.SamplesPerSecond;
			}
		}

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06000DD3 RID: 3539 RVA: 0x000564F4 File Offset: 0x000554F4
		public int BitsPerSample
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAudioFormat.BitsPerSample;
			}
		}

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x06000DD4 RID: 3540 RVA: 0x00056530 File Offset: 0x00055530
		public int Channels
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAudioFormat.Channels;
			}
		}

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06000DD5 RID: 3541 RVA: 0x0005656C File Offset: 0x0005556C
		public int BlockSize
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAudioFormat.Channels * (this.m_pAudioFormat.BitsPerSample / 8);
			}
		}

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06000DD6 RID: 3542 RVA: 0x000565B4 File Offset: 0x000555B4
		// (set) Token: 0x06000DD7 RID: 3543 RVA: 0x000565F0 File Offset: 0x000555F0
		public int Volume
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pWaveOut.Volume;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value < 0 || value > 100;
				if (flag)
				{
					throw new ArgumentException("Property 'Volume' value must be >=0 and <= 100.");
				}
				this.m_pWaveOut.Volume = value;
			}
		}

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06000DD8 RID: 3544 RVA: 0x00056644 File Offset: 0x00055644
		public int BytesBuffered
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pWaveOut.BytesBuffered;
			}
		}

		// Token: 0x040005CB RID: 1483
		private bool m_IsDisposed = false;

		// Token: 0x040005CC RID: 1484
		private AudioOutDevice m_pDevice = null;

		// Token: 0x040005CD RID: 1485
		private AudioFormat m_pAudioFormat = null;

		// Token: 0x040005CE RID: 1486
		private AudioOut.WaveOut m_pWaveOut = null;

		// Token: 0x02000310 RID: 784
		private class WaveOut
		{
			// Token: 0x06001A0F RID: 6671 RVA: 0x000A0138 File Offset: 0x0009F138
			public WaveOut(AudioOutDevice outputDevice, int samplesPerSec, int bitsPerSample, int channels)
			{
				bool flag = outputDevice == null;
				if (flag)
				{
					throw new ArgumentNullException("outputDevice");
				}
				bool flag2 = samplesPerSec < 8000;
				if (flag2)
				{
					throw new ArgumentException("Argument 'samplesPerSec' value must be >= 8000.");
				}
				bool flag3 = bitsPerSample < 8;
				if (flag3)
				{
					throw new ArgumentException("Argument 'bitsPerSample' value must be >= 8.");
				}
				bool flag4 = channels < 1;
				if (flag4)
				{
					throw new ArgumentException("Argument 'channels' value must be >= 1.");
				}
				this.m_pOutDevice = outputDevice;
				this.m_SamplesPerSec = samplesPerSec;
				this.m_BitsPerSample = bitsPerSample;
				this.m_Channels = channels;
				this.m_BlockSize = this.m_Channels * (this.m_BitsPerSample / 8);
				this.m_pPlayItems = new List<AudioOut.WaveOut.PlayItem>();
				AudioOut.WaveOut.WAVEFORMATEX waveformatex = new AudioOut.WaveOut.WAVEFORMATEX();
				waveformatex.wFormatTag = 1;
				waveformatex.nChannels = (ushort)this.m_Channels;
				waveformatex.nSamplesPerSec = (uint)samplesPerSec;
				waveformatex.nAvgBytesPerSec = (uint)(this.m_SamplesPerSec * this.m_Channels * (this.m_BitsPerSample / 8));
				waveformatex.nBlockAlign = (ushort)this.m_BlockSize;
				waveformatex.wBitsPerSample = (ushort)this.m_BitsPerSample;
				waveformatex.cbSize = 0;
				this.m_pWaveOutProc = new AudioOut.WaveOut.waveOutProc(this.OnWaveOutProc);
				int num = AudioOut.WaveOut.WavMethods.waveOutOpen(out this.m_pWavDevHandle, this.m_pOutDevice.Index, waveformatex, this.m_pWaveOutProc, 0, 196608);
				bool flag5 = num != 0;
				if (flag5)
				{
					throw new Exception("Failed to open wav device, error: " + num.ToString() + ".");
				}
			}

			// Token: 0x06001A10 RID: 6672 RVA: 0x000A0300 File Offset: 0x0009F300
			~WaveOut()
			{
				this.Dispose();
			}

			// Token: 0x06001A11 RID: 6673 RVA: 0x000A0330 File Offset: 0x0009F330
			public void Dispose()
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					this.m_IsDisposed = true;
					try
					{
						AudioOut.WaveOut.WavMethods.waveOutReset(this.m_pWavDevHandle);
						AudioOut.WaveOut.WavMethods.waveOutClose(this.m_pWavDevHandle);
						foreach (AudioOut.WaveOut.PlayItem playItem in this.m_pPlayItems)
						{
							AudioOut.WaveOut.WavMethods.waveOutUnprepareHeader(this.m_pWavDevHandle, playItem.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(playItem.Header));
							playItem.Dispose();
						}
						AudioOut.WaveOut.WavMethods.waveOutClose(this.m_pWavDevHandle);
						this.m_pOutDevice = null;
						this.m_pWavDevHandle = IntPtr.Zero;
						this.m_pPlayItems = null;
						this.m_pWaveOutProc = null;
					}
					catch
					{
					}
				}
			}

			// Token: 0x06001A12 RID: 6674 RVA: 0x000A0424 File Offset: 0x0009F424
			private void OnWaveOutProc(IntPtr hdrvr, int uMsg, int dwUser, int dwParam1, int dwParam2)
			{
				try
				{
					bool flag = uMsg == 957;
					if (flag)
					{
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnCleanUpFirstBlock));
					}
				}
				catch
				{
				}
			}

			// Token: 0x06001A13 RID: 6675 RVA: 0x000A046C File Offset: 0x0009F46C
			private void OnCleanUpFirstBlock(object state)
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					try
					{
						List<AudioOut.WaveOut.PlayItem> pPlayItems = this.m_pPlayItems;
						lock (pPlayItems)
						{
							AudioOut.WaveOut.PlayItem playItem = this.m_pPlayItems[0];
							AudioOut.WaveOut.WavMethods.waveOutUnprepareHeader(this.m_pWavDevHandle, playItem.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(playItem.Header));
							this.m_pPlayItems.Remove(playItem);
							this.m_BytesBuffered -= playItem.DataSize;
							playItem.Dispose();
						}
					}
					catch
					{
					}
				}
			}

			// Token: 0x06001A14 RID: 6676 RVA: 0x000A0530 File Offset: 0x0009F530
			public void Play(byte[] audioData, int offset, int count)
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("WaveOut");
				}
				bool flag = audioData == null;
				if (flag)
				{
					throw new ArgumentNullException("audioData");
				}
				bool flag2 = count % this.m_BlockSize != 0;
				if (flag2)
				{
					throw new ArgumentException("Audio data is not n * BlockSize.");
				}
				byte[] array = new byte[count];
				Array.Copy(audioData, offset, array, 0, count);
				GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				AudioOut.WaveOut.WAVEHDR wavehdr = new AudioOut.WaveOut.WAVEHDR
				{
					lpData = gchandle.AddrOfPinnedObject(),
					dwBufferLength = (uint)array.Length,
					dwBytesRecorded = 0U,
					dwUser = IntPtr.Zero,
					dwFlags = 0U,
					dwLoops = 0U,
					lpNext = IntPtr.Zero,
					reserved = 0U
				};
				GCHandle gchandle2 = GCHandle.Alloc(wavehdr, GCHandleType.Pinned);
				int num = AudioOut.WaveOut.WavMethods.waveOutPrepareHeader(this.m_pWavDevHandle, gchandle2.AddrOfPinnedObject(), Marshal.SizeOf(wavehdr));
				bool flag3 = num == 0;
				if (flag3)
				{
					AudioOut.WaveOut.PlayItem item = new AudioOut.WaveOut.PlayItem(ref gchandle2, ref gchandle, array.Length);
					this.m_pPlayItems.Add(item);
					this.m_BytesBuffered += array.Length;
					bool flag4 = this.m_BytesBuffered < 1000;
					if (flag4)
					{
						bool flag5 = !this.m_IsPaused;
						if (flag5)
						{
							AudioOut.WaveOut.WavMethods.waveOutPause(this.m_pWavDevHandle);
							this.m_IsPaused = true;
						}
					}
					else
					{
						bool flag6 = this.m_IsPaused && this.m_BytesBuffered > this.m_MinBuffer;
						if (flag6)
						{
							AudioOut.WaveOut.WavMethods.waveOutRestart(this.m_pWavDevHandle);
							this.m_IsPaused = false;
						}
					}
					num = AudioOut.WaveOut.WavMethods.waveOutWrite(this.m_pWavDevHandle, gchandle2.AddrOfPinnedObject(), Marshal.SizeOf(wavehdr));
				}
				else
				{
					gchandle.Free();
					gchandle2.Free();
				}
			}

			// Token: 0x17000858 RID: 2136
			// (get) Token: 0x06001A15 RID: 6677 RVA: 0x000A0708 File Offset: 0x0009F708
			public static AudioOutDevice[] Devices
			{
				get
				{
					List<AudioOutDevice> list = new List<AudioOutDevice>();
					int num = AudioOut.WaveOut.WavMethods.waveOutGetNumDevs();
					for (int i = 0; i < num; i++)
					{
						AudioOut.WaveOut.WAVEOUTCAPS waveoutcaps = default(AudioOut.WaveOut.WAVEOUTCAPS);
						bool flag = AudioOut.WaveOut.WavMethods.waveOutGetDevCaps((uint)i, ref waveoutcaps, Marshal.SizeOf(waveoutcaps)) == 0U;
						if (flag)
						{
							list.Add(new AudioOutDevice(i, waveoutcaps.szPname, (int)waveoutcaps.wChannels));
						}
					}
					return list.ToArray();
				}
			}

			// Token: 0x17000859 RID: 2137
			// (get) Token: 0x06001A16 RID: 6678 RVA: 0x000A0784 File Offset: 0x0009F784
			public bool IsDisposed
			{
				get
				{
					return this.m_IsDisposed;
				}
			}

			// Token: 0x1700085A RID: 2138
			// (get) Token: 0x06001A17 RID: 6679 RVA: 0x000A079C File Offset: 0x0009F79C
			public bool IsPlaying
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException("WaveOut");
					}
					return this.m_pPlayItems.Count > 0;
				}
			}

			// Token: 0x1700085B RID: 2139
			// (get) Token: 0x06001A18 RID: 6680 RVA: 0x000A07E0 File Offset: 0x0009F7E0
			// (set) Token: 0x06001A19 RID: 6681 RVA: 0x000A0820 File Offset: 0x0009F820
			public int Volume
			{
				get
				{
					int num = 0;
					AudioOut.WaveOut.WavMethods.waveOutGetVolume(this.m_pWavDevHandle, out num);
					ushort num2 = (ushort)(num & 65535);
					ushort num3 = (ushort)(num >> 16);
					return (int)((double)num2 / 655.35);
				}
				set
				{
					bool flag = value < 0 || value > 100;
					if (flag)
					{
						throw new ArgumentException("Property 'Volume' value must be >=0 and <= 100.");
					}
					int num = (int)((double)value * 655.35);
					AudioOut.WaveOut.WavMethods.waveOutSetVolume(this.m_pWavDevHandle, num << 16 | (num & 65535));
				}
			}

			// Token: 0x1700085C RID: 2140
			// (get) Token: 0x06001A1A RID: 6682 RVA: 0x000A0870 File Offset: 0x0009F870
			public int BytesBuffered
			{
				get
				{
					return this.m_BytesBuffered;
				}
			}

			// Token: 0x04000BB2 RID: 2994
			private AudioOutDevice m_pOutDevice = null;

			// Token: 0x04000BB3 RID: 2995
			private int m_SamplesPerSec = 8000;

			// Token: 0x04000BB4 RID: 2996
			private int m_BitsPerSample = 16;

			// Token: 0x04000BB5 RID: 2997
			private int m_Channels = 1;

			// Token: 0x04000BB6 RID: 2998
			private int m_MinBuffer = 1200;

			// Token: 0x04000BB7 RID: 2999
			private IntPtr m_pWavDevHandle = IntPtr.Zero;

			// Token: 0x04000BB8 RID: 3000
			private int m_BlockSize = 0;

			// Token: 0x04000BB9 RID: 3001
			private int m_BytesBuffered = 0;

			// Token: 0x04000BBA RID: 3002
			private bool m_IsPaused = false;

			// Token: 0x04000BBB RID: 3003
			private List<AudioOut.WaveOut.PlayItem> m_pPlayItems = null;

			// Token: 0x04000BBC RID: 3004
			private AudioOut.WaveOut.waveOutProc m_pWaveOutProc = null;

			// Token: 0x04000BBD RID: 3005
			private bool m_IsDisposed = false;

			// Token: 0x020003D3 RID: 979
			// (Invoke) Token: 0x06001C5A RID: 7258
			private delegate void waveOutProc(IntPtr hdrvr, int uMsg, int dwUser, int dwParam1, int dwParam2);

			// Token: 0x020003D4 RID: 980
			private class MMSYSERR
			{
				// Token: 0x04000DA9 RID: 3497
				public const int NOERROR = 0;

				// Token: 0x04000DAA RID: 3498
				public const int ERROR = 1;

				// Token: 0x04000DAB RID: 3499
				public const int BADDEVICEID = 2;

				// Token: 0x04000DAC RID: 3500
				public const int NOTENABLED = 3;

				// Token: 0x04000DAD RID: 3501
				public const int ALLOCATED = 4;

				// Token: 0x04000DAE RID: 3502
				public const int INVALHANDLE = 5;

				// Token: 0x04000DAF RID: 3503
				public const int NODRIVER = 6;

				// Token: 0x04000DB0 RID: 3504
				public const int NOMEM = 7;

				// Token: 0x04000DB1 RID: 3505
				public const int NOTSUPPORTED = 8;

				// Token: 0x04000DB2 RID: 3506
				public const int BADERRNUM = 9;

				// Token: 0x04000DB3 RID: 3507
				public const int INVALFLAG = 1;

				// Token: 0x04000DB4 RID: 3508
				public const int INVALPARAM = 11;

				// Token: 0x04000DB5 RID: 3509
				public const int HANDLEBUSY = 12;

				// Token: 0x04000DB6 RID: 3510
				public const int INVALIDALIAS = 13;

				// Token: 0x04000DB7 RID: 3511
				public const int BADDB = 14;

				// Token: 0x04000DB8 RID: 3512
				public const int KEYNOTFOUND = 15;

				// Token: 0x04000DB9 RID: 3513
				public const int READERROR = 16;

				// Token: 0x04000DBA RID: 3514
				public const int WRITEERROR = 17;

				// Token: 0x04000DBB RID: 3515
				public const int DELETEERROR = 18;

				// Token: 0x04000DBC RID: 3516
				public const int VALNOTFOUND = 19;

				// Token: 0x04000DBD RID: 3517
				public const int NODRIVERCB = 20;

				// Token: 0x04000DBE RID: 3518
				public const int LASTERROR = 20;
			}

			// Token: 0x020003D5 RID: 981
			private class WavConstants
			{
				// Token: 0x04000DBF RID: 3519
				public const int MM_WOM_OPEN = 955;

				// Token: 0x04000DC0 RID: 3520
				public const int MM_WOM_CLOSE = 956;

				// Token: 0x04000DC1 RID: 3521
				public const int MM_WOM_DONE = 957;

				// Token: 0x04000DC2 RID: 3522
				public const int MM_WIM_OPEN = 958;

				// Token: 0x04000DC3 RID: 3523
				public const int MM_WIM_CLOSE = 959;

				// Token: 0x04000DC4 RID: 3524
				public const int MM_WIM_DATA = 960;

				// Token: 0x04000DC5 RID: 3525
				public const int CALLBACK_FUNCTION = 196608;

				// Token: 0x04000DC6 RID: 3526
				public const int WAVERR_STILLPLAYING = 33;

				// Token: 0x04000DC7 RID: 3527
				public const int WHDR_DONE = 1;

				// Token: 0x04000DC8 RID: 3528
				public const int WHDR_PREPARED = 2;

				// Token: 0x04000DC9 RID: 3529
				public const int WHDR_BEGINLOOP = 4;

				// Token: 0x04000DCA RID: 3530
				public const int WHDR_ENDLOOP = 8;

				// Token: 0x04000DCB RID: 3531
				public const int WHDR_INQUEUE = 16;
			}

			// Token: 0x020003D6 RID: 982
			private class WavMethods
			{
				// Token: 0x06001C5F RID: 7263
				[DllImport("winmm.dll")]
				public static extern int waveOutClose(IntPtr hWaveOut);

				// Token: 0x06001C60 RID: 7264
				[DllImport("winmm.dll")]
				public static extern uint waveOutGetDevCaps(uint hwo, ref AudioOut.WaveOut.WAVEOUTCAPS pwoc, int cbwoc);

				// Token: 0x06001C61 RID: 7265
				[DllImport("winmm.dll")]
				public static extern int waveOutGetNumDevs();

				// Token: 0x06001C62 RID: 7266
				[DllImport("winmm.dll")]
				public static extern int waveOutGetPosition(IntPtr hWaveOut, out int lpInfo, int uSize);

				// Token: 0x06001C63 RID: 7267
				[DllImport("winmm.dll")]
				public static extern int waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

				// Token: 0x06001C64 RID: 7268
				[DllImport("winmm.dll")]
				public static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, AudioOut.WaveOut.WAVEFORMATEX lpFormat, AudioOut.WaveOut.waveOutProc dwCallback, int dwInstance, int dwFlags);

				// Token: 0x06001C65 RID: 7269
				[DllImport("winmm.dll")]
				public static extern int waveOutPause(IntPtr hWaveOut);

				// Token: 0x06001C66 RID: 7270
				[DllImport("winmm.dll")]
				public static extern int waveOutPrepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

				// Token: 0x06001C67 RID: 7271
				[DllImport("winmm.dll")]
				public static extern int waveOutReset(IntPtr hWaveOut);

				// Token: 0x06001C68 RID: 7272
				[DllImport("winmm.dll")]
				public static extern int waveOutRestart(IntPtr hWaveOut);

				// Token: 0x06001C69 RID: 7273
				[DllImport("winmm.dll")]
				public static extern int waveOutSetVolume(IntPtr hWaveOut, int dwVolume);

				// Token: 0x06001C6A RID: 7274
				[DllImport("winmm.dll")]
				public static extern int waveOutUnprepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

				// Token: 0x06001C6B RID: 7275
				[DllImport("winmm.dll")]
				public static extern int waveOutWrite(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);
			}

			// Token: 0x020003D7 RID: 983
			private struct WAVEOUTCAPS
			{
				// Token: 0x04000DCC RID: 3532
				public ushort wMid;

				// Token: 0x04000DCD RID: 3533
				public ushort wPid;

				// Token: 0x04000DCE RID: 3534
				public uint vDriverVersion;

				// Token: 0x04000DCF RID: 3535
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
				public string szPname;

				// Token: 0x04000DD0 RID: 3536
				public uint dwFormats;

				// Token: 0x04000DD1 RID: 3537
				public ushort wChannels;

				// Token: 0x04000DD2 RID: 3538
				public ushort wReserved1;

				// Token: 0x04000DD3 RID: 3539
				public uint dwSupport;
			}

			// Token: 0x020003D8 RID: 984
			[StructLayout(LayoutKind.Sequential)]
			private class WAVEFORMATEX
			{
				// Token: 0x04000DD4 RID: 3540
				public ushort wFormatTag;

				// Token: 0x04000DD5 RID: 3541
				public ushort nChannels;

				// Token: 0x04000DD6 RID: 3542
				public uint nSamplesPerSec;

				// Token: 0x04000DD7 RID: 3543
				public uint nAvgBytesPerSec;

				// Token: 0x04000DD8 RID: 3544
				public ushort nBlockAlign;

				// Token: 0x04000DD9 RID: 3545
				public ushort wBitsPerSample;

				// Token: 0x04000DDA RID: 3546
				public ushort cbSize;
			}

			// Token: 0x020003D9 RID: 985
			private struct WAVEHDR
			{
				// Token: 0x04000DDB RID: 3547
				public IntPtr lpData;

				// Token: 0x04000DDC RID: 3548
				public uint dwBufferLength;

				// Token: 0x04000DDD RID: 3549
				public uint dwBytesRecorded;

				// Token: 0x04000DDE RID: 3550
				public IntPtr dwUser;

				// Token: 0x04000DDF RID: 3551
				public uint dwFlags;

				// Token: 0x04000DE0 RID: 3552
				public uint dwLoops;

				// Token: 0x04000DE1 RID: 3553
				public IntPtr lpNext;

				// Token: 0x04000DE2 RID: 3554
				public uint reserved;
			}

			// Token: 0x020003DA RID: 986
			private class PlayItem
			{
				// Token: 0x06001C6E RID: 7278 RVA: 0x000AD610 File Offset: 0x000AC610
				public PlayItem(ref GCHandle headerHandle, ref GCHandle dataHandle, int dataSize)
				{
					this.m_HeaderHandle = headerHandle;
					this.m_DataHandle = dataHandle;
					this.m_DataSize = dataSize;
				}

				// Token: 0x06001C6F RID: 7279 RVA: 0x000AD640 File Offset: 0x000AC640
				public void Dispose()
				{
					this.m_HeaderHandle.Free();
					this.m_DataHandle.Free();
				}

				// Token: 0x170008AA RID: 2218
				// (get) Token: 0x06001C70 RID: 7280 RVA: 0x000AD65C File Offset: 0x000AC65C
				public GCHandle HeaderHandle
				{
					get
					{
						return this.m_HeaderHandle;
					}
				}

				// Token: 0x170008AB RID: 2219
				// (get) Token: 0x06001C71 RID: 7281 RVA: 0x000AD674 File Offset: 0x000AC674
				public AudioOut.WaveOut.WAVEHDR Header
				{
					get
					{
						return (AudioOut.WaveOut.WAVEHDR)this.m_HeaderHandle.Target;
					}
				}

				// Token: 0x170008AC RID: 2220
				// (get) Token: 0x06001C72 RID: 7282 RVA: 0x000AD698 File Offset: 0x000AC698
				public GCHandle DataHandle
				{
					get
					{
						return this.m_DataHandle;
					}
				}

				// Token: 0x170008AD RID: 2221
				// (get) Token: 0x06001C73 RID: 7283 RVA: 0x000AD6B0 File Offset: 0x000AC6B0
				public int DataSize
				{
					get
					{
						return this.m_DataSize;
					}
				}

				// Token: 0x04000DE3 RID: 3555
				private GCHandle m_HeaderHandle;

				// Token: 0x04000DE4 RID: 3556
				private GCHandle m_DataHandle;

				// Token: 0x04000DE5 RID: 3557
				private int m_DataSize = 0;
			}
		}
	}
}
