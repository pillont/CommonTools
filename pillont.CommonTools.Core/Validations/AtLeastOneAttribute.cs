using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace pillont.CommonTools.Core.Validations;
public class AtLeastOneAttribute: ValidationAttribute
{
    public string[] Properties { get; }
    
    public AtLeastOneAttribute(params string[] properties)
        :base($"properties {string.Join(", ", properties)} values missing")
    {
        Properties = properties;
    }

    public override bool IsValid(object value)
    {
        var allValues = Properties.Select(prop => value
            .GetType()
            .GetProperty(prop)
            .GetValue(value));
    
        
        return allValues.Any(v=> v is not null);
    }
}
