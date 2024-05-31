using GGXrdReversalTool.Library.Characters;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Event;
using GGXrdReversalTool.Library.Scenarios.Frequency;
using System.Runtime.InteropServices;

namespace GGXrdReversalTool.Library.Scenarios;

public class Scenario : IDisposable
{
    private readonly IMemoryReader _memoryReader;
    
    private readonly IScenarioEvent _scenarioEvent;
    private readonly IScenarioAction _scenarioAction;
    private readonly IScenarioFrequency _scenarioFrequency;


    private static bool _runThread;
    private static readonly object RunThreadLock = new object();

    public bool IsRunning => _runThread;



    public Scenario(
        IMemoryReader memoryReader,
        IScenarioEvent scenarioEvent, 
        IScenarioAction scenarioAction, 
        IScenarioFrequency scenarioFrequency)
    {
        _memoryReader = memoryReader; 
        _scenarioEvent = scenarioEvent;
        _scenarioAction = scenarioAction;
        _scenarioFrequency = scenarioFrequency;
    }


    private void Init()
    {
        
        //TODO Inject via factory
        _scenarioEvent.MemoryReader = _memoryReader;
        _scenarioAction.MemoryReader = _memoryReader;
        _scenarioFrequency.MemoryReader = _memoryReader;

        _scenarioAction.Init();
    }

    public void Run()
    {
        lock (RunThreadLock)
        {
            _runThread = true;
        }
        
        Init();
        
        

        var scenarioThread = new Thread(() =>
        {
            LogManager.Instance.WriteLine("Scenario Thread start");
            var localRunThread = true;

            var oldEventType = new EventAnimationInfo();
            var engineTicks = _memoryReader.GetEngineTickCount();
            var prevEngineTicks = engineTicks;

            timeBeginPeriod(1);

            while (localRunThread)
            {
                // Approximately synchronise with the game's main loop finishing game state updates
                // In practice this leaves >13ms for our work before the next tick
                engineTicks = _memoryReader.GetEngineTickCount();
                if (engineTicks != prevEngineTicks)
                {
                    var worldInTick = _memoryReader.IsWorldInTick();
                    if (engineTicks - prevEngineTicks > 1 || worldInTick)
                    {
                        LogManager.Instance.WriteLine("Overslept through tick");
                    }
                    // Very unlikely, but skip this tick if somehow we overslept into the middle of a new tick
                    if (!worldInTick)
                    {
                        var eventAnimationInfo = _scenarioEvent.CheckEvent();
                        if (eventAnimationInfo.EventType != oldEventType.EventType && eventAnimationInfo.EventType != AnimationEventTypes.None)
                        {
                            LogManager.Instance.WriteLine("Event Occured");

                            //TODO should remove from loop?
                            var currentDummy = _memoryReader.GetCurrentDummy();

                            var timing = GetTiming(eventAnimationInfo, currentDummy, _scenarioAction.Input);

                            Wait(timing);

                            var shouldExecuteAction = _scenarioFrequency.ShouldHappen();

                            if (shouldExecuteAction)
                            {
                                _scenarioAction.Execute();

                                LogManager.Instance.WriteLine("Action Executed");
                            }
                            engineTicks = _memoryReader.GetEngineTickCount();
                        }

                        oldEventType = eventAnimationInfo;
                    }
                }

                lock (RunThreadLock)
                {
                    localRunThread = _runThread;
                }

                prevEngineTicks = engineTicks;
                Thread.Sleep(1);
            }

            timeEndPeriod(1);

            LogManager.Instance.WriteLine("Scenario Thread ended");
        });

        scenarioThread.Start();

    }

    


    public void Stop()
    {
        lock (RunThreadLock)
        {
            _runThread = false;
        }
    }

    
    private int GetTiming(EventAnimationInfo eventAnimationInfo, Character currentDummy, SlotInput scenarioActionInput)
    {
        switch (eventAnimationInfo.EventType)
        {
            case AnimationEventTypes.KDFaceUp:
                return currentDummy.FaceUpFrames - scenarioActionInput.ReversalFrameIndex - 1;
            case AnimationEventTypes.KDFaceDown:
                return currentDummy.FaceDownFrames - scenarioActionInput.ReversalFrameIndex - 1;
            case AnimationEventTypes.WallSplat:
                return currentDummy.WallSplatWakeupTiming - scenarioActionInput.ReversalFrameIndex - 1;
            case AnimationEventTypes.StartBlocking:
                return 0;
            case AnimationEventTypes.EndBlocking:
                return eventAnimationInfo.Delay - scenarioActionInput.ReversalFrameIndex - 2;
            case AnimationEventTypes.Combo:
                return 0;
            case AnimationEventTypes.Tech:
                // don't ask me why this is correct
                return 8 - scenarioActionInput.ReversalFrameIndex;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventAnimationInfo.EventType), eventAnimationInfo, null);
        }
    }
    private void Wait(int frames)
    {
        if (frames > 0)
        {
            int startFrame = _memoryReader.FrameCount();
            int frameCount;

            do
            {
                Thread.Sleep(1);
                frameCount = _memoryReader.FrameCount() - startFrame;
            } while (frameCount < frames);
        }
    }


    public void Dispose()
    {
        Stop();
    }

    #region DLL Imports
    [DllImport("winmm.dll")]
    private static extern int timeBeginPeriod(uint uPeriod);

    [DllImport("winmm.dll")]
    private static extern int timeEndPeriod(uint uPeriod);

    #endregion
}
