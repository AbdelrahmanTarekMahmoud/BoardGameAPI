using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBGList.DTO;

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
        public IEnumerable<BoardGame> GetBoardGames()
        {
            return new[]
            {
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

            };
        }
    }
}
