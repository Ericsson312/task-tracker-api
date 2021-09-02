using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using TaskTrackerApi.Data;
using TaskTrackerApi.Domain;
using TaskTrackerApi.Repositories;
using TaskTrackerApi.Services;
using Xunit;

namespace TaskTracker_Moq
{
    public class BoardServiceTests
    {
        private readonly BoardService _sut;
        private readonly Mock<IBoardRepository> _boardRepository = new Mock<IBoardRepository>();
        private readonly Mock<ICardRepository> _cardRepository = new Mock<ICardRepository>();
        private readonly Mock<IMemberRepository> _memberRepository = new Mock<IMemberRepository>();
        private readonly Mock<ITagRepository> _tagRepository = new Mock<ITagRepository>();

        public BoardServiceTests()
        {
            _sut = new BoardService(_boardRepository.Object, _cardRepository.Object,
                _memberRepository.Object, _tagRepository.Object);
        }
        
        [Fact]
        public async Task GetBoards_ShouldReturnAllBoards_WhenBoardsExists()
        {
            // Arrange
            var boardList = new List<Board>{
                new Board
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Name",
                    Description = "Test Description"
                },
                new Board
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Name",
                    Description = "Test Description"
                }
            };

            var paginationFilter = new PaginationFilter
            {
                PageNumber = 1,
                PageSize = 10
            };
            
            _boardRepository.Setup(x => x.GetBoardsAsync(paginationFilter))
                .ReturnsAsync(boardList);
                
            // Act
            var boards = await _sut.GetBoardsAsync(paginationFilter);

            // Assert
            Assert.Equal(boardList.Count, boards.Count);
        }
        
        
        [Fact]
        public async Task GetBoardById_ShouldReturnBoard_WhenBoardExists()
        {
            // Arrange
            var boardId = Guid.NewGuid();
            var boardName = "Test Name";
            var description = "Test Description";
            var cardId = Guid.NewGuid();
            var cardName = "Test";
            var tagName = "Test";
            var email = "test@test,com";
            var memberCount = 1;
            var cardCount = 1;

            var tag = new Tag
            {
                Name = tagName
            };
            
            var member = new Member
            {
                Email = email
            };
            
            var boardMember = new BoardMember
            {
                BoardId = boardId,
                MemberEmail = email,
                Member = member
            };

            var cardTag = new CardTag
            {
                CardId = cardId,
                TagName = tag.Name,
                Tag = tag
            };
            
            var card = new Card
            {
                Id = cardId,
                Name = cardName,
                Tags = new List<CardTag>
                {
                    cardTag
                }
            };

            var boardDto = new Board
            {
                Id = boardId,
                Name = boardName,
                Description = description,
                Cards = new List<Card>
                {
                    card
                },
                Members = new List<BoardMember>
                {
                    boardMember
                }
            };
            
            _boardRepository.Setup(x => x.GetBoardByIdAsync(boardId))
                .ReturnsAsync(boardDto);
                
            // Act
            var board = await _sut.GetBoardByIdAsync(boardId);

            // Assert
            Assert.Equal(boardId, board.Id);
            Assert.Equal(boardName, board.Name);
            Assert.Equal(description, board.Description);
            Assert.Equal(cardCount, board.Cards.Count);
            Assert.Equal(memberCount, board.Members.Count);
            Assert.Equal(cardId, board.Cards[0].Id);
            Assert.Equal(cardName, board.Cards[0].Name);
            Assert.Equal(email, board.Members[0].MemberEmail);
            Assert.Equal(tagName, board.Cards[0].Tags[0].TagName);
        }

        [Fact]
        public async Task UpdateBoard_ShouldReturnBoard_WhenBoardExists()
        {
            // Arrange
            var name = "Test Name";
            var description = "Test Description";
            
            var boardDto = new Board
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description
            };
            
            _boardRepository.Setup(x => x.UpdateBoardAsync(boardDto))
                .ReturnsAsync(true);
            
            // Act
            var boardUpdated = await _sut.UpdateBoardAsync(boardDto);

            // Assert
            Assert.True(boardUpdated);
        }

        [Fact]
        public async Task DeleteBoard_ShouldReturnTrue_WhenBoardDeleted()
        {
            // Arrange
            var boardId = Guid.NewGuid();

            var boardDto = new Board
            {
                Id = boardId,
                Name = "Test Name",
                Description = "Test Description",
                Cards = new List<Card>(),
                Members = new List<BoardMember>()
            };
            
            _boardRepository.Setup(x => x.GetBoardByIdAsync(boardId))
                .ReturnsAsync(boardDto);
            _boardRepository.Setup(x => x.DeleteBoardAsync(boardDto))
                .ReturnsAsync(true);
                
            // Act
            var boardDeleted = await _sut.DeleteBoardByIdAsync(boardId);

            // Assert
             Assert.True(boardDeleted);
        }

        [Fact]
        public async Task UserBelongsToBoard_ShouldReturnTrue_WhenUserBelongsToBoard()
        {
            // Arrange
            var boardId = Guid.NewGuid();
            var email = "test@test,com";

            var member = new Member
            {
                Email = email
            };
            
            var boardMember = new BoardMember
            {
                BoardId = boardId,
                MemberEmail = email,
                Member = member
            };
            
            var boardDto = new Board
            {
                Id = boardId,
                Name = "Test Name",
                Description = "Test Description",
                Cards = new List<Card>(),
                Members = new List<BoardMember>
                {
                    boardMember
                }
            };
            
            _boardRepository.Setup(x => x.GetBoardWhereUserIsMemberAsync(boardId))
                .ReturnsAsync(boardDto);
            
            // Act
            var userStatus = await _sut.UserBelongsToBoard(boardId, email);

            // Assert
            Assert.True(userStatus);
        }

        [Fact]
        public async Task UserOwnsBoardAsync_ShouldReturnTrue_WhenUserOwnsTheBoard()
        {
            // Arrange
            var boardId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var boardDto = new Board
            {
                Id = boardId,
                UserId = userId.ToString(),
                Name = "Test Name",
                Description = "Test Description",
                Cards = new List<Card>(),
                Members = new List<BoardMember>()
            };

            _boardRepository.Setup(x => x.GetBoardOwnedByUserAsync(boardId, userId.ToString()))
                .ReturnsAsync(boardDto);
            
            // Act
            var userIsBoardOwner = await _sut.UserOwnsBoardAsync(boardId, userId.ToString());

            // Assert
            Assert.True(userIsBoardOwner);
        }
    }
}