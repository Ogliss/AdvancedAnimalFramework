using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000026 RID: 38
	public class WorkGiver_WPMilk : WorkGiver_WPGatherAnimalBodyResources
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00005F58 File Offset: 0x00004158
		protected override JobDef JobDef
		{
			get
			{
				return WPJobDefOf.WPMilk;
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005F70 File Offset: 0x00004170
		protected override CompHasGatherableBodyResource GetComp(Pawn animal)
		{
			return animal.TryGetComp<CompMilkable>();
		}
	}
}
