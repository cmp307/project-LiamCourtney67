using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    public class Department
    {
        private int _id;
        private string _name;
        private List<Employee> _employees;

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
        public List<Employee> Employees
        {
            get { return _employees; }
            set { _employees = value; }
        }
    }
}
