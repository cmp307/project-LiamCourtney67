using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    public class Asset
    {
        private int _id;
        private string _name;
        private string _model;
        private string _manufacturer;
        private string _type;
        private string _ipAddress;
        private DateTime? _purchaseDate;      // Purchase date can be empty
        private string? _notes;             // Notes can be empty
        private Employee _employee;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }
        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }
        public DateTime? PurchaseDate
        {
            get { return _purchaseDate; }
            set { _purchaseDate = value; }
        }
        public string? Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        public Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }
    }
}
