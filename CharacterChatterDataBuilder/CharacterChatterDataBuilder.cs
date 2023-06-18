using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ShinyShoe;
using Trainworks.Managers;
using Trainworks.Utilities;
using Trainworks.Builders;
using UnityEngine;
using Void.Init;


namespace Void.Builders 
{
	/// <summary>
	/// Determines what the character can say in a variety of situations. All string vales are treated as localization keys.
	/// </summary>
	public class CharacterChatterDataBuilder
	{
		public string name = "Default chatter data name";

		/// <summary>
		/// The gender of the speaking character.
		/// </summary>
		public CharacterChatterData.Gender gender = CharacterChatterData.Gender.Neutral;

		/// <summary>
		/// What the character can say when played.
		/// </summary>
		public List<string> characterAddedExpressionKeys = new List<string>() { };

		/// <summary>
		/// What the character can say when attacking.
		/// </summary>
		public List<string> characterAttackingExpressionKeys = new List<string>() { };

		/// <summary>
		/// What the character can say upon scoring a kill.
		/// </summary>
		public List<string> characterSlayedExpressionKeys = new List<string>() { };

		/// <summary>
		/// What the character can say during the player's turn.<br></br>
		/// Note: These are the only chatter messages that can appear when the game speed is set to Super Ultra.
		/// </summary>
		public List<string> characterIdleExpressionKeys = new List<string>() { };

		/// <summary>
		/// What the character can say when preforming specific, triggered actions. Each trigger can have its own list of sayings.<br></br>
		/// Note: Chatter triggers are independent of any triggered abilities the characters have.<br></br>
		/// Note: Triggers that overlap can fire off multiple chatter messages at the same time, leading to chatter clutter.<br></br>
		/// Thus, I advise against configuring both OnAttacking and, say, characterAttacking expression keys at the same time.
		/// </summary>
		public List<CharacterTriggerDataChatterExpressionKeys> characterTriggerExpressionKeys = new List<CharacterTriggerDataChatterExpressionKeys>() { };

		[Serializable]
		public struct CharacterTriggerDataChatterExpressionKeys
        {
			/// <summary>
			/// The character trigger that will activate the chatter message.
			/// </summary>
			public CharacterTriggerData.Trigger Trigger { get; set; }
			/// <summary>
			/// The localization key.
			/// </summary>
			public string Key { get; set; }
        }

        public CharacterChatterData Build() 
		{
			//Ponies.Log("Building Chatter data");

			CharacterChatterData chatter = ScriptableObject.CreateInstance<CharacterChatterData>();
			//CharacterChatterData baseData = ScriptableObject.CreateInstance<CharacterChatterData>();
			Traverse.Create(chatter).Field("baseData").SetValue(chatter);
			Traverse.Create(chatter).Field("gender").SetValue(gender);

			chatter.name = name;

			//Ponies.Log("Line 57");

			if (characterAddedExpressionKeys.Count > 0) 
			{
				List<CharacterChatterData.ChatterExpressionData> expressions = new List<CharacterChatterData.ChatterExpressionData>();

				foreach (string key in characterAddedExpressionKeys) 
				{
					if (!key.IsNullOrEmpty())
					{
						CharacterChatterData.ChatterExpressionData chatter1 = new CharacterChatterData.ChatterExpressionData()
						{
							locKey = key,
						};
						//Ponies.Log("Line 71");

						expressions.Add(chatter1);
					}
				}

				//Ponies.Log("Expressions: " + expressions.Count);

				Traverse.Create(chatter).Field("characterAddedExpressions").SetValue(expressions);
			}

			//Ponies.Log("Line 82");

			if (characterAttackingExpressionKeys.Count > 0)
			{
				List<CharacterChatterData.ChatterExpressionData> expressions = new List<CharacterChatterData.ChatterExpressionData>();

				foreach (string key in characterAttackingExpressionKeys)
				{
					if (!key.IsNullOrEmpty())
					{
						CharacterChatterData.ChatterExpressionData chatter1 = new CharacterChatterData.ChatterExpressionData()
						{
							locKey = key,
						};

						expressions.Add(chatter1);
					}
				}

				Traverse.Create(chatter).Field("characterAttackingExpressions").SetValue(expressions);
			}

			if (characterSlayedExpressionKeys.Count > 0)
			{
				List<CharacterChatterData.ChatterExpressionData> expressions = new List<CharacterChatterData.ChatterExpressionData>();

				foreach (string key in characterSlayedExpressionKeys)
				{
					if (!key.IsNullOrEmpty())
					{
						CharacterChatterData.ChatterExpressionData chatter1 = new CharacterChatterData.ChatterExpressionData()
						{
							locKey = key,
						};

						expressions.Add(chatter1);
					}
				}

				Traverse.Create(chatter).Field("characterSlayedExpressions").SetValue(expressions);
			}

			if (characterIdleExpressionKeys.Count > 0)
			{
				List<CharacterChatterData.ChatterExpressionData> expressions = new List<CharacterChatterData.ChatterExpressionData>();

				foreach (string key in characterIdleExpressionKeys)
				{
					if (!key.IsNullOrEmpty())
					{
						CharacterChatterData.ChatterExpressionData chatter1 = new CharacterChatterData.ChatterExpressionData()
						{
							locKey = key,
						};

						expressions.Add(chatter1);
					}
				}

				Traverse.Create(chatter).Field("characterIdleExpressions").SetValue(expressions);
			}

			if (characterTriggerExpressionKeys.Count > 0)
			{
				List<CharacterChatterData.TriggerChatterExpressionData> expressions = new List<CharacterChatterData.TriggerChatterExpressionData>();

				foreach (CharacterTriggerDataChatterExpressionKeys pair in characterTriggerExpressionKeys)
				{
					if (!pair.Key.IsNullOrEmpty())
					{
						CharacterChatterData.TriggerChatterExpressionData chatter1 = new CharacterChatterData.TriggerChatterExpressionData()
						{
							locKey = pair.Key,
							trigger = pair.Trigger,
						};

						expressions.Add(chatter1);
					}
				}

				Traverse.Create(chatter).Field("characterTriggerExpressions").SetValue(expressions);
			}

			//Traverse.Create(chatter).Field("baseData").SetValue(baseData);

			return chatter;
		}
	}
}