using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	public class JobGiver_WPPlantHarvest : ThinkNode_JobGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			Region region = pawn.GetRegion(RegionType.Set_Passable);
			Job result;
			if (region == null) result = null;
			else
			{
				for (int i = 0; i < 500; i++)
				{
					IntVec3 randomCell = region.RandomCell;
					for (int j = 0; j < 4; j++)
					{
						IntVec3 c = randomCell + GenAdj.CardinalDirections[j];
						Plant plant = c.GetPlant(pawn.Map);
						if (plant != null && plant.def.plant.Harvestable && plant.LifeStage == PlantLifeStage.Mature && pawn.CanReserve(plant, 1, -1, null, false) && plant.IsCrop)
						{
							return new Job(JobDefOf.Harvest, plant);
						}
						foreach (Designation designation in pawn.Map.designationManager.AllDesignationsOn(plant))
						{
							if (designation.def == DesignationDefOf.HarvestPlant && pawn.CanReserve(plant, 1, -1, null, false))
							{
								return new Job(JobDefOf.Harvest, plant);
							}
						}
					}
				}
				result = null;
			}
			return result;
		}
	}
}
