using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000028 RID: 40
	public class WorkGiver_WPShear : WorkGiver_WPGatherAnimalBodyResources
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00006074 File Offset: 0x00004274
		protected override JobDef JobDef
		{
			get
			{
				return WPJobDefOf.WPShear;
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x0000608C File Offset: 0x0000428C
		protected override CompHasGatherableBodyResource GetComp(Pawn animal)
		{
			return animal.TryGetComp<CompShearable>();
		}
	}
}
