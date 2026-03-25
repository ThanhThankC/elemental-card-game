using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [TKC] Generic object pool for any GameObject
/// </summary>
/// <typeparam name="T">Component type to pool</typeparam>
public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> availableObjects = new Queue<T>();
    private readonly List<T> allObjects = new List<T>();

    private readonly int maxSize;

    public ObjectPool(T prefab, Transform parent, int initialSize = 10, int maxSize = 100)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.maxSize = maxSize;

        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    /// <summary>
    /// [TKC] Get an object from Pool (or create new if needed)
    /// </summary>
    public T Get(Vector3 position)
    {
        T obj;

        if (availableObjects.Count > 0)
        {
            obj = availableObjects.Dequeue();
        }
        else
        {
            if (allObjects.Count >= maxSize)
            {
                Debug.LogWarning($"[ObjectPool] Reached max size ({maxSize}). Resuing oldest object.");
                obj = allObjects[0];
            }
            else
            {
                obj = CreateNewObject();
            }
        }

        obj.transform.localPosition = position;
        obj.gameObject.SetActive(true);
        return obj;
    }

    /// <summary>
    /// [TKC] Return object to Pool.
    /// </summary>
    public void Release(T obj)
    {
        if (obj == null) return;

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);

        if (!availableObjects.Contains(obj))
        {
            availableObjects.Enqueue(obj);
        }
    }

    /// <summary>
    /// [TKC] Release all active objects.
    /// </summary>
    public void ReleaseAll()
    {
        foreach (T obj in allObjects)
        {
            if (obj.gameObject.activeSelf)
            {
                Release(obj);
            }
        }
    }

    private T CreateNewObject()
    {
        T newObj = Object.Instantiate(prefab, parent);
        newObj.gameObject.SetActive(false);

        allObjects.Add(newObj);
        availableObjects.Enqueue(newObj);

        return newObj;
    }

    public int TotalCount => allObjects.Count;
    public int AvailableCount => availableObjects.Count;
    public int ActiveCount => allObjects.Count - availableObjects.Count;
}
