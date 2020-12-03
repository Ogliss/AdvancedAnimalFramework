using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000002 RID: 2
	public class JobGiver_ArtyWeapon : ThinkNode_JobGiver
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		protected override Job TryGiveJob(Pawn pawn)
		{
			bool flag = Rand.Value < 0.75f;
			Job result;
			if (flag)
			{
				result = new Job(WPJobDefOf.ArtyWaitCombat)
				{
					expiryInterval = 500
				};
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
						result = new Job(JobDefOf.AttackMelee, pawn2)
						{
							maxNumMeleeAttacks = 1,
							expiryInterval = Rand.Range(42, 200),
							canBash = true
						};
					}
					else
					{
						Building building = this.FindTurretTarget(pawn);
						bool flag4 = building != null;
						if (flag4)
						{
							result = new Job(JobDefOf.AttackMelee, pawn2)
							{
								maxNumMeleeAttacks = 1,
								expiryInterval = Rand.Range(420, 900),
								canBash = true
							};
						}
						else
						{
							bool flag5 = pawn2 != null;
							if (flag5)
							{
								using (PawnPath pawnPath = pawn.Map.pathFinder.FindPath(pawn.Position, pawn2.Position, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassDoors, false), PathEndMode.OnCell))
								{
									bool flag6 = !pawnPath.Found;
									if (flag6)
									{
										return null;
									}
									IntVec3 loc;
									bool flag7 = !pawnPath.TryFindLastCellBeforeBlockingDoor(pawn, out loc);
									if (flag7)
									{
										Log.Error(((pawn != null) ? pawn.ToString() : null) + " did TryFindLastCellBeforeDoor but found none when it should have been one. Target: " + pawn2.LabelCap, false);
										return null;
									}
									IntVec3 randomCell = CellFinder.RandomRegionNear(loc.GetRegion(pawn.Map, RegionType.Set_Passable), 9, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), null, null, RegionType.Set_Passable).RandomCell;
									bool flag8 = randomCell == pawn.Position;
									if (flag8)
									{
										return new Job(WPJobDefOf.ArtyWaitCombat, JobGiver_WPArtyAIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true);
									}
									return new Job(JobDefOf.Goto, randomCell);
								}
							}
							result = null;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000227C File Offset: 0x0000047C
		private Pawn FindPawnTarget(Pawn pawn)
		{
			return (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.None, (Thing x) => x is Pawn && x.HostileTo(pawn) && !pawn.health.Downed, 0f, 9999f, default(IntVec3), float.MaxValue, true, true);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000022D4 File Offset: 0x000004D4
		private Building FindTurretTarget(Pawn pawn)
		{
			return (Building)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.None, (Thing t) => t is Building && t.HostileTo(pawn), 0f, 9999f, default(IntVec3), float.MaxValue, false, true);
		}

		// Token: 0x04000001 RID: 1
		private const float WaitChance = 0.75f;

		// Token: 0x04000002 RID: 2
		private const int WaitTicks = 90;

		// Token: 0x04000003 RID: 3
		private const int MinMeleeChaseTicks = 420;

		// Token: 0x04000004 RID: 4
		private const int MaxMeleeChaseTicks = 900;

		// Token: 0x04000005 RID: 5
		private const int WanderOutsideDoorRegions = 9;
	}
}
