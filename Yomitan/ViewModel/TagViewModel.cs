using CommunityToolkit.Mvvm.DependencyInjection;
using Yomitan.Contracts;

namespace Yomitan.ViewModel
{
    public class TagViewModel
    {
        private const string DefaultTagColor = "#8a8a91";
        private readonly ITagColorService _tagColorService;

        public string Name { get; set; }
        public string Color { get; set; }


        public TagViewModel(string name, string category)
        {
            _tagColorService = Ioc.Default.GetService<ITagColorService>();

            Name = name;
            Color = GetColor(category);
        }

        private string GetColor(string category)
        {
            var tagColor = _tagColorService.GetTagColor(category);
            if (tagColor == null)
                return DefaultTagColor;

            return tagColor.Color;
        }
    }
}
