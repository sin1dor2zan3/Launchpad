using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image image;

    public Color normalColor = Color.white;
    public Color occupiedColor = Color.gray;
    public Color highlightValid = Color.green;
    public Color highlightInvalid = Color.red;

    public bool isOccupied = false;
    public GameObject occupyingItem;

    private void Awake()
    {
        image = GetComponent<Image>();
        ResetVisual();
    }

    public void ResetVisual()
    {
        if (isOccupied)
            image.color = occupiedColor;
        else
            image.color = normalColor;
    }

    public void SetHighlight(Color color)
    {
        image.color = color;
    }

    public void SetOccupied(GameObject item)
    {
        isOccupied = true;
        occupyingItem = item;
        image.color = occupiedColor;
    }
}