using System;

namespace Data
{
	internal enum SynchronizationState
	{
		Waiting,
		Done,
		Pushing,
		InProgress,
		Pulling,
		NotPossible,
		Error
	}
}