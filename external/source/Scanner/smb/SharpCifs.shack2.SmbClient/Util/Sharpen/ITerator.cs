using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200004E RID: 78
	public interface ITerator
	{
		// Token: 0x060001E5 RID: 485
		bool HasNext();

		// Token: 0x060001E6 RID: 486
		object Next();

		// Token: 0x060001E7 RID: 487
		void Remove();
	}
}
