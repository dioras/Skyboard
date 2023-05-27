using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StoreStorage", fileName = "StoreStorageSO")]
public class StoreStorage : ScriptableObject
{
    [SerializeField] private List<Board> boards = new List<Board>();

    public List<Board> Boards { get => boards; set => boards = value; }
}

[Serializable]
public class Board
{
    [SerializeField] private BoardType boardType = default;
    [SerializeField] private Sprite sprite = default;
    [SerializeField] private GameObject prefabBoard = default; 

    public BoardType BoardType { get => boardType; }
    public Sprite Sprite { get => sprite; }
    public GameObject PrefabBoard { get => prefabBoard; }
}
