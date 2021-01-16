////////////////////////////////////////////////////////////////////////////////////
// Sqrt-Longhand.cs - Compute Square Root by Longhand method
// Can compute square root of any number, any number of digits, given
// enough computing power.   Million digits?  may take a few hours...
// Option to show its work, works best on numbers that fit on screen.
////////////////////////////////////////////////////////////////////////////////////
//
// MIT License
//
// Sqrt-Longhand.cs - Compute Square Root by Longhand method - Version 1.0
// Copyright(c) 2020-2021 by David R. Van Wagner
// davevw.com
// github.com/davervw
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Numerics;

namespace SquareRoot
{
    class SqrtLonghand
    {
        static void Main(string[] args)
        {
            DateTime start = DateTime.UtcNow;

            bool interactive = (args.Length == 0);
            if (interactive)
            {
                Console.WriteLine(@"
Sqrt-Longhand.cs - Compute Square Root by Longhand method - Version 1.0
Copyright(c) 2020-2021 by David R.Van Wagner
davevw.com
github.com/davervw
");
                Console.WriteLine("Command line usage example");
                Console.WriteLine("   Sqrt-Longhand.exe 1234567890.12345678901234567890 True");
                Console.WriteLine("(Boolean in usage specifies whether to show work)");
                Console.WriteLine();
                Console.WriteLine("Interactive mode");
                Console.WriteLine("Enter decimal number (any accuracy, hint include about twice as many decimal digits/zeros as you want returned)");
            }
            do
            {
                string s;
                if (interactive)
                {
                    Console.Write(": ");
                    s = Console.ReadLine();
                    if (!s.Contains("."))
                        s += '.';
                }
                else
                    s = args[0];

                bool show_work = true;
                if (interactive)
                {
                    Console.Write("Show work? (Y/n): ");
                    string show = Console.ReadLine();
                    if (show.ToUpper().StartsWith("N"))
                        show_work = false;
                }
                else if (args.Length == 2)
                {
                    bool.TryParse(args[1], out show_work);
                }

                var answer = SquareRoot(s, show_work, interactive);
                if (!show_work)
                    Console.WriteLine(answer);

                TimeSpan dur = DateTime.UtcNow - start;
                Console.Error.WriteLine($"[{dur} elapsed]");
            } while (interactive);
        } // Main

        static string SquareRoot(string s, bool show_work = false, bool interactive = true)
        {
            string answer = string.Empty;
            int i_dec = s.IndexOf('.');
            int len; // length of first digit pair, always 1 or 2 to start with, then 2 from next iteration on
            if (i_dec == 0)
                throw new InvalidOperationException();
            else if (i_dec > 0)
                len = 2 - (i_dec % 2); // 1 or 2
            else
                len = answer.Length % 2;

            if (i_dec > 0 && (s.Length - i_dec - 1) % 2 == 1)
                s += '0'; // need one more zero for pairs of digits

            int left_side = 0; // calculate length of answer, that's going to be the same length as our working area
            int pad = 2 - len; // 1 if space needed before answer, 0 if none
            int show_iter = 0;
            int save_top = 0;
            int save_left = 0;
            if (show_work)
            {
                if (interactive)
                {
                    save_top = Console.CursorTop;
                    save_left = Console.CursorLeft;
                }

                if (i_dec > 0)
                    left_side = (i_dec + 1) / 2 + 1 + (s.Length - i_dec - 1) / 2; // left of decimal digit pairs + decimal + right of decimal digit pairs
                else
                    left_side = (s.Length + 1) / 2;

                Console.WriteLine(); // we don't know answer yet... leave a line, maybe we can come back and fill it in, or let user edit result

                Console.WriteLine("".PadRight(s.Length + pad, '-').PadLeft(s.Length + left_side + 2 + pad)); // 2 is for square root symbol

                Console.Write("\\/".PadLeft(left_side + 2)); // square root symbol
                Console.Write("".PadRight(pad)); // extra space if odd number of digits left of decimal
                Console.WriteLine(s); // number we are taking square root of
            }

            int i = 0;
            BigInteger rem = int.Parse(s.Substring(i, len));
            bool end = false;
            BigInteger left_last = 0;
            while (!end)
            {
                // multiplier is twice answer so far, and multiply by ten
                var k = 20 * (answer.Length == 0 ? BigInteger.Zero : BigInteger.Parse(answer.Replace(".", "")));

                // find multiplier digit that is big enough but not too big
                int j;
                for (j = 0; j < 10 && (k + j + 1) * (j + 1) <= rem; ++j)
                    ;

                // combine multiplier and digit
                left_last = k + j;

                int digit = (int)(left_last % 10);
                answer += digit.ToString();
                rem = rem - left_last * digit;

                // advance in problem number
                i += len; // usually +2, but will be +1 for first run if odd number of whole digits
                if (i < s.Length && s[i] == '.') // found decimal point? add to answer
                {
                    answer += '.';
                    ++i; // advance past decimal
                }

                // drop down next 2 digits for remainder
                if (i < s.Length)
                {
                    len = 2; // once get going (first is one for odd length whole digits), always two digits each following iteration
                    rem = rem * 100 + int.Parse(s.Substring(i, len));
                }
                else
                    end = true;

                if (show_work)
                {
                    var mul = left_last * digit;
                    string rem_s = rem.ToString();

                    Console.Write(left_last.ToString().PadLeft(left_side));

                    Console.WriteLine(mul.ToString().PadLeft(4 + 2 * show_iter));

                    Console.Write("".PadLeft(left_side));
                    Console.WriteLine("".PadRight(rem_s.Length, '-').PadLeft(6 + 2 * show_iter - (end ? 2 : 0)));

                    Console.Write("".PadLeft(left_side));
                    Console.WriteLine(rem_s.PadLeft(6 + 2 * show_iter - (end ? 2 : 0)));

                    ++show_iter;
                }
            }

            if (show_work && interactive)
            {
                Console.WriteLine();

                // save current position
                int top = Console.CursorTop;
                int left = Console.CursorLeft;

                // restore to location for answer
                Console.SetCursorPosition(save_left, save_top);

                Console.Write("".PadLeft(left_side + 2));
                foreach (char c in answer)
                {
                    if (c != '.')
                        Console.Write(" ");
                    Console.Write(c);
                }
                Console.WriteLine();

                // restore position
                Console.SetCursorPosition(left, top);
            }

            if (show_work && !interactive)
            {
                Console.WriteLine("=");
                Console.WriteLine(answer);
            }

            return answer;
        } // SquareRoot()

    } // class SqrtLonghand

} // namespace SquareRoot

// [END] Sqrt-Longhand.cs
////////////////////////////////////////////////////////////////////////////////////
