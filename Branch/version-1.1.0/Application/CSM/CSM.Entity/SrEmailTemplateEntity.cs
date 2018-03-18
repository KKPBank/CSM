using System;

namespace CSM.Entity
{
    [Serializable]
    public class SrEmailTemplateEntity
    {
        public int SrEmailTemplateId { get; set; }
        public string Name { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}