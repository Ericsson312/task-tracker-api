using System.Collections.Generic;
using System.Linq;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Examples.V1.Requests.Queries;
using TaskTrackerApi.Examples.V1.Responses;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Helpers
{
    public static class PaginationHelpers
    {
        public static PagedResponse<T> CreatePaginationResponse<T>(IUriService uriService, PaginationFilter filter, List<T> response)
        {
            var nextPage = filter.PageNumber >= 1
                ? uriService.GetAllItemsUri(new PaginationQuery(filter.PageNumber + 1, filter.PageSize)).ToString()
                : null;
            
            var previousPage = filter.PageNumber - 1 >= 1
                ? uriService.GetAllItemsUri(new PaginationQuery(filter.PageNumber - 1, filter.PageSize)).ToString()
                : null;

            return new PagedResponse<T>()
            {
                Data = response,
                PageNumber = filter.PageNumber >= 1 ? filter.PageNumber : (int?)null,
                PageSize = filter.PageSize >= 1 ? filter.PageSize : (int?)null,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = previousPage,
                ElementsCont = response.Count
                
            };
        }
    }
}