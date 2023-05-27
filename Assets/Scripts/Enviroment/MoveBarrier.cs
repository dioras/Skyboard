using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBarrier : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speedRotate = 1f;
    [SerializeField] private bool customDirection = false;
    [SerializeField] private Vector3 direction = default;

    private void FixedUpdate()
    {
        if (customDirection == false)
        {
            transform.Rotate(transform.forward * speedRotate);
        }
        else
        {
            transform.Rotate(direction * speedRotate);
        }
    }
}
