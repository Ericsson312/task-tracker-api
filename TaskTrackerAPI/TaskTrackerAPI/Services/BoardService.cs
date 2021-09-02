using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Repositories;

namespace TaskTrackerApi.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ITagRepository _tagRepository;

        public BoardService(IBoardRepository boardRepository, ICardRepository cardRepository, 
            IMemberRepository memberRepository, ITagRepository tagRepository)
        {
            _boardRepository = boardRepository;
            _cardRepository = cardRepository;
            _memberRepository = memberRepository;
            _tagRepository = tagRepository;
        }

        #region Board service

         public async Task<List<Board>> GetBoardsAsync(PaginationFilter paginationFilter)
         {
             return await _boardRepository.GetBoardsAsync(paginationFilter);
         }

         public async Task<Board> GetBoardByIdAsync(Guid boardId)
         {
             return await _boardRepository.GetBoardByIdAsync(boardId);
         }

         public async Task<bool> CreateBoardAsync(Board board)
        {
            return await _boardRepository.CreateBoardAsync(board);;
        }

        public async Task<bool> UpdateBoardAsync(Board board)
        {
            return await _boardRepository.UpdateBoardAsync(board);
        }

        public async Task<bool> DeleteBoardByIdAsync(Guid boardId)
        {
            var board = await _boardRepository.GetBoardByIdAsync(boardId);

            if (board == null)
            {
                return false;
            }

            return await _boardRepository.DeleteBoardAsync(board);
        }

        public async Task<bool> UserOwnsBoardAsync(Guid boardId, string userId)
        {
            var board = await _boardRepository.GetBoardOwnedByUserAsync(boardId, userId);

            if (board == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UserBelongsToBoard(Guid boardId, string email)
        {
            var board = await _boardRepository.GetBoardWhereUserIsMemberAsync(boardId);

            if (board == null)
            {
                return false;
            }

            var result = board.Members.SingleOrDefault(x => x.MemberEmail == email);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Member service

        public async Task<List<Member>> GetMembersAsync(PaginationFilter paginationFilter)
        {
            return await _memberRepository.GetMembersAsync(paginationFilter);
        }

        public async Task<Member> GetMemberAsync(string email)
        {
            return await _memberRepository.GetMemberAsync(email);
        }

        public async Task<bool> DeleteMemberFromBoardAsync(string email, Board board)
        {
            var memberToDelete = await _memberRepository.GetMemberAsync(email);

            if (memberToDelete == null)
            {
                return true;
            }

            var boardMember = board.Members.Find(x => x.MemberEmail == memberToDelete.Email);
            
            if (boardMember == null)
            {
                return true;
            }

            return await _memberRepository.DeleteMemberFromBoardAsync(boardMember);
        }
        
        public async Task<bool> AddMemberToBoardAsync(string email, Board board)
        {
            var memberToAdd = await _memberRepository.GetMemberAsync(email);

            if (memberToAdd == null)
            {
                return false;
            }

            var boardMember = board.Members?.SingleOrDefault(x => x.MemberEmail == email);
            
            if (boardMember != null)
            {
                return true;
            }

            return await _memberRepository.AddMemberToBoardAsync(memberToAdd, board);
        }

        #endregion
        
        #region Card service
        
        public async Task<List<Card>> GetCardsAsync(PaginationFilter paginationFilter)
        {
            return await _cardRepository.GetCardsAsync(paginationFilter);
        }

        public async Task<Card> GetCardByIdAsync(Guid cardId)
        {
            return await _cardRepository.GetCardByIdAsync(cardId);
        }
        
        public async Task<bool> CreateCardAsync(Card card)
        {
            // normalize Tag names of the Card
            if (card.Tags != null)
            {
                card.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());
                await AddNewTag(card);   
            }

            return await _cardRepository.CreateCardAsync(card);
        }

        public async Task<bool> UpdateCardAsync(Card card)
        {
            if (card.Tags != null)
            {
                card.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());
                await AddNewTag(card);   
            }
            
            return await _cardRepository.UpdateCardAsync(card);
        }

        public async Task<bool> DeleteCardAsync(Guid cardId)
        {
            var card = await _cardRepository.GetCardByIdAsync(cardId);

            if (card == null)
            {
                return false;
            }

            return await _cardRepository.DeleteCardAsync(card);
        }

        #endregion

        #region Tag service
        
        public async Task<List<Tag>> GetTagsAsync(PaginationFilter paginationFilter)
        {
            return await _tagRepository.GetTagsAsync(paginationFilter);
        }

        public async Task<Tag> GetTagByNameAsync(string tagName)
        {
            return await _tagRepository.GetTagByNameAsync(tagName);
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            tag.Name = tag.Name.ToLower();

            var tagExist = await _tagRepository.GetTagByNameAsync(tag.Name);

            if (tagExist != null)
            {
                return true;
            }

            return await _tagRepository.AddTagAsync(new Tag
            {
                Name = tag.Name,
                CreatedOn = DateTime.UtcNow,
                CreatorId = tag.CreatorId
            });
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            var tag = await _tagRepository.GetTagByNameAsync(tagName.ToLower());

            if (tag == null)
            {
                return true;
            }

            var removeTagResult = await _tagRepository.RemoveTagAsync(tag);
            return removeTagResult;
        }
        
        #endregion

        private async Task AddNewTag(Card card)
        {
            foreach (var tag in card.Tags)
            {
                var tagExist = await _tagRepository.GetTagByNameAsync(tag.TagName);

                if (tagExist != null)
                {
                    continue;
                }

                await _tagRepository.AddTagAsync(new Tag
                {
                    Name = tag.TagName,
                    CreatedOn = DateTime.UtcNow,
                    CreatorId = card.UserId
                });
            }
        }
    }
}
