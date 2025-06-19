using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace Stoocker.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException() : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, List<string>>();
        }

        public Dictionary<string, List<string>> Errors { get; }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            foreach (var failure in failures)
            {
                if (!Errors.ContainsKey(failure.PropertyName))
                {
                    Errors[failure.PropertyName] = new List<string>();
                }
                Errors[failure.PropertyName].Add(failure.ErrorMessage);
            }
        }
    }
}
