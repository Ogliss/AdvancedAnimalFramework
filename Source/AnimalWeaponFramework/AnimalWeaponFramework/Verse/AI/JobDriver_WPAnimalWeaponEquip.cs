using System;
using System.Collections.Generic;
using Verse.Sound;

namespace Verse.AI
{
	// Token: 0x02000017 RID: 23
	public class JobDriver_WPAnimalWeaponEquip : JobDriver
	{
		// Token: 0x0600004E RID: 78 RVA: 0x000040F4 File Offset: 0x000022F4
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo targetA = this.job.targetA;
			Job job = this.job;
			return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000412C File Offset: 0x0000232C
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return new Toil
			{
				initAction = delegate()
				{
					ThingWithComps thingWithComps = (ThingWithComps)this.job.targetA.Thing;
					bool flag = thingWithComps.def.stackLimit > 1 && thingWithComps.stackCount > 1;
					ThingWithComps thingWithComps2;
					if (flag)
					{
						thingWithComps2 = (ThingWithComps)thingWithComps.SplitOff(1);
					}
					else
					{
						thingWithComps2 = thingWithComps;
						thingWithComps2.DeSpawn(DestroyMode.Vanish);
					}
					this.pawn.equipment.MakeRoomFor(thingWithComps2);
					this.pawn.equipment.AddEquipment(thingWithComps2);
					bool flag2 = thingWithComps.def.soundInteract != null;
					if (flag2)
					{
						thingWithComps.def.soundInteract.PlayOneShot(new TargetInfo(this.pawn.Position, this.pawn.Map, false));
					}
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield break;
		}
	}
}
