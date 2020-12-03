using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200000B RID: 11
	public class JobGiver_WPVehicleAutoWeapon : ThinkNode_JobGiver
	{
		// Token: 0x0600002D RID: 45 RVA: 0x0000379C File Offset: 0x0000199C
		protected override Job TryGiveJob(Pawn pawn)
		{
			bool flag = pawn.equipment.HasAnything();
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool downed = pawn.Downed;
				if (downed)
				{
					result = null;
				}
				else
				{
					bool flag2 = pawn.RaceProps.Humanlike && pawn.WorkTagIsDisabled(WorkTags.Violent);
					if (flag2)
					{
						result = null;
					}
					else
					{
						bool flag3 = !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
						if (flag3)
						{
							result = null;
						}
						else
						{
							bool flag4 = pawn.GetRegion(RegionType.Set_Passable) == null;
							if (flag4)
							{
								result = null;
							}
							else
							{
								Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Weapon), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 8f, (Thing x) => pawn.CanReserve(x, 1, -1, null, false), null, 0, 15, false, RegionType.Set_Passable, false);
								bool flag5 = thing != null && thing.def.IsRangedWeapon && thing.def.techLevel > TechLevel.Medieval;
								if (flag5)
								{
									result = new Job(JobGiver_WPVehicleAutoWeapon.WPAnimalJobDefOf.AnimalWeaponEquip, thing);
								}
								else
								{
									result = null;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x02000024 RID: 36
		[DefOf]
		public static class WPAnimalJobDefOf
		{
			// Token: 0x04000029 RID: 41
			public static JobDef AnimalWeaponEquip;
		}
	}
}
