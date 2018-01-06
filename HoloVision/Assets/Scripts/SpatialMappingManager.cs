using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class SpatialMappingManager : MonoBehaviour
{
    public static int SpatialMappingLayer = 31;

    public static int SpatialMappingMask
    {
        get
        {
            return 1 << SpatialMappingLayer;
        }

    }

	void Start ()
    {
        SpatialMappingCollider collider = gameObject.GetComponent<SpatialMappingCollider>();
        collider.layer = SpatialMappingLayer;
	}
}
