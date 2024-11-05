using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottishGlenAssetTracking.Data;

namespace ScottishGlenAssetTracking.Services
{
    /// <summary>
    /// DepartmentService class used to interact with the database for the Department entity.
    /// </summary>
    public class DepartmentService
    {
        // Private field for the ScottishGlenContext.
        private readonly ScottishGlenContext _context;

        /// <summary>
        /// Constructor for the DepartmentService class.
        /// </summary>
        /// <param name="context">ScottishGlenContext to be injected using dependency injection.</param>
        public DepartmentService(ScottishGlenContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to add a Department to the database.
        /// </summary>
        /// <param name="id">Id of the Department to be retrieved.</param>
        /// <returns>Department from the database with the chosen Id.</returns>
        public Department GetDepartment(int id)
        {
            // Return the Department from the database with the chosen Id.
            return _context.Departments.Find(id);
        }

        /// <summary>
        /// Method to get all Departments from the database.
        /// </summary>
        /// <returns>List of all Departments from the database.</returns>
        public List<Department> GetDepartments()
        {
            // Return the list of all Departments from the database.
            return _context.Departments.ToList();
        }
    }
}
