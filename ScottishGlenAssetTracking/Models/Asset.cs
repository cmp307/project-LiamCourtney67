using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    /// <summary>
    /// Model class for the Asset entity.
    /// </summary>
    public class Asset
    {
        // Private fields for the Asset entity.
        private int _id;
        private string _name;
        private string _model;
        private string _manufacturer;
        private string _type;
        private string _ipAddress;
        private DateTime? _purchaseDate;        // Purchase date can be empty
        private string? _notes;                 // Notes can be empty
        private Employee _employee;

        /// <summary>
        /// Id property for the Asset entity.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Name property for the Asset entity.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Model property for the Asset entity.
        /// </summary>
        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        /// <summary>
        /// Manufacturer property for the Asset entity.
        /// </summary>
        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }

        /// <summary>
        /// Type property for the Asset entity.
        /// </summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// IpAddress property for the Asset entity.
        /// </summary>
        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        /// <summary>
        /// Optional PurchaseDate property for the Asset entity.
        /// </summary>
        public DateTime? PurchaseDate
        {
            get { return _purchaseDate; }
            set { _purchaseDate = value; }
        }

        /// <summary>
        /// Optional Notes property for the Asset entity.
        /// </summary>
        public string? Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        /// <summary>
        /// Navigational property for the Employee entity within the Asset entity.
        /// </summary>
        public Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }
    }
}
