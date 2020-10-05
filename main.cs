using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;

namespace ImageProcessing
{ 
    class Program
    {
        struct TreeNode
        {
            public List<int> kids;
            public int key;
            public double probabilty;
            public string code;
        }

        static Image myImage;
        static void Main(string[] args)
        {
            myImage = Image.FromFile("image.png");
            Bitmap imageBitmap = new Bitmap(myImage);
            int width = myImage.Width;
            int height = myImage.Height;
            int[] message = new int[width];
            for (int i =0;i<width;i++)
            {
                message[i] = imageBitmap.GetPixel(i, height / 2).R/20*20;
            }
            
            ImmutableSortedSet<int> alphabet = message.ToImmutableSortedSet();
            
            Dictionary<int, int> frequency = new Dictionary<int, int>();
            for (int i = 0; i<alphabet.Count;i++)
            {
                frequency[alphabet[i]] = message.Count(j => j==alphabet[i]);
            }

            Dictionary<int, double> probability = new Dictionary<int, double>();

            for (int i =0;i<alphabet.Count;i++)
            {
                probability[alphabet[i]] = (double)message.Count(j => j==alphabet[i])/(double)message.Length;
            }

            double entropy = 0;
            for (int i=0;i<alphabet.Count;i++)
            {
                entropy -= probability[alphabet[i]] * Math.Log2(probability[alphabet[i]]);
            }

            int lenAlphabet = alphabet.Count;
            int minLenEqBinaryCode = (int)Math.Ceiling(Math.Log2(lenAlphabet));

            Dictionary<int, string> eqBinaryCode = GetEqBinaryCode(alphabet.ToArray());
            Dictionary<int, double> sortedProbability  = probability.OrderBy(x => -x.Value).ToDictionary(x => x.Key, x => x.Value);

            Dictionary<int, string> shannonCode = new Dictionary<int, string>();

            shannonCode = GetShannonCode(sortedProbability,"",new Dictionary<int, string>());

            sortedProbability = probability.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            List<TreeNode> nodes = new List<TreeNode>();
            int counter= 0;
            foreach (int key in sortedProbability.Keys)
            {
                TreeNode node = new TreeNode();
                node.key = key;
                node.probabilty = sortedProbability[key];
                node.kids = new List<int>();
                nodes.Add(node);
                counter++;
            }
            nodes = nodes.OrderBy(it => it.probabilty).ToList();
            Dictionary<int, string> Result = new Dictionary<int, string>();
            for (int i = 0; i<alphabet.Count;i++)
            {
                Result[alphabet[i]] = "";
            }
            Dictionary<int, string> haffmanCode = GetHaffmanCode(nodes,Result);

            double midLenShannon = 0;
            foreach (int key in shannonCode.Keys)
            {
                midLenShannon += shannonCode[key].Length * probability[key];
            }

            double midLenHaffman = 0;
            foreach (int key in shannonCode.Keys)
            {
                midLenHaffman += haffmanCode[key].Length * probability[key];
            }

            string[] messageCoded = new string[message.Length];

            int sizeCoded = minLenEqBinaryCode * message.Length;

            for (int i=0;i<message.Length;i++)
            {
                messageCoded[i] = eqBinaryCode[message[i]];
            }

            string[] messageShannon = new string[message.Length];

            int sizeShannon = 0;

            for (int i = 0; i < message.Length; i++)
            {
                messageShannon[i] = shannonCode[message[i]];
                sizeShannon += shannonCode[message[i]].Length;
            }

            string[] messageHaffman = new string[message.Length];

            int sizeHaffman = 0;

            for (int i = 0; i < message.Length; i++)
            {
                messageHaffman[i] = haffmanCode[message[i]];
                sizeHaffman += haffmanCode[message[i]].Length;
            }

            double compressionShannon = (double)sizeShannon / sizeCoded;
            double compressionHaffman = (double)sizeHaffman / sizeCoded;

            double excessivenessShannon = 1- (entropy/midLenShannon);
            double excessivenessHaffman = 1-(entropy/midLenHaffman);


            Console.WriteLine($"Message: {String.Join(" ",message)}");
            Console.WriteLine();
            Console.WriteLine($"Alphabe: - {String.Join(" ", alphabet)}");
            Console.WriteLine();
            Console.WriteLine("Frequency of items:");
            foreach (int key in frequency.Keys)
            {
                Console.WriteLine($"{key} : {frequency[key]}");
            }
            Console.WriteLine("\nProbability of items:");
            foreach (int key in probability.Keys)
            {
                Console.WriteLine($"{key} : {probability[key]}");
            }

            Console.WriteLine();

            Console.WriteLine($"Entropy - {entropy}");
            Console.WriteLine($"Len of alphabet - {lenAlphabet}");
            Console.WriteLine($"Minimal length of length-equal binary code - {minLenEqBinaryCode}");
            Console.WriteLine("Length-equal binary code:");
            foreach (int key in eqBinaryCode.Keys)
            {
                Console.WriteLine($"{key} : {eqBinaryCode[key]}");
            }
            Console.WriteLine($"\nMessage size in len-equal code: {sizeCoded}");
            Console.WriteLine($"Message coded in len-equal binary code:{String.Join(" ",messageCoded)}");

            Console.WriteLine();

            Console.WriteLine("Shannon-Fano code:");
            foreach (int key in shannonCode.Keys)
            {
                Console.WriteLine($"{key} : {shannonCode[key]}");
            }
            Console.WriteLine($"Shannon-Fano mid length: {midLenShannon}");
            Console.WriteLine($"Message size in Shannon-Fano code: {sizeShannon}");
            Console.WriteLine($"Message coded in Shannon-Fano code:{String.Join(" ", messageShannon)}");
            Console.WriteLine($"Shannon code compression: {compressionShannon}");
            Console.WriteLine($"Shannon code excessiveness: {excessivenessShannon}");

            Console.WriteLine();

            Console.WriteLine("Haffman code:");
            foreach (int key in haffmanCode.Keys)
            {
                Console.WriteLine($"{key} : {haffmanCode[key]}");
            }
            Console.WriteLine($"Haffman mid length: {midLenHaffman}");
            Console.WriteLine($"Message size in Haffman code: {sizeHaffman}");
            Console.WriteLine($"Message coded in Haffman code:{String.Join(" ", messageHaffman)}");
            Console.WriteLine($"Haffman code compression: {compressionHaffman}");
            Console.WriteLine($"Haffman code excessiveness: {excessivenessHaffman}");
        }

        private static Dictionary<int, string> GetHaffmanCode(List<TreeNode> nodes, Dictionary<int, string> res)
        {
            if (nodes.Count<2)
            {
                return res;
            }
            TreeNode firstNode = nodes.First();
            nodes.Remove(firstNode);
            TreeNode secondNode = nodes.First();
            nodes.Remove(secondNode);
            foreach (int kid in secondNode.kids)
            {
                res[kid] = "1" + res[kid];
            }
            foreach (int kid in firstNode.kids)
            {
                res[kid] = "0" + res[kid];
                secondNode.kids.Add(kid);
            }
            res[firstNode.key] = "0" + res[firstNode.key];
            res[secondNode.key] = "1" + res[secondNode.key];
            secondNode.kids.Add(firstNode.key);
            secondNode.probabilty += secondNode.probabilty;
            nodes.Add(secondNode);
            nodes = nodes.OrderBy(it => it.probabilty).ToList();
            res = GetHaffmanCode(nodes, res);
            return res;
        }

        private static Dictionary<int, string> GetShannonCode(Dictionary<int,double> alphabet,string code, Dictionary<int,string> nowResult)
        {
            if (alphabet.Keys.Count<2) {
                nowResult[alphabet.Keys.ToArray()[0]] = code;
                return nowResult;
            }
            Dictionary<int, double> left = new Dictionary<int, double>();
            Dictionary<int, double> right = new Dictionary<int, double>();
            double x = 0;
            bool leftAdd = true;
            double allProbability = alphabet.Values.Sum();
            foreach (int i in alphabet.Keys)
            {
                if (leftAdd)
                {
                    left[i] = alphabet[i];
                } else
                {
                    right[i] = alphabet[i];
                }
                
                x += alphabet[i];
                if (allProbability-2*x<0.1)
                {
                    leftAdd = false;
                }
            }
            nowResult = GetShannonCode(left, code+"0", nowResult);
            nowResult = GetShannonCode(right, code + "1", nowResult);
            return nowResult;
        }

        private static Dictionary<int, string> GetEqBinaryCode(int[] alphabet) 
        {
            int minLen = (int)Math.Ceiling(Math.Log2(alphabet.Length));
            Dictionary<int, string> result = new Dictionary<int, String>();
            for (int i=0;i<alphabet.Length;i++)
            {
                string numBin = GetNumBin(i);
                while (numBin.Length < minLen)
                {
                    numBin = "0" + numBin;
                }
                result[alphabet[i]] = numBin;
            }
            return result;
            
        }

        private static string GetNumBin(int x)
        {
            string res = "";
            while (x>0)
            {
                res = (x % 2).ToString()+res;
                x = x/2;
            }
            return res!= "" ? res : "0";
        }
    }
}
