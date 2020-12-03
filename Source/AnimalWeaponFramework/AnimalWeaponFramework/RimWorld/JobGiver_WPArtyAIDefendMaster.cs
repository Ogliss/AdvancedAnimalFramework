using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000005 RID: 5
	public class JobGiver_WPArtyAIDefendMaster : JobGiver_WPArtyAIDefendPawn
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002408 File Offset: 0x00000608
		protected override Pawn GetDefendee(Pawn pawn)
		{
			bool flag = pawn.playerSettings.Master == null;
			Pawn result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = pawn.playerSettings.Master != null || !pawn.playerSettings.Master.health.Dead;
				if (flag2)
				{
					result = pawn.playerSettings.Master;
				}
				else
				{
					result = pawn;
				}
			}
			return result;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002470 File Offset: 0x00000670
		protected override float GetFlagRadius(Pawn pawn)
		{
			float range = pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
			bool flag = pawn.playerSettings.Master.playerSettings.animalsReleased && pawn.training.HasLearned(TrainableDefOf.Release);
			if (flag)
			{
				bool flag2 = range <= 50f;
				if (flag2)
				{
					return 50f;
				}
				bool flag3 = range > 50f;
				if (flag3)
				{
					return pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
				}
			}
			return 5f;
		}

		// Token: 0x04000006 RID: 6
		private const float RadiusUnreleased = 5f;
	}
}
