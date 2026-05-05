using UnityEngine;

public class PickupHighlight : MonoBehaviour
{
    private Renderer[] renderers;
    private Material[][] materials;
    private Color[][] originalColors;
    private Color[][] originalEmissions;

    public Color highlightColor = Color.yellow;
    public float emissionStrength = 1.5f;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        materials = new Material[renderers.Length][];
        originalColors = new Color[renderers.Length][];
        originalEmissions = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].materials;
            originalColors[i] = new Color[materials[i].Length];
            originalEmissions[i] = new Color[materials[i].Length];

            for (int j = 0; j < materials[i].Length; j++)
            {
                if (materials[i][j].HasProperty("_BaseColor"))
                    originalColors[i][j] = materials[i][j].GetColor("_BaseColor");
                else if (materials[i][j].HasProperty("_Color"))
                    originalColors[i][j] = materials[i][j].color;

                if (materials[i][j].HasProperty("_EmissionColor"))
                    originalEmissions[i][j] = materials[i][j].GetColor("_EmissionColor");
            }
        }
    }

    public void TurnHighlightOn()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                Material mat = materials[i][j];

                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", highlightColor);
                else if (mat.HasProperty("_Color"))
                    mat.color = highlightColor;

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", highlightColor * emissionStrength);
                }
            }
        }
    }

    public void TurnHighlightOff()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                Material mat = materials[i][j];

                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", originalColors[i][j]);
                else if (mat.HasProperty("_Color"))
                    mat.color = originalColors[i][j];

                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", originalEmissions[i][j]);
            }
        }
    }
}