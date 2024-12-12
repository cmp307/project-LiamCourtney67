using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenUnitTesting.Models
{
    /// <summary>
    /// Unit tests for Employee model.
    /// </summary>
    [TestClass]
    public class EmployeeUnitTests
    {
        /// <summary>
        /// Unit test for a valid Employee.
        /// </summary>
        [TestMethod]
        public void ValidEmployeeUnitTest()
        {
            // Arrange
            Employee employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "employee@email.com",
                Department = new Department { Name = "IT" }
            };

            // Act
            string expectedFirstName = "John";
            string expectedLastName = "Doe";
            string expectedEmail = "employee@email.com";
            string expectedDepartmentName = "IT";

            // Assert
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
            Assert.AreEqual(expectedEmail, employee.Email);
            Assert.AreEqual(expectedDepartmentName, employee.Department.Name);
        }

        /// <summary>
        /// Unit test for a valid Employee with a HardwareAsset.
        /// </summary>
        [TestMethod]
        public void ValidEmployeeWithHardwareAssetUnitTest()
        {
            // Arrange
            Employee employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "test@email.com",
                Department = new Department { Name = "IT" }
            };

            HardwareAsset hardwareAsset = new HardwareAsset
            {
                Name = "Laptop",
                Model = "Latitude 7400",
                Manufacturer = "Dell",
                PurchaseDate = new DateTime(2020, 1, 1),
                Type = "x64",
                IpAddress = "192.168.0.1",
                Employee = employee
            };

            employee.HardwareAssets = new List<HardwareAsset> { hardwareAsset };

            // Act
            string expectedEmployeeFirstName = "John";
            string expectedEmployeeLastName = "Doe";
            string expectedEmployeeEmail = "test@email.com";

            string expectedDepartmentName = "IT";

            string expectedHardwareAssetName = "Laptop";
            string expectedHardwareAssetModel = "Latitude 7400";
            string expectedHardwareAssetManufacturer = "Dell";
            DateTime expectedHardwareAssetPurchaseDate = new DateTime(2020, 1, 1);
            string expectedHardwareAssetType = "x64";
            string expectedHardwareAssetIpAddress = "192.168.0.1";

            int expectedHardwareAssetCount = 1;

            // Assert
            Assert.AreEqual(expectedEmployeeFirstName, employee.FirstName);
            Assert.AreEqual(expectedEmployeeLastName, employee.LastName);
            Assert.AreEqual(expectedEmployeeEmail, employee.Email);

            Assert.AreEqual(expectedDepartmentName, employee.Department.Name);

            Assert.AreEqual(expectedHardwareAssetName, employee.HardwareAssets.First().Name);
            Assert.AreEqual(expectedHardwareAssetModel, employee.HardwareAssets.First().Model);
            Assert.AreEqual(expectedHardwareAssetManufacturer, employee.HardwareAssets.First().Manufacturer);
            Assert.AreEqual(expectedHardwareAssetPurchaseDate, employee.HardwareAssets.First().PurchaseDate);
            Assert.AreEqual(expectedHardwareAssetType, employee.HardwareAssets.First().Type);
            Assert.AreEqual(expectedHardwareAssetIpAddress, employee.HardwareAssets.First().IpAddress);

            Assert.AreEqual(expectedHardwareAssetCount, employee.HardwareAssets.Count);
        }

        /// <summary>
        /// Unit test for an invalid Employee.
        /// </summary>
        [TestMethod]
        public void InvalidEmployeeUnitTest()
        {
            // Arrange, Act, Assert
            Assert.ThrowsException<ArgumentException>(() => new Employee
            {
                FirstName = "_J_@_£",
                LastName = "Doe",
                Email = "email"
            }
            );
        }

        /// <summary>
        /// Unit test for an Employee with an invalid HardwareAsset.
        /// </summary>
        [TestMethod]
        public void EmployeeWithInvalidHardwareAssetUnitTest()
        {
            // Arrange
            Employee employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "test@email.com",
                Department = new Department { Name = "IT" }
            };

            // Act, Assert
            Assert.ThrowsException<ArgumentException>(() =>
                employee.HardwareAssets = new List<HardwareAsset>
                {
                    new HardwareAsset
                    {
                        Name = "~",
                        Model = "Latitude 7400",
                        Manufacturer = "Dell",
                        PurchaseDate = new DateTime(2026, 1, 1),
                        Type = "x64",
                        IpAddress = "4",
                        Employee = employee
                    }
                }
            );
        }
    }
}
