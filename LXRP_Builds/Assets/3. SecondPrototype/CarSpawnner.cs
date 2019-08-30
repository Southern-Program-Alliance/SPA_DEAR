using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class CarSpawnner : MonoBehaviour
{
    [SerializeField] PathCreator _path;
    [SerializeField] List<GameObject> _listVehicle;

    bool _canSpawn = true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCountdown());
    }

    IEnumerator SpawnCountdown()
    {
        while (_canSpawn)
        {
            yield return new WaitForSeconds(5.0f);
            SpawnACar();
        }
    }

    void SpawnACar()
    {
        Debug.Log("SpawnACar");
        if (!_canSpawn)
            return;

        Debug.Log("Spawnning a car");

        // Random.seed = (int)Time.deltaTime;
        GameObject spawnCar = Instantiate(_listVehicle[Random.Range(0, _listVehicle.Count-1)]);

        PathFollower spawnCarComponent = spawnCar.AddComponent<PathFollower>();
        spawnCarComponent.pathCreator = _path;
        spawnCarComponent.endOfPathInstruction = EndOfPathInstruction.Loop;

        spawnCar.GetComponent<MeshRenderer>().receiveShadows = false;
        spawnCar.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

}
