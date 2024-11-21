using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    /// <summary>
    /// Model class for the Employee entity.
    /// </summary>
    public class Employee
    {
        // Private fields for the Employee entity.
        private int _id;
        private string _firstName;
        private string _lastName;
        private string _email;
        private Department _department;
        private List<HardwareAsset> _hardwareAssets;

        /// <summary>
        /// Id property for the Employee entity.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// FirstName property for the Employee entity.
        /// </summary>
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        /// <summary>
        /// LastName property for the Employee entity.
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        /// <summary>
        /// Non-mapped property for the full name of the employee entity.
        /// </summary>
        [NotMapped]
        public string Name
        {
            get { return _firstName + " " + _lastName; }
        }

        /// <summary>
        /// Email property for the Employee entity.
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// Navigational property for the Department entity within the Employee entity.
        /// </summary>
        public Department Department
        {
            get { return _department; }
            set { _department = value; }
        }

        /// <summary>
        /// List of navigational properties for the HardwareAsset entities within the Employee entity.
        /// </summary>
        public List<HardwareAsset> HardwareAssets
        {
            get { return _hardwareAssets; }
            set { _hardwareAssets = value; }
        }
    }
}
