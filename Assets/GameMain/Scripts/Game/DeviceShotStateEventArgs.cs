using GameFramework.Event;

public class DeviceShotStateEventArgs : GameEventArgs
{
	public static readonly int EventId = typeof(DeviceShotStateEventArgs).GetHashCode();

	public override int Id
	{
		get
		{
			return EventId;
		}
	}

	public DeviceShotState State;

	public DeviceShotStateEventArgs(DeviceShotState state)
	{
		State = state;
	}

	public override void Clear()
	{

	}

	public enum DeviceShotState
	{
		Start,
		End,
		Error,
	}
}
