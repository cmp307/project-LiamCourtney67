using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottishGlenAssetTracking.Data;

namespace ScottishGlenAssetTracking.Services
{
    public class DepartmentService
    {
        public Department GetDepartment(int id)
        {
            using (var context = new ScottishGlenContext())
            {
                return context.Departments.Find(id);
            }
        }

        public List<Department> GetDepartments()
        {
            using (var context = new ScottishGlenContext())
            {
                return context.Departments.ToList();
            }
        }
    }
}
