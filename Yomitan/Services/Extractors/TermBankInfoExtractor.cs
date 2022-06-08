using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Yomitan.Core.Services;
using Yomitan.Models;

namespace Yomitan.Services.Extractors
{
    internal class TermBankInfoExtractor : IExtractor<TermBankInfo>
    {
        public bool TryExtract(string source, out IEnumerable<TermBankInfo> entries)
        {
            try
            {
                string content = File.ReadAllText(source);
                TermBankInfo info = JsonConvert.DeserializeObject<TermBankInfo>(content);

                entries = new TermBankInfo[] { info };
                return true;
            }
            catch (Exception)
            {
                entries = null;
                return false;
            }
        }
    }
}
