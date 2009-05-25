using System.Xml.Linq;

namespace CoverDiff
{
    internal class CoverageDocument
    {
        public int Id { get; private set; }
        public string Url { get; private set; }

        public CoverageDocument(XElement docElement)
        {
            Id = int.Parse(docElement.Attribute("id").Value);
            Url = docElement.Attribute("url").Value;
        }
    }
}