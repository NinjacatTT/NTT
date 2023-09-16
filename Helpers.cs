public static class Helpers
{
    public static Player GetPlayer(string alias, List<Player> players)
    {
        foreach (var player in players)
        {
            if (player.Alias == alias)
            {
                return player;
            }
        }

        return null;
    }
}