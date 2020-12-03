using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Verse.DamageWorker;

namespace RimWorld
{
	// Token: 0x02000007 RID: 7
	internal class Recipe_WPRepairVehicle : RecipeWorker
	{
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			float num = pawn.health.hediffSet.hediffs.Count;
			for (int i = 0; (float)i < num; i++)
			{
				if (!GenCollection.TryRandomElement<BodyPartRecord>(pawn.health.hediffSet.GetInjuredParts(), out part))
				{
					break;
				}
				Hediff_Injury val = null;
				foreach (Hediff_Injury item in from x in pawn.health.hediffSet.GetHediffs<Hediff_Injury>()
											   where x.Part == part
											   select x)
				{
					if (val != null)
					{
						((Hediff)val).Heal(2000f);
						if (val != null)
						{
							pawn.health.RemoveHediff((Hediff)(object)val);
						}
					}
				}
				pawn.health.AddHediff(WPVehicleHediffDefOf.WPVehicleReboot, (BodyPartRecord)null, (DamageInfo?)null, (DamageResult)null);
			}
		}
		// Token: 0x02000009 RID: 9
		[DefOf]
		public static class WPVehicleHediffDefOf
		{
			// Token: 0x04000003 RID: 3
			public static HediffDef WPVehicleReboot;
		}
	}
}
