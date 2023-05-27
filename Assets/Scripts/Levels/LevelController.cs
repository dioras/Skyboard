using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;

    [Header("Components")]
    [SerializeField] private Transform playerSpawnPoint = default;
    [SerializeField] private Transform finishTrigger = default;
    [SerializeField] private Transform finishCameraPosition = default;
    [SerializeField] private Transform finishPlayerPoint = default;

    [Header("Actions")]
    public static Action LevelCreatedAction = default;

    #region get/set
    public Transform PlayerSpawnPoint { get => playerSpawnPoint; }
    #endregion

    private void OnEnable()
    {
        GameManager.LevelStartAction += StartLevel;
    }

    private void OnDisable()
    {
        GameManager.LevelStartAction -= StartLevel;
    }

    private void Awake()
    {
        dependencyContainerSO.FinishCameraPosition = finishCameraPosition;
        dependencyContainerSO.PlayerFinishPoint = finishPlayerPoint;
    }

    private void Start()
    {
        LevelCreatedAction?.Invoke();
        LoadDestructions();
    }

    private void StartLevel()
    {
        GamePanelController.SetPointsLevelsAction?.Invoke(playerSpawnPoint, finishTrigger);
        PlayerController.SetPlayerPositionAction?.Invoke(playerSpawnPoint);
        GamePanelController.ShowTutorialAction?.Invoke(true);
    }

    private void LoadDestructions()
    {
        dependencyContainerSO.Destructions.Clear();
        var destructions = FindObjectsOfType<DestructionBarrier>();
        foreach (var destruct in destructions)
        {
            dependencyContainerSO.Destructions.Add(destruct.transform, destruct);
        }
    }
}
