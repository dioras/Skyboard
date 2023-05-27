using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class GamePanelController : MonoBehaviour, IInterfacePanel
{
    [Header("Base")]
    [SerializeField] private UIPanelType uiPanelType = UIPanelType.Game;
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;

    [Header("Settings")]
    [SerializeField] private Sprite[] reactions = default;

    [Header("Components")]
    [SerializeField] private Transform panelContainer = default;
    [SerializeField] private Slider sliderProgressBar = default;
    [SerializeField] private TMP_Text levelText = default;
    [SerializeField] private TMP_Text progressText = default;
    [SerializeField] private RectTransform rectReaction = default;
    [SerializeField] private Image reactionImage = default;

    [Header("Tutorial")]
    [SerializeField] private RectTransform tutorialContaier = default;
    [SerializeField] private RectTransform handRectTransform = default;
    [SerializeField] private float handMoveDuration = 1.5f;
    [SerializeField] private Vector2 handMoveFrom = default;
    [SerializeField] private Vector2 handMoveTo = default;

    [Header("Strings")]
    [SerializeField] private string formatProgressText = "0<color=orange>m</color>";
    [SerializeField] private string prefixLevel = "Level: ";

    [Header("Actions")]
    public static Action<Transform, Transform> SetPointsLevelsAction = default;
    public static Action ShowReactionAction = default;
    public static Action<bool> ShowTutorialAction = default;

    private Transform startPoint = default;
    private Transform finishPoint = default;
    private Sequence sequenceReaction = default;
    private Sequence sequenceTutorial = default;
    private Coroutine coroutineHideTutorial = default;

    private void OnEnable()
    {
        SetPointsLevelsAction += SetPointsLevels;
        ShowReactionAction += ShowReaction;
        ShowTutorialAction += ShowTutorial;

        GameManager.LevelStartAction += OnGameStarted;
    }

    private void OnDisable()
    {
        SetPointsLevelsAction -= SetPointsLevels;
        ShowReactionAction -= ShowReaction;
        ShowTutorialAction -= ShowTutorial;

        GameManager.LevelStartAction -= OnGameStarted;
    }

    private void Awake()
    {
        Init();
        PrepareButtons();
        HideReaction();
        ShowTutorial(false);
    }

    private void Update()
    {
        if (startPoint != null && finishPoint != null) {
            sliderProgressBar.value = sliderProgressBar.maxValue -  Vector3.Distance(dependencyContainerSO.PlayerTransform.position, finishPoint.position);
            progressText.text = sliderProgressBar.value.ToString(formatProgressText);
        }
    }

    private void PrepareButtons()
    {
        
    }

    private void SetPointsLevels(Transform _startPoint, Transform _finishPoint)
    {
        startPoint = _startPoint;
        finishPoint = _finishPoint;
        sliderProgressBar.minValue = 0f;
        sliderProgressBar.maxValue = Vector3.Distance(_startPoint.position, finishPoint.position);
    }

    private void OnGameStarted()
    {
        UIController.ShowUIPanelAloneAction(UIPanelType.None);
    }

    private void ShowReaction()
    {
        sequenceReaction.Complete();

        rectReaction.gameObject.SetActive(true);
        rectReaction.localScale = Vector3.zero;
        reactionImage.sprite = reactions[UnityEngine.Random.Range(0, reactions.Length)];
        //textReaction.text = reactions[UnityEngine.Random.Range(0, reactions.Length)];
        //textReaction.color = reactionColors[UnityEngine.Random.Range(0, reactionColors.Length)];
        sequenceReaction = DOTween.Sequence();
        sequenceReaction.Append(rectReaction.transform.DOScale(1.3f, .4f));
        sequenceReaction.Append(rectReaction.transform.DOScale(1f, .2f));
        sequenceReaction.Join(rectReaction.transform.DOShakeRotation(.3f, new Vector3(0f, 5f, 90f), 10, 50f));
        sequenceReaction.OnComplete(() => {
            rectReaction.transform.DOScale(0f, .2f);
        });
    }

    private void HideReaction()
    {
        rectReaction.gameObject.SetActive(false);
    }

    private void ShowTutorial(bool _show)
    {
        if (coroutineHideTutorial != null)
        {
            StopCoroutine(coroutineHideTutorial);
        }
        dependencyContainerSO.IsTutorialShow = _show;
        if (_show)
        {
            sequenceTutorial.Kill();
            handRectTransform.DOAnchorPos(handMoveFrom, 0f);

            sequenceTutorial = DOTween.Sequence();
            sequenceTutorial.Append(handRectTransform.DOAnchorPos(handMoveTo, handMoveDuration / 2f));
            sequenceTutorial.Append(handRectTransform.DOAnchorPos(handMoveFrom, handMoveDuration / 2f));
            sequenceTutorial.SetLoops(-1);
            sequenceTutorial.Play();
            //coroutineHideTutorial = StartCoroutine(HideTutorial());
            tutorialContaier.gameObject.SetActive(true);
            tutorialContaier.DOAnchorPosY(0f, .6f);
        }
        else
        {
            tutorialContaier.DOAnchorPosY(-700f, .6f).OnComplete(() => tutorialContaier.gameObject.SetActive(false));
            sequenceTutorial.Kill();
        }
    }

    //private void HideTutorial()
    //{
    //    //yield return new WaitForSecondsRealtime(5f);
    //    ShowTutorial(false);
    //}

    #region IInterfacePanel
    public UIPanelType UIPanelType { get => uiPanelType; }

    public void Hide()
    {
        panelContainer.gameObject.SetActive(false);
    }

    public void Show()
    {
        panelContainer.gameObject.SetActive(true);
        levelText.text = prefixLevel + (playerStorageSO.ConcretePlayer.PlayerCurrentMission + 1);
    }

    public void Init()
    {
        UIController.InterfacePanels.Add(this);
    }
    #endregion
}
