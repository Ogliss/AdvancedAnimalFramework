using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000005 RID: 5
	public class JobGiver_WPAutoSkill : ThinkNode_JobGiver
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000023F8 File Offset: 0x000005F8
		protected override Job TryGiveJob(Pawn pawn)
		{
			pawn.training.SetWantedRecursive(TrainableDefOf.Obedience, true);
			bool flag = pawn.training.NextTrainableToTrain() != null;
			if (flag)
			{
				pawn.training.SetWantedRecursive(TrainableDefOf.Release, true);
				TrainableDef td = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td, null, false);
			}
			bool flag2 = pawn.training.NextTrainableToTrain() != null;
			if (flag2)
			{
				TrainableDef td2 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td2, null, false);
			}
			bool flag3 = pawn.training.NextTrainableToTrain() != null;
			if (flag3)
			{
				TrainableDef td3 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td3, null, false);
			}
			bool flag4 = pawn.training.NextTrainableToTrain() != null;
			if (flag4)
			{
				TrainableDef td4 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td4, null, false);
			}
			bool flag5 = pawn.training.NextTrainableToTrain() != null;
			if (flag5)
			{
				TrainableDef td5 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td5, null, false);
			}
			bool flag6 = pawn.training.NextTrainableToTrain() != null;
			if (flag6)
			{
				TrainableDef td6 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td6, null, false);
			}
			bool flag7 = pawn.training.NextTrainableToTrain() != null;
			if (flag7)
			{
				TrainableDef td7 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td7, null, false);
			}
			bool flag8 = pawn.training.NextTrainableToTrain() != null;
			if (flag8)
			{
				TrainableDef td8 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td8, null, false);
			}
			bool flag9 = pawn.training.NextTrainableToTrain() != null;
			if (flag9)
			{
				TrainableDef td9 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td9, null, false);
			}
			bool flag10 = pawn.training.NextTrainableToTrain() != null;
			if (flag10)
			{
				TrainableDef td10 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td10, null, false);
			}
			bool flag11 = pawn.training.NextTrainableToTrain() != null;
			if (flag11)
			{
				TrainableDef td11 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td11, null, false);
			}
			bool flag12 = pawn.training.NextTrainableToTrain() != null;
			if (flag12)
			{
				TrainableDef td12 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td12, null, false);
			}
			bool flag13 = pawn.training.NextTrainableToTrain() != null;
			if (flag13)
			{
				TrainableDef td13 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td13, null, false);
			}
			bool flag14 = pawn.training.NextTrainableToTrain() != null;
			if (flag14)
			{
				TrainableDef td14 = pawn.training.NextTrainableToTrain();
				pawn.training.Train(td14, null, false);
			}
			return null;
		}
	}
}
