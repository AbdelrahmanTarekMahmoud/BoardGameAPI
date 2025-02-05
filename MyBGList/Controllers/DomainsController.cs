using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyBGList.Helpers.CustomModelState;
using MyBGList.Presistence;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DomainsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DomainsController> _logger;

        public DomainsController(ApplicationDbContext context, ILogger<DomainsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        [ManualValidationFilterAttribute]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<ActionResult<RestDTO<Domain[]>>> Get([FromQuery] RequestDTO<DomainDTO> input)
        {
            if(!ModelState.IsValid)
            {
                var details = new ValidationProblemDetails(ModelState);
                details.Extensions["traceId"] =
                System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                //CHECK if the problem is due to pageSize
                if(ModelState.Keys.Any(k => k == "pageSize"))
                {
                    details.Type =
                    "https://tools.ietf.org/html/rfc7231#section-6.6.2";
                    details.Status = StatusCodes.Status501NotImplemented;
                    return new ObjectResult(details)
                    {
                        StatusCode = StatusCodes.Status501NotImplemented
                    };
                }
                else
                {
                    details.Type =
                    "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    details.Status = StatusCodes.Status400BadRequest;
                    return new BadRequestObjectResult(details);
                }
            }

            var query = _context.Domains.AsQueryable();

            if(!string.IsNullOrEmpty(input.filterQuery))
            {
                query = query.Where(x => x.Name.Contains(input.filterQuery));
            }

            var totalCount = await query.CountAsync();
            var numberOfPages = totalCount / input.pageSize;

            query = query
                .OrderBy($"{input.sortColumn} {input.sortOrder}")
                .Skip(input.pageNumber * input.pageSize)
                .Take(input.pageSize);

            

            return new RestDTO<Domain[]>()
            {
                Data = await query.ToArrayAsync(),
                PageNumber = input.pageNumber,
                PageSize = input.pageSize,
                TotalCount = totalCount,
                NumberOfPages = numberOfPages,

                Links = new List<LinkDTO> {
                new LinkDTO(
                Url.Action(null, "Domains", new {input.pageNumber , input.pageSize }, Request.Scheme)!,
                "self",
                "GET"),
                }

            };
                
        }

        [HttpPost]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<Domain?>> Update([FromBody]DomainDTO request)
        {
            var domain = await _context.Domains.Where(x => x.Id ==  request.Id).FirstOrDefaultAsync();

            if(domain != null)
            {
                if(!string.IsNullOrEmpty(request.Name))
                {
                    domain.Name = request.Name;
                }
                domain.LastModifiedDate = DateTime.Now;
                _context.Update(domain);
                await _context.SaveChangesAsync();
            }
            return new RestDTO<Domain?>()
            {
                Data = domain,
                Links = new List<LinkDTO>
                {
                    new LinkDTO
                    (Url.Action(null , "Domains" , request , Request.Scheme)
                        ,"self"
                        ,"POST"),
                }
            };
        }


        [HttpDelete("{id}")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<Domain?>> Delete([FromRoute] int id)
        {
            var domain = await _context.Domains.Where(x => x.Id == id).FirstOrDefaultAsync();
            if(domain != null)
            {
                _context.Domains.Remove(domain);
                await _context.SaveChangesAsync();
            }

            return new RestDTO<Domain?>()
            {
                Data = domain,
                Links = new List<LinkDTO>
                {
                    new LinkDTO
                    (Url.Action(null , "Domains" , id , Request.Scheme)
                        ,"self"
                        ,"Delete"),
                }
            };
        }


        [HttpDelete("all", Name = "DeleteAllDomains")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> DeleteAllDomains()
        {
            var allDomains = _context.Domains.ToList();

            if (!allDomains.Any())
            {
                return NotFound(new { message = "No Domains found to delete." });
            }

            _context.Domains.RemoveRange(allDomains);
            await _context.SaveChangesAsync();

            return Ok(new { message = "All Domains have been deleted successfully." });
        }
    }
}
