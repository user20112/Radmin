public class ShipDestination : DestinationScript
{
    public override void Start()
    {
    }

    public override void ItemCollected(CollectedEventArgs Args)
    {
        CarryAble Retrieved = Args.ObjectCollected as CarryAble;
        Retrieved.ReleaseRadmin();
        base.ItemCollected(Args);
    }
}