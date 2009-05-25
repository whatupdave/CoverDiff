using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CoverDiff
{
    internal class CoverageClass
    {
        public CoverageFile File { get; set; }
        public string Name { get; private set; }
        public List<CoverageMethod> Methods { get; private set; }

        public int UnvisitedPoints { get; private set; }

        public CoverageClass(CoverageFile file, XElement classElement)
        {
            File = file;
            Name = classElement.Attribute("name").Value;
            Methods = (from m in classElement.Descendants("method")
                       where m.Attribute("excluded").Value == "false"
                       select new CoverageMethod(this, m)).ToList();

            UnvisitedPoints = Methods.Sum(m => m.UnvisitedPoints);
        }

        public CoverageMethod this[string methodName]
        {
            get { return Methods.FirstOrDefault(m => m.Name == methodName); }
        }

        public override string ToString()
        {
            return Name;
        }

        public TextReader GetSourceFile()
        {
            throw new NotImplementedException();
        }
    }
}