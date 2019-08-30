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
        SpawnCountdown();
    }

    IEnumerator SpawnCountdown()
    {
        yield return new WaitForEndOfFrame();
    }


    void SpawnACar()
    {
        if (!_canSpawn)
            return;
        Random.seed = (int)Time.deltaTime;
        GameObject spawnCar = _listVehicle[Random.Range(0, _listVehicle.Count-1)];
    }

}
