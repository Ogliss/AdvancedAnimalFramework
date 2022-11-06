using RimWorld;
using System;
using Verse;

namespace AnimalJobs
{
	public class WorkGiver_WPMilk : WorkGiver_WPGatherAnimalBodyResources
	{
		protected override JobDef JobDef
		{
			get
			{
				return WPJobDefOf.WPMilk;
			}
		}

		protected override CompHasGatherableBodyResource GetComp(Pawn animal)
		{
			return animal.TryGetComp<CompMilkable>();
		}
	}
}
