using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class SpawnManager : MonoBehaviour
{
    // Singleton Members
    private static SpawnManager _instance;
    public static SpawnManager Instance { get { return _instance; } }

    [SerializeField] private ObjectSpawnComponent pedestrianComponent = null;
    [SerializeField] private ObjectSpawnComponent vehicleComponent = null;

    [Space]
    private GameObject[] vehiclePrefabs;
    private GameObject[] pedestriansPrefabs;

    [Space]
    [SerializeField] private IPoolInfo pedestrianPoolInfo;
    [SerializeField] private IPoolInfo vehiclePoolInfo;

    [Space]
    [SerializeField] float pedestrianMaxSpawnDelay = 5.0f;
    [SerializeField] float vehicleMaxSpawnDelay = 5.0f;

    [SerializeField] int pedestrianPoolAmt = 25;
    [SerializeField] int vehiclePoolAmt = 25;

    [Space]
    [SerializeField] PathCreator[] pedestrianPaths = null;
    [SerializeField] PathCreator[] vehiclePaths = null;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        // Initialize Random Seed
        int seed = System.DateTime.Now.Millisecond;
        Random.InitState(seed);

        // Prepare the pools
        //pedestrianComponent.StartPool(int poolAmt);
        pedestrianPoolInfo = new IPoolInfo(SPAWNSELECTION.PEDESTRIANS, pedestrianPoolAmt, pedestrianMaxSpawnDelay,
            pedestrianPaths, true);

        vehiclePoolInfo = new IPoolInfo(SPAWNSELECTION.VEHICLES, vehiclePoolAmt, vehicleMaxSpawnDelay,
            vehiclePaths, true);

        // Get all prefabs from the list 
        pedestriansPrefabs = Resources.LoadAll<GameObject>("PEDESTRIANS");
        // Get all prefabs from the list 
        vehiclePrefabs = Resources.LoadAll<GameObject>("VEHICLES");

        pedestrianComponent.InitialzePool(pedestrianPoolInfo, seed, pedestriansPrefabs);
        vehicleComponent.InitialzePool(vehiclePoolInfo, seed, vehiclePrefabs);
    }

    public void StartSpawn(SPAWNSELECTION whatToSpawn)
    {
        switch(whatToSpawn)
        {
            case SPAWNSELECTION.PEDESTRIANS:
                pedestrianComponent.StartSpawn();
                break;

            case SPAWNSELECTION.VEHICLES:
                vehicleComponent.StartSpawn();
                break;
        }
    }
}

