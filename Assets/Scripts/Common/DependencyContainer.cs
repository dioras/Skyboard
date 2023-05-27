using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DependencyContainer", fileName = "DependencyContainer")]
public class DependencyContainer : ScriptableObject
{
    public bool InGame { get; set; } = false;
    public Transform PlayerTransform { get; set; }
    public PlayerController PlayerController { get; set; }
    public Joystick Joystick { get; set; }
    public Transform MainMenuCameraPosition { get; set; }
    public Transform StoreMenuCameraPosition { get; set; }
    public Transform FinishCameraPosition { get; set; }
    public Transform PlayerFinishPoint { get; set; }
    public int RewardedCoins { get; set; }
    public Dictionary<Transform, DestructionBarrier> Destructions { get; set; } = new Dictionary<Transform, DestructionBarrier>();
    public bool IsTutorialShow { get; set; } 
}
