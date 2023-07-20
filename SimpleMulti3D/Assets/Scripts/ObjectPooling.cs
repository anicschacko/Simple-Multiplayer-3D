using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    
    private List<GameObject> pickupItemPool;

    private void Start()
    {
        pickupItemPool = CreatePickUpItemPool(5);
    }

    private List<GameObject> CreatePickUpItemPool(int size)
    {
        List<GameObject> gameObjects = new List<GameObject>();
        for (int i = 0; i < size; i++)
        {
            gameObjects.Add(Instantiate(_prefab, this.transform));
            gameObjects[i].SetActive(false);
        }

        return gameObjects;
    }

    public GameObject GetItemFromPool()
    {
        GameObject go = null;
        foreach (var item in pickupItemPool)
        {
            if (!item.activeSelf)
                go = item;
            else
            {
                var temp = Instantiate(_prefab, this.transform);
                pickupItemPool.Add(temp);
                return temp;
            }
        }

        if (go != null) go.SetActive(true);

        return go;
    }
}
