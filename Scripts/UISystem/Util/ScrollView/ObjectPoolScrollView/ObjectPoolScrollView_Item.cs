using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPoolScrollView_Item : MonoBehaviour
{
    public abstract void UpdateItem(int _index);
    public abstract void Clear();

    public void SetActive(bool _active)
    {
        gameObject.SetActive(_active);
    }
}
