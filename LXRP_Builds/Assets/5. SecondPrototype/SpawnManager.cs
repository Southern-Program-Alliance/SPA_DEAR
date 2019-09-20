using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class SpawnManager : MonoBehaviour
{
    [SerializeField] PathCreator[] _arrayPath = null;

    [SerializeField] int _objPoolAmt = 25;
    [SerializeField] GameObject[] _arrayCharacterPool = null;

    [SerializeField] bool _canSpawn = true;

    GameObject[] targetArray;

    private void SpawnPedestrains()
    {

    }

    private void ReadyTargetArray()
    {
        
    }

    void Start()
    {
        // Get all prefabs from the list 
        GameObject[] _characterArray = Resources.LoadAll<GameObject>("AICharacters");

        // Get all prefabs from the list 
        GameObject[] _vehicleArray = Resources.LoadAll<GameObject>("Vehicles");


        // Initialize Random Seed
        Random.InitState(System.DateTime.Now.Millisecond);

        InitialzePool(_characterArray); 
        StartCoroutine(CountdownSpawn());
    }


    void InitialzePool(GameObject[] characterArray)
    {
        // Initialize Object Pool
        _arrayCharacterPool = new GameObject[_objPoolAmt];
        for (int i = 0; i < _objPoolAmt; i++)
        {
            GameObject car = Instantiate(characterArray[Random.Range(0, characterArray.Length)]);
            car.SetActive(false);
            car.transform.parent = this.gameObject.transform;
            _arrayCharacterPool[i] = car;
        }

        //Clear array
        System.Array.Clear(characterArray, 0, characterArray.Length);
        characterArray = null;
    }


    IEnumerator CountdownSpawn()
    {
        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < _objPoolAmt && _canSpawn; i++)
        {
            GameObject spawnCharacter = _arrayCharacterPool[i];

            // Skip if already activew
            if (spawnCharacter.activeSelf)
                continue;

            // Add Car Script containing the Path follower code
            PathFollower pathFollowComponent = spawnCharacter.AddComponent<PathFollower>();

            // Choose a random path in the scene
            PathCreator path = _arrayPath[Random.Range(0, _arrayPath.Length)];
            pathFollowComponent.pathCreator = path;
            pathFollowComponent.endOfPathInstruction = EndOfPathInstruction.Loop;
            pathFollowComponent.speed = 0.5f;
            // Make it child of the path gameobject
            spawnCharacter.transform.parent = path.gameObject.transform;

            yield return new WaitForSeconds(Random.Range(1 , 5));
            spawnCharacter.SetActive(true);
        }
    }
}
