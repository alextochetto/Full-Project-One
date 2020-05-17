namespace IdentityProvider.Configuration
{
    public class AppSettings
    {
        public bool UseInMemory { get; set; }
        public bool UseAzure { get; set; }
        public string DigitalCertificateRsaFileName { get; set; }
        public string DigitalCertificatePfxFileName { get; set; }
        public string DigitalCertificatePfxPassword { get; set; }
        public string DigitalCertificateThumbPrint { get; set; }
    }
}