using Blazored.LocalStorage;
using JudoScoreBoard.Components;
using Timer = System.Timers.Timer;

namespace JudoScoreBoard;

public class ScoreBoard
{
    private Settings _settings;

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
        Timer = new MatchTimer();
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
            case (Score.Ippon, Player.White): 
                await _whiteJudoka.IncreaseIpponScore(); 
                break;
            case (Score.Wazari, Player.White): await _whiteJudoka.IncreaseWazariScore(); break;
            case (Score.Yuko, Player.White): await _whiteJudoka.IncreaseYukoScore(); break;
            case (Score.Ippon, Player.Blue): await _blueJudoka.IncreaseIpponScore(); break;
            case (Score.Wazari, Player.Blue): await _blueJudoka.IncreaseWazariScore(); break;
            case (Score.Yuko, Player.Blue): await _blueJudoka.IncreaseYukoScore(); break;
        }

        await CheckWinner();
    }
    
    public async Task DecreaseScore(Score score, Player player)
    {
        switch (score, player)
        {
            case (Score.Ippon, Player.White): await _whiteJudoka.DecreaseIpponScore(); break;
            case (Score.Wazari, Player.White): await _whiteJudoka.DecreaseWazariScore(); break;
            case (Score.Yuko, Player.White): await _whiteJudoka.DecreaseYukoScore(); break;
            case (Score.Ippon, Player.Blue): await _blueJudoka.DecreaseIpponScore(); break;
            case (Score.Wazari, Player.Blue): await _blueJudoka.DecreaseWazariScore(); break;
            case (Score.Yuko, Player.Blue): await _blueJudoka.DecreaseYukoScore(); break;
        }

        await CheckWinner();
    }
    
    private async Task CheckWinner()
    {
        if (_blueJudoka.IpponScore > _whiteJudoka.IpponScore)
        {
            _winner = "BLUE";
    
            Timer.StopTimer();
            // await JsRuntime.InvokeAsync<string>("PlayAudio", "sound");
            // await TriggerBlueFlash();
            return;
        }
    
        if (_whiteJudoka.IpponScore > _blueJudoka.IpponScore)
        {
            _winner = "WHITE";
    
             Timer.StopTimer();
            // await JsRuntime.InvokeAsync<string>("PlayAudio", "sound");
            // await TriggerWhiteFlash();
            return;
        }
    
        if (Timer._currentCount > TimeSpan.Zero && !Timer._isGoldenScore)
        {
            _winner = "";
            return;
        }
    
        if (_blueJudoka.WazariScore> _whiteJudoka.WazariScore)
        {
            _winner = "BLUE";
            Timer.StopTimer();
            // await JsRuntime.InvokeAsync<string>("PlayAudio", "sound");
            // await TriggerBlueFlash();
            return;
        }
    
        if (_whiteJudoka.WazariScore > _blueJudoka.WazariScore)
        {
            _winner = "WHITE";
             Timer.StopTimer();
            // await JsRuntime.InvokeAsync<string>("PlayAudio", "sound");
            // await TriggerWhiteFlash();
    
            return;
        }
    
        if (_blueJudoka.YukoScore > _whiteJudoka.YukoScore)
        {
            _winner = "BLUE";
             Timer.StopTimer();
            // await JsRuntime.InvokeAsync<string>("PlayAudio", "sound");
            // await TriggerBlueFlash();
    
            return;
        }
    
        if (_whiteJudoka.YukoScore > _blueJudoka.YukoScore)
        {
            _winner = "WHITE";
            Timer.StopTimer();
            // await JsRuntime.InvokeAsync<string>("PlayAudio", "sound");
            // await TriggerWhiteFlash();
            return;
        }
    
        _winner = "";
    }
}

public class OsaekomiTimer
{
    private TimeSpan _currentCount;
    private Timer _timer;
}