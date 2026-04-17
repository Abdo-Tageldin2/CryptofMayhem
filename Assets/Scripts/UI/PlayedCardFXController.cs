using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayedCardFXController : MonoBehaviour
{
    public static PlayedCardFXController I;

    [Header("Where played cards end up (has HorizontalLayoutGroup)")]
    public RectTransform playedCardsArea;

    [Header("Prefab (UI Image) for a played card slot")]
    public Image playedCardPrefab;

    [Header("Animation")]
    [Range(0.05f, 2f)] public float slideDuration = 0.25f;
    [Range(0.1f, 2f)]  public float startScale = 0.95f;
    [Range(0.1f, 2f)]  public float endScale = 1.0f;

    private readonly List<GameObject> spawned = new List<GameObject>();

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
    }

    public void AddPlayedCardAnimatedFromWorldPos(Vector3 startWorldPos, Sprite sprite)
    {
        if (playedCardsArea == null || playedCardPrefab == null || sprite == null)
            return;

        Image slot = Instantiate(playedCardPrefab, playedCardsArea);
        slot.sprite = sprite;
        slot.preserveAspect = true;

        RectTransform slotRect = slot.GetComponent<RectTransform>();
        spawned.Add(slot.gameObject);

        StartCoroutine(SlideToSlot(slotRect, startWorldPos));
    }

    IEnumerator SlideToSlot(RectTransform slotRect, Vector3 startWorldPos)
    {
        if (slotRect == null) yield break;

        // allow layout to place it
        yield return null;
        if (slotRect == null) yield break;

        Vector3 endWorldPos = slotRect.position;

        slotRect.position = startWorldPos;
        slotRect.localScale = Vector3.one * startScale;

        float t = 0f;
        while (t < slideDuration)
        {
            if (slotRect == null) yield break;

            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / slideDuration);

            slotRect.position = Vector3.Lerp(startWorldPos, endWorldPos, a);
            slotRect.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, a);

            yield return null;
        }

        if (slotRect != null)
        {
            slotRect.position = endWorldPos;
            slotRect.localScale = Vector3.one * endScale;
        }
    }

    public void ClearPlayedCards()
    {
        for (int i = 0; i < spawned.Count; i++)
            if (spawned[i] != null) Destroy(spawned[i]);

        spawned.Clear();
    }
}