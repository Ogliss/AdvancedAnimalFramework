using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000009 RID: 9
	public class JobGiver_WPAutoMeleeWeapon : ThinkNode_JobGiver
	{
		// Token: 0x06000029 RID: 41 RVA: 0x000031F0 File Offset: 0x000013F0
		protected override Job TryGiveJob(Pawn pawn)
		{
			bool flag = pawn.Downed || pawn.CurJob != null;
			Job result;
			if (flag)
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
							bool flag5 = pawn.equipment.HasAnything();
							if (flag5)
							{
								Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Weapon), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 15f, (Thing x) => pawn.CanReserve(x, 1, -1, null, false), null, 0, 15, false, RegionType.Set_Passable, false);
								bool flag6 = thing == null;
								if (flag6)
								{
									return null;
								}
								bool flag7 = pawn.equipment.Primary.def != null;
								if (flag7)
								{
									ThingDef def = pawn.equipment.Primary.def;
									bool flag8 = thing.def.BaseMarketValue <= def.BaseMarketValue;
									if (flag8)
									{
										return null;
									}
									bool flag9 = thing != null && !thing.IsBurning() && !thing.IsForbidden(pawn) && thing.def.IsMeleeWeapon && thing.def.BaseMarketValue > def.BaseMarketValue && pawn.CurJob == null;
									if (flag9)
									{
										pawn.equipment.DropAllEquipment(pawn.Position, false);
										return new Job(JobGiver_WPAutoMeleeWeapon.WPAnimalJobDefOf.AnimalWeaponEquip, thing);
									}
								}
							}
							bool flag10 = !pawn.equipment.HasAnything();
							if (flag10)
							{
								Thing thing2 = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Weapon), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 8f, (Thing x) => pawn.CanReserve(x, 1, -1, null, false), null, 0, 15, false, RegionType.Set_Passable, false);
								bool flag11 = thing2 != null && thing2.def.IsMeleeWeapon && !thing2.IsBurning() && !thing2.IsForbidden(pawn) && pawn.CurJob == null;
								if (flag11)
								{
									return new Job(JobGiver_WPAutoMeleeWeapon.WPAnimalJobDefOf.AnimalWeaponEquip, thing2);
								}
							}
							result = null;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x02000020 RID: 32
		[DefOf]
		public static class WPAnimalJobDefOf
		{
			// Token: 0x04000025 RID: 37
			public static JobDef AnimalWeaponEquip;
		}
	}
}
