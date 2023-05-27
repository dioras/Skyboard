using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private List<ParticleObject> particlePrefab = new List<ParticleObject>();

    [Header("Actions")]
    public static Action<Vector3, ParticleType> PlayParticleAction = default;

    private List<ParticleObject> particleObjects = new List<ParticleObject>();

    private void OnEnable()
    {
        PlayParticleAction += PlayParticle;
    }

    private void OnDisable()
    {
        PlayParticleAction -= PlayParticle;
    }

    private void PlayParticle(Vector3 position, ParticleType particleType)
    {
        ParticleObject particle = GetFreeParticle(particleType);
        particle.transform.position = position;
        particle.Play();
    }

    private ParticleObject GetFreeParticle(ParticleType particleType)
    {
        ParticleObject result = particleObjects.Find((_part) => _part.IsBusy == false && _part.Type == particleType);
        if (result == null)
        {
            result = CreateParticle(particleType);
        }

        return result;
    }

    private ParticleObject CreateParticle(ParticleType particleType)
    {
        ParticleObject result = Instantiate(particlePrefab.Find((_prefab) => _prefab.Type == particleType).gameObject, transform).GetComponent<ParticleObject>();
        particleObjects.Add(result);
        return result;
    }
}
