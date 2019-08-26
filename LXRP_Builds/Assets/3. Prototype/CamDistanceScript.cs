using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class CamDistanceScript : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer = null;
    [SerializeField] Camera mainCam = null;
    [SerializeField] Text dispTxt = null;

    Vector3[] positions = new Vector3[2];

    bool isPlaced = false;

    // Set up listener to event
    private void OnEnable()
    {
        PlacementScript.onplaced += OnObjectPlaced;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!CheckRefs())
            return;
        
        EnableLineRenderer();  
    }

    private void EnableLineRenderer()
    {
        positions[0] = mainCam.transform.position;
        positions[1] = Vector3.zero;
        lineRenderer.SetPositions(positions);
        lineRenderer.enabled = false;
    }

    public void OnObjectPlaced(Vector3 pos)
    {
        isPlaced = true;

        positions[1] = pos;
        lineRenderer.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaced)
            return;
        UpdateCamPosition();
        CalculateDistance();
    }

    void UpdateCamPosition()
    {

    }

    void CalculateDistance()
    {
        float distance;
        distance = Vector3.Distance(lineRenderer.GetPosition(0), 
            lineRenderer.GetPosition(1));

        if(distance < 5)
        {
            lineRenderer.material.color = Color.red;
        }
        else
        {
            lineRenderer.material.color = Color.green;
        }

        // Display Text
        dispTxt.text = "Distance = " + distance;

    }

    private bool CheckRefs()
    {
        if (lineRenderer == null)
        {
            Debug.Log("CamDistanceScript - Reference Not Set: lineRenderer");
            return false;
        }
        if (mainCam == null)
        {
            Debug.Log("CamDistanceScript - Reference Not Set: mainCam");
            return false;
        }
        return true;
    }

    public void SetPlaced(Vector3 pos)
    {
        isPlaced = true;
    }

}
