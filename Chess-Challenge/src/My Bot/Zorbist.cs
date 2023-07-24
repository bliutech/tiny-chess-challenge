using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;

public class Zorbist : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 320, 330, 500, 900, 20000 };
    int nodes = 0;
    int nodeLimit = 10_000_000;
    Random rng = new();
    Dictionary<ulong, int> abTable = new();

    public Move Think(Board board, Timer timer)
    {
        nodes = 0;
        nodeLimit = 10_000_000 + board.PlyCount * 1_000_000;
        abTable.Clear();
        int bestScore = -int.MaxValue;

        Move[] moves = GetMoves(board);
        Move bestMove = moves[0];

        foreach (Move move in moves)
        {
            if (timer.MillisecondsRemaining < 1000 || timer.MillisecondsElapsedThisTurn * 20 > timer.MillisecondsRemaining) break;
            board.MakeMove(move);
            int score = 0;
            if (timer.MillisecondsRemaining < 2000)
            {
                score = -AlphaBeta(board, -int.MaxValue, int.MaxValue, 1);
            }
            else
            {
                score = -AlphaBeta(board, -int.MaxValue, int.MaxValue, 3);
            }
            board.UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        Console.WriteLine("Nodes: {0}", nodes);

        return bestMove;
    }

    Move[] GetMoves(Board board)
    {
        return board.GetLegalMoves().OrderBy(x => pieceValues[(int)x.CapturePieceType] + rng.Next(100)).ToArray();
    }

    int Quiesce(Board board, int alpha, int beta)
    {
        nodes++;
        int stand_pat = Evaluate(board);
        // if (nodes > nodeLimit) return stand_pat;
        if (stand_pat >= beta)
            return beta;
        if (alpha < stand_pat)
            alpha = stand_pat;

        foreach (Move move in board.GetLegalMoves(true))
        {
            board.MakeMove(move);
            int score = -Quiesce(board, -beta, -alpha);
            board.UndoMove(move);

            if (score >= beta)
                return beta;
            if (score > alpha)
                alpha = score;
        }
        return alpha;
    }

    int Evaluate(Board board)
    {
        if (board.IsInCheckmate()) return -10000;
        if (board.IsDraw()) return 0;
        int score = 0;
        PieceList[] pieceLists = board.GetAllPieceLists();
        for (int i = 0; i < 5; i++)
        {
            int val = pieceValues[i + 1];
            score += (pieceLists[i].Count - pieceLists[i + 6].Count) * val;
        }
        return score * (board.IsWhiteToMove ? 1 : -1);
    }

    int AlphaBeta(Board board, int alpha, int beta, int depthLeft)
    {
        nodes++;
        if (depthLeft == 0 || nodes > nodeLimit) return Quiesce(board, alpha, beta);
        foreach (Move move in board.GetLegalMoves())
        {
            board.MakeMove(move);
            int score = abTable.ContainsKey(board.ZobristKey)
                ? abTable[board.ZobristKey]
                : -AlphaBeta(board, -beta, -alpha, depthLeft - 1);
            abTable[board.ZobristKey] = score;
            board.UndoMove(move);
            if (score >= beta)
                return beta;
            if (score > alpha)
                alpha = score;
        }
        return alpha;
    }
}
