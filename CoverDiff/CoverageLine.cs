using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CoverDiff
{
    internal class CoverageLine
    {
        public CoverageMethod ParentMethod { get; set; }
        public int LineNumber { get; private set; }
        protected IEnumerable<CoverageSequencePoint> SequencePoints { get; set; }

        public string Code
        {
            get { return ParentMethod.GetSourceLine(LineNumber); }
        }

        public bool WasVisited
        {
            get { return VisitCount > 0; }
        }

        protected int VisitCount
        {
            get { return SequencePoints.Sum(pt => pt.VisitCount); }
        }


        public CoverageLine(CoverageMethod parentMethod, int lineNumber, IEnumerable<XElement> sequenceElements)
        {
            ParentMethod = parentMethod;
            LineNumber = lineNumber;
            SequencePoints = from pt in sequenceElements
                             select new CoverageSequencePoint(this, pt);
        }
    }
}