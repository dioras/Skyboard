using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObject : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;

    [Header("Components")]
    [SerializeField] private Rigidbody rigidbodyBullet = default;

    public Rigidbody RigidbodyBullet { get => rigidbodyBullet; set => rigidbodyBullet = value; }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag.Equals(Tags.DestructionBarrier))
        {
            if (dependencyContainerSO.Destructions.ContainsKey(collision.transform))
            {
                //dependencyContainerSO.Destructions[collision.transform].Destruction();
            }
            rigidbodyBullet.isKinematic = true;
        }
    }
}
