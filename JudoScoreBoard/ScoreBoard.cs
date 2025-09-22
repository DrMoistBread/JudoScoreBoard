using Blazored.LocalStorage;
using JudoScoreBoard.Components;
using Microsoft.VisualBasic;

namespace JudoScoreBoard;

public class ScoreBoard
{
    private Settings _settings;

    private const string BLUE = "BLUE";
    private const string WHITE = "WHITE";
    private const int HANSOKUMAKE = 3;
    private const string NO_WINNER = "";
    private readonly PlayerScore _whiteJudoka;
    private readonly PlayerScore _blueJudoka;

    public MatchTimer Timer { get; set; }
    public OsaekomiTimer OsaekomiTimer;

    private string _winner = "";

    public ScoreBoard()
    {
        _whiteJudoka = new PlayerScore();
        _blueJudoka = new PlayerScore();
        _settings = new Settings();
        Timer = new MatchTimer(this);
        OsaekomiTimer = new OsaekomiTimer(this);
    }

    public int GetScoreWhite(Score score)
    {
        switch (score)
        {
            case Score.Ippon: return _whiteJudoka.IpponScore;
            case Score.Wazari: return _whiteJudoka.WazariScore;
            case Score.Yuko: return _whiteJudoka.YukoScore;
            case Score.Shido: return _whiteJudoka.ShidoScore;
            default: return 0;
        }
    }

    public int GetScoreBlue(Score score)
    {
        switch (score)
        {
            case Score.Ippon: return _blueJudoka.IpponScore;
            case Score.Wazari: return _blueJudoka.WazariScore;
            case Score.Yuko: return _blueJudoka.YukoScore;
            case Score.Shido: return _blueJudoka.ShidoScore;
            default: return 0;
        }
    }

    public TimeSpan GetMatchTime()
    {
        return Timer._currentCount;
    }

    public TimeSpan SetMatchTime(int deltaSeconds)
    {
        var newtime = Timer._currentCount.Add(TimeSpan.FromSeconds(deltaSeconds));
        if (newtime.TotalSeconds > 0)
        {
            Timer._currentCount = newtime;
        }

        return Timer._currentCount;
    }

    public async Task IncreaseScore(Score score, Player player)
    {
        switch (score, player)
        {
            case (Score.Ippon, Player.White): await _whiteJudoka.IncreaseIpponScore(); break;
            case (Score.Wazari, Player.White): await _whiteJudoka.IncreaseWazariScore(); break;
            case (Score.Yuko, Player.White): await _whiteJudoka.IncreaseYukoScore(); break;
            case (Score.Shido, Player.White):
                await _whiteJudoka.IncreaseShidoScore();
                if (_whiteJudoka.ShidoScore == HANSOKUMAKE)
                {
                    await _blueJudoka.IncreaseIpponScore();
                }

                break;
            case (Score.Ippon, Player.Blue): await _blueJudoka.IncreaseIpponScore(); break;
            case (Score.Wazari, Player.Blue): await _blueJudoka.IncreaseWazariScore(); break;
            case (Score.Yuko, Player.Blue): await _blueJudoka.IncreaseYukoScore(); break;
            case (Score.Shido, Player.Blue):
                await _blueJudoka.IncreaseShidoScore();
                if (_blueJudoka.ShidoScore == HANSOKUMAKE)
                {
                    await _whiteJudoka.IncreaseIpponScore();
                }

                break;
        }

        CheckWinner();
    }

    public bool IsBlueWinner()
    {
        return _winner == BLUE;
    }

    public bool HasWinner()
    {
        return _winner != NO_WINNER;
    }

    public bool IsWhiteWinner()
    {
        return _winner == WHITE;
    }


    public async Task DecreaseScore(Score score, Player player)
    {
        switch (score, player)
        {
            case (Score.Ippon, Player.White): await _whiteJudoka.DecreaseIpponScore(); break;
            case (Score.Wazari, Player.White): await _whiteJudoka.DecreaseWazariScore(); break;
            case (Score.Yuko, Player.White): await _whiteJudoka.DecreaseYukoScore(); break;
            case (Score.Shido, Player.White):
                if (_whiteJudoka.ShidoScore == HANSOKUMAKE)
                {
                    await _blueJudoka.DecreaseIpponScore();
                }
                
                await _whiteJudoka.DecreaseShidoScore();
                break;
            case (Score.Ippon, Player.Blue): await _blueJudoka.DecreaseIpponScore(); break;
            case (Score.Wazari, Player.Blue): await _blueJudoka.DecreaseWazariScore(); break;
            case (Score.Yuko, Player.Blue): await _blueJudoka.DecreaseYukoScore(); break;
            case (Score.Shido, Player.Blue):
                if (_blueJudoka.ShidoScore == HANSOKUMAKE)
                {
                    await _whiteJudoka.DecreaseIpponScore();
                }
                await _blueJudoka.DecreaseShidoScore(); break;
        }

        CheckWinner();
         
    }

    public void CheckWinner()
    {
        if (_blueJudoka.IpponScore > _whiteJudoka.IpponScore)
        {
            _winner = BLUE;
            return;
        }

        if (_whiteJudoka.IpponScore > _blueJudoka.IpponScore)
        {
            _winner = WHITE;
            return;
        }

        if (Timer._currentCount > TimeSpan.Zero && !Timer._isGoldenScore)
        {
            _winner = NO_WINNER;
            return;
        }

        if (_blueJudoka.WazariScore > _whiteJudoka.WazariScore)
        {
            _winner = BLUE;
            return;
        }

        if (_whiteJudoka.WazariScore > _blueJudoka.WazariScore)
        {
            _winner = WHITE;
            return;
        }

        if (_blueJudoka.YukoScore > _whiteJudoka.YukoScore)
        {
            _winner = BLUE;
            return;
        }

        if (_whiteJudoka.YukoScore > _blueJudoka.YukoScore)
        {
            _winner = WHITE;
            return;
        }

        if (Timer._currentCount == TimeSpan.Zero)
        {
            Timer._isGoldenScore = true;
        }

        _winner = NO_WINNER;
    }
    
    
}