using System;

namespace CSM.Entity
{
    [Serializable]
    public class ChannelEntity
    {
        public int ChannelId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ChannelEntity(string code)
        {
            this.Code = code;
        }

        public ChannelEntity()
        {
            
        }
    }
}
