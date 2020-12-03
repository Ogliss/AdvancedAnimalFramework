using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x0200000D RID: 13
	internal class Recipe_WPRemoveWeaponOnAnimal : RecipeWorker
	{
		// Token: 0x06000031 RID: 49 RVA: 0x00003A28 File Offset: 0x00001C28
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			bool flag = pawn.equipment.Primary != null;
			if (flag)
			{
				pawn.equipment.DropAllEquipment(pawn.Position, true);
			}
		}
	}
}
