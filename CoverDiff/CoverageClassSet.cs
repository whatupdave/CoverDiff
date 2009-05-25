using System.Collections.Generic;
using System.Linq;

namespace CoverDiff
{
    internal class CoverageClassSet : List<CoverageClass>
    {
        public CoverageClassSet(IEnumerable<CoverageClass> classes) : base(classes)
        {
        }

        public CoverageClass this[string className]
        {
            get { return this.FirstOrDefault(c => c.Name == className); }
        }
    }
}