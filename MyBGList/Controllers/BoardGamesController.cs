using Microsoft.IdentityModel.Tokens;
using MyBGList.Entities;
using MyBGList.Helpers.CustomValidators;
using MyBGList.Presistence;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        //dependency Injection
        private readonly ILogger<BoardGamesController> _logger;
        private readonly ApplicationDbContext _context;


        public BoardGamesController(ILogger<BoardGamesController> logger
            , ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }



        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any , Duration = 60)]
        public async Task<ActionResult<RestDTO<BoardGame[]>>> GetBoardGames([FromQuery] RequestDTO<BoardGameDTO> input)
        {
          
            //for chained query
            var query = _context.BoardGames.AsQueryable();
            //checks if there is a filterQuery 
            if(!string.IsNullOrEmpty(input.filterQuery))
            {
                query = query.Where(x => x.Name.Contains(input.filterQuery));
            }

            var totalCount = await query.CountAsync();
            var numberOfPages = totalCount / input.pageSize;

            //chained query applying sorting and paging
            query = query
                .OrderBy($"{input.sortColumn} {input.sortOrder}")
                .Skip(input.pageNumber * input.pageSize)
                .Take(input.pageSize);

            return new RestDTO<BoardGame[]>()
            {
                Data = await query.ToArrayAsync(),

                
                PageNumber = input.pageNumber,
                PageSize = input.pageSize,
                TotalCount = totalCount,
                NumberOfPages = numberOfPages,

                Links = new List<LinkDTO> {
                new LinkDTO(
                Url.Action(null, "BoardGames", new {input.pageNumber , input.pageSize }, Request.Scheme)!,
                "self",
                "GET"),
                }

            };
        }


        [HttpPost(Name = "UpdateBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> UpdateBoardGame(BoardGameDTO request)
        {
            var boardGame = await _context.BoardGames.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if(boardGame != null)
            {
                if (!string.IsNullOrEmpty(request.Name))
                    boardGame.Name = request.Name;
                if (request.Year.HasValue && request.Year.Value > 0)
                    boardGame.Year = request.Year.Value;
                boardGame.LastModifiedDate = DateTime.Now;
                _context.BoardGames.Update(boardGame);
                await _context.SaveChangesAsync();
            }

            return new RestDTO<BoardGame?>()
            {
                Data = boardGame,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(
                    Url.Action(
                            null,
                            "BoardGames",
                            request,
                            Request.Scheme)!,
                    "self",
                    "POST"),
                }
            };
        }


        [HttpDelete("{id}")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Delete(int id)
        {
            var boardGame = await _context.BoardGames.Where(x => x.Id == id).FirstOrDefaultAsync();
            if( boardGame != null )
            {
                 _context.BoardGames.Remove(boardGame);
                await _context.SaveChangesAsync();
            }

            return new RestDTO<BoardGame?>()
            {
                Data = boardGame,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(

                        Url.Action(nameof(Delete), "BoardGames", new { id }, Request.Scheme)!,
                        "self",
                        "Delete"
                    )
                }
            };
        }

        [HttpDelete("all", Name = "DeleteAllBoardGames")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> DeleteAllBoardGames()
        {
            var allBoardGames = _context.BoardGames.ToList();

            if (!allBoardGames.Any())
            {
                return NotFound(new { message = "No board games found to delete." });
            }

            _context.BoardGames.RemoveRange(allBoardGames);
            await _context.SaveChangesAsync();

            return Ok(new { message = "All board games have been deleted successfully." });
        }

    }
}
