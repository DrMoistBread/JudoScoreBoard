using Timer = System.Timers.Timer;

namespace JudoScoreBoard;

public class MatchTimer
{
    public TimeSpan _currentCount;

    public MatchTimer( ScoreBoard scoreBoard)
    {
        _scoreBoard = scoreBoard;
    }

    public Func<Task> OnTimeChanged;

    public bool _isGoldenScore = false;

    private Timer _timer;
    private ScoreBoard _scoreBoard;

    public void StartGoldenScore()
    {
        _isGoldenScore = true;
        
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
                    _scoreBoard.CheckWinner();
                }

            }

            await OnTimeChanged();
        };
    }

    public async Task StartTimer()
    {
        
        if (!_scoreBoard.HasWinner())
        {
            _timer.Start();
        }
    }

    public async Task StopTimer()
    {
        _timer.Stop();
        await _scoreBoard.OsaekomiTimer.Stop();
    }

    public async Task<bool> ToggleTimer()
    {
        if (!_scoreBoard.HasWinner())
        {
            _timer.Enabled = !_timer.Enabled;
            if (!_timer.Enabled)
            {
                await _scoreBoard.OsaekomiTimer.Stop();
            }
            return _timer.Enabled;
            
        }

        return false;
    }

    public bool IsRunning() => _timer.Enabled;
}