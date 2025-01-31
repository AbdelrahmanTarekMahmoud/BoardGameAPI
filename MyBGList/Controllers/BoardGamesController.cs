namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        //dependency Injection
        private readonly ILogger<BoardGamesController> _logger;
        public BoardGamesController(ILogger<BoardGamesController> logger) 
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any , Duration = 60)]
        public RestDTO<BoardGame[]> GetBoardGames()
        {
            return new RestDTO<BoardGame[]>()
            {
                Data = new BoardGame[] {
                new BoardGame()
                {
                    Id = 1,
                    Name = "Max Payne",
                    Year = 2001,
                    MinPlayers = 1,
                    MaxPlayers = 2
                },
                new BoardGame()
                {
                    Id = 2,
                    Name = "Ghost Of Thsiuma",
                    Year = 2020,
                    MinPlayers = 1,
                    MaxPlayers = 3
                },
                new BoardGame()
                {
                    Id = 3,
                    Name = "Sekiro",
                    Year = 2017,
                    MinPlayers = 1,
                    MaxPlayers = 1
                }
                },
                Links = new List<LinkDTO> { 
                new LinkDTO(
                Url.Action(null, "BoardGames", null, Request.Scheme)!,
                "self",
                "GET"),
                }

            };
        }
    }
}
