using System;
using TaskTrackerApi.Examples.V1.Requests.Queries;

namespace TaskTrackerApi.Services
{
    public interface IUriService
    {
        Uri GetCardUri(string cardId);
        Uri GetBoardUri(string boardId);
        Uri GetTagUri(string tagName);
        Uri GetAllItemsUri(PaginationQuery pagination);
    }
}