using System;
using System.IO;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Web.Configuration;

namespace LocalSmtpMailHelperLib
{
    //originally from http://www.singular.co.nz/2007/12/programmatically-setting-the-smtpclient-pickup-directory-location-at-runtime/
    public static class LocalSmtpMailHelper
    {
        private static bool? _isUsingPickupDirectory;

        /// <summary>
        /// Gets a value to indicate if the default SMTP Delivery
        /// method is SpecifiedPickupDirectory
        /// </summary>
        public static bool IsUsingPickupDirectory
        {
            get
            {
                if (_isUsingPickupDirectory.HasValue) return _isUsingPickupDirectory.Value;
                var config = WebConfigurationManager.OpenWebConfiguration("~/web.config");
                var mail = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
                _isUsingPickupDirectory = mail.Smtp.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory;
                return _isUsingPickupDirectory.Value;
            }
        }

        /// <summary>
        /// Sets the default PickupDirectoryLocation for the SmtpClient.
        /// </summary>
        /// <remarks>
        /// This method should be called to set the PickupDirectoryLocation
        /// for the SmtpClient at runtime (Application_Start)
        ///
        /// Reflection is used to set the private variable located in the
        /// internal class for the SmtpClient's mail configuration:
        /// System.Net.Mail.SmtpClient.MailConfiguration.Smtp.SpecifiedPickupDirectory.PickupDirectoryLocation
        ///
        /// The folder must exist.
        /// </remarks>
        /// <param name="path"></param>
        public static void SetPickupDirectoryLocation(string path)
        {
            const BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            // get static internal property: MailConfiguration
            var prop = typeof(SmtpClient).GetProperty("MailConfiguration", BindingFlags.Static | BindingFlags.NonPublic);
            var mailConfiguration = prop.GetValue(null, null);

            // get internal property: Smtp
            prop = mailConfiguration.GetType().GetProperty("Smtp", instanceFlags);
            var smtp = prop.GetValue(mailConfiguration, null);

            // get internal property: SpecifiedPickupDirectory
            prop = smtp.GetType().GetProperty("SpecifiedPickupDirectory", instanceFlags);
            var specifiedPickupDirectory = prop.GetValue(smtp, null);

            // get private field: pickupDirectoryLocation, then set it to the supplied path
            var field = specifiedPickupDirectory.GetType().GetField("pickupDirectoryLocation", instanceFlags);
            field.SetValue(specifiedPickupDirectory, path);
        }

        /// <summary>
        /// Sets the default PickupDirectoryLocation for the SmtpClient
        /// to the relative path from the current web root.
        /// </summary>
        /// <param name="path">Relative path to the web root</param>
        public static void SetRelativePickupDirectoryLocation(string path)
        {
            SetPickupDirectoryLocation(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        /// <summary>
        /// Sets the default PickupDirectoryLocation for the SmtpClient.
        /// </summary>
        /// <remarks>
        /// This is a shortcut for passing in two paths, which are then
        /// combined to set the pickup directory.
        /// </remarks>
        /// <param name="path1">Base path</param>
        /// <param name="path3">Relative path to be combined with </param>
        public static void SetPickupDirectoryLocation(string path1, string path3)
        {
            var path = Path.Combine(path1, path3);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            SetPickupDirectoryLocation(path);
        }
    }
}