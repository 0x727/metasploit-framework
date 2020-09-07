using System;
using System.IO;
using System.Security.Cryptography;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000055 RID: 85
	public class MessageDigest<TAlgorithm> : MessageDigest where TAlgorithm : HashAlgorithm, new()
	{
		// Token: 0x06000216 RID: 534 RVA: 0x00009EBF File Offset: 0x000080BF
		public MessageDigest()
		{
			this.Init();
		}

		// Token: 0x06000217 RID: 535 RVA: 0x00009ED0 File Offset: 0x000080D0
		public override byte[] Digest()
		{
			this._stream.FlushFinalBlock();
			byte[] hash = this._hash.Hash;
			this.Reset();
			return hash;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00009F08 File Offset: 0x00008108
		public void Dispose()
		{
			bool flag = this._stream != null;
			if (flag)
			{
				this._stream.Dispose();
			}
			this._stream = null;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00009F38 File Offset: 0x00008138
		public override int GetDigestLength()
		{
			return this._hash.HashSize / 8;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00009F5C File Offset: 0x0000815C
		private void Init()
		{
			this._hash = Activator.CreateInstance<TAlgorithm>();
			this._stream = new CryptoStream(Stream.Null, this._hash, CryptoStreamMode.Write);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00009F86 File Offset: 0x00008186
		public override void Reset()
		{
			this.Dispose();
			this.Init();
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00009F97 File Offset: 0x00008197
		public override void Update(byte[] input)
		{
			this._stream.Write(input, 0, input.Length);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00009FAB File Offset: 0x000081AB
		public override void Update(byte input)
		{
			this._stream.WriteByte(input);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00009FBC File Offset: 0x000081BC
		public override void Update(byte[] input, int index, int count)
		{
			bool flag = count < 0;
			if (flag)
			{
				Console.WriteLine("Argh!");
			}
			this._stream.Write(input, index, count);
		}

		// Token: 0x0400006A RID: 106
		private TAlgorithm _hash;

		// Token: 0x0400006B RID: 107
		private CryptoStream _stream;
	}
}
