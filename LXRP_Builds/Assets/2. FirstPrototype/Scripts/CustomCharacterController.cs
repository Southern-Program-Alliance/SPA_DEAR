using UnityEngine;
using UnityEngine.AI;

public class CustomCharacterController : MonoBehaviour
{
    public Camera cam;

    public NavMeshAgent agent;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
            

        }
    }
}
