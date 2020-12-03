using System;
using RimWorld;
using Verse.AI;

namespace Verse
{
	// Token: 0x02000008 RID: 8
	public class JobGiver_WPPlantHarvest : ThinkNode_JobGiver
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002F24 File Offset: 0x00001124
		protected override Job TryGiveJob(Pawn pawn)
		{
			Region region = pawn.GetRegion(RegionType.Set_Passable);
			bool flag = region == null;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < 500; i++)
				{
					IntVec3 randomCell = region.RandomCell;
					for (int j = 0; j < 4; j++)
					{
						IntVec3 c = randomCell + GenAdj.CardinalDirections[j];
						Plant plant = c.GetPlant(pawn.Map);
						bool flag2 = plant != null && plant.def.plant.Harvestable && plant.LifeStage == PlantLifeStage.Mature && pawn.CanReserve(plant, 1, -1, null, false) && plant.IsCrop;
						if (flag2)
						{
							return new Job(JobDefOf.Harvest, plant);
						}
						foreach (Designation designation in pawn.Map.designationManager.AllDesignationsOn(plant))
						{
							bool flag3 = designation.def == DesignationDefOf.HarvestPlant && pawn.CanReserve(plant, 1, -1, null, false);
							if (flag3)
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
