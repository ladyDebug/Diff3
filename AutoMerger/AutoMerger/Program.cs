using System;
using System.IO;
using System.Linq;

namespace AutoMerger
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Please, set path to the base file:");
            string pathBase = Console.ReadLine();
            if (pathBase != null && File.Exists(pathBase))
            {

                Console.WriteLine("Please, set path to the right file:");
                string pathFile1 = Console.ReadLine();
                if (pathFile1 != null && File.Exists(pathFile1))
                {

                    Console.WriteLine("Please, set path to the left file:");
                    string pathFile2 = Console.ReadLine();

                    Console.WriteLine(@"Please, set path to the result file:");
                    string resultFile = Console.ReadLine();
                    if (string.IsNullOrEmpty(resultFile))
                    {
                        return;
                    }

                    if (!string.IsNullOrEmpty(pathFile2) && File.Exists(pathFile2))
                    {
                        string[] baseLines = File.ReadLines(pathBase).ToArray();
                        string[] file1Lines = File.ReadLines(pathFile1).ToArray();
                        string[] file2Lines = File.ReadLines(pathFile2).ToArray();


                        Merger merger = new Merger(new HuntSzymanskiLcs());
                        var result = merger.MergeThreeWay(baseLines, file1Lines, file2Lines);


                        using (StreamWriter file = new StreamWriter(resultFile, false))
                        {

                            foreach (var resultItem in result)
                            {
                                if (!resultItem.IsConflicted)
                                {
                                    foreach (var line in resultItem.Line)
                                    {
                                        file.WriteLine(line);
                                    }
                                }
                                else
                                {
                                    file.WriteLine("Conflict left file");
                                    foreach (var line in resultItem.LeftLine)
                                    {
                                        file.WriteLine(line);
                                    }
                                    file.WriteLine("End conflict left file");

                                    file.WriteLine("Conflict base file");
                                    foreach (var line in resultItem.BaseLine)
                                    {
                                        file.WriteLine(line);
                                    }
                                    file.WriteLine("End conflict base file");

                                    file.WriteLine("Conflict right file");
                                    foreach (var line in resultItem.RightLine)
                                    {
                                        file.WriteLine(line);
                                    }
                                    file.WriteLine("End conflict right file");
                                }
                            }
                        }

                    }
                }
            }

        }
    }
}
