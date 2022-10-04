using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnimalJobs
{
	public static class WPGenRecipe
	{
		public static IEnumerable<Thing> MakeRecipeProducts(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient)
		{
			Log.Message(string.Concat(new string[]
			{
				"init WPGenRecipe: recipeDef ",
				(recipeDef != null) ? recipeDef.ToString() : null,
				" worker ",
				(worker != null) ? worker.ToString() : null,
				" ingredients ",
				(ingredients != null) ? ingredients.ToString() : null,
				" dominantIngredient ",
				(dominantIngredient != null) ? dominantIngredient.ToString() : null
			}));
			float efficiency;
			if (recipeDef.efficiencyStat == null) efficiency = 1f;
			else efficiency = worker.GetStatValue(recipeDef.efficiencyStat, true);
			if (recipeDef.products != null)
			{
				int num2;
				for (int i = 0; i < recipeDef.products.Count; i = num2 + 1)
				{
					ThingDefCountClass prod = recipeDef.products[i];
					ThingDef stuffDef;
					if (prod.thingDef.MadeFromStuff) stuffDef = dominantIngredient.def;
					else stuffDef = null;
					Thing product = ThingMaker.MakeThing(prod.thingDef, stuffDef);
					product.stackCount = Mathf.CeilToInt((float)prod.count * efficiency);
					if (dominantIngredient != null) product.SetColor(dominantIngredient.DrawColor, false);
					CompIngredients ingredientsComp = product.TryGetComp<CompIngredients>();
					if (ingredientsComp != null)
					{
						for (int j = 0; j < ingredients.Count; j = num2 + 1)
						{
							ingredientsComp.RegisterIngredient(ingredients[j].def);
							num2 = j;
						}
					}
					CompFoodPoisonable foodPoisonable = product.TryGetComp<CompFoodPoisonable>();
					if (foodPoisonable != null)
					{
						float num = worker.GetStatValue(StatDefOf.FoodPoisonChance, true);
						Room room = worker.GetRoom(RegionType.Set_Passable);
						float chance = (room == null) ? RoomStatDefOf.FoodPoisonChance.roomlessScore : room.GetStat(RoomStatDefOf.FoodPoisonChance);
						Rand.PushState();
						if (Rand.Chance(chance))
						{
							foodPoisonable.SetPoisoned(FoodPoisonCause.FilthyKitchen);
						}
						else
						{
							float statValue = worker.GetStatValue(StatDefOf.FoodPoisonChance, true);
							if (Rand.Chance(statValue))
							{
								foodPoisonable.SetPoisoned(FoodPoisonCause.IncompetentCook);
							}
						}
						Rand.PopState();
						room = null;
					}
					yield return WPGenRecipe.PostProcessProduct(product, recipeDef, worker);
					prod = null;
					stuffDef = null;
					product = null;
					ingredientsComp = null;
					foodPoisonable = null;
					num2 = i;
				}
			}
			if (recipeDef.specialProducts != null)
			{
				string str = "special recipedef not null";
				List<SpecialProductType> specialProducts = recipeDef.specialProducts;
			//	Log.Message(str + ((specialProducts != null) ? specialProducts.ToString() : null));
			//	Log.Message("recipeDef.specialProducts.Count " + recipeDef.specialProducts.Count.ToString());
				int num2;
				for (int k = 0; k < recipeDef.specialProducts.Count; k = num2 + 1)
				{
				//	Log.Message("recipeDef.specialProducts[j] " + recipeDef.specialProducts[k].ToString());
				//	Log.Message("ingredients.Count " + ingredients.Count.ToString());
					string str2 = "recipeDef.ingredients ";
					List<IngredientCount> ingredients2 = recipeDef.ingredients;
				//	Log.Message(str2 + ((ingredients2 != null) ? ingredients2.ToString() : null));
					for (int l = 0; l < ingredients.Count; l = num2 + 1)
					{
						Thing ing = ingredients[l];
						string str3 = "ingredients[k] ";
						Thing thing = ingredients[l];
					//	Log.Message(str3 + ((thing != null) ? thing.ToString() : null));
						string str4 = "ing ";
						Thing thing2 = ing;
					//	Log.Message(str4 + ((thing2 != null) ? thing2.ToString() : null));
						SpecialProductType specialProductType = recipeDef.specialProducts[k];
						if (specialProductType > SpecialProductType.Butchery)
						{
						//	Log.Message("not butchery");
							if (specialProductType == SpecialProductType.Smelted)
							{
								foreach (Thing product2 in ing.SmeltProducts(efficiency))
								{
									yield return WPGenRecipe.PostProcessProduct(product2, recipeDef, worker);
								}
							}
						}
						else
						{
						//	Log.Message("butchery");
							foreach (Thing product3 in ing.ButcherProducts(worker, efficiency))
							{
								string[] array = new string[8];
								array[0] = "recipeDef.specialProducts[k] ";
								array[1] = recipeDef.specialProducts[l].ToString();
								array[2] = "product3 ";
								int num3 = 3;
								Thing thing3 = product3;
								array[num3] = ((thing3 != null) ? thing3.ToString() : null);
								array[4] = " recipeDef ";
								array[5] = ((recipeDef != null) ? recipeDef.ToString() : null);
								array[6] = " worker ";
								array[7] = ((worker != null) ? worker.ToString() : null);
							//	Log.Message(string.Concat(array));
								yield return WPGenRecipe.PostProcessProduct(product3, recipeDef, worker);
							}
						}
						ing = null;
						num2 = l;
					}
					num2 = k;
				}
			}
		//	Log.Message("yield break");
			yield break;
		}

		private static Thing PostProcessProduct(Thing product, RecipeDef recipeDef, Pawn worker)
		{
			string[] array = new string[6];
			array[0] = "post processing: product ";
			int num = 1;
			Thing thing = product;
			array[num] = ((thing != null) ? thing.ToString() : null);
			array[2] = " recipeDef ";
			array[3] = ((recipeDef != null) ? recipeDef.ToString() : null);
			array[4] = " worker ";
			array[5] = ((worker != null) ? worker.ToString() : null);
		//	Log.Message(string.Concat(array));
			CompQuality compQuality = product.TryGetComp<CompQuality>();
			if (compQuality != null)
			{
				if (recipeDef.workSkill == null)
				{
					Log.Error(((recipeDef != null) ? recipeDef.ToString() : null) + " needs workSkill because it creates a product with a quality.");
				}
				int relevantSkillLevel = 1;
				QualityCategory q = QualityUtility.GenerateQualityCreatedByPawn(relevantSkillLevel, false);
				if (worker.InspirationDef == InspirationDefOf.Inspired_Creativity && (product.def.IsArt || (product.def.minifiedDef != null && product.def.minifiedDef.IsArt)))
				{
					int relevantSkillLevel2 = 4;
					q = QualityUtility.GenerateQualityCreatedByPawn(relevantSkillLevel2, false);
				}
				compQuality.SetQuality(q, ArtGenerationContext.Colony);
			}
			CompArt compArt = product.TryGetComp<CompArt>();
			if (compArt != null)
			{
				compArt.JustCreatedBy(worker);
			}
			if (product.def.Minifiable)
			{
				product = product.MakeMinified();
			}
			string[] array2 = new string[6];
			array2[0] = "end post processing:product ";
			int num2 = 1;
			Thing thing2 = product;
			array2[num2] = ((thing2 != null) ? thing2.ToString() : null);
			array2[2] = " recipeDef ";
			array2[3] = ((recipeDef != null) ? recipeDef.ToString() : null);
			array2[4] = " worker ";
			array2[5] = ((worker != null) ? worker.ToString() : null);
		//	Log.Message(string.Concat(array2));
			return product;
		}
	}
}
