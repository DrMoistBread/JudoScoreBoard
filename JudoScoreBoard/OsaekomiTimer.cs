using System.Timers;
using JudoScoreBoard.Components;
using Timer = System.Timers.Timer;

namespace JudoScoreBoard;

public class OsaekomiTimer
{
    public int CurrentCount { get; private set; }
    private Timer _timer;
    public Func<Player?, Task> OnTimeChanged;
    private Player? _currentOsaekomiPlayer;
    private ScoreBoard _scoreBoard;

    public OsaekomiTimer(ScoreBoard scoreBoard)
    {
        _scoreBoard = scoreBoard;

        _timer = new Timer();
        _timer.Interval = 1000;
        _timer.Elapsed += async (object? sender, ElapsedEventArgs e) =>
        {
            CurrentCount++;

            if (_scoreBoard.Timer.IsGoldenScore)
            {
                if (CurrentCount == 5)
                {
                    await StopAsync();
                    return;
                }
            }

            if (CurrentCount == 10 )
            {
                if (_scoreBoard.GetScoreBlue(Score.Wazari) == 1 && _currentOsaekomiPlayer == Player.Blue)
                {
                    await StopAsync();
                    return;
                }

                if (_scoreBoard.GetScoreWhite(Score.Wazari) == 1 && _currentOsaekomiPlayer == Player.White)
                {
                    await StopAsync();
                    return;
                }
                
            }


            if (CurrentCount == 20)
            {
                await StopAsync();
                return;
            }

            await OnTimeChanged(_currentOsaekomiPlayer);
        };
    }

    public void Start()
    {
        if (_scoreBoard.Timer.IsRunning())
        {
            _timer.Enabled = true;
        }
    }

    public async Task StopAsync()
    {
        _timer.Enabled = false;
         await SetScoreOnStop();
    }

    public async Task ToggleAsync(Player player)
    {
        await OnTimeChanged(player);
        if (!_scoreBoard.Timer.IsRunning() && !IsRunning())
        {
            return;
        }
        if (_currentOsaekomiPlayer == player)
        {
            await StopAsync();
            return;
        }

        _currentOsaekomiPlayer = player;

        if (!_timer.Enabled)
        {
            CurrentCount = 0;
            Start();
        }
    }

    private async Task SetScoreOnStop()
    {
        if (_currentOsaekomiPlayer is null)
        {
            return;
        }
        switch (CurrentCount)
        {
            case >= 10 and <20:
                await _scoreBoard.IncreaseScore(Score.Wazari, _currentOsaekomiPlayer.Value);
                break;
            case >= 5 and <10:
                await _scoreBoard.IncreaseScore(Score.Yuko, _currentOsaekomiPlayer.Value);
                break;
            case >= 20:
                await _scoreBoard.IncreaseScore(Score.Ippon, _currentOsaekomiPlayer.Value);
                break;
        }
        
        await _scoreBoard.CheckWinnerAsync();
        
        _currentOsaekomiPlayer = null;
    }

    public bool IsRunning()
    {
        return _timer.Enabled;
    }
}