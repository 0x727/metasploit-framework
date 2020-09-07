using System;
using System.Security.Cryptography;
using SharpCifs.Util.Sharpen;

// Token: 0x02000002 RID: 2
public class MD5Managed : MD5
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public MD5Managed()
	{
		this.HashSizeValue = 128;
		this.Initialize();
	}

	// Token: 0x06000002 RID: 2 RVA: 0x0000206C File Offset: 0x0000026C
	public override void Initialize()
	{
		this._data = new byte[64];
		this._dataSize = 0;
		this._totalLength = 0L;
		this._abcd = default(AbcdStruct);
		this._abcd.A = 1732584193U;
		this._abcd.B = 4023233417U;
		this._abcd.C = 2562383102U;
		this._abcd.D = 271733878U;
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000020E4 File Offset: 0x000002E4
	protected override void HashCore(byte[] array, int ibStart, int cbSize)
	{
		int i = this._dataSize + cbSize;
		bool flag = i >= 64;
		if (flag)
		{
			Array.Copy(array, ibStart, this._data, this._dataSize, 64 - this._dataSize);
			Md5Core.GetHashBlock(this._data, ref this._abcd, 0);
			int num = ibStart + (64 - this._dataSize);
			i -= 64;
			while (i >= 64)
			{
				Array.Copy(array, num, this._data, 0, 64);
				Md5Core.GetHashBlock(array, ref this._abcd, num);
				i -= 64;
				num += 64;
			}
			this._dataSize = i;
			Array.Copy(array, num, this._data, 0, i);
		}
		else
		{
			Array.Copy(array, ibStart, this._data, this._dataSize, cbSize);
			this._dataSize = i;
		}
		this._totalLength += (long)cbSize;
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000021D0 File Offset: 0x000003D0
	protected override byte[] HashFinal()
	{
		this.HashValue = Md5Core.GetHashFinalBlock(this._data, 0, this._dataSize, this._abcd, this._totalLength * 8L);
		return this.HashValue;
	}

	// Token: 0x04000001 RID: 1
	private byte[] _data;

	// Token: 0x04000002 RID: 2
	private AbcdStruct _abcd;

	// Token: 0x04000003 RID: 3
	private long _totalLength;

	// Token: 0x04000004 RID: 4
	private int _dataSize;
}
