public class PikminDestination : DestinationScript
{
    public PikminType PikminType;
    private PikminSpawner Spawner;

    public override void Start()
    {
        Spawner = GetComponent<PikminSpawner>();
        base.Start();
    }

    public override void ItemCollected(CollectedEventArgs Args)
    {
        PikminCarryAble Retrieved = Args.ObjectCollected as PikminCarryAble;
        Retrieved.ReleasePikmin();
        base.ItemCollected(Args);
        int value = Retrieved.CapturePoints;
        if (Retrieved.PreferredType == PikminType)
            value *= 2;
        Spawner.SpawnPikmin(ref PikminManager.Instance.AllPikmin, value);
    }
}