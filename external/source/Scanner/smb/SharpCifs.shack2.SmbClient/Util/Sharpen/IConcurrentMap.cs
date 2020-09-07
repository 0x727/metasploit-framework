using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000046 RID: 70
	internal interface IConcurrentMap<T, TU> : IDictionary<T, TU>, ICollection<KeyValuePair<T, TU>>, IEnumerable<KeyValuePair<T, TU>>, IEnumerable
	{
		// Token: 0x060001C5 RID: 453
		TU PutIfAbsent(T key, TU value);

		// Token: 0x060001C6 RID: 454
		bool Remove(object key, object value);

		// Token: 0x060001C7 RID: 455
		bool Replace(T key, TU oldValue, TU newValue);
	}
}
