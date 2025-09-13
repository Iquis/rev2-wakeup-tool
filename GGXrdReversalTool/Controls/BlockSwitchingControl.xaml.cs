using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Scenarios.BlockSwitching;
using GGXrdReversalTool.Library.Scenarios.BlockSwitching.Implementations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace GGXrdReversalTool.Controls
{
    public partial class BlockSwitchingControl : NotifiedUserControl
    {
        public BlockSwitchingControl()
        {
            BlockSwitchingElements = new ObservableCollection<BlockSwitchingElement>();
            InitializeComponent();
        }
        
        private string _blockSwitchingText = string.Empty;
        public string BlockSwitchingText
        {
            get => _blockSwitchingText;
            set
            {
                if (_blockSwitchingText == value) return;
                _blockSwitchingText = value;
                OnPropertyChanged();
                OnBlockSequenceChanged(true);
            }
        }
        private void SetBlockSwitchingTextAndSelection(string newText, int selectionStart)
        {
            _blockSwitchingText = newText;
            OnPropertyChanged("BlockSwitchingText");
            TextBoxControl.SelectionStart = selectionStart;
            OnBlockSequenceChanged(true);
        }
        
        private void InsertNewElementAfterCurrentElement(string str)
        {
            if (string.IsNullOrWhiteSpace(BlockSwitchingText))
            {
                SetBlockSwitchingTextAndSelection(str, str.Length);
                return;
            }
            int nextComma = -1;
            int selStart = TextBoxControl.SelectionStart;
            if (selStart < BlockSwitchingText.Length) nextComma = BlockSwitchingText.IndexOf(',', selStart);
            if (nextComma == -1)
            {
                string newText = BlockSwitchingText + "," + str;
                SetBlockSwitchingTextAndSelection(newText, newText.Length);
                return;
            }
            
            SetBlockSwitchingTextAndSelection(BlockSwitchingText.Substring(0, nextComma) + "," + str + BlockSwitchingText.Substring(nextComma),
                nextComma + 1 + str.Length);
        }
        public static string TranslateStanceToNotation(BlockInputStanceType value)
        {
            return value switch
            {
                BlockInputStanceType.Standing => "s",
                BlockInputStanceType.Crouching => "c",
                BlockInputStanceType.Jumping => "j",
                BlockInputStanceType.Opposite => "w",
                BlockInputStanceType.Random => "?",
                BlockInputStanceType.Pin => "p",
                _ => "_"
            };
        }
        public static string TranslateBlockStanceToNotation(BlockInputBlockType value)
        {
            return value switch
            {
                BlockInputBlockType.Normal => "n",
                BlockInputBlockType.FD => "f",
                BlockInputBlockType.IB => "i",
                BlockInputBlockType.Random => "?",
                BlockInputBlockType.GamesOwnImplementationOfRandom => "g",
                BlockInputBlockType.Pin => "p",
                _ => "_"
            };
        }
        public static string TranslateBlockSwitchingToNotation(BlockInputSwitchingType value)
        {
            return value switch
            {
                BlockInputSwitchingType.On => "e",
                BlockInputSwitchingType.Off => "d",
                BlockInputSwitchingType.OnSecond => "2",
                _ => "_"
            };
        }
        public static string TranslateBlockSettingsToNotation(BlockInputBlockSettingsType value)
        {
            return value switch
            {
                BlockInputBlockSettingsType.NotBlock => "h",
                BlockInputBlockSettingsType.Everything => "b",
                BlockInputBlockSettingsType.AfterFirst => "2",
                BlockInputBlockSettingsType.FirstOnly => "1",
                BlockInputBlockSettingsType.Random => "?",
                BlockInputBlockSettingsType.Pin => "p",
                _ => "_"
            };
        }
        
        public ObservableCollection<BlockSwitchingElement> BlockSwitchingElements { get; set; }
        
        public static IEnumerable<SplitStringElement> SplitString(string str, char separator)
        {
            return new SplitStringEnumerable(str, separator);
        }
                
        public static IEnumerable<SplitStringElement> SplitString(string str, string separator)
        {
            return new SplitStringEnumerable(str, separator);
        }
        
        // universal:
        // _ - not change this setting. Which setting, depends on its position and what settings are already defined
        // ?/r - random. Same meaning intricacies as _
        // ! - reversal frame
        // *34589734985734 (at the end of an element) - repeat this many times
        
        // Stance
        // s = standing
        // c = crouching
        // ?/r = random stance between standing and crouching
        // w = switch stance from crouching to standing or vice versa
        // j = jumping
        // p = pin
        
        // Block Type
        // n = normal block
        // i = IB
        // f = FD
        // p = pin
        // ?/r = random block
        
        // Block Switching
        // e = set Block Switching to 'Enabled'
        // d = set Block Switching to 'Disabled'
        // 2 = set Block Switching to 'Swith on the 2nd'
        
        // Block Settings
        // b - block everything
        // h - block nothing
        // ?/r - randomly block or not block
        // p - pin
        // 1 - first hit only
        // 2 - after first hit
        
        public static IEnumerable<BlockSwitchingElement> ParseInputText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Enumerable.Empty<BlockSwitchingElement>();
            }
            
            ParseCharResult parseResult = new ParseCharResult();
            int elementCounter = 0;
            
            return SplitString(text, ',').Select(splitElem => {
                
                string str = splitElem.Str;
                
                parseResult.Stance = null;
                parseResult.Block = null;
                parseResult.Switching = null;
                parseResult.Settings = null;
                parseResult.IsReversal = false;
                
                foreach (char c in str)
                {
                    if (c == '*') break;
                    if (c == '!') parseResult.IsReversal = true;
                    else ParseChar(parseResult, c);
                }
                
                bool hasMultiplier = false;
                int multiplier = 1;
                int index = str.IndexOf('*');
                if (index != -1) hasMultiplier = int.TryParse(str.Substring(index + 1).Trim(), out multiplier);
                
                BlockSwitchingElement newElem = new (
                    parseResult.Stance ?? BlockInputStanceType.Unchanged,
                    parseResult.Block ?? BlockInputBlockType.Unchanged,
                    parseResult.Switching ?? BlockInputSwitchingType.Unchanged,
                    parseResult.Settings ?? BlockInputBlockSettingsType.Unchanged,
                    parseResult.IsReversal,
                    hasMultiplier,
                    multiplier);
                
                newElem.Start = splitElem.Start;
                newElem.End = splitElem.End;
                newElem.Index = elementCounter++;
                
                return newElem;
            });
        }
        private class ParseCharResult
        {
            public BlockInputStanceType? Stance;
            public BlockInputBlockType? Block;
            public BlockInputSwitchingType? Switching;
            public BlockInputBlockSettingsType? Settings;
            public bool IsReversal;
        }
        private static void ParseChar(ParseCharResult result, char c)
        {
            switch (c)
            {
                case 's': result.Stance = BlockInputStanceType.Standing; return;
                case 'c': result.Stance = BlockInputStanceType.Crouching; return;
                case '?' when result.Stance == null:
                case 'r' when result.Stance == null:
                    result.Stance = BlockInputStanceType.Random; return;
                case 'w': result.Stance = BlockInputStanceType.Opposite; return;
                case 'j': result.Stance = BlockInputStanceType.Jumping; return;
                case 'p' when result.Stance == null:
                    result.Stance = BlockInputStanceType.Pin; return;
                case '_' when result.Stance == null:
                    result.Stance = BlockInputStanceType.Unchanged; return;
                
            }
            
            switch (c)
            {
                case 'n': result.Block = BlockInputBlockType.Normal; return;
                case 'i': result.Block = BlockInputBlockType.IB; return;
                case 'f': result.Block = BlockInputBlockType.FD; return;
                case 'p' when result.Block == null:
                    result.Block = BlockInputBlockType.Pin; return;
                case '?' when result.Block == null:
                case 'r' when result.Block == null:
                    result.Block = BlockInputBlockType.Random; return;
                case 'g': result.Block = BlockInputBlockType.GamesOwnImplementationOfRandom; return;
                case '_' when result.Block == null:
                    result.Block = BlockInputBlockType.Unchanged; return;
            }
            
            switch (c)
            {
                case 'e': result.Switching = BlockInputSwitchingType.On; return;
                case 'd': result.Switching = BlockInputSwitchingType.Off; return;
                case '2' when result.Switching == null:
                    result.Switching = BlockInputSwitchingType.OnSecond; return;
                case '_' when result.Switching == null:
                    result.Switching = BlockInputSwitchingType.Unchanged; return;
            }
            
            switch (c)
            {
                case 'b': result.Settings = BlockInputBlockSettingsType.Everything; return;
                case 'h': result.Settings = BlockInputBlockSettingsType.NotBlock; return;
                case '?' when result.Settings == null:
                case 'r' when result.Settings == null:
                    result.Settings = BlockInputBlockSettingsType.Random; return;
                case 'p' when result.Settings == null:
                    result.Settings = BlockInputBlockSettingsType.Pin; return;
                case '1': result.Settings = BlockInputBlockSettingsType.FirstOnly; return;
                case '2' when result.Settings == null:
                    result.Settings = BlockInputBlockSettingsType.AfterFirst; return;
                case '_' when result.Settings == null:
                    result.Settings = BlockInputBlockSettingsType.Unchanged; return;
            }
            
        }
        private void OnBlockSequenceChanged(bool updateSelectedElement)
        {
            _currentSelectedElement = null;
            var elements = ParseInputText(BlockSwitchingText);
            BlockSwitchingElements = new ObservableCollection<BlockSwitchingElement>(
                elements.Take(70)  // it starts to freeze up for a while when drawing too many men, even though they're off-screen
                // I wonder if VirtualizingStackPanel can help with this, if we can somehow calculate the available horizontal space to determine
                // what maximum number of men fit on one row.
            );
            OnPropertyChanged("BlockSwitchingElements");
            
            CreateScenario();
            
            if (updateSelectedElement)
            {
                SetSelectedElement(FindCurrentSelectedElement(TextBoxControl.SelectionStart));
            }
        }
        
        private void CreateScenario()
        {
            ScenarioBlockSwitching = new BlockSwitching()
            {
                BlockedHitTimer = BlockTimer,
                IsLooping = IsLooping,
                Elements = BlockSwitchingElements.ToArray()
            };
        }
        
        public IScenarioBlockSwitching? ScenarioBlockSwitching
        {
            get => (IScenarioBlockSwitching?)GetValue(ScenarioBlockSwitchingProperty);
            set => SetValue(ScenarioBlockSwitchingProperty, value);
        }
        public static readonly DependencyProperty ScenarioBlockSwitchingProperty =
            DependencyProperty.Register(nameof(ScenarioBlockSwitching), typeof(IScenarioBlockSwitching), typeof(BlockSwitchingControl),
                new PropertyMetadata(null));
        
        private string _blockTimerText = "30";
        public string BlockTimerText
        {
            get => _blockTimerText;
            set
            {
                if (_blockTimerText.Equals(value)) return;
                _blockTimerText = value;
                int intValue;
                if (!int.TryParse(value, out intValue)) intValue = 30;
                BlockTimer = intValue;
                CreateScenario();
            }
        }
        
        private bool _isLooping = false;
        public bool IsLooping
        {
            get => _isLooping;
            set
            {
                if (_isLooping == value) return;
                _isLooping = value;
                OnPropertyChanged();
                CreateScenario();
            }
        }
        
        public int BlockTimer
        {
            get => (int)GetValue(BlockTimerProperty);
            set => SetValue(BlockTimerProperty, value);
        }
        public static readonly DependencyProperty BlockTimerProperty =
            DependencyProperty.Register(nameof(BlockTimer), typeof(int), typeof(BlockSwitchingControl),
                new PropertyMetadata(30));
        
        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }
        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register(nameof(IsRunning), typeof(bool), typeof(BlockSwitchingControl),
                new PropertyMetadata(false));
        
        public RelayCommand AddNewElementCommand => new (AddNewElement);
        public RelayCommand EditCurrentElementCommand => new (EditCurrentElement);
        
        private void AddNewElement()
        {
            var window = new NewBlockSwitchingElementWindow();
            if (window.ShowDialog() == true)
            {
                InsertNewElementAfterCurrentElement(window.TextRepresentationText);
            }
        }
        
        private void EditCurrentElement()
        {
            if (string.IsNullOrWhiteSpace(BlockSwitchingText))
            {
                AddNewElement();
                return;
            }
            
            int selectionStart = TextBoxControl.SelectionStart;
            int prevComma = selectionStart == BlockSwitchingText.Length
                ? BlockSwitchingText.LastIndexOf(',')
                : BlockSwitchingText.LastIndexOf(',', selectionStart);
            int nextComma = selectionStart == BlockSwitchingText.Length
                ? -1
                : BlockSwitchingText.IndexOf(',', selectionStart);
            if (nextComma == -1) nextComma = BlockSwitchingText.Length;
            
            string part = BlockSwitchingText.Substring(prevComma + 1, nextComma - (prevComma + 1));
            
            var window = new NewBlockSwitchingElementWindow();
            
            IEnumerable<BlockSwitchingElement> parseResult = ParseInputText(part);
            var enumerator = parseResult.GetEnumerator();
            if (enumerator.MoveNext())
            {
                window.SetInitialValues(enumerator.Current);
            }
            
            if (window.ShowDialog() == true)
            {
                SetBlockSwitchingTextAndSelection(
                    BlockSwitchingText.Substring(0, prevComma + 1)
                        + window.TextRepresentationText
                        + (
                            nextComma == BlockSwitchingText.Length
                                ? string.Empty
                                : BlockSwitchingText.Substring(nextComma, BlockSwitchingText.Length - nextComma)
                        ),
                    prevComma + 1 + window.TextRepresentationText.Length);
            }
        }
        
        private BlockSwitchingElement? _currentSelectedElement = null;
        private void TextBoxControl_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (BlockSwitchingElements.Count() == 0) return;
            TextBox tb = (TextBox)e.Source;
            
            int selStart = tb.SelectionStart;
            
            SetSelectedElement(FindCurrentSelectedElement(selStart));
        }
        
        private void SetSelectedElement(BlockSwitchingElement? element)
        {
            if (_currentSelectedElement != null)
            {
                _currentSelectedElement.IsSelected = false;
            }
            _currentSelectedElement = element;
            if (_currentSelectedElement != null)
            {
                _currentSelectedElement.IsSelected = true;
            }
        }
        
        private BlockSwitchingElement? FindCurrentSelectedElement(int selStart)
        {
            
            int start = 0;
            int end = BlockSwitchingElements.Count();
            BlockSwitchingElement elem;
            
            while (end - start > 1)
            {
                int mid = (start + end) >> 1;
                // possible values of mid:
                // end == start + 2  =>  mid = start + 1 = end - 1
                // end == start + 3  =>  mid = start + 1 = end - 2
                
                elem = BlockSwitchingElements[mid];
                if (selStart >= elem.Start)
                {
                    if (selStart <= elem.End) return elem;
                    start = mid + 1;  // start could become equal to end here
                }
                else end = mid;
                
            }
            
            if (start == end) return null;
            // start == end + 1
            elem = BlockSwitchingElements[start];
            if (selStart >= elem.Start && selStart <= elem.End) return elem;
            return null;
        }
        
        public RelayCommand<BlockSwitchingElement> ElementClickCommand => new (ElementClick);
        public RelayCommand<BlockSwitchingElement> ElementDoubleClickCommand => new (ElementDoubleClick);
        public RelayCommand<BlockSwitchingElement> ElementEditCommand => new (ElementEdit);
        public RelayCommand<BlockSwitchingElement> ElementDeleteCommand => new (ElementDelete);
        public RelayCommand<BlockSwitchingElement> ElementInsertNewAfterCommand => new (ElementInsertNewAfter);
        public RelayCommand<BlockSwitchingElement> ElementInsertNewBeforeCommand => new (ElementInsertNewBefore);
        public RelayCommand<BlockSwitchingElement> ElementMoveLeftCommand => new (ElementMoveLeft);
        public RelayCommand<BlockSwitchingElement> ElementMoveRightCommand => new (ElementMoveRight);
        
        private void ElementClick(BlockSwitchingElement element)
        {
            SetSelectedElement(element);
            TextBoxControl.SelectionStart = element.End;
        }
        
        private void ElementDoubleClick(BlockSwitchingElement element)
        {
            if (!IsRunning)
                ElementEdit(element);
        }
        
        private void ElementEdit(BlockSwitchingElement element)
        {
            SetSelectedElement(element);
            TextBoxControl.SelectionStart = element.Start;
            EditCurrentElement();
        }
        
        private void ElementDelete(BlockSwitchingElement element)
        {
            if (element.Index == BlockSwitchingElements.Count() - 1)
            {
                if (BlockSwitchingElements.Count() == 1)
                {
                    BlockSwitchingText = string.Empty;
                    return;
                }
                
                SetBlockSwitchingTextAndSelection(
                    BlockSwitchingText.Substring(0, element.Start - 1),
                    element.Start - 1);
                return;
            }
            SetBlockSwitchingTextAndSelection(
                    BlockSwitchingText.Substring(0, element.Start)
                    + (
                        element.End + 1 >= BlockSwitchingText.Length
                            ? string.Empty
                            : BlockSwitchingText.Substring(element.End + 1)
                    ),
                    element.Start);
        }
        
        private void ElementInsertNewAfter(BlockSwitchingElement element)
        {
            SetSelectedElement(element);
            TextBoxControl.SelectionStart = element.End;
            AddNewElement();
        }
        
        private void ElementInsertNewBefore(BlockSwitchingElement element)
        {
            SetSelectedElement(element);
            TextBoxControl.SelectionStart = element.Start;
            
            var window = new NewBlockSwitchingElementWindow();
            if (window.ShowDialog() == true)
            {
                if (BlockSwitchingElements.Count() == 0)
                {
                    BlockSwitchingText = window.TextRepresentationText;
                    return;
                }
                
                SetBlockSwitchingTextAndSelection(
                    BlockSwitchingText.Substring(0, element.Start)
                        + window.TextRepresentationText + "," + BlockSwitchingText.Substring(element.Start),
                    element.Start + window.TextRepresentationText.Length);
                
            }
        }
        
        private void ElementMoveLeft(BlockSwitchingElement element)
        {
            if (element.Index == 0) return;
            
            BlockSwitchingElement prev = BlockSwitchingElements[element.Index - 1];
            
            SetBlockSwitchingTextAndSelection(
                BlockSwitchingText.Substring(0, prev.Start)
                    + BlockSwitchingText.Substring(element.Start, element.End - element.Start) + ","
                    + BlockSwitchingText.Substring(prev.Start, prev.End - prev.Start)
                    + (prev.End == BlockSwitchingText.Length ? string.Empty : BlockSwitchingText.Substring(element.End)),
                prev.Start + element.End - element.Start);
        }
        
        private void ElementMoveRight(BlockSwitchingElement element)
        {
            if (element.Index >= BlockSwitchingElements.Count() - 1) return;
            
            BlockSwitchingElement next = BlockSwitchingElements[element.Index + 1];
            
            SetBlockSwitchingTextAndSelection(
                BlockSwitchingText.Substring(0, element.Start)
                    + BlockSwitchingText.Substring(next.Start, next.End - next.Start) + ","
                    + BlockSwitchingText.Substring(element.Start, element.End - element.Start)
                    + (next.End >= BlockSwitchingText.Length ? string.Empty : BlockSwitchingText.Substring(next.End)),
                next.Start + element.End - element.Start);
        }

        private void HelpClick(object sender, RoutedEventArgs e)
        {
            BlockSwitchingControlHelpWindow helpWindow = new BlockSwitchingControlHelpWindow();
            helpWindow.Show();
        }
    }

    public struct SplitStringElement
    {
        public int Start;
        public int End;
        public string Str;
    }

    public class SplitStringEnumerator : IEnumerator<SplitStringElement>
    {
        public SplitStringEnumerator(string TargetString, char Separator)
        {
            _targetString = TargetString;
            _separator = Separator;
            _separatorString = null!;
            _separatorIsString = false;
            _separatorLength = 1;
        }
        public SplitStringEnumerator(string TargetString, string Separator)
        {
            _targetString = TargetString;
            _separatorString = Separator;
            _separatorIsString = true;
            _separatorLength = _separatorString.Length;
        }
        private int _separatorLength;
        private string _targetString;
        private char _separator;
        private string _separatorString;
        private bool _separatorIsString;
        private int Index = -1;
        private SplitStringElement _current;
        public object Current => _current;
        SplitStringElement IEnumerator<SplitStringElement>.Current => _current;
        
        public void Dispose()
        {
        }
        
        public bool MoveNext()
        {
            int pos;
            if (Index == -1)
            {
                pos = (_separatorIsString ? _targetString.IndexOf(_separatorString) : _targetString.IndexOf(_separator));
                _current.Start = 0;
                if (pos == -1)
                {
                    _current.End = _targetString.Length;
                    _current.Str = _targetString;  // it should be OK to return the source string because if they try to modify it in any way it produces a new string
                    Index = _targetString.Length;
                }
                else
                {
                    _current.End = pos;
                    _current.Str = _targetString.Substring(0, pos);
                    Index = pos;
                }
                return true;
            }
            if (Index >= _targetString.Length) return false;
            
            _current.Start = Math.Min(_targetString.Length, Index + _separatorLength);
            
            if (Index + _separatorLength >= _targetString.Length)
            {
                _current.End = _targetString.Length;
                _current.Str = string.Empty;
                Index = _targetString.Length;
                return true;
            }
            
            pos = (_separatorIsString
                ? _targetString.IndexOf(_separatorString, Index + _separatorLength)
                : _targetString.IndexOf(_separator, Index + 1));
            
            if (pos == -1)
            {
                _current.End = _targetString.Length;
                _current.Str = _targetString.Substring(Index + _separatorLength);
                Index = _targetString.Length;
            }
            else
            {
                _current.End = pos;
                _current.Str = _targetString.Substring(Index + _separatorLength, pos - (Index + _separatorLength));
                Index = pos;
            }
            return true;
        }

        public void Reset()
        {
            Index = -1;
        }
    }

    public class SplitStringEnumerable : IEnumerable<SplitStringElement>
    {
        public SplitStringEnumerable(string TargetString, char Separator)
        {
            _targetString = TargetString;
            _separator = Separator;
            _separatorString = null!;
            _separatorIsString = false;
        }
        public SplitStringEnumerable(string TargetString, string Separator)
        {
            _targetString = TargetString;
            _separatorString = Separator;
            _separatorIsString = true;
        }
        private string _targetString;
        private bool _separatorIsString;
        private char _separator;
        private string _separatorString;
        public IEnumerator GetEnumerator()
        {
            return _separatorIsString
                ? new SplitStringEnumerator(_targetString, _separatorString)
                : new SplitStringEnumerator(_targetString, _separator);
        }

        IEnumerator<SplitStringElement> IEnumerable<SplitStringElement>.GetEnumerator()
        {
            return _separatorIsString
                ? new SplitStringEnumerator(_targetString, _separatorString)
                : new SplitStringEnumerator(_targetString, _separator);
        }
    }
}
