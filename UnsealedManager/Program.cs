using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnsealedManager
{
    internal class Program
    {
        private static string StaticPath => "D:/UNITY/Unity Projects/free-rider-2-port/Assets/Scripts/";

        private const string ClassName = "ClassName",
                             InheritedClass = "InheritedClass";

        private static void Main(string[] args)
        {
            var files = Directory.GetFiles(StaticPath, "*.cs", SearchOption.AllDirectories);

            Console.WriteLine($"Number of files: {files.Length}");

            var classes = files.Select(file => GetClassesInFile(file)).SelectMany(c => c).ToList();

            Console.WriteLine($"Number of classes: {classes.Count}");

            classes.ForEach(tuple => Console.WriteLine($"{tuple.Item1}{(tuple.Item2.HasContent(string.Empty, " ") ? " : " + string.Join(", ", tuple.Item2) + " " : " ")}(on {Path.GetFileName(tuple.Item3)})"));

            //var unsealedClasses = files.Select(file => !classes.Any(cName => Regex.IsMatch(File.ReadAllText(file), $": {cName}")) ? GetClassesInFile(file) : null).SelectMany(c => c).Where(s => !string.IsNullOrEmpty(s));
            var unsealedClasses = classes.Where(c => !classes.Any(subClass => subClass.Item2.Contains(c.Item1))).ToList();

            Console.WriteLine($"Unsealed classes count: {unsealedClasses.Count}");

            //unsealedClasses.ForEach(c => Console.WriteLine(c));

            Console.Read();
        }

        private static IEnumerable<Tuple<string, string[], string>> GetClassesInFile(string file)
        {
            return Regex.Matches(File.ReadAllText(file), $"(public|internal|private) class (?<{ClassName}>(.+?))( : (?<{InheritedClass}>(.+?))|)\n")
                .Cast<Match>()
                .Select(m => new Tuple<string, string[], string>(GetGroupValue(m, ClassName), GetGroupValue(m, InheritedClass).CleanLineBreak().Split(','), file));
        }

        private static string GetGroupValue(Match m, string groupName)
        {
            return m.Groups[groupName].Value.ToString();
        }
    }
}