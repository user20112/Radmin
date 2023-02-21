using UnityEngine;

public class PikminCarryAble : CarryAble
{
    public PikminType PreferredType;
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
        if (PikminCarryingCount > 0)
        {
            DestinationScript[] Destinations = FindObjectsOfType<DestinationScript>();
            int BluePikmin = 0;
            int RedPikmin = 0;
            int YellowPikmin = 0;
            int WhitePikmin = 0;
            int PurplePikmin = 0;
            foreach (Pikmin pikmin in PikminAssigned)
            {
                if (pikmin is BluePikmin)
                    BluePikmin++;
                if (pikmin is RedPikmin)
                    RedPikmin++;
                if (pikmin is YellowPikmin)
                    YellowPikmin++;
                if (pikmin is WhitePikmin)
                    WhitePikmin++;
                if (pikmin is PurplePikmin)
                    PurplePikmin++;
            }
            bool BlueHighest = BluePikmin >= RedPikmin && BluePikmin >= YellowPikmin;
            bool RedHighest = RedPikmin >= BluePikmin && RedPikmin >= YellowPikmin;
            bool YellowHighest = YellowPikmin >= RedPikmin && YellowPikmin >= BluePikmin;
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
            PikminDestinationEnum TypeToReturnToo = PikminDestinationEnum.Red;
            if (BlueHighest)
                TypeToReturnToo = PikminDestinationEnum.Blue;
            if (RedHighest)
                TypeToReturnToo = PikminDestinationEnum.Red;
            if (YellowHighest)
                TypeToReturnToo = PikminDestinationEnum.Yellow;
            for (int x = 0; x < Destinations.Length; x++)
                if (Destinations[x].DestinationType == TypeToReturnToo)
                    return Destinations[x];
            return Destinations[0];
        }
        else
            return null;
    }

    public override void ReleasePikmin(Pikmin pikmin)
    {
        base.ReleasePikmin(pikmin);
    }

    public override void ReleasePikmin()
    {
        base.ReleasePikmin();
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