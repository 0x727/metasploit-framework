using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.Media
{
	// Token: 0x02000151 RID: 337
	public class AudioIn : Stream
	{
		// Token: 0x06000DB4 RID: 3508 RVA: 0x00055CF4 File Offset: 0x00054CF4
		public AudioIn(AudioInDevice device, int samplesPerSec, int bitsPerSample, int channels)
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
			this.m_SamplesPerSec = samplesPerSec;
			this.m_BitsPerSample = bitsPerSample;
			this.m_Channels = channels;
			this.m_pWaveIn = new AudioIn.WaveIn(device, samplesPerSec, bitsPerSample, channels, 320);
			this.m_pWaveIn.Start();
		}

		// Token: 0x06000DB5 RID: 3509 RVA: 0x00055DD8 File Offset: 0x00054DD8
		public new void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.m_pWaveIn.Dispose();
				this.m_pWaveIn = null;
			}
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x00055E10 File Offset: 0x00054E10
		public override void Flush()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("Base64Stream");
			}
		}

		// Token: 0x06000DB7 RID: 3511 RVA: 0x00055E34 File Offset: 0x00054E34
		public override long Seek(long offset, SeekOrigin origin)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("Base64Stream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x06000DB8 RID: 3512 RVA: 0x00055E60 File Offset: 0x00054E60
		public override void SetLength(long value)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("Base64Stream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x00055E8C File Offset: 0x00054E8C
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' value must be >= 0.");
			}
			bool flag4 = offset + count > buffer.Length;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' is bigger than than argument 'buffer' can store.");
			}
			while (this.m_pWaveIn.ReadBuffer.Available == 0)
			{
				Thread.Sleep(1);
			}
			return this.m_pWaveIn.ReadBuffer.Read(buffer, offset, count - count % this.m_pWaveIn.BlockSize);
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x00055F48 File Offset: 0x00054F48
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06000DBB RID: 3515 RVA: 0x00055F74 File Offset: 0x00054F74
		public static AudioInDevice[] Devices
		{
			get
			{
				return AudioIn.WaveIn.Devices;
			}
		}

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06000DBC RID: 3516 RVA: 0x00055F8C File Offset: 0x00054F8C
		public override bool CanRead
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return true;
			}
		}

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06000DBD RID: 3517 RVA: 0x00055FB8 File Offset: 0x00054FB8
		public override bool CanSeek
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return false;
			}
		}

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06000DBE RID: 3518 RVA: 0x00055FE4 File Offset: 0x00054FE4
		public override bool CanWrite
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return false;
			}
		}

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06000DBF RID: 3519 RVA: 0x00056010 File Offset: 0x00055010
		public override long Length
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				throw new NotSupportedException();
			}
		}

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06000DC0 RID: 3520 RVA: 0x0005603C File Offset: 0x0005503C
		// (set) Token: 0x06000DC1 RID: 3521 RVA: 0x00056068 File Offset: 0x00055068
		public override long Position
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				throw new NotSupportedException();
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				throw new NotSupportedException();
			}
		}

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06000DC2 RID: 3522 RVA: 0x00056094 File Offset: 0x00055094
		public int SamplesPerSec
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SamplesPerSec;
			}
		}

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x06000DC3 RID: 3523 RVA: 0x000560C8 File Offset: 0x000550C8
		public int BitsPerSample
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_BitsPerSample;
			}
		}

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06000DC4 RID: 3524 RVA: 0x000560FC File Offset: 0x000550FC
		public int Channels
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Channels;
			}
		}

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06000DC5 RID: 3525 RVA: 0x00056130 File Offset: 0x00055130
		public int BlockSize
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Channels * (this.m_BitsPerSample / 8);
			}
		}

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06000DC6 RID: 3526 RVA: 0x00056170 File Offset: 0x00055170
		public int Available
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pWaveIn.ReadBuffer.Available;
			}
		}

		// Token: 0x040005C2 RID: 1474
		private bool m_IsDisposed = false;

		// Token: 0x040005C3 RID: 1475
		private AudioInDevice m_pDevice = null;

		// Token: 0x040005C4 RID: 1476
		private int m_SamplesPerSec = 8000;

		// Token: 0x040005C5 RID: 1477
		private int m_BitsPerSample = 16;

		// Token: 0x040005C6 RID: 1478
		private int m_Channels = 1;

		// Token: 0x040005C7 RID: 1479
		private AudioIn.WaveIn m_pWaveIn = null;

		// Token: 0x0200030F RID: 783
		private class WaveIn
		{
			// Token: 0x060019F4 RID: 6644
			[DllImport("winmm.dll")]
			private static extern int waveInAddBuffer(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

			// Token: 0x060019F5 RID: 6645
			[DllImport("winmm.dll")]
			private static extern int waveInClose(IntPtr hWaveOut);

			// Token: 0x060019F6 RID: 6646
			[DllImport("winmm.dll")]
			private static extern uint waveInGetDevCaps(uint hwo, ref AudioIn.WaveIn.WAVEOUTCAPS pwoc, int cbwoc);

			// Token: 0x060019F7 RID: 6647
			[DllImport("winmm.dll")]
			private static extern int waveInGetNumDevs();

			// Token: 0x060019F8 RID: 6648
			[DllImport("winmm.dll")]
			private static extern int waveInOpen(out IntPtr hWaveOut, int uDeviceID, AudioIn.WaveIn.WAVEFORMATEX lpFormat, AudioIn.WaveIn.waveInProc dwCallback, int dwInstance, int dwFlags);

			// Token: 0x060019F9 RID: 6649
			[DllImport("winmm.dll")]
			private static extern int waveInPrepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

			// Token: 0x060019FA RID: 6650
			[DllImport("winmm.dll")]
			private static extern int waveInReset(IntPtr hWaveOut);

			// Token: 0x060019FB RID: 6651
			[DllImport("winmm.dll")]
			private static extern int waveInStart(IntPtr hWaveOut);

			// Token: 0x060019FC RID: 6652
			[DllImport("winmm.dll")]
			private static extern int waveInStop(IntPtr hWaveOut);

			// Token: 0x060019FD RID: 6653
			[DllImport("winmm.dll")]
			private static extern int waveInUnprepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

			// Token: 0x060019FE RID: 6654 RVA: 0x0009F894 File Offset: 0x0009E894
			public WaveIn(AudioInDevice device, int samplesPerSec, int bitsPerSample, int channels, int bufferSize)
			{
				bool flag = device == null;
				if (flag)
				{
					throw new ArgumentNullException("device");
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
				this.m_pInDevice = device;
				this.m_SamplesPerSec = samplesPerSec;
				this.m_BitsPerSample = bitsPerSample;
				this.m_Channels = channels;
				this.m_BufferSize = bufferSize;
				this.m_BlockSize = this.m_Channels * (this.m_BitsPerSample / 8);
				this.m_pBuffers = new CircleCollection<AudioIn.WaveIn.BufferItem>();
				this.m_pReadBuffer = new FifoBuffer(32000);
				AudioIn.WaveIn.WAVEFORMATEX waveformatex = new AudioIn.WaveIn.WAVEFORMATEX();
				waveformatex.wFormatTag = 1;
				waveformatex.nChannels = (ushort)this.m_Channels;
				waveformatex.nSamplesPerSec = (uint)samplesPerSec;
				waveformatex.nAvgBytesPerSec = (uint)(this.m_SamplesPerSec * this.m_Channels * (this.m_BitsPerSample / 8));
				waveformatex.nBlockAlign = (ushort)this.m_BlockSize;
				waveformatex.wBitsPerSample = (ushort)this.m_BitsPerSample;
				waveformatex.cbSize = 0;
				this.m_pWaveInProc = new AudioIn.WaveIn.waveInProc(this.OnWaveInProc);
				int num = AudioIn.WaveIn.waveInOpen(out this.m_pWavDevHandle, this.m_pInDevice.Index, waveformatex, this.m_pWaveInProc, 0, 196608);
				bool flag5 = num != 0;
				if (flag5)
				{
					throw new Exception("Failed to open wav device, error: " + num.ToString() + ".");
				}
				this.CreateBuffers();
			}

			// Token: 0x060019FF RID: 6655 RVA: 0x0009FA8C File Offset: 0x0009EA8C
			~WaveIn()
			{
				this.Dispose();
			}

			// Token: 0x06001A00 RID: 6656 RVA: 0x0009FABC File Offset: 0x0009EABC
			public void Dispose()
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					this.m_IsDisposed = true;
					try
					{
						AudioIn.WaveIn.waveInReset(this.m_pWavDevHandle);
						foreach (AudioIn.WaveIn.BufferItem bufferItem in this.m_pBuffers.ToArray())
						{
							AudioIn.WaveIn.waveInUnprepareHeader(this.m_pWavDevHandle, bufferItem.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(bufferItem.Header));
							bufferItem.Dispose();
						}
						AudioIn.WaveIn.waveInClose(this.m_pWavDevHandle);
						this.m_pInDevice = null;
						this.m_pWavDevHandle = IntPtr.Zero;
					}
					catch
					{
					}
				}
			}

			// Token: 0x06001A01 RID: 6657 RVA: 0x0009FB78 File Offset: 0x0009EB78
			public void Start()
			{
				bool isRecording = this.m_IsRecording;
				if (!isRecording)
				{
					this.m_IsRecording = true;
					int num = AudioIn.WaveIn.waveInStart(this.m_pWavDevHandle);
					bool flag = num != 0;
					if (flag)
					{
						throw new Exception("Failed to start wav device, error: " + num + ".");
					}
				}
			}

			// Token: 0x06001A02 RID: 6658 RVA: 0x0009FBCC File Offset: 0x0009EBCC
			public void Stop()
			{
				bool flag = !this.m_IsRecording;
				if (!flag)
				{
					this.m_IsRecording = false;
					int num = AudioIn.WaveIn.waveInStop(this.m_pWavDevHandle);
					bool flag2 = num != 0;
					if (flag2)
					{
						throw new Exception("Failed to stop wav device, error: " + num + ".");
					}
				}
			}

			// Token: 0x06001A03 RID: 6659 RVA: 0x0009FC24 File Offset: 0x0009EC24
			private void OnWaveInProc(IntPtr hdrvr, int uMsg, int dwUser, int dwParam1, int dwParam2)
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						try
						{
							bool flag2 = uMsg == 960;
							if (flag2)
							{
								this.m_pReadBuffer.Write(this.m_pCurrentBuffer.Data, 0, this.m_pCurrentBuffer.Data.Length, true);
								AudioIn.WaveIn.BufferItem buffer = this.m_pCurrentBuffer;
								this.m_pCurrentBuffer = this.m_pBuffers.Next();
								ThreadPool.QueueUserWorkItem(delegate(object state)
								{
									try
									{
										bool isDisposed2 = this.m_IsDisposed;
										if (!isDisposed2)
										{
											AudioIn.WaveIn.waveInUnprepareHeader(this.m_pWavDevHandle, buffer.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(buffer.Header));
											AudioIn.WaveIn.waveInPrepareHeader(this.m_pWavDevHandle, buffer.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(buffer.Header));
											AudioIn.WaveIn.waveInAddBuffer(this.m_pWavDevHandle, buffer.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(buffer.Header));
										}
									}
									catch
									{
									}
								});
							}
						}
						catch
						{
						}
					}
				}
			}

			// Token: 0x06001A04 RID: 6660 RVA: 0x0009FCFC File Offset: 0x0009ECFC
			private void ProcessActiveBuffer(object state)
			{
				try
				{
					CircleCollection<AudioIn.WaveIn.BufferItem> pBuffers = this.m_pBuffers;
					lock (pBuffers)
					{
						AudioIn.WaveIn.waveInUnprepareHeader(this.m_pWavDevHandle, this.m_pCurrentBuffer.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(this.m_pCurrentBuffer.Header));
						AudioIn.WaveIn.waveInPrepareHeader(this.m_pWavDevHandle, this.m_pCurrentBuffer.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(this.m_pCurrentBuffer.Header));
						AudioIn.WaveIn.waveInAddBuffer(this.m_pWavDevHandle, this.m_pCurrentBuffer.HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(this.m_pCurrentBuffer.Header));
					}
				}
				catch
				{
				}
			}

			// Token: 0x06001A05 RID: 6661 RVA: 0x0009FDEC File Offset: 0x0009EDEC
			private void CreateBuffers()
			{
				while (this.m_pBuffers.Count < 10)
				{
					byte[] array = new byte[this.m_BufferSize];
					GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
					AudioIn.WaveIn.WAVEHDR wavehdr = new AudioIn.WaveIn.WAVEHDR
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
					int num = AudioIn.WaveIn.waveInPrepareHeader(this.m_pWavDevHandle, gchandle2.AddrOfPinnedObject(), Marshal.SizeOf(wavehdr));
					bool flag = num != 0;
					if (flag)
					{
						throw new Exception("Error preparing wave in buffer, error: " + num + ".");
					}
					this.m_pBuffers.Add(new AudioIn.WaveIn.BufferItem(ref gchandle2, ref gchandle, this.m_BufferSize));
					num = AudioIn.WaveIn.waveInAddBuffer(this.m_pWavDevHandle, gchandle2.AddrOfPinnedObject(), Marshal.SizeOf(wavehdr));
					bool flag2 = num != 0;
					if (flag2)
					{
						throw new Exception("Error adding wave in buffer, error: " + num + ".");
					}
				}
				this.m_pCurrentBuffer = this.m_pBuffers[0];
			}

			// Token: 0x1700084F RID: 2127
			// (get) Token: 0x06001A06 RID: 6662 RVA: 0x0009FF54 File Offset: 0x0009EF54
			public static AudioInDevice[] Devices
			{
				get
				{
					List<AudioInDevice> list = new List<AudioInDevice>();
					int num = AudioIn.WaveIn.waveInGetNumDevs();
					for (int i = 0; i < num; i++)
					{
						AudioIn.WaveIn.WAVEOUTCAPS waveoutcaps = default(AudioIn.WaveIn.WAVEOUTCAPS);
						bool flag = AudioIn.WaveIn.waveInGetDevCaps((uint)i, ref waveoutcaps, Marshal.SizeOf(waveoutcaps)) == 0U;
						if (flag)
						{
							list.Add(new AudioInDevice(i, waveoutcaps.szPname, (int)waveoutcaps.wChannels));
						}
					}
					return list.ToArray();
				}
			}

			// Token: 0x17000850 RID: 2128
			// (get) Token: 0x06001A07 RID: 6663 RVA: 0x0009FFD0 File Offset: 0x0009EFD0
			public bool IsDisposed
			{
				get
				{
					return this.m_IsDisposed;
				}
			}

			// Token: 0x17000851 RID: 2129
			// (get) Token: 0x06001A08 RID: 6664 RVA: 0x0009FFE8 File Offset: 0x0009EFE8
			public AudioInDevice InputDevice
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException("WavRecorder");
					}
					return this.m_pInDevice;
				}
			}

			// Token: 0x17000852 RID: 2130
			// (get) Token: 0x06001A09 RID: 6665 RVA: 0x000A0018 File Offset: 0x0009F018
			public int SamplesPerSec
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException("WavRecorder");
					}
					return this.m_SamplesPerSec;
				}
			}

			// Token: 0x17000853 RID: 2131
			// (get) Token: 0x06001A0A RID: 6666 RVA: 0x000A0048 File Offset: 0x0009F048
			public int BitsPerSample
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException("WavRecorder");
					}
					return this.m_BitsPerSample;
				}
			}

			// Token: 0x17000854 RID: 2132
			// (get) Token: 0x06001A0B RID: 6667 RVA: 0x000A0078 File Offset: 0x0009F078
			public int Channels
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException("WavRecorder");
					}
					return this.m_Channels;
				}
			}

			// Token: 0x17000855 RID: 2133
			// (get) Token: 0x06001A0C RID: 6668 RVA: 0x000A00A8 File Offset: 0x0009F0A8
			public int BufferSize
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException("WavRecorder");
					}
					return this.m_BufferSize;
				}
			}

			// Token: 0x17000856 RID: 2134
			// (get) Token: 0x06001A0D RID: 6669 RVA: 0x000A00D8 File Offset: 0x0009F0D8
			public int BlockSize
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException("WavRecorder");
					}
					return this.m_BlockSize;
				}
			}

			// Token: 0x17000857 RID: 2135
			// (get) Token: 0x06001A0E RID: 6670 RVA: 0x000A0108 File Offset: 0x0009F108
			public FifoBuffer ReadBuffer
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException("WavRecorder");
					}
					return this.m_pReadBuffer;
				}
			}

			// Token: 0x04000BA4 RID: 2980
			private bool m_IsDisposed = false;

			// Token: 0x04000BA5 RID: 2981
			private AudioInDevice m_pInDevice = null;

			// Token: 0x04000BA6 RID: 2982
			private int m_SamplesPerSec = 8000;

			// Token: 0x04000BA7 RID: 2983
			private int m_BitsPerSample = 8;

			// Token: 0x04000BA8 RID: 2984
			private int m_Channels = 1;

			// Token: 0x04000BA9 RID: 2985
			private int m_BufferSize = 400;

			// Token: 0x04000BAA RID: 2986
			private IntPtr m_pWavDevHandle = IntPtr.Zero;

			// Token: 0x04000BAB RID: 2987
			private int m_BlockSize = 0;

			// Token: 0x04000BAC RID: 2988
			private AudioIn.WaveIn.BufferItem m_pCurrentBuffer = null;

			// Token: 0x04000BAD RID: 2989
			private CircleCollection<AudioIn.WaveIn.BufferItem> m_pBuffers = null;

			// Token: 0x04000BAE RID: 2990
			private AudioIn.WaveIn.waveInProc m_pWaveInProc = null;

			// Token: 0x04000BAF RID: 2991
			private bool m_IsRecording = false;

			// Token: 0x04000BB0 RID: 2992
			private FifoBuffer m_pReadBuffer = null;

			// Token: 0x04000BB1 RID: 2993
			private object m_pLock = new object();

			// Token: 0x020003CA RID: 970
			// (Invoke) Token: 0x06001C49 RID: 7241
			private delegate void waveInProc(IntPtr hdrvr, int uMsg, int dwUser, int dwParam1, int dwParam2);

			// Token: 0x020003CB RID: 971
			private class BufferItem
			{
				// Token: 0x06001C4C RID: 7244 RVA: 0x000AD44C File Offset: 0x000AC44C
				public BufferItem(ref GCHandle headerHandle, ref GCHandle dataHandle, int dataSize)
				{
					this.m_HeaderHandle = headerHandle;
					this.m_DataHandle = dataHandle;
					this.m_DataSize = dataSize;
				}

				// Token: 0x06001C4D RID: 7245 RVA: 0x000AD47C File Offset: 0x000AC47C
				public void Dispose()
				{
					this.m_HeaderHandle.Free();
					this.m_DataHandle.Free();
				}

				// Token: 0x170008A5 RID: 2213
				// (get) Token: 0x06001C4E RID: 7246 RVA: 0x000AD498 File Offset: 0x000AC498
				public GCHandle HeaderHandle
				{
					get
					{
						return this.m_HeaderHandle;
					}
				}

				// Token: 0x170008A6 RID: 2214
				// (get) Token: 0x06001C4F RID: 7247 RVA: 0x000AD4B0 File Offset: 0x000AC4B0
				public AudioIn.WaveIn.WAVEHDR Header
				{
					get
					{
						return (AudioIn.WaveIn.WAVEHDR)this.m_HeaderHandle.Target;
					}
				}

				// Token: 0x170008A7 RID: 2215
				// (get) Token: 0x06001C50 RID: 7248 RVA: 0x000AD4D4 File Offset: 0x000AC4D4
				public GCHandle DataHandle
				{
					get
					{
						return this.m_DataHandle;
					}
				}

				// Token: 0x170008A8 RID: 2216
				// (get) Token: 0x06001C51 RID: 7249 RVA: 0x000AD4EC File Offset: 0x000AC4EC
				public byte[] Data
				{
					get
					{
						return (byte[])this.m_DataHandle.Target;
					}
				}

				// Token: 0x170008A9 RID: 2217
				// (get) Token: 0x06001C52 RID: 7250 RVA: 0x000AD510 File Offset: 0x000AC510
				public int DataSize
				{
					get
					{
						return this.m_DataSize;
					}
				}

				// Token: 0x04000D69 RID: 3433
				private GCHandle m_HeaderHandle;

				// Token: 0x04000D6A RID: 3434
				private GCHandle m_DataHandle;

				// Token: 0x04000D6B RID: 3435
				private int m_DataSize = 0;
			}

			// Token: 0x020003CC RID: 972
			private class MMSYSERR
			{
				// Token: 0x04000D6C RID: 3436
				public const int NOERROR = 0;

				// Token: 0x04000D6D RID: 3437
				public const int ERROR = 1;

				// Token: 0x04000D6E RID: 3438
				public const int BADDEVICEID = 2;

				// Token: 0x04000D6F RID: 3439
				public const int NOTENABLED = 3;

				// Token: 0x04000D70 RID: 3440
				public const int ALLOCATED = 4;

				// Token: 0x04000D71 RID: 3441
				public const int INVALHANDLE = 5;

				// Token: 0x04000D72 RID: 3442
				public const int NODRIVER = 6;

				// Token: 0x04000D73 RID: 3443
				public const int NOMEM = 7;

				// Token: 0x04000D74 RID: 3444
				public const int NOTSUPPORTED = 8;

				// Token: 0x04000D75 RID: 3445
				public const int BADERRNUM = 9;

				// Token: 0x04000D76 RID: 3446
				public const int INVALFLAG = 1;

				// Token: 0x04000D77 RID: 3447
				public const int INVALPARAM = 11;

				// Token: 0x04000D78 RID: 3448
				public const int HANDLEBUSY = 12;

				// Token: 0x04000D79 RID: 3449
				public const int INVALIDALIAS = 13;

				// Token: 0x04000D7A RID: 3450
				public const int BADDB = 14;

				// Token: 0x04000D7B RID: 3451
				public const int KEYNOTFOUND = 15;

				// Token: 0x04000D7C RID: 3452
				public const int READERROR = 16;

				// Token: 0x04000D7D RID: 3453
				public const int WRITEERROR = 17;

				// Token: 0x04000D7E RID: 3454
				public const int DELETEERROR = 18;

				// Token: 0x04000D7F RID: 3455
				public const int VALNOTFOUND = 19;

				// Token: 0x04000D80 RID: 3456
				public const int NODRIVERCB = 20;

				// Token: 0x04000D81 RID: 3457
				public const int LASTERROR = 20;
			}

			// Token: 0x020003CD RID: 973
			private class WavConstants
			{
				// Token: 0x04000D82 RID: 3458
				public const int MM_WOM_OPEN = 955;

				// Token: 0x04000D83 RID: 3459
				public const int MM_WOM_CLOSE = 956;

				// Token: 0x04000D84 RID: 3460
				public const int MM_WOM_DONE = 957;

				// Token: 0x04000D85 RID: 3461
				public const int MM_WIM_OPEN = 958;

				// Token: 0x04000D86 RID: 3462
				public const int MM_WIM_CLOSE = 959;

				// Token: 0x04000D87 RID: 3463
				public const int MM_WIM_DATA = 960;

				// Token: 0x04000D88 RID: 3464
				public const int CALLBACK_FUNCTION = 196608;

				// Token: 0x04000D89 RID: 3465
				public const int WAVERR_STILLPLAYING = 33;

				// Token: 0x04000D8A RID: 3466
				public const int WHDR_DONE = 1;

				// Token: 0x04000D8B RID: 3467
				public const int WHDR_PREPARED = 2;

				// Token: 0x04000D8C RID: 3468
				public const int WHDR_BEGINLOOP = 4;

				// Token: 0x04000D8D RID: 3469
				public const int WHDR_ENDLOOP = 8;

				// Token: 0x04000D8E RID: 3470
				public const int WHDR_INQUEUE = 16;
			}

			// Token: 0x020003CE RID: 974
			[StructLayout(LayoutKind.Sequential)]
			private class WAVEFORMATEX
			{
				// Token: 0x04000D8F RID: 3471
				public ushort wFormatTag;

				// Token: 0x04000D90 RID: 3472
				public ushort nChannels;

				// Token: 0x04000D91 RID: 3473
				public uint nSamplesPerSec;

				// Token: 0x04000D92 RID: 3474
				public uint nAvgBytesPerSec;

				// Token: 0x04000D93 RID: 3475
				public ushort nBlockAlign;

				// Token: 0x04000D94 RID: 3476
				public ushort wBitsPerSample;

				// Token: 0x04000D95 RID: 3477
				public ushort cbSize;
			}

			// Token: 0x020003CF RID: 975
			private struct WAVEOUTCAPS
			{
				// Token: 0x04000D96 RID: 3478
				public ushort wMid;

				// Token: 0x04000D97 RID: 3479
				public ushort wPid;

				// Token: 0x04000D98 RID: 3480
				public uint vDriverVersion;

				// Token: 0x04000D99 RID: 3481
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
				public string szPname;

				// Token: 0x04000D9A RID: 3482
				public uint dwFormats;

				// Token: 0x04000D9B RID: 3483
				public ushort wChannels;

				// Token: 0x04000D9C RID: 3484
				public ushort wReserved1;

				// Token: 0x04000D9D RID: 3485
				public uint dwSupport;
			}

			// Token: 0x020003D0 RID: 976
			private struct WAVEHDR
			{
				// Token: 0x04000D9E RID: 3486
				public IntPtr lpData;

				// Token: 0x04000D9F RID: 3487
				public uint dwBufferLength;

				// Token: 0x04000DA0 RID: 3488
				public uint dwBytesRecorded;

				// Token: 0x04000DA1 RID: 3489
				public IntPtr dwUser;

				// Token: 0x04000DA2 RID: 3490
				public uint dwFlags;

				// Token: 0x04000DA3 RID: 3491
				public uint dwLoops;

				// Token: 0x04000DA4 RID: 3492
				public IntPtr lpNext;

				// Token: 0x04000DA5 RID: 3493
				public uint reserved;
			}

			// Token: 0x020003D1 RID: 977
			internal class WavFormat
			{
				// Token: 0x04000DA6 RID: 3494
				public const int PCM = 1;
			}
		}
	}
}
