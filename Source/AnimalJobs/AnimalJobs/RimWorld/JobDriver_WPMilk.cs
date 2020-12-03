using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000013 RID: 19
	public class JobDriver_WPMilk : JobDriver_WPGatherAnimalBodyResources
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000040D4 File Offset: 0x000022D4
		protected override float WorkTotal
		{
			get
			{
				return 400f;
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000040EC File Offset: 0x000022EC
		protected override CompHasGatherableBodyResource GetComp(Pawn animal)
		{
			return animal.TryGetComp<CompMilkable>();
		}
	}
}
