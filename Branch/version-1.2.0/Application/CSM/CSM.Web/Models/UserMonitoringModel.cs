using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSM.Entity;
using CSM.Common.Utilities;

namespace CSM.Web.Models
{
    public class UserMonitoringModel
    {
        public List<UserAssignInformationModel> UserAssignInformation { get; set; }
        public List<GroupAssignInformationModel> GroupAssignInformation { get; set; }
        public List<BranchEntity> Branchs { get; set; }
        public List<SelectListItem> BranchSelectListItems
        {
            get
            {
                if (Branchs == null)
                    return new List<SelectListItem>();
                return Branchs.Select(b => new SelectListItem() { Text = b.BranchName, Value = b.BranchId + "" }).ToList();
            }
        }
    }

    public class UserAssignInformationModel
    {
        public string Role { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string BranchName { get; set; }
        public IEnumerable<ServiceRequestEntity> ServiceRequests { get; set; }

        public int Draft
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.Draft)
                        + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.Draft);
                }
                return 0;
            }
        }
        public int Open
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.Open)
                                + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.Open);
                }
                return 0;
            }
        }

        public int WaitingCustomer
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.WaitingCustomer)
                                + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.WaitingCustomer);
                }

                return 0;
            }
        }
        public int InProgress
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.InProgress)
                                + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.InProgress);
                }
                return 0;
            }
        }

        public int ReturnEditor
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.RouteBack)
                                + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.RouteBack);
                }
                return 0;
            }
        }

        public int Summary { get { return Draft + Open + WaitingCustomer + InProgress + ReturnEditor; } }

    }
    public class GroupAssignInformationModel
    {
        public int UserId { get; set; }
        public string UserIds { get; set; }
        public string BranchCodeTeam { get; set; }
        public string BranchNameTeam { get; set; }
        public string Email { get; set; }
        public IEnumerable<ServiceRequestEntity> ServiceRequests { get; set; }

        public int Draft
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.Draft)
                        + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.Draft);
                }
                return 0;
            }
        }
        public int Open
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.Open)
                                + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.Open);
                }
                return 0;
            }
        }

        public int WaitingCustomer
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.WaitingCustomer)
                                + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.WaitingCustomer);
                }

                return 0;
            }
        }
        public int InProgress
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.InProgress)
                                + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.InProgress);
                }
                return 0;
            }
        }

        public int ReturnEditor
        {
            get
            {
                if (ServiceRequests != null && ServiceRequests.Count() > 0)
                {
                    return ServiceRequests.Count(x => x.OwnerUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.RouteBack)
                                + ServiceRequests.Count(x => x.DelegateUserId == UserId && x.SrStatusCode == Constants.SRStatusCode.RouteBack);
                }
                return 0;
            }
        }

        public int Summary { get { return Draft + Open + WaitingCustomer + InProgress + ReturnEditor; } }
    }
}