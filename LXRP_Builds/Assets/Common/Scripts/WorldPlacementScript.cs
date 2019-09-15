﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
//using UnityEngine.XR.ARSubsystems;
//using UnityEngine.Experimental.XR;
using System;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(ARSessionOrigin))]
public class WorldPlacementScript : MonoBehaviour
{
    private ARRaycastManager arRaycast;
    private ARSessionOrigin arOrigin;
    private Camera mainCam;

    private bool isObjectPlaced = false;
    private ARSTATE CURRSTATE = ARSTATE.BLANK;

    [SerializeField] GameObject objectToPlace = null;
    [SerializeField] GameObject placementIndicator = null;

    private Pose placementPose;
    private bool placementPoseIsValid = false;

    [Space]
    /// The time to delay, after ARCore loses tracking of any planes, showing the plane
    /// discovery guide.
    [Tooltip("The time to delay, after ARCore loses tracking of any planes, showing the plane " +
                "discovery guide.")]
    public float DisplayGuideDelay = 3.0f;

    /// The time to delay, after displaying the plane discovery guide, offering more detailed
    /// instructions on how to find a plane.
    [Tooltip("The time to delay, after displaying the plane discovery guide, offering more detailed " +
                "instructions on how to find a plane.")]
    public float OfferDetailedInstructionsDelay = 8.0f;

    /// The time to delay, after Unity Start, showing the plane discovery guide.
    private const float k_OnStartDelay = 1f;

    /// The time to delay, after a at least one plane is tracked by ARCore, hiding the plane discovery guide.
    private const float k_HideGuideDelay = 0.75f;

    /// The duration of the hand animation fades.
    private const float k_AnimationFadeDuration = 0.15f;

    /// The duration of the hand animation fades.
    private const float k_AnimationHideDelay = 3.15f;

    [Space]
    /// The RawImage that provides rotating hand animation.
    [Tooltip("The RawImage that provides rotating hand animation.")]
    [SerializeField] private RawImage m_HandAnimation = null;

    /// The snackbar Game Object.
    [Tooltip("The snackbar Game Object.")]
    [SerializeField] private GameObject m_SnackBar = null;

    /// The snackbar text.
    [Tooltip("The snackbar text.")]
    [SerializeField] private Text m_SnackBarText = null;

    /// The Game Object that contains the button to open the help window.
    [Tooltip("The Game Object that contains the button to open the help window.")]
    [SerializeField] private GameObject m_OpenButton = null;

    /// The Game Object that contains the window with more instructions on how to find a plane.
    [Tooltip(
        "The Game Object that contains the window with more instructions on how to find " +
        "a plane.")]
    [SerializeField] private GameObject m_MoreHelpWindow = null;

    /// The Game Object that contains the button to close the help window.
    [Tooltip("The Game Object that contains the button to close the help window.")]
    [SerializeField] private Button m_GotItButton = null;

    [Space][Space]
    /// The elapsed time ARCore has been detecting at least one plane.
    [SerializeField] private float m_DetectedPlaneElapsed = 0;

    /// The elapsed time ARCore has been tracking but not detected any planes.
    [SerializeField] private float m_NotDetectedPlaneElapsed;

    /// Indicates whether a lost tracking reason is displayed.
    [SerializeField] private bool m_IsLostTrackingDisplayed;

    private void Awake()
    {
        mainCam = GameObject.FindObjectOfType<Camera>();

        arRaycast = GetComponent<ARRaycastManager>();
        arOrigin = GetComponent<ARSessionOrigin>();

        m_OpenButton.GetComponent<Button>().onClick.AddListener(_OnOpenButtonClicked);
        m_GotItButton.onClick.AddListener(_OnGotItButtonClicked);
    }

    void Start()
    {
        // Disable and log error if missing references 
        _CheckFieldsAreNotNull();

        // Disable world model
        objectToPlace.SetActive(false);

        // Start the state machince
        //SetState(ARSTATE.BLANK);
    }

    public void OnDestroy()
    {
        m_OpenButton.GetComponent<Button>().onClick.RemoveListener(_OnOpenButtonClicked);
        m_GotItButton.onClick.RemoveListener(_OnGotItButtonClicked);
    }

    // Function to change the state
    public void SetState(ARSTATE newState)
    {
        if (CURRSTATE == newState)
        {
            return;
        }

        Debug.Log("ARSTATE change from:  " + CURRSTATE + "  to:  " + newState);
        CURRSTATE = newState;
        HandleStateChangedEvent(CURRSTATE);
    }

    // Function to handle state changes
    void HandleStateChangedEvent(ARSTATE state)
    {
        // Show tutorial
        if (state == ARSTATE.TUTORIAL)
        {
            placementIndicator.SetActive(false);
        }

        // Starting state of the game
        if (state == ARSTATE.PLACEMENT)
        {
            
        }

        if(state == ARSTATE.PLACED)
        {
            isObjectPlaced = true;

            m_HandAnimation.enabled = false;
            m_SnackBar.SetActive(false);
            m_OpenButton.SetActive(false);

            MainManager.Instance.SetState(GAMESTATE.PLACED);
        }
    }

    void Update()
    {
        // Dont pass if world is already placed.
        if (isObjectPlaced)
            return;

        if(CURRSTATE == ARSTATE.TUTORIAL)
        {
            UpdatePlacementPose(); // Check for trackable plane surfaces

            if (m_NotDetectedPlaneElapsed > DisplayGuideDelay)
            {
                if (!m_HandAnimation.enabled)
                {
                    m_HandAnimation.GetComponent<CanvasRenderer>().SetAlpha(0f);
                    m_HandAnimation.CrossFadeAlpha(1f, k_AnimationFadeDuration, false);
                }

                m_HandAnimation.enabled = true;
                m_SnackBar.SetActive(true);

                if (m_NotDetectedPlaneElapsed > OfferDetailedInstructionsDelay)
                {
                    m_SnackBarText.text = "Need Help?";
                    m_OpenButton.SetActive(true);
                }
                else
                {
                    m_SnackBarText.text = "Look for a flat surface with objects near it.";
                    m_OpenButton.SetActive(false);
                }
            }

            else if (m_NotDetectedPlaneElapsed <= 0f || m_DetectedPlaneElapsed > k_AnimationHideDelay)
            {
                m_OpenButton.SetActive(false);

                if (m_HandAnimation.enabled)
                {
                    m_HandAnimation.GetComponent<CanvasRenderer>().SetAlpha(1f);
                    m_HandAnimation.CrossFadeAlpha(0f, k_AnimationFadeDuration, false);
                }

                m_HandAnimation.enabled = false;

                m_SnackBar.SetActive(true);
                m_SnackBarText.text = "Point your camera to where you want to place an object.";

                SetState(ARSTATE.PLACEMENT);
            }
        }

        else if(CURRSTATE == ARSTATE.PLACEMENT)
        {
            //Placeobject at origin if in the editor
            if (Application.isEditor)
            {
                PlaceObject(Vector3.zero, Quaternion.identity);
                return;
            }

            UpdatePlacementPose(); // Check for trackable plane surfaces
            UpdatePlacementIndicator();

            // If player taps the screen when a plane is detected
            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject(placementPose.position, placementPose.rotation);
            }
        }
    }

    /// <summary>
    /// Callback executed when the open button has been clicked by the user.
    /// </summary>
    private void _OnOpenButtonClicked()
    {
        m_MoreHelpWindow.SetActive(true);

        enabled = false;
        //-------------------------------------------------------------------------
        //m_FeaturePoints.SetActive(false);
        m_HandAnimation.enabled = false;
        m_SnackBar.SetActive(false);
    }

    /// <summary>
    /// Callback executed when the got-it button has been clicked by the user.
    /// </summary>
    private void _OnGotItButtonClicked()
    {
        m_MoreHelpWindow.SetActive(false);
        enabled = true;
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = mainCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycast.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            m_DetectedPlaneElapsed += Time.deltaTime;
            m_NotDetectedPlaneElapsed = 0;

            placementPose = hits[0].pose;

            var cameraForward = mainCam.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        else
        {
            m_DetectedPlaneElapsed += 0;
            m_NotDetectedPlaneElapsed += Time.deltaTime;
        }
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

    private void PlaceObject(Vector3 pos, Quaternion rot)
    {
        //Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        objectToPlace.SetActive(true);

        objectToPlace.transform.position = pos;
        objectToPlace.transform.rotation = rot;

        SetState(ARSTATE.PLACED);
    }


    /// <summary>
    /// Checks the required fields are not null, and logs a Warning otherwise.
    /// </summary>
    private void _CheckFieldsAreNotNull()
    {
        if (m_MoreHelpWindow == null)
        {
            Debug.LogError("MoreHelpWindow is null");
        }

        if (m_GotItButton == null)
        {
            Debug.LogError("GotItButton is null");
        }

        if (m_SnackBarText == null)
        {
            Debug.LogError("SnackBarText is null");
        }

        if (m_SnackBar == null)
        {
            Debug.LogError("SnackBar is null");
        }

        if (m_OpenButton == null)
        {
            Debug.LogError("OpenButton is null");
        }
        else if (m_OpenButton.GetComponent<Button>() == null)
        {
            Debug.LogError("OpenButton does not have a Button Component.");
        }

        if (m_HandAnimation == null)
        {
            Debug.LogError("HandAnimation is null");
        }
    }

}