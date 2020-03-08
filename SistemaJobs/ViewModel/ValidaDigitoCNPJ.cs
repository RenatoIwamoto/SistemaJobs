using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaJobs.ViewModel
{
    class ValidaDigitoCNPJ : ValidationAttribute, IClientValidatable
    {
        public ValidaDigitoCNPJ()
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;
            bool valido = ValidaCNPJ(value.ToString());
            return valido;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.FormatErrorMessage(null),
                ValidationType = "customvalidationcnpj"
            };
        }


        public static string RemoveNaoNumericos(string text)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[^0-9]");
            string ret = reg.Replace(text, string.Empty);
            return ret;
        }


        public static Boolean ValidaCNPJ(string cnpj)
        {
            cnpj = RemoveNaoNumericos(cnpj);

            // Verifica se o CNPJ informado tem os 14 digitos 
            if (cnpj.Length > 14)
            {
                return false;
            }

            return true;            
        }


        private static Boolean isDigito(string digito)
        {
            int n;
            return Int32.TryParse(digito, out n);
        }
    }
}