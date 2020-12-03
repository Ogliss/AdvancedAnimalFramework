using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AnimalWeaponPatch
{
	// Token: 0x02000019 RID: 25
	[StaticConstructorOnStartup]
	internal static class HarmonyPatches
	{
		// Token: 0x06000059 RID: 89 RVA: 0x0000444C File Offset: 0x0000264C
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("rimworld.walkingproblem.animalweaponpatch");
			MethodInfo original = AccessTools.Method(typeof(PawnWeaponGenerator), "TryGenerateWeaponFor", null, null);
			MethodInfo original2 = AccessTools.Method(typeof(PawnComponentsUtility), "CreateInitialComponents", null, null);
			MethodInfo original3 = AccessTools.Method(typeof(PawnComponentsUtility), "AddAndRemoveDynamicComponents", null, null);
			MethodInfo original4 = AccessTools.Method(typeof(PawnComponentsUtility), "AddComponentsForSpawn", null, null);
			HarmonyMethod prefix = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("TryGenerateWeaponFor_Prefix"));
			HarmonyMethod prefix2 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CreateInitialComponents_Prefix"));
			HarmonyMethod prefix3 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("AddAndRemoveDynamicComponents_Prefix"));
			HarmonyMethod prefix4 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("AddComponentsForSpawn_Prefix"));
			harmony.Patch(original, prefix, null, null, null);
			harmony.Patch(original2, prefix2, null, null, null);
			harmony.Patch(original3, prefix3, null, null, null);
			harmony.Patch(original4, prefix4, null, null, null);
			harmony.Patch(AccessTools.Property(typeof(ITab_Pawn_Gear), "IsVisible").GetGetMethod(false), null, new HarmonyMethod(HarmonyPatches.patchType, "IsVisible_Prefix", null), null, null);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000045A8 File Offset: 0x000027A8
		public static void TryGenerateWeaponFor_Prefix(Pawn pawn)
		{
			List<ThingStuffPair> value = Traverse.Create(typeof(PawnWeaponGenerator)).Field("workingWeapons").GetValue<List<ThingStuffPair>>();
			List<ThingStuffPair> value2 = Traverse.Create(typeof(PawnWeaponGenerator)).Field("allWeaponPairs").GetValue<List<ThingStuffPair>>();
			value.Clear();
			bool flag = pawn.kindDef.weaponTags == null || pawn.kindDef.weaponTags.Count == 0;
			if (!flag)
			{
				bool flag2 = !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
				if (!flag2)
				{
					bool flag3 = pawn.story != null && pawn.WorkTagIsDisabled(WorkTags.Violent);
					if (!flag3)
					{
						float randomInRange = pawn.kindDef.weaponMoney.RandomInRange;
						for (int i = 0; i < value2.Count; i++)
						{
							ThingStuffPair w = value2[i];
							bool flag4 = w.Price <= randomInRange;
							if (flag4)
							{
								bool flag5 = pawn.kindDef.weaponTags.Any((string tag) => w.thing.weaponTags.Contains(tag));
								if (flag5)
								{
									bool flag6 = w.thing.generateAllowChance >= 1f || Rand.ValueSeeded(pawn.thingIDNumber ^ 28554824) <= w.thing.generateAllowChance;
									if (flag6)
									{
										value.Add(w);
									}
								}
							}
						}
						bool flag7 = value.Count == 0;
						if (!flag7)
						{
							pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
							ThingStuffPair thingStuffPair;
							bool flag8 = value.TryRandomElementByWeight((ThingStuffPair w) => w.Commonality * w.Price, out thingStuffPair);
							if (flag8)
							{
								ThingWithComps thingWithComps = (ThingWithComps)ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
								PawnGenerator.PostProcessGeneratedGear(thingWithComps, pawn);
								pawn.equipment.AddEquipment(thingWithComps);
							}
							value.Clear();
						}
					}
				}
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000047D4 File Offset: 0x000029D4
		public static void CreateInitialComponents_Prefix(Pawn pawn)
		{
			bool animal = pawn.RaceProps.Animal;
			if (animal)
			{
				bool flag = pawn.equipment == null;
				if (flag)
				{
					pawn.equipment = new Pawn_EquipmentTracker(pawn);
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004810 File Offset: 0x00002A10
		public static void AddAndRemoveDynamicComponents_Prefix(Pawn pawn)
		{
			bool flag = pawn.RaceProps.Animal && pawn.equipment == null;
			if (flag)
			{
				pawn.equipment = new Pawn_EquipmentTracker(pawn);
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000484C File Offset: 0x00002A4C
		public static void AddComponentsForSpawn_Prefix(Pawn pawn)
		{
			bool flag = pawn.equipment == null;
			if (flag)
			{
				pawn.equipment = new Pawn_EquipmentTracker(pawn);
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004878 File Offset: 0x00002A78
		public static void IsVisible_Prefix(ref bool __result, ITab_Pawn_Gear __instance)
		{
			Pawn value = Traverse.Create(__instance).Property("SelPawnForGear", null).GetValue<Pawn>();
			__result = (value.RaceProps.ToolUser || value.inventory.innerContainer.Count > 0 || value.equipment != null);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000048CC File Offset: 0x00002ACC
		public static void InAggroMentalStatee_Prefix(ref bool __result, Pawn __instance)
		{
			bool flag = __instance.RaceProps.intelligence < Intelligence.ToolUser && __instance.equipment.PrimaryEq != null && !HealthAIUtility.ShouldSeekMedicalRest(__instance);
			if (flag)
			{
				__result = true;
			}
			else
			{
				bool flag2 = !__instance.Dead && __instance.mindState.mentalStateHandler.InMentalState && __instance.mindState.mentalStateHandler.CurStateDef.IsAggro;
				if (flag2)
				{
					__result = true;
				}
			}
		}

		// Token: 0x0400001C RID: 28
		private static readonly Type patchType = typeof(HarmonyPatches);
	}
}
