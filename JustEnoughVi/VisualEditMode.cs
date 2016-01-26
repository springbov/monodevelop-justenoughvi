﻿using System;
using Mono.TextEditor;

namespace JustEnoughVi
{
    public class VisualEditMode : BaseEditMode
    {
        private string _countString;
        private int lineStart;

        private int Count {
            get {
                try {
                    return Convert.ToInt32(_countString);
                } catch (FormatException) {
                    return 0;
                }
            }
        }

        public VisualEditMode(ViEditMode vi) : base(vi)
        {
            _countString = "";
        }

        public override void InternalActivate(TextEditor editor, TextEditorData data)
        {
            _countString = "";
            data.Caret.Mode = CaretMode.Block;
            lineStart = data.Caret.Line;
        }

        public override void InternalDeactivate(TextEditor editor, TextEditorData data)
        {
            data.ClearSelection();
        }

        protected override void HandleKeypress(Gdk.Key key, uint unicodeKey, Gdk.ModifierType modifier)
        {
            if (modifier == 0)
            {
                // build repeat buffer
                if (unicodeKey >= '0' && unicodeKey <= '9')
                {
                    _countString += Char.ToString((char)unicodeKey);
                    return;
                }

                if (unicodeKey == 'j' || unicodeKey == 'k')
                {
                    if (unicodeKey == 'j')
                    {
                        Caret.Line++;
                    }
                    else if (unicodeKey == 'k')
                    {
                        Caret.Line--;
                    }

                    int start = lineStart;
                    int end = Caret.Line;

                    if (end < start)
                    {
                        end--;
                        start++;
                    }

                    Data.SetSelectLines(start, end);
                }

                if (unicodeKey == 'd')
                {
                    ClipboardActions.Cut(Data);
                    Vi.SetMode(ViMode.Normal);
                }

                if (unicodeKey == 'y' || unicodeKey == 'Y')
                {
                    ClipboardActions.Copy(Data);
                    Data.ClearSelection();
                    Vi.SetMode(ViMode.Normal);
                }

                if (unicodeKey == '<')
                {
                    var count = Math.Max(1, Count);
                    for (int i = 0; i < count; i++)
                    {
                        RunAction(MiscActions.RemoveIndentSelection);
                    }
                    Data.ClearSelection();
                    Vi.SetMode(ViMode.Normal);
                }

                if (unicodeKey == '>')
                {
                    var count = Math.Max(1, Count);
                    for (int i = 0; i < count; i++)
                    {
                        RunAction(MiscActions.IndentSelection);
                    }
                    Data.ClearSelection();
                    Vi.SetMode(ViMode.Normal);
                }
            }
        }
    }
}
