using System;
using System.IO;
using System.Xml;

namespace CoverDiff
{
    public class ProgramArgs
    {
        public bool AreValid { get; private set; }
        public string Errors { get; private set; }

        public string CurrentFile { get; private set; }
        public string PrevFile { get; private set; }

        public ProgramArgs(string[] commandLineArgs)
        {
            Errors = "";
            if (commandLineArgs.Length < 2)
            {
                var assemblyName = typeof(Program).Assembly.GetName();
                Errors += string.Format("{0} Version {1}\nUsage: {0} current.xml prev.xml", assemblyName.Name, assemblyName.Version);
            }
            CurrentFile = commandLineArgs[0];
            if (!File.Exists(CurrentFile))
            {
                Errors += string.Format("Couldn't find file {0}", CurrentFile);
            }
            PrevFile = commandLineArgs[1];
            if (!File.Exists(PrevFile))
            {
                Errors += string.Format("Couldn't find file {0}", PrevFile);
            }
            AreValid = string.IsNullOrEmpty(Errors);
        }
    }

    class Program
    {
        static void Main(string[] commandLineArgs)
        {
            var args = new ProgramArgs(commandLineArgs);

            if (!args.AreValid)
            {
                Console.WriteLine(args.Errors);
                return;
            }

            var currentCoverage = new CoverageFile(args.CurrentFile, new XmlTextReader(new FileStream(args.CurrentFile, FileMode.Open)));
            var prevCoverage = new CoverageFile(args.PrevFile, new XmlTextReader(new FileStream(args.PrevFile, FileMode.Open)));

            using(var htmlFileStream = new FileStream("coverdiff.html", FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var comparer = new HtmlReportGenerator(htmlFileStream);
                comparer.Report(new CoverageComparer(currentCoverage, prevCoverage));
            }
        }
    }
}
