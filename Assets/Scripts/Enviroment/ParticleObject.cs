using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ParticleType particleType = default;

    [Header("Components")]
    private new ParticleSystem particleSystem = default;

    #region get/set
    public bool IsBusy { get; set; }
    public ParticleType Type { get => particleType; }
    #endregion

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        particleSystem.Play();
        StartCoroutine(WaitToDeadParticle(particleSystem.main.duration));
    }

    private IEnumerator WaitToDeadParticle(float duration)
    {
        IsBusy = true;
        yield return new WaitForSecondsRealtime(duration);
        IsBusy = false;
    }
}

