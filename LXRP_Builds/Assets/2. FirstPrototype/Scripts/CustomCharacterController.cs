using UnityEngine;
using UnityEngine.AI;

public class CustomCharacterController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    const float locomotionAnimationSmoothTime = .1f;

    private void Start()
    {
        if (cam == null && agent == null && animator)
        {
            Debug.Log("Referrences missing in CustomCharacterController Script");
        }
    }

    void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);

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
