using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class SpawnPedestrianComponent : MonoBehaviour
{
    [SerializeField] PathCreator[] _arrayPath = null;

    [SerializeField] GameObject[] _arrayCharacterPool = null;

    [SerializeField] bool _canSpawn = true;


    void InitialzePool(int poolAmt, int seed)
    {
        Random.InitState(seed);

        // Get all prefabs from the list 
        GameObject[] characterArray = Resources.LoadAll<GameObject>("AICharacters");

        // Initialize Object Pool
        _arrayCharacterPool = new GameObject[poolAmt];
        for (int i = 0; i < poolAmt; i++)
        {
            SpawnPedestrian(characterArray);
        }
        //Clear array
        System.Array.Clear(characterArray, 0, characterArray.Length);
        characterArray = null;
    }

    private void SpawnPedestrian(GameObject[] characterArray)
    {
        GameObject pedestrian = Instantiate(characterArray[Random.Range(0, characterArray.Length)]);
        pedestrian.SetActive(false);
        pedestrian.transform.parent = this.gameObject.transform;
        _arrayCharacterPool[_arrayCharacterPool.Length - 1] = pedestrian;
    }


    public void StartPedSpawn()
    {
        StartCoroutine(PedSpawnCountdown());
    }

    IEnumerator PedSpawnCountdown()
    {
        Debug.Log("Character Spawnner started");

        for (int i = 0; i < _objPoolAmt && _canSpawn; i++)
        {
            GameObject spawnedPedestrian = _arrayCharacterPool[i];

            // Skip if already active
            if (spawnedPedestrian.activeSelf)
                continue;

            // Add Path Follower Component with PedAnimController
            PathFollower pathFollowComponent = spawnedPedestrian.AddComponent<PathFollower>();
            spawnedPedestrian.AddComponent<PedAnimController>();

            // Choose a random path in the scene
            PathCreator path = _arrayPath[UnityEngine.Random.Range(0, _arrayPath.Length)];
            pathFollowComponent.pathCreator = path;
            pathFollowComponent.endOfPathInstruction = EndOfPathInstruction.Loop;
            pathFollowComponent.speed = 0.5f;
            // Make it child of the path gameobject
            spawnedPedestrian.transform.parent = path.gameObject.transform;
            spawnedPedestrian.SetActive(true);

            yield return new WaitForSeconds(UnityEngine.Random.Range(1 , 5));   
        }
    }
}
