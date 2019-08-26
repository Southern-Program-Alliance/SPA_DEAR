using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objectmotion : MonoBehaviour
{
    private float minRange, maxRange;
    private float speed, restartSpeed;
    private Vector3 newPosition;
    private void OnEnable()
    {
        EventManager.onReset += Reset;
        EventManager.onReStart += Restart;
    }
    void Start()
    {
        minRange = 1;
        maxRange = 3;
        speed = Random.Range(minRange, maxRange);
        restartSpeed = speed;
    }
    void Update()
    {
        newPosition = transform.position;
        newPosition.x = Mathf.Sin(Time.time) * speed;
        transform.position = newPosition;
    }
    private void OnDisable()
    {
        EventManager.onReset -= Reset;
        EventManager.onReStart -= Restart;
    }
    public void Reset()
    {
        speed = 0;
    }
    public void Restart()
    {
        speed = restartSpeed;
    }
}
