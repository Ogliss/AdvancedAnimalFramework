using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000010 RID: 16
	public class JobDriver_WPDeconstruct : JobDriver_WPRemoveBuilding
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00003DD4 File Offset: 0x00001FD4
		protected override DesignationDef Designation
		{
			get
			{
				return DesignationDefOf.Deconstruct;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00003DEC File Offset: 0x00001FEC
		protected override int TotalNeededWork
		{
			get
			{
				Building building = base.Building;
				int value = Mathf.RoundToInt(building.GetStatValue(StatDefOf.WorkToBuild, true));
				return Mathf.Clamp(value, 20, 3000);
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003E24 File Offset: 0x00002024
		protected override void FinishedRemoving()
		{
			base.Target.Destroy(DestroyMode.Deconstruct);
			this.pawn.records.Increment(RecordDefOf.ThingsDeconstructed);
		}

		// Token: 0x0400000C RID: 12
		private const int MaxDeconstructWork = 3000;
	}
}
