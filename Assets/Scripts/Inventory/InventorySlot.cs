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
        image.color = isOccupied ? occupiedColor : normalColor;
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

    public void ClearSlot()
    {
        isOccupied = false;
        occupyingItem = null;
        ResetVisual();
    }

    public bool IsFree()
    {
        return !isOccupied;
    }
}