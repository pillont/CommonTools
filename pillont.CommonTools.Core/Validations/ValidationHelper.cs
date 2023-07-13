using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace pillont.CommonTools.Core.Validations;

public static class ValidationHelper
{
    public static List<ValidationResult> GetResultOfValidation(this object obj)
    {
        var validationContext = new ValidationContext(obj, null, null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(obj, validationContext, results, true);

        var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(prop => prop.CanRead
            && prop.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string)
            || property.PropertyType.IsValueType)
                continue;

            var value = obj.GetType().GetProperty(property.Name)?.GetValue(obj);

            if (value == null)
                continue;

            if (value is IEnumerable asEnumerable)
            {
                var index = 0;
                foreach (var enumObj in asEnumerable)
                {
                    if (enumObj == null)
                        continue;

                    var listValidationResults = enumObj.GetResultOfValidation();
                    foreach (var validationResult in listValidationResults)
                    {
                        results.Add(
                            new ValidationResult(
                                validationResult.ErrorMessage,
                                validationResult.MemberNames.Select(x => property.Name + '[' + index + ']' + '.' + x).ToList()));
                    }
                    index++;
                }

                continue;
            }

            var subValidationResults = value.GetResultOfValidation();
            foreach (var validationResult in subValidationResults)
            {
                results.Add(
                    new ValidationResult(
                        validationResult.ErrorMessage,
                        validationResult.MemberNames.Select(x => property.Name + '.' + x)));
            }
        }

        return results;
    }

    public static bool Validate(this object obj)
    {
        var results = obj.GetResultOfValidation();
        return results.Any();
    }

    public static void ValidateOrThrow<TException>(this object obj)
        where TException : Exception
    {
        var results = obj.GetResultOfValidation();
        if (results.Any())
        {
            var json = JsonSerializer.Serialize(results);
            var exception = Activator.CreateInstance(typeof(TException), new[] { json }) as Exception;

            throw exception!;
        }
    }
}