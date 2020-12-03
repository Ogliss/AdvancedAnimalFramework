using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000029 RID: 41
	public class WorkGiver_WPTend : WorkGiver_Scanner
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000BA RID: 186 RVA: 0x000060B0 File Offset: 0x000042B0
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000060C4 File Offset: 0x000042C4
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000BC RID: 188 RVA: 0x000060D8 File Offset: 0x000042D8
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000060F4 File Offset: 0x000042F4
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			bool flag = pawn2 != null && (!this.def.tendToHumanlikesOnly || pawn2.RaceProps.Humanlike) && (!this.def.tendToAnimalsOnly || pawn2.RaceProps.Animal) && WorkGiver_Tend.GoodLayingStatusForTend(pawn2, pawn) && HealthAIUtility.ShouldBeTendedNowByPlayer(pawn2);
			if (flag)
			{
				LocalTargetInfo target = pawn2;
				bool flag2 = pawn.CanReserve(target, 1, -1, null, forced);
				if (flag2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000617C File Offset: 0x0000437C
		public static bool GoodLayingStatusForTend(Pawn patient, Pawn doctor)
		{
			bool flag = patient == doctor;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool humanlike = patient.RaceProps.Humanlike;
				if (humanlike)
				{
					result = patient.InBed();
				}
				else
				{
					result = (patient.GetPosture() > PawnPosture.Standing);
				}
			}
			return result;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000061C0 File Offset: 0x000043C0
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			Thing thing = null;
			bool flag = Medicine.GetMedicineCountToFullyHeal(pawn2) > 0;
			if (flag)
			{
				thing = HealthAIUtility.FindBestMedicine(pawn, pawn2);
			}
			bool flag2 = thing != null;
			Job result;
			if (flag2)
			{
				result = new Job(WPJobDefOf.WPTendPatient, pawn2, thing);
			}
			else
			{
				result = new Job(WPJobDefOf.WPTendPatient, pawn2);
			}
			return result;
		}
	}
}
