using System;
using System.Collections.Generic;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200001A RID: 26
	internal static class Collections<T>
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00006900 File Offset: 0x00004B00
		public static IList<T> EmptySet
		{
			get
			{
				return Collections<T>.Empty;
			}
		}

		// Token: 0x0400004F RID: 79
		private static readonly IList<T> Empty = new T[0];
	}
}
