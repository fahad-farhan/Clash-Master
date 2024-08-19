using UnityEngine;

public class PlayersColorizer : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Material soldiersMaterial;

    private void Awake()
    {
        SpinWheel.OnColorSelected += ColorizeSoldiers;
    }

    private void OnDestroy()
    {
        SpinWheel.OnColorSelected -= ColorizeSoldiers;
    }

    private void ColorizeSoldiers(Color color)
    {
        soldiersMaterial.SetColor("_BaseColor", color);
    }
}
