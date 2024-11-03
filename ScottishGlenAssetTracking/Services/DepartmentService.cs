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
        private readonly ScottishGlenContext _context;

        public DepartmentService(ScottishGlenContext context)
        {
            _context = context;
        }
        public Department GetDepartment(int id)
        {
            return _context.Departments.Find(id);
        }

        public List<Department> GetDepartments()
        {
            return _context.Departments.ToList();
        }
    }
}
