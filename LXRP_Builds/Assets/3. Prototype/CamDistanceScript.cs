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

    Vector3 camPostion = Vector3.zero;
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
        
        //EnableLineRenderer();  
    }

    public void OnObjectPlaced(Vector3 pos)
    {
        isPlaced = true;

        positions[0] = camPostion;
        positions[1] = pos;
        lineRenderer.enabled = true;

        lineRenderer.SetPositions(positions);
    }

    void Update()
    {
        if (!isPlaced)
            return;
        
        CalculateDistance();
    }

    void CalculateDistance()
    {

        positions[0] = mainCam.transform.position;
        lineRenderer.SetPositions(positions);
        Debug.DrawLine(positions[0], positions[1], Color.green, 0.5f);

        float distance;
        distance = Vector3.Distance(lineRenderer.GetPosition(0), 
            lineRenderer.GetPosition(1));
        

        // Display Text
        dispTxt.text = "Distance = " + distance;

        if (distance < 5)
        {
            lineRenderer.material.color = Color.red;
        }
        else
        {
            lineRenderer.material.color = Color.green;
        }
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
}
