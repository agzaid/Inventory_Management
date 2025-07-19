using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Localization
{
    public class LocalizedRequiredAttribute : RequiredAttribute, IClientModelValidator
    {
        private readonly string _key;

        public LocalizedRequiredAttribute(string key)
        {
            _key = key;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var localizer = validationContext.GetService(typeof(IStringLocalizer)) as IStringLocalizer;
            var errorMessage = localizer?[_key] ?? _key;

            return base.IsValid(value, validationContext) ??
                   new ValidationResult(errorMessage);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            var localizer = context.ActionContext.HttpContext.RequestServices.GetService(typeof(IStringLocalizer)) as IStringLocalizer;
            var errorMessage = localizer?[_key] ?? _key;

            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-required"] = errorMessage;
        }
    }

}
