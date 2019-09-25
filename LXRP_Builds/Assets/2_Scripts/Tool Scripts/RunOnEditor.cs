using UnityEngine;
using System.Collections;

public class RunOnEditor : MonoBehaviour
{
    [SerializeField] Camera ARCamera = null;

    private void Awake()
    {
        if (Application.isEditor)
        {
            Destroy(ARCamera.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
