using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenUnitTesting.Models
{
    /// <summary>
    /// Unit tests for Department model.
    /// </summary>
    [TestClass]
    public class DepartmentUnitTests
    {
        /// <summary>
        /// Unit test for a valid Department.
        /// </summary>
        [TestMethod]
        public void ValidDepartmentUnitTest()
        {
            // Arrange
            Department department = new Department
            {
                Name = "IT"
            };

            // Act
            string expected = "IT";

            // Assert
            Assert.AreEqual(expected, department.Name);
        }

        /// <summary>
        /// Unit test for a valid Department with Employees.
        /// </summary>
        [TestMethod]
        public void ValidDepartmentWithEmployeesUnitTest()
        {
            // Arrange
            Department department = new Department
            {
                Name = "IT"
            };

            List<Employee> employees = new List<Employee>
            {
                new Employee { FirstName = "John", LastName = "Doe", Email = "j.doe@email.com", Department = department },
                new Employee { FirstName = "Jane", LastName = "Smith", Email = "j.smith@email.com", Department = department }
            };

            department.Employees = employees;

            // Act
            string expectedName = "IT";
            int expectedEmployeeCount = 2;


            // Assert
            Assert.AreEqual(expectedName, department.Name);
            Assert.AreEqual(expectedEmployeeCount, department.Employees.Count);
        }

        /// <summary>
        /// Unit test for an invalid Department.
        /// </summary>
        [TestMethod]
        public void InvalidDepartmentUnitTest()
        {
            // Arrange, Act, Assert
            Assert.ThrowsException<ArgumentException>(() => new Department { Name = "I.T." });
        }

        /// <summary>
        /// Unit test for a valid Department with invalid Employees.
        /// </summary>
        [TestMethod]
        public void ValidDepartmentWithInvalidEmployeesUnitTest()
        {
            // Arrange
            Department department = new Department
            {
                Name = "IT"
            };

            // Act, Assert
            Assert.ThrowsException<ArgumentException>(() =>
                department.Employees = new List<Employee>
                {
                    new Employee
                    {
                        FirstName = "J@hn",
                        LastName = "Do£",
                        Email = "j.doeemail.com",
                        Department = department
                    }
                }
            );

        }
    }
}
