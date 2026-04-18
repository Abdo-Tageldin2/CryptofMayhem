using System.Collections.Generic;
using UnityEngine;

// object pool for CardView — avoids Instantiate/Destroy every time a card is drawn or discarded
public class CardPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public CardView cardPrefab;
    [Range(5, 30)] public int startSize = 13;

    private List<CardView> pool = new List<CardView>();
    private Transform poolParent;

    void Awake()
    {
        // hidden container so pooled cards don't show up in hand layouts
        GameObject holder = new GameObject("_CardPool");
        holder.transform.SetParent(transform);
        holder.SetActive(false);
        poolParent = holder.transform;
    }

    public void FillPool()
    {
        for (int i = 0; i < startSize; i++)
        {
            CardView cv = Instantiate(cardPrefab, poolParent);
            cv.gameObject.SetActive(false);
            pool.Add(cv);
        }
    }

    public CardView Get(Transform parent)
    {
        CardView cv = null;

        // find an inactive card in the pool
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                cv = pool[i];
                break;
            }
        }

        // pool empty — make a new one
        if (cv == null)
        {
            cv = Instantiate(cardPrefab, poolParent);
            pool.Add(cv);
        }

        cv.transform.SetParent(parent);
        cv.gameObject.SetActive(true);
        return cv;
    }

    public void Return(CardView cv)
    {
        if (cv == null) return;
        cv.gameObject.SetActive(false);
        cv.transform.SetParent(poolParent);
    }
}
