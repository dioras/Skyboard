using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NeonController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float durationEnable = .3f;
    [SerializeField] private float durationDisable = .3f;
    [SerializeField] private Color colorDisable = Color.white;
    [SerializeField] private Color colorEnable = Color.white;

    [Header("Components")]
    [SerializeField] private List<MeshRenderer> neons = new List<MeshRenderer>();

    [Header("Params")]
    [SerializeField] private string colorString = "_Color";

    private Sequence sequence = default;
    private MeshRenderer preMesh = default;

    private void Awake()
    {
        StartAnimation();
    }

    private void StartAnimation()
    {
        sequence = DOTween.Sequence();
        preMesh = null;
        neons.ForEach((_neon) =>
        {
            _neon.material.SetColor(colorString, colorDisable);
            sequence.Append(_neon.material.DOColor(colorEnable, durationEnable));
            if (preMesh != null)
            {
                sequence.Join(preMesh.material.DOColor(colorDisable, durationDisable));
            }
            preMesh = _neon;
        });
        sequence.SetLoops(-1);
        sequence.Play();
    }
}
