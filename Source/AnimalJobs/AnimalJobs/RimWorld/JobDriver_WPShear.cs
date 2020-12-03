using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000018 RID: 24
	public class JobDriver_WPShear : JobDriver_WPGatherAnimalBodyResources
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00004424 File Offset: 0x00002624
		protected override float WorkTotal
		{
			get
			{
				return 1700f;
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000443C File Offset: 0x0000263C
		protected override CompHasGatherableBodyResource GetComp(Pawn animal)
		{
			return animal.TryGetComp<CompShearable>();
		}
	}
}
