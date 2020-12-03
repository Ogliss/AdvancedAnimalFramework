using System;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x0200000C RID: 12
	internal class Recipe_WPEquipWeaponOnAnimal : RecipeWorker
	{
		// Token: 0x0600002F RID: 47 RVA: 0x000038F0 File Offset: 0x00001AF0
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			bool flag = pawn.equipment.Primary != null;
			if (flag)
			{
				pawn.equipment.DropAllEquipment(pawn.Position, true);
			}
			for (int i = 0; i < ingredients.Count; i++)
			{
				Thing thing = ThingMaker.MakeThing(ingredients[i].def, ingredients[i].Stuff);
				GenSpawn.Spawn(thing, pawn.Position, pawn.Map, WipeMode.Vanish);
				ThingWithComps thingWithComps = (ThingWithComps)thing;
				bool flag2 = thingWithComps.def.stackLimit > 1 && thingWithComps.stackCount > 1;
				ThingWithComps thingWithComps2;
				if (flag2)
				{
					thingWithComps2 = (ThingWithComps)thing.SplitOff(1);
				}
				else
				{
					thingWithComps2 = thingWithComps;
					thingWithComps2.DeSpawn(DestroyMode.Vanish);
				}
				pawn.equipment.MakeRoomFor(thingWithComps2);
				pawn.equipment.AddEquipment(thingWithComps2);
				bool flag3 = thingWithComps2.def.soundInteract != null;
				if (flag3)
				{
					thingWithComps2.def.soundInteract.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map, false));
				}
			}
		}
	}
}
