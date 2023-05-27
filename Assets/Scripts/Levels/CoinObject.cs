using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinObject : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private PlayerStorage playerStorageSO = default;
    [SerializeField] private GameStorage gameStorageSO = default;
    [SerializeField] private DependencyContainer dependencyContainerSO = default;

    [Header("Settings")]
    [SerializeField] private float rotateSpeed = default;

    [Header("Components")]
    [SerializeField] private Transform coinBodyTransform = default;

    private void PickUp()
    {
        VibrationController.Vibrate(30);
        SoundManager.PlaySomeSoundOnce?.Invoke(SoundType.PickUpCoin);
        ParticleController.PlayParticleAction?.Invoke(coinBodyTransform.position, ParticleType.Coin);
        playerStorageSO.ConcretePlayer.ChangeCoins(gameStorageSO.GameBaseParameters.CoinReward);
        dependencyContainerSO.RewardedCoins += gameStorageSO.GameBaseParameters.CoinReward;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        coinBodyTransform.localEulerAngles = new Vector3(0f, coinBodyTransform.localEulerAngles.y + rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Tags.Player))
        {
            PickUp();
        }
    }
}
