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
    /// <summary>
    /// EmployeeService class used to interact with the database for the Employee entity.
    /// </summary>
    public class EmployeeService
    {
        // Private field for the ScottishGlenContext.
        private readonly ScottishGlenContext _context;

        /// <summary>
        /// Constructor for the EmployeeService class.
        /// </summary>
        /// <param name="context">ScottishGlenContext to be injected using dependency injection.</param>
        public EmployeeService(ScottishGlenContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to add an Employee to the database.
        /// </summary>
        /// <param name="employee">Employee to be added to the database.</param>
        /// <returns>True if added to the database, false if not.</returns>
        public bool AddEmployee(Employee employee)
        {
            // Add the Employee to the database, attach the Department and save the changes.
            _context.Employees.Add(employee);
            _context.Departments.Attach(employee.Department);
            _context.SaveChanges();

            // Return true if the Employee was added to the database.
            return true;
        }

        /// <summary>
        /// Method to get an Employee from the database by its Id.
        /// </summary>
        /// <param name="employeeId">Id of the Employee to be retrieved.</param>
        /// <returns>Employee from the database with the chosen Id.</returns>
        public Employee GetEmployee(int employeeId)
        {
            // Return the Employee from the database with the chosen Id.
            return _context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == employeeId);
        }

        /// <summary>
        /// Method to get all Employees from the database.
        /// </summary>
        /// <param name="departmentId">Id of the department to retrieve the Employees for.</param>
        /// <returns>List of Employees from the database for the chosen Department.</returns>
        public List<Employee> GetEmployees(int departmentId)
        {
            // Return the list of all Employees from the database for the chosen Department.
            return _context.Employees.Include(e => e.HardwareAssets).Include(e => e.Department).Where(e => e.Department.Id == departmentId).ToList();
        }

        /// <summary>
        /// Method to update an Employee in the database.
        /// </summary>
        /// <param name="employee">Employee to be updated in the database.</param>
        /// <returns>True if updated in the database, false if not.</returns>
        public bool UpdateEmployee(Employee employee)
        {
            // Set the state of the Department to Unchanged to prevent adding a new Department.
            _context.Entry(employee.Department).State = EntityState.Unchanged;

            // Update the Employee in the database and save the changes.
            _context.Employees.Update(employee);
            _context.SaveChanges();

            // Return true if the Employee was updated in the database.
            return true;
        }

        /// <summary>
        /// Method to delete an Employee from the database.
        /// </summary>
        /// <param name="employeeId">Id of the employee to be deleted from the database.</param>
        /// <returns>True if deleted from the database, false if not.</returns>
        public bool DeleteEmployee(int employeeId)
        {
            // Find the employee in the database.
            var employee = _context.Employees.Include(e => e.HardwareAssets).FirstOrDefault(e => e.Id == employeeId);

            // Find accounts with the employee record in the database.
            var account = _context.Accounts.FirstOrDefault(a => a.EmployeeId == employeeId);

            // Find the HardwareAssets without Employee record in the database.
            var assetEmployee = _context.Employees.Find(1);

            // Reassign assets to the employee for assets without an employee
            foreach (var asset in employee.HardwareAssets)
            {
                if (asset != null)
                {
                    asset.Employee = assetEmployee;
                    _context.HardwareAssets.Update(asset);
                }
            }

            // Clear the assets from the employee
            employee.HardwareAssets.Clear();

            // Remove the employee from the account and set the account to not be an admin.
            if (account != null)
            {
                account.IsAdmin = false;
                account.Employee = null;
                _context.Accounts.Update(account);
            }

            // Remove the account from the employee.
            employee.Account = null;

            // Remove the employee from the database and save the changes.
            _context.Employees.Remove(employee);
            _context.SaveChanges();

            // Return true if the employee was deleted from the database.
            return true;
        }
    }
}
