using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class SpawnVehicleComponent : MonoBehaviour
{
    [SerializeField] PathCreator[] _arrayPath = null;

    [SerializeField] GameObject[] _arrayVehiclePool = null;

    [SerializeField] bool _canSpawn = true;

    private void Awake()
    {
        TrafficLightScript.stopCarSapwn += EventStopSpawn;
    }


    void InitialzePool(int poolAmt, int seed)
    {
        Random.InitState(seed);
        // Get all prefabs from the list 
        GameObject[] vehicleArray = Resources.LoadAll<GameObject>("Vehicles");

        // Initialize Object Pool
        _arrayVehiclePool = new GameObject[poolAmt];
        for (int i = 0; i < poolAmt; i++)
        {
            GameObject car = Instantiate(vehicleArray[Random.Range(0, vehicleArray.Length)]);
            car.SetActive(false);
            car.transform.parent = this.gameObject.transform;
            _arrayVehiclePool[i] = car;
        }

        //Clear array
        System.Array.Clear(vehicleArray, 0, vehicleArray.Length);
        vehicleArray = null;
    }

    public void StartCarSpawn()
    {
        StartCoroutine(VehicleSpawnCountdown());
    }

    IEnumerator VehicleSpawnCountdown()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Vehicle Spawnner started");

        for (int i = 0; i <   && _canSpawn; i++)
        {
            GameObject spawnCar = _arrayVehiclePool[i];

            // Skip if already active
            if (spawnCar.activeSelf)
                continue;

            // Add Car Script containing the Path follower code
            CarScript spawnCarComponent = spawnCar.AddComponent<CarScript>();
            // Choose a random path in the scene
            PathCreator path = _arrayPath[Random.Range(0, _arrayPath.Length)];
            spawnCarComponent._pathCreator = path;
            spawnCarComponent._endOfPathInstruction = EndOfPathInstruction.Loop;
            // Make it child of the path gameobject
            spawnCar.transform.parent = path.gameObject.transform;

            // Disables the shadows 
            spawnCar.GetComponent<MeshRenderer>().receiveShadows = false;
            spawnCar.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            yield return new WaitForSeconds(5.0f);
            spawnCar.SetActive(true);
        }
    }

    void EventStopSpawn(bool condition)
    {
        //Debug.Log("Stopping Car Spawn");
        _canSpawn = condition;
    }
}
