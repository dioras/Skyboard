using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GameStorage", fileName = "GameStorage")]
public class GameStorage : ScriptableObject
{
	[SerializeField] private PlayerStorage playerStorageSO = default;

	[Header("GameBaseParameters")]
	[SerializeField] private GameBaseParameters gameBaseParameters = default;

	[Header("Levels")]
	[SerializeField] private List<Level> levelsFirstWorld = default;
	[SerializeField] private List<Level> levelsSecondWorld = default;
	[SerializeField] private List<Level> levelsThirdWorld = default;

	[Header("World")]
	[SerializeField] private List<WorldSettings> worldSettings = new List<WorldSettings>();

	#region Geters/Seters
	public GameBaseParameters GameBaseParameters { get => gameBaseParameters; }
	public List<Level> Levels { get => Levels; }
	#endregion

	public Level GetLevelFromMission(int _mission, World _world)
    {
		var levels = GetLevelsFromWorld(_world);

		if (_mission >= levels.Count)
        {
			_mission = 0;
			playerStorageSO.ConcretePlayer.PlayerCurrentMission = _mission;
			playerStorageSO.ConcretePlayer.PlayerWorld++;
			if (playerStorageSO.ConcretePlayer.PlayerWorld == World.None)
            {
				playerStorageSO.ConcretePlayer.PlayerWorld = World.First;
			}
			levels = GetLevelsFromWorld(playerStorageSO.ConcretePlayer.PlayerWorld);
		}

		return levels[_mission];
    }

	public int GetLevelCountFromWorld(World _world)
    {
		return GetLevelsFromWorld(_world).Count;
	}

	public WorldSettings GetWorldSettings(World _world)
    {
		return worldSettings.Find((_settings) => _settings.World == _world);
	}

	private List<Level> GetLevelsFromWorld(World _world)
    {
        switch (_world)
        {
			case World.First:
				return levelsFirstWorld;
			case World.Second:
				return levelsSecondWorld;
			case World.Third:
				return levelsThirdWorld;
			default:
				return null;
		}
    }
}

[Serializable]
public class GameBaseParameters
{
	[Header("UI Block")]
	[SerializeField] private float timeToInteractUI = 0.3f;
	[SerializeField] private float timeToShowHiddenButton = 3f;

	[Header("Rate US Block")]
	[SerializeField] private int reRateUsDelta = 259200;
	[SerializeField] private int playerLevelToRateUs = 1;

	[Header("ADS Block")]
	[SerializeField] private int playerLevelToInterstitial = 3;

	[Header("Coins")]
	[SerializeField] private int coinReward = 55;

	[Header("Formating")]
	[SerializeField] private string formatingMoneyString = "$#,0";


	#region Geters/Seters
	public float TimeToShowHiddenButton { get => timeToShowHiddenButton; }
	public float TimeToInteractUI { get => timeToInteractUI; }
	public int ReRateUsDelta { get => reRateUsDelta; }
	public int PlayerLevelToRateUs { get => playerLevelToRateUs; }
	public int PlayerLevelToInterstitial { get => playerLevelToInterstitial; }
	public string FormatingMoneyString { get => formatingMoneyString; }
	public int CoinReward { get => coinReward; }
	#endregion
}

[Serializable]
public class Level
{
	[SerializeField] private LevelController controller = default;

	public LevelController LevelController { get => controller; }
}

[Serializable] 
public class WorldSettings
{
	[SerializeField] private World world = default;
	[SerializeField] private Sprite sprite = default;

	#region getter/setter
	public World World => world;
	public Sprite Sprite => sprite;
    #endregion
}
