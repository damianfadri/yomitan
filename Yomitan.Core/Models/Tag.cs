namespace Yomitan.Core.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int Order { get; set; }
        public string Notes { get; set; }
        public int PopularityScore { get; set; }

        public Tag() { }

        public Tag(string name) : this(name, string.Empty) { }

        public Tag(string name, string category) : this(name, category, 0, string.Empty, 0) { }

        public Tag(string name, string category, int order, string notes, int popularityScore)
        {
            Name = name;
            Category = category;
            Order = order;
            Notes = notes;
            PopularityScore = popularityScore;
        }

        public void Update(Tag updatedTag)
        {
            Name = updatedTag.Name;
            Category = updatedTag.Category;
            Order = updatedTag.Order;
            Notes = updatedTag.Notes;
            PopularityScore = updatedTag.PopularityScore;
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
