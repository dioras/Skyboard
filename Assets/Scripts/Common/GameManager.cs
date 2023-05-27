using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Base")]
	[SerializeField] private PlayerStorage playerStorageSO = default;
	[SerializeField] private DependencyContainer dependencyContainerSO = default;

	[Header("Global Actions")]
	public static Action GameStartAction = default;
	public static Action PlayerLoadedAction = default;
	public static Action LevelStartAction = default;
	public static Action<LevelResult> LevelFinishAction = default;
	
	[Header("Variables")]
	private float startLevelTime = default;

	private void OnEnable()
	{
		LevelStartAction += StartLevel;
		LevelFinishAction += FinishLevel;
		PlayerLoadedAction += PlayMusic;
	}

	private void Start()
	{
		dependencyContainerSO.InGame = false;
		GameStartAction?.Invoke();
	}

	private void OnDisable()
	{
		LevelStartAction -= StartLevel;
		LevelFinishAction -= FinishLevel;
		PlayerLoadedAction -= PlayMusic;
		playerStorageSO.SavePlayer();
	}

	private void OnDestroy()
	{
		playerStorageSO.SavePlayer();
	}

	private void OnApplicationQuit()
	{
		playerStorageSO.SavePlayer();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			playerStorageSO.SavePlayer();
		}
	}

	private void PlayMusic()
	{
		SoundManager.PlaySomeSoundContinuous?.Invoke(SoundType.MainMelody, () => true);
	}

	private void StartLevel()
	{
		dependencyContainerSO.InGame = true;
		dependencyContainerSO.RewardedCoins = 0;
		//Metrica.StartLevelEvent(playerStorageSO.ConcretePlayer.PlayerCurrentMission);
		startLevelTime = Time.time;
	}

	private void FinishLevel(LevelResult _levelResult)
	{
		//Metrica.EndLevelEvent(playerStorageSO.ConcretePlayer.PlayerCurrentMission, _levelResult, (int)(Time.time - startLevelTime));
		VibrationController.Vibrate(30);
		if (_levelResult == LevelResult.Win)
        {
			playerStorageSO.ConcretePlayer.PlayerCurrentMission++;
        }
	}
}
