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
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> DeleteMemberFromBoardAsync(Member member, Board board)
        {
            var boardMember = await _dataContext.BoardMembers
                .Where(x => x.MemberEmail == member.Email && 
                            x.BoardId == board.Id).SingleOrDefaultAsync();
            
            _dataContext.BoardMembers.Remove(boardMember);
            var result = await _dataContext.SaveChangesAsync();
                
            return result > 0;
        }

        public async Task<bool> AddMemberToBoardAsync(BoardMember boardMember)
        {
            await _dataContext.BoardMembers.AddAsync(boardMember);
            var result = await _dataContext.SaveChangesAsync();
            
            return result > 0;
        }
    }
}