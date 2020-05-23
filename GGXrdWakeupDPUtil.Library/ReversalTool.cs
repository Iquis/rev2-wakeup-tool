using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using GGXrdWakeupDPUtil.Library.Enums;

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

        private char FrameDelimiter = ',';
        private char WakeUpFrameDelimiter = '!';
        const int WakeupFrameMask = 0x200;

        #region Offsets
        private readonly IntPtr _p2IdOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2IdOffset"), 16));
        private readonly IntPtr _recordingSlotPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("RecordingSlotPtr"), 16));
        private readonly IntPtr _p1AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1AnimStringPtr"), 16));
        private readonly int _p1AnimStringPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1AnimStringPtrOffset"), 16);
        private readonly IntPtr _p2AnimStringPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2AnimStringPtr"), 16));
        private readonly int _p2AnimStringPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P2AnimStringPtrOffset"), 16);
        private readonly IntPtr _frameCountOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("FrameCountOffset"), 16));
        private readonly IntPtr _scriptOffset = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("ScriptOffset"), 16));
        private readonly IntPtr _p1ComboCountPtr = new IntPtr(Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1ComboCountPtr"), 16));
        private readonly int _p1ComboCountPtrOffset = Convert.ToInt32(ConfigurationManager.AppSettings.Get("P1ComboCountPtrOffset"), 16);
        #endregion

        private readonly string FaceDownAnimation = "CmnActFDown2Stand";
        private readonly string FaceUpAnimation = "CmnActBDown2Stand";

        private const int RecordingSlotSize = 4808;

        private Process _process;

        private MemoryReader _memoryReader;

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


        #region Dll Imports
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion




        public void AttachToProcess()
        {
            var process = Process.GetProcessesByName(_ggprocname).FirstOrDefault();

            if (process == null)
            {
                throw new Exception("GG process not found!");
            }

            this._process = process;
            this._memoryReader = new MemoryReader(process);


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
            IntPtr address = IntPtr.Add(this._process.MainModule.BaseAddress, 0x1BDBE08); //TODO Config
            var index = this._memoryReader.Read<int>(address);

            var result = _nameWakeupDataList[index];

            return result;

        }

        public SlotInput SetInputInSlot(int slotNumber, string input)
        {
            List<string> inputList = GetInputList(input);
            int wakeupFrameIndex = inputList.FindLastIndex(x => x.StartsWith(WakeUpFrameDelimiter.ToString()));

            if (wakeupFrameIndex < 0)
            {
                throw new Exception("No ! frame specified.  See the readme for more information.");
            }

            IEnumerable<ushort> inputShorts = GetInputShorts(inputList);

            var enumerable = inputShorts as ushort[] ?? inputShorts.ToArray();
            OverwriteSlot(slotNumber, enumerable);

            return new SlotInput(input, enumerable, wakeupFrameIndex);
        }
        private void WaitAndReversal(SlotInput slotInput, int wakeupTiming, Keyboard.DirectXKeyStrokes stroke)
        {
            int fc = FrameCount();
            var frames = wakeupTiming - slotInput.WakeupFrameIndex - 1;
            while (FrameCount() < fc + frames)
            {
            }
            PlayReversal(stroke);

            Thread.Sleep(320); //20 frames, approximately, it's actually 333.333333333 ms.  Nobody should be able to be knocked down and get up in this time, causing the code to execute again.
        }

        public void PlayReversal(Keyboard.DirectXKeyStrokes stroke)
        {

#if DEBUG
            Console.WriteLine("Reversal!");
#endif

            BringWindowToFront();
            Keyboard keyboard = new Keyboard();


            keyboard.SendKey(stroke, false, Keyboard.InputType.Keyboard);
            Thread.Sleep(150);
            keyboard.SendKey(stroke, true, Keyboard.InputType.Keyboard);



#if DEBUG
            Console.WriteLine("Reversal Wait Finished!");
#endif

        }

        public void StartReversalLoop(SlotInput slotInput)
        {
            lock (RunReversalThreadLock)
            {
                _runReversalThread = true;
            }

            Thread reversalThread = new Thread(() =>
            {
                var currentDummy = GetDummy();
                bool localRunReversalThread = true;

                Keyboard.DirectXKeyStrokes stroke = this.GetReplayKeyStroke();

                while (localRunReversalThread)
                {
                    try
                    {
                        int wakeupTiming = GetWakeupTiming(currentDummy);


                        if (wakeupTiming != 0)
                        {
                            WaitAndReversal(slotInput, wakeupTiming, stroke);
                        }
                    }
                    catch (Exception ex)
                    {
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

#if DEBUG
                Console.WriteLine("reversalThread ended");
#endif
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
                bool localRunRandomBurstThread = true;


                SetInputInSlot(1, "!5HD");

                Random rnd = new Random();

                int valueToBurst = rnd.Next(min, max + 1);
                bool willBurst = rnd.Next(0, 101) <= burstPercentage;

                Keyboard.DirectXKeyStrokes stroke = this.GetReplayKeyStroke();


                while (localRunRandomBurstThread)
                {
                    try
                    {
                        int currentCombo = GetCurrentComboCount(1);



                        while (currentCombo > 0)
                        {
                            if (currentCombo == valueToBurst && willBurst)
                            {
                                PlayReversal(stroke);
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
                        StopRandomBurstLoop();
                        RandomBurstlLoopErrorOccured?.Invoke(ex);
                        return;
                    }

                }

#if DEBUG
                Console.WriteLine("randomBurstThread ended");
#endif

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

        public bool CheckValidInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var inputList = GetInputList(input);



            return inputList.All(x =>
            {
                Regex regex = new Regex(@"^!{0,1}[1-9][PKSHDpksdh]*(?:,|$)");

                return regex.IsMatch(x);
            })
            && inputList.Where(x =>
            {
                Regex regex = new Regex(@"^!{1}[1-9][PKSHDpksdh]*(?:,|$)");
                return regex.IsMatch(x);
            }).Count() == 1
            ;
        }

        #region Private
        private List<string> GetInputList(string input)
        {
            Regex whitespaceRegex = new Regex(@"\s+");

            var text = whitespaceRegex.Replace(input, "");

            string[] splitText = text.Split(FrameDelimiter);

            return splitText.ToList();
        }

        private IEnumerable<ushort> GetInputShorts(List<string> inputList)
        {
            List<ushort> result = inputList.Select(x =>
            {
                var value = SingleInputParse(x);

                if (value == 0 && x != "5")
                {
                    throw new Exception("Invalid input with input '" + value + "'.  Read the README for formatting information.");
                }

                return value;
            }).ToList();

            int wakeupFrameIndex = inputList.FindLastIndex(x => x.StartsWith(WakeUpFrameDelimiter.ToString()));

            result[wakeupFrameIndex] -= WakeupFrameMask;

            return result;
        }

        private ushort SingleInputParse(string input)
        {
            Regex inputregex = new Regex(WakeUpFrameDelimiter + @"?[1-9]{1}[PKSHD]{0,5}");

            int[] directions = { 0b0110, 0b0010, 0b1010, 0b0100, 0b0000, 0b1000, 0b0101, 0b0001, 0b1001 };

            if (inputregex.IsMatch(input))
            {
                var result = 0;
                if (input[0] == WakeUpFrameDelimiter)
                {
                    result += WakeupFrameMask;
                    input = input.Substring(1);
                }
                var direction = Int32.Parse(input.Substring(0, 1));
                result |= directions[direction - 1];
                if (input.Length == 1)
                {
                    return (ushort)result;
                }
                var buttons = input.Substring(1).ToCharArray();
                foreach (char button in buttons)
                {
                    switch (button)
                    {
                        case 'P':
                        case 'p':
                            result |= (int)Buttons.P;
                            break;
                        case 'K':
                        case 'k':
                            result |= (int)Buttons.K;
                            break;
                        case 'S':
                        case 's':
                            result |= (int)Buttons.S;
                            break;
                        case 'H':
                        case 'h':
                            result |= (int)Buttons.H;
                            break;
                        case 'D':
                        case 'd':
                            result |= (int)Buttons.D;
                            break;
                    }
                }
                return (ushort)result;
            }
            else
            {
                //TODO test
                return 0;
            }
        }

        private void OverwriteSlot(int slotNumber, IEnumerable<ushort> inputs)
        {
            var ptr = this._memoryReader.ReadWithOffsets<IntPtr>(_recordingSlotPtr);

            var slotAddr = IntPtr.Add(ptr, RecordingSlotSize * (slotNumber - 1));


            var inputList2 = inputs as ushort[] ?? inputs.ToArray();
            var header2 = new List<ushort> { 0, 0, (ushort)inputList2.Length, 0 };

            var content = header2.Concat(inputList2).ToArray();

            this._memoryReader.Write(slotAddr, content);
        }

        private string ReadAnimationString(int player)
        {
            if (player == 1)
            {
                var baseAddress = new IntPtr(0x1B18C78);
                var offset = 0x244C;
                var length = 32;
                return this._memoryReader.ReadStringWithOffsets(baseAddress, length, offset);
            }

            if (player == 2)
            {
                var baseAddress = new IntPtr(0x1B18C7C);
                var offset = 0x244C;
                var length = 32;
                return this._memoryReader.ReadStringWithOffsets(baseAddress, length, offset);
            }

            return string.Empty;
        }

        private int FrameCount()
        {
            var address = IntPtr.Add(this._process.MainModule.BaseAddress, 0x1BD1F90);
            return this._memoryReader.Read<int>(address);
        }
        private int GetWakeupTiming(NameWakeupData currentDummy)
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

            return 0;

        }

        private int GetCurrentComboCount(int player)
        {
            //TODO find the pointer for player 2

            if (player == 1)
            {
                //var ptr = _memorySharp[_p1ComboCountPtr].Read<IntPtr>();

                //return _memorySharp.Read<int>(ptr + _p1ComboCountPtrOffset, false);
            }


            throw new NotImplementedException();
        }

        private int GetReplayKey()
        {
            IntPtr address = IntPtr.Add(this._process.MainModule.BaseAddress, 0x1AD79EC);
            return this._memoryReader.Read<int>(address);
        }

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

        private void StartDummyLoop()
        {
            lock (RunDummyThreadLock)
            {
                _runDummyThread = true;
            }

            Thread dummyThread = new Thread(() =>
                {
                    NameWakeupData currentDummy = null;
                    bool localRunDummyThread = true;

                    while (localRunDummyThread)//TODO !_memorySharp.Handle.IsClosed)
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
#if DEBUG
                    Console.WriteLine("dummyThread ended");
#endif
                })
            { Name = "dummyThread" };

            dummyThread.Start();
        }

        private void StopDummyLoop()
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
        }
        #endregion



    }
}
