using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WPVehiclePatch
{
	// Token: 0x02000002 RID: 2
	[StaticConstructorOnStartup]
	internal static class HarmonyPatches
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("rimworld.walkingproblem.wpvehiclepatch");
			harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "HasHediffsNeedingTend", null, null), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("HasHediffsNeedingTend_Prefix")), null, null, null);
			harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "HasHediffsNeedingTendByPlayer", null, null), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("HasHediffsNeedingTendByPlayer_Prefix")), null, null, null);
			harmony.Patch(AccessTools.Method(typeof(RestUtility), "CanUseBedEver", null, null), new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CanUseBedEver_Prefix")), null, null, null);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002124 File Offset: 0x00000324
		public static void HasHediffsNeedingTend_Prefix(ref bool __result, Pawn_HealthTracker __instance, bool forAlert = false)
		{
			Pawn value = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
			bool flag = value.kindDef.race.GetStatValueAbstract(WPVehicleStatDefOf.WPVehicle, null) == 1f;
			if (flag)
			{
				__result = false;
			}
			else
			{
				bool flag2 = __instance.hediffSet.HasTendableHediff(forAlert);
				if (flag2)
				{
					__result = true;
				}
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002188 File Offset: 0x00000388
		public static void HasHediffsNeedingTendByPlayer_Prefix(ref bool __result, Pawn_HealthTracker __instance, bool forAlert = false)
		{
			Pawn value = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
			bool flag = value.kindDef.race.GetStatValueAbstract(WPVehicleStatDefOf.WPVehicle, null) == 1f;
			if (flag)
			{
				__result = false;
			}
			else
			{
				bool flag2 = __instance.HasHediffsNeedingTend(forAlert);
				if (flag2)
				{
					bool flag3 = value.NonHumanlikeOrWildMan();
					if (flag3)
					{
						bool flag4 = value.Faction == Faction.OfPlayer;
						if (flag4)
						{
							__result = true;
						}
						Building_Bed building_Bed = value.CurrentBed();
						bool flag5 = building_Bed != null && building_Bed.Faction == Faction.OfPlayer;
						if (flag5)
						{
							__result = true;
						}
					}
					else
					{
						bool flag6 = (value.Faction == Faction.OfPlayer && value.HostFaction == null) || value.HostFaction == Faction.OfPlayer;
						if (flag6)
						{
							__result = true;
						}
					}
				}
			}
			__result = false;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002268 File Offset: 0x00000468
		public static void CanUseBedEver_Prefix(ref bool __result, Pawn p, ThingDef bedDef)
		{
			__result = (p.BodySize <= bedDef.building.bed_maxBodySize && p.RaceProps.Humanlike == bedDef.building.bed_humanlike && p.kindDef.race.GetStatValueAbstract(WPVehicleStatDefOf.WPVehicle, null) == bedDef.GetStatValueAbstract(WPVehicleStatDefOf.WPVehicle, null));
		}

		// Token: 0x04000001 RID: 1
		private static readonly Type patchType = typeof(HarmonyPatches);
	}
}
