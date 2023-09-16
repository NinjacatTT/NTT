public class Player
{
    public Player(string alias, int rating, int num_matches)
    {
        this.Alias = alias;
        this.Rating = rating;
        this.Num_matches = num_matches;
    }

    public string Alias { get; set; }
    public int Rating { get; set; }
    public int Num_matches { get; set; }
}