using System.Timers;

namespace BossQuest.Services;

public class TimerService : IDisposable
{
    private System.Timers.Timer? _timer;
    private int _focusDuration;

    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsBreak { get; private set; }
    public int RemainingSeconds { get; private set; }
    public int TotalSeconds { get; private set; }
    public string CurrentSubject { get; private set; } = "";
    public string SessionType => IsBreak ? "Break" : (IsRunning || IsPaused ? "Focus" : "Ready");
    public string TimeDisplay => $"{RemainingSeconds / 60:D2}:{RemainingSeconds % 60:D2}";
    public double ProgressPercent => TotalSeconds > 0 ? Math.Clamp((double)(TotalSeconds - RemainingSeconds) / TotalSeconds * 100, 0, 100) : 0;

    public event Action? OnTick;
    public event Action? OnFocusCompleted;
    public event Action? OnBreakCompleted;

    public void Start(int minutes, string subject)
    {
        _focusDuration = minutes;
        CurrentSubject = string.IsNullOrWhiteSpace(subject) ? "Study Session" : subject;
        TotalSeconds = minutes * 60;
        RemainingSeconds = TotalSeconds;
        IsRunning = true;
        IsPaused = false;
        IsBreak = false;

        _timer?.Dispose();
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        _timer.Start();
        OnTick?.Invoke();
    }

    public void Pause()
    {
        if (!IsRunning || IsPaused) return;
        IsPaused = true;
        _timer?.Stop();
        OnTick?.Invoke();
    }

    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;
        _timer?.Start();
        OnTick?.Invoke();
    }

    public void Stop()
    {
        IsRunning = false;
        IsPaused = false;
        IsBreak = false;
        RemainingSeconds = 0;
        TotalSeconds = 0;
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        OnTick?.Invoke();
    }

    public void Reset()
    {
        Stop();
        RemainingSeconds = 0;
        TotalSeconds = 0;
        CurrentSubject = "";
        OnTick?.Invoke();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (RemainingSeconds > 0)
        {
            RemainingSeconds--;
            OnTick?.Invoke();
        }

        if (RemainingSeconds <= 0)
        {
            _timer?.Stop();

            if (!IsBreak)
            {
                // Focus session completed! Fire critical strike event
                OnFocusCompleted?.Invoke();

                // Switch to break mode
                int breakMinutes = _focusDuration >= 50 ? 10 : 5;
                IsBreak = true;
                TotalSeconds = breakMinutes * 60;
                RemainingSeconds = TotalSeconds;
                _timer?.Start();
            }
            else
            {
                // Break ended
                OnBreakCompleted?.Invoke();
                IsRunning = false;
                IsBreak = false;
                RemainingSeconds = 0;
                TotalSeconds = 0;
            }
            OnTick?.Invoke();
        }
    }

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
    }
}
