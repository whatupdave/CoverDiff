using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace CoverDiff
{
    internal class CoverageFile
    {
        public string FileName { get; set; }
        public CoverageClassSet Classes { get; private set; }

        public int UnvisitedPoints { get; private set; }

        public DocumentFileSet Documents { get; private set; }

        public CoverageFile(string fileName, XmlReader reader)
        {
            FileName = fileName;
            var doc = XDocument.Load(reader);
            Classes = new CoverageClassSet(from c in doc.Descendants("class")
                                           where c.Attribute("excluded").Value == "false"
                                           select new CoverageClass(this, c));

            Documents = new DocumentFileSet(from d in doc.Descendants("doc")
                                            select new CoverageDocument(d));
            UnvisitedPoints = Classes.Sum(c => c.UnvisitedPoints);
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}