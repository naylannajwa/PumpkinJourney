using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopupImageButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject popup;
    public GameObject backgroundDim;

    private Image img;
    private Color normalColor;
    
    [Header("Hover Settings")]
    [Range(0f, 1f)]
    public float normalAlpha = 1f;
    [Range(0f, 1f)]
    public float hoverAlpha = 0.7f;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    void Start()
    {
        // ⭐ FORCE SET - Abaikan warna asli, set manual
        normalColor = new Color(1f, 1f, 1f, normalAlpha);  // White dengan alpha normal
        
        // ⭐ PAKSA APPLY sekarang juga
        img.color = normalColor;
        
        Debug.Log($"Forced color set - Alpha: {img.color.a}");
    }

    void OnEnable()
    {
        // ⭐ Reset setiap kali enabled
        if (img != null)
        {
            img.color = normalColor;
        } 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool active = !popup.activeSelf;
        popup.SetActive(active);
        backgroundDim.SetActive(active);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Color c = normalColor;  // ⭐ Pakai normalColor, bukan img.color
        c.a = hoverAlpha;
        img.color = c;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.color = normalColor;
    }
} 