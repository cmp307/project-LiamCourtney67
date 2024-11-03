using Microsoft.EntityFrameworkCore;
using ScottishGlenAssetTracking.Data;
using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Services
{
    public class EmployeeService
    {
        private readonly ScottishGlenContext _context;

        public EmployeeService(ScottishGlenContext context)
        {
            _context = context;
        }
        public bool AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.Departments.Attach(employee.Department);
            _context.SaveChanges();
            return true;
        }
        public Employee GetEmployee(int employeeId)
        {
            return _context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == employeeId);
        }

        public List<Employee> GetEmployees(int departmentId)
        {
            return _context.Employees.Include(e => e.Assets).Include(e => e.Department).Where(e => e.Department.Id == departmentId).ToList();
        }

        public bool UpdateEmployee(Employee employee)
        {
            _context.Entry(employee.Department).State = EntityState.Unchanged;
            _context.Employees.Update(employee);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteEmployee(int employeeId)
        {
            var employee = _context.Employees.Include(e => e.Assets).FirstOrDefault(e => e.Id == employeeId);

            // Reassign assets to the employee for assets without an employee
            var assetEmployee = _context.Employees.Find(1);
            foreach (var asset in employee.Assets)
            {
                asset.Employee = assetEmployee;
                _context.Assets.Update(asset);
            }
            employee.Assets.Clear();

            // Remove the employee
            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return true;
        }
    }
}
