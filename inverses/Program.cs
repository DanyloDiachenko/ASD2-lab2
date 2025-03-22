using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Inverses
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Input filename was not provided");
                return;
            }

            string inputFileName = args[0];
            if (!File.Exists(inputFileName))
            {
                Console.WriteLine("File was not found");
                return;
            }

            int users, movies;
            Dictionary<int, int[]> userPreferences = new Dictionary<int, int[]>();

            using (StreamReader sr = new StreamReader(inputFileName))
            {
                string firstLine = sr.ReadLine();
                string[] tokens = firstLine.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                users = int.Parse(tokens[0]);
                movies = int.Parse(tokens[1]);

                for (int i = 0; i < users; i++)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        break;

                    string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    int userId = int.Parse(parts[0]);
                    int[] prefs = new int[movies];
                    for (int j = 0; j < movies; j++)
                    {
                        prefs[j] = int.Parse(parts[j + 1]);
                    }
                    userPreferences[userId] = prefs;
                }
            }

            Console.Write("Enter user number: ");
            int x;
            while (!int.TryParse(Console.ReadLine(), out x) || !userPreferences.ContainsKey(x))
            {
                Console.WriteLine("User with index not found:");
            }

            int[] refList = userPreferences[x];

            Dictionary<int, int> filmRank = new Dictionary<int, int>();
            for (int i = 0; i < movies; i++)
            {
                filmRank[refList[i]] = i;
            }

            List<Tuple<int, long>> similarityList = new List<Tuple<int, long>>();
            foreach (var kvp in userPreferences)
            {
                int userId = kvp.Key;
                if (userId == x)
                    continue;

                int[] currentList = kvp.Value;
                int[] rankArray = new int[movies];
                for (int i = 0; i < movies; i++)
                {
                    rankArray[i] = filmRank[currentList[i]];
                }

                long inversions = CountInversions(rankArray);
                similarityList.Add(new Tuple<int, long>(userId, inversions));
            }

            similarityList.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            string outputFileName = "output.txt";

            using (StreamWriter sw = new StreamWriter(outputFileName))
            {
                sw.WriteLine(x);
                foreach (var pair in similarityList)
                {
                    sw.WriteLine("{0} {1}", pair.Item1, pair.Item2);
                }
            }

            Console.WriteLine($"Result written to: {outputFileName}");
        }

        static long CountInversions(int[] arr)
        {
            int[] aux = new int[arr.Length];
            return MergeSortAndCount(arr, aux, 0, arr.Length - 1);
        }

        static long MergeSortAndCount(int[] arr, int[] aux, int left, int right)
        {
            if (left >= right)
                return 0;

            int mid = (left + right) / 2;
            long count = 0;
            count += MergeSortAndCount(arr, aux, left, mid);
            count += MergeSortAndCount(arr, aux, mid + 1, right);
            count += Merge(arr, aux, left, mid, right);
            return count;
        }

        static long Merge(int[] arr, int[] aux, int left, int mid, int right)
        {
            for (int k = left; k <= right; k++)
            {
                aux[k] = arr[k];
            }

            long inversions = 0;
            int i = left;
            int j = mid + 1;
            int kIndex = left;

            while (i <= mid && j <= right)
            {
                if (aux[i] <= aux[j])
                {
                    arr[kIndex++] = aux[i++];
                }
                else
                {
                    arr[kIndex++] = aux[j++];
                    inversions += (mid - i + 1);
                }
            }
            while (i <= mid)
            {
                arr[kIndex++] = aux[i++];
            }

            return inversions;
        }
    }
}
