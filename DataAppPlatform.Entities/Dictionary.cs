using DataAppPlatform.Entities.Common.Attributes;

namespace DataAppPlatform.Entities
{
    [DisplayValue(nameof(Name))]
    public class Dictionary: Entity
    {
        public string Name { get; set; }
    }
}