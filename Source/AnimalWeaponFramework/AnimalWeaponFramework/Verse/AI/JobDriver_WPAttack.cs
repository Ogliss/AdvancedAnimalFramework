using System;
using System.Collections.Generic;
using RimWorld;

namespace Verse.AI
{
	// Token: 0x02000018 RID: 24
	public class JobDriver_WPAttack : JobDriver
	{
		// Token: 0x06000052 RID: 82 RVA: 0x00004205 File Offset: 0x00002405
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.numAttacksMade, "numAttacksMade", 0, false);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004224 File Offset: 0x00002424
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			IAttackTarget attackTarget = this.job.targetA.Thing as IAttackTarget;
			bool flag = attackTarget != null;
			if (flag)
			{
				this.pawn.Map.attackTargetReservationManager.Reserve(this.pawn, this.job, attackTarget);
			}
			return true;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x0000427A File Offset: 0x0000247A
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_General.DoAtomic(delegate
			{
				Pawn pawn = this.job.targetA.Thing as Pawn;
				bool flag = pawn != null && pawn.Downed && this.pawn.mindState.duty != null && this.pawn.mindState.duty.attackDownedIfStarving && this.pawn.Starving();
				if (flag)
				{
					this.job.killIncappedTarget = true;
				}
			});
			yield return Toils_Combat.TrySetJobToUseAttackVerb(TargetIndex.A);
			yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);
			yield return Toils_Combat.GotoCastPosition(TargetIndex.A, false, 1f);
			yield return Toils_Combat.CastVerb(TargetIndex.A, true);
			yield break;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000428C File Offset: 0x0000248C
		public override void Notify_PatherFailed()
		{
			bool attackDoorIfTargetLost = this.job.attackDoorIfTargetLost;
			if (attackDoorIfTargetLost)
			{
				Thing thing;
				using (PawnPath pawnPath = base.Map.pathFinder.FindPath(this.pawn.Position, base.TargetA.Cell, TraverseParms.For(this.pawn, Danger.Deadly, TraverseMode.PassDoors, false), PathEndMode.OnCell))
				{
					bool flag = !pawnPath.Found;
					if (flag)
					{
						return;
					}
					IntVec3 intVec;
					thing = pawnPath.FirstBlockingBuilding(out intVec, this.pawn);
				}
				bool flag2 = thing != null;
				if (flag2)
				{
					bool flag3 = thing.Position.InHorDistOf(this.pawn.Position, 6f);
					if (flag3)
					{
						this.job.targetA = thing;
						this.job.maxNumMeleeAttacks = Rand.RangeInclusive(2, 5);
						this.job.expiryInterval = Rand.Range(2000, 4000);
						return;
					}
				}
			}
			base.Notify_PatherFailed();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000043A8 File Offset: 0x000025A8
		public override bool IsContinuation(Job j)
		{
			return this.job.GetTarget(TargetIndex.A) == j.GetTarget(TargetIndex.A);
		}

		// Token: 0x0400001B RID: 27
		private int numAttacksMade;
	}
}
