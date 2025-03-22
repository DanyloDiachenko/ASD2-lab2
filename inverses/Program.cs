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

            using (StreamReader streamReader = new StreamReader(inputFileName))
            {
                string[] tokens = streamReader.ReadLine().Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                users = int.Parse(tokens[0]);
                movies = int.Parse(tokens[1]);

                for (int i = 0; i < users; i++)
                {
                    string line = streamReader.ReadLine();
                    if (line == null)
                        break;

                    string[] parts = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                    int[] preferences = new int[movies];

                    for (int j = 0; j < movies; j++)
                    {
                        preferences[j] = int.Parse(parts[j + 1]);
                    }

                    int userIndex = int.Parse(parts[0]);

                    userPreferences[userIndex] = preferences;
                }
            }

            Console.Write("Enter user number: ");
            int referenceUserIndex;
            while (!int.TryParse(Console.ReadLine(), out referenceUserIndex) || !userPreferences.ContainsKey(referenceUserIndex))
            {
                Console.WriteLine("User with index not found:");
            }

            int[] referenceList = userPreferences[referenceUserIndex];

            Dictionary<int, int> filmRank = [];
            for (int i = 0; i < movies; i++)
            {
                filmRank[referenceList[i]] = i;
            }

            List<Tuple<int, long>> similarityList = [];
            foreach (var pair in userPreferences)
            {
                int userIndex = pair.Key;
                if (userIndex == referenceUserIndex)
                    continue;

                int[] currentList = pair.Value;
                int[] rankArray = new int[movies];
                for (int i = 0; i < movies; i++)
                {
                    rankArray[i] = filmRank[currentList[i]];
                }

                long inversions = CountInversions(rankArray);
                similarityList.Add(new Tuple<int, long>(userIndex, inversions));
            }

            similarityList.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            string outputFileName = "output.txt";

            using (StreamWriter streamWriter = new StreamWriter(outputFileName))
            {
                streamWriter.WriteLine(referenceUserIndex);
                foreach (var pair in similarityList)
                {
                    streamWriter.WriteLine("{0} {1}", pair.Item1, pair.Item2);
                }
            }

            Console.WriteLine($"Result written to: {outputFileName}");
        }

        static long CountInversions(int[] arr)
        {
            int[] tempArr = new int[arr.Length];

            return MergeSortAndCount(arr, tempArr, 0, arr.Length - 1);
        }

        static long MergeSortAndCount(int[] arr, int[] tempArr, int left, int right)
        {
            if (left >= right)
                return 0;

            int mid = (left + right) / 2;
            long count = 0;

            count += MergeSortAndCount(arr, tempArr, left, mid);
            count += MergeSortAndCount(arr, tempArr, mid + 1, right);
            count += Merge(arr, tempArr, left, mid, right);

            return count;
        }

        static long Merge(int[] arr, int[] tempArr, int left, int mid, int right)
        {
            for (int k = left; k <= right; k++)
            {
                tempArr[k] = arr[k];
            }

            long inversions = 0;
            int i = left;
            int j = mid + 1;
            int kIndex = left;

            while (i <= mid && j <= right)
            {
                if (tempArr[i] <= tempArr[j])
                {
                    arr[kIndex++] = tempArr[i++];
                }
                else
                {
                    arr[kIndex++] = tempArr[j++];
                    inversions += (mid - i + 1);
                }
            }
            while (i <= mid)
            {
                arr[kIndex++] = tempArr[i++];
            }

            return inversions;
        }
    }
}
