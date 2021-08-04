using System.Collections.Generic;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public interface IMemberRepository
    {
        Task<List<Member>> GetMembersAsync();
        Task<Member> GetMemberAsync(string email);
        Task<bool> DeleteMemberFromBoardAsync(Member member, Board board);
        Task<bool> AddMemberToBoardAsync(BoardMember boardMember);
    }
}