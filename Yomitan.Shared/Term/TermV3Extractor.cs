using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yomitan.Shared.Repository;

namespace Yomitan.Shared.Term
{
    internal class TermV3Extractor : IExtractor<Term>
    {
        public bool TryExtract(string source, out IEnumerable<Term> entries)
        {
            try
            {
                string content = File.ReadAllText(source);
                entries = JsonConvert.DeserializeObject<IEnumerable<Term>>(
                        content, new TermV3JsonConverter()).Where(term => term != null);
            }
            catch (Exception)
            {
                entries = null;
                return false;
            }

            return true;
        }
    }
}
