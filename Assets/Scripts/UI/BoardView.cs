using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BoardView : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private PlayerStorage playerStorageSO = default;

    [Header("Settings")]
    [SerializeField] private bool isClear = false;
    [SerializeField] private bool isSelected = false;

    [Header("Components")]
    [SerializeField] private Button buttonTrigger = default;
    [SerializeField] private Image imageNew = default;

    [Header("Containers")]
    [SerializeField] private RectTransform containerClosed = default;
    [SerializeField] private RectTransform containerInProgress = default;
    [SerializeField] private RectTransform containerOpened = default;

    [Header("'In Progress' components")]
    [SerializeField] private Image inProgressImage = default;
    [SerializeField] private Slider inProgressSlider = default;
    [SerializeField] private TMP_Text inProgressText = default;

    [Header("'Opened' components")]
    [SerializeField] private Image openedImage = default;
    [SerializeField] private Image openedSelectedImage = default;

    private BoardViewType currencyViewType = default;
    private Board currencyBoard = default;

    #region get/set
    public bool IsClear { get => isClear; set => isClear = value; }
    #endregion

    private void Awake()
    {
        var newItemTransform = imageNew.GetComponent<RectTransform>();
        newItemTransform.DOScale(new Vector3(.7f, .7f, 1f), .5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void Config(BoardViewType boardViewType, Board board)
    {
        currencyViewType = boardViewType;
        currencyBoard = board;

        InitView();
        PrepareButtons();
    }

    private void PrepareButtons()
    {
        buttonTrigger.onClick.RemoveAllListeners();
        if (currencyViewType == BoardViewType.Opened && isSelected == false)
        {
            buttonTrigger.onClick.AddListener(() =>
            {
                playerStorageSO.ConcretePlayer.CurrencyBoard = currencyBoard.BoardType;
                playerStorageSO.ConcretePlayer.AvailableBoards.Find((_board) => _board.Type == currencyBoard.BoardType).IsNew = false;
                StorePanelController.RefreshBoardListAction?.Invoke(false);
                SkyboardController.ChangedBoardAction?.Invoke();
            });
        }
    }

    private void InitView()
    {
        containerOpened.gameObject.SetActive(false);
        containerInProgress.gameObject.SetActive(false);
        containerClosed.gameObject.SetActive(false);
        openedSelectedImage.enabled = false;
        imageNew.enabled = false;
        switch (currencyViewType)
        {
            case BoardViewType.Closed:
                containerClosed.gameObject.SetActive(true);
                break;
            case BoardViewType.InProgress:
                containerInProgress.gameObject.SetActive(true);
                inProgressSlider.value = playerStorageSO.ConcretePlayer.BoardProgress;
                inProgressImage.sprite = currencyBoard.Sprite;
                inProgressText.text = playerStorageSO.ConcretePlayer.BoardProgress + "%";
                break;
            case BoardViewType.Opened:
                imageNew.enabled = playerStorageSO.ConcretePlayer.AvailableBoards.Find((_board) => _board.Type == currencyBoard.BoardType).IsNew;
                openedImage.sprite = currencyBoard.Sprite;
                openedSelectedImage.enabled = playerStorageSO.ConcretePlayer.CurrencyBoard == currencyBoard.BoardType;
                isSelected = playerStorageSO.ConcretePlayer.CurrencyBoard == currencyBoard.BoardType;
                containerOpened.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
