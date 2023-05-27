using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class MainPanelController : MonoBehaviour, IInterfacePanel
{
	[Header("Base")]
	[SerializeField] private PlayerStorage playerStorageSO = default;
	[SerializeField] private GameStorage gameStorageSO = default;

	[Header("Settings")]
	[SerializeField] private UIPanelType uiPanelType = UIPanelType.Main;
	[SerializeField] private Transform panelContainer = default;

	[Header("Component")]
	[SerializeField] private EventTrigger eventTriggerPlay = default;
	[SerializeField] private Button buttonStore = default;
	[SerializeField] private Image imageNewStore = default;

	[Header("Level Bar")]
	[SerializeField] private RectTransform prizeImage = default;
	[SerializeField] private List<LayoutElement> levelBarMarks = new List<LayoutElement>();
	[SerializeField] private Color compliteLevelMark = Color.white;
	[SerializeField] private Color currencyLevelMark = Color.white;
	[SerializeField] private Color defaultLevelMark = Color.white;
	[SerializeField] private float defaultHeight = 96f;
	[SerializeField] private float currencyLevelHeight = 112f;
	[SerializeField] private Image imageCurrencyWorld = default;
	[SerializeField] private Image imageNextWorld = default;

	private Sequence sequenceLevelBar = default;
	
	private void Awake()
	{
		Init();
		PrepareButtons();

		prizeImage.anchoredPosition = new Vector2(0f, 50.5f);
		prizeImage.DOAnchorPosY(80f, .5f).SetLoops(-1, LoopType.Yoyo);

		var newItemTransform = imageNewStore.GetComponent<RectTransform>();
		newItemTransform.DOScale(new Vector3(.8f, .8f, 1f), .5f).SetLoops(-1, LoopType.Yoyo);
	}

	private void PrepareButtons()
    {
		eventTriggerPlay.triggers.Clear();
		EventTrigger.Entry _tap = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
		_tap.callback.AddListener((data) => {
			GameManager.LevelStartAction?.Invoke();
		});
		eventTriggerPlay.triggers.Add(_tap);

		buttonStore.onClick.AddListener(() => {
			SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
			UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Store);
		});
	}

	private void ShowCurrencyProgress()
    {
		sequenceLevelBar = DOTween.Sequence();

		for (int i = 0; i < levelBarMarks.Count; i++)
        {
			levelBarMarks[i].gameObject.SetActive(false);
		}
		for (int i = 0; i < levelBarMarks.Count; i++)
		{
			levelBarMarks[i].gameObject.SetActive(false);
		}


		imageCurrencyWorld.sprite = gameStorageSO.GetWorldSettings(playerStorageSO.ConcretePlayer.PlayerWorld).Sprite;
		var nextWorld = playerStorageSO.ConcretePlayer.PlayerWorld + 1;
		imageNextWorld.sprite = gameStorageSO.GetWorldSettings(nextWorld == World.None ? World.First : nextWorld).Sprite;

		levelBarMarks.ForEach((_mark) => SetMarkLevelBar(_mark, defaultLevelMark, defaultHeight));
		bool _compliteLevels = true;
		for (int i = 0; i < gameStorageSO.GetLevelCountFromWorld(playerStorageSO.ConcretePlayer.PlayerWorld); i++)
        {
			levelBarMarks[i].gameObject.SetActive(true);
			prizeImage.transform.SetParent(levelBarMarks[i].transform);
			prizeImage.anchoredPosition = new Vector2(0f, prizeImage.anchoredPosition.y);
			if (playerStorageSO.ConcretePlayer.PlayerCurrentMission == i)
            {
				SetMarkLevelBar(levelBarMarks[i], currencyLevelMark, currencyLevelHeight);
				_compliteLevels = false;
			}
			else
            {
				SetMarkLevelBar(levelBarMarks[i], _compliteLevels ? compliteLevelMark : defaultLevelMark, defaultHeight);
			}
        }

		sequenceLevelBar.Play();
	}

	private void SetMarkLevelBar(LayoutElement mark, Color color, float height)
    {
		var markImage = mark.GetComponent<Image>();
		markImage.color = defaultLevelMark;
		if (color != defaultLevelMark)
		{
			sequenceLevelBar.Append(mark.DOPreferredSize(new Vector2(mark.preferredWidth, currencyLevelHeight), .3f / 2f).OnComplete(() =>
			{
				markImage.DOColor(color, .2f / 2f);
				mark.DOPreferredSize(new Vector2(mark.preferredWidth, height), .15f / 2f);
			}));
		}
		else
        {
			mark.preferredHeight = height;
			markImage.DOColor(color, .2f / 2f);
		}
	}

	#region IInterfacePanel
	public UIPanelType UIPanelType { get => uiPanelType; }

	public void Hide()
	{
		panelContainer.gameObject.SetActive(false);
	}

	public void Show()
	{
		panelContainer.gameObject.SetActive(true);
		ShowCurrencyProgress();
		var availableNewBoard = (playerStorageSO.ConcretePlayer.AvailableBoards.Find((_board) => _board.IsNew == true) != null);
		imageNewStore.enabled = availableNewBoard;
	}

	public void Init()
	{
		UIController.InterfacePanels.Add(this);
	}
	#endregion
}
