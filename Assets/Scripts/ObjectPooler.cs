using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    #region Singleton
    public static ObjectPooler instance;
    public bool dontDesroyOnLoad;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        if (dontDesroyOnLoad)
            DontDestroyOnLoad(this);

        InitPool();
    }
    #endregion


    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public void InitPool(List<string> ignorePoolTags = null)
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            if (ignorePoolTags != null && ignorePoolTags.Contains(pool.tag)) //pool only necessary objects
                return;

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, pool.prefab.transform.position, pool.prefab.transform.rotation);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Transform parent = null, Vector3 position = default, Quaternion rotation = default, Vector3 scale = default)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            print("Pool tag with name " + tag + " doesn't exists!");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        if (parent != null)
        {
            objectToSpawn.transform.SetParent(parent);
            objectToSpawn.transform.localPosition = position;
            objectToSpawn.transform.localRotation = rotation;
        }
        else
        {
            objectToSpawn.transform.parent = null;
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
        }

        if(scale != Vector3.zero)
        {
            objectToSpawn.transform.localScale = scale;
        }

        objectToSpawn.SetActive(true);

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void AddToPool(GameObject objToAdd)
    {
        objToAdd.SetActive(false);
        objToAdd.transform.SetParent(transform);
    }
}
