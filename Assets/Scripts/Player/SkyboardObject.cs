using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboardObject : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private List<MeshRenderer> boardRenderrers = new List<MeshRenderer>();
    [SerializeField] private List<TrailRenderer> trails = new List<TrailRenderer>();

    public List<MeshRenderer> BoardRenderrers => boardRenderrers;
    public List<TrailRenderer> Trails => trails;
}
