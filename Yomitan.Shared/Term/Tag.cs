using System.Collections.Generic;

namespace Yomitan.Shared.Term
{
    public class Tag
    {
        // TODO: Refactor to configuration file.
        private static IDictionary<string, string> _colorMap = new Dictionary<string, string>
        {
            { "default", "#8a8a91" },
            { "name", "#b6327a" },
            { "expression", "#f0ad4e" },
            { "popular", "#0275d8" },
            { "frequent", "#5bc0de" },
            { "archaic", "#d9534f" },
            { "dictionary", "#aa66cc" },
            { "frequency", "#5cb85c" },
            { "pos", "#565656" },
            { "search", "#8a8a91" },
        };

        public string Name { get; set; }
        public string Category { get; set; }
        public int Order { get; set; }
        public string Notes { get; set; }
        public int PopularityScore { get; set; }
        public string Color { get; set; }

        public Tag() { }

        public Tag(string name) : this(name, string.Empty) { }

        public Tag(string name, string category) : this(name, category, 0, string.Empty, 0)
        {
            Name = name;
            Category = category;
        }

        public Tag(string name, string category, int order, string notes, int popularityScore)
        {
            Name = name;
            Category = category;
            Order = order;
            Notes = notes;
            PopularityScore = popularityScore;

            if (_colorMap.TryGetValue(category, out string color))
                Color = color;
            else
                Color = _colorMap["default"];
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tag))
                return false;

            Tag other = (Tag)obj;
            return Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
