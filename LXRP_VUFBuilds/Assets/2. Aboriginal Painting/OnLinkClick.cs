using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLinkClick : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                print(hit.collider.name);
                if (hit.collider.gameObject.tag == "Finish")
                {
                    Application.OpenURL("https://parkweb.vic.gov.au/explore/parks/you-yangs-r.p.");
                }
            }
            

        }
    }
}
