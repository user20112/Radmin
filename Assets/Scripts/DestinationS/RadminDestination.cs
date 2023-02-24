public class RadminDestination : DestinationScript
{
    public RadminType RadminType;
    private RadminSpawner Spawner;

    public override void Start()
    {
        Spawner = GetComponent<RadminSpawner>();
        base.Start();
    }

    public override void ItemCollected(CollectedEventArgs Args)
    {
        RadminCarryAble Retrieved = Args.ObjectCollected as RadminCarryAble;
        Retrieved.ReleaseRadmin();
        base.ItemCollected(Args);
        int value = Retrieved.CapturePoints;
        if (Retrieved.PreferredType == RadminType)
            value *= 2;
        Spawner.SpawnRadmin(ref RadminManager.Instance.AllRadmin, value);
    }
}