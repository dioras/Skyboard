using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkyboardController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private StoreStorage storeStorageSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;

    [Header("Components")]
    [SerializeField] private SkyboardObject boardBodyObject = default;
    
    [Header("Actions")]
    public static Action ChangedBoardAction = default;
    public static Action HideBoardAtion = default;
    public static Action ShowBoardAction = default;

    private Sequence sequenceChanging = default;

    private void OnEnable()
    {
        ChangedBoardAction += ChangedBoard;
        HideBoardAtion += HideBoard;
        ShowBoardAction += ShowBoard;
    }

    private void OnDisable()
    {
        ChangedBoardAction -= ChangedBoard;
        HideBoardAtion -= HideBoard;
        ShowBoardAction -= ShowBoard;
    }

    private void ChangedBoard()
    {
        if (boardBodyObject != null)
        {
            sequenceChanging = DOTween.Sequence();
            boardBodyObject.BoardRenderrers.ForEach((_renderrer) => sequenceChanging.Join(_renderrer.material.DOFloat(1f, "_DissolveAmount", .5f)));
            sequenceChanging.OnComplete(() =>
            {
                Destroy(boardBodyObject.gameObject);
                ShowBoard();
            });
        }
        else
        {
            ShowBoard();
        }
    }

    private void HideBoard()
    {
        sequenceChanging = DOTween.Sequence();
        boardBodyObject.BoardRenderrers.ForEach((_renderrer) => sequenceChanging.Join(_renderrer.material.DOFloat(1f, "_DissolveAmount", .5f)));
        boardBodyObject.Trails.ForEach((_trail) => _trail.enabled = false);
    }

    private void ShowBoard()
    {
        var _prefab = storeStorageSO.Boards.Find((board) => board.BoardType == playerStorageSO.ConcretePlayer.CurrencyBoard).PrefabBoard;
        boardBodyObject = Instantiate(_prefab, Vector3.zero, Quaternion.identity, transform).GetComponent<SkyboardObject>();
        boardBodyObject.transform.localPosition = Vector3.zero;
        boardBodyObject.transform.eulerAngles = Vector3.zero;

        boardBodyObject.BoardRenderrers.ForEach((_renderrer) => _renderrer.material.SetFloat("_DissolveAmount", 1f));
        sequenceChanging = DOTween.Sequence();
        boardBodyObject.BoardRenderrers.ForEach((_renderrer) => sequenceChanging.Join(_renderrer.material.DOFloat(0f, "_DissolveAmount", .5f)));
        boardBodyObject.Trails.ForEach((_trail) => _trail.enabled = true);
    }
}
