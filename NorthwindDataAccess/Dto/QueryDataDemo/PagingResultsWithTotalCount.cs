namespace NorthwindDataAccess.Dto.QueryDataDemo
{
    public class PagingResultsWithTotalCount : PagingResults
    {
        public int ProductId { get; set; }
        public int TotalCount { get; set; }
    }
}
