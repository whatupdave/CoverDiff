using System.Collections.Generic;

namespace CoverDiff
{
    internal class DocumentFileSet
    {
        readonly Dictionary<int,CoverageDocument> _documents = new Dictionary<int, CoverageDocument>();

        public DocumentFileSet(IEnumerable<CoverageDocument> documents)
        {
            foreach (var document in documents)
            {
                _documents[document.Id] = document;
            }
        }

        public CoverageDocument this[int documentId]
        {
            get 
            {
                if (_documents.ContainsKey(documentId))
                    return _documents[documentId];
                return null;
            }
        }
    }
}