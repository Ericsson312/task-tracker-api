using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly DataContext _dataContext;

        public MemberRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<List<Member>> GetMembersAsync()
        {
            return await _dataContext.Members.AsNoTracking().ToListAsync();
        }

        public async Task<Member> GetMemberAsync(string email)
        {
            return await _dataContext.Members
                .SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> DeleteMemberFromBoardAsync(BoardMember boardMember)
        {
            _dataContext.BoardMembers.Remove(boardMember);
            var result = await _dataContext.SaveChangesAsync();
                
            return result > 0;
        }

        public async Task<bool> AddMemberToBoardAsync(Member member, Board board)
        {
            board.Members.Add(new BoardMember
            {
                Board = board,
                Member = member,
                BoardId = board.Id,
                MemberEmail = member.Email
            });
            
            var result = await _dataContext.SaveChangesAsync();
            
            return result > 0;
        }
    }
}