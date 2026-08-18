using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Surface LayerMask Settings", menuName = "VRIS/Physics/Create New Surface LayerMask Settings")]
public class SurfaceLayerMaskSettings : ScriptableObject
{
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask wallLayer;
    public LayerMask PlatformLayer => platformLayer;
    public LayerMask WallLayer => wallLayer;
}
