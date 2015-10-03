﻿namespace ExBuddy.OrderBotTags.Gather.Rotations
{
	using System.Threading.Tasks;

	using ExBuddy.Attributes;
	using ExBuddy.Interfaces;

	using ff14bot;
	using ff14bot.Managers;

	// Get One ++
	[GatheringRotation("Collect550", 600, 33)]
	public sealed class Collect550GatheringRotation : CollectableGatheringRotation, IGetOverridePriority
	{
		#region IGetOverridePriority Members

		int IGetOverridePriority.GetOverridePriority(ExGatherTag tag)
		{
			if (tag.IsUnspoiled())
			{
				// We need 5 swings to use this rotation
				if (GatheringManager.SwingsRemaining < 5)
				{
					return -1;
				}
			}

			if (tag.IsEphemeral())
			{
				// We need 4 swings to use this rotation
				if (GatheringManager.SwingsRemaining < 4)
				{
					return -1;
				}
			}

			// if we have a collectable && the collectable value is greater than or equal to 550: Priority 550
			if (tag.CollectableItem != null && tag.CollectableItem.Value >= 550)
			{
				return 550;
			}

			return -1;
		}

		#endregion

		public override async Task<bool> ExecuteRotation(ExGatherTag tag)
		{
			if (tag.IsUnspoiled())
			{
				await UtmostCaution(tag);
				await AppraiseAndRebuff(tag);
				await UtmostMethodical(tag);
				await AppraiseAndRebuff(tag);
				await Methodical(tag);
			}
			else
			{
				// level 56
				if (tag.GatherItem.Chance > 98 || Core.Player.CurrentGP < 600)
				{
					await Impulsive(tag);
					await Impulsive(tag);
					await Methodical(tag);

					return true;
				}

				var appraisalsRemaining = 4;
				await Impulsive(tag);
				appraisalsRemaining--;

				if (HasDiscerningEye)
				{
					await UtmostSingleMindMethodical(tag);
					appraisalsRemaining--;
				}

				await Impulsive(tag);
				appraisalsRemaining--;

				if (HasDiscerningEye)
				{
					await UtmostSingleMindMethodical(tag);
					appraisalsRemaining--;
				}

				if (appraisalsRemaining == 2)
				{
					await Methodical(tag);
				}

				if (appraisalsRemaining == 1)
				{
					await UtmostDiscerningMethodical(tag);
				}
			}

			await IncreaseChance(tag);
			return true;
		}
	}
}