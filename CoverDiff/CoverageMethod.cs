using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CoverDiff
{
    internal class CoverageMethod
    {
        public CoverageClass ParentClass { get; set; }
        public string Name { get; set; }
        
        public IEnumerable<CoverageLine> Lines { get; private set; }

        public int UnvisitedPoints { get; private set; }

        public CoverageMethod(CoverageClass parentClass, XElement methodElement)
        {
            ParentClass = parentClass;
            Name = methodElement.Attribute("name").Value;
            var lineSequences = from s in methodElement.Descendants("seqpnt")
                                let line = int.Parse(s.Attribute("l").Value)
                                where line > 0
                                group s by line;

            Lines = from line in lineSequences
                    select new CoverageLine(this, line.Key, line);
                    
        }

        public override string ToString()
        {
            return string.Format("{0}=>{1}", ParentClass.Name, Name);
        }

        public CoverageLine this[int lineNumber]
        {
            get { return Lines.FirstOrDefault(l => l.LineNumber == lineNumber); }
        }

        public string GetSourceLine(int line)
        {
            return ParentClass.GetSourceFile()
        }
    }
}