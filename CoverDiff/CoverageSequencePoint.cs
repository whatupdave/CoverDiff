using System;
using System.Xml.Linq;

namespace CoverDiff
{
    internal class CoverageSequencePoint
    {
        public CoverageLine ParentLine { get; set; }
        public int VisitCount { get; private set; }

        public bool Unvisited { get; private set; }

        public int StartColumn { get; private set; }
        public int EndColumn { get; private set; }
        public int LineNumber { get; private set; }

        public int DocumentId { get; private set; }

        private readonly XElement _seqPointElement;

        public CoverageSequencePoint(CoverageLine parentLine, XElement seqPointElement)
        {
            ParentLine = parentLine;
            _seqPointElement = seqPointElement;
            VisitCount = int.Parse(seqPointElement.Attribute("vc").Value);
            StartColumn = int.Parse(seqPointElement.Attribute("c").Value);
            EndColumn = int.Parse(seqPointElement.Attribute("ec").Value);
            LineNumber = int.Parse(seqPointElement.Attribute("l").Value);
            DocumentId = int.Parse(seqPointElement.Attribute("doc").Value);
            Unvisited = VisitCount == 0;
        }

        public override string ToString()
        {
            return _seqPointElement.ToString();
        }
    }
}