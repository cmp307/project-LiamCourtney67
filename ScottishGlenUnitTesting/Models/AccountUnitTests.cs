using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenUnitTesting.Models
{
    /// <summary>
    /// Unit tests for Account model.
    /// </summary>
    [TestClass]
    public class AccountUnitTests
    {
        /// <summary>
        /// Unit test for a valid Account.
        /// </summary>
        [TestMethod]
        public void ValidAccountUnitTest()
        {
            // Arrange
            Account account = new Account
            {
                Email = "test@email.com",
                Password = "password",
                IsAdmin = false
            };

            // Act
            string expectedEmail = "test@email.com";
            string expectedPassword = "password";
            bool expectedIsAdmin = false;

            // Assert
            Assert.AreEqual(expectedEmail, account.Email);
            Assert.AreEqual(true, BCrypt.Net.BCrypt.EnhancedVerify(expectedPassword, account.Password));
            Assert.AreEqual(expectedIsAdmin, account.IsAdmin);
        }

        /// <summary>
        /// Unit test for a valid Account with an Employee.
        /// </summary>
        [TestMethod]
        public void ValidAccountWithEmployeeUnitTest()
        {
            // Arrange
            Account account = new Account
            {
                Email = "test@email.com",
                Password = "password",
                Employee = new Employee
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test@email.com"
                },
                IsAdmin = false
            };

            // Act
            string expectedEmail = "test@email.com";
            string expectedPassword = "password";
            string expectedEmployeeFirstName = "John";
            string expectedEmployeeLastName = "Doe";
            string expectedEmployeeEmail = "test@email.com";
            bool expectedIsAdmin = false;

            // Assert
            Assert.AreEqual(expectedEmail, account.Email);
            Assert.AreEqual(true, BCrypt.Net.BCrypt.EnhancedVerify(expectedPassword, account.Password));
            Assert.AreEqual(expectedEmployeeFirstName, account.Employee.FirstName);
            Assert.AreEqual(expectedEmployeeLastName, account.Employee.LastName);
            Assert.AreEqual(expectedEmployeeEmail, account.Employee.Email);
            Assert.AreEqual(expectedIsAdmin, account.IsAdmin);
        }

        /// <summary>
        /// Unit test for an invalid Account.
        /// </summary>
        [TestMethod]
        public void InvalidAccountUnitTest()
        {
            // Arrange, Act, Assert
            Account account = new Account();

            Assert.ThrowsException<ArgumentException>(() => account.Email = "testemail");
            Assert.ThrowsException<ArgumentException>(() => account.Password = "pass");

            // No Employee set.
            Assert.ThrowsException<NullReferenceException>(() => account.IsAdmin = true);
        }

        /// <summary>
        /// Unit test for a valid Account with an invalid Employee.
        /// </summary>
        [TestMethod]
        public void ValidAccountWithInvalidEmployeeUnitTest()
        {
            // Arrange
            Account account = new Account
            {
                Email = "test@email.com",
                Password = "password",
                IsAdmin = false
            };

            // Act, Assert
            Assert.ThrowsException<ArgumentException>(() =>
                account.Employee = new Employee
                {
                    FirstName = "J@hn",
                    LastName = "Do£",
                    Email = "testemail"
                }
            );
        }

        /// <summary>
        /// Unit test for a valid Account as an administrator.
        /// </summary>
        [TestMethod]
        public void ValidAccountAsAdminUnitTest()
        {
            // Arrange
            Account account = new Account
            {
                Email = "admin@email.com",
                Password = "password",
                Employee = new Employee
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "admin@email.com",
                    Department = new Department
                    {
                        Id = 5,
                        Name = "Information Technology"
                    }
                },
                IsAdmin = true
            };

            // Act
            bool expectedIsAdmin = true;

            // Assert
            Assert.AreEqual(expectedIsAdmin, account.IsAdmin);
        }

        /// <summary>
        /// Unit test for an invalid Account as an administrator.
        /// </summary>
        [TestMethod]
        public void InvalidAccountAsAdminUnitTest()
        {
            // Arrange
            Account account = new Account
            {
                Email = "admin@email.com",
                Password = "password",
                Employee = new Employee
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "employee@email.com",
                    Department = new Department
                    {
                        Name = "Human Resources"
                    }
                }
            };

            // Act, Assert
            Assert.ThrowsException<ArgumentException>(() => account.IsAdmin = true);
        }
    }
}
