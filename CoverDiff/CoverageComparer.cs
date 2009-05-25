using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoverDiff
{
    internal class ComparedClass
    {
        private readonly CoverageClass _currentClass;
        private readonly CoverageClass _prevClass;

        public string Name { get; private set; }
        public CoverageFile ParentFile { get; set; }
        public IEnumerable<ComparedMethod> ComparedMethods { get; set; }


        public ComparedClass(CoverageFile parentFile, CoverageClass current, CoverageClass prev)
        {
            ParentFile = parentFile;
            _currentClass = current;
            _prevClass = prev;

            Name = current.Name;

            if (_prevClass != null)
                ComparedMethods = from m in current.Methods
                                  select new ComparedMethod(this, m, _prevClass[m.Name]);
        }
    }

    public enum LineState
    {
        NoChange,
        Covered,
        Uncovered
    }

    internal class ComparedLine
    {
        public ComparedMethod ParentMethod { get; private set; }
        public CoverageLine CurrentLine { get; private set; }
        public CoverageLine PrevLine { get; private set; }

        public LineState State { get; private set; }
        public int LineNumber { get; private set; }

        public ComparedLine(ComparedMethod parentMethod, CoverageLine currentLine, CoverageLine prevLine)
        {
            ParentMethod = parentMethod;
            CurrentLine = currentLine;
            PrevLine = prevLine;
            LineNumber = currentLine.LineNumber;

            State = LineState.NoChange;
            if (currentLine.WasVisited && !prevLine.WasVisited)
                State = LineState.Covered;
            else if (!currentLine.WasVisited && prevLine.WasVisited)
                State = LineState.Uncovered;
        }
    }

    internal class ComparedMethod
    {
        public ComparedClass ParentClass { get; set; }
        public CoverageMethod CurrentMethod { get; set; }
        public CoverageMethod PrevMethod { get; set; }

        public IEnumerable<ComparedLine> ComparedLines { get; private set; }

        public ComparedMethod(ComparedClass parentClass, CoverageMethod currentMethod, CoverageMethod prevMethod)
        {
            ParentClass = parentClass;
            CurrentMethod = currentMethod;
            PrevMethod = prevMethod;

            if (PrevMethod != null)
                ComparedLines = from m in CurrentMethod.Lines
                                  select new ComparedLine(this, m, PrevMethod[m.LineNumber]);
        }
    }

    internal class CoverageComparer
    {
        private readonly CoverageFile _current;
        private readonly CoverageFile _prev;

        public int CurrentPoints { get; private set; }
        public int PrevPoints { get; private set; }
        public int CoveredPoints { get; private set; }

        public IEnumerable<ComparedClass> ComparedClasses { get; private set; }

        public CoverageComparer(CoverageFile current, CoverageFile prev)
        {
            _current = current;
            _prev = prev;

            PrevPoints = prev.UnvisitedPoints;
            CurrentPoints = current.UnvisitedPoints;
            CoveredPoints = PrevPoints - CurrentPoints;

            ComparedClasses = from c in _current.Classes
                              select new ComparedClass(current, c, _prev.Classes[c.Name]);

//            if (current.UnvisitedPoints < prev.UnvisitedPoints)
//            {
//                ReportLine(ConsoleColor.Green, "You have covered {0} more points ({1} local vs {2} prev)", prev.UnvisitedPoints - current.UnvisitedPoints, current.UnvisitedPoints, prev.UnvisitedPoints);
//            }
//            else if (current.UnvisitedPoints > prev.UnvisitedPoints)
//            {
//                ReportLine(ConsoleColor.Red, "You have uncovered {0} points ({1} local vs {2} prev)", current.UnvisitedPoints - prev.UnvisitedPoints, current.UnvisitedPoints, prev.UnvisitedPoints);
//
//                CompareClasses();
//            }
        }



//        private void CompareClasses()
//        {
//            foreach (var currentClass in _current.Classes)
//            {
//                var prevClass = _prev.Classes[currentClass.Name];
//                if (prevClass == null)
//                {
//                    ReportLine(ConsoleColor.Green, "+ {0} (#{1})", currentClass.Name, currentClass.UnvisitedPoints);
//                }
//                else if (currentClass.UnvisitedPoints > prevClass.UnvisitedPoints)
//                {
//                    ReportLine(ConsoleColor.Green, "{0} (#{1})", currentClass.Name, currentClass.UnvisitedPoints - prevClass.UnvisitedPoints);
//                    CompareMethods(currentClass, prevClass);
//                }
//            }
//        }

//        private void CompareMethods(CoverageClass currentClass, CoverageClass prevClass)
//        {
//            foreach (var currentMethod in currentClass.Methods)
//            {
//                var prevMethod = prevClass[currentMethod.Name];
//                if (prevMethod == null)
//                {
//                    ReportLine(ConsoleColor.White, "  + {0} (#{1})", currentMethod.Name, currentMethod.UnvisitedPoints);
//                }
//                else if (currentMethod.UnvisitedPoints > prevMethod.UnvisitedPoints)
//                {
//                    ReportLine(ConsoleColor.White, "  {0} (#{1})", currentMethod.Name, currentMethod.UnvisitedPoints - prevMethod.UnvisitedPoints);
//
//                    CompareLines(currentMethod);
//                }
//            }
//        }

        private void CompareLines(CoverageMethod currentMethod)
        {
            foreach (var line in currentMethod.Lines)
            {
                var srcFileName = _current.Documents[line.DocumentId].Url;
                using (var srcFileStream = new FileStream(srcFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var srcFile = new StreamReader(srcFileStream))
                {
                    int i = 0;
                    while (i < line.LineNumber - 1 && !srcFile.EndOfStream)
                    {
                        i++;
                        srcFile.ReadLine();
                    }
                }
            }
        }

        private static void ReportLine(ConsoleColor color, string message, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message, args);
            }
            finally
            {
                Console.ForegroundColor = oldColor;
            }
        }

        private static void Report(ConsoleColor color, string message, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                Console.Write(message, args);
            }
            finally
            {
                Console.ForegroundColor = oldColor;
            }
        }
    }
}