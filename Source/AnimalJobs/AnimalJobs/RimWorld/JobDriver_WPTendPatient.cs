using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000019 RID: 25
	public class JobDriver_WPTendPatient : JobDriver
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00004460 File Offset: 0x00002660
		protected Thing MedicineUsed
		{
			get
			{
				return this.job.targetB.Thing;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00004484 File Offset: 0x00002684
		protected Pawn Deliveree
		{
			get
			{
				return (Pawn)this.job.targetA.Thing;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000044AB File Offset: 0x000026AB
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.usesMedicine, "usesMedicine", false, false);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000044C8 File Offset: 0x000026C8
		public override void Notify_Starting()
		{
			base.Notify_Starting();
			this.usesMedicine = (this.MedicineUsed != null);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000044E4 File Offset: 0x000026E4
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			bool flag = this.Deliveree != this.pawn && !this.pawn.Reserve(this.Deliveree, this.job, 1, -1, null, errorOnFailed);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.usesMedicine;
				if (flag2)
				{
					int num = this.pawn.Map.reservationManager.CanReserveStack(this.pawn, this.MedicineUsed, 10, null, false);
					bool flag3 = num <= 0 || !this.pawn.Reserve(this.MedicineUsed, this.job, 10, Mathf.Min(num, Medicine.GetMedicineCountToFullyHeal(this.Deliveree)), null, errorOnFailed);
					if (flag3)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000045B4 File Offset: 0x000027B4
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOn(delegate()
			{
				bool flag4 = !WorkGiver_WPTend.GoodLayingStatusForTend(this.Deliveree, this.pawn);
				bool result;
				if (flag4)
				{
					result = true;
				}
				else
				{
					bool flag5 = this.MedicineUsed != null;
					if (flag5)
					{
						bool flag6 = this.Deliveree.playerSettings == null;
						if (flag6)
						{
							return true;
						}
						bool flag7 = !this.Deliveree.playerSettings.medCare.AllowsMedicine(this.MedicineUsed.def);
						if (flag7)
						{
							return true;
						}
					}
					result = (this.pawn == this.Deliveree && (this.pawn.playerSettings == null || !this.pawn.playerSettings.selfTend));
				}
				return result;
			});
			base.AddEndCondition(delegate
			{
				bool flag4 = HealthAIUtility.ShouldBeTendedNowByPlayer(this.Deliveree);
				JobCondition result;
				if (flag4)
				{
					result = JobCondition.Ongoing;
				}
				else
				{
					result = JobCondition.Succeeded;
				}
				return result;
			});
			this.FailOnAggroMentalState(TargetIndex.A);
			Toil reserveMedicine = null;
			bool flag = this.usesMedicine;
			if (flag)
			{
				reserveMedicine = Toils_Tend.ReserveMedicine(TargetIndex.B, this.Deliveree).FailOnDespawnedNullOrForbidden(TargetIndex.B);
				yield return reserveMedicine;
				yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B);
				yield return Toils_WPTend.PickupMedicine(TargetIndex.B, this.Deliveree).FailOnDestroyedOrNull(TargetIndex.B);
				yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveMedicine, TargetIndex.B, TargetIndex.None, true, null);
			}
			PathEndMode interactionCell = (this.Deliveree != this.pawn) ? PathEndMode.OnCell : PathEndMode.InteractionCell;
			Toil gotoToil = Toils_Goto.GotoThing(TargetIndex.A, interactionCell);
			yield return gotoToil;
			Toil toil = Toils_General.Wait((int)(1f / this.pawn.GetStatValue(StatDefOf.MedicalTendSpeed, true) * 600f), TargetIndex.None).FailOnCannotTouch(TargetIndex.A, interactionCell).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f).PlaySustainerOrSound(SoundDefOf.Interact_Tend);
			bool flag2 = this.pawn == this.Deliveree && this.pawn.Faction != Faction.OfPlayer;
			if (flag2)
			{
				toil.tickAction = delegate()
				{
					bool flag4 = this.pawn.IsHashIntervalTick(100) && !this.pawn.Position.Fogged(this.pawn.Map);
					if (flag4)
					{
						MoteMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, ThingDefOf.Mote_HealingCross);
					}
				};
			}
			yield return toil;
			yield return Toils_WPTend.FinalizeTend(this.Deliveree);
			bool flag3 = this.usesMedicine;
			if (flag3)
			{
				yield return new Toil
				{
					initAction = delegate()
					{
						bool flag4 = this.MedicineUsed.DestroyedOrNull();
						if (flag4)
						{
							Thing thing = HealthAIUtility.FindBestMedicine(this.pawn, this.Deliveree);
							bool flag5 = thing != null;
							if (flag5)
							{
								this.job.targetB = thing;
								this.JumpToToil(reserveMedicine);
							}
						}
					}
				};
			}
			yield return Toils_Jump.Jump(gotoToil);
			yield break;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000045C4 File Offset: 0x000027C4
		public override void Notify_DamageTaken(DamageInfo dinfo)
		{
			base.Notify_DamageTaken(dinfo);
			bool flag = dinfo.Def.ExternalViolenceFor(this.pawn) && this.pawn.Faction != Faction.OfPlayer && this.pawn == this.Deliveree;
			if (flag)
			{
				this.pawn.jobs.CheckForJobOverride();
			}
		}

		// Token: 0x04000018 RID: 24
		private bool usesMedicine;

		// Token: 0x04000019 RID: 25
		private const int BaseTendDuration = 600;

		// Token: 0x0400001A RID: 26
		private const int TicksBetweenSelfTendMotes = 100;
	}
}
