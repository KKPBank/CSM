namespace CSM.Entity
{
    public class UserMarketingEntity
    {
        public int CustomerId { get; set; }

        public UserEntity UserEntity { get; set; }

        public BranchEntity BranchEntity { get; set; }
        public BranchEntity UpperBranch1 { get; set; }
        public BranchEntity UpperBranch2 { get; set; }
    }
}