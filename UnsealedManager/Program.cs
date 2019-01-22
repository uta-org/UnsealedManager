using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            //Console.WriteLine($"Number of files: {files.Length}");

            var classes = files.Select(file => GetClassesInFile(file)).SelectMany(c => c).ToList();

            //Console.WriteLine($"Number of classes: {classes.Count}");

            //string fullMsg = string.Join(Environment.NewLine, classes.Select(tuple => $"{tuple.Item1}{(tuple.Item2.HasContent(string.Empty, " ") ? " : " + string.Join(", ", tuple.Item2) + " " : " ")}(on {Path.GetFileName(tuple.Item3)})"));
            //Console.WriteLine(fullMsg);

            //var unsealedClasses = files.Select(file => !classes.Any(cName => Regex.IsMatch(File.ReadAllText(file), $": {cName}")) ? GetClassesInFile(file) : null).SelectMany(c => c).Where(s => !string.IsNullOrEmpty(s));
            var unsealedClasses = classes.Where(c => !classes.Any(subClass => subClass.Item2.Contains(c.Item1))).ToList();
            //var sealedClasses = classes.Where(c => classes.Any(subClass => subClass.Item2.Contains(c.Item1))).ToList();

            StringBuilder sb = new StringBuilder($"Unsealed classes count: {unsealedClasses.Count} ({string.Join(", ", unsealedClasses.Select(c => c.Item1))})");
            sb.AppendLine($"What do you wish to do with this all {unsealedClasses.Count} unsealed classes?");

            Console.WriteLine(sb.ToString());

            //Console.WriteLine($"Can't be sealed classes count: {sealedClasses.Count}: {string.Join(", ", sealedClasses)}");

            //unsealedClasses.ForEach(c => Console.WriteLine(c));

            Console.Read();
        }

        private static IEnumerable<Tuple<string, string[], string>> GetClassesInFile(string file)
        {
            return Regex.Matches(File.ReadAllText(file), $"(public|internal|private) class (?<{ClassName}>(.+?))( : (?<{InheritedClass}>(.+?))|)\n")
                .Cast<Match>()
                .Select(m => new Tuple<string, string[], string>(GetGroupValue(m, ClassName), GetGroupValue(m, InheritedClass).CleanLineBreak().Split(','), file))
                .Where(t => !string.IsNullOrEmpty(t.Item1));
        }

        private static string GetGroupValue(Match m, string groupName)
        {
            return m.Groups[groupName].Value.ToString().Trim();
        }
    }
}