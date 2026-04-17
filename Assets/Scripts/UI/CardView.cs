using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CardView : MonoBehaviour
{
    [Header("UI References")]
    public Image cardArtImage;
    public TMP_Text cardNameText;
    public TMP_Text cardDescText;

    [Header("Data")]
    public CardData data;

    [Header("Runtime")]
    [SerializeField] private Owner owner = Owner.Player;
    private Button btn;

    private Sprite faceUpSprite;
    private bool faceDown;
    private Sprite cardBackSprite;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClicked);
    }

    public void SetCard(CardData newData)
    {
        data = newData;
        if (data != null) faceUpSprite = data.artwork;
        Refresh();
    }

    public void SetOwner(Owner newOwner)
    {
        owner = newOwner;

        if (btn != null)
            btn.interactable = (owner == Owner.Player);
    }

    public void SetFaceDown(bool isFaceDown, Sprite backSprite)
    {
        faceDown = isFaceDown;
        cardBackSprite = backSprite;
        Refresh();
    }

    public void Refresh()
    {
        if (cardArtImage == null) return;

        if (faceDown)
        {
            cardArtImage.sprite = cardBackSprite;
            if (cardNameText) cardNameText.text = "";
            if (cardDescText) cardDescText.text = "";
            return;
        }

        if (data == null) return;

        cardArtImage.sprite = data.artwork;
        if (cardNameText) cardNameText.text = data.cardName;
        if (cardDescText) cardDescText.text = CardEffectText.Build(data);
    }

    void OnClicked()
    {
        if (owner != Owner.Player) return;
        if (data == null) return;

        if (BattleManager.I != null)
            BattleManager.I.OnPlayerCardClicked(this);
    }
}