using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GGXrdWakeupDPUtil.Library.Enums;

namespace GGXrdWakeupDPUtil.Library
{
    public class SlotInput
    {
        public SlotInput()
        : this(string.Empty)
        {

        }
        public SlotInput(string input)
        {
            this.ProcessBuild(input);
        }
        public SlotInput(IList<byte> result)
        {
            var header = result.Take(8).ToArray();
            bool isP2 = header[0] == 1;

            var inputShorts = new List<ushort>();

            for (int i = 8; i < result.Count; i += 2)
            {
                var value = (ushort)(result[i] + (byte.MaxValue + 1) * result[i + 1]);
                inputShorts.Add(value);
            }

            var input = inputShorts
                .Select(x => SingleInputParse(x, isP2))
                .Aggregate((a, b) => $"{a},{b}");

            this.ProcessBuild(input);
        }


        const char FrameDelimiter = ',';
        const char WakeUpFrameDelimiter = '!';
        const int WakeupFrameMask = 0x200;




        public string Input { get; private set; }
        public IList<ushort> InputList { get; private set; }
        public IList<string> InputSplit { get; private set; }

        public byte[] CleanInputs
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int ReversalFrameIndex { get; private set; }

        public IEnumerable<ushort> Content { get; private set; }

        public bool IsReversalValid { get; private set; }
        public bool IsValid { get; private set; }


        #region MyRegion
        private IList<string> GetInputSplit(string input)
        {
            Regex whitespaceRegex = new Regex(@"\s+");

            var text = whitespaceRegex.Replace(input, "");

            string[] splitText = text.Split(FrameDelimiter);

            return splitText.ToList();
        }

        private IList<ushort> GetInputShorts(IEnumerable<string> inputList)
        {
            List<ushort> result = inputList.Select(x =>
            {
                var value = SingleInputParse(x);

                if (value == 0 && x != "5" && x != "!5")
                {
                    throw new Exception("Invalid input with input '" + value + "'.  Read the README for formatting information.");
                }

                return value;
            }).ToList();

            return result;
        }
        private ushort SingleInputParse(string singleInput)
        {
            Regex inputregex = new Regex(WakeUpFrameDelimiter + @"?[1-9]{1}[PKSHD]{0,5}");

            //TODO Replace by Enum Directions?
            int[] directions = { 0b0110, 0b0010, 0b1010, 0b0100, 0b0000, 0b1000, 0b0101, 0b0001, 0b1001 };

            if (inputregex.IsMatch(singleInput))
            {
                var result = 0;
                if (singleInput[0] == WakeUpFrameDelimiter)
                {
                    //result += WakeupFrameMask;
                    singleInput = singleInput.Substring(1);
                }
                var direction = Int32.Parse(singleInput.Substring(0, 1));
                result |= directions[direction - 1];
                if (singleInput.Length == 1)
                {
                    return (ushort)result;
                }
                var buttons = singleInput.Substring(1).ToCharArray();
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
                return 0;
            }
        }
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
        private bool IsDirectionPressed(ushort input, Directions direction)
        {
            return (input & (int)direction) == (int)direction;
        }
        private bool IsButtonPressed(ushort input, Buttons button)
        {
            return (input & (int)button) == (int)button;
        }
        private bool CheckValidInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var inputList = GetInputSplit(input);



            return inputList.All(x =>
                   {
                       Regex regex = new Regex(@"^!{0,1}[1-9][PKSHDpksdh]*(?:,|$)");

                       return regex.IsMatch(x);
                   })
                   && inputList.Where(x =>
                   {
                       Regex regex = new Regex(@"^!{1}[1-9][PKSHDpksdh]*(?:,|$)");
                       return regex.IsMatch(x);
                   }).Count() <=1
                ;
        }
        private bool CheckReversalValidInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var inputList = GetInputSplit(input);



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

        private void ProcessBuild(string input)
        {
            input = string.Concat(input.Select(x => x.ToString().ToUpper()));

            this.InputSplit = GetInputSplit(input);

            this.IsReversalValid = this.CheckReversalValidInput(input);
            this.IsValid = this.CheckValidInput(input);

            if (this.IsReversalValid || this.IsValid)
            {
                this.Input = String.Join(FrameDelimiter.ToString(), InputSplit.ToArray());
                this.ReversalFrameIndex = InputSplit.ToList().FindLastIndex(x => x.StartsWith(WakeUpFrameDelimiter.ToString()));
                this.InputList = GetInputShorts(InputSplit);

                var header = new List<ushort> {  0,0 , (ushort)InputList.Count(), 0 };


                this.Content = header.Concat(InputList).ToArray();
            }
        }
        #endregion
    }
}
