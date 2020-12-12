using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextProcessing
{
    class Program
    {
        private static readonly char[] ExtraChars = {'!', '?', ',', ';', ':', '.', '—','\"', '«', '»', '(',')' };

        static string[] ParseText(string text)
        { 
            foreach (char remChar in ExtraChars)
            {
                text = text.Replace(remChar,' ');
            }

            string[] result = text.Split(' ');
            result = result.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                for (int i = 0; i < result.Length; i++)
            {
                result[i] = result[i].ToLower();
            }
            return result;
        }

        static Dictionary<string,int> LoadDict()
        {
            using var inputDict = new StreamReader("dict.txt");
            string inp;
            var res = new Dictionary<string,int>();
            while ((inp = inputDict.ReadLine()) != "" && inp != null)
            {
                res[inp.Split()[0]] = Int32.Parse(inp.Split()[1]);
            }

            return res;
        }

        static string[] MakeCalculation(string[] wordForms, Dictionary<string,int> dictionary)
        {
            int wordFormsCount = wordForms.Length;
            string[] differentWordForms = wordForms.Distinct().ToArray();
            int differentWordFormsCount = differentWordForms.Length;
            int countInDict = 0;
            var potentialMistakes = new List<string>();
            foreach (string word in differentWordForms)
            {
                if (dictionary.Keys.Contains(word))
                {
                    countInDict += 1;
                }
                else if (!(potentialMistakes.Contains(word)))
                {
                    potentialMistakes.Add(word);
                }
            }

            Console.WriteLine($"Word forms count - {wordFormsCount}");
            Console.WriteLine($"Diferent word forms count - {differentWordFormsCount}");
            Console.WriteLine($"My word forms count in dictionary - {countInDict}");

            return potentialMistakes.ToArray();
        }

        static int LevinshteinInstruction(string S1, string S2)
        {
            int M  = S1.Length;
            int N = S2.Length;
            var D = new int[M+1, N+1];
            D[0, 0] = 0;
            for (int j = 1; j <= N; j++)
            {
                D[0, j] = D[0,j-1]+1;
            }

            for (int i = 1; i <= M; i++)
            {
                D[i, 0] = D[i - 1, 0]+1;
                for (int j = 1; j < N; j++)
                {
                    if (S1[i-1] != S2[j-1])
                    {
                        D[i, j] = Math.Min(D[i - 1, j], Math.Min(D[i, j - 1], D[i - 1, j - 1]))+1;
                    }
                    else
                    {
                        D[i, j] = D[i - 1,j - 1];
                    }
                }
            }

            return D[M - 1, N - 1];
        }

        static void Main(string[] args)
        {
            Dictionary<string, int> dict = LoadDict();

            string rawText;
            using (var inputText = new StreamReader("text.txt", Encoding.UTF8))
            {
                rawText = inputText.ReadToEnd();
            }
            string[] wordForms = ParseText(rawText);
            string[] posibleMistakes = MakeCalculation(wordForms, dict);

            var mistakes = new Dictionary<string, (string, int)>();
            foreach (string word  in posibleMistakes)
            {
                int minLen = Int32.MaxValue;
                string newWord = "";
                int len;
                int freq = 0;
                foreach (string key in dict.Keys)
                {
                    len = LevinshteinInstruction(word, key);
                    if (len < minLen || (len==minLen && dict[key] > freq))
                    {
                        minLen = len;
                        newWord = key;
                        freq = dict[key];
                    }
                }

                if (minLen < 3)
                {
                    mistakes[word] = (newWord,minLen);
                }
                else
                {
                    mistakes[word] = ("Not found",-1);
                }
            }

            string[] sortKey = mistakes.Keys.ToArray().OrderBy(x => wordForms.Select(z=>x==z).Count()).ToArray();


            foreach (string word in mistakes.Keys)
            {
                if (mistakes[word].Item2 != -1)
                {
                    wordForms = wordForms.Select(x => x.Replace(word,mistakes[word].Item1)).ToArray();
                }
            }

            Console.WriteLine("----------");
            Console.WriteLine($"Not in dictionary - {posibleMistakes.Length} words");
            Console.WriteLine("----------");
            Console.WriteLine("After fixing mistakes:");
            MakeCalculation(wordForms,dict);
            Console.WriteLine("----------");
            Console.WriteLine("All mistakes:");
            for (int i = 0; i < sortKey.Length; i++)
            {
                Console.Write($"{i + 1}) {sortKey[i]} - {mistakes[sortKey[i]].Item1}");
                if (mistakes[sortKey[i]].Item2 != -1)
                {
                    Console.Write($" - {mistakes[sortKey[i]].Item2}");
                }
                Console.WriteLine();
                if (mistakes[sortKey[i]].Item1 != "Not found")
                {
                    rawText = rawText.Replace(sortKey[i], mistakes[sortKey[i]].Item1);
                }
            }

            using (var outputFile = new StreamWriter("resText.txt"))
            {
                outputFile.Write(rawText);
            }

        }
    }
}
