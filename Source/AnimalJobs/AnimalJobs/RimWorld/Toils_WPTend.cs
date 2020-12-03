using System;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200001B RID: 27
	public class Toils_WPTend
	{
		// Token: 0x06000070 RID: 112 RVA: 0x0000469C File Offset: 0x0000289C
		public static Toil PickupMedicine(TargetIndex ind, Pawn injured)
		{
			Toil toil = new Toil();
			toil.initAction = delegate()
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				Thing thing = curJob.GetTarget(ind).Thing;
				int num = Medicine.GetMedicineCountToFullyHeal(injured);
				bool flag = actor.carryTracker.CarriedThing != null;
				if (flag)
				{
					num -= actor.carryTracker.CarriedThing.stackCount;
				}
				int num2 = Mathf.Min(thing.stackCount, num);
				bool flag2 = num2 > 0;
				if (flag2)
				{
					actor.carryTracker.TryStartCarry(thing, num2, true);
				}
				curJob.count = num - num2;
				bool spawned = thing.Spawned;
				if (spawned)
				{
					toil.actor.Map.reservationManager.Release(thing, actor, curJob);
				}
				curJob.SetTarget(ind, actor.carryTracker.CarriedThing);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000046F8 File Offset: 0x000028F8
		public static Toil FinalizeTend(Pawn patient)
		{
			Toil toil = new Toil();
			toil.initAction = delegate()
			{
				Pawn actor = toil.actor;
				Medicine medicine = (Medicine)actor.jobs.curJob.targetB.Thing;
				float num = (!patient.RaceProps.Animal) ? 500f : 175f;
				TendUtility.DoTend(actor, patient, medicine);
				bool flag = medicine != null && medicine.Destroyed;
				if (flag)
				{
					actor.CurJob.SetTarget(TargetIndex.B, LocalTargetInfo.Invalid);
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
	}
}
