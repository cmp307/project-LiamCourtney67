using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using ScottishGlenAssetTracking.Data;
using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Services
{
    /// <summary>
    /// SoftwareAssetService class used to interact with the database for the SoftwareAsset entity.
    /// </summary>
    public class SoftwareAssetService
    {
        // Private field for the ScottishGlenContext.
        private readonly ScottishGlenContext _context;

        /// <summary>
        /// Constructor for the SoftwareAssetService class.
        /// </summary>
        /// <param name="context">ScottishGlenContext to be injected using dependency injection.</param>
        public SoftwareAssetService(ScottishGlenContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to add a SoftwareAsset to the database.
        /// </summary>
        /// <param name="softwareAsset">SoftwareAsset to be added to the database.</param>
        /// <returns>True if added to the database, false if not.</returns>
        public bool AddSoftwareAsset(SoftwareAsset softwareAsset)
        {
            ///// Logic for existing SoftwareAsset

            if (DoesSoftwareAssetExist(softwareAsset))
            {
                LinkHardwareAssetsToSoftwareAsset(softwareAsset);
                return true;
            }

            ///// Logic for new SoftwareAsset

            // Set the state of the HardwareAssets to Unchanged to prevent adding new HardwareAssets, and attach them to the context if there are any.
            if (softwareAsset.HardwareAssets != null)
            {
                foreach (var hardwareAsset in softwareAsset.HardwareAssets)
                {
                    HardwareAsset trackedHardwareAsset = _context.HardwareAssets.Local.FirstOrDefault(a => a.Id == hardwareAsset.Id);

                    if (trackedHardwareAsset == null)
                    {
                        _context.HardwareAssets.Attach(hardwareAsset);
                        _context.Entry(trackedHardwareAsset).State = EntityState.Unchanged;
                    }
                }
            }

            // Add the SoftwareAsset to the database and save the changes.
            _context.SoftwareAssets.Add(softwareAsset);

            _context.SaveChanges();

            // Return true if the SoftwareAsset was added to the database.
            return true;
        }

        /// <summary>
        /// Method to get a SoftwareAsset from the database by its Id.
        /// </summary>
        /// <param name="softwareAssetId">Id of the SoftwareAsset to be retrieved.</param>
        /// <returns>SoftwareAsset from the database with the chosen Id.</returns>
        public SoftwareAsset GetSoftwareAsset(int softwareAssetId)
        {
            // Return the SoftwareAsset from the database with the chosen Id.
            return _context.SoftwareAssets.Include(a => a.HardwareAssets).FirstOrDefault(a => a.Id == softwareAssetId);
        }

        /// <summary>
        /// Method to get all SoftwareAssets from the database.
        /// </summary>
        /// <returns>List of the SoftwareAssets from the database.</returns>
        public List<SoftwareAsset> GetSoftwareAssets()
        {
            // Return the list of SoftwareAssets from the database.
            return _context.SoftwareAssets.Include(a => a.HardwareAssets).ToList();
        }

        /// <summary>
        /// Method to update an SoftwareAsset in the database.
        /// </summary>
        /// <param name="softwareAsset">SoftwareAsset to be updated in the database.</param>
        /// <returns>True if updated in the database, false if not.</returns>
        public bool UpdateSoftwareAsset(SoftwareAsset softwareAsset)
        {
            // Check if a software asset with the same name and version already exists.
            var existingSoftwareAsset = _context.SoftwareAssets
                                                .Include(sa => sa.HardwareAssets)
                                                .FirstOrDefault(sa => sa.Name == softwareAsset.Name && sa.Version == softwareAsset.Version);

            // Check if the software asset already exists and is not the same as the provided software asset.
            if (existingSoftwareAsset != null && existingSoftwareAsset.Id != softwareAsset.Id)
            {
                // Attach hardware assets to the existing software asset.
                foreach (var hardwareAsset in softwareAsset.HardwareAssets)
                {
                    var existingHardwareAsset = _context.HardwareAssets.Find(hardwareAsset.Id)
                                              ?? _context.HardwareAssets.Attach(hardwareAsset).Entity;

                    existingHardwareAsset.SoftwareAsset = existingSoftwareAsset;
                    existingHardwareAsset.SoftwareLinkDate = DateTime.Now;
                }

                // Delete the provided software asset (the "old" one) from the database.
                if (_context.SoftwareAssets.Local.Contains(softwareAsset))
                {
                    _context.SoftwareAssets.Remove(softwareAsset);
                }
                else
                {
                    _context.Entry(softwareAsset).State = EntityState.Deleted;
                }
            }
            else
            {
                _context.SoftwareAssets.Update(softwareAsset);
            }

            // Save changes to the database.
            _context.SaveChanges();
            return true;
        }


        /// <summary>
        /// Method to delete a SoftwareAsset from the database.
        /// </summary>
        /// <param name="softwareAssetId">Id of the SoftwareAsset to be deleted from the database.</param>
        /// <returns>True if deleted from the database, false if not.</returns>
        public bool DeleteSoftwareAsset(int softwareAssetId)
        {
            // Find the SoftwareAsset in the database with the chosen Id.
            var softwareAsset = _context.SoftwareAssets.Find(softwareAssetId);

            // Remove the SoftwareAsset from the database and save the changes.
            _context.SoftwareAssets.Remove(softwareAsset);
            _context.SaveChanges();

            // Return true if the SoftwareAsset was deleted from the database.
            return true;
        }

        /// <summary>
        /// Method to create a SoftwareAsset with automatically retrieved system information.
        /// </summary>
        /// <returns>SoftwareAsset with the system information.</returns>
        public SoftwareAsset GetSoftwareAssetWithSystemInfo()
        {
            // Initialize the name, version, and manufacturer of the system with "Unknown" in case of failure.
            string name = "Unknown";
            string version = "Unknown";
            string manufacturer = "Unknown";

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                // Iterate through the ManagementObjectSearcher to retrieve the system information.
                foreach (ManagementObject obj in searcher.Get())
                {
                    name = obj["Caption"]?.ToString() ?? "Unknown";
                    version = obj["Version"]?.ToString() ?? "Unknown";
                    manufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                }
            }

            // Return the SoftwareAsset with the system information.
            return new SoftwareAsset
            {
                Name = name,
                Version = version,
                Manufacturer = manufacturer
            };
        }

        /// <summary>
        /// Method to get vulnerabilities for a specific version of Windows from the NIST NVD API.
        /// </summary>
        /// <param name="version">Version to be checked.</param>
        /// <returns>List of Vulnerabilities for the specific version of Windows, or null if there are no vulnerabilities.</returns>
        /// <exception cref="ArgumentException">The version is invalid, must be a valid Windows 10/11 version..</exception>
        public async Task<List<Vulnerability>> GetVulnerabilitiesAsync(string version)
        {
            // Check if the version is null or empty.
            if (string.IsNullOrEmpty(version)) { return null; }

            // https://chatgpt.com/share/67670b36-f7f4-800c-aeb3-f42f0f497c09
            if (!Regex.IsMatch(version, @"^(?:\d{4}|\d{2}H\d)$"))
            {
                // Get the version name from the version number.
                version = GetVersionNameFromVersionNumber(version);
            }

            // Check if the version is for Windows 10.
            if (Int32.Parse(version.Substring(0, 2)) < 23) { return await GetVulnerabilitiesForWindows10Async(version); }

            // Check if the version is for Windows 11.
            else if (Int32.Parse(version.Substring(0, 2)) > 22) { return await GetVulnerabilitiesForWindows11Async(version); }

            // Return null if the version is not for Windows 10 or Windows 11.
            else { return null; }
        }

        /// <summary>
        /// Helper method to get the version name from the version number.
        /// </summary>
        /// <param name="versionNumber">Version</param>
        /// <returns>The version name to be used with the NIST API.</returns>
        /// <exception cref="ArgumentException">Thrown when the version number is not found.</exception>
        private string GetVersionNameFromVersionNumber(string versionNumber)
        {
            // NIST API uses version names, so we need to map the version number to the version name.
            // https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions
            // This will need to be updated as new versions are released.

            // Create a dictionary to map Windows version numbers to version names.
            var windowsVersionMap = new Dictionary<string, string>
            {
                // Windows 10 Versions
                { "10.0.10240", "1507" },
                { "10.0.10586", "1511" },
                { "10.0.14393", "1607" },
                { "10.0.15063", "1703" },
                { "10.0.16299", "1709" },
                { "10.0.17134", "1803" },
                { "10.0.17763", "1809" },
                { "10.0.18362", "1903" },
                { "10.0.18363", "1909" },
                { "10.0.19041", "2004" },
                { "10.0.19042", "20H2" },
                { "10.0.19043", "21H1" },
                { "10.0.19044", "21H2" },
                { "10.0.19045", "22H2" },

                // Windows 11 Versions
                { "10.0.22000", "21H2" },
                { "10.0.22621", "22H2" },
                { "10.0.22631", "23H2" },
                { "10.0.26100", "24H2" }
            };

            // Check if the version number is in the dictionary and return the version name.
            windowsVersionMap.TryGetValue(versionNumber, out string versionName);
            return versionName ?? throw new ArgumentException("Version number not found, please try again later, if the issue persists, contact an administrator.");

        }

        /// <summary>
        /// Method to get vulnerabilities for a specific version of Windows 10 from the NIST NVD API.
        /// </summary>
        /// <param name="version">Version to be checked.</param>
        /// <returns>List of Vulnerabilities for the specific version of Windows 10, or null if there are no vulnerabilities.</returns>
        /// <exception cref="HttpRequestException">Thrown when the request to the NIST NVD API fails.</exception>
        private async Task<List<Vulnerability>> GetVulnerabilitiesForWindows10Async(string version)
        {
            // Create a list to store the vulnerabilities.
            List<Vulnerability> vulnerabilitiesList = new List<Vulnerability>();

            // Get the NIST API key from the configuration.
            string apiKey = App.Configuration["ApiKeys:NistApiKey"];

            // Create a new HttpClient with a timeout of 60 seconds.
            HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(60)
            };

            // Add the API key to the request headers and create the query.
            client.DefaultRequestHeaders.Add("apiKey", apiKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Scottish Glen");

            // Create an array of severities to check for.
            string[] severities = { "CRITICAL", "HIGH" };

            // Iterate through the severities and create a query for each one.
            foreach (var severity in severities)
            {
                // Create the query for the NIST NVD API.
                string query = $"https://services.nvd.nist.gov/rest/json/cves/2.0?cpeName=cpe:2.3:o:microsoft:windows_10:{version}&cvssV3Severity={severity}";

                // Send the request to the NIST NVD API and store the status code and reason phrase.
                HttpResponseMessage response = await client.GetAsync(query);
                var statusCode = (int)response.StatusCode;
                var reasonPhrase = response.ReasonPhrase;

                // Check if the request was successful and parse the JSON response, adding the vulnerabilities to the list, or return null if there are no vulnerabilities.
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(jsonResponse);

                    var vulnerabilities = json["vulnerabilities"];
                    if (vulnerabilities == null || !vulnerabilities.HasValues)
                    {
                        return null;
                    }

                    // Add the vulnerabilities to the list using the CveId, Description, and Severity.
                    foreach (var vulnerability in vulnerabilities)
                    {
                        vulnerabilitiesList.Add(new Vulnerability
                        {
                            CveId = vulnerability["cve"]["id"]?.ToString(),
                            Description = vulnerability["cve"]["descriptions"]?[0]?["value"]?.ToString(),
                            Severity = vulnerability["cve"]["metrics"]?["cvssMetricV30"]?[0]?["cvssData"]?["baseSeverity"]?.ToString() ?? vulnerability["cve"]["metrics"]?["cvssMetricV31"]?[0]?["cvssData"]?["baseSeverity"]?.ToString()
                        });
                    }
                }
                else
                {
                    throw new HttpRequestException($"Failed to retrieve vulnerabilities: Error {statusCode} {reasonPhrase}.");
                }
            }

            return vulnerabilitiesList;
        }

        /// <summary>
        /// Method to get vulnerabilities for a specific version of Windows 11 from the NIST NVD API.
        /// </summary>
        /// <param name="version">Version to be checked.</param>
        /// <returns>List of Vulnerabilities for the specific version of Windows 11, or null if there are no vulnerabilities.</returns>
        /// <exception cref="HttpRequestException">Thrown when the request to the NIST NVD API fails.</exception>
        private async Task<List<Vulnerability>> GetVulnerabilitiesForWindows11Async(string version)
        {
            // Windows 11 currently has no CRITICAL vulnerabilities, so we just check for all vulnerabilities, and filter in the application.

            // Create a list to store the vulnerabilities.
            List<Vulnerability> vulnerabilitiesList = new List<Vulnerability>();

            // Get the NIST API key from the configuration.
            string apiKey = App.Configuration["ApiKeys:NistApiKey"];

            // Create a new HttpClient with a timeout of 60 seconds.
            HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(60)
            };

            // Add the API key to the request headers and create the query.
            client.DefaultRequestHeaders.Add("apiKey", apiKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Scottish Glen");

            // Create the query for the NIST NVD API.
            string query = $"https://services.nvd.nist.gov/rest/json/cves/2.0?cpeName=cpe:2.3:o:microsoft:windows_11:{version}";

            // Send the request to the NIST NVD API and store the status code and reason phrase.
            HttpResponseMessage response = await client.GetAsync(query);
            var statusCode = (int)response.StatusCode;
            var reasonPhrase = response.ReasonPhrase;

            // Check if the request was successful and parse the JSON response, adding the vulnerabilities to the list, or return null if there are no vulnerabilities.
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonResponse);

                var vulnerabilities = json["vulnerabilities"];
                if (vulnerabilities == null || !vulnerabilities.HasValues)
                {
                    return null;
                }

                // Add the vulnerabilities to the list using the CveId, Description, and Severity.
                foreach (var vulnerability in vulnerabilities)
                {
                    // Get the severity from the cvssMetricV3.0 or cvssMetricV3.1 objects.
                    string severity = vulnerability["cve"]["metrics"]?["cvssMetricV30"]?[0]?["cvssData"]?["baseSeverity"]?.ToString() ??
                        vulnerability["cve"]["metrics"]?["cvssMetricV31"]?[0]?["cvssData"]?["baseSeverity"]?.ToString();

                    // Add the vulnerability to the list if the severity is HIGH or CRITICAL.
                    if (severity.Equals("HIGH") || severity.Equals("CRITICAL"))
                    {
                        vulnerabilitiesList.Add(new Vulnerability
                        {
                            CveId = vulnerability["cve"]["id"]?.ToString(),
                            Description = vulnerability["cve"]["descriptions"]?[0]?["value"]?.ToString(),
                            Severity = severity
                        });
                    }
                }
            }
            else
            {
                throw new HttpRequestException($"Failed to retrieve vulnerabilities: Error {statusCode} {reasonPhrase}.");
            }

            return vulnerabilitiesList;
        }

        /// <summary>
        /// Method to get an existing SoftwareAsset from the database by its name and version.
        /// </summary>
        /// <param name="name">Name of the software.</param>
        /// <param name="version">Version of the software.</param>
        /// <returns>SoftwareAsset from the database with the provided name and version.</returns>
        public SoftwareAsset GetExistingSoftwareAsset(string name, string version) => _context.SoftwareAssets.Include(a => a.HardwareAssets).FirstOrDefault(a => a.Name == name && a.Version == version);

        /// <summary>
        /// Method to check if a SoftwareAsset already exists in the database.
        /// </summary>
        /// <param name="softwareAsset">SoftwareAsset to check if it already exists in the database.</param>
        /// <returns>True if the SoftwareAsset already exists in the database, false if not.</returns>
        private bool DoesSoftwareAssetExist(SoftwareAsset softwareAsset)
        {
            // Check if the SoftwareAsset already exists in the database.
            SoftwareAsset existingSoftwareAsset = GetExistingSoftwareAsset(softwareAsset.Name, softwareAsset.Version);

            // Return true if the SoftwareAsset already exists in the database, false if not.
            return existingSoftwareAsset != null;
        }

        /// <summary>
        /// Helper method to link HardwareAssets to an existing SoftwareAsset in the database.
        /// </summary>
        /// <param name="softwareAsset">SoftwareAsset to link the HardwareAssets to.</param>
        /// <returns>True if linked, false if not.</returns>
        private bool LinkHardwareAssetsToSoftwareAsset(SoftwareAsset softwareAsset)
        {
            // Get the existing SoftwareAsset from the database.
            SoftwareAsset existingSoftwareAsset = GetExistingSoftwareAsset(softwareAsset.Name, softwareAsset.Version);

            // Set each HardwareAssets's SoftwareAsset to the existing SoftwareAsset in the database if there are any.
            if (softwareAsset.HardwareAssets != null)
            {
                foreach (var hardwareAsset in softwareAsset.HardwareAssets)
                {
                    HardwareAsset existingHardwareAsset = _context.HardwareAssets.Find(hardwareAsset.Id);
                    existingHardwareAsset.SoftwareAsset = existingSoftwareAsset;
                    existingHardwareAsset.SoftwareLinkDate = DateTime.Now;
                    _context.HardwareAssets.Attach(existingHardwareAsset);
                }
            }

            if (_context.SoftwareAssets.Contains(softwareAsset))
            {
                _context.SoftwareAssets.Remove(softwareAsset);
            }

            // Save the changes to the database and return true.
            _context.SaveChanges();
            return true;
        }
    }
}
