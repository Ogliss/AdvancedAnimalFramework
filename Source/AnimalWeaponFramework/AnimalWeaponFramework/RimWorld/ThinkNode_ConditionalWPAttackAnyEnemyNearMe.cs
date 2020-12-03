using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000010 RID: 16
	public class ThinkNode_ConditionalWPAttackAnyEnemyNearMe : ThinkNode_Conditional
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00003B24 File Offset: 0x00001D24
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			ThinkNode_ConditionalWPAttackAnyEnemyNearMe thinkNode_ConditionalWPAttackAnyEnemyNearMe = (ThinkNode_ConditionalWPAttackAnyEnemyNearMe)base.DeepCopy(resolve);
			thinkNode_ConditionalWPAttackAnyEnemyNearMe.distance = this.distance;
			return thinkNode_ConditionalWPAttackAnyEnemyNearMe;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003B50 File Offset: 0x00001D50
		protected override bool Satisfied(Pawn pawn)
		{
			bool flag = !pawn.Spawned || pawn.MentalStateDef == MentalStateDefOf.PanicFlee || pawn.Downed || pawn.Dead;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = pawn.equipment.PrimaryEq != null && this.distance <= pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
				if (flag2)
				{
					this.distance = pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
					List<Pawn> allPawnsSpawned = pawn.Map.mapPawns.AllPawnsSpawned;
					for (int i = 0; i < allPawnsSpawned.Count; i++)
					{
						bool flag3 = pawn.Position.DistanceTo(allPawnsSpawned[i].Position) <= this.distance && allPawnsSpawned[i].HostileTo(pawn) && !allPawnsSpawned[i].Downed;
						if (flag3)
						{
							pawn.TryStartAttack(allPawnsSpawned[i]);
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x04000016 RID: 22
		private float distance;
	}
}
