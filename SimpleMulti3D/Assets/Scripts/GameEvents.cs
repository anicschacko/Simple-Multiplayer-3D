using System;
using System.Collections.Generic;

public static class GameConstants
{
	public enum Scenes
	{
		MainMenu = 0,
		Loading = 1,
		Game = 2
	}

	public const string Score = "score";
	public const string LeaderboardData = "ldData";
}

public static class GameEvents
{
	public static Action<string> UpdateScore;
}
