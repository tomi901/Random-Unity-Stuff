using UnityEngine;


public class ObjectSpawner : MonoBehaviour
{

    public string spawnTag;
    int spawnId;


    private void Start()
    {
        spawnId = ObjectPooler.GetIdByTag(spawnTag);
    }

    private void Update()
    {
        ObjectPooler.SpawnObject(spawnId, transform.position, Quaternion.identity);
    }

}
