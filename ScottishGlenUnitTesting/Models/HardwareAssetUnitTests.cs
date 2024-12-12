using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenUnitTesting.Models
{
    /// <summary>
    /// Unit tests for HardwareAsset model.
    /// </summary>
    [TestClass]
    public class HardwareAssetUnitTests
    {
        /// <summary>
        /// Unit test for a valid HardwareAsset.
        /// </summary>
        [TestMethod]
        public void ValidHardwareAssetUnitTest()
        {
            // Arrange
            HardwareAsset hardwareAsset = new HardwareAsset
            {
                Name = "Laptop",
                Model = "Latitude 7400",
                Manufacturer = "Dell",
                Type = "x64",
                IpAddress = "192.168.0.1",
                Notes = "Test notes",
                Employee = new Employee
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test@email.com",
                    Department = new Department { Name = "IT" }
                }
            };

            // Act
            string expectedName = "Laptop";
            string expectedModel = "Latitude 7400";
            string expectedManufacturer = "Dell";
            string expectedType = "x64";
            string expectedIpAddress = "192.168.0.1";
            string expectedNotes = "Test notes";

            string expectedEmployeeFirstName = "John";
            string expectedEmployeeLastName = "Doe";
            string expectedEmployeeEmail = "test@email.com";

            string expectedEmployeeDepartmentName = "IT";

            // Assert
            Assert.AreEqual(expectedName, hardwareAsset.Name);
            Assert.AreEqual(expectedModel, hardwareAsset.Model);
            Assert.AreEqual(expectedManufacturer, hardwareAsset.Manufacturer);
            Assert.AreEqual(expectedType, hardwareAsset.Type);
            Assert.AreEqual(expectedIpAddress, hardwareAsset.IpAddress);
            Assert.AreEqual(expectedNotes, hardwareAsset.Notes);

            Assert.AreEqual(expectedEmployeeFirstName, hardwareAsset.Employee.FirstName);
            Assert.AreEqual(expectedEmployeeLastName, hardwareAsset.Employee.LastName);
            Assert.AreEqual(expectedEmployeeEmail, hardwareAsset.Employee.Email);

            Assert.AreEqual(expectedEmployeeDepartmentName, hardwareAsset.Employee.Department.Name);
        }

        /// <summary>
        /// Unit test for a valid HardwareAsset with a SoftwareAsset.
        /// </summary>
        [TestMethod]
        public void ValidHardwareAssetWithSoftwareAssetUnitTest()
        {
            // Arrange
            HardwareAsset hardwareAsset = new HardwareAsset
            {
                Name = "Laptop",
                Model = "Latitude 7400",
                Manufacturer = "Dell",
                Type = "x64",
                IpAddress = "192.168.0.1",
                Notes = "Test notes",
                Employee = new Employee
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test@email.com",
                    Department = new Department { Name = "IT" }
                },
                SoftwareAsset = new SoftwareAsset
                {
                    Name = "Windows 10",
                    Version = "20H2",
                    Manufacturer = "Microsoft"
                },
                SoftwareLinkDate = new DateTime(2024, 12, 1)
            };

            // Act
            string expectedName = "Laptop";
            string expectedModel = "Latitude 7400";
            string expectedManufacturer = "Dell";
            string expectedType = "x64";
            string expectedIpAddress = "192.168.0.1";
            string expectedNotes = "Test notes";

            string expectedEmployeeFirstName = "John";
            string expectedEmployeeLastName = "Doe";
            string expectedEmployeeEmail = "test@email.com";

            string expectedEmployeeDepartmentName = "IT";

            string expectedSoftwareAssetName = "Windows 10";
            string expectedSoftwareAssetVersion = "20H2";
            string expectedSoftwareAssetManufacturer = "Microsoft";
            DateTime expectedSoftwareLinkDate = new DateTime(2024, 12, 1);

            // Assert
            Assert.AreEqual(expectedName, hardwareAsset.Name);
            Assert.AreEqual(expectedModel, hardwareAsset.Model);
            Assert.AreEqual(expectedManufacturer, hardwareAsset.Manufacturer);
            Assert.AreEqual(expectedType, hardwareAsset.Type);
            Assert.AreEqual(expectedIpAddress, hardwareAsset.IpAddress);
            Assert.AreEqual(expectedNotes, hardwareAsset.Notes);

            Assert.AreEqual(expectedEmployeeFirstName, hardwareAsset.Employee.FirstName);
            Assert.AreEqual(expectedEmployeeLastName, hardwareAsset.Employee.LastName);
            Assert.AreEqual(expectedEmployeeEmail, hardwareAsset.Employee.Email);

            Assert.AreEqual(expectedEmployeeDepartmentName, hardwareAsset.Employee.Department.Name);

            Assert.AreEqual(expectedSoftwareAssetName, hardwareAsset.SoftwareAsset.Name);
            Assert.AreEqual(expectedSoftwareAssetVersion, hardwareAsset.SoftwareAsset.Version);
            Assert.AreEqual(expectedSoftwareAssetManufacturer, hardwareAsset.SoftwareAsset.Manufacturer);
            Assert.AreEqual(expectedSoftwareLinkDate, hardwareAsset.SoftwareLinkDate);
        }

        /// <summary>
        /// Unit test for an invalid HardwareAsset.
        /// </summary>
        [TestMethod]
        public void InvalidHardwareAssetUnitTest()
        {
            // Arrange
            HardwareAsset hardwareAsset = new HardwareAsset();

            // Act, Assert
            Assert.ThrowsException<ArgumentException>(() => hardwareAsset.Name = "@");
            Assert.ThrowsException<ArgumentException>(() => hardwareAsset.Model = "#");
            Assert.ThrowsException<ArgumentException>(() => hardwareAsset.Manufacturer = "!");
            Assert.ThrowsException<ArgumentException>(() => hardwareAsset.Type = "£");
            Assert.ThrowsException<ArgumentException>(() => hardwareAsset.IpAddress = "x");
            Assert.ThrowsException<ArgumentException>(() => hardwareAsset.Notes = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHI");
            Assert.ThrowsException<ArgumentException>(() => hardwareAsset.PurchaseDate = new DateTime(1989, 12, 1));

        }

        /// <summary>
        /// Unit test for a valid HardwareAsset with an invalid SoftwareAsset.
        /// </summary>
        [TestMethod]
        public void ValidHardwareAssetWithInvalidSoftwareAssetUnitTest()
        {
            // Arrange
            HardwareAsset hardwareAsset = new HardwareAsset
            {
                Name = "Laptop",
                Model = "Latitude 7400",
                Manufacturer = "Dell",
                Type = "x64",
                IpAddress = "192.168.0.1",
                Notes = "Test notes",
                PurchaseDate = new DateTime(2020, 1, 1),
                Employee = new Employee
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test@email.com",
                    Department = new Department { Name = "IT" }
                }
            };

            // Act, Assert
            Assert.ThrowsException<ArgumentException>(() =>
                hardwareAsset.SoftwareAsset = new SoftwareAsset
                {
                    Name = "@ 10",
                    Version = "~",
                    Manufacturer = "£$%",
                }
            );
        }
    }
}
