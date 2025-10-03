using Blazored.LocalStorage;
using JudoScoreBoard.Components;
using Microsoft.VisualBasic;

namespace JudoScoreBoard;

public class ScoreBoard
{
    private Settings _settings;

    private const int Hansokumake = 3;
    private readonly PlayerScore _whiteJudoka;
    private readonly PlayerScore _blueJudoka;
    public Func<Task> OnTimeChanged;


    public MatchTimer Timer { get; set; }
    public OsaekomiTimer OsaekomiTimer;

    private Player? _winner = null;

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
        return Timer.GetTimerCount();
    }

    public TimeSpan SetMatchTime(int deltaSeconds)
    {
        return Timer.SetTimer(deltaSeconds);
    }

    public async Task IncreaseScore(Score score, Player player)
    {
        switch (score, player)
        {
            case (Score.Ippon, Player.White): _whiteJudoka.IncreaseIpponScore(); break;
            case (Score.Wazari, Player.White): _whiteJudoka.IncreaseWazariScore(); break;
            case (Score.Yuko, Player.White): _whiteJudoka.IncreaseYukoScore(); break;
            case (Score.Shido, Player.White):
                _whiteJudoka.IncreaseShidoScore();
                if (_whiteJudoka.ShidoScore == Hansokumake)
                {
                    _blueJudoka.IncreaseIpponScore();
                }

                break;
            case (Score.Ippon, Player.Blue): _blueJudoka.IncreaseIpponScore(); break;
            case (Score.Wazari, Player.Blue): _blueJudoka.IncreaseWazariScore(); break;
            case (Score.Yuko, Player.Blue): _blueJudoka.IncreaseYukoScore(); break;
            case (Score.Shido, Player.Blue):
                _blueJudoka.IncreaseShidoScore();
                if (_blueJudoka.ShidoScore == Hansokumake)
                {
                    _whiteJudoka.IncreaseIpponScore();
                }

                break;
        }

        await CheckWinnerAsync();
    }

    public bool IsBlueWinner()
    {
        return _winner == Player.Blue;
    }

    public bool HasWinner()
    {
        return _winner is not null;
    }

    public bool IsWhiteWinner()
    {
        return _winner == Player.White;
    }


    public async Task DecreaseScore(Score score, Player player)
    {
        switch (score, player)
        {
            case (Score.Ippon, Player.White): _whiteJudoka.DecreaseIpponScore(); break;
            case (Score.Wazari, Player.White): _whiteJudoka.DecreaseWazariScore(); break;
            case (Score.Yuko, Player.White): _whiteJudoka.DecreaseYukoScore(); break;
            case (Score.Shido, Player.White):
                if (_whiteJudoka.ShidoScore == Hansokumake)
                {
                    _blueJudoka.DecreaseIpponScore();
                }

                _whiteJudoka.DecreaseShidoScore();
                break;
            case (Score.Ippon, Player.Blue): _blueJudoka.DecreaseIpponScore(); break;
            case (Score.Wazari, Player.Blue): _blueJudoka.DecreaseWazariScore(); break;
            case (Score.Yuko, Player.Blue): _blueJudoka.DecreaseYukoScore(); break;
            case (Score.Shido, Player.Blue):
                if (_blueJudoka.ShidoScore == Hansokumake)
                {
                    _whiteJudoka.DecreaseIpponScore();
                }

                _blueJudoka.DecreaseShidoScore();
                break;
        }

        await CheckWinnerAsync();
    }

    public async Task CheckWinnerAsync()
    {
        if (_blueJudoka.IpponScore > _whiteJudoka.IpponScore)
        {
            _winner = Player.Blue;
            await OnTimeChanged();
            return;
        }

        if (_whiteJudoka.IpponScore > _blueJudoka.IpponScore)
        {
            _winner = Player.White;
            await OnTimeChanged();
            return;
        }

        if (Timer.GetTimerCount() > TimeSpan.Zero && !Timer.IsGoldenScore)
        {
            _winner = null;
            await OnTimeChanged();
            return;
        }

        if (_blueJudoka.WazariScore > _whiteJudoka.WazariScore)
        {
            _winner = Player.Blue;
            await OnTimeChanged();
            return;
        }

        if (_whiteJudoka.WazariScore > _blueJudoka.WazariScore)
        {
            _winner = Player.White;
            await OnTimeChanged();
            return;
        }

        if (_blueJudoka.YukoScore > _whiteJudoka.YukoScore)
        {
            _winner = Player.Blue;
            await OnTimeChanged();
            return;
        }

        if (_whiteJudoka.YukoScore > _blueJudoka.YukoScore)
        {
            _winner = Player.White;
            await OnTimeChanged();
            return;
        }

        if (Timer.GetTimerCount() == TimeSpan.Zero)
        {
            Timer.IsGoldenScore = true;
        }

        _winner = null;
        await OnTimeChanged();
    }
}