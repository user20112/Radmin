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
    public PrefabRetrieverScript()
    {
        Instance = this;
    }
    private void Start()
    {
        Instance = this;
    }

    public static GameObject GetPrefabFromDestinationType(RadminDestinationEnum Type)
    {
        switch (Type)
        {
            case RadminDestinationEnum.Red:
                return Instance.RedFractionPrefab;

            case RadminDestinationEnum.Blue:
                return Instance.BlueFractionPrefab;

            case RadminDestinationEnum.Yellow:
                return Instance.YellowFractionPrefab;

            case RadminDestinationEnum.White:
                return Instance.WhiteFractionPrefab;

            case RadminDestinationEnum.Purple:
                return Instance.PurpleFractionPrefab;

            case RadminDestinationEnum.Ship:
                return Instance.ShipFractionPrefab;
        }
        return Instance.ShipFractionPrefab;
    }
}