using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionTrigger : MonoBehaviour
{
    [SerializeField] private DependencyContainer dependencyContainerSO = default;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Tags.Player) && dependencyContainerSO.InGame)
        {
            VibrationController.Vibrate(30);
            GamePanelController.ShowReactionAction?.Invoke();
        }
    }
}
