using SharpCifs.Smb;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BASTION.Resources.Helper
{
    public class SmbHelper
    {
        private readonly string _smbPath;
        private readonly string _username;
        private readonly string _password;

        public SmbHelper(string smbPath, string username, string password)
        {
            _smbPath = smbPath ?? throw new ArgumentNullException(nameof(smbPath));
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public async Task<string> GetFileContentsAsync(string filePath)
        {
            try
            {
                // Set up SMB authentication
                var auth = new NtlmPasswordAuthentication(null, _username, _password);

                // Connect to the SMB share
                var smbFile = new SmbFile($"{_smbPath}/{filePath}", auth);

                if (!smbFile.Exists())
                {
                    throw new FileNotFoundException("File not found on SMB share.");
                }

                // Open and read the file from the SMB share
                using (var inputStream = smbFile.GetInputStream())
                using (var reader = new StreamReader(inputStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException($"Error accessing SMB share: {ex.Message}", ex);
            }
        }
    }
}
