using System.Collections;
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
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    private bool ObjectPlaced = false;

    // Variables for placement script state machine
    private ARState curState = ARState.BLANK;

    [SerializeField] GameObject objectToPlace = null;
    [SerializeField] GameObject placementIndicator = null;
    Camera MainCam = null;

    [SerializeField] MainManager manager = null;

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

    /// The elapsed time ARCore has been detecting at least one plane.
    private float m_DetectedPlaneElapsed = 0;

    /// The elapsed time ARCore has been tracking but not detected any planes.
    private float m_NotDetectedPlaneElapsed;

    /// Indicates whether a lost tracking reason is displayed.
    private bool m_IsLostTrackingDisplayed;

    void Start()
    {
        MainCam = GameObject.FindObjectOfType<Camera>();

        // Disable and log error if missing references 
        _CheckFieldsAreNotNull();

        arRaycast = GetComponent<ARRaycastManager>();
        arOrigin = GetComponent<ARSessionOrigin>();

        // Disable world model
        objectToPlace.SetActive(false);

        // Start the state machince
        SetState(ARState.TUTORIAL);
    }

    // Function to change the state
    void SetState(ARState newState)
    {
        if (curState == newState)
        {
            return;
        }

        curState = newState;
        Debug.Log(curState);
        HandleStateChangedEvent(curState);
    }

    // Function to handle state changes
    void HandleStateChangedEvent(ARState state)
    {
        // Show tutorial
        if (state == ARState.TUTORIAL)
        {
            Debug.Log("Playing Tutorial");
            placementIndicator.SetActive(false);
        }

        // Starting state of the game
        if (state == ARState.PLACEMENT)
        {
            Debug.Log("Placement Started");
        }

        if(state == ARState.PLACED)
        {
            Debug.Log("World Placed");
            ObjectPlaced = true;
            m_SnackBar.SetActive(false);

            // Change the state of the Game Manager to 'Placed'
            
        //Manager.instance.PSetState(2);
        }
    }

    void Update()
    {
        // Dont pass if world is already placed.
        if (ObjectPlaced)
            return;

        Debug.Log("Placement script working");

        if(curState == ARState.TUTORIAL)
        {
            UpdatePlacementPose(); // Check for trackable plane surfaces

            if(m_NotDetectedPlaneElapsed > DisplayGuideDelay)
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
                    m_SnackBarText.text = "Point your camera to where you want to place an object.";
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

                SetState(ARState.PLACEMENT);
            }
        }

        else if(curState == ARState.PLACEMENT)
        {
            UpdatePlacementPose(); // Check for trackable plane surfaces
            UpdatePlacementIndicator();

            // If player taps the screen when a plane is detected
            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
            }
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
            m_DetectedPlaneElapsed += Time.deltaTime;
            m_NotDetectedPlaneElapsed = 0;

            placementPose = hits[0].pose;

            var cameraForward = MainCam.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        else
        {
            m_DetectedPlaneElapsed += 0;
            m_NotDetectedPlaneElapsed = Time.deltaTime;
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

    private void PlaceObject()
    {
        //Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        objectToPlace.SetActive(true);

        objectToPlace.transform.position = placementPose.position;
        objectToPlace.transform.rotation = placementPose.rotation;

        SetState(ARState.PLACED);
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