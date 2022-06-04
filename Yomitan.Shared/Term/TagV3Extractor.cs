using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yomitan.Shared.Repository;

namespace Yomitan.Shared.Term
{
    public class TagV3Extractor : IExtractor<Tag>
    {
        public bool TryExtract(string source, out IEnumerable<Tag> entries)
        {
            try
            {
                string content = File.ReadAllText(source);
                entries = JsonConvert.DeserializeObject<IEnumerable<Tag>>(
                        content, new TagV3JsonConverter()).Where(tag => tag != null);
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
