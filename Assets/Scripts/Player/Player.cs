using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Player
{
	[Header("Player's Property")]
	[SerializeField] private int playerCoins = 0;
	[SerializeField] private BoardType currecyBoard = default;
	[SerializeField] private List<PlayerBoard> availableBoards = new List<PlayerBoard>();
	[SerializeField] private BoardType boardInProgress = default;
	[SerializeField] private int boardProgress = default;

	[Header("Player Achivemant")]
	[SerializeField] private World playerWorld = World.First;
	[SerializeField] private int playerCurrentMission = 1;

	[Header("Settings")]
	[SerializeField] private bool isMusicOn = true;
	[SerializeField] private bool isSoundOn = true;
	[SerializeField] private bool isVibrationcOn = true;

	[Header("RateUS")]
	[SerializeField] private bool isRateUs = false;
	[SerializeField] private JsonDateTime dateLastRateTry = new JsonDateTime();

	[Header("Player Last Session")]
	[SerializeField] private JsonDateTime timeLastSession = new JsonDateTime();

	[Header("No ADS")]
	[SerializeField] private bool isBuyADSOffer = false;

	[Header("Actions")]
	public static Action PlayerBuyBoxAction = default;
	public static Action PlayerBuySellCarrAction = default;
	public static Action PlayerChangeCoinAction = default;

	#region Geters/Seters
	public int PlayerCoins { get => playerCoins; }
	public JsonDateTime DateLastRateTry { get => dateLastRateTry; set => dateLastRateTry = value; }
	public bool IsRateUs { get => isRateUs; set => isRateUs = value; }
	public JsonDateTime TimeLastSession { get => timeLastSession; set => timeLastSession = value; }
	public bool IsBuyADSOffer { get => isBuyADSOffer; }
	public bool IsMusicOn { get => isMusicOn; }
	public bool IsSoundOn { get => isSoundOn; }
	public bool IsVibrationcOn { get => isVibrationcOn; }
	public int PlayerCurrentMission { get => playerCurrentMission; set => playerCurrentMission = value; }
	public World PlayerWorld { get => playerWorld; set => playerWorld = value; }
	public List<PlayerBoard> AvailableBoards { get => availableBoards; set => availableBoards = value; }
	public BoardType BoardInProgress { get => boardInProgress; set => boardInProgress = value; }
	public BoardType CurrencyBoard { get => currecyBoard; set => currecyBoard = value; }
	public int BoardProgress { get => boardProgress; set => boardProgress = value; }
	#endregion

	public Player()
    {
		playerCoins = 0;
		playerCurrentMission = 0;
		playerWorld = World.First;

		availableBoards = new List<PlayerBoard>();
		availableBoards.Add(new PlayerBoard(BoardType.First, false));
		availableBoards.Add(new PlayerBoard(BoardType.Second, true));

		boardInProgress = BoardType.Third;
		currecyBoard = BoardType.First;
	}

	public void ChangeCoins(int _coin) {
		playerCoins += _coin;
		PlayerChangeCoinAction?.Invoke();
	}

	public void SetSoundStatus(bool _status)
	{
		isSoundOn = _status;
	}

	public void SetMusicStatus(bool _status)
	{
		isMusicOn = _status;
	}

	public void SetVibrationtatus(bool _status)
	{
		isVibrationcOn = _status;
	}
}

[Serializable] 
public class PlayerBoard
{
	[SerializeField] private BoardType boardType = default;
	[SerializeField] private bool isNew = false;

	public BoardType Type { get => boardType; set => boardType = value; }
	public bool IsNew { get => isNew; set => isNew = value; }

	public PlayerBoard(BoardType _boardType, bool _isNew)
    {
		boardType = _boardType;
		isNew = _isNew;
    }
}
