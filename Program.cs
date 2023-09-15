var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// use port 80 and 443 if not in development
if (!app.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://*:8080");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var players = new List<Player>
{
    new Player("arianga", 1000, 5),
    new Player("eldasilva", 1000, 6)
};

app.MapGet("/players", () => players).WithName("GetPlayers");

app.MapPost("/players", (Player player) =>
{
    foreach (var p in players)
    {
        if (p.Alias == player.Alias)
        {
            return Results.BadRequest("Player already exists");
        }
    }

    players.Add(player);
    return Results.Created($"/players/{player.Alias}", player);
}).WithName("AddPlayer");

app.MapGet("/players/{alias}", (string alias) =>
{
    Player player = Helpers.GetPlayer(alias, players);
    return player!=null ? Results.Ok(player) : Results.NotFound($"Player with alias {alias} not found");
}).WithName("GetPlayer");

app.MapPost("/match/report", (Match match) =>
{
    Player player_1 = Helpers.GetPlayer(match.Player_1_alias, players);
    Player player_2 = Helpers.GetPlayer(match.Player_2_alias, players);

    foreach (var player in players)
    {
        if (player.Alias == match.Player_1_alias)
        {
            player_1 = player;
        }

        if (player.Alias == match.Player_2_alias)
        {
            player_2 = player;
        }
    }

    var e1 = 1 / (1 + Math.Pow(10, ((player_2.Rating - player_1.Rating) / 400)));
    var e2 = 1 / (1 + Math.Pow(10, ((player_1.Rating - player_2.Rating) / 400)));

    var f1 = player_1.Num_matches > 10 ? 32 : 16;
    var f2 = player_2.Num_matches > 10 ? 32 : 16;

    int n1 = (int)(player_1.Rating + f1 * ((match.Score_1 > match.Score_2 ? 1 : 0) - e1));
    int n2 = (int)(player_2.Rating + f2 * ((match.Score_1 < match.Score_2 ? 1 : 0) - e2));

    players.Remove(player_1);
    players.Remove(player_2);

    players.Add(new Player(player_1.Alias, n1, player_1.Num_matches + 1));
    players.Add(new Player(player_2.Alias, n2, player_2.Num_matches + 1));

    return new List<Player> { new Player(player_1.Alias, n1, player_1.Num_matches + 1), new Player(player_2.Alias, n2, player_2.Num_matches + 1) };
}).WithName("ReportMatch");

app.MapGet("/leaderboard", () =>
{
    var sorted_players = players.OrderByDescending(player => player.Rating);
    return sorted_players;
}).WithName("GetLeaderboard");

app.Run();
