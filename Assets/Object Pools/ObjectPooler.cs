using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : MonoBehaviour
{

    #region Singleton
    static ObjectPooler instance;

    private void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
            DontDestroyOnLoad(this);
            Initialize();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    #endregion

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int amount;

        Queue<GameObject> objectsQueue = null;
        Transform poolContainer = null;

        public Pool (string tag, GameObject prefab, int amount, bool expandable = true)
        {
            this.tag = tag;
            this.prefab = prefab;
            this.amount = Mathf.Max(amount, 1);
        }

        public void InitializePool (ObjectPooler objectPooler)
        {
            //Check if the queue exist
            //If it does exist, delete all the objects and clear the queue
            //If not, create a new queue instance
            if (objectsQueue != null)
            {
                foreach (GameObject obj in objectsQueue) Destroy(obj);
                objectsQueue.Clear();
            }
            else objectsQueue = new Queue<GameObject>();

            //Create a new pool container if it doesn't exist and parent it to the Object Pooler
            poolContainer = new GameObject(tag).transform;
            poolContainer.parent = objectPooler.transform;

            //Add all the objects to the pool
            for (int i = 0; i < amount; i++) AddObject(prefab);
        }
        void AddObject (GameObject gameObject)
        {
            GameObject newObject = Instantiate(prefab);

            objectsQueue.Enqueue(newObject);
            newObject.SetActive(false);
            newObject.transform.parent = poolContainer.transform;
        }

        public GameObject Spawn (Vector3 position, Quaternion rotation)
        {
            if (objectsQueue.Count <= 0) return null;

            GameObject spawnedObject = objectsQueue.Dequeue();
            spawnedObject.SetActive(false);

            spawnedObject.transform.position = position;
            spawnedObject.transform.rotation = rotation;

            objectsQueue.Enqueue(spawnedObject);
            spawnedObject.SetActive(true);
            return spawnedObject;
        }
    }

    public Pool[] addPools;
    Dictionary<int, Pool> poolDictionary = new Dictionary<int, Pool>();
    int lastUsableKey = 0;

    const int nullId = -1;
    public static int GetIdByTag (string tag)
    {
        if (!instance) return nullId;

        bool tagFound = false;
        int foundId = 0;
        foreach (var kvp in instance.poolDictionary)
        {
            if (kvp.Value.tag == tag)
            {
                foundId = kvp.Key;
                tagFound = true;
            }
        }

        return tagFound ? foundId : nullId;
    }

    public static Pool GetPoolByTag (string tag)
    {
        if (!instance) return null;

        foreach (var kvp in instance.poolDictionary)
        {
            if (kvp.Value.tag == tag)
            {
                return kvp.Value;
            }
        }
        return null;
    }

    void Initialize()
    {
        foreach (Pool pool in addPools) AddPool(pool);
    }

    bool AddPool (Pool pool)
    {
        poolDictionary.Add(lastUsableKey++, pool);
        pool.InitializePool(this);

        return true;
    }

    public static GameObject SpawnObject (string tag, Vector3 position, Quaternion rotation)
    {
        return GetPoolByTag(tag)?.Spawn(position, rotation);
    }
    public static GameObject SpawnObject(int id, Vector3 position, Quaternion rotation)
    {
        Pool getPool = null;
        if (instance && instance.poolDictionary.TryGetValue(id, out getPool))
        {
            return getPool.Spawn(position, rotation);
        }
        else return null;
    }
}
