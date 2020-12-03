using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000027 RID: 39
	public abstract class WorkGiver_WPRemoveBuilding : WorkGiver_Scanner
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000B0 RID: 176
		protected abstract DesignationDef Designation { get; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000B1 RID: 177
		protected abstract JobDef WPRemoveBuildingJob { get; }

		// Token: 0x060000B2 RID: 178 RVA: 0x00005F91 File Offset: 0x00004191
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			foreach (Designation des in pawn.Map.designationManager.SpawnedDesignationsOfDef(this.Designation))
			{
				yield return des.target.Thing;
			}
			yield break;
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00005FA8 File Offset: 0x000041A8
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00005FBC File Offset: 0x000041BC
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			bool claimable = t.def.Claimable;
			if (claimable)
			{
				bool flag = t.Faction != pawn.Faction;
				if (flag)
				{
					return false;
				}
			}
			else
			{
				bool flag2 = pawn.Faction != Faction.OfPlayer;
				if (flag2)
				{
					return false;
				}
			}
			LocalTargetInfo target = t;
			return pawn.CanReserve(target, 1, -1, null, forced) && pawn.Map.designationManager.DesignationOn(t, this.Designation) != null;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00006048 File Offset: 0x00004248
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(this.WPRemoveBuildingJob, t);
		}
	}
}
