using GGXrdWakeupDPUtil.Library.Enums;
using GGXrdWakeupDPUtil.Library.Memory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using GGXrdWakeupDPUtil.Library.Replay;
using GGXrdWakeupDPUtil.Library.Replay.AsmInjection;
using GGXrdWakeupDPUtil.Library.Replay.Keyboard;

namespace GGXrdWakeupDPUtil.Library
{
    public class ReversalTool : IDisposable
    {
        private readonly string _ggprocname = ConfigurationManager.AppSettings.Get("GGProcessName");


        private readonly List<NameWakeupData> _nameWakeupDataList = new List<NameWakeupData>
        {
            new NameWakeupData("Sol", 25, 21),
            new NameWakeupData("Ky", 23, 21),
            new NameWakeupData("May", 25, 22),
            new NameWakeupData("Millia", 25, 23),
            new NameWakeupData("Zato", 25, 22),
            new NameWakeupData("Potemkin", 24, 22),
            new NameWakeupData("Chipp", 30, 24),
            new NameWakeupData("Faust", 25, 29),
            new NameWakeupData("Axl", 25, 21),
            new NameWakeupData("Venom", 21, 26),
            new NameWakeupData("Slayer", 26, 20),
            new NameWakeupData("I-No", 24, 20),
            new NameWakeupData("Bedman", 24, 30),
            new NameWakeupData("Ramlethal", 25, 23),
            new NameWakeupData("Sin", 30, 21),
            new NameWakeupData("Elphelt", 27, 27),
            new NameWakeupData("Leo", 28, 26),
            new NameWakeupData("Johnny", 25, 24),
            new NameWakeupData("Jack-O'", 25, 23),
            new NameWakeupData("Jam", 26, 25),
            new NameWakeupData("Haehyun", 22, 27),
            new NameWakeupData("Raven", 25, 24),
            new NameWakeupData("Dizzy", 25, 24),
            new NameWakeupData("Baiken", 25, 21),
            new NameWakeupData("Answer", 25, 25)
        };

        private const int WallSplatWakeupTiming = 15;


        #region Offsets
        private readonly IntPtr _p2IdOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2IdOffset"), 16));
        private readonly IntPtr _recordingSlotPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("RecordingSlotPtr"), 16));
        private readonly int _recordingSlotPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RecordingSlotPtrOffset"), 16);
        private readonly IntPtr _p1AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1AnimStringPtr"), 16));
        private readonly int _p1AnimStringPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1AnimStringPtrOffset"), 16);
        private readonly IntPtr _p2AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2AnimStringPtr"), 16));
        private readonly int _p2AnimStringPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2AnimStringPtrOffset"), 16);
        private readonly int _frameCountOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("FrameCountOffset"), 16);
        private readonly IntPtr _p1ComboCountPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1ComboCountPtr"), 16));
        private readonly int _p1ComboCountPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1ComboCountPtrOffset"), 16);
        private readonly IntPtr _p2ComboCountPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2ComboCountPtr"), 16));
        private readonly int _p2ComboCountPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2ComboCountPtrOffset"), 16);
        private readonly IntPtr _dummyIdPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("DummyIdPtr"), 16));
        private readonly int _dummyIdPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("DummyIdPtrOffset"), 16);
        private readonly int _replayKeyOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ReplayKeyOffset"), 16);

        private readonly IntPtr _blockStunPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("BlockStunPtr"), 16));
        private readonly int _blockStunOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BlockStunPtrOffset"), 16);
        
        #endregion

        private readonly string FaceDownAnimation = "CmnActFDown2Stand";
        private readonly string FaceUpAnimation = "CmnActBDown2Stand";
        private readonly string WallSplatAnimation = "CmnActWallHaritsukiGetUp";

        private const int RecordingSlotSize = 4808;

        private Process _process;

        private MemoryReader _memoryReader;
        private ReplayTrigger _replayTrigger;
        
        public ReplayTriggerTypes ReplayTriggerType
        {
            get
            {
                if (!Enum.TryParse(ConfigurationManager.AppSettings.Get("ReplayTriggerType"), false, out ReplayTriggerTypes replayTriggerType))
                {
                    replayTriggerType = ReplayTriggerTypes.AsmInjection;
                }

                return replayTriggerType;
            }
            private set => ConfigManager.Set("ReplayTriggerType", value.ToString());
        }

        #region Reversal Loop
        private static bool _runReversalThread;
        private static readonly object RunReversalThreadLock = new object();
        public delegate void ReversalLoopErrorHandler(Exception ex);

        public event ReversalLoopErrorHandler ReversalLoopErrorOccured;
        #endregion

        #region Dummy Loop
        private static bool _runDummyThread;
        private static readonly object RunDummyThreadLock = new object();

        public delegate void DummyChangedHandler(NameWakeupData dummy);

        public event DummyChangedHandler DummyChanged;


        public delegate void DummyLoopErrorHandler(Exception ex);

        public event DummyLoopErrorHandler DummyLoopErrorOccured;
        #endregion

        #region Random Burst Loop
        private static bool _runRandomBurstThread;
        private static readonly object RunRandomBurstThreadLock = new object();
        public delegate void RandomBurstLoopErrorHandler(Exception ex);
        public event RandomBurstLoopErrorHandler RandomBurstlLoopErrorOccured;
        #endregion

        #region Reversal Loop
        private static bool _runBlockReversalThread;
        private static readonly object RunBlockReversalThreadLock = new object();
        public delegate void BlockReversalLoopErrorHandler(Exception ex);

        public event BlockReversalLoopErrorHandler BlockReversalLoopErrorOccured;
        #endregion


        #region Dll Imports
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion


        #region Replay Trigger
        private ReplayTrigger GetReplayTrigger()
        {
            ReplayTriggerTypes replayTriggerType = this.ReplayTriggerType;

            ReplayTriggerFactory factory;
            
            if (replayTriggerType == ReplayTriggerTypes.Keystroke)
            {
                Keyboard.DirectXKeyStrokes keyStroke = this.GetReplayKeyStroke();
                factory = new KeyboardTriggerFactory(keyStroke);
            }
            else
            {
                factory = new AsmInjectionTriggerFactory(this._process, this._memoryReader);
            }

            return factory.GetReplayTrigger();
        }

        public void ChangeReplayTrigger(ReplayTriggerTypes replayTriggerType)
        {
            if (replayTriggerType != this.ReplayTriggerType)
            {
                this.ReplayTriggerType = replayTriggerType;

                this._replayTrigger?.Dispose();

                this._replayTrigger = GetReplayTrigger();

                this._replayTrigger.InitTrigger();
            }
        }
        #endregion

        public void AttachToProcess()
        {
            var process = Process.GetProcessesByName(_ggprocname).FirstOrDefault();

            this._process = process ?? throw new Exception("GG process not found!");

            this._memoryReader = new MemoryReader(process);


            this._replayTrigger = GetReplayTrigger();

            this._replayTrigger.InitTrigger();

            StartDummyLoop();
        }



        public void BringWindowToFront()
        {
            IntPtr handle = GetForegroundWindow();
            if (this._process != null && this._process.MainWindowHandle != handle)
            {
                SetForegroundWindow(this._process.MainWindowHandle);
            }
        }

        public NameWakeupData GetDummy()
        {
            var index = this._memoryReader.ReadWithOffsets<int>(_dummyIdPtr, _dummyIdPtrOffset);

            var result = _nameWakeupDataList[index];

            return result;

        }

        public bool SetInputInSlot(int slotNumber, SlotInput slotInput)
        {
            var baseAddress = this._memoryReader.GetAddressWithOffsets(_recordingSlotPtr, _recordingSlotPtrOffset);
            var slotAddress = IntPtr.Add(baseAddress, RecordingSlotSize * (slotNumber - 1));

            return this._memoryReader.Write(slotAddress, slotInput.Content);
        }
        public byte[] ReadInputInSlot(int slotNumber)
        {
            var baseAddress = this._memoryReader.GetAddressWithOffsets(_recordingSlotPtr, _recordingSlotPtrOffset);
            var slotAddress = IntPtr.Add(baseAddress, RecordingSlotSize * (slotNumber - 1));

            var readBytes = this._memoryReader.ReadBytes(slotAddress, RecordingSlotSize);

            var inputLength = Byte.MaxValue * readBytes[5] + readBytes[4];

            var headerLength = 4;

            var length = 2 * (inputLength + headerLength);


            byte[] result = new byte[length];
            Array.Copy(readBytes, result, 2 * (inputLength + headerLength));

            return result;
        }

        public bool WriteInputFile(string filePath, byte[] input)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(input.Select(x =>
                        {
                            return x.ToString("X");
                        })
                        .Aggregate((a, b) => { return $"{a},{b}"; })
                    );
                }
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e);

                return false;
            }

            return true;
        }
        public byte[] ReadInputFile(string filePath)
        {
            try
            {
                string text;
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    text = streamReader.ReadToEnd().Trim();
                }

                var result = text.Split(',').Select(x =>
                {
                    return Convert.ToByte(x, 16);
                }).ToArray();
                return result;
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e);
                return new byte[0];
            }

        }

        public string TranslateFromFile(string filePath)
        {
            try
            {
                byte[] inputs = this.ReadInputFile(filePath);

                List<string> values = new List<string>();

                bool isP2 = inputs[0] == 1;

                for (int i = 9; i < inputs.Length; i += 2)
                {
                    ushort item = (ushort)((byte.MaxValue + 1) * inputs[i] + inputs[i - 1]);

                    string value = this.SingleInputParse(item, isP2);

                    values.Add(value);

                }

                return values.Aggregate((a, b) => $"{a},{b}");

            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e);
                return string.Empty;
            }
        }

        public bool TranslateIntoFile(string filePath, string input)
        {
            try
            {
                var slotInput = new SlotInput(input);

                return this.WriteInputFile(filePath, slotInput.CleanInputs);
            }
            catch (Exception e)
            {
                LogManager.Instance.WriteException(e);
                return false;
            }
        }

        public void PlayReversal()
        {
            LogManager.Instance.WriteLine("Reversal!");

            BringWindowToFront();

            this._replayTrigger.TriggerReplay();

            LogManager.Instance.WriteLine("Reversal Done!");
        }

        public void StartWakeupReversalLoop(SlotInput slotInput, int wakeupReversalPercentage, bool playReversalOnWallSplat)
        {
            lock (RunReversalThreadLock)
            {
                _runReversalThread = true;
            }

            Thread reversalThread = new Thread(() =>
            {
                LogManager.Instance.WriteLine("Reversal Thread start");
                var currentDummy = GetDummy();
                bool localRunReversalThread = true;

                Random rnd = new Random();

                bool willReversal = rnd.Next(0, 101) <= wakeupReversalPercentage;

                while (localRunReversalThread && !this._process.HasExited)
                {
                    try
                    {
                        int wakeupTiming = GetWakeupTiming(currentDummy, playReversalOnWallSplat);


                        if (wakeupTiming != 0)
                        {
                            int fc = FrameCount();
                            var frames = wakeupTiming - slotInput.ReversalFrameIndex - 1;
                            while (FrameCount() < fc + frames)
                            {
                            }

                            if (willReversal)
                            {
                                PlayReversal();
                            }
                            willReversal = rnd.Next(0, 101) <= wakeupReversalPercentage;


                            Thread.Sleep(1); // ~1 frame
                            //Thread.Sleep(320); //20 frames, approximately, it's actually 333.333333333 ms.  Nobody should be able to be knocked down and get up in this time, causing the code to execute again.
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.WriteException(ex);
                        StopReversalLoop();
                        ReversalLoopErrorOccured?.Invoke(ex);
                        return;
                    }

                    lock (RunReversalThreadLock)
                    {
                        localRunReversalThread = _runReversalThread;
                    }

                    Thread.Sleep(1);
                }


                LogManager.Instance.WriteLine("Reversal Thread ended");

            })
            { Name = "reversalThread" };

            reversalThread.Start();

            this.BringWindowToFront();
        }

        public void StopReversalLoop()
        {
            lock (RunReversalThreadLock)
            {
                _runReversalThread = false;
            }
        }

        public void StartRandomBurstLoop(int min, int max, int replaySlot, int burstPercentage)
        {
            lock (RunRandomBurstThreadLock)
            {
                _runRandomBurstThread = true;
            }

            Thread randomBurstThread = new Thread(() =>
            {
                LogManager.Instance.WriteLine("RandomBurst Thread start");
                bool localRunRandomBurstThread = true;

                var slotInput = new SlotInput("!5HD");
                SetInputInSlot(replaySlot, slotInput);

                Random rnd = new Random();

                int valueToBurst = rnd.Next(min, max + 1);
                bool willBurst = rnd.Next(0, 101) <= burstPercentage;


                while (localRunRandomBurstThread && !this._process.HasExited)
                {
                    try
                    {
                        int currentCombo = GetCurrentComboCount(1);



                        while (currentCombo > 0)
                        {
                            if (currentCombo == valueToBurst && willBurst)
                            {
                                PlayReversal();
                                Thread.Sleep(850); //50 frames, approximately, Burst recovery is around 50f. 
                            }

                            currentCombo = GetCurrentComboCount(1);

                            if (currentCombo == 0)
                            {
                                valueToBurst = rnd.Next(min, max + 1);
                                willBurst = rnd.Next(0, 101) <= burstPercentage;
                            }
                            Thread.Sleep(1);
                        }


                        lock (RunRandomBurstThreadLock)
                        {
                            localRunRandomBurstThread = _runRandomBurstThread;
                        }
                        Thread.Sleep(1);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.WriteException(ex);
                        StopRandomBurstLoop();
                        RandomBurstlLoopErrorOccured?.Invoke(ex);
                        return;
                    }

                }

                LogManager.Instance.WriteLine("RandomBurst Thread ended");

            })
            {
                Name = "randomBurstThread"
            };

            randomBurstThread.Start();

            this.BringWindowToFront();
        }

        public void StopRandomBurstLoop()
        {
            lock (RunRandomBurstThreadLock)
            {
                _runRandomBurstThread = false;
            }
        }


        public void StartBlockReversalLoop(SlotInput slotInput, int blockstunReversalPercentage, int blockstunReversalDelay)
        {
            lock (RunBlockReversalThreadLock)
            {
                _runBlockReversalThread = true;
            }

            Thread blockReversalThread = new Thread(() =>
                {
                    LogManager.Instance.WriteLine("Block Reversal Thread start");

                    bool localRunBlockReversalThread = true;

                    Random rnd = new Random();

                    bool willReversal = rnd.Next(0, 101) <= blockstunReversalPercentage;

                    int oldBlockstun = 0;

                    while (localRunBlockReversalThread && !this._process.HasExited)
                    {
                        try
                        {
                            int blockStun = this.GetBlockstun(2);

                            if (slotInput.ReversalFrameIndex + 2 == blockStun && oldBlockstun != blockStun)
                            {
                                if (willReversal)
                                {
                                    this.Wait(blockstunReversalDelay);

                                    this.PlayReversal();
                                }

                                willReversal = rnd.Next(0, 101) <= blockstunReversalPercentage;

                                Thread.Sleep(32);
                            }

                            oldBlockstun = blockStun;

                            Thread.Sleep(10); //check about twice by frame
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.WriteException(ex);
                            StopBlockReversalLoop();
                            BlockReversalLoopErrorOccured?.Invoke(ex);
                            return;
                        }

                        lock (RunBlockReversalThreadLock)
                        {
                            localRunBlockReversalThread = _runBlockReversalThread;
                        }

                        Thread.Sleep(1);
                    }


                    LogManager.Instance.WriteLine("Block Reversal Thread ended");

                })
            { Name = "blockReversalThread" };

            blockReversalThread.Start();

            this.BringWindowToFront();
        }

        public void StopBlockReversalLoop()
        {
            lock (RunBlockReversalThreadLock)
            {
                _runBlockReversalThread = false;
            }
        }


        #region Private

        private string SingleInputParse(ushort input, bool isP2 = false)
        {

            string result = string.Empty;


            //direction
            if (IsDirectionPressed(input, Directions.Dir2) && IsDirectionPressed(input, Directions.Dir4))
            {
                result += !isP2 ? "1" : "3";
            }
            else if (IsDirectionPressed(input, Directions.Dir2) && IsDirectionPressed(input, Directions.Dir6))
            {
                result += !isP2 ? "3" : "1";
            }
            else if (IsDirectionPressed(input, Directions.Dir4) && IsDirectionPressed(input, Directions.Dir8))
            {
                result += !isP2 ? "7" : "9";
            }
            else if (IsDirectionPressed(input, Directions.Dir8) && IsDirectionPressed(input, Directions.Dir6))
            {
                result += !isP2 ? "9" : "7";
            }
            else if (IsDirectionPressed(input, Directions.Dir2))
            {
                result += "2";
            }
            else if (IsDirectionPressed(input, Directions.Dir6))
            {
                result += !isP2 ? "6" : "4";
            }
            else if (IsDirectionPressed(input, Directions.Dir4))
            {
                result += !isP2 ? "4" : "6";
            }
            else if (IsDirectionPressed(input, Directions.Dir8))
            {
                result += "8";
            }
            else
            {
                result += "5";
            }


            //button
            foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
            {
                if (IsButtonPressed(input, button))
                {
                    result += button.ToString();
                }
            }

            return result;

        }

        private bool IsButtonPressed(ushort input, Buttons button)
        {
            return (input & (int)button) == (int)button;
        }
        private bool IsDirectionPressed(ushort input, Directions direction)
        {
            return (input & (int)direction) == (int)direction;
        }

        private string ReadAnimationString(int player)
        {
            if (player == 1)
            {
                var length = 32;
                return this._memoryReader.ReadStringWithOffsets(_p1AnimStringPtr, length, _p1AnimStringPtrOffset);
            }

            if (player == 2)
            {
                var length = 32;
                
                return this._memoryReader.ReadStringWithOffsets(_p2AnimStringPtr, length, _p2AnimStringPtrOffset);
            }

            return string.Empty;
        }

        private int GetBlockstun(int player)
        {
            if (player == 2)
            {
                return this._memoryReader.ReadWithOffsets<int>(this._blockStunPtr, this._blockStunOffset);
            }

            throw new NotImplementedException();
        }

        private int FrameCount()
        {
            var address = IntPtr.Add(this._process.MainModule.BaseAddress, _frameCountOffset);
            return this._memoryReader.Read<int>(address);
        }
        private int GetWakeupTiming(NameWakeupData currentDummy, bool playReversalOnWallSplat)
        {
            var animationString = ReadAnimationString(2);

            if (animationString == FaceDownAnimation)
            {
                return currentDummy.FaceDownFrames;
            }
            if (animationString == FaceUpAnimation)
            {
                return currentDummy.FaceUpFrames;
            }
            if (animationString == WallSplatAnimation && playReversalOnWallSplat)
            {
                //wakeup timing is universal on wall splat recovery
                return WallSplatWakeupTiming;
            }

            return 0;

        }

        private int GetCurrentComboCount(int player)
        {
            //TODO find the pointer for player 2

            if (player == 1)
            {
                return this._memoryReader.ReadWithOffsets<int>(_p1ComboCountPtr, _p1ComboCountPtrOffset);
            }

            if (player == 2)
            {
                return this._memoryReader.ReadWithOffsets<int>(_p2ComboCountPtr, _p2ComboCountPtrOffset);
            }

            throw new NotImplementedException();
        }

        private int GetReplayKey()
        {
            IntPtr address = IntPtr.Add(this._process.MainModule.BaseAddress, _replayKeyOffset);
            return this._memoryReader.Read<int>(address);
        }

        //TODO Remove?
        public Keyboard.DirectXKeyStrokes GetReplayKeyStroke()
        {
            int replayKeyCode = this.GetReplayKey();
            char replayKey = (char)replayKeyCode;

            if (!Enum.TryParse($"DIK_{replayKey}", out Keyboard.DirectXKeyStrokes stroke))
            {
                stroke = Keyboard.DirectXKeyStrokes.DIK_P;
            }

            return stroke;
        }


        private void Wait(int frames)
        {
            if (frames > 0)
            {
                int startFrame = this.FrameCount();
                int frameCount = 0;

                while (frameCount < frames)
                {
                    Thread.Sleep(10);

                    frameCount = this.FrameCount() - startFrame;
                }
            }
        }


        private void StartDummyLoop()
        {
            lock (RunDummyThreadLock)
            {
                _runDummyThread = true;
            }

            Thread dummyThread = new Thread(() =>
                {
                    LogManager.Instance.WriteLine("dummyThread start");
                    NameWakeupData currentDummy = null;
                    bool localRunDummyThread = true;

                    while (localRunDummyThread && !this._process.HasExited)
                    {
                        try
                        {
                            var dummy = GetDummy();

                            if (!Equals(dummy, currentDummy))
                            {
                                currentDummy = dummy;

                                DummyChanged?.Invoke(dummy);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.Instance.WriteException(ex);
                            StopDummyLoop();
                            DummyLoopErrorOccured?.Invoke(ex);
                            return;
                        }

                        lock (RunDummyThreadLock)
                        {
                            localRunDummyThread = _runDummyThread;
                        }

                        Thread.Sleep(2000);
                    }

                    LogManager.Instance.WriteLine("dummyThread ended");
                })
            { Name = "dummyThread" };

            dummyThread.Start();
        }

        public void StopDummyLoop()
        {
            lock (RunDummyThreadLock)
            {
                _runDummyThread = false;
            }
        }

        #endregion

        #region Dispose Members
        public void Dispose()
        {
            StopDummyLoop();
            StopReversalLoop();
            StopBlockReversalLoop();
        }
        #endregion



    }
}
