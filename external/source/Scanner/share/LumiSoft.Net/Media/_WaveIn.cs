using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LumiSoft.Net.Media
{
	// Token: 0x0200014C RID: 332
	internal class _WaveIn
	{
		// Token: 0x06000D6F RID: 3439
		[DllImport("winmm.dll")]
		private static extern int waveInAddBuffer(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

		// Token: 0x06000D70 RID: 3440
		[DllImport("winmm.dll")]
		private static extern int waveInClose(IntPtr hWaveOut);

		// Token: 0x06000D71 RID: 3441
		[DllImport("winmm.dll")]
		private static extern uint waveInGetDevCaps(uint hwo, ref _WaveIn.WAVEOUTCAPS pwoc, int cbwoc);

		// Token: 0x06000D72 RID: 3442
		[DllImport("winmm.dll")]
		private static extern int waveInGetNumDevs();

		// Token: 0x06000D73 RID: 3443
		[DllImport("winmm.dll")]
		private static extern int waveInOpen(out IntPtr hWaveOut, int uDeviceID, _WaveIn.WAVEFORMATEX lpFormat, _WaveIn.waveInProc dwCallback, int dwInstance, int dwFlags);

		// Token: 0x06000D74 RID: 3444
		[DllImport("winmm.dll")]
		private static extern int waveInPrepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

		// Token: 0x06000D75 RID: 3445
		[DllImport("winmm.dll")]
		private static extern int waveInReset(IntPtr hWaveOut);

		// Token: 0x06000D76 RID: 3446
		[DllImport("winmm.dll")]
		private static extern int waveInStart(IntPtr hWaveOut);

		// Token: 0x06000D77 RID: 3447
		[DllImport("winmm.dll")]
		private static extern int waveInStop(IntPtr hWaveOut);

		// Token: 0x06000D78 RID: 3448
		[DllImport("winmm.dll")]
		private static extern int waveInUnprepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

		// Token: 0x06000D79 RID: 3449 RVA: 0x00054860 File Offset: 0x00053860
		public _WaveIn(AudioInDevice device, int samplesPerSec, int bitsPerSample, int channels, int bufferSize)
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
			this.m_pBuffers = new Dictionary<long, _WaveIn.BufferItem>();
			_WaveIn.WAVEFORMATEX waveformatex = new _WaveIn.WAVEFORMATEX();
			waveformatex.wFormatTag = 1;
			waveformatex.nChannels = (ushort)this.m_Channels;
			waveformatex.nSamplesPerSec = (uint)samplesPerSec;
			waveformatex.nAvgBytesPerSec = (uint)(this.m_SamplesPerSec * (this.m_Channels * (this.m_BitsPerSample / 8)));
			waveformatex.nBlockAlign = (ushort)this.m_BlockSize;
			waveformatex.wBitsPerSample = (ushort)this.m_BitsPerSample;
			waveformatex.cbSize = 0;
			this.m_pWaveInProc = new _WaveIn.waveInProc(this.OnWaveInProc);
			int num = _WaveIn.waveInOpen(out this.m_pWavDevHandle, this.m_pInDevice.Index, waveformatex, this.m_pWaveInProc, 0, 196608);
			bool flag5 = num != 0;
			if (flag5)
			{
				throw new Exception("Failed to open wav device, error: " + num.ToString() + ".");
			}
			this.CreateBuffers();
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x00054A40 File Offset: 0x00053A40
		~_WaveIn()
		{
			this.Dispose();
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x00054A70 File Offset: 0x00053A70
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				try
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						_WaveIn.waveInReset(this.m_pWavDevHandle);
						foreach (_WaveIn.BufferItem bufferItem in this.m_pBuffers.Values)
						{
							bufferItem.Dispose();
						}
						_WaveIn.waveInClose(this.m_pWavDevHandle);
						this.m_pInDevice = null;
						this.m_pWavDevHandle = IntPtr.Zero;
						this.AudioFrameReceived = null;
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x00054B5C File Offset: 0x00053B5C
		public void Start()
		{
			bool isRecording = this.m_IsRecording;
			if (!isRecording)
			{
				this.m_IsRecording = true;
				int num = _WaveIn.waveInStart(this.m_pWavDevHandle);
				bool flag = num != 0;
				if (flag)
				{
					throw new Exception("Failed to start wav device, error: " + num + ".");
				}
			}
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x00054BB0 File Offset: 0x00053BB0
		public void Stop()
		{
			bool flag = !this.m_IsRecording;
			if (!flag)
			{
				this.m_IsRecording = false;
				int num = _WaveIn.waveInStop(this.m_pWavDevHandle);
				bool flag2 = num != 0;
				if (flag2)
				{
					throw new Exception("Failed to stop wav device, error: " + num + ".");
				}
			}
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x00054C08 File Offset: 0x00053C08
		private void OnWaveInProc(IntPtr hdrvr, int uMsg, IntPtr dwUser, IntPtr dwParam1, IntPtr dwParam2)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				try
				{
					bool flag = uMsg == 960;
					if (flag)
					{
						try
						{
							bool isDisposed2 = this.m_IsDisposed;
							if (!isDisposed2)
							{
								object pLock = this.m_pLock;
								lock (pLock)
								{
									_WaveIn.BufferItem bufferItem = this.m_pBuffers[dwParam1.ToInt64()];
									this.OnAudioFrameReceived(bufferItem.EventArgs);
									bufferItem.Queue(true);
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x00054CD8 File Offset: 0x00053CD8
		private void CreateBuffers()
		{
			while (this.m_pBuffers.Count < 10)
			{
				_WaveIn.BufferItem bufferItem = new _WaveIn.BufferItem(this.m_pWavDevHandle, this.m_BufferSize);
				this.m_pBuffers.Add(bufferItem.HeaderHandle.AddrOfPinnedObject().ToInt64(), bufferItem);
				bufferItem.Queue(false);
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06000D80 RID: 3456 RVA: 0x00054D3C File Offset: 0x00053D3C
		public static AudioInDevice[] Devices
		{
			get
			{
				List<AudioInDevice> list = new List<AudioInDevice>();
				int num = _WaveIn.waveInGetNumDevs();
				for (int i = 0; i < num; i++)
				{
					_WaveIn.WAVEOUTCAPS waveoutcaps = default(_WaveIn.WAVEOUTCAPS);
					bool flag = _WaveIn.waveInGetDevCaps((uint)i, ref waveoutcaps, Marshal.SizeOf(waveoutcaps)) == 0U;
					if (flag)
					{
						list.Add(new AudioInDevice(i, waveoutcaps.szPname, (int)waveoutcaps.wChannels));
					}
				}
				return list.ToArray();
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06000D81 RID: 3457 RVA: 0x00054DB8 File Offset: 0x00053DB8
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06000D82 RID: 3458 RVA: 0x00054DD0 File Offset: 0x00053DD0
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

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06000D83 RID: 3459 RVA: 0x00054E00 File Offset: 0x00053E00
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

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06000D84 RID: 3460 RVA: 0x00054E30 File Offset: 0x00053E30
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

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06000D85 RID: 3461 RVA: 0x00054E60 File Offset: 0x00053E60
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

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06000D86 RID: 3462 RVA: 0x00054E90 File Offset: 0x00053E90
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

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06000D87 RID: 3463 RVA: 0x00054EC0 File Offset: 0x00053EC0
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

		// Token: 0x1400005A RID: 90
		// (add) Token: 0x06000D88 RID: 3464 RVA: 0x00054EF0 File Offset: 0x00053EF0
		// (remove) Token: 0x06000D89 RID: 3465 RVA: 0x00054F28 File Offset: 0x00053F28
		
		public event EventHandler<EventArgs<byte[]>> AudioFrameReceived = null;

		// Token: 0x06000D8A RID: 3466 RVA: 0x00054F60 File Offset: 0x00053F60
		private void OnAudioFrameReceived(EventArgs<byte[]> eArgs)
		{
			bool flag = this.AudioFrameReceived != null;
			if (flag)
			{
				this.AudioFrameReceived(this, eArgs);
			}
		}

		// Token: 0x0400059D RID: 1437
		private object m_pLock = new object();

		// Token: 0x0400059E RID: 1438
		private bool m_IsDisposed = false;

		// Token: 0x0400059F RID: 1439
		private AudioInDevice m_pInDevice = null;

		// Token: 0x040005A0 RID: 1440
		private int m_SamplesPerSec = 8000;

		// Token: 0x040005A1 RID: 1441
		private int m_BitsPerSample = 8;

		// Token: 0x040005A2 RID: 1442
		private int m_Channels = 1;

		// Token: 0x040005A3 RID: 1443
		private int m_BufferSize = 400;

		// Token: 0x040005A4 RID: 1444
		private IntPtr m_pWavDevHandle = IntPtr.Zero;

		// Token: 0x040005A5 RID: 1445
		private int m_BlockSize = 0;

		// Token: 0x040005A6 RID: 1446
		private Dictionary<long, _WaveIn.BufferItem> m_pBuffers = null;

		// Token: 0x040005A7 RID: 1447
		private _WaveIn.waveInProc m_pWaveInProc = null;

		// Token: 0x040005A8 RID: 1448
		private bool m_IsRecording = false;

		// Token: 0x02000302 RID: 770
		// (Invoke) Token: 0x060019C8 RID: 6600
		private delegate void waveInProc(IntPtr hdrvr, int uMsg, IntPtr dwUser, IntPtr dwParam1, IntPtr dwParam2);

		// Token: 0x02000303 RID: 771
		private class MMSYSERR
		{
			// Token: 0x04000B52 RID: 2898
			public const int NOERROR = 0;

			// Token: 0x04000B53 RID: 2899
			public const int ERROR = 1;

			// Token: 0x04000B54 RID: 2900
			public const int BADDEVICEID = 2;

			// Token: 0x04000B55 RID: 2901
			public const int NOTENABLED = 3;

			// Token: 0x04000B56 RID: 2902
			public const int ALLOCATED = 4;

			// Token: 0x04000B57 RID: 2903
			public const int INVALHANDLE = 5;

			// Token: 0x04000B58 RID: 2904
			public const int NODRIVER = 6;

			// Token: 0x04000B59 RID: 2905
			public const int NOMEM = 7;

			// Token: 0x04000B5A RID: 2906
			public const int NOTSUPPORTED = 8;

			// Token: 0x04000B5B RID: 2907
			public const int BADERRNUM = 9;

			// Token: 0x04000B5C RID: 2908
			public const int INVALFLAG = 1;

			// Token: 0x04000B5D RID: 2909
			public const int INVALPARAM = 11;

			// Token: 0x04000B5E RID: 2910
			public const int HANDLEBUSY = 12;

			// Token: 0x04000B5F RID: 2911
			public const int INVALIDALIAS = 13;

			// Token: 0x04000B60 RID: 2912
			public const int BADDB = 14;

			// Token: 0x04000B61 RID: 2913
			public const int KEYNOTFOUND = 15;

			// Token: 0x04000B62 RID: 2914
			public const int READERROR = 16;

			// Token: 0x04000B63 RID: 2915
			public const int WRITEERROR = 17;

			// Token: 0x04000B64 RID: 2916
			public const int DELETEERROR = 18;

			// Token: 0x04000B65 RID: 2917
			public const int VALNOTFOUND = 19;

			// Token: 0x04000B66 RID: 2918
			public const int NODRIVERCB = 20;

			// Token: 0x04000B67 RID: 2919
			public const int LASTERROR = 20;
		}

		// Token: 0x02000304 RID: 772
		private class WavConstants
		{
			// Token: 0x04000B68 RID: 2920
			public const int MM_WOM_OPEN = 955;

			// Token: 0x04000B69 RID: 2921
			public const int MM_WOM_CLOSE = 956;

			// Token: 0x04000B6A RID: 2922
			public const int MM_WOM_DONE = 957;

			// Token: 0x04000B6B RID: 2923
			public const int MM_WIM_OPEN = 958;

			// Token: 0x04000B6C RID: 2924
			public const int MM_WIM_CLOSE = 959;

			// Token: 0x04000B6D RID: 2925
			public const int MM_WIM_DATA = 960;

			// Token: 0x04000B6E RID: 2926
			public const int CALLBACK_FUNCTION = 196608;

			// Token: 0x04000B6F RID: 2927
			public const int WAVE_FORMAT_DIRECT = 8;

			// Token: 0x04000B70 RID: 2928
			public const int WAVERR_STILLPLAYING = 33;

			// Token: 0x04000B71 RID: 2929
			public const int WHDR_DONE = 1;

			// Token: 0x04000B72 RID: 2930
			public const int WHDR_PREPARED = 2;

			// Token: 0x04000B73 RID: 2931
			public const int WHDR_BEGINLOOP = 4;

			// Token: 0x04000B74 RID: 2932
			public const int WHDR_ENDLOOP = 8;

			// Token: 0x04000B75 RID: 2933
			public const int WHDR_INQUEUE = 16;
		}

		// Token: 0x02000305 RID: 773
		[StructLayout(LayoutKind.Sequential)]
		private class WAVEFORMATEX
		{
			// Token: 0x04000B76 RID: 2934
			public ushort wFormatTag;

			// Token: 0x04000B77 RID: 2935
			public ushort nChannels;

			// Token: 0x04000B78 RID: 2936
			public uint nSamplesPerSec;

			// Token: 0x04000B79 RID: 2937
			public uint nAvgBytesPerSec;

			// Token: 0x04000B7A RID: 2938
			public ushort nBlockAlign;

			// Token: 0x04000B7B RID: 2939
			public ushort wBitsPerSample;

			// Token: 0x04000B7C RID: 2940
			public ushort cbSize;
		}

		// Token: 0x02000306 RID: 774
		private struct WAVEOUTCAPS
		{
			// Token: 0x04000B7D RID: 2941
			public ushort wMid;

			// Token: 0x04000B7E RID: 2942
			public ushort wPid;

			// Token: 0x04000B7F RID: 2943
			public uint vDriverVersion;

			// Token: 0x04000B80 RID: 2944
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szPname;

			// Token: 0x04000B81 RID: 2945
			public uint dwFormats;

			// Token: 0x04000B82 RID: 2946
			public ushort wChannels;

			// Token: 0x04000B83 RID: 2947
			public ushort wReserved1;

			// Token: 0x04000B84 RID: 2948
			public uint dwSupport;
		}

		// Token: 0x02000307 RID: 775
		[StructLayout(LayoutKind.Sequential)]
		private class WAVEHDR
		{
			// Token: 0x04000B85 RID: 2949
			public IntPtr lpData;

			// Token: 0x04000B86 RID: 2950
			public uint dwBufferLength;

			// Token: 0x04000B87 RID: 2951
			public uint dwBytesRecorded;

			// Token: 0x04000B88 RID: 2952
			public IntPtr dwUser;

			// Token: 0x04000B89 RID: 2953
			public uint dwFlags;

			// Token: 0x04000B8A RID: 2954
			public uint dwLoops;

			// Token: 0x04000B8B RID: 2955
			public IntPtr lpNext;

			// Token: 0x04000B8C RID: 2956
			public uint reserved;
		}

		// Token: 0x02000308 RID: 776
		internal class WavFormat
		{
			// Token: 0x04000B8D RID: 2957
			public const int PCM = 1;
		}

		// Token: 0x02000309 RID: 777
		private class BufferItem
		{
			// Token: 0x060019D0 RID: 6608 RVA: 0x0009EE88 File Offset: 0x0009DE88
			public BufferItem(IntPtr wavDevHandle, int dataSize)
			{
				this.m_WavDevHandle = wavDevHandle;
				this.m_ThisHandle = GCHandle.Alloc(this);
				this.m_pBuffer = new byte[dataSize];
				this.m_DataHandle = GCHandle.Alloc(this.m_pBuffer, GCHandleType.Pinned);
				this.m_Header = new _WaveIn.WAVEHDR();
				this.m_Header.lpData = this.m_DataHandle.AddrOfPinnedObject();
				this.m_Header.dwBufferLength = (uint)dataSize;
				this.m_Header.dwBytesRecorded = 0U;
				this.m_Header.dwUser = (IntPtr)this.m_ThisHandle;
				this.m_Header.dwFlags = 0U;
				this.m_Header.dwLoops = 0U;
				this.m_Header.lpNext = IntPtr.Zero;
				this.m_Header.reserved = 0U;
				this.m_HeaderHandle = GCHandle.Alloc(this.m_Header, GCHandleType.Pinned);
				this.m_pEventArgs = new EventArgs<byte[]>(this.m_pBuffer);
			}

			// Token: 0x060019D1 RID: 6609 RVA: 0x0009EF94 File Offset: 0x0009DF94
			public void Dispose()
			{
				_WaveIn.waveInUnprepareHeader(this.m_WavDevHandle, this.m_HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(this.m_Header));
				this.m_ThisHandle.Free();
				this.m_HeaderHandle.Free();
				this.m_DataHandle.Free();
				this.m_pEventArgs = null;
			}

			// Token: 0x060019D2 RID: 6610 RVA: 0x0009EFF0 File Offset: 0x0009DFF0
			internal void Queue(bool unprepare)
			{
				this.m_Header.dwFlags = 0U;
				int num;
				if (unprepare)
				{
					num = _WaveIn.waveInUnprepareHeader(this.m_WavDevHandle, this.m_HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(this.m_Header));
					bool flag = num != 0;
					if (flag)
					{
						throw new Exception("Error unpreparing wave in buffer, error: " + num + ".");
					}
				}
				num = _WaveIn.waveInPrepareHeader(this.m_WavDevHandle, this.m_HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(this.m_Header));
				bool flag2 = num != 0;
				if (flag2)
				{
					throw new Exception("Error preparing wave in buffer, error: " + num + ".");
				}
				num = _WaveIn.waveInAddBuffer(this.m_WavDevHandle, this.m_HeaderHandle.AddrOfPinnedObject(), Marshal.SizeOf(this.m_Header));
				bool flag3 = num != 0;
				if (flag3)
				{
					throw new Exception("Error adding wave in buffer, error: " + num + ".");
				}
			}

			// Token: 0x1700083D RID: 2109
			// (get) Token: 0x060019D3 RID: 6611 RVA: 0x0009F0EC File Offset: 0x0009E0EC
			public _WaveIn.WAVEHDR Header
			{
				get
				{
					return this.m_Header;
				}
			}

			// Token: 0x1700083E RID: 2110
			// (get) Token: 0x060019D4 RID: 6612 RVA: 0x0009F104 File Offset: 0x0009E104
			public GCHandle HeaderHandle
			{
				get
				{
					return this.m_HeaderHandle;
				}
			}

			// Token: 0x1700083F RID: 2111
			// (get) Token: 0x060019D5 RID: 6613 RVA: 0x0009F11C File Offset: 0x0009E11C
			public byte[] Data
			{
				get
				{
					return this.m_pBuffer;
				}
			}

			// Token: 0x17000840 RID: 2112
			// (get) Token: 0x060019D6 RID: 6614 RVA: 0x0009F134 File Offset: 0x0009E134
			public int DataSize
			{
				get
				{
					return this.m_DataSize;
				}
			}

			// Token: 0x17000841 RID: 2113
			// (get) Token: 0x060019D7 RID: 6615 RVA: 0x0009F14C File Offset: 0x0009E14C
			public EventArgs<byte[]> EventArgs
			{
				get
				{
					return this.m_pEventArgs;
				}
			}

			// Token: 0x04000B8E RID: 2958
			private IntPtr m_WavDevHandle = IntPtr.Zero;

			// Token: 0x04000B8F RID: 2959
			private _WaveIn.WAVEHDR m_Header;

			// Token: 0x04000B90 RID: 2960
			private byte[] m_pBuffer = null;

			// Token: 0x04000B91 RID: 2961
			private GCHandle m_ThisHandle;

			// Token: 0x04000B92 RID: 2962
			private GCHandle m_HeaderHandle;

			// Token: 0x04000B93 RID: 2963
			private GCHandle m_DataHandle;

			// Token: 0x04000B94 RID: 2964
			private int m_DataSize = 0;

			// Token: 0x04000B95 RID: 2965
			private EventArgs<byte[]> m_pEventArgs = null;
		}
	}
}
