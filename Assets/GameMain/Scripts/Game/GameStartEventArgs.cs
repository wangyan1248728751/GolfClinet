using GameFramework.Event;

public class GameStartEventArgs : GameEventArgs
{
	public static readonly int EventId = typeof(GameStartEventArgs).GetHashCode();

	public override int Id
	{
		get
		{
			return EventId;
		}
	}

	public GameStartEventArgs()
	{

	}



	public override void Clear()
	{

	}
}