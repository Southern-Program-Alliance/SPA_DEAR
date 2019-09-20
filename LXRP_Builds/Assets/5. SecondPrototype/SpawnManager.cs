using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private SpawnPedestrianComponent pedestrianComponent = null;
    [SerializeField] private SpawnVehicleComponent vehicleComponent = null;

    private SPAWNSTATE SPAWNSTATE;

    private GameObject[] targetArray;


    [SerializeField] PathCreator[] _arrayPath = null;

    [SerializeField] int _objPoolAmt = 25;
    [SerializeField] GameObject[] _arrayPool = null;

    [SerializeField] bool _canSpawn = true;

    void Start()
    {
        // Initialize Random Seed
        Random.InitState(System.DateTime.Now.Millisecond);

        // Prepare the pools
        //pedestrianComponent.StartPool(int poolAmt);
    }
}
