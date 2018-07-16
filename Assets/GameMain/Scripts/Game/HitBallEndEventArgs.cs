using GameFramework.Event;

public class HitBallEndEventArgs : GameEventArgs
{ 
	public static readonly int EventId = typeof(HitBallEndEventArgs).GetHashCode();

	public override int Id
	{
		get
		{
			return EventId;
		}
	}

	public HitBallEndEventArgs()
	{
		
	}

	

	public override void Clear()
	{
		
	}
}
