using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public interface IMemberRepository
    {
        Task<List<Member>> GetMembersAsync();
        Task<Member> GetMemberAsync(string email);
        Task<bool> DeleteMemberFromBoardAsync(BoardMember boardMember);
        Task<bool> AddMemberToBoardAsync(Member member, Board board);
    }
}