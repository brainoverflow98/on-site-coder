using FluentValidation;
using System.Collections.Generic;

namespace WebApp.Helpers
{
    public static class ValidatorExtensions
    {
        public static void ValidateOrThrow<T>(this IValidator<T> validator, T instance)
        {
            var result = validator.Validate(instance);
            if (!result.IsValid)
            {
                var errorList = new List<string>();
                foreach (var error in result.Errors)
                {
                    errorList.Add(error.ErrorMessage);
                }
                throw new Exceptions.ValidationException(errorList, instance);
            }
        }
    }
}
