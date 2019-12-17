using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton <T>: MonoBehaviour where T: MonoSingleton<T>
{
    private static T _instance;

    public static T Instance 
    {
        get

        { if (_instance == null)
            {
                Debug.LogError($" Instance type of {typeof(T)} could not be found");
            }

            return _instance;
        }
 

    }
    private void Awake()
    {
        _instance = (T) this;

    }
   
}
