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
        public bool AddEmployee(Employee employee)
        {
            using (var context = new ScottishGlenContext())
            {
                context.Employees.Add(employee);
                context.Departments.Attach(employee.Department);
                context.SaveChanges();
                return true;
            }
        }
        public Employee GetEmployee(int employeeId)
        {
            using (var context = new ScottishGlenContext())
            {
                return context.Employees.Find(employeeId);
            }
        }

        public List<Employee> GetEmployees(int departmentId)
        {
            using (var context = new ScottishGlenContext())
            {
                return context.Employees.Where(e => e.Department.Id == departmentId).ToList();
            }
        }

        public bool UpdateEmployee(Employee employee)
        {
            using (var context = new ScottishGlenContext())
            {
                context.Employees.Update(employee);
                context.SaveChanges();
                return true;
            }
        }

        public bool DeleteEmployee(int employeeId)
        {
            using (var context = new ScottishGlenContext())
            {
                var employee = context.Employees.Include(e => e.Assets).FirstOrDefault(e => e.Id == employeeId);

                // Reassign assets to the employee for assets without an employee
                var assetEmployee = context.Employees.Find(1);
                foreach (var asset in employee.Assets)
                {
                    asset.Employee = assetEmployee;
                    context.Assets.Update(asset);
                }
                employee.Assets.Clear();

                // Remove the employee
                context.Employees.Remove(employee);
                context.SaveChanges();
                return true;
            }
        }
    }
}
