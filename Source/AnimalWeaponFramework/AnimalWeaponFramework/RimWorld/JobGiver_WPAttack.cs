using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000008 RID: 8
	public class JobGiver_WPAttack : ThinkNode_JobGiver
	{
		// Token: 0x06000022 RID: 34 RVA: 0x00002D28 File Offset: 0x00000F28
		protected override Job TryGiveJob(Pawn pawn)
		{
			bool flag = pawn.Downed || pawn.Dead || pawn.equipment.PrimaryEq == null;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = pawn.TryGetAttackVerb(null, false) == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					Pawn pawn2 = this.FindPawnTarget(pawn);
					bool flag3 = pawn2 != null && pawn.CanReach(pawn2, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn);
					if (flag3)
					{
						bool flag4 = pawn2.Downed || pawn2.Dead;
						if (flag4)
						{
							return null;
						}
						bool flag5 = !pawn2.Downed || !pawn2.Dead;
						if (flag5)
						{
							bool flag6 = pawn.equipment.Primary.def.IsRangedWeapon && pawn.Position.DistanceTo(pawn2.Position) <= pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
							if (flag6)
							{
								pawn.TryStartAttack(pawn2);
								return null;
							}
							bool flag7 = pawn.equipment.Primary.def.IsRangedWeapon && pawn.Position.DistanceTo(pawn2.Position) > pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
							if (flag7)
							{
								return this.RangeAttackJob(pawn, pawn2);
							}
							return this.MeleeAttackJob(pawn, pawn2);
						}
					}
					Building building = this.FindTurretTarget(pawn);
					bool flag8 = building != null;
					if (flag8)
					{
						bool flag9 = pawn.equipment.Primary.def.IsRangedWeapon && pawn.Position.DistanceTo(building.Position) <= pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
						if (flag9)
						{
							pawn.TryStartAttack(building);
							result = null;
						}
						else
						{
							bool flag10 = pawn.Position.DistanceTo(building.Position) > pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
							if (flag10)
							{
								result = this.RangeAttackJob(pawn, building);
							}
							else
							{
								result = this.MeleeAttackJob(pawn, building);
							}
						}
					}
					else
					{
						bool flag11 = pawn2 != null;
						if (flag11)
						{
							using (PawnPath pawnPath = pawn.Map.pathFinder.FindPath(pawn.Position, pawn2.Position, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassDoors, false), PathEndMode.OnCell))
							{
								bool flag12 = !pawnPath.Found;
								if (flag12)
								{
									return null;
								}
								IntVec3 loc;
								bool flag13 = !pawnPath.TryFindLastCellBeforeBlockingDoor(pawn, out loc);
								if (flag13)
								{
									Log.Error(((pawn != null) ? pawn.ToString() : null) + " did TryFindLastCellBeforeDoor but found none when it should have been one. Target: " + pawn2.LabelCap, false);
									return null;
								}
								IntVec3 randomCell = CellFinder.RandomRegionNear(loc.GetRegion(pawn.Map, RegionType.Set_Passable), 9, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), null, null, RegionType.Set_Passable).RandomCell;
								bool flag14 = randomCell == pawn.Position;
								if (flag14)
								{
									return JobMaker.MakeJob(JobDefOf.Wait, 30, false);
								}
								return JobMaker.MakeJob(JobDefOf.Goto, randomCell);
							}
						}
						result = null;
					}
				}
			}
			return result;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000030A4 File Offset: 0x000012A4
		private Job MeleeAttackJob(Pawn pawn, Thing target)
		{
			Job job = JobMaker.MakeJob(JobDefOf.AttackMelee, target);
			job.maxNumMeleeAttacks = 1;
			job.expiryInterval = Rand.Range(420, 900);
			job.attackDoorIfTargetLost = true;
			return job;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000030EC File Offset: 0x000012EC
		private Job RangeAttackJob(Pawn pawn, Thing target)
		{
			Job job = JobMaker.MakeJob(JobDefOf.Goto, target);
			job.checkOverrideOnExpire = true;
			job.expiryInterval = 30;
			job.collideWithPawns = true;
			return job;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003128 File Offset: 0x00001328
		private Pawn FindPawnTarget(Pawn pawn)
		{
			return (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedLOSToPawns, (Thing x) => x is Pawn && x.def.race != pawn.RaceProps && !pawn.Downed && !pawn.Dead, 0f, 9999f, default(IntVec3), float.MaxValue, true, true);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003180 File Offset: 0x00001380
		private Building_Turret FindTurretTarget(Pawn pawn)
		{
			return (Building_Turret)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedReachable | TargetScanFlags.NeedThreat, (Thing t) => t is Building_Turret, 0f, 70f, default(IntVec3), float.MaxValue, false, true);
		}

		// Token: 0x04000010 RID: 16
		private const float WaitChance = 0.75f;

		// Token: 0x04000011 RID: 17
		private const int WaitTicks = 90;

		// Token: 0x04000012 RID: 18
		private const int MinMeleeChaseTicks = 900;

		// Token: 0x04000013 RID: 19
		private const int MaxMeleeChaseTicks = 1800;

		// Token: 0x04000014 RID: 20
		private const int WanderOutsideDoorRegions = 9;

		// Token: 0x04000015 RID: 21
		public static readonly IntRange ExpiryInterval_ShooterSucceeded = new IntRange(450, 550);

		// Token: 0x0200001D RID: 29
		[DefOf]
		public static class WPJobDefOf
		{
			// Token: 0x04000021 RID: 33
			public static JobDef WPAttack;
		}
	}
}
