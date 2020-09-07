using System;
using System.Security.Cryptography;

namespace LumiSoft.Net
{
	// Token: 0x02000009 RID: 9
	internal abstract class _MD4 : HashAlgorithm
	{
		// Token: 0x06000018 RID: 24 RVA: 0x000023C4 File Offset: 0x000013C4
		protected _MD4()
		{
			this.HashSizeValue = 128;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000023DC File Offset: 0x000013DC
		public new static _MD4 Create()
		{
			return _MD4.Create("MD4");
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000023F8 File Offset: 0x000013F8
		public new static _MD4 Create(string hashName)
		{
			object obj = CryptoConfig.CreateFromName(hashName);
			bool flag = obj == null;
			if (flag)
			{
				obj = new MD4Managed();
			}
			return (_MD4)obj;
		}
	}
}
