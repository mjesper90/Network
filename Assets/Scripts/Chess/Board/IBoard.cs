using Chess.Pieces;

namespace Chess
{
    public interface IBoard
    {
        void Initialize();

        Tile GetTile(int x, int y);

        void SetPiece(Tile tile, Piece piece);

        void MovePiece(Tile from, Tile to);

        void RemovePiece(Tile tile);

        void AddPiece(Tile tile, Piece piece);

        void Reset();
    }
}