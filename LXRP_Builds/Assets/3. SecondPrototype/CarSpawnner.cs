using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class CarSpawnner : MonoBehaviour
{
    [SerializeField] PathCreator[] _arrayPath = null;

    [SerializeField] int _objPoolAmt = 25;
    [SerializeField] GameObject[] _arrayVehiclePool = null;

    bool _canSpawn = true;

    void Start()
    {
        // Get all prefabs from the list 
        GameObject[] _vehicleArray = Resources.LoadAll<GameObject>("Vehicles");
        // Initialize Random Seed
        Random.InitState(System.DateTime.Now.Millisecond);

        InitialzePool(_vehicleArray);
        StartCoroutine(SpawnCountdown());
    }


    void InitialzePool(GameObject[] vehicleArray)
    {
        // Initialize Object Pool
        _arrayVehiclePool = new GameObject[_objPoolAmt];
        for (int i = 0; i < _objPoolAmt; i++)
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


    IEnumerator SpawnCountdown()
    {
        for(int i = 0; i < _objPoolAmt && _canSpawn; i++)
        {
            GameObject spawnCar = _arrayVehiclePool[i];

            // Skip if already active
            if (spawnCar.activeSelf)
                continue;

            // Add Car Script containing the Path follower code
            CarScript spawnCarComponent = spawnCar.AddComponent<CarScript>();
            // Choose a random path in the scene
            PathCreator path = _arrayPath[Random.Range(0, _arrayPath.Length)];
            spawnCarComponent.pathCreator = path;
            spawnCarComponent.endOfPathInstruction = EndOfPathInstruction.Loop;
            // Make it child of the path gameobject
            spawnCar.transform.parent = path.gameObject.transform;

            // Disables the shadows 
            spawnCar.GetComponent<MeshRenderer>().receiveShadows = false;
            spawnCar.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            yield return new WaitForSeconds(5.0f);
            spawnCar.SetActive(true);
        }
    }
}
