using ChessChallenge.API;
using ChessChallenge.Application;

public class MyBot : IChessBot
{
    private IChessBot bot;

    public MyBot()
    {
        string bott = "Zorb";
        string file = "";

        switch (bott)
        {
            case "Quiesce":
                file = "AlphaBetaQuiesce.cs";
                bot = new AlphaBetaQuiesce();
                break;
            case "Zorb":
                file = "Zorbist.cs";
                bot = new Zorbist();
                break;
            default:
                file = "AlphaBetaMinMax.cs";
                bot = new AlphaBetaMinMax();
                break;
        }
        ChallengeController.tokenCount = ChallengeController.GetTokenCount(file);
    }

    public Move Think(Board board, Timer timer)
    {
        return bot.Think(board, timer);
    }
}
