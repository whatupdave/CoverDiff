using System.Collections.Generic;

namespace CoverDiff
{
    internal class LineComparer : IEqualityComparer<CoverageLine>
    {
        public bool Equals(CoverageLine x, CoverageLine y)
        {
            return Equals(x.LineNumber, y.LineNumber);
        }

        public int GetHashCode(CoverageLine obj)
        {
            return obj.LineNumber.GetHashCode();
        }
    }
}