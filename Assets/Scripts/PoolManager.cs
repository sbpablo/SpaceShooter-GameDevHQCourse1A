using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class Item
{
    [SerializeField]
    private GameObject _item;
    [SerializeField]
    private int _count;
    [SerializeField]
    private bool _expandable;

    public int GetCount()
    {
        return _count;
    }

    public GameObject GetObject()
    {
        return _item;
    }

    public bool IsExpandable()
    {
        return _expandable;
    }

}
public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField]
    private List<Item> _objectsToPool;

    [SerializeField]
    private List<GameObject> _pooledObjects = new List<GameObject>();


    private void Start()
    {
        
        foreach (var item in _objectsToPool)
        {
            for (int i =0; i < item.GetCount(); i++ )
            {

                if(item.GetObject() != null)
                {
                    AddObjectToPool(item.GetObject());
                }
                else
                {
                    Debug.LogError("Prefabs for ObjectPooling not found");
                }

                
            }

        }

    }

    public GameObject RetrieveObjectFromPool (string tag)
    {
       var obj= _pooledObjects.FirstOrDefault(o => o.tag == tag && o.activeInHierarchy == false);

        if (obj != null)
        {
            return obj;
        }
        else
        {
            var poolItem = _objectsToPool.FirstOrDefault(o => o.GetObject().tag == tag);

            if (poolItem.IsExpandable())
            {
                obj= AddToAndGetObjectFromPool(poolItem.GetObject());
                return obj;
            }
            
            else return null;

        } 

   

    }

    public void AddObjectToPool (GameObject obj)
    {
        var instantiatedObject = Instantiate(obj);
        instantiatedObject.transform.SetParent(this.transform);
        instantiatedObject.SetActive(false);
         _pooledObjects.Add(instantiatedObject);
    }

    public GameObject AddToAndGetObjectFromPool (GameObject obj)
    {
        var instantiatedObject = Instantiate(obj);
        instantiatedObject.transform.SetParent(this.transform);
        _pooledObjects.Add(instantiatedObject);
        return instantiatedObject;

    }


}
