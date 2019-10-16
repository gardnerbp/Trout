namespace Trout
{
    public class Board
    {
        public const string StartPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public static readonly int[] Files;
        public static readonly int[] WhiteRanks;
        public static readonly int[] BlackRanks;
        public static readonly int[] UpDiagonals;
        public static readonly int[] DownDiagonals;
        public static readonly bool[] LightSquares;
        public static readonly int[][] SquareDistances;
        public static readonly int[] DistanceToCentralSquares;
        public static readonly int[] DistanceToNearestCorner;
        public static readonly int[] DistanceToNearestLightCorner;
        public static readonly int[] DistanceToNearestDarkCorner;
        public static readonly string[] SquareLocations;
        public static readonly ulong[] SquareMasks;
        public static readonly ulong[] FileMasks;
        public static readonly ulong[] RankMasks;
        public static readonly ulong[] UpDiagonalMasks;
        public static readonly ulong[] DownDiagonalMasks;
        public static readonly ulong AllSquaresMask;
        public static readonly ulong EdgeSquareMask;
        public static readonly ulong WhiteCastleQEmptySquaresMask;
        public static readonly ulong WhiteCastleKEmptySquaresMask;
        public static readonly ulong BlackCastleQEmptySquaresMask;
        public static readonly ulong BlackCastleKEmptySquaresMask;
        public readonly ulong[] EnPassantAttackerMasks;
        public readonly ulong[] WhitePawnMoveMasks;
        public readonly ulong[] WhitePawnDoubleMoveMasks;
        public readonly ulong[] WhitePawnAttackMasks;
        public readonly ulong[] BlackPawnMoveMasks;
        public readonly ulong[] BlackPawnDoubleMoveMasks;
        public readonly ulong[] BlackPawnAttackMasks;
        public readonly ulong[] KnightMoveMasks;
        public readonly ulong[] BishopMoveMasks;
        public readonly ulong[] RookMoveMasks;
        public readonly ulong[] KingMoveMasks;
        public PrecalculatedMoves PrecalculatedMoves;
        public long Nodes;
        public long NodesInfoUpdate;
        public long NodesExamineTime;

        private const int _maxPositions = 1024;
        private static readonly ulong[] _squareUnmasks;
        private static readonly ulong _whiteCastleQAttackedSquareMask;
        private static readonly ulong _whiteCastleKAttackedSquareMask;
        private static readonly ulong _blackCastleQAttackedSquareMask;
        private static readonly ulong _blackCastleKAttackedSquareMask;
        private readonly int[][] _neighborSquares;
        private readonly ulong[] _whitePassedPawnMasks;
        private readonly ulong[] _whiteFreePawnMasks;
        private readonly ulong[] _blackPassedPawnMasks;
        private readonly ulong[] _blackFreePawnMasks;
        private readonly int[] _enPassantTargetSquares;
        private readonly int[] _eEnPassantVictimSquares;
        private readonly ulong _piecesSquaresInitialKey;
        private readonly ulong[][] _pieceSquareKeys;
        private readonly ulong[] _sideToMoveKeys;
        private readonly ulong[] _castlingKeys;
        private readonly ulong[] _enPassantKeys;
        private readonly Position[] _positions;
        private readonly Delegates.WriteMessageLine _writeMessageLine;
        private int _positionIndex;

        public Position PreviousPosition => _positionIndex > 0 ? _positions[_positionIndex - 1] : null;

        public Position CurrentPosition => _positions[_positionIndex];

        private Position NextPosition => _positions[_positionIndex + 1];

    }
}