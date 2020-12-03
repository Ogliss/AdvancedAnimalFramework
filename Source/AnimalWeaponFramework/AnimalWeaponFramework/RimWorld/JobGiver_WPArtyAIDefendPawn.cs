using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000006 RID: 6
	public abstract class JobGiver_WPArtyAIDefendPawn : JobGiver_WPArtyAIFightEnemy
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00002520 File Offset: 0x00000720
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_WPArtyAIDefendPawn jobGiver_WPArtyAIDefendPawn = (JobGiver_WPArtyAIDefendPawn)base.DeepCopy(resolve);
			jobGiver_WPArtyAIDefendPawn.attackMeleeThreatEvenIfNotHostile = this.attackMeleeThreatEvenIfNotHostile;
			return jobGiver_WPArtyAIDefendPawn;
		}

		// Token: 0x0600000F RID: 15
		protected abstract Pawn GetDefendee(Pawn pawn);

		// Token: 0x06000010 RID: 16 RVA: 0x0000254C File Offset: 0x0000074C
		protected override IntVec3 GetFlagPosition(Pawn pawn)
		{
			Pawn defendee = this.GetDefendee(pawn);
			bool flag = defendee.Spawned || defendee.CarriedBy != null;
			IntVec3 result;
			if (flag)
			{
				result = defendee.PositionHeld;
			}
			else
			{
				result = IntVec3.Invalid;
			}
			return result;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002590 File Offset: 0x00000790
		protected override Job TryGiveJob(Pawn pawn)
		{
			Pawn defendee = this.GetDefendee(pawn);
			bool flag = defendee == null;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Pawn carriedBy = defendee.CarriedBy;
				bool flag2 = carriedBy != null;
				if (flag2)
				{
					bool flag3 = !pawn.CanReach(carriedBy, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
					if (flag3)
					{
						return null;
					}
				}
				else
				{
					bool flag4 = !defendee.Spawned || !pawn.CanReach(defendee, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
					if (flag4)
					{
						return null;
					}
				}
				result = base.TryGiveJob(pawn);
			}
			return result;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000261C File Offset: 0x0000081C
		protected override Thing FindAttackTarget(Pawn pawn)
		{
			bool flag = this.attackMeleeThreatEvenIfNotHostile;
			if (flag)
			{
				Pawn defendee = this.GetDefendee(pawn);
				bool flag2 = defendee.Spawned && !defendee.InMentalState && defendee.mindState.meleeThreat != null && defendee.mindState.meleeThreat != pawn;
				if (flag2)
				{
					return defendee.mindState.meleeThreat;
				}
			}
			return base.FindAttackTarget(pawn);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002690 File Offset: 0x00000890
		protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest)
		{
			Verb verb = pawn.TryGetAttackVerb(null, !pawn.IsColonist);
			bool flag = verb == null;
			bool result;
			if (flag)
			{
				dest = IntVec3.Invalid;
				result = false;
			}
			else
			{
				float range = pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
				result = CastPositionFinder.TryFindCastPosition(new CastPositionRequest
				{
					caster = pawn,
					target = pawn.mindState.enemyTarget,
					verb = verb,
					maxRangeFromTarget = 9999f,
					locus = this.GetDefendee(pawn).PositionHeld,
					maxRangeFromLocus = this.GetFlagRadius(pawn),
					wantCoverFromTarget = (verb.verbProps.range > 7f),
					maxRegions = 999
				}, out dest);
			}
			return result;
		}

		// Token: 0x04000007 RID: 7
		private bool attackMeleeThreatEvenIfNotHostile;
	}
}
