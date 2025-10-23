using Timer = System.Timers.Timer;

namespace JudoScoreBoard;

public class MatchTimer
{
    private TimeSpan _currentCount;

    public TimeSpan GetTimerCount() => _currentCount;

    public TimeSpan SetTimer(int seconds)
    {
        var newtime = _currentCount.Add(TimeSpan.FromSeconds(seconds));
        if (newtime.TotalSeconds > 0)
        {
            _currentCount = newtime;
        }

        return _currentCount;
    }

    public MatchTimer(ScoreBoard scoreBoard)
    {
        _scoreBoard = scoreBoard;
    }

    public Func<Task> OnTimeChanged;

    public bool IsGoldenScore { get; set; }

    private Timer _timer;
    private ScoreBoard _scoreBoard;

    public void StartGoldenScore()
    {
        IsGoldenScore = true;
    }

    public void InitializeTimer(TimeSpan timer)
    {
        _currentCount = timer;
        _timer = new Timer();

        _timer.Interval = 1000;
        _timer.Elapsed += async (_, _) =>
        {
            _currentCount -= TimeSpan.FromSeconds(1);

            if (_currentCount is { Minutes: 0, Seconds: 0 })
            {
                _timer.Stop();
                if (!_scoreBoard.OsaekomiTimer.IsRunning())
                {
                    await _scoreBoard.CheckWinnerAsync();
                }
            }

            await OnTimeChanged();
        };
    }

    public async Task StartTimerAsync()
    {
        if (!_scoreBoard.HasWinner())
        {
            _timer.Start();
        }
    }

    public async Task StopTimer()
    {
        _timer.Stop();
        if (_scoreBoard.OsaekomiTimer.IsRunning())
        {
            await _scoreBoard.OsaekomiTimer.StopAsync();
        }
    }

    public async Task<bool> ToggleTimer()
    {
        if (!_scoreBoard.HasWinner())
        {
            _timer.Enabled = !_timer.Enabled;
            if (!_timer.Enabled)
            {
                await _scoreBoard.OsaekomiTimer.StopAsync();
            }

            return _timer.Enabled;
        }

        return false;
    }

    public bool IsRunning() => _timer.Enabled;
}