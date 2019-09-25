using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextComponent : MonoBehaviour, IClickable
{
    [SerializeField] float destroyDelay = 1.0f;
    [SerializeField] GameObject floatingTextPrefab = null;
    [SerializeField] Transform floatingTextSpawnLoc = null;
    [SerializeField] Vector3 randomSpawnOffset = Vector3.zero;

    void Start()
    {

        Destroy(this, destroyDelay);
    }

    public void ShowFloatingText(string txt)
    {
        floatingTextPrefab.GetComponent<TMPro.TextMeshPro>().text = txt;
        Instantiate(floatingTextPrefab, GetSpawnPosition(), transform.rotation, this.transform);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 spawnLoc = floatingTextSpawnLoc.position + new Vector3(
            Random.Range(-randomSpawnOffset.x, randomSpawnOffset.x),
            Random.Range(-randomSpawnOffset.y, randomSpawnOffset.y),
            Random.Range(-randomSpawnOffset.z, randomSpawnOffset.z));

       return spawnLoc;
    }

    public void OnClick()
    {
        throw new System.NotImplementedException();
    }

    public void SendMessageToManager()
    {
        throw new System.NotImplementedException();
    }
}
