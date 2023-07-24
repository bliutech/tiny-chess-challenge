using ChessChallenge.API;
using ChessChallenge.Application;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        string bot = "MinMax";

        switch (bot)
        {
            case "Quiesce":
                ChallengeController.tokenCount = ChallengeController.GetTokenCount("AlphaBetaQuiesce.cs");
                AlphaBetaQuiesce abq = new();
                return abq.Think(board, timer);
            case "MinMax":
            default:
                ChallengeController.tokenCount = ChallengeController.GetTokenCount("AlphaBetaMinMax.cs");
                AlphaBetaMinMax abmm = new();
                return abmm.Think(board, timer);
        }
    }
}
