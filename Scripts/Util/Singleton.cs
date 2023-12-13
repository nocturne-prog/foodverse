using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;

    public static T s
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;
                //Object not found, we create a temporary one.
                if (instance == null)
                {
                    GameObject gameObject = new GameObject(typeof(T).Name);
                    instance = gameObject.AddComponent<T>();

                    GameObject container = GameObject.Find("ManagerContainer");

                    if (container is null)
                    {
                        container = new GameObject("ManagerContainer");
                    }

                    gameObject.transform.parent = container.transform;
                }

            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        instance = this as T;

    }

    private void OnDestroy()
    {
        if (instance != null)
        {
            instance = null;
        }
    }

    public void Refresh()
    {
    }

}