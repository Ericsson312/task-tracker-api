using System;
using System.Threading.Tasks;
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
        public async Task GetBoardById_ShouldReturnBoard_WhenBoardExists()
        {
            // Arrange
            var boardId = Guid.NewGuid();
            var name = "Test Name";
            var description = "Test Description";
            
            var boardDto = new Board
            {
                Id = boardId,
                Name = name,
                Description = description
            };
            _boardRepository.Setup(x => x.GetBoardByIdAsNoTrackingAsync(boardId))
                .ReturnsAsync(boardDto);
                
            // Act
            var board = await _sut.GetBoardByIdAsync(boardId);

            // Assert
            Assert.Equal(boardId, board.Id);
        }
    }
}