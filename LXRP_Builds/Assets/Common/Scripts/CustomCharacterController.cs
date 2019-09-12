using System;
using UnityEngine;
using UnityEngine.AI;

public class CustomCharacterController : MonoBehaviour
{
    private Camera mainCamera;
    private NavMeshAgent agent;
    private Animator animator;

    const float locomotionAnimationSmoothTime = .1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainCamera = GameObject.FindObjectOfType<Camera>();
    }

    private void Start()
    {
        CheckReferences();
    }

    private void CheckReferences()
    {
        if(mainCamera == null)
        {
            Debug.Log("CustomCharacterController: Reference missing - 'camera'");
        }
        else if(agent == null)
        {
            Debug.Log("CustomCharacterController: Reference missing - 'agent'");
        }
        else if(animator == null)
        {
            Debug.Log("CustomCharacterController: Reference missing - 'animator'");
        }
    }

    private void OnDisable()
    {
        animator.SetFloat("speedPercent", 0, 0, 0);
        agent.isStopped = true;
    }

    private void OnEnable()
    {
        agent.isStopped = false;
    }

    void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, locomotionAnimationSmoothTime, Time.deltaTime);

        if (agent.remainingDistance <= 0.001)
            TargetLocation.Instance.Disable();


#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
                TargetLocation.Instance.SetLocation(hit.point);
            }
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    agent.SetDestination(hit.point);
                    TargetLocation.Instance.SetLocation(hit.point);
                }
            }
#endif
    }
}
