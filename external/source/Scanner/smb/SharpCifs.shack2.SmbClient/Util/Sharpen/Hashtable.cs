using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000043 RID: 67
	public class Hashtable : Dictionary<object, object>
	{
		// Token: 0x060001C0 RID: 448 RVA: 0x00008C8F File Offset: 0x00006E8F
		public void Put(object key, object value)
		{
			base.Add(key, value);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00008C9C File Offset: 0x00006E9C
		public object Get(object key)
		{
			object obj = base.Keys.SingleOrDefault((object k) => k.Equals(key));
			return (obj != null) ? base[obj] : null;
		}
	}
}
