using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public interface IMemberRepository
    {
        Task<List<Member>> GetMembersAsNoTrackingAsync();
        Task<Member> GetMemberAsNoTrackingAsync(string email);
        Task<bool> DeleteMemberFromBoardAsync(Member member, Board board);
        Task<bool> AddMemberToBoardAsync(BoardMember boardMember);
    }
}