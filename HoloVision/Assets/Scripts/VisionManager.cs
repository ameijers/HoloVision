using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if !UNITY_EDITOR
using HoloVisionAPI;
#endif

[Serializable]
public class Tags
{
    public List<Tag> tags;
    public string requestId;
    public Metadata metadata;
}

[Serializable]
public class Tag
{
    public string name;
    public string confidence;
}

[Serializable]
public class Metadata
{
    public string width;
    public string height;
    public string format;
}


public class VisionInformation
{
    public string Id;
    public Matrix4x4 Matrix;
    public Vector3 SpatialHitPoint;
    public Quaternion SpatialHitRotation;
    public Tags Tags;

    public GameObject SceneObject;

    public bool IsDirty;
}

public class VisionInformationList : Dictionary<string, VisionInformation>
{
    public VisionInformation GetVisionInformationById(string id)
    {
        return this[id];
    }
}

public class VisionManager : MonoBehaviour
{
    private PhotoCaptureManager mgr = null;

    private VisionInformationList visionInformationList = new VisionInformationList();

    void Start()
    {
        mgr = PhotoCaptureManager.Instance;
    }

    void Update()
    {
        foreach(VisionInformation vi in visionInformationList.Values)
        {
            if (vi.IsDirty)
            {
                CreateVisionPoint(vi);
            }
        }
    }

    private void CreateVisionPoint(VisionInformation vi)
    {
        if (vi.SceneObject == null)
        {
            // Make sure that this is not called from inside the asynchronous call
            // it will cause issues because you need to call it from the main thread
            vi.SceneObject = Instantiate(Resources.Load("VisionPoint", typeof(GameObject))) as GameObject;
        }

        if (vi.SceneObject != null)
        {
            vi.SceneObject.transform.position = vi.SpatialHitPoint;
            vi.SceneObject.transform.rotation = vi.SpatialHitRotation;

            TextMesh mesh = vi.SceneObject.GetComponentInChildren<TextMesh>();
            if (mesh != null)
            {
                string tags = "";

                foreach(Tag tag in vi.Tags.tags)
                {
                    tags += string.Format("{0} ({1})\r\n", tag.name, tag.confidence);
                }

                mesh.text = tags;
            }
        }

        vi.IsDirty = false;
    }

    public void OnTapped()
    {
        // create new vision information
        VisionInformation vi = new VisionInformation();
        vi.Id = Guid.NewGuid().ToString();
        visionInformationList.Add(vi.Id, vi);

        // get hit point 
        GetHitPoint(vi.Id);

        // take photo and enrich vision information
        mgr.TakePhoto(vi.Id, OnPhotoReady);
    }

    public void OnPhotoReady(string id, byte[] photo, Matrix4x4 cameraToWorldMatrix, Resolution cameraResolution)
    {
        VisionInformation vi = visionInformationList.GetVisionInformationById(id);

        if (vi != null)
        {
            vi.Matrix = cameraToWorldMatrix;
        }

#if !UNITY_EDITOR
        VisionAPI visionAPI = new VisionAPI();

        visionAPI.GetDataAsync(id, ConfigSettingscs.VisionURL, photo, ConfigSettingscs.VisionKey, OnGetDataCompleted);
#endif
    }

    private void OnGetDataCompleted(string id, string json)
    {
        VisionInformation vi = visionInformationList.GetVisionInformationById(id);

        if (vi != null)
        {
            vi.Tags = JsonUtility.FromJson<Tags>(json);

            vi.IsDirty = true;
        }
    }

    private void GetHitPoint(string id)
    {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, SpatialMappingManager.SpatialMappingMask, QueryTriggerInteraction.Collide))
        {
            VisionInformation vi = visionInformationList.GetVisionInformationById(id);

            if (vi != null)
            {
                vi.SpatialHitPoint = hitInfo.point;
                vi.SpatialHitRotation = Quaternion.LookRotation(gazeDirection);
            }
        }
    }
}
