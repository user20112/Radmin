using UnityEngine;

public class RadminCarryAble : CarryAble
{
    public RadminType PreferredType;
    public int CapturePoints;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override DestinationScript GetUpdatedDestination()
    {
        if (RadminCarryingCount > 0)
        {
            DestinationScript[] Destinations = FindObjectsOfType<DestinationScript>();
            int BlueRadmin = 0;
            int RedRadmin = 0;
            int YellowRadmin = 0;
            int WhiteRadmin = 0;
            int PurpleRadmin = 0;
            foreach (Radmin radmin in RadminAssigned)
            {
                if (radmin is BlueRadmin)
                    BlueRadmin++;
                if (radmin is RedRadmin)
                    RedRadmin++;
                if (radmin is YellowRadmin)
                    YellowRadmin++;
                if (radmin is WhiteRadmin)
                    WhiteRadmin++;
                if (radmin is PurpleRadmin)
                    PurpleRadmin++;
            }
            bool BlueHighest = BlueRadmin >= RedRadmin && BlueRadmin >= YellowRadmin;
            bool RedHighest = RedRadmin >= BlueRadmin && RedRadmin >= YellowRadmin;
            bool YellowHighest = YellowRadmin >= RedRadmin && YellowRadmin >= BlueRadmin;
            bool GetRandom = !BlueHighest && !YellowHighest && !RedHighest;
            if (GetRandom)
                switch (Random.Range(0, 2))
                {
                    case 0:
                        RedHighest = true;
                        break;

                    case 1:
                        BlueHighest = true;
                        break;

                    case 2:
                        YellowHighest = true;
                        break;
                }
            RadminDestinationEnum TypeToReturnToo = RadminDestinationEnum.Red;
            if (BlueHighest)
                TypeToReturnToo = RadminDestinationEnum.Blue;
            if (RedHighest)
                TypeToReturnToo = RadminDestinationEnum.Red;
            if (YellowHighest)
                TypeToReturnToo = RadminDestinationEnum.Yellow;
            for (int x = 0; x < Destinations.Length; x++)
                if (Destinations[x].DestinationType == TypeToReturnToo)
                    return Destinations[x];
            return Destinations[0];
        }
        else
            return null;
    }

    public override void ReleaseRadmin(Radmin radmin)
    {
        base.ReleaseRadmin(radmin);
    }

    public override void ReleaseRadmin()
    {
        base.ReleaseRadmin();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}