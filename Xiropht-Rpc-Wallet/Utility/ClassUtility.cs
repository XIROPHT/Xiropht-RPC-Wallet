using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xiropht_Connector_All.Utils;

namespace Xiropht_Rpc_Wallet.Utility
{
    public class ClassUtility
    {
        private static readonly List<string> ListOfCharacters = new List<string>
        {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
            "v", "w", "x", "y", "z"
        };

        private static readonly List<string> ListOfNumbers = new List<string> {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};

        private static readonly List<string> ListOfSpecialCharacters = new List<string> { "&", "~", "#", "@", "'", "(", "\\", ")", "=" };

        public static string ConvertPath(string path)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                path = path.Replace("\\", "/");
            }
            return path;
        }

        /// <summary>
        /// Hide characters from console pending input.
        /// </summary>
        /// <returns></returns>
        public static string GetHiddenConsoleInput()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }

            return input.ToString();
        }



        /// <summary>
        /// Check if the word contain number(s)
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool CheckNumber(string word)
        {
            return ListOfNumbers.Where((t, i) => i < ListOfNumbers.Count).Any(t => word.Contains(Convert.ToString(t)));
        }

        /// <summary>
        /// Check if the word contain letter(s)
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool CheckLetter(string word)
        {
            return ListOfCharacters.Where((t, i) => i < ListOfCharacters.Count).Any(word.Contains);
        }

        /// <summary>
        /// Check if the word contain special character(s)
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool CheckSpecialCharacters(string word)
        {
            var regx = new Regex("[^a-zA-Z0-9_.]");
            return regx.IsMatch(word);
        }

        /// <summary>
        /// Get string between two strings.
        /// </summary>
        /// <param name="STR"></param>
        /// <param name="FirstString"></param>
        /// <param name="LastString"></param>
        /// <returns></returns>
        public static string GetStringBetween(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }

        /// <summary>
        /// Make a new genesis key for dynamic encryption.
        /// </summary>
        /// <returns></returns>
        public static string MakeRandomWalletPassword()
        {
            string walletPassword = string.Empty;
            while (!CheckSpecialCharacters(walletPassword) || !CheckLetter(walletPassword) || !CheckNumber(walletPassword))
            {
                for (int i = 0; i < ClassUtils.GetRandomBetween(10, 128); i++)
                {
                    var randomUpper = ClassUtils.GetRandomBetween(0, 100);
                    if (randomUpper <= 30)
                    {
                        walletPassword += ListOfCharacters[ClassUtils.GetRandomBetween(0, ListOfCharacters.Count - 1)];
                    }
                    else if (randomUpper > 30 && randomUpper <= 50)
                    {
                        walletPassword += ListOfCharacters[ClassUtils.GetRandomBetween(0, ListOfCharacters.Count - 1)].ToUpper();
                    }
                    else if (randomUpper > 50 && randomUpper <= 70)
                    {
                        walletPassword += ListOfSpecialCharacters[ClassUtils.GetRandomBetween(0, ListOfSpecialCharacters.Count - 1)];
                    }
                    else if (randomUpper > 70)
                    {
                        walletPassword += ListOfNumbers[ClassUtils.GetRandomBetween(0, ListOfNumbers.Count - 1)];
                    }
                }
            }
            return walletPassword;
        }
    }
}
