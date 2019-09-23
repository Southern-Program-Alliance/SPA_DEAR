using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class SpawnManager : MonoBehaviour
{
    // Singleton Members
    private static SpawnManager _instance;
    public static SpawnManager Instance { get { return _instance; } }

    // Class members
    [SerializeField] private ObjectSpawnComponent pedestrianComponent = null;
    [SerializeField] int pedestrianPoolAmt = 25;
    [SerializeField] float pedestrianMaxSpawnDelay = 5.0f;
    [SerializeField] PathCreator[] pedestrianPaths = null;

    private GameObject[] pedestriansPrefabs;
    private IPoolInfo pedestrianPoolInfo;

    [Space]
    [SerializeField] private ObjectSpawnComponent vehicleComponent = null;
    [SerializeField] int vehiclePoolAmt = 25;
    [SerializeField] float vehicleMaxSpawnDelay = 5.0f;
    [SerializeField] PathCreator[] vehiclePaths = null;

    private GameObject[] vehiclePrefabs;
    private IPoolInfo vehiclePoolInfo;

    [Space]
    [SerializeField] PlayerObjectsSpawnComponent playerObjectsComponent = null;
    
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

        PreparePools(seed);
    }

    // CAN BE REFACTORED
    public void PreparePools(int seed)
    {
        // Prepare the poolInfos
        pedestrianPoolInfo = new IPoolInfo(SPAWNSELECTION.PEDESTRIANS, pedestrianPoolAmt, pedestrianMaxSpawnDelay,
            pedestrianPaths, true);
        // Get all prefabs from the list 
        pedestriansPrefabs = Resources.LoadAll<GameObject>("PEDESTRIANS");
        pedestrianComponent.InitialzePool(pedestrianPoolInfo, seed, pedestriansPrefabs);

        // Same for the vehicles as well
        vehiclePoolInfo = new IPoolInfo(SPAWNSELECTION.VEHICLES, vehiclePoolAmt, vehicleMaxSpawnDelay,
            vehiclePaths, true);
        vehiclePrefabs = Resources.LoadAll<GameObject>("VEHICLES");
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

            case SPAWNSELECTION.RULES:
                playerObjectsComponent.SpawnRules();
                break;
        }
    }


}

