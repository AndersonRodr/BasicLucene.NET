using System;

namespace ConsoleAppElasticSearch
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Beginning Index");
            BasicSearchEngine.Index();
            Console.WriteLine("Completed Index");
            Console.WriteLine("Beginning Data");
            BasicSearchEngine.GetData();
            Console.WriteLine("Beginning Index Update");
            BasicSearchEngine.AddToIndex();
            BasicSearchEngine.ChangeInIndex();
            BasicSearchEngine.DeleteFromIndex();
            Console.WriteLine("Completed Index Update");
            var run = true;
            while (run)
            {
                Console.WriteLine($"{Environment.NewLine} Enter a Person Description To Search  or Just Press Enter to End Program:");
                run = RunSearch();
            }
        }

        private static bool RunSearch()
        {
            var input = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(input))
            {
                BasicSearchEngine.Dispose();
                return false;
            }

            foreach (var s in BasicSearchEngine.Search(input))
            {
                Console.WriteLine(s);
            }

            return true;
        }
    }
}
