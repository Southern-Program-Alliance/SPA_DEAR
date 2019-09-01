using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightScript : MonoBehaviour
{
    [SerializeField] Animation _animButtonPress;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void playAnim()
    {
        _animButtonPress.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
