using System;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cameraTargetSpeed = 10f;
    [SerializeField] private float cameraTargetSpeedRotation = 10f;
    [SerializeField] private float cameraMoveDuration = 1.5f;
    [SerializeField] private float cameraRotateDuration = 1f;

    private Transform target = default;
    private Sequence camSequence = default;

    public static Action<Transform> SetTargetAction = default;
    public static Action<Vector3, Vector3, Action> SetTweenCameraPositionAction = default;
    public static Action<Vector3, Vector3> SetCameraPositionAction = default;


    private void OnEnable()
    {
        SetTargetAction += SetCameraTarget;
        SetTweenCameraPositionAction += SetTweenCameraPosition;
        SetCameraPositionAction += SetCameraPosition;
    }

    private void OnDisable()
    {
        SetTargetAction -= SetCameraTarget;
        SetTweenCameraPositionAction -= SetTweenCameraPosition;
        SetCameraPositionAction -= SetCameraPosition;
    }

    private void Update()
    {
        if (target != null)
        {
            //transform.position = Vector3.Slerp(transform.position, target.position, cameraTargetSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, cameraTargetSpeedRotation * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, target.position, cameraTargetSpeed * Time.deltaTime);
        }
    }

    //private void FixedUpdate()
    //{
    //    if (target != null)
    //    {
    //        //transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, cameraTargetSpeedRotation * Time.deltaTime);
    //        transform.position = Vector3.Lerp(transform.position, target.position, cameraTargetSpeed * Time.deltaTime);
    //    }
    //}

    private void SetCameraTarget(Transform _target)
    {
        ClearCameraTarget();
        target = _target;
    }

    private void ClearCameraTarget()
    {
        target = null;
        if (camSequence != null)
        {
            camSequence.Complete();
        }
    }

    private void SetCameraPosition(Vector3 _position, Vector3 _euler)
    {
        ClearCameraTarget();
        transform.position = _position;
        transform.eulerAngles = _euler;
    }

    private void SetTweenCameraPosition(Vector3 _position, Vector3 _euler, Action _action = null)
    {
        ClearCameraTarget();
        camSequence = DOTween.Sequence();
        camSequence.Append(transform.DOMove(_position, cameraMoveDuration));
        camSequence.Join(transform.DORotate(_euler, cameraRotateDuration));
        camSequence.OnComplete(() => {
            _action?.Invoke();
            camSequence = null;
        });
    }
}
