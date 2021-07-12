using System.Collections.Generic;
using TaskTrackerApi.Domain;

namespace TaskTrackerApi.Repositories
{
    public interface IBoardRepository
    {
        public List<Board> GetBoards();
        public bool CreateBoard(Board board);
    }
}