using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace AnimalWeapons
{
	public abstract class JobGiver_WPArtyAIDefendPawn : JobGiver_WPArtyAIFightEnemy
	{
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_WPArtyAIDefendPawn jobGiver_WPArtyAIDefendPawn = (JobGiver_WPArtyAIDefendPawn)base.DeepCopy(resolve);
			jobGiver_WPArtyAIDefendPawn.attackMeleeThreatEvenIfNotHostile = this.attackMeleeThreatEvenIfNotHostile;
			return jobGiver_WPArtyAIDefendPawn;
		}

		protected abstract Pawn GetDefendee(Pawn pawn);

		protected override IntVec3 GetFlagPosition(Pawn pawn)
		{
			Pawn defendee = this.GetDefendee(pawn);
			IntVec3 result;
			if (defendee.Spawned || defendee.CarriedBy != null)
			{
				result = defendee.PositionHeld;
			}
			else
			{
				result = IntVec3.Invalid;
			}
			return result;
		}

		public override Job TryGiveJob(Pawn pawn)
		{
			Pawn defendee = this.GetDefendee(pawn);
			Job result;
			if (defendee == null)
			{
				result = null;
			}
			else
			{
				Pawn carriedBy = defendee.CarriedBy;
				if (carriedBy != null)
				{
					if (!pawn.CanReach(carriedBy, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
					{
						return null;
					}
				}
				else
				{
					if (!defendee.Spawned || !pawn.CanReach(defendee, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
					{
						return null;
					}
				}
				result = base.TryGiveJob(pawn);
			}
			return result;
		}

		protected override Thing FindAttackTarget(Pawn pawn)
		{
			if (this.attackMeleeThreatEvenIfNotHostile)
			{
				Pawn defendee = this.GetDefendee(pawn);
				if (defendee.Spawned && !defendee.InMentalState && defendee.mindState.meleeThreat != null && defendee.mindState.meleeThreat != pawn)
				{
					return defendee.mindState.meleeThreat;
				}
			}
			return base.FindAttackTarget(pawn);
		}

		protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest)
		{
			Verb verb = pawn.TryGetAttackVerb(null, !pawn.IsColonist);
			bool result;
			if (verb == null)
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

		private bool attackMeleeThreatEvenIfNotHostile;
	}
}
