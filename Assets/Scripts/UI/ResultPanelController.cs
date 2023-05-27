using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultPanelController : MonoBehaviour, IInterfacePanel
{
	[Header("Base")]
	[SerializeField] private UIPanelType uiPanelType = UIPanelType.LevelResult;
	[SerializeField] private Transform panelContainer = default;
	[SerializeField] private GameStorage gameStorageSO = default;
	[SerializeField] private PlayerStorage playerStorageSO = default;
	[SerializeField] private DependencyContainer dependencyContainerSO = default;
	[SerializeField] private StoreStorage storeStorageSO = default;

	[Header("Components")]
	[SerializeField] private TMP_Text resultText = default;
	[SerializeField] private TMP_Text coinsText = default;
	[SerializeField] private TMP_Text levelText = default;
	[SerializeField] private Image imageResult = default;
	[SerializeField] private GameObject winParticles = default;
	[SerializeField] private Image progressBackgoround = default;
	[SerializeField] private Image progressImage = default;
	[SerializeField] private TMP_Text progressText = default;

	[Header("New Item container")]
	[SerializeField] private RectTransform containerNewItem = default;
	[SerializeField] private Image imageNewItem = default;
	[SerializeField] private Button buttonSelectNewItem = default;
	[SerializeField] private Button buttonCloseNewItem = default;
	 
	[Header("Buttons")]
	[SerializeField] private Button backButton = default;

	[Header("Strings")]
	[SerializeField] private string prefixCoins = "x";
	[SerializeField] private Sprite spriteWin = default;
	[SerializeField] private Sprite spriteLose = default;

	private int preProgress = 0;
	private BoardType rewardedBoard = default;

	private void Awake() {
		Init();
	}

	private void OnEnable() {
		GameManager.LevelFinishAction += ShowResultPanel;
	}

	private void OnDisable() {
		GameManager.LevelFinishAction -= ShowResultPanel;
	}

	private void PrepareButtons() {
		backButton.onClick.RemoveAllListeners();
		backButton.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
			EnviromentController.PrepareLevelAction?.Invoke();
			SkyboardController.ChangedBoardAction?.Invoke();
		});
	}

	private void ShowResultPanel(LevelResult _levelResult) {

		UIController.ShowUIPanelAloneAction(UIPanelType.None);
		StartCoroutine(WaitToShowResult());

		resultText.text = _levelResult.ToString();
		coinsText.text = prefixCoins + dependencyContainerSO.RewardedCoins;

		if (_levelResult == LevelResult.Win)
        {
			levelText.text = (playerStorageSO.ConcretePlayer.PlayerCurrentMission).ToString();
			imageResult.sprite = spriteWin;
			winParticles.SetActive(true);

			playerStorageSO.ConcretePlayer.BoardProgress = (100 / gameStorageSO.GetLevelCountFromWorld(playerStorageSO.ConcretePlayer.PlayerWorld)) * (playerStorageSO.ConcretePlayer.PlayerCurrentMission);
		}
		else
        {
			levelText.text = (playerStorageSO.ConcretePlayer.PlayerCurrentMission + 1).ToString();
			imageResult.sprite = spriteLose;
			winParticles.SetActive(false);

			playerStorageSO.ConcretePlayer.BoardProgress = (100 / gameStorageSO.GetLevelCountFromWorld(playerStorageSO.ConcretePlayer.PlayerWorld)) * (playerStorageSO.ConcretePlayer.PlayerCurrentMission);
		}

		playerStorageSO.ConcretePlayer.BoardProgress = playerStorageSO.ConcretePlayer.BoardProgress > 90 ? 100 : playerStorageSO.ConcretePlayer.BoardProgress;
	}

	private IEnumerator WaitToShowResult()
    {
		yield return new WaitForSecondsRealtime(1.5f);
		UIController.ShowUIPanelAloneAction(UIPanelType.LevelResult);

		var _progressSprite = storeStorageSO.Boards.Find((_board) => _board.BoardType == playerStorageSO.ConcretePlayer.BoardInProgress).Sprite;
		progressBackgoround.sprite = _progressSprite;
		progressImage.sprite = _progressSprite;

		progressImage.fillAmount = preProgress / 100f;
		progressImage.DOFillAmount(playerStorageSO.ConcretePlayer.BoardProgress / 100f, .5f).OnComplete(() => {
			CheckNewItem();
		});

		if (playerStorageSO.ConcretePlayer.BoardProgress > 100)
        {
			playerStorageSO.ConcretePlayer.BoardProgress = 100;
		}
		progressText.text = preProgress + "%";
		DOTween.To((_val) => progressText.text = (int)_val + "%", preProgress, playerStorageSO.ConcretePlayer.BoardProgress, 1f);

		
	}

	private void CheckNewItem()
    {
		if (playerStorageSO.ConcretePlayer.BoardProgress >= 100)
		{
			playerStorageSO.ConcretePlayer.BoardProgress = 100;
			bool boardContains = (playerStorageSO.ConcretePlayer.AvailableBoards.Find((_board) => _board.Type == playerStorageSO.ConcretePlayer.BoardInProgress) != null);
			if (boardContains == false)
			{
				playerStorageSO.ConcretePlayer.AvailableBoards.Add(new PlayerBoard(playerStorageSO.ConcretePlayer.BoardInProgress, true));
				rewardedBoard = playerStorageSO.ConcretePlayer.BoardInProgress;

				if (playerStorageSO.ConcretePlayer.CurrencyBoard != playerStorageSO.ConcretePlayer.BoardInProgress)
				{
					SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.RewardPopup);
					containerNewItem.gameObject.SetActive(true);
					imageNewItem.sprite = storeStorageSO.Boards.Find((_board) => _board.BoardType == playerStorageSO.ConcretePlayer.BoardInProgress).Sprite;
					buttonCloseNewItem.onClick.RemoveAllListeners();
					buttonSelectNewItem.onClick.RemoveAllListeners();
					buttonCloseNewItem.onClick.AddListener(() =>
					{
						SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
						containerNewItem.gameObject.SetActive(false);
					});
					buttonSelectNewItem.onClick.AddListener(() =>
					{
						SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
						containerNewItem.gameObject.SetActive(false);
						playerStorageSO.ConcretePlayer.CurrencyBoard = rewardedBoard;
						playerStorageSO.ConcretePlayer.AvailableBoards.Find((_board) => _board.Type == rewardedBoard).IsNew = false;
					});

					foreach (var _board in storeStorageSO.Boards)
					{
						boardContains = (playerStorageSO.ConcretePlayer.AvailableBoards.Find((_pBoard) => _pBoard.Type == _board.BoardType) != null);
						if (boardContains == false)
						{
							playerStorageSO.ConcretePlayer.BoardInProgress = _board.BoardType;
							playerStorageSO.ConcretePlayer.BoardProgress = 0;
							break;
						}
					}
				}
			}
		}
	}

	#region IInterfacePanel
	public UIPanelType UIPanelType { get => uiPanelType; }

	public void Hide() {
		if (panelContainer != null)
		{
			panelContainer.gameObject.SetActive(false);
		}
	}

	public void Show() {
		panelContainer.gameObject.SetActive(true);
		backButton.gameObject.SetActive(true);
		PrepareButtons();
	}

	public void Init() {
		UIController.InterfacePanels.Add(this);
	}
	#endregion
}
