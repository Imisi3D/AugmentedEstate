using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size = 10;
    }


    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;



    #region Sigleton


    public static ObjectPooler instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion
    
   // private Queue<GameObject> objectPool;
    //private Pool pool;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();


        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            
            poolDictionary.Add(pool.tag,objectPool);
        }
    }

    public GameObject SpawnObject(string tag)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        if (poolDictionary[tag].Count == 2)
        {

            GameObject objectpo = poolDictionary[tag].Peek();

            for (int i = 0; i < 10; i++)
            {
                GameObject obj = Instantiate(objectpo);
                obj.SetActive(false);
                poolDictionary[tag].Enqueue(obj);
            }
            
        
            
            
        }
        
        GameObject imageobj = poolDictionary[tag].Dequeue();
        imageobj.SetActive(true);
        
        poolDictionary[tag].Enqueue(imageobj);

        return imageobj;

    }

    public void RemoveObject(string tag,GameObject obj)
    {
        obj.SetActive(false);
        
        poolDictionary[tag].Enqueue(obj);
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
