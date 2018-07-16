using GameFramework.Event;

public class HitBallResultEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(HitBallResultEventArgs).GetHashCode();

    public override int Id
    {
        get
        {
            return EventId;
        }
    }

    public HitBallResultEventArgs(int _rewardMapId)
    {
        rewardMapId = _rewardMapId;
    }

    public int rewardMapId
    {
        private set;
        get;
    }

    public override void Clear()
    {
        rewardMapId = default(int);
    }
}
