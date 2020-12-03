using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000023 RID: 35
	public abstract class WorkGiver_WPGatherAnimalBodyResources : WorkGiver_Scanner
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600009C RID: 156
		protected abstract JobDef JobDef { get; }

		// Token: 0x0600009D RID: 157
		protected abstract CompHasGatherableBodyResource GetComp(Pawn animal);

		// Token: 0x0600009E RID: 158 RVA: 0x00005907 File Offset: 0x00003B07
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			List<Pawn> pawns = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
			int num;
			for (int i = 0; i < pawns.Count; i = num + 1)
			{
				yield return pawns[i];
				num = i;
			}
			yield break;
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00005920 File Offset: 0x00003B20
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005934 File Offset: 0x00003B34
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			bool flag = pawn2 == null || !pawn2.RaceProps.Animal;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CompHasGatherableBodyResource comp = this.GetComp(pawn2);
				bool flag2 = comp != null && comp.ActiveAndFull && !pawn2.Downed && pawn2.CanCasuallyInteractNow(false);
				if (flag2)
				{
					LocalTargetInfo target = pawn2;
					bool flag3 = pawn.CanReserve(target, 1, -1, null, forced);
					if (flag3)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000059B8 File Offset: 0x00003BB8
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(this.JobDef, t);
		}
	}
}
