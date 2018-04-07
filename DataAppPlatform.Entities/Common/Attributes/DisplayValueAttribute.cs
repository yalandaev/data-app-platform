namespace DataAppPlatform.Entities.Common.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)]
    public class DisplayValueAttribute : System.Attribute
    {
        public DisplayValueAttribute(string fieldName)
        {
            Name = fieldName;
        }

        public string Name { get; }
    }
}