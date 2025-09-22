using System.Timers;
using JudoScoreBoard.Components;
using Timer = System.Timers.Timer;

namespace JudoScoreBoard;

public class OsaekomiTimer
{
    public int _currentCount { get; private set; }
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
            _currentCount++;

            if (_scoreBoard.Timer._isGoldenScore)
            {
                if (_currentCount >= 5)
                {
                    await Stop();
                    if (_currentOsaekomiPlayer == Player.Blue)
                    {
                        await scoreBoard.IncreaseScore(Score.Yuko, Player.Blue);
                    }

                    if (_currentOsaekomiPlayer == Player.White)
                    {
                        await scoreBoard.IncreaseScore(Score.Yuko, Player.White);
                    }
                    
                }
               
            }

            if (_currentCount is >= 10 and < 20)
            {
                if (_scoreBoard.GetScoreBlue(Score.Wazari) == 1 && _currentOsaekomiPlayer == Player.Blue)
                {
                    await _scoreBoard.IncreaseScore(Score.Wazari, Player.Blue);
                    await Stop();
                }

                if (_scoreBoard.GetScoreWhite(Score.Wazari) == 1 && _currentOsaekomiPlayer == Player.White)
                {
                    await _scoreBoard.IncreaseScore(Score.Wazari, Player.White);
                    await Stop();
                }
                
            }


            if (_currentCount >= 20)
            {
                await Stop();
            
                if (_currentOsaekomiPlayer == Player.Blue)
                {
                    await _scoreBoard.IncreaseScore(Score.Ippon,Player.Blue);
                }
                else
                {
                    await _scoreBoard.IncreaseScore(Score.Ippon, Player.White);
                }
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

    public async Task Stop()
    {
        _timer.Enabled = false;
        await SetScoreOnStop();
        // await OnTimeChanged(_currentOsaekomiPlayer);
    }

    public async Task ToggleAsync(Player player)
    {
        if (!_scoreBoard.Timer.IsRunning() && !IsRunning())
        {
            return;
        }
        if (_currentOsaekomiPlayer == player)
        {
            await Stop();
            return;
        }

        _currentOsaekomiPlayer = player;

        if (!_timer.Enabled)
        {
            _currentCount = 0;
            Start();
        }

        // await OnTimeChanged(_currentOsaekomiPlayer);
    }

    private async Task SetScoreOnStop()
    {
        if (_currentOsaekomiPlayer is null)
        {
            return;
        }
        switch (_currentCount)
        {
            case >= 10 and <20:
                await _scoreBoard.IncreaseScore(Score.Wazari, _currentOsaekomiPlayer.Value);
                break;
            case >= 5 and <10:
                await _scoreBoard.IncreaseScore(Score.Yuko, _currentOsaekomiPlayer.Value);
                break;
        }

        _currentOsaekomiPlayer = null;
    }

    public bool IsRunning()
    {
        return _timer.Enabled;
    }
}