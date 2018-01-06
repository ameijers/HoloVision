using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialCursor : MonoBehaviour
{
    private Renderer meshRenderer = null;

    // Use this for initialization
    void Start ()
    {
        meshRenderer = gameObject.GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update ()
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, SpatialMappingManager.SpatialMappingMask, QueryTriggerInteraction.Collide))
        {
            gameObject.transform.position = hitInfo.point;

            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }

    }
}
