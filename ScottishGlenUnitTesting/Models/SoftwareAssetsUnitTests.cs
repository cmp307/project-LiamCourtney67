using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenUnitTesting.Models
{
    /// <summary>
    /// Unit tests for SoftwareAsset model.
    /// </summary>
    [TestClass]
    public class SoftwareAssetsUnitTests
    {
        /// <summary>
        /// Unit test for a valid SoftwareAsset.
        /// </summary>
        [TestMethod]
        public void ValidSoftwareAssetUnitTest()
        {
            // Arrange
            SoftwareAsset softwareAsset = new SoftwareAsset
            {
                Name = "Windows 10",
                Version = "22H2",
                Manufacturer = "Microsoft",
            };

            // Act
            string expectedName = "Windows 10";
            string expectedVersion = "22H2";
            string expectedManufacturer = "Microsoft";

            // Assert
            Assert.AreEqual(expectedName, softwareAsset.Name);
            Assert.AreEqual(expectedVersion, softwareAsset.Version);
            Assert.AreEqual(expectedManufacturer, softwareAsset.Manufacturer);
        }

        /// <summary>
        /// Unit test for a valid SoftwareAsset with a HardwareAsset.
        /// </summary>
        [TestMethod]
        public void ValidSoftwareAssetWithHardwareAssetUnitTest()
        {
            // Arrange
            SoftwareAsset softwareAsset = new SoftwareAsset
            {
                Name = "Windows 10",
                Version = "22H2",
                Manufacturer = "Microsoft",
                HardwareAssets = new List<HardwareAsset>
                {
                    new HardwareAsset
                    {
                        Name = "Laptop",
                        Model = "Latitude 7400",
                        Manufacturer = "Dell",
                        Type = "x64",
                        IpAddress = "192.168.0.1",
                        Notes = "Test notes"
                    }
                }
            };

            // Act
            string expectedName = "Windows 10";
            string expectedVersion = "22H2";
            string expectedManufacturer = "Microsoft";

            int expectedHardwareAssetCount = 1;

            // Assert
            Assert.AreEqual(expectedName, softwareAsset.Name);
            Assert.AreEqual(expectedVersion, softwareAsset.Version);
            Assert.AreEqual(expectedManufacturer, softwareAsset.Manufacturer);

            Assert.AreEqual(expectedHardwareAssetCount, softwareAsset.HardwareAssets.Count);
        }

        /// <summary>
        /// Unit test for an invalid SoftwareAsset.
        /// </summary>
        [TestMethod]
        public void InvalidSoftwareAssetUnitTest()
        {
            // Arrange
            SoftwareAsset softwareAsset = new SoftwareAsset();

            // Act, Assert
            Assert.ThrowsException<ArgumentException>(() => softwareAsset.Name = "#£$");
            Assert.ThrowsException<ArgumentException>(() => softwareAsset.Version = "#£$");
            Assert.ThrowsException<ArgumentException>(() => softwareAsset.Manufacturer = "#£$");
        }

        /// <summary>
        /// Unit test for a valid SoftwareAsset with invalid HardwareAssets.
        /// </summary>
        [TestMethod]
        public void ValidSoftwareAssetWithInvalidHardwareAssetsUnitTest()
        {
            // Arrange
            SoftwareAsset softwareAsset = new SoftwareAsset
            {
                Name = "Windows 10",
                Version = "22H2",
                Manufacturer = "Microsoft",
            };

            // Act, Assert
            Assert.ThrowsException<ArgumentException>(() =>
                softwareAsset.HardwareAssets = new List<HardwareAsset>
                {
                    new HardwareAsset
                    {
                        Name = "Laptop",
                        Model = "Latitude 7400",
                        Manufacturer = "Dell",
                        Type = "x64",
                        IpAddress = "x",
                        Notes = "Test notes"
                    }
                }
            );
        }
    }
}
