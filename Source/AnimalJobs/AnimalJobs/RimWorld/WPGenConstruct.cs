using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200002D RID: 45
	public static class WPGenConstruct
	{
		// Token: 0x060000DD RID: 221 RVA: 0x00006F42 File Offset: 0x00005142
		public static void Reset()
		{
			WPGenConstruct.ConstructionSkillTooLowTrans = "ConstructionSkillTooLow".Translate();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00006F5C File Offset: 0x0000515C
		public static bool CanBuildOnTerrain(BuildableDef entDef, IntVec3 c, Map map, Rot4 rot, Thing thingToIgnore = null, ThingDef stuffDef = null)
		{
			bool flag = entDef is TerrainDef && !c.GetTerrain(map).changeable;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				TerrainAffordanceDef terrainAffordanceNeed = entDef.GetTerrainAffordanceNeed(stuffDef);
				bool flag2 = terrainAffordanceNeed != null;
				if (flag2)
				{
					CellRect cellRect = GenAdj.OccupiedRect(c, rot, entDef.Size);
					cellRect.ClipInsideMap(map);
					foreach (IntVec3 c2 in cellRect)
					{
						bool flag3 = !map.terrainGrid.TerrainAt(c2).affordances.Contains(terrainAffordanceNeed);
						if (flag3)
						{
							return false;
						}
						List<Thing> thingList = c2.GetThingList(map);
						for (int i = 0; i < thingList.Count; i++)
						{
							bool flag4 = thingList[i] != thingToIgnore;
							if (flag4)
							{
								TerrainDef terrainDef = thingList[i].def.entityDefToBuild as TerrainDef;
								bool flag5 = terrainDef != null && !terrainDef.affordances.Contains(terrainAffordanceNeed);
								if (flag5)
								{
									return false;
								}
							}
						}
					}
					result = true;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000070B4 File Offset: 0x000052B4
		public static Thing MiniToInstallOrBuildingToReinstall(Blueprint b)
		{
			Blueprint_Install blueprint_Install = b as Blueprint_Install;
			bool flag = blueprint_Install != null;
			Thing result;
			if (flag)
			{
				result = blueprint_Install.MiniToInstallOrBuildingToReinstall;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000070E0 File Offset: 0x000052E0
		public static bool CanConstruct(Thing t, Pawn p, bool forced = false)
		{
			bool flag = GenConstruct.FirstBlockingThing(t, p) != null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				LocalTargetInfo target = t;
				PathEndMode peMode = PathEndMode.Touch;
				Danger maxDanger = (!forced) ? p.NormalMaxDanger() : Danger.Deadly;
				bool flag2 = !p.CanReserveAndReach(target, peMode, maxDanger, 1, -1, null, forced);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = t.IsBurning();
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool animal = p.RaceProps.Animal;
						result = (!animal || true);
					}
				}
			}
			return result;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00007164 File Offset: 0x00005364
		public static int AmountNeededByOf(IConstructible c, ThingDef resDef)
		{
			foreach (ThingDefCountClass thingDefCountClass in c.MaterialsNeeded())
			{
				bool flag = thingDefCountClass.thingDef == resDef;
				if (flag)
				{
					return thingDefCountClass.count;
				}
			}
			return 0;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000071D4 File Offset: 0x000053D4
		public static AcceptanceReport CanPlaceBlueprintAt(BuildableDef entDef, IntVec3 center, Rot4 rot, Map map, bool godMode = false, Thing thingToIgnore = null, Thing thing = null, ThingDef stuffDef = null)
		{
			CellRect cellRect = GenAdj.OccupiedRect(center, rot, entDef.Size);
			bool flag = stuffDef == null && thing != null;
			if (flag)
			{
				stuffDef = thing.Stuff;
			}
			foreach (IntVec3 c in cellRect)
			{
				bool flag2 = !c.InBounds(map);
				if (flag2)
				{
					return new AcceptanceReport("OutOfBounds".Translate());
				}
				bool flag3 = c.InNoBuildEdgeArea(map) && !godMode;
				if (flag3)
				{
					return "TooCloseToMapEdge".Translate();
				}
			}
			bool flag4 = center.Fogged(map);
			AcceptanceReport result;
			if (flag4)
			{
				result = "CannotPlaceInUndiscovered".Translate();
			}
			else
			{
				List<Thing> thingList = center.GetThingList(map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing2 = thingList[i];
					bool flag5 = thing2 != thingToIgnore && thing2.Position == center && thing2.Rotation == rot;
					if (flag5)
					{
						bool flag6 = thing2.def == entDef;
						if (flag6)
						{
							return new AcceptanceReport("IdenticalThingExists".Translate());
						}
						bool flag7 = thing2.def.entityDefToBuild == entDef;
						if (flag7)
						{
							bool flag8 = thing2 is Blueprint;
							if (flag8)
							{
								return new AcceptanceReport("IdenticalBlueprintExists".Translate());
							}
							return new AcceptanceReport("IdenticalThingExists".Translate());
						}
					}
				}
				ThingDef thingDef = entDef as ThingDef;
				bool flag9 = thingDef != null && thingDef.hasInteractionCell;
				if (flag9)
				{
					IntVec3 c2 = ThingUtility.InteractionCellWhenAt(thingDef, center, rot, map);
					bool flag10 = !c2.InBounds(map);
					if (flag10)
					{
						return new AcceptanceReport("InteractionSpotOutOfBounds".Translate());
					}
					List<Thing> list = map.thingGrid.ThingsListAtFast(c2);
					for (int j = 0; j < list.Count; j++)
					{
						bool flag11 = list[j] != thingToIgnore;
						if (flag11)
						{
							bool flag12 = list[j].def.passability == Traversability.Impassable || list[j].def == thingDef;
							if (flag12)
							{
								return new AcceptanceReport("InteractionSpotBlocked".Translate(list[j].LabelNoCount, list[j]).CapitalizeFirst());
							}
							BuildableDef entityDefToBuild = list[j].def.entityDefToBuild;
							bool flag13 = entityDefToBuild != null && (entityDefToBuild.passability == Traversability.Impassable || entityDefToBuild == thingDef);
							if (flag13)
							{
								return new AcceptanceReport("InteractionSpotWillBeBlocked".Translate(list[j].LabelNoCount, list[j]).CapitalizeFirst());
							}
						}
					}
				}
				foreach (IntVec3 c3 in GenAdj.CellsAdjacentCardinal(center, rot, entDef.Size))
				{
					bool flag14 = c3.InBounds(map);
					if (flag14)
					{
						thingList = c3.GetThingList(map);
						for (int k = 0; k < thingList.Count; k++)
						{
							Thing thing3 = thingList[k];
							bool flag15 = thing3 != thingToIgnore;
							if (flag15)
							{
								Blueprint blueprint = thing3 as Blueprint;
								bool flag16 = blueprint != null;
								ThingDef thingDef3;
								if (flag16)
								{
									ThingDef thingDef2 = blueprint.def.entityDefToBuild as ThingDef;
									bool flag17 = thingDef2 == null;
									if (flag17)
									{
										goto IL_48F;
									}
									thingDef3 = thingDef2;
								}
								else
								{
									thingDef3 = thing3.def;
								}
								bool flag18 = thingDef3.hasInteractionCell && (entDef.passability == Traversability.Impassable || entDef == thingDef3) && cellRect.Contains(ThingUtility.InteractionCellWhenAt(thingDef3, thing3.Position, thing3.Rotation, thing3.Map));
								if (flag18)
								{
									return new AcceptanceReport("WouldBlockInteractionSpot".Translate(entDef.label, thingDef3.label).CapitalizeFirst());
								}
							}
							IL_48F:;
						}
					}
				}
				TerrainDef terrainDef = entDef as TerrainDef;
				bool flag19 = terrainDef != null;
				if (flag19)
				{
					bool flag20 = map.terrainGrid.TerrainAt(center) == terrainDef;
					if (flag20)
					{
						return new AcceptanceReport("TerrainIsAlready".Translate(terrainDef.label));
					}
					bool flag21 = map.designationManager.DesignationAt(center, DesignationDefOf.SmoothFloor) != null;
					if (flag21)
					{
						return new AcceptanceReport("SpaceBeingSmoothed".Translate());
					}
				}
				bool flag22 = WPGenConstruct.CanBuildOnTerrain(entDef, center, map, rot, thingToIgnore, stuffDef);
				if (flag22)
				{
					bool flag23 = !godMode;
					if (flag23)
					{
						foreach (IntVec3 c4 in cellRect)
						{
							thingList = c4.GetThingList(map);
							for (int l = 0; l < thingList.Count; l++)
							{
								Thing thing4 = thingList[l];
								bool flag24 = thing4 != thingToIgnore && !WPGenConstruct.CanPlaceBlueprintOver(entDef, thing4.def);
								if (flag24)
								{
									return new AcceptanceReport("SpaceAlreadyOccupied".Translate());
								}
							}
						}
					}
					bool flag25 = entDef.PlaceWorkers != null;
					if (flag25)
					{
						for (int m = 0; m < entDef.PlaceWorkers.Count; m++)
						{
							AcceptanceReport result2 = entDef.PlaceWorkers[m].AllowsPlacing(entDef, center, rot, map, thingToIgnore, thing);
							bool flag26 = !result2.Accepted;
							if (flag26)
							{
								return result2;
							}
						}
					}
					result = AcceptanceReport.WasAccepted;
				}
				else
				{
					bool flag27 = entDef.GetTerrainAffordanceNeed(stuffDef) == null;
					if (flag27)
					{
						result = new AcceptanceReport("TerrainCannotSupport".Translate(entDef).CapitalizeFirst());
					}
					else
					{
						bool flag28 = entDef.useStuffTerrainAffordance && stuffDef != null;
						if (flag28)
						{
							result = new AcceptanceReport("TerrainCannotSupport_TerrainAffordanceFromStuff".Translate(entDef, entDef.GetTerrainAffordanceNeed(stuffDef), stuffDef).CapitalizeFirst());
						}
						else
						{
							result = new AcceptanceReport("TerrainCannotSupport_TerrainAffordance".Translate(entDef, entDef.GetTerrainAffordanceNeed(stuffDef)).CapitalizeFirst());
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00007978 File Offset: 0x00005B78
		public static BuildableDef BuiltDefOf(ThingDef def)
		{
			return (def.entityDefToBuild == null) ? def : def.entityDefToBuild;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000799C File Offset: 0x00005B9C
		public static bool CanPlaceBlueprintOver(BuildableDef newDef, ThingDef oldDef)
		{
			bool everHaulable = oldDef.EverHaulable;
			bool result;
			if (everHaulable)
			{
				result = true;
			}
			else
			{
				TerrainDef terrainDef = newDef as TerrainDef;
				bool flag = terrainDef != null;
				if (flag)
				{
					bool flag2 = oldDef.category == ThingCategory.Building && !terrainDef.affordances.Contains(oldDef.terrainAffordanceNeeded);
					if (flag2)
					{
						return false;
					}
					bool flag3 = (oldDef.IsBlueprint || oldDef.IsFrame) && !terrainDef.affordances.Contains(oldDef.entityDefToBuild.terrainAffordanceNeeded);
					if (flag3)
					{
						return false;
					}
				}
				ThingDef thingDef = newDef as ThingDef;
				BuildableDef buildableDef = WPGenConstruct.BuiltDefOf(oldDef);
				ThingDef thingDef2 = buildableDef as ThingDef;
				bool flag4 = oldDef == ThingDefOf.SteamGeyser && !newDef.ForceAllowPlaceOver(oldDef);
				if (flag4)
				{
					result = false;
				}
				else
				{
					bool flag5 = oldDef.category == ThingCategory.Plant && oldDef.passability == Traversability.Impassable && thingDef != null && thingDef.category == ThingCategory.Building && !thingDef.building.canPlaceOverImpassablePlant;
					if (flag5)
					{
						result = false;
					}
					else
					{
						bool flag6 = oldDef.category == ThingCategory.Building || oldDef.IsBlueprint || oldDef.IsFrame;
						if (flag6)
						{
							bool flag7 = thingDef != null;
							if (flag7)
							{
								bool flag8 = !thingDef.IsEdifice();
								if (flag8)
								{
									return (oldDef.building == null || oldDef.building.canBuildNonEdificesUnder) && (!thingDef.EverTransmitsPower || !oldDef.EverTransmitsPower);
								}
								bool flag9 = thingDef.IsEdifice() && oldDef != null && oldDef.category == ThingCategory.Building && !oldDef.IsEdifice();
								if (flag9)
								{
									return thingDef.building == null || thingDef.building.canBuildNonEdificesUnder;
								}
								bool flag10 = thingDef2 != null && thingDef2 == ThingDefOf.Wall && thingDef.building != null && thingDef.building.canPlaceOverWall;
								if (flag10)
								{
									return true;
								}
								bool flag11 = newDef != ThingDefOf.PowerConduit && buildableDef == ThingDefOf.PowerConduit;
								if (flag11)
								{
									return true;
								}
							}
							result = ((newDef is TerrainDef && buildableDef is ThingDef && ((ThingDef)buildableDef).CoexistsWithFloors) || (buildableDef is TerrainDef && !(newDef is TerrainDef)));
						}
						else
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00007BFC File Offset: 0x00005DFC
		public static Thing FirstBlockingThing(Thing constructible, Pawn pawnToIgnore)
		{
			Blueprint blueprint = constructible as Blueprint;
			bool flag = blueprint != null;
			Thing thing;
			if (flag)
			{
				thing = WPGenConstruct.MiniToInstallOrBuildingToReinstall(blueprint);
			}
			else
			{
				thing = null;
			}
			foreach (IntVec3 c in constructible.OccupiedRect())
			{
				List<Thing> thingList = c.GetThingList(constructible.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Thing thing2 = thingList[i];
					bool flag2 = WPGenConstruct.BlocksConstruction(constructible, thing2) && thing2 != pawnToIgnore && thing2 != thing;
					if (flag2)
					{
						return thing2;
					}
				}
			}
			return null;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00007CD8 File Offset: 0x00005ED8
		public static Job HandleBlockingThingJob(Thing constructible, Pawn worker, bool forced = false)
		{
			Thing thing = WPGenConstruct.FirstBlockingThing(constructible, worker);
			bool flag = thing == null;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = thing.def.category == ThingCategory.Plant;
				if (flag2)
				{
					LocalTargetInfo target = thing;
					PathEndMode peMode = PathEndMode.ClosestTouch;
					Danger maxDanger = worker.NormalMaxDanger();
					bool flag3 = worker.CanReserveAndReach(target, peMode, maxDanger, 1, -1, null, forced);
					if (flag3)
					{
						return new Job(JobDefOf.CutPlant, thing);
					}
				}
				else
				{
					bool flag4 = thing.def.category == ThingCategory.Item;
					if (flag4)
					{
						bool everHaulable = thing.def.EverHaulable;
						if (everHaulable)
						{
							return HaulAIUtility.HaulAsideJobFor(worker, thing);
						}
						Log.ErrorOnce(string.Concat(new object[]
						{
							"Never haulable ",
							thing,
							" blocking ",
							constructible.ToStringSafe<Thing>(),
							" at ",
							constructible.Position
						}), 6429262, false);
					}
					else
					{
						bool flag5 = thing.def.category == ThingCategory.Building;
						if (flag5)
						{
							LocalTargetInfo target2 = thing;
							PathEndMode peMode2 = PathEndMode.Touch;
							Danger maxDanger2 = worker.NormalMaxDanger();
							bool flag6 = worker.CanReserveAndReach(target2, peMode2, maxDanger2, 1, -1, null, forced) && worker.RaceProps.Animal;
							if (flag6)
							{
								return new Job(WPJobDefOf.WPDeconstruct, thing)
								{
									ignoreDesignations = true
								};
							}
							bool flag7 = worker.CanReserveAndReach(target2, peMode2, maxDanger2, 1, -1, null, forced) && !worker.RaceProps.Animal;
							if (flag7)
							{
								return new Job(JobDefOf.Deconstruct, thing)
								{
									ignoreDesignations = true
								};
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00007E94 File Offset: 0x00006094
		public static bool BlocksConstruction(Thing constructible, Thing t)
		{
			bool flag = t == constructible;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = constructible is Blueprint;
				ThingDef thingDef;
				if (flag2)
				{
					thingDef = constructible.def;
				}
				else
				{
					bool flag3 = constructible is Frame;
					if (flag3)
					{
						thingDef = constructible.def.entityDefToBuild.blueprintDef;
					}
					else
					{
						thingDef = constructible.def.blueprintDef;
					}
				}
				bool flag4 = t.def.category == ThingCategory.Building && GenSpawn.SpawningWipes(thingDef.entityDefToBuild, t.def);
				if (flag4)
				{
					result = true;
				}
				else
				{
					bool flag5 = t.def.category == ThingCategory.Plant;
					if (flag5)
					{
						result = (t.def.plant.harvestWork >= 200f);
					}
					else
					{
						bool flag6 = !thingDef.clearBuildingArea;
						if (flag6)
						{
							result = false;
						}
						else
						{
							bool flag7 = t.def == ThingDefOf.SteamGeyser && thingDef.entityDefToBuild.ForceAllowPlaceOver(t.def);
							if (flag7)
							{
								result = false;
							}
							else
							{
								ThingDef thingDef2 = thingDef.entityDefToBuild as ThingDef;
								bool flag8 = thingDef2 != null;
								if (flag8)
								{
									bool flag9 = thingDef2.EverTransmitsPower && t.def == ThingDefOf.PowerConduit && thingDef2 != ThingDefOf.PowerConduit;
									if (flag9)
									{
										return false;
									}
									bool flag10 = t.def == ThingDefOf.Wall && thingDef2.building != null && thingDef2.building.canPlaceOverWall;
									if (flag10)
									{
										return false;
									}
								}
								result = ((t.def.IsEdifice() && thingDef2.IsEdifice()) || t.def.category == ThingCategory.Pawn || (t.def.category == ThingCategory.Item && thingDef.entityDefToBuild.passability == Traversability.Impassable) || t.def.Fillage >= FillCategory.Partial);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00008078 File Offset: 0x00006278
		public static bool TerrainCanSupport(CellRect rect, Map map, ThingDef thing)
		{
			foreach (IntVec3 c in rect)
			{
				bool flag = !c.SupportsStructureType(map, thing.terrainAffordanceNeeded);
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400002F RID: 47
		private static string ConstructionSkillTooLowTrans;
	}
}
