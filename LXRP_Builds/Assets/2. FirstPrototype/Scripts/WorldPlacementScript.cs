using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;
//using UnityEngine.Experimental.XR;
using System;

public class WorldPlacementScript : MonoBehaviour
{
    [SerializeField] GameObject objectToPlace;
    [SerializeField] GameObject placementIndicator;
    [SerializeField] Camera MainCam;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycast;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private bool ObjectPlaced = false;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRaycast = arOrigin.GetComponent<ARRaycastManager>();

        objectToPlace.SetActive(false);
    }

    void Update()
    {
        if (ObjectPlaced)
            return;

        Debug.Log("Placement script working");

        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        //Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        objectToPlace.SetActive(true);

        objectToPlace.transform.position = placementPose.position;
        objectToPlace.transform.rotation = placementPose.rotation;

        ObjectPlaced = true;
        GameManager.instance.PSetState(2);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = MainCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycast.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = MainCam.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}