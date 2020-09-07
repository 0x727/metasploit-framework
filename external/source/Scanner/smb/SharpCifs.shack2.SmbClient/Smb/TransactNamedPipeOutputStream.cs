using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000C1 RID: 193
	internal class TransactNamedPipeOutputStream : SmbFileOutputStream
	{
		// Token: 0x0600063E RID: 1598 RVA: 0x000222AC File Offset: 0x000204AC
		internal TransactNamedPipeOutputStream(SmbNamedPipe pipe) : base(pipe, false, (pipe.PipeType & -65281) | 32)
		{
			this._pipe = pipe;
			this._dcePipe = ((pipe.PipeType & 1536) == 1536);
			this._path = pipe.Unc;
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x0002230A File Offset: 0x0002050A
		public override void Close()
		{
			this._pipe.Close();
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00022319 File Offset: 0x00020519
		public override void Write(int b)
		{
			this._tmp[0] = (byte)b;
			this.Write(this._tmp, 0, 1);
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x0000A1BC File Offset: 0x000083BC
		public override void Write(byte[] b)
		{
			this.Write(b, 0, b.Length);
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00022338 File Offset: 0x00020538
		public override void Write(byte[] b, int off, int len)
		{
			bool flag = len < 0;
			if (flag)
			{
				len = 0;
			}
			bool flag2 = (this._pipe.PipeType & 256) == 256;
			if (flag2)
			{
				this._pipe.Send(new TransWaitNamedPipe(this._path), new TransWaitNamedPipeResponse());
				this._pipe.Send(new TransCallNamedPipe(this._path, b, off, len), new TransCallNamedPipeResponse(this._pipe));
			}
			else
			{
				bool flag3 = (this._pipe.PipeType & 512) == 512;
				if (flag3)
				{
					this.EnsureOpen();
					TransTransactNamedPipe transTransactNamedPipe = new TransTransactNamedPipe(this._pipe.Fid, b, off, len);
					bool dcePipe = this._dcePipe;
					if (dcePipe)
					{
						transTransactNamedPipe.MaxDataCount = 1024;
					}
					this._pipe.Send(transTransactNamedPipe, new TransTransactNamedPipeResponse(this._pipe));
				}
			}
		}

		// Token: 0x040003C6 RID: 966
		private string _path;

		// Token: 0x040003C7 RID: 967
		private SmbNamedPipe _pipe;

		// Token: 0x040003C8 RID: 968
		private byte[] _tmp = new byte[1];

		// Token: 0x040003C9 RID: 969
		private bool _dcePipe;
	}
}
