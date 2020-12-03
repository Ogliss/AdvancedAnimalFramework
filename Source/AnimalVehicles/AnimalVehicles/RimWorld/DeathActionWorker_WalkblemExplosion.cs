using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000003 RID: 3
	public class DeathActionWorker_WalkblemExplosion : DeathActionWorker
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000005 RID: 5 RVA: 0x000022CC File Offset: 0x000004CC
		public override RulePackDef DeathRules
		{
			get
			{
				return RulePackDefOf.Transition_DiedExplosive;
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000022E4 File Offset: 0x000004E4
		public override void PawnDied(Corpse corpse)
		{
			bool flag = corpse.InnerPawn.ageTracker.CurLifeStageIndex == 0;
			float radius;
			if (flag)
			{
				radius = 1.9f;
			}
			else
			{
				bool flag2 = corpse.InnerPawn.ageTracker.CurLifeStageIndex == 1;
				if (flag2)
				{
					radius = 2.9f;
				}
				else
				{
					radius = 4.9f;
				}
			}
			GenExplosion.DoExplosion(corpse.Position, corpse.Map, radius, DamageDefOf.Flame, corpse.InnerPawn, 100, 1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false, null, null);
		}
	}
}
