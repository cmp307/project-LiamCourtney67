using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    /// <summary>
    /// Model class for the Department entity.
    /// </summary>
    public class Department
    {
        // Private fields for the Department entity.
        private int _id;                                // Set by the database, so no validation needed.
        private string _name;
        private List<Employee> _employees;              // No validation needed, handled by Employee entity.

        /// <summary>
        /// Id property for the Department entity.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Name property for the Department entity.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (IsValidName(value))
                {
                    _name = value;
                }
                else
                {
                    throw new ArgumentException("Name must be between 1 and 64 characters and contain only letters and spaces.");
                }
            }
        }

        /// <summary>
        /// List of navigational properties for the Employee entities within the Department entity.
        /// </summary>
        public List<Employee> Employees
        {
            get { return _employees; }
            set { _employees = value; }
        }

        /// <summary>
        /// Validation method for the Name property of the Department entity, must be between 1 and 64 characters and contain only letters and spaces.
        /// </summary>
        /// <param name="name">Name to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidName(string name)
        {
            // Validation is based off ScottishGlen's current Departments, they have advised these will not change.
            bool isNullOrWhitespace = string.IsNullOrWhiteSpace(name);
            bool isTooLong = name.Length > 64;
            bool hasInvalidCharacters = name.Any(c => !char.IsLetter(c) && c != ' ');

            return !isNullOrWhitespace && !isTooLong && !hasInvalidCharacters;
        }
    }
}
