using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestructionBarrier : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private DependencyContainer dependencyContainerSO = default;

    [Header("Settings")]
    [SerializeField] private bool destructed = false;
    [SerializeField] private float destructionSpeed = .5f;
    [SerializeField] private float bulletSpeed = .3f;
    [SerializeField] private Vector3 bulletPointOffset = new Vector3(0f, 0f, -3f);

    [Header("Components")]
    [SerializeField] private MeshRenderer meshRenderer = default;
    [SerializeField] private Transform bulletTransform = default;
    [SerializeField] private GameObject bulletSphereObject = default;
    [SerializeField] private ParticleSystem particleSystemBullet = default;

    public bool Destructed { get => destructed; set => destructed = value; }

    private void Awake()
    {
        bulletTransform.gameObject.SetActive(false);
    }
    private void Destruction()
    {
        GamePanelController.ShowReactionAction?.Invoke();
        VibrationController.Vibrate(45);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(.4f);
        sequence.Append(meshRenderer.material.DOFloat(1f, "_DissolveAmount", destructionSpeed));
        sequence.OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Tags.Player))
        {
            if (destructed == false)
            {
                destructed = true;
                particleSystemBullet.Play();
                bulletTransform.gameObject.SetActive(true);
                bulletSphereObject.SetActive(true);
                bulletTransform.position = dependencyContainerSO.PlayerController.SkyboardTransform.position;
                bulletTransform.DOMove(transform.position - bulletPointOffset, bulletSpeed).OnComplete(() => {
                    ParticleController.PlayParticleAction?.Invoke(transform.position, ParticleType.Explosion);
                    particleSystemBullet.Stop();
                    bulletSphereObject.SetActive(false);
                    Destruction();
                });
            }
        }
    }
}
