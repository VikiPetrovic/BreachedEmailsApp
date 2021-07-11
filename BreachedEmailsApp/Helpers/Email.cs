using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreachedEmailsApp.Helpers
{
    public class Email
    {
        public string address { get; set; }
        public string domain { get; set; }

        public Email(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email is null. ");
            }
            this.address = email.Trim();
            if (!IsValidEmail())
            {
                throw new Exception("Invalid email address: " + this.address);
            }
            ExtractDomain();
        }

        private void ExtractDomain()
        {
            this.domain = this.address.Split('@')[1];
        }

        private bool IsValidEmail()
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(this.address);
                return addr.Address == this.address;
            }
            catch
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            return this.address.Equals(((Email)obj).address);
        }

        public override int GetHashCode()
        {
            return this.address.GetHashCode();
        }
    }
}
