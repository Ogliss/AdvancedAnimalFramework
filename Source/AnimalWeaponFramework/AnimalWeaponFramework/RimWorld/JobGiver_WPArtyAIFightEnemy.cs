using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	// Token: 0x02000007 RID: 7
	public abstract class JobGiver_WPArtyAIFightEnemy : ThinkNode_JobGiver
	{
		// Token: 0x06000015 RID: 21
		protected abstract bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest);

		// Token: 0x06000016 RID: 22 RVA: 0x0000277C File Offset: 0x0000097C
		protected virtual float GetFlagRadius(Pawn pawn)
		{
			return 999999f;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002794 File Offset: 0x00000994
		protected virtual IntVec3 GetFlagPosition(Pawn pawn)
		{
			return IntVec3.Invalid;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000027AC File Offset: 0x000009AC
		protected virtual bool ExtraTargetValidator(Pawn pawn, Thing target)
		{
			return true;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000027C0 File Offset: 0x000009C0
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_WPArtyAIFightEnemy jobGiver_WPArtyAIFightEnemy = (JobGiver_WPArtyAIFightEnemy)base.DeepCopy(resolve);
			jobGiver_WPArtyAIFightEnemy.targetAcquireRadius = this.targetAcquireRadius;
			jobGiver_WPArtyAIFightEnemy.targetKeepRadius = this.targetKeepRadius;
			jobGiver_WPArtyAIFightEnemy.needLOSToAcquireNonPawnTargets = this.needLOSToAcquireNonPawnTargets;
			jobGiver_WPArtyAIFightEnemy.chaseTarget = this.chaseTarget;
			return jobGiver_WPArtyAIFightEnemy;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002810 File Offset: 0x00000A10
		protected override Job TryGiveJob(Pawn pawn)
		{
			bool flag = pawn.playerSettings == null;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = pawn.playerSettings.Master == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					Pawn respectedMaster = pawn.playerSettings.RespectedMaster;
					bool flag3 = respectedMaster == null;
					if (flag3)
					{
						result = null;
					}
					else
					{
						this.UpdateEnemyTarget(pawn);
						Thing enemyTarget = pawn.mindState.enemyTarget;
						bool flag4 = enemyTarget == null;
						if (flag4)
						{
							result = null;
						}
						else
						{
							bool flag5 = !pawn.IsColonist;
							Verb currentEffectiveVerb = pawn.CurrentEffectiveVerb;
							bool flag6 = currentEffectiveVerb == null;
							if (flag6)
							{
								result = null;
							}
							else
							{
								bool isMeleeAttack = currentEffectiveVerb.verbProps.IsMeleeAttack;
								if (isMeleeAttack)
								{
									result = this.MeleeAttackJob(enemyTarget);
								}
								else
								{
									bool flag7 = CoverUtility.CalculateOverallBlockChance(pawn.Position, enemyTarget.Position, pawn.Map) > 0.01f;
									bool flag8 = pawn.Position.Standable(pawn.Map);
									bool flag9 = flag7 || flag8;
									if (flag9)
									{
										result = new Job(WPJobDefOf.ArtyWaitCombat, JobGiver_WPArtyAIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true);
									}
									else
									{
										IntVec3 intVec;
										bool flag10 = !this.TryFindShootingPosition(pawn, out intVec);
										if (flag10)
										{
											result = null;
										}
										else
										{
											bool flag11 = intVec == pawn.Position;
											if (flag11)
											{
												result = new Job(WPJobDefOf.ArtyWaitCombat, JobGiver_WPArtyAIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true);
											}
											else
											{
												result = new Job(JobDefOf.Goto, intVec)
												{
													expiryInterval = JobGiver_WPArtyAIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
													checkOverrideOnExpire = true
												};
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000029BC File Offset: 0x00000BBC
		protected virtual Job MeleeAttackJob(Thing enemyTarget)
		{
			return new Job(JobDefOf.AttackMelee, enemyTarget)
			{
				expiryInterval = JobGiver_WPArtyAIFightEnemy.ExpiryInterval_Melee.RandomInRange,
				checkOverrideOnExpire = true,
				expireRequiresEnemiesNearby = true
			};
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002A00 File Offset: 0x00000C00
		protected virtual void UpdateEnemyTarget(Pawn pawn)
		{
			Thing thing = pawn.mindState.enemyTarget;
			bool flag = thing != null && (thing.Destroyed || Find.TickManager.TicksGame - pawn.mindState.lastEngageTargetTick > 200 || ((IAttackTarget)thing).ThreatDisabled(pawn));
			if (flag)
			{
				thing = null;
			}
			bool flag2 = thing == null;
			if (flag2)
			{
				thing = this.FindAttackTargetIfPossible(pawn);
				bool flag3 = thing != null;
				if (flag3)
				{
					pawn.mindState.lastEngageTargetTick = Find.TickManager.TicksGame;
					Lord lord = pawn.GetLord();
					bool flag4 = lord != null;
					if (flag4)
					{
						lord.Notify_PawnAcquiredTarget(pawn, thing);
					}
				}
			}
			else
			{
				Thing thing2 = this.FindAttackTargetIfPossible(pawn);
				bool flag5 = thing2 == null && !this.chaseTarget;
				if (flag5)
				{
					thing = null;
				}
				else
				{
					bool flag6 = thing2 != null && thing2 != thing;
					if (flag6)
					{
						pawn.mindState.lastEngageTargetTick = Find.TickManager.TicksGame;
						thing = thing2;
					}
				}
			}
			pawn.mindState.enemyTarget = thing;
			Pawn pawn2 = thing as Pawn;
			bool flag7 = pawn2 != null && pawn2.Faction == Faction.OfPlayer && pawn.Position.InHorDistOf(pawn2.Position, 30f);
			if (flag7)
			{
				Find.TickManager.slower.SignalForceNormalSpeed();
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002B60 File Offset: 0x00000D60
		private Thing FindAttackTargetIfPossible(Pawn pawn)
		{
			bool flag = pawn.TryGetAttackVerb(null, !pawn.IsColonist) == null;
			Thing result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.FindAttackTarget(pawn);
			}
			return result;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002B98 File Offset: 0x00000D98
		protected virtual Thing FindAttackTarget(Pawn pawn)
		{
			TargetScanFlags targetScanFlags = TargetScanFlags.None;
			bool flag = this.needLOSToAcquireNonPawnTargets;
			if (flag)
			{
				targetScanFlags |= TargetScanFlags.None;
			}
			bool flag2 = this.PrimaryVerbIsIncendiary(pawn);
			if (flag2)
			{
				targetScanFlags |= TargetScanFlags.NeedNonBurning;
			}
			float maxDist = pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range * 2f;
			return (Thing)AttackTargetFinder.BestAttackTarget(pawn, targetScanFlags, (Thing x) => this.ExtraTargetValidator(pawn, x), 0f, maxDist, this.GetFlagPosition(pawn), this.GetFlagRadius(pawn), true, true);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002C54 File Offset: 0x00000E54
		private bool PrimaryVerbIsIncendiary(Pawn pawn)
		{
			bool flag = pawn.equipment != null && pawn.equipment.Primary != null;
			if (flag)
			{
				List<Verb> allVerbs = pawn.equipment.Primary.GetComp<CompEquippable>().AllVerbs;
				for (int i = 0; i < allVerbs.Count; i++)
				{
					bool isPrimary = allVerbs[i].verbProps.isPrimary;
					if (isPrimary)
					{
						return allVerbs[i].IsIncendiary();
					}
				}
			}
			return false;
		}

		// Token: 0x04000008 RID: 8
		private float targetAcquireRadius = 56f;

		// Token: 0x04000009 RID: 9
		private float targetKeepRadius = 65f;

		// Token: 0x0400000A RID: 10
		private bool needLOSToAcquireNonPawnTargets;

		// Token: 0x0400000B RID: 11
		private bool chaseTarget;

		// Token: 0x0400000C RID: 12
		public static readonly IntRange ExpiryInterval_ShooterSucceeded = new IntRange(450, 550);

		// Token: 0x0400000D RID: 13
		private static readonly IntRange ExpiryInterval_Melee = new IntRange(360, 480);

		// Token: 0x0400000E RID: 14
		private const int MinTargetDistanceToMove = 5;

		// Token: 0x0400000F RID: 15
		private const int TicksSinceEngageToLoseTarget = 400;
	}
}
