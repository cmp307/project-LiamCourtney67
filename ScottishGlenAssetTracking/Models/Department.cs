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
        private int _id;
        private string _name;
        private List<Employee> _employees;

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
            set { _name = value; }
        }

        /// <summary>
        /// List of navigational properties for the Employee entities within the Department entity.
        /// </summary>
        public List<Employee> Employees
        {
            get { return _employees; }
            set { _employees = value; }
        }
    }
}
