using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class CharacterSpawnner : MonoBehaviour
{
    [SerializeField] PathCreator[] _arrayPath = null;

    [SerializeField] int _objPoolAmt = 25;
    [SerializeField] GameObject[] _arrayCharacterPool = null;

    [SerializeField] bool _canSpawn = true;

    void Start()
    {
        // Get all prefabs from the list 
        GameObject[] _characterArray = Resources.LoadAll<GameObject>("AICharacters");
        // Initialize Random Seed
        Random.InitState(System.DateTime.Now.Millisecond);

        InitialzePool(_characterArray);
        StartCoroutine(SpawnCountdown());
    }


    void InitialzePool(GameObject[] characterArray)
    {
        // Initialize Object Pool
        _arrayCharacterPool = new GameObject[_objPoolAmt];
        for (int i = 0; i < _objPoolAmt; i++)
        {
            GameObject pedestrian = Instantiate(characterArray[Random.Range(0, characterArray.Length)]);
            pedestrian.SetActive(false);
            pedestrian.transform.parent = this.gameObject.transform;
            _arrayCharacterPool[i] = pedestrian;
        }

        //Clear array
        System.Array.Clear(characterArray, 0, characterArray.Length);
        characterArray = null;
    }


    IEnumerator SpawnCountdown()
    {
        Debug.Log("SpawnCountdown of Character Spawnner started");
        yield return new WaitForSeconds(3.0f);

        for (int i = 0; i < _objPoolAmt && _canSpawn; i++)
        {
            GameObject spawnedPedestrian = _arrayCharacterPool[i];

            Debug.Log("asdfhgasdfkjhgasdfkhjgasdfs-------------------");

            // Skip if already active
            if (spawnedPedestrian.activeSelf)
                continue;

            Debug.Log("----------------------------------------------");

            // Add Path Follower Component with PedAnimController
            PathFollower pathFollowComponent = spawnedPedestrian.AddComponent<PathFollower>();
            spawnedPedestrian.AddComponent<PedAnimController>();

            // Choose a random path in the scene
            PathCreator path = _arrayPath[Random.Range(0, _arrayPath.Length)];
            pathFollowComponent.pathCreator = path;
            pathFollowComponent.endOfPathInstruction = EndOfPathInstruction.Loop;
            pathFollowComponent.speed = 0.5f;
            // Make it child of the path gameobject
            spawnedPedestrian.transform.parent = path.gameObject.transform;

            yield return new WaitForSeconds(Random.Range(1 , 5));
            spawnedPedestrian.SetActive(true);
        }
    }
}
