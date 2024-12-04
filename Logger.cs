using Rage;

namespace RoutineCallouts
{
	public static class Logger
	{
		private const string Prefix = "[RoutineCallouts]: ";

		public static void Log(string message)
		{
			Game.LogTrivial($"{Prefix}{message}");
		}
	}
}