using Timer = System.Timers.Timer;

namespace JudoScoreBoard;

public class MatchTimer
{
    public TimeSpan _currentCount;
    
    public event Action OnTimeChanged;
    
    public bool _isGoldenScore = false;
    
    private Timer _timer;

    public void StartGoldenScore()
    {
        _isGoldenScore = true;
        
    }
    public void InitializeTimer(TimeSpan timer)
    {
        _currentCount = timer;
        _timer = new Timer();

        _timer.Interval = 1000;
        _timer.Elapsed += ( _, _) =>
        {
            _currentCount -= TimeSpan.FromSeconds(1);

            if (_currentCount is { Minutes: 0, Seconds: 0 })
            {
                _timer.Stop();
                _isGoldenScore = true;
                
                // await JsRuntime.InvokeAsync<string>("PlayAudio", "sound");
                // if (_isOsaekomiEnabled)
                // {
                //     _timer.Enabled = false;
                // }
                // else
                // {
                //     ToggleTime();
                //     await CheckWinner();
                // }
            }
            OnTimeChanged.Invoke();
        };
    }

    public void StartTimer()
    {
        _timer.Start(); 
    }

    public void StopTimer()
    {
        _timer.Stop();
    }

    public bool ToggleTimer()
    {
        _timer.Enabled = !_timer.Enabled;
        return _timer.Enabled;
    }

    public bool IsTimerRunning() => _timer.Enabled;
}