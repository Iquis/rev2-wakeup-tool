using System.Text.RegularExpressions;
using GGXrdReversalTool.Library.Extensions;

namespace GGXrdReversalTool.Library.Models.Inputs;

public class SlotInput
{
    private const char FrameDelimiter = ',';
    private const char WakeUpFrameDelimiter = '!';
    private const int WakeupFrameMask = 0x200;
    private readonly string _inputPattern = @"(?<frameInput>" + WakeUpFrameDelimiter + @"{0,1}[1-9]{1}[pkshd]{0,5})(?>(?>\*(?<multiplicator>[0-9]+))|)";
    //TODO Replace by Enum Directions?
    private readonly int[] _directions = { 0b0110, 0b0010, 0b1010, 0b0100, 0b0000, 0b1000, 0b0101, 0b0001, 0b1001 };

    #region Constructors
    public SlotInput(string rawInputText)
    {
        RawInputText = rawInputText;
    }

    public SlotInput()
        :this(string.Empty)
    {
    }

    #endregion
    
    public string RawInputText { get; set; }

    public IEnumerable<string> ExpandedInputList => GetExpandedFrameInputList(RawInputText);
    public IEnumerable<string> CondensedInputListText => GetCondensedFrameInputListText(RawInputText);
    public IEnumerable<CondensedInput> CondensedInputList => GetCondensedFrameInputList(RawInputText);
    public IEnumerable<ushort> Content => GetContent(RawInputText);


    

    public int ReversalFrameIndex
    {
        get
        {
            return ExpandedInputList.FirstIndexOf(input => input.Contains(WakeUpFrameDelimiter));
        }
    }

    

    private IEnumerable<string> GetExpandedFrameInputList(string rawInputText)
    {
        var regex = new Regex(_inputPattern, RegexOptions.IgnoreCase);
        
        var frameInputList = rawInputText
            .Split(FrameDelimiter)
            .Where(frameInput => !string.IsNullOrEmpty(frameInput))
            .Where(frameInput => regex.IsMatch(frameInput))
            .Select(frameInput =>
            {
                var match = regex.Match(frameInput);
                var frameInputWithoutMultiplicator = match.Groups["frameInput"].Value;
                var multiplicator = int.TryParse(match.Groups["multiplicator"].Value, out var tmpMultiplicative)
                    ? tmpMultiplicative
                    : 1;
                
                return new
                {
                    FrameInputWithoutMultiplicator = frameInputWithoutMultiplicator,
                    Multiplicator = multiplicator
                };
            })
            .SelectMany(x => Enumerable.Range(0, x.Multiplicator).Select(_ => x.FrameInputWithoutMultiplicator))
            ;

        return frameInputList;
    }

    private IEnumerable<ushort> GetContent(string rawInputText)
    {
        var inputList = GetExpandedFrameInputList(rawInputText);
        
        var regex = new Regex(_inputPattern, RegexOptions.IgnoreCase);
        
        var result = inputList
                .SelectMany(rawFrameInput =>
                {
                    if (!regex.IsMatch(rawFrameInput))
                    {
                        return Enumerable.Empty<ushort>();
                    }

                    var match = regex.Match(rawFrameInput);
                    var frameInput = match.Groups["frameInput"].Value;

                    var multiplicative = int.TryParse(match.Groups["multiplicator"].Value, out var tmpMultiplicative)
                        ? tmpMultiplicative
                        : 1;

                    var values = frameInput
                            .Select(singleFrameInput =>
                            {
                                if (int.TryParse(singleFrameInput.ToString(), out var direction) &&
                                    direction is >= 1 and <= 9)
                                {
                                    return _directions[direction - 1];
                                }

                                var buttonText = singleFrameInput.ToString().ToUpper();

                                if (buttonText is not ("P" or "K" or "S" or "H" or "D"))
                                {
                                    return 0;
                                }

                                var button = Enum.Parse<Buttons>(buttonText);

                                return (int)button;

                            })
                        ;

                    var value = values.Aggregate((a, b) => a | b);

                    return Enumerable.Range(0, multiplicative).Select(_ => (ushort)value);

                })
            ;
        return result;
    }


    [Obsolete]
    private IEnumerable<ushort> ProcessInput(string inputText)
    {
        IEnumerable<string> inputList = inputText.Split(FrameDelimiter).Where(i => !string.IsNullOrEmpty(i));

        var regex = new Regex(_inputPattern, RegexOptions.IgnoreCase);

        var result = inputList
                .SelectMany(rawFrameInput =>
                {
                    if (!regex.IsMatch(rawFrameInput))
                    {
                        return Enumerable.Empty<ushort>();
                    }

                    var match = regex.Match(rawFrameInput);
                    var frameInput = match.Groups["frameInput"].Value;

                    var multiplicative = int.TryParse(match.Groups["multiplicator"].Value, out var tmpMultiplicative)
                        ? tmpMultiplicative
                        : 1;

                    var values = frameInput
                            .Select(singleFrameInput =>
                            {
                                if (int.TryParse(singleFrameInput.ToString(), out var direction) &&
                                    direction is >= 1 and <= 9)
                                {
                                    return _directions[direction - 1];
                                }

                                var buttonText = singleFrameInput.ToString().ToUpper();

                                if (buttonText is not ("P" or "K" or "S" or "H" or "D"))
                                {
                                    return 0;
                                }

                                var button = Enum.Parse<Buttons>(buttonText);

                                return (int)button;

                            })
                        ;

                    var value = values.Aggregate((a, b) => a | b);

                    return Enumerable.Range(0, multiplicative).Select(_ => (ushort)value);

                })
            ;
    

        return result;
    }


    private IEnumerable<CondensedInput> GetCondensedFrameInputList(string rawInputText)
    {
        var expandedFrameInputList = GetExpandedFrameInputList(rawInputText).ToList();

        var result = expandedFrameInputList
                .GroupWhile((prev, next) => prev == next)
                .Select(group =>
                {
                    var enumerable = group.ToList();
                    return new CondensedInput
                    {
                        FrameInput = enumerable.First(),
                        Multiplicator = enumerable.Count
                    };
                })
                .ToList() //TODO Remove
            ;

        return result;

    }

    private IEnumerable<string> GetCondensedFrameInputListText(string rawInputText)
    {
        return GetCondensedFrameInputList(rawInputText)
            .Select(x => x.Multiplicator > 1 ? $"{x.FrameInput}*{x.Multiplicator}" : $"{x.FrameInput}");
    }
    //TODO Unit tests


    public bool IsReversalValid => Content.Any(); //TODO check reversal frame mask

    public bool IsValid => Content.Any();
    


    public IEnumerable<ushort> Header => new List<ushort> { 0, 0, (ushort)Content.Count(), 0 };

    public IEnumerable<string> InputSplit
    {
        get
        {
            // var pattern = @"(?<frameInput>" + WakeUpFrameDelimiter + "{0,1}[1-9]{1}[pkshd]{0,5})";
            // var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            // return _inputText.Split(FrameDelimiter).Where(i => regex.IsMatch(i));
            
            return Enumerable.Empty<string>(); //TODO Implement
        }
    }
}



//TODO Remove
public class SlotInput_Old
{
    public SlotInput_Old()
        : this(string.Empty)
    {

    }

    public SlotInput_Old(string input)
    {
        InputText = input;
    }

    public SlotInput_Old(IList<byte> result)
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

        InputText = input;
    }


    private string _inputText;

    public string InputText
    {
        get => _inputText;
        set
        {
            _inputText = value;
            ProcessBuild(_inputText);
        }
    }

    const char FrameDelimiter = ',';
    const char WakeUpFrameDelimiter = '!';
    const int WakeupFrameMask = 0x200;


    private string Input { get; set; }
    public IList<ushort> InputList { get; private set; }
    public IList<string> InputSplit { get; private set; }

    public byte[] CleanInputs
    {
        get { throw new NotImplementedException(); }
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
                throw new Exception("Invalid input with input '" + value +
                                    "'.  Read the README for formatting information.");
            }

            return value;
        }).ToList();

        return result;
    }

    private ushort SingleInputParse(string singleInput)
    {
        Regex inputRegex = new Regex(WakeUpFrameDelimiter + @"?[1-9]{1}[PKSHD]{0,5}");

        //TODO Replace by Enum Directions?
        int[] directions = { 0b0110, 0b0010, 0b1010, 0b0100, 0b0000, 0b1000, 0b0101, 0b0001, 0b1001 };

        if (inputRegex.IsMatch(singleInput))
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
               }).Count() <= 1
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
            this.ReversalFrameIndex =
                InputSplit.ToList().FindLastIndex(x => x.StartsWith(WakeUpFrameDelimiter.ToString()));
            this.InputList = GetInputShorts(InputSplit);

            var header = new List<ushort> { 0, 0, (ushort)InputList.Count(), 0 };


            this.Content = header.Concat(InputList).ToArray();
        }
    }

    #endregion
}


