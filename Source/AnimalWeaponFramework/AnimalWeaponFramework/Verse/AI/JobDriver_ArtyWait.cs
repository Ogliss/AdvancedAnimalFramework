using System;
using System.Collections.Generic;
using RimWorld;

namespace Verse.AI
{
	// Token: 0x02000016 RID: 22
	public class JobDriver_ArtyWait : JobDriver
	{
		// Token: 0x06000044 RID: 68 RVA: 0x00003E04 File Offset: 0x00002004
		public override string GetReport()
		{
			bool flag = this.job.def != WPJobDefOf.ArtyWaitCombat;
			string result;
			if (flag)
			{
				result = base.GetReport();
			}
			else
			{
				bool flag2 = this.pawn.RaceProps.Humanlike && this.pawn.WorkTagIsDisabled(WorkTags.Violent);
				if (flag2)
				{
					result = "ReportStanding".Translate();
				}
				else
				{
					result = base.GetReport();
				}
			}
			return result;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003E78 File Offset: 0x00002078
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003E8B File Offset: 0x0000208B
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil wait = new Toil();
			wait.initAction = delegate()
			{
				base.Map.pawnDestinationReservationManager.Reserve(this.pawn, this.job, this.pawn.Position);
				this.pawn.pather.StopDead();
				this.CheckForAutoAttack();
			};
			wait.tickAction = delegate()
			{
				bool flag = this.job.expiryInterval == -1 && this.job.def == WPJobDefOf.ArtyWaitCombat && !this.pawn.Drafted;
				if (flag)
				{
					Pawn pawn = this.pawn;
					Log.Error(((pawn != null) ? pawn.ToString() : null) + " in eternal WaitCombat without being drafted.", false);
					base.ReadyForNextToil();
				}
				else
				{
					bool flag2 = (Find.TickManager.TicksGame + this.pawn.thingIDNumber) % 4 == 0;
					if (flag2)
					{
						this.CheckForAutoAttack();
					}
				}
			};
			this.DecorateWaitToil(wait);
			wait.defaultCompleteMode = ToilCompleteMode.Never;
			yield return wait;
			yield break;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003E9B File Offset: 0x0000209B
		public virtual void DecorateWaitToil(Toil wait)
		{
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003EA0 File Offset: 0x000020A0
		public override void Notify_StanceChanged()
		{
			bool flag = this.pawn.stances.curStance is Stance_Mobile;
			if (flag)
			{
				this.CheckForAutoAttack();
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003ED4 File Offset: 0x000020D4
		protected virtual bool ExtraTargetValidator(Pawn pawn, Thing target)
		{
			return true;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003EE8 File Offset: 0x000020E8
		private void CheckForAutoAttack()
		{
			bool downed = this.pawn.Downed;
			if (!downed)
			{
				bool fullBodyBusy = this.pawn.stances.FullBodyBusy;
				if (!fullBodyBusy)
				{
					bool flag = this.pawn.jobs.jobQueue != null;
					if (!flag)
					{
						bool flag2 = this.pawn.Faction != null && this.pawn.jobs.curJob.def == WPJobDefOf.ArtyWaitCombat;
						if (flag2)
						{
							Verb currentEffectiveVerb = this.pawn.CurrentEffectiveVerb;
							bool flag3 = currentEffectiveVerb != null && !currentEffectiveVerb.verbProps.IsMeleeAttack;
							if (flag3)
							{
								TargetScanFlags targetScanFlags = TargetScanFlags.None;
								bool flag4 = currentEffectiveVerb.IsIncendiary();
								if (flag4)
								{
									targetScanFlags |= TargetScanFlags.NeedNonBurning;
								}
								Thing thing = (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(this.pawn, targetScanFlags, null, 0f, 9999f);
								bool flag5 = thing != null;
								if (flag5)
								{
									this.pawn.TryStartAttack(thing);
									this.collideWithPawns = true;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0400001A RID: 26
		private const int TargetSearchInterval = 4;
	}
}
