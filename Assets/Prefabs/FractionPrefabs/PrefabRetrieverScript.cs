using UnityEngine;

public class PrefabRetrieverScript : MonoBehaviour
{
    public GameObject RedFractionPrefab;
    public GameObject YellowFractionPrefab;
    public GameObject BlueFractionPrefab;
    public GameObject PurpleFractionPrefab;
    public GameObject WhiteFractionPrefab;
    public GameObject ShipFractionPrefab;
    public static PrefabRetrieverScript Instance;
    public Transform CanvasToDrawOn;

    private void Start()
    {
        Instance = this;
    }

    public static GameObject GetPrefabFromDestinationType(PikminDestinationEnum Type)
    {
        switch (Type)
        {
            case PikminDestinationEnum.Red:
                return Instance.RedFractionPrefab;

            case PikminDestinationEnum.Blue:
                return Instance.BlueFractionPrefab;

            case PikminDestinationEnum.Yellow:
                return Instance.YellowFractionPrefab;

            case PikminDestinationEnum.White:
                return Instance.WhiteFractionPrefab;

            case PikminDestinationEnum.Purple:
                return Instance.PurpleFractionPrefab;

            case PikminDestinationEnum.Ship:
                return Instance.ShipFractionPrefab;
        }
        return Instance.ShipFractionPrefab;
    }
}