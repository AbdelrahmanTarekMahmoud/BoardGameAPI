/*
RestDTO.cs The class containing the data and the
links that will be sent to the client 
 */
namespace MyBGList.DTO
{
    public class RestDTO<T>
    {
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
        public int? PageSize { get; set; }
        public int? PageNumber {  get; set; }
        public int? TotalCount { get; set; }
        public int? NumberOfPages { get; set; }
        public T Data { get; set; } = default!;

    }
}
