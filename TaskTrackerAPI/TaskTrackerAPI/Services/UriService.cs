using System;
using Microsoft.AspNetCore.WebUtilities;
using TaskTrackerApi.Contracts.V1;
using TaskTrackerApi.Examples.V1.Requests.Queries;

namespace TaskTrackerApi.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }
        
        public Uri GetCardUri(string cardId)
        {
            return new Uri(_baseUri + ApiRoutes.Cards.Get.Replace("{cardId}", cardId));
        }

        public Uri GetBoardUri(string boardId)
        {
            return new Uri(_baseUri + ApiRoutes.Boards.Get.Replace("{boardId}", boardId));
        }

        public Uri GetTagUri(string tagName)
        {
            return new Uri(_baseUri + ApiRoutes.Tags.Get.Replace("{tagName}", tagName));
        }

        public Uri GetAllItemsUri(PaginationQuery pagination)
        {
            var uri = new Uri(_baseUri);
            
            var modifiedUri = QueryHelpers.AddQueryString(uri.ToString(), "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());

            return new Uri(modifiedUri);
        }
    }
}