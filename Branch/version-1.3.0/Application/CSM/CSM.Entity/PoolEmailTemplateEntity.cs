using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSM.Entity
{
    [Serializable]
    public class PoolEmailTemplateEntity
    {
        public int PoolEmailTemplateId { get; set; }
        public string Name { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}