using System;
using UnityEngine;

public class EnviromentController : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;
    [SerializeField] private PlayerStorage playerStorageSO = default;
    [SerializeField] private GameStorage gameStorageSO = default;

    [Header("Components")]
    [SerializeField] private Transform playerBodyPreparePosition = default;

    [Header("Camera positions")]
    [SerializeField] private Transform mainCameraTransfrom = default;
    [SerializeField] private Transform storeCameraTransfrom = default;

    [Header("Level")]
    [SerializeField] private Vector3 offsetPosition = new Vector3(0f, 0f, 30f);
    [SerializeField] private LevelController currencyLevel = default;

    [Header("Actions")]
    public static Action PrepareLevelAction = default;

    private void OnEnable()
    {
        PrepareLevelAction += PrepareLevel;
    }

    private void OnDisable()
    {
        PrepareLevelAction -= PrepareLevel;
    }

    private void Awake()
    {
        dependencyContainerSO.MainMenuCameraPosition = mainCameraTransfrom;
        dependencyContainerSO.StoreMenuCameraPosition = storeCameraTransfrom;
    }

    private void PrepareLevel()
    {
        LoadCurrencyLevel();
        PlayerController.HidePlayerBodyAction?.Invoke();
        UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.None);
        CameraController.SetCameraPositionAction?.Invoke(mainCameraTransfrom.position, mainCameraTransfrom.eulerAngles);
        UIController.ShowUIPanelAloneAction?.Invoke(UIPanelType.Main);
        PlayerController.EffectShowPlayerAction?.Invoke();
        PlayerController.SetPlayerBodyPositionAction?.Invoke(playerBodyPreparePosition);
        PlayerController.SetPlayerPositionAction?.Invoke(currencyLevel.PlayerSpawnPoint);
    }

    private void LoadCurrencyLevel()
    {
        if (currencyLevel != null)
        {
            Destroy(currencyLevel.gameObject);
        }

        Level loadedLevel = gameStorageSO.GetLevelFromMission(playerStorageSO.ConcretePlayer.PlayerCurrentMission, playerStorageSO.ConcretePlayer.PlayerWorld);
        currencyLevel = Instantiate(loadedLevel.LevelController.gameObject, Vector3.zero + offsetPosition, Quaternion.identity, transform).GetComponent<LevelController>();
    }
}
