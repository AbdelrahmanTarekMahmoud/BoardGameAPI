using MyBGList.Presistence;

[Route("[controller]")]
[ApiController]
public class MechanicsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MechanicsController> _logger;

    public MechanicsController(ApplicationDbContext context, ILogger<MechanicsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
    public async Task<RestDTO<Mechanic[]>> Get([FromQuery] RequestDTO<MechanicsDTO> input)
    {
        var query = _context.Mechanics.AsQueryable();

        if (!string.IsNullOrEmpty(input.filterQuery))
        {
            query = query.Where(x => x.Name.Contains(input.filterQuery));
        }

        var totalCount = await query.CountAsync();
        var numberOfPages = (int)Math.Ceiling(totalCount / (double)input.pageSize);

        query = query
            .OrderBy($"{input.sortColumn} {input.sortOrder}")
            .Skip(input.pageNumber * input.pageSize)
            .Take(input.pageSize);

        

        return new RestDTO<Mechanic[]>()
        {
            Data = await query.ToArrayAsync(),
            PageNumber = input.pageNumber,
            PageSize = input.pageSize,
            TotalCount = totalCount,
            NumberOfPages = numberOfPages,

            Links = new List<LinkDTO> {
                new LinkDTO(
                    Url.Action(null, "Mechanics", new { input.pageNumber, input.pageSize }, Request.Scheme)!,
                    "self",
                    "GET"),
            }
        };
    }

    [HttpPost]
    [ResponseCache(NoStore = true)]
    public async Task<RestDTO<Mechanic?>> Update([FromBody] MechanicsDTO request)
    {
        var mechanic = await _context.Mechanics.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

        if (mechanic != null)
        {
            if (!string.IsNullOrEmpty(request.Name))
            {
                mechanic.Name = request.Name;
            }
            mechanic.LastModifiedDate = DateTime.Now;
            _context.Update(mechanic);
            await _context.SaveChangesAsync();
        }

        return new RestDTO<Mechanic?>()
        {
            Data = mechanic,
            Links = new List<LinkDTO>
            {
                new LinkDTO
                (Url.Action(null, "Mechanics", new { id = request.Id }, Request.Scheme)
                    ,"self"
                    ,"POST"),
            }
        };
    }

    [HttpDelete("{id}")]
    [ResponseCache(NoStore = true)]
    public async Task<RestDTO<Mechanic?>> Delete([FromRoute] int id)
    {
        var mechanic = await _context.Mechanics.Where(x => x.Id == id).FirstOrDefaultAsync();
        if (mechanic != null)
        {
            _context.Mechanics.Remove(mechanic);
            await _context.SaveChangesAsync();
        }

        return new RestDTO<Mechanic?>()
        {
            Data = mechanic,
            Links = new List<LinkDTO>
            {
                new LinkDTO
                (Url.Action(null, "Mechanics", new { id }, Request.Scheme)
                    ,"self"
                    ,"Delete"),
            }
        };
    }

    [HttpDelete("all", Name = "DeleteAllMechanics")]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> DeleteAllMechanics()
    {
        var allMechanics = _context.Mechanics.ToList();

        if (!allMechanics.Any())
        {
            return NotFound(new { message = "No Mechanics found to delete." });
        }

        _context.Mechanics.RemoveRange(allMechanics);
        await _context.SaveChangesAsync();

        return Ok(new { message = "All Mechanics have been deleted successfully." });
    }
}
