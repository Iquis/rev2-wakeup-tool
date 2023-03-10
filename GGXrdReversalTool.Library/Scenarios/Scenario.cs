using GGXrdReversalTool.Library.Characters;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Event;
using GGXrdReversalTool.Library.Scenarios.Frequency;

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
        
        

        Thread scenarioThread = new Thread(() =>
        {
            LogManager.Instance.WriteLine("Scenario Thread start");
            bool localRunThread = true;

            var oldEventType = AnimationEventTypes.None;


            while (localRunThread)
            {
                var eventType = _scenarioEvent.CheckEvent();
                if (eventType != oldEventType && eventType != AnimationEventTypes.None)
                {
                    LogManager.Instance.WriteLine("Event Occured");
                    
                    //TODO should remove from loop?
                    var currentDummy = _memoryReader.GetCurrentDummy();
                    

                    var timing = GetTiming(eventType, currentDummy, _scenarioAction.Input);

                    Wait(timing);

                    var shouldExecuteAction = _scenarioFrequency.ShouldHappen();

                    if (shouldExecuteAction)
                    {
                        _scenarioAction.Execute();
                        
                        LogManager.Instance.WriteLine("Action Executed");
                    }
                }
                

                oldEventType = eventType;


                lock (RunThreadLock)
                {
                    localRunThread = _runThread;
                }

                Thread.Sleep(1);
            }


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

    
    private int GetTiming(AnimationEventTypes animationEventType, Character currentDummy, SlotInput scenarioActionInput)
    {
        //TODO fix why - 2 ?
        switch (animationEventType)
        {
            case AnimationEventTypes.KDFaceUp:
                return currentDummy.FaceUpFrames - scenarioActionInput.ReversalFrameIndex - 2;
            case AnimationEventTypes.KDFaceDown:
                return currentDummy.FaceDownFrames - scenarioActionInput.ReversalFrameIndex - 2;
            case AnimationEventTypes.WallSplat:
                return currentDummy.WallSplatWakeupTiming - scenarioActionInput.ReversalFrameIndex - 2;
            case AnimationEventTypes.Blocking:
                return 0;
            case AnimationEventTypes.Combo:
                return 0;
            case AnimationEventTypes.Tech:
                //TODO tech reversal recovery = 6?
                return 6 - 2;
            default:
                throw new ArgumentOutOfRangeException(nameof(animationEventType), animationEventType, null);
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
                Thread.Sleep(10);
                frameCount = _memoryReader.FrameCount() - startFrame;
            } while (frameCount < frames);
        }
    }


    public void Dispose()
    {
        Stop();
    }
}