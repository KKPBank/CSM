using System;

namespace CSM.Common.Mail
{
    [Serializable]
    public sealed class EmailAddress
    {
        public EmailAddress()
        {
        }

        public EmailAddress(string address, string name)
        {
            Address = address;
            Name = name;
        }

        public string Address { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}