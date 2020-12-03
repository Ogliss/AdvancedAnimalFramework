using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x0200002C RID: 44
	[StaticConstructorOnStartup]
	public class WPFrame : Building, IThingHolder, IConstructible
	{
		// Token: 0x060000C9 RID: 201 RVA: 0x00006319 File Offset: 0x00004519
		public WPFrame()
		{
			this.resourceContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000CA RID: 202 RVA: 0x0000633C File Offset: 0x0000453C
		public float WorkToMake
		{
			get
			{
				return this.def.entityDefToBuild.GetStatValueAbstract(StatDefOf.WorkToBuild, base.Stuff);
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000CB RID: 203 RVA: 0x0000636C File Offset: 0x0000456C
		public float WorkLeft
		{
			get
			{
				return this.WorkToMake - this.workDone;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000CC RID: 204 RVA: 0x0000638C File Offset: 0x0000458C
		public float PercentComplete
		{
			get
			{
				return this.workDone / this.WorkToMake;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000CD RID: 205 RVA: 0x000063AC File Offset: 0x000045AC
		public override string Label
		{
			get
			{
				string text = this.def.entityDefToBuild.label + "FrameLabelExtra".Translate();
				bool flag = base.Stuff != null;
				string result;
				if (flag)
				{
					result = base.Stuff.label + " " + text;
				}
				else
				{
					result = text;
				}
				return result;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000CE RID: 206 RVA: 0x0000640C File Offset: 0x0000460C
		public override Color DrawColor
		{
			get
			{
				bool flag = !this.def.MadeFromStuff;
				Color result;
				if (flag)
				{
					List<ThingDefCountClass> costList = this.def.entityDefToBuild.costList;
					bool flag2 = costList != null;
					if (flag2)
					{
						for (int i = 0; i < costList.Count; i++)
						{
							ThingDef thingDef = costList[i].thingDef;
							bool flag3 = thingDef.IsStuff && thingDef.stuffProps.color != Color.white;
							if (flag3)
							{
								return thingDef.stuffProps.color;
							}
						}
					}
					result = new Color(0.6f, 0.6f, 0.6f);
				}
				else
				{
					result = base.DrawColor;
				}
				return result;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000CF RID: 207 RVA: 0x000064D0 File Offset: 0x000046D0
		public EffecterDef ConstructionEffect
		{
			get
			{
				bool flag = base.Stuff != null && base.Stuff.stuffProps.constructEffect != null;
				EffecterDef result;
				if (flag)
				{
					result = base.Stuff.stuffProps.constructEffect;
				}
				else
				{
					bool flag2 = this.def.entityDefToBuild.constructEffect != null;
					if (flag2)
					{
						result = this.def.entityDefToBuild.constructEffect;
					}
					else
					{
						result = EffecterDefOf.ConstructMetal;
					}
				}
				return result;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00006548 File Offset: 0x00004748
		private Material CornerMat
		{
			get
			{
				bool flag = this.cachedCornerMat == null;
				if (flag)
				{
					this.cachedCornerMat = MaterialPool.MatFrom(WPFrame.CornerTex, ShaderDatabase.Cutout, this.DrawColor);
				}
				return this.cachedCornerMat;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00006590 File Offset: 0x00004790
		private Material TileMat
		{
			get
			{
				bool flag = this.cachedTileMat == null;
				if (flag)
				{
					this.cachedTileMat = MaterialPool.MatFrom(WPFrame.TileTex, ShaderDatabase.Cutout, this.DrawColor);
				}
				return this.cachedTileMat;
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x000065D8 File Offset: 0x000047D8
		public ThingOwner GetDirectlyHeldThings()
		{
			return this.resourceContainer;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x000065F0 File Offset: 0x000047F0
		public void GetChildHolders(List<IThingHolder> outChildren)
		{
			ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00006600 File Offset: 0x00004800
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.workDone, "workDone", 0f, false);
			Scribe_Deep.Look<ThingOwner>(ref this.resourceContainer, "resourceContainer", new object[]
			{
				this
			});
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000663C File Offset: 0x0000483C
		public ThingDef UIStuff()
		{
			return base.Stuff;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00006654 File Offset: 0x00004854
		public List<ThingDefCountClass> MaterialsNeeded()
		{
			this.cachedMaterialsNeeded.Clear();
			List<ThingDefCountClass> list = this.def.entityDefToBuild.CostListAdjusted(base.Stuff, true);
			for (int i = 0; i < list.Count; i++)
			{
				ThingDefCountClass thingDefCountClass = list[i];
				int num = this.resourceContainer.TotalStackCountOfDef(thingDefCountClass.thingDef);
				int num2 = thingDefCountClass.count - num;
				bool flag = num2 > 0;
				if (flag)
				{
					this.cachedMaterialsNeeded.Add(new ThingDefCountClass(thingDefCountClass.thingDef, num2));
				}
			}
			return this.cachedMaterialsNeeded;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000066F8 File Offset: 0x000048F8
		public void CompleteConstruction(Pawn worker)
		{
			Map map = base.Map;
			bool flag = this.GetStatValue(StatDefOf.WorkToBuild, true) > 150f && this.def.entityDefToBuild is ThingDef && ((ThingDef)this.def.entityDefToBuild).category == ThingCategory.Building;
			if (flag)
			{
				SoundDefOf.Building_Complete.PlayOneShot(new TargetInfo(base.Position, map, false));
			}
			ThingDef thingDef = this.def.entityDefToBuild as ThingDef;
			Thing thing = null;
			bool flag2 = thingDef != null;
			if (flag2)
			{
				thing = ThingMaker.MakeThing(thingDef, base.Stuff);
				thing.SetFactionDirect(base.Faction);
				CompQuality compQuality = thing.TryGetComp<CompQuality>();
				bool flag3 = compQuality != null;
				if (flag3)
				{
					int relevantSkillLevel = 1;
					compQuality.SetQuality(QualityUtility.GenerateQualityCreatedByPawn(relevantSkillLevel, false), ArtGenerationContext.Colony);
				}
				CompArt compArt = thing.TryGetComp<CompArt>();
				bool flag4 = compArt != null;
				if (flag4)
				{
					bool flag5 = compQuality == null;
					if (flag5)
					{
						compArt.InitializeArt(ArtGenerationContext.Colony);
					}
					compArt.JustCreatedBy(worker);
				}
				thing.HitPoints = Mathf.CeilToInt((float)this.HitPoints / (float)base.MaxHitPoints * (float)thing.MaxHitPoints);
				GenSpawn.Spawn(thing, base.Position, map, base.Rotation, WipeMode.FullRefund, false);
			}
			else
			{
				map.terrainGrid.SetTerrain(base.Position, (TerrainDef)this.def.entityDefToBuild);
				FilthMaker.RemoveAllFilth(base.Position, map);
			}
			bool flag6 = thingDef != null && (thingDef.passability == Traversability.Impassable || thingDef.Fillage == FillCategory.Full) && (thing == null || !(thing is Building_Door));
			if (flag6)
			{
				foreach (IntVec3 c in GenAdj.CellsOccupiedBy(base.Position, base.Rotation, this.def.Size))
				{
					foreach (Thing thing2 in map.thingGrid.ThingsAt(c).ToList<Thing>())
					{
						bool flag7 = thing2 is Plant;
						if (flag7)
						{
							thing2.Destroy(DestroyMode.KillFinalize);
						}
						else
						{
							bool flag8 = thing2.def.category == ThingCategory.Item || thing2 is Pawn;
							if (flag8)
							{
								GenPlace.TryPlaceThing(thing2, thing2.Position, thing2.Map, ThingPlaceMode.Near, null, null, default(Rot4));
							}
						}
					}
				}
			}
			worker.records.Increment(RecordDefOf.ThingsConstructed);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000069CC File Offset: 0x00004BCC
		public override void Draw()
		{
			Vector2 vector = new Vector2((float)this.def.size.x, (float)this.def.size.z);
			vector.x *= 1.15f;
			vector.y *= 1.15f;
			Vector3 s = new Vector3(vector.x, 1f, vector.y);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(this.DrawPos, base.Rotation.AsQuat, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, WPFrame.UnderfieldMat, 0);
			int num = 4;
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)Mathf.Min(base.RotatedSize.x, base.RotatedSize.z);
				float num3 = num2 * 0.38f;
				IntVec3 intVec = default(IntVec3);
				bool flag = i == 0;
				if (flag)
				{
					intVec = new IntVec3(-1, 0, -1);
				}
				else
				{
					bool flag2 = i == 1;
					if (flag2)
					{
						intVec = new IntVec3(-1, 0, 1);
					}
					else
					{
						bool flag3 = i == 2;
						if (flag3)
						{
							intVec = new IntVec3(1, 0, 1);
						}
						else
						{
							bool flag4 = i == 3;
							if (flag4)
							{
								intVec = new IntVec3(1, 0, -1);
							}
						}
					}
				}
				Vector3 b = default(Vector3);
				b.x = (float)intVec.x * ((float)base.RotatedSize.x / 2f - num3 / 2f);
				b.z = (float)intVec.z * ((float)base.RotatedSize.z / 2f - num3 / 2f);
				Vector3 s2 = new Vector3(num3, 1f, num3);
				Matrix4x4 matrix2 = default(Matrix4x4);
				Vector3 pos = this.DrawPos + Vector3.up * 0.03f + b;
				Rot4 rot = new Rot4(i);
				matrix2.SetTRS(pos, rot.AsQuat, s2);
				Graphics.DrawMesh(MeshPool.plane10, matrix2, this.CornerMat, 0);
			}
			float num4 = this.PercentComplete / 1f;
			int num5 = Mathf.CeilToInt(num4 * (float)base.RotatedSize.x * (float)base.RotatedSize.z * 4f);
			IntVec2 intVec2 = base.RotatedSize * 2;
			for (int j = 0; j < num5; j++)
			{
				IntVec2 intVec3 = default(IntVec2);
				intVec3.z = j / intVec2.x;
				intVec3.x = j - intVec3.z * intVec2.x;
				Vector3 a = new Vector3((float)intVec3.x * 0.5f, 0f, (float)intVec3.z * 0.5f) + this.DrawPos;
				a.x -= (float)base.RotatedSize.x * 0.5f - 0.25f;
				a.z -= (float)base.RotatedSize.z * 0.5f - 0.25f;
				Vector3 s3 = new Vector3(0.5f, 1f, 0.5f);
				Matrix4x4 matrix3 = default(Matrix4x4);
				matrix3.SetTRS(a + Vector3.up * 0.02f, Quaternion.identity, s3);
				Graphics.DrawMesh(MeshPool.plane10, matrix3, this.TileMat, 0);
			}
			base.Comps_PostDraw();
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00006D5C File Offset: 0x00004F5C
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.GetInspectString());
			stringBuilder.AppendLine(Translator.Translate("ContainedResources") + ":");
			List<ThingDefCountClass> list = CostListCalculator.CostListAdjusted(base.def.entityDefToBuild, this.Stuff, true);
			for (int i = 0; i < list.Count; i++)
			{
				ThingDefCountClass need = list[i];
				int num = need.count;
				foreach (ThingDefCountClass item in from needed in MaterialsNeeded()
													where needed.thingDef == need.thingDef
													select needed)
				{
					num -= item.count;
				}
				stringBuilder.AppendLine(need.thingDef.LabelCap + ": " + num + " / " + need.count);
			}
			stringBuilder.Append(Translator.Translate("WorkLeft") + ": " + GenText.ToStringWorkAmount(WorkLeft));
			return stringBuilder.ToString();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006EFC File Offset: 0x000050FC
		List<ThingDefCountClass> IConstructible.MaterialsNeeded()
		{
			throw new NotImplementedException();
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00006F04 File Offset: 0x00005104
		ThingDef IConstructible.EntityToBuildStuff()
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000024 RID: 36
		public ThingOwner resourceContainer;

		// Token: 0x04000025 RID: 37
		public float workDone;

		// Token: 0x04000026 RID: 38
		private Material cachedCornerMat;

		// Token: 0x04000027 RID: 39
		private Material cachedTileMat;

		// Token: 0x04000028 RID: 40
		protected const float UnderfieldOverdrawFactor = 1.15f;

		// Token: 0x04000029 RID: 41
		protected const float CenterOverdrawFactor = 0.5f;

		// Token: 0x0400002A RID: 42
		private const int LongConstructionProjectThreshold = 10000;

		// Token: 0x0400002B RID: 43
		private static readonly Material UnderfieldMat = MaterialPool.MatFrom("Things/Building/BuildingFrame/Underfield", ShaderDatabase.Transparent);

		// Token: 0x0400002C RID: 44
		private static readonly Texture2D CornerTex = ContentFinder<Texture2D>.Get("Things/Building/BuildingFrame/Corner", true);

		// Token: 0x0400002D RID: 45
		private static readonly Texture2D TileTex = ContentFinder<Texture2D>.Get("Things/Building/BuildingFrame/Tile", true);

		// Token: 0x0400002E RID: 46
		private List<ThingDefCountClass> cachedMaterialsNeeded = new List<ThingDefCountClass>();
	}
}
