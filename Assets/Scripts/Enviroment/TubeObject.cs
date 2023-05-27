using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool fullColored = false;
    [SerializeField] private int materialIndex = 0;
    [SerializeField] private Color defaultColor = default;
    [SerializeField] private Color[] colors = default;

    [Header("Components")]
    [SerializeField] private MeshRenderer meshRenderer = default;

    private void Awake()
    {
        SetFullColor(defaultColor);
        var _randomColor = colors[Random.Range(0, colors.Length)];
        if (fullColored)
        {
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                meshRenderer.materials[i].SetColor("_Color", _randomColor);
            }
        }
        else
        {
            meshRenderer.materials[materialIndex].SetColor("_Color", _randomColor);
        }
    }

    private void SetFullColor(Color color)
    {
        for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            meshRenderer.materials[i].SetColor("_Color", color);
        }
    }
}
