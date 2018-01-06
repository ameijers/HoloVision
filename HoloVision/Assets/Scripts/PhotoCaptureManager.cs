using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.WebCam;

public delegate void OnPhotoReady(string id, byte[] photo, Matrix4x4 cameraToWorldMatrix, Resolution cameraResolution);

public class PhotoCaptureManager
{
    private Resolution cameraResolution;
    private PhotoCapture photoCapture;

    private OnPhotoReady photoReady = null;
    private string id;

    private static PhotoCaptureManager instance = null;

    private PhotoCaptureManager()
    {
    }

    public static PhotoCaptureManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PhotoCaptureManager();
            }

            return instance;
        }
    }


    public void TakePhoto(string id, OnPhotoReady photoReady)
    {
        this.photoReady = photoReady;
        this.id = id;

        //Get the highest resolution
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        PhotoCapture.CreateAsync(false, OnCaptureResourceCreated);
    }

    void OnCaptureResourceCreated(PhotoCapture captureObject)
    {
        //Assign capture object
        photoCapture = captureObject;

        //Configure camera
        CameraParameters cameraParameters = new CameraParameters();
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        cameraParameters.pixelFormat = CapturePixelFormat.JPEG;

        //Start the photo mode and start taking pictures
        photoCapture.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (photoCapture != null)
        {
            //Take a picture
            photoCapture.TakePhotoAsync(OnCapturePhotoToMemory);
        }
    }

    void OnCapturePhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        Matrix4x4 cameraToWorldMatrix;
        List<byte> buffer = new List<byte>();

        photoCaptureFrame.CopyRawImageDataIntoBuffer(buffer);

        //Check if we can receive the position where the photo was taken
        if (!photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix))
        {
            cameraToWorldMatrix = Matrix4x4.identity;
        }

        if (photoReady != null)
        {
            photoReady(id,  buffer.ToArray(), cameraToWorldMatrix, cameraResolution);
        }

        // stop the photo mode
        photoCapture.StopPhotoModeAsync(OnPhotoModeStopped);
    }

    public void OnPhotoModeStopped(PhotoCapture.PhotoCaptureResult res)
    {
        photoCapture.Dispose();
        photoCapture = null;
    }
}
