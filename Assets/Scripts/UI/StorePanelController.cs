using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class StorePanelController : MonoBehaviour, IInterfacePanel
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private StoreStorage storeStorageSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;

    [Header("Settings")]
    [SerializeField] private UIPanelType uiPanelType = UIPanelType.Store;
    [SerializeField] private float durationShowHide = .5f;

    [Header("Components")]
    [SerializeField] private RectTransform panelContainer = default;
    [SerializeField] private RectTransform boardContainer = default;
    [SerializeField] private Button buttonBack = default;
    [SerializeField] private RectTransform boardListContent = default;
    [SerializeField] private BoardView prefabBoardItem = default;
    [SerializeField] private ScrollRect scrollRectBoards = default;

    [Header("Actions")]
    public static Action<bool> RefreshBoardListAction = default;

    private RectTransform buttonBackRect = default;
    private Vector2 defaultBoardsPosition = default;
    private Vector2 defaultButtonBackPosition = default;

    private List<BoardView> boardViews = new List<BoardView>();

    private void OnEnable()
    {
        RefreshBoardListAction += LoadBoardList;
    }

    private void OnDisable()
    {
        RefreshBoardListAction -= LoadBoardList;
    }

    private void Awake()
    {
        Init();
        PrepareButtons();
        SetDefaultsParams();
    }

    private void SetDefaultsParams()
    {
        buttonBackRect = buttonBack.GetComponent<RectTransform>();
        defaultBoardsPosition = new Vector2(0f, 400f);
        defaultButtonBackPosition = buttonBackRect.anchoredPosition;
    }

    private void PrepareButtons()
    {
        buttonBack.onClick.AddListener(() => {
            SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.ButtonClick);
            UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.None);
            CameraController.SetTweenCameraPositionAction?.Invoke(dependencyContainerSO.MainMenuCameraPosition.position, dependencyContainerSO.MainMenuCameraPosition.eulerAngles, () => {
                UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Main);
            });
        });
    }

    private void LoadBoardList(bool _normalized = true)
    {
        boardViews.ForEach((_view) => _view.IsClear = true);
        storeStorageSO.Boards.ForEach((_board) => {
            var _view = boardViews.Find((view) => view.IsClear);
            if (_view == null)
            {
                _view = Instantiate(prefabBoardItem, boardListContent);
                boardViews.Add(_view);
            }

            _view.Config(GetBoardViewType(_board.BoardType), _board);
            _view.IsClear = false;
        });

        if (_normalized)
        {
            scrollRectBoards.horizontalNormalizedPosition = 0f;
        }
    }

    private BoardViewType GetBoardViewType(BoardType boardType)
    {
        if (playerStorageSO.ConcretePlayer.AvailableBoards.FindAll((_board) => _board.Type == boardType).Count != 0)
        {
            return BoardViewType.Opened;
        }

        if (playerStorageSO.ConcretePlayer.BoardInProgress == boardType)
        {
            return BoardViewType.InProgress;
        }

        return BoardViewType.Closed;
    }

    #region IInterfacePanel
    public UIPanelType UIPanelType { get => uiPanelType; }

    public void Hide()
    {
        buttonBackRect.DOAnchorPos(-defaultButtonBackPosition, durationShowHide);
        boardContainer.DOAnchorPos(-defaultBoardsPosition, durationShowHide).OnComplete(() => {
            panelContainer.gameObject.SetActive(false);
        });
    }

    public void Show()
    {
        buttonBackRect.DOKill();
        boardContainer.DOKill();

        LoadBoardList();
        panelContainer.gameObject.SetActive(true);
        boardContainer.anchoredPosition = -defaultBoardsPosition;
        buttonBackRect.anchoredPosition = -defaultButtonBackPosition;
        buttonBack.interactable = false;

        boardContainer.DOAnchorPos(Vector2.zero, durationShowHide);
        buttonBackRect.DOAnchorPos(defaultButtonBackPosition, durationShowHide).OnComplete(() => buttonBack.interactable = true);

        CameraController.SetTweenCameraPositionAction?.Invoke(dependencyContainerSO.StoreMenuCameraPosition.position, dependencyContainerSO.StoreMenuCameraPosition.eulerAngles, null);
    }

    public void Init()
    {
        UIController.InterfacePanels.Add(this);
    }
    #endregion
}
