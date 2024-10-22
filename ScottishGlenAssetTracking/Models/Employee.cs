using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    public class Employee
    {
        private int _id;
        private string _firstName;
        private string _lastName;
        private string _email;
        private Department _department;
        private List<Asset> _assets;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        public Department Department
        {
            get { return _department; }
            set { _department = value; }
        }
        public List<Asset> Assets
        {
            get { return _assets; }
            set { _assets = value; }
        }
    }
}
