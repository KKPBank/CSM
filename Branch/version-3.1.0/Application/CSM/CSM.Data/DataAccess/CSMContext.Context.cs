﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSM.Data.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CSMContext : DbContext
    {
        public CSMContext()
            : base("name=CSMContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<TB_C_ERROR> TB_C_ERROR { get; set; }
        public virtual DbSet<TB_C_FONT> TB_C_FONT { get; set; }
        public virtual DbSet<TB_C_JOB_STATUS> TB_C_JOB_STATUS { get; set; }
        public virtual DbSet<TB_C_MENU> TB_C_MENU { get; set; }
        public virtual DbSet<TB_C_PARAMETER> TB_C_PARAMETER { get; set; }
        public virtual DbSet<TB_C_ROLE> TB_C_ROLE { get; set; }
        public virtual DbSet<TB_C_ROLE_SCREEN> TB_C_ROLE_SCREEN { get; set; }
        public virtual DbSet<TB_C_ROLE_SR_PAGE> TB_C_ROLE_SR_PAGE { get; set; }
        public virtual DbSet<TB_C_RULE_OPTION> TB_C_RULE_OPTION { get; set; }
        public virtual DbSet<TB_C_SCREEN> TB_C_SCREEN { get; set; }
        public virtual DbSet<TB_C_SR_ACTIVITY_TYPE> TB_C_SR_ACTIVITY_TYPE { get; set; }
        public virtual DbSet<TB_C_SR_EMAIL_TEMPLATE> TB_C_SR_EMAIL_TEMPLATE { get; set; }
        public virtual DbSet<TB_C_SR_PAGE> TB_C_SR_PAGE { get; set; }
        public virtual DbSet<TB_C_SR_STATUS> TB_C_SR_STATUS { get; set; }
        public virtual DbSet<TB_C_SR_STATUS_CHANGE> TB_C_SR_STATUS_CHANGE { get; set; }
        public virtual DbSet<TB_I_HP_ACTIVITY> TB_I_HP_ACTIVITY { get; set; }
        public virtual DbSet<TB_I_PROPERTY> TB_I_PROPERTY { get; set; }
        public virtual DbSet<TB_I_SALE_ZONE> TB_I_SALE_ZONE { get; set; }
        public virtual DbSet<TB_L_AUDIT_LOG> TB_L_AUDIT_LOG { get; set; }
        public virtual DbSet<TB_L_LOGIN> TB_L_LOGIN { get; set; }
        public virtual DbSet<TB_L_SR_LOGGING> TB_L_SR_LOGGING { get; set; }
        public virtual DbSet<TB_M_ACCOUNT> TB_M_ACCOUNT { get; set; }
        public virtual DbSet<TB_M_ACCOUNT_ADDRESS> TB_M_ACCOUNT_ADDRESS { get; set; }
        public virtual DbSet<TB_M_ACCOUNT_EMAIL> TB_M_ACCOUNT_EMAIL { get; set; }
        public virtual DbSet<TB_M_ACCOUNT_PHONE> TB_M_ACCOUNT_PHONE { get; set; }
        public virtual DbSet<TB_M_ADDRESS> TB_M_ADDRESS { get; set; }
        public virtual DbSet<TB_M_AREA> TB_M_AREA { get; set; }
        public virtual DbSet<TB_M_AREA_SUBAREA> TB_M_AREA_SUBAREA { get; set; }
        public virtual DbSet<TB_M_CONFIG_ROLE> TB_M_CONFIG_ROLE { get; set; }
        public virtual DbSet<TB_M_CONFIG_URL> TB_M_CONFIG_URL { get; set; }
        public virtual DbSet<TB_M_CONTACT_PHONE> TB_M_CONTACT_PHONE { get; set; }
        public virtual DbSet<TB_M_DOCUMENT_TYPE> TB_M_DOCUMENT_TYPE { get; set; }
        public virtual DbSet<TB_M_MAP_AUTO_FORWARD> TB_M_MAP_AUTO_FORWARD { get; set; }
        public virtual DbSet<TB_M_MAP_PRODUCT> TB_M_MAP_PRODUCT { get; set; }
        public virtual DbSet<TB_M_MAP_PRODUCT_QUESTIONGROUP> TB_M_MAP_PRODUCT_QUESTIONGROUP { get; set; }
        public virtual DbSet<TB_M_MEDIA_SOURCE> TB_M_MEDIA_SOURCE { get; set; }
        public virtual DbSet<TB_M_PHONE> TB_M_PHONE { get; set; }
        public virtual DbSet<TB_M_PHONE_TYPE> TB_M_PHONE_TYPE { get; set; }
        public virtual DbSet<TB_M_POOL> TB_M_POOL { get; set; }
        public virtual DbSet<TB_M_POOL_BRANCH> TB_M_POOL_BRANCH { get; set; }
        public virtual DbSet<TB_M_QUESTION> TB_M_QUESTION { get; set; }
        public virtual DbSet<TB_M_QUESTIONGROUP> TB_M_QUESTIONGROUP { get; set; }
        public virtual DbSet<TB_M_QUESTIONGROUP_QUESTION> TB_M_QUESTIONGROUP_QUESTION { get; set; }
        public virtual DbSet<TB_M_SLA> TB_M_SLA { get; set; }
        public virtual DbSet<TB_M_SUBAREA> TB_M_SUBAREA { get; set; }
        public virtual DbSet<TB_M_TYPE> TB_M_TYPE { get; set; }
        public virtual DbSet<TB_R_BRANCH> TB_R_BRANCH { get; set; }
        public virtual DbSet<TB_R_BRANCH_CALENDAR> TB_R_BRANCH_CALENDAR { get; set; }
        public virtual DbSet<TB_R_CAMPAIGNSERVICE> TB_R_CAMPAIGNSERVICE { get; set; }
        public virtual DbSet<TB_R_CHANNEL> TB_R_CHANNEL { get; set; }
        public virtual DbSet<TB_R_PRODUCT> TB_R_PRODUCT { get; set; }
        public virtual DbSet<TB_R_PRODUCTGROUP> TB_R_PRODUCTGROUP { get; set; }
        public virtual DbSet<TB_R_USER> TB_R_USER { get; set; }
        public virtual DbSet<TB_R_USER_PHONE> TB_R_USER_PHONE { get; set; }
        public virtual DbSet<TB_T_ATTACHMENT_TYPE> TB_T_ATTACHMENT_TYPE { get; set; }
        public virtual DbSet<TB_T_CALL_INFO> TB_T_CALL_INFO { get; set; }
        public virtual DbSet<TB_T_DEFAULT_SEARCH> TB_T_DEFAULT_SEARCH { get; set; }
        public virtual DbSet<TB_T_JOB_ATTACHMENT> TB_T_JOB_ATTACHMENT { get; set; }
        public virtual DbSet<TB_T_NEWS> TB_T_NEWS { get; set; }
        public virtual DbSet<TB_T_NEWS_ATTACHMENT> TB_T_NEWS_ATTACHMENT { get; set; }
        public virtual DbSet<TB_T_NEWS_BRANCH> TB_T_NEWS_BRANCH { get; set; }
        public virtual DbSet<TB_T_READ_NEWS> TB_T_READ_NEWS { get; set; }
        public virtual DbSet<TB_T_SR_ACTIVITY> TB_T_SR_ACTIVITY { get; set; }
        public virtual DbSet<TB_T_SR_ATTACHMENT> TB_T_SR_ATTACHMENT { get; set; }
        public virtual DbSet<TB_T_SR_ATTACHMENT_DOCUMENT_TYPE> TB_T_SR_ATTACHMENT_DOCUMENT_TYPE { get; set; }
        public virtual DbSet<TB_T_SR_PREPARE_EMAIL> TB_T_SR_PREPARE_EMAIL { get; set; }
        public virtual DbSet<TB_T_SR_REPLY_EMAIL> TB_T_SR_REPLY_EMAIL { get; set; }
        public virtual DbSet<TB_T_SR_VERIFY_RESULT_GROUP> TB_T_SR_VERIFY_RESULT_GROUP { get; set; }
        public virtual DbSet<TB_T_SR_VERIFY_RESULT_QUESTION> TB_T_SR_VERIFY_RESULT_QUESTION { get; set; }
        public virtual DbSet<TB_C_SEQUENCE> TB_C_SEQUENCE { get; set; }
        public virtual DbSet<TB_C_SEQUENCE_SR> TB_C_SEQUENCE_SR { get; set; }
        public virtual DbSet<TB_C_PROCESS> TB_C_PROCESS { get; set; }
        public virtual DbSet<TB_I_CIS_ADDRESS_TYPE> TB_I_CIS_ADDRESS_TYPE { get; set; }
        public virtual DbSet<TB_I_CIS_CORPORATE> TB_I_CIS_CORPORATE { get; set; }
        public virtual DbSet<TB_I_CIS_COUNTRY> TB_I_CIS_COUNTRY { get; set; }
        public virtual DbSet<TB_I_CIS_CUSTOMER_EMAIL> TB_I_CIS_CUSTOMER_EMAIL { get; set; }
        public virtual DbSet<TB_I_CIS_CUSTOMER_PHONE> TB_I_CIS_CUSTOMER_PHONE { get; set; }
        public virtual DbSet<TB_I_CIS_DISTRICT> TB_I_CIS_DISTRICT { get; set; }
        public virtual DbSet<TB_I_CIS_EMAIL_TYPE> TB_I_CIS_EMAIL_TYPE { get; set; }
        public virtual DbSet<TB_I_CIS_INDIVIDUAL> TB_I_CIS_INDIVIDUAL { get; set; }
        public virtual DbSet<TB_I_CIS_PHONE_TYPE> TB_I_CIS_PHONE_TYPE { get; set; }
        public virtual DbSet<TB_I_CIS_PRODUCT_GROUP> TB_I_CIS_PRODUCT_GROUP { get; set; }
        public virtual DbSet<TB_I_CIS_PROVINCE> TB_I_CIS_PROVINCE { get; set; }
        public virtual DbSet<TB_I_CIS_SUBDISTRICT> TB_I_CIS_SUBDISTRICT { get; set; }
        public virtual DbSet<TB_I_CIS_SUBSCRIPTION> TB_I_CIS_SUBSCRIPTION { get; set; }
        public virtual DbSet<TB_I_CIS_SUBSCRIPTION_TYPE> TB_I_CIS_SUBSCRIPTION_TYPE { get; set; }
        public virtual DbSet<TB_I_CIS_TITLE> TB_I_CIS_TITLE { get; set; }
        public virtual DbSet<TB_I_BDW_CONTACT> TB_I_BDW_CONTACT { get; set; }
        public virtual DbSet<TB_M_AFS_ASSET> TB_M_AFS_ASSET { get; set; }
        public virtual DbSet<TB_T_JOB> TB_T_JOB { get; set; }
        public virtual DbSet<TB_I_CIS_SUBSCRIPTION_ADDRESS> TB_I_CIS_SUBSCRIPTION_ADDRESS { get; set; }
        public virtual DbSet<TB_I_CIS_SUBSCRIPTION_EMAIL> TB_I_CIS_SUBSCRIPTION_EMAIL { get; set; }
        public virtual DbSet<TB_I_CIS_SUBSCRIPTION_PHONE> TB_I_CIS_SUBSCRIPTION_PHONE { get; set; }
        public virtual DbSet<TB_T_SR> TB_T_SR { get; set; }
        public virtual DbSet<TB_C_SEQ_SEND_OTP> TB_C_SEQ_SEND_OTP { get; set; }
        public virtual DbSet<TB_C_SR_PAGE_STATUS> TB_C_SR_PAGE_STATUS { get; set; }
        public virtual DbSet<TB_C_SR_STATE> TB_C_SR_STATE { get; set; }
        public virtual DbSet<TB_I_HR_EMPLOYEE> TB_I_HR_EMPLOYEE { get; set; }
        public virtual DbSet<TB_M_COMPLAINT_BU_GROUP> TB_M_COMPLAINT_BU_GROUP { get; set; }
        public virtual DbSet<TB_M_COMPLAINT_CAUSE_SUMMARY> TB_M_COMPLAINT_CAUSE_SUMMARY { get; set; }
        public virtual DbSet<TB_M_COMPLAINT_ISSUES> TB_M_COMPLAINT_ISSUES { get; set; }
        public virtual DbSet<TB_M_COMPLAINT_MAPPING> TB_M_COMPLAINT_MAPPING { get; set; }
        public virtual DbSet<TB_M_COMPLAINT_ROOT_CAUSE> TB_M_COMPLAINT_ROOT_CAUSE { get; set; }
        public virtual DbSet<TB_M_COMPLAINT_SUBJECT> TB_M_COMPLAINT_SUBJECT { get; set; }
        public virtual DbSet<TB_M_COMPLAINT_SUMMARY> TB_M_COMPLAINT_SUMMARY { get; set; }
        public virtual DbSet<TB_M_COMPLAINT_TYPE> TB_M_COMPLAINT_TYPE { get; set; }
        public virtual DbSet<TB_M_HP_STATUS> TB_M_HP_STATUS { get; set; }
        public virtual DbSet<TB_M_HR_BU> TB_M_HR_BU { get; set; }
        public virtual DbSet<TB_M_MSH_BRANCH> TB_M_MSH_BRANCH { get; set; }
        public virtual DbSet<TB_M_OTP_TEMPLATE> TB_M_OTP_TEMPLATE { get; set; }
        public virtual DbSet<TB_T_SEND_OTP> TB_T_SEND_OTP { get; set; }
        public virtual DbSet<TB_M_HR_EMPLOYEE> TB_M_HR_EMPLOYEE { get; set; }
        public virtual DbSet<TB_T_SR_ACCOUNT> TB_T_SR_ACCOUNT { get; set; }
        public virtual DbSet<TB_T_SR_CUSTOMER_PHONE> TB_T_SR_CUSTOMER_PHONE { get; set; }
        public virtual DbSet<TB_L_CUSTOMER_LOG> TB_L_CUSTOMER_LOG { get; set; }
        public virtual DbSet<TB_M_CUSTOMER_CONTACT> TB_M_CUSTOMER_CONTACT { get; set; }
        public virtual DbSet<TB_M_NOTE> TB_M_NOTE { get; set; }
        public virtual DbSet<TB_M_CUSTOMER_ATTACHMENT> TB_M_CUSTOMER_ATTACHMENT { get; set; }
        public virtual DbSet<TB_M_BANK> TB_M_BANK { get; set; }
        public virtual DbSet<TB_C_IGNORE_PRODUCT> TB_C_IGNORE_PRODUCT { get; set; }
        public virtual DbSet<TB_M_SUBSCRIPT_TYPE> TB_M_SUBSCRIPT_TYPE { get; set; }
        public virtual DbSet<TB_M_TITLE> TB_M_TITLE { get; set; }
        public virtual DbSet<TB_M_ACCOUNT_STATUS> TB_M_ACCOUNT_STATUS { get; set; }
        public virtual DbSet<TB_M_CARD_TYPE> TB_M_CARD_TYPE { get; set; }
        public virtual DbSet<TB_M_ADDRESS_FORMAT> TB_M_ADDRESS_FORMAT { get; set; }
        public virtual DbSet<TB_M_ADDRESS_TYPE> TB_M_ADDRESS_TYPE { get; set; }
        public virtual DbSet<TB_M_AREA_CODE> TB_M_AREA_CODE { get; set; }
        public virtual DbSet<TB_M_CUST_TYPE> TB_M_CUST_TYPE { get; set; }
        public virtual DbSet<TB_M_ELECTRONIC_ADDRESS> TB_M_ELECTRONIC_ADDRESS { get; set; }
        public virtual DbSet<TB_M_CUSTOMER> TB_M_CUSTOMER { get; set; }
        public virtual DbSet<TB_M_CONTACT> TB_M_CONTACT { get; set; }
        public virtual DbSet<TB_T_SR_CUSTOMER> TB_T_SR_CUSTOMER { get; set; }
        public virtual DbSet<TB_M_BRANCH> TB_M_BRANCH { get; set; }
        public virtual DbSet<TB_M_RELATIONSHIP> TB_M_RELATIONSHIP { get; set; }
        public virtual DbSet<TB_M_DISTRICT> TB_M_DISTRICT { get; set; }
        public virtual DbSet<TB_M_PROVINCE> TB_M_PROVINCE { get; set; }
        public virtual DbSet<TB_M_SUBDISTRICT> TB_M_SUBDISTRICT { get; set; }
        public virtual DbSet<TB_M_COUNTRY> TB_M_COUNTRY { get; set; }
        public virtual DbSet<TB_M_ACCOUNT_TYPE> TB_M_ACCOUNT_TYPE { get; set; }
    
        public virtual int SP_GET_NEXT_ATTACHMENT_SEQ(ObjectParameter nextSeq)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GET_NEXT_ATTACHMENT_SEQ", nextSeq);
        }
    
        public virtual int SP_GET_NEXT_SR_SEQ(ObjectParameter nextSeq)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GET_NEXT_SR_SEQ", nextSeq);
        }
    
        public virtual int SP_IMPORT_BDW_CONTACT(ObjectParameter numOfComplete, ObjectParameter numOfError, ObjectParameter isError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_BDW_CONTACT", numOfComplete, numOfError, isError, messageError);
        }
    
        public virtual int SP_IMPORT_CIS_ACCOUNT_ADDRESS(ObjectParameter numOfComplete, ObjectParameter isError, ObjectParameter numOfError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_CIS_ACCOUNT_ADDRESS", numOfComplete, isError, numOfError, messageError);
        }
    
        public virtual int SP_IMPORT_CIS_ACCOUNT_EMAIL(ObjectParameter numOfComplete, ObjectParameter isError, ObjectParameter numOfError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_CIS_ACCOUNT_EMAIL", numOfComplete, isError, numOfError, messageError);
        }
    
        public virtual int SP_IMPORT_CIS_ACCOUNT_PHONE(ObjectParameter numOfComplete, ObjectParameter isError, ObjectParameter numOfError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_CIS_ACCOUNT_PHONE", numOfComplete, isError, numOfError, messageError);
        }
    
        public virtual int SP_IMPORT_CIS_CORPORATE(ObjectParameter numOfComplete, ObjectParameter isError, ObjectParameter numOfError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_CIS_CORPORATE", numOfComplete, isError, numOfError, messageError);
        }
    
        public virtual int SP_IMPORT_CIS_CUSTOMER_EMAIL(ObjectParameter numOfComplete, ObjectParameter isError, ObjectParameter numOfError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_CIS_CUSTOMER_EMAIL", numOfComplete, isError, numOfError, messageError);
        }
    
        public virtual int SP_IMPORT_CIS_CUSTOMER_PHONE(ObjectParameter numOfComplete, ObjectParameter isError, ObjectParameter numOfError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_CIS_CUSTOMER_PHONE", numOfComplete, isError, numOfError, messageError);
        }
    
        public virtual int SP_IMPORT_CIS_INDIVIDUAL(ObjectParameter numOfComplete, ObjectParameter isError, ObjectParameter numOfError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_CIS_INDIVIDUAL", numOfComplete, isError, numOfError, messageError);
        }
    
        public virtual int SP_IMPORT_CIS_SUBSCRIPTION(ObjectParameter numOfComplete, ObjectParameter isError, ObjectParameter numOfError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_CIS_SUBSCRIPTION", numOfComplete, isError, numOfError, messageError);
        }
    
        public virtual int SP_IMPORT_CIS_SUBSCRIPTION_TYPE(ObjectParameter numOfComplete, ObjectParameter isError, ObjectParameter numOfError, ObjectParameter messageError)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_CIS_SUBSCRIPTION_TYPE", numOfComplete, isError, numOfError, messageError);
        }
    
        public virtual int SP_GET_CUSTOMER_COUNT(string customerType, string firstName, string lastName, string registration, string cardNo, string accountNo, string product, string grade, string branchName, string status, string phoneNo, ObjectParameter totalRecords)
        {
            var customerTypeParameter = customerType != null ?
                new ObjectParameter("CustomerType", customerType) :
                new ObjectParameter("CustomerType", typeof(string));
    
            var firstNameParameter = firstName != null ?
                new ObjectParameter("FirstName", firstName) :
                new ObjectParameter("FirstName", typeof(string));
    
            var lastNameParameter = lastName != null ?
                new ObjectParameter("LastName", lastName) :
                new ObjectParameter("LastName", typeof(string));
    
            var registrationParameter = registration != null ?
                new ObjectParameter("Registration", registration) :
                new ObjectParameter("Registration", typeof(string));
    
            var cardNoParameter = cardNo != null ?
                new ObjectParameter("CardNo", cardNo) :
                new ObjectParameter("CardNo", typeof(string));
    
            var accountNoParameter = accountNo != null ?
                new ObjectParameter("AccountNo", accountNo) :
                new ObjectParameter("AccountNo", typeof(string));
    
            var productParameter = product != null ?
                new ObjectParameter("Product", product) :
                new ObjectParameter("Product", typeof(string));
    
            var gradeParameter = grade != null ?
                new ObjectParameter("Grade", grade) :
                new ObjectParameter("Grade", typeof(string));
    
            var branchNameParameter = branchName != null ?
                new ObjectParameter("BranchName", branchName) :
                new ObjectParameter("BranchName", typeof(string));
    
            var statusParameter = status != null ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(string));
    
            var phoneNoParameter = phoneNo != null ?
                new ObjectParameter("PhoneNo", phoneNo) :
                new ObjectParameter("PhoneNo", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GET_CUSTOMER_COUNT", customerTypeParameter, firstNameParameter, lastNameParameter, registrationParameter, cardNoParameter, accountNoParameter, productParameter, gradeParameter, branchNameParameter, statusParameter, phoneNoParameter, totalRecords);
        }
    
        public virtual ObjectResult<SP_GET_CUSTOMER_LIST_SET_Result> SP_GET_CUSTOMER_LIST(string customerType, string firstName, string lastName, string registration, string cardNo, string accountNo, string product, string grade, string branchName, string status, string phoneNo, Nullable<int> pageNum, Nullable<int> pageSize, string sortBy)
        {
            var customerTypeParameter = customerType != null ?
                new ObjectParameter("CustomerType", customerType) :
                new ObjectParameter("CustomerType", typeof(string));
    
            var firstNameParameter = firstName != null ?
                new ObjectParameter("FirstName", firstName) :
                new ObjectParameter("FirstName", typeof(string));
    
            var lastNameParameter = lastName != null ?
                new ObjectParameter("LastName", lastName) :
                new ObjectParameter("LastName", typeof(string));
    
            var registrationParameter = registration != null ?
                new ObjectParameter("Registration", registration) :
                new ObjectParameter("Registration", typeof(string));
    
            var cardNoParameter = cardNo != null ?
                new ObjectParameter("CardNo", cardNo) :
                new ObjectParameter("CardNo", typeof(string));
    
            var accountNoParameter = accountNo != null ?
                new ObjectParameter("AccountNo", accountNo) :
                new ObjectParameter("AccountNo", typeof(string));
    
            var productParameter = product != null ?
                new ObjectParameter("Product", product) :
                new ObjectParameter("Product", typeof(string));
    
            var gradeParameter = grade != null ?
                new ObjectParameter("Grade", grade) :
                new ObjectParameter("Grade", typeof(string));
    
            var branchNameParameter = branchName != null ?
                new ObjectParameter("BranchName", branchName) :
                new ObjectParameter("BranchName", typeof(string));
    
            var statusParameter = status != null ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(string));
    
            var phoneNoParameter = phoneNo != null ?
                new ObjectParameter("PhoneNo", phoneNo) :
                new ObjectParameter("PhoneNo", typeof(string));
    
            var pageNumParameter = pageNum.HasValue ?
                new ObjectParameter("PageNum", pageNum) :
                new ObjectParameter("PageNum", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            var sortByParameter = sortBy != null ?
                new ObjectParameter("SortBy", sortBy) :
                new ObjectParameter("SortBy", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_CUSTOMER_LIST_SET_Result>("SP_GET_CUSTOMER_LIST", customerTypeParameter, firstNameParameter, lastNameParameter, registrationParameter, cardNoParameter, accountNoParameter, productParameter, gradeParameter, branchNameParameter, statusParameter, phoneNoParameter, pageNumParameter, pageSizeParameter, sortByParameter);
        }
    
        public virtual ObjectResult<SP_GET_CUSTOMER_LIST_SET_Result> SP_GET_CUSTOMER_LIST_SET()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_CUSTOMER_LIST_SET_Result>("SP_GET_CUSTOMER_LIST_SET");
        }
    
        public virtual ObjectResult<SP_VALIDATE_CREATE_SR_Result> SP_VALIDATE_CREATE_SR(string cusSubsTypeCode, string cusCardNo, string cusAccountNo, string conSubsTypeCode, string conCardNo, string conRelatName, string conAccountNo, string campServCode, string areaCode, string subAreaCode, string typeCode, string channelCode, string mediaSrcName, string creatorEmpCode, string ownerEmpCode, string delegateEmpCode, string afsAssetNo, string ncbEmpCode, Nullable<int> srActTypeId)
        {
            var cusSubsTypeCodeParameter = cusSubsTypeCode != null ?
                new ObjectParameter("cusSubsTypeCode", cusSubsTypeCode) :
                new ObjectParameter("cusSubsTypeCode", typeof(string));
    
            var cusCardNoParameter = cusCardNo != null ?
                new ObjectParameter("cusCardNo", cusCardNo) :
                new ObjectParameter("cusCardNo", typeof(string));
    
            var cusAccountNoParameter = cusAccountNo != null ?
                new ObjectParameter("cusAccountNo", cusAccountNo) :
                new ObjectParameter("cusAccountNo", typeof(string));
    
            var conSubsTypeCodeParameter = conSubsTypeCode != null ?
                new ObjectParameter("conSubsTypeCode", conSubsTypeCode) :
                new ObjectParameter("conSubsTypeCode", typeof(string));
    
            var conCardNoParameter = conCardNo != null ?
                new ObjectParameter("conCardNo", conCardNo) :
                new ObjectParameter("conCardNo", typeof(string));
    
            var conRelatNameParameter = conRelatName != null ?
                new ObjectParameter("conRelatName", conRelatName) :
                new ObjectParameter("conRelatName", typeof(string));
    
            var conAccountNoParameter = conAccountNo != null ?
                new ObjectParameter("conAccountNo", conAccountNo) :
                new ObjectParameter("conAccountNo", typeof(string));
    
            var campServCodeParameter = campServCode != null ?
                new ObjectParameter("campServCode", campServCode) :
                new ObjectParameter("campServCode", typeof(string));
    
            var areaCodeParameter = areaCode != null ?
                new ObjectParameter("areaCode", areaCode) :
                new ObjectParameter("areaCode", typeof(string));
    
            var subAreaCodeParameter = subAreaCode != null ?
                new ObjectParameter("subAreaCode", subAreaCode) :
                new ObjectParameter("subAreaCode", typeof(string));
    
            var typeCodeParameter = typeCode != null ?
                new ObjectParameter("typeCode", typeCode) :
                new ObjectParameter("typeCode", typeof(string));
    
            var channelCodeParameter = channelCode != null ?
                new ObjectParameter("channelCode", channelCode) :
                new ObjectParameter("channelCode", typeof(string));
    
            var mediaSrcNameParameter = mediaSrcName != null ?
                new ObjectParameter("mediaSrcName", mediaSrcName) :
                new ObjectParameter("mediaSrcName", typeof(string));
    
            var creatorEmpCodeParameter = creatorEmpCode != null ?
                new ObjectParameter("creatorEmpCode", creatorEmpCode) :
                new ObjectParameter("creatorEmpCode", typeof(string));
    
            var ownerEmpCodeParameter = ownerEmpCode != null ?
                new ObjectParameter("ownerEmpCode", ownerEmpCode) :
                new ObjectParameter("ownerEmpCode", typeof(string));
    
            var delegateEmpCodeParameter = delegateEmpCode != null ?
                new ObjectParameter("delegateEmpCode", delegateEmpCode) :
                new ObjectParameter("delegateEmpCode", typeof(string));
    
            var afsAssetNoParameter = afsAssetNo != null ?
                new ObjectParameter("afsAssetNo", afsAssetNo) :
                new ObjectParameter("afsAssetNo", typeof(string));
    
            var ncbEmpCodeParameter = ncbEmpCode != null ?
                new ObjectParameter("ncbEmpCode", ncbEmpCode) :
                new ObjectParameter("ncbEmpCode", typeof(string));
    
            var srActTypeIdParameter = srActTypeId.HasValue ?
                new ObjectParameter("srActTypeId", srActTypeId) :
                new ObjectParameter("srActTypeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_VALIDATE_CREATE_SR_Result>("SP_VALIDATE_CREATE_SR", cusSubsTypeCodeParameter, cusCardNoParameter, cusAccountNoParameter, conSubsTypeCodeParameter, conCardNoParameter, conRelatNameParameter, conAccountNoParameter, campServCodeParameter, areaCodeParameter, subAreaCodeParameter, typeCodeParameter, channelCodeParameter, mediaSrcNameParameter, creatorEmpCodeParameter, ownerEmpCodeParameter, delegateEmpCodeParameter, afsAssetNoParameter, ncbEmpCodeParameter, srActTypeIdParameter);
        }
    
        public virtual int SP_GET_NEXT_OTP_SEQ(ObjectParameter o_currDate, ObjectParameter o_nextSeq, ObjectParameter o_succ, ObjectParameter o_msg)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_GET_NEXT_OTP_SEQ", o_currDate, o_nextSeq, o_succ, o_msg);
        }
    
        public virtual int SP_IMPORT_HRIS(ObjectParameter o_empInsert, ObjectParameter o_empUpdate, ObjectParameter o_empMarkDelete, ObjectParameter o_buInsert, ObjectParameter o_buUpdate, ObjectParameter o_buMarkDelete, ObjectParameter o_brInsert, ObjectParameter o_brUpdate, ObjectParameter o_brMarkDelete, ObjectParameter o_succ, ObjectParameter o_msg)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_IMPORT_HRIS", o_empInsert, o_empUpdate, o_empMarkDelete, o_buInsert, o_buUpdate, o_buMarkDelete, o_brInsert, o_brUpdate, o_brMarkDelete, o_succ, o_msg);
        }
    
        public virtual ObjectResult<SP_GET_CUSTOMER_BY_PHONE_Result> SP_GET_CUSTOMER_BY_PHONE(Nullable<int> customerId, string phoneNo1, string phoneNo2, string phoneNo3)
        {
            var customerIdParameter = customerId.HasValue ?
                new ObjectParameter("CustomerId", customerId) :
                new ObjectParameter("CustomerId", typeof(int));
    
            var phoneNo1Parameter = phoneNo1 != null ?
                new ObjectParameter("PhoneNo1", phoneNo1) :
                new ObjectParameter("PhoneNo1", typeof(string));
    
            var phoneNo2Parameter = phoneNo2 != null ?
                new ObjectParameter("PhoneNo2", phoneNo2) :
                new ObjectParameter("PhoneNo2", typeof(string));
    
            var phoneNo3Parameter = phoneNo3 != null ?
                new ObjectParameter("PhoneNo3", phoneNo3) :
                new ObjectParameter("PhoneNo3", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_CUSTOMER_BY_PHONE_Result>("SP_GET_CUSTOMER_BY_PHONE", customerIdParameter, phoneNo1Parameter, phoneNo2Parameter, phoneNo3Parameter);
        }
    
        public virtual ObjectResult<SP_GET_CONTACT_LIST_Result> SP_GET_CONTACT_LIST(Nullable<int> contactId, string firstNameTh, string lastNameTh, string firstNameEn, string lastNameEn, string phoneNo1, string phoneNo2, string phoneNo3)
        {
            var contactIdParameter = contactId.HasValue ?
                new ObjectParameter("ContactId", contactId) :
                new ObjectParameter("ContactId", typeof(int));
    
            var firstNameThParameter = firstNameTh != null ?
                new ObjectParameter("FirstNameTh", firstNameTh) :
                new ObjectParameter("FirstNameTh", typeof(string));
    
            var lastNameThParameter = lastNameTh != null ?
                new ObjectParameter("LastNameTh", lastNameTh) :
                new ObjectParameter("LastNameTh", typeof(string));
    
            var firstNameEnParameter = firstNameEn != null ?
                new ObjectParameter("FirstNameEn", firstNameEn) :
                new ObjectParameter("FirstNameEn", typeof(string));
    
            var lastNameEnParameter = lastNameEn != null ?
                new ObjectParameter("LastNameEn", lastNameEn) :
                new ObjectParameter("LastNameEn", typeof(string));
    
            var phoneNo1Parameter = phoneNo1 != null ?
                new ObjectParameter("PhoneNo1", phoneNo1) :
                new ObjectParameter("PhoneNo1", typeof(string));
    
            var phoneNo2Parameter = phoneNo2 != null ?
                new ObjectParameter("PhoneNo2", phoneNo2) :
                new ObjectParameter("PhoneNo2", typeof(string));
    
            var phoneNo3Parameter = phoneNo3 != null ?
                new ObjectParameter("PhoneNo3", phoneNo3) :
                new ObjectParameter("PhoneNo3", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_CONTACT_LIST_Result>("SP_GET_CONTACT_LIST", contactIdParameter, firstNameThParameter, lastNameThParameter, firstNameEnParameter, lastNameEnParameter, phoneNo1Parameter, phoneNo2Parameter, phoneNo3Parameter);
        }
    
        public virtual ObjectResult<SP_GET_SR_ACTIVITY_Result> SP_GET_SR_ACTIVITY(Nullable<int> sR_ACTIVITY_ID)
        {
            var sR_ACTIVITY_IDParameter = sR_ACTIVITY_ID.HasValue ?
                new ObjectParameter("SR_ACTIVITY_ID", sR_ACTIVITY_ID) :
                new ObjectParameter("SR_ACTIVITY_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_SR_ACTIVITY_Result>("SP_GET_SR_ACTIVITY", sR_ACTIVITY_IDParameter);
        }
    
        public virtual ObjectResult<SP_GET_SR_Result> SP_GET_SR(Nullable<int> sR_ID)
        {
            var sR_IDParameter = sR_ID.HasValue ?
                new ObjectParameter("SR_ID", sR_ID) :
                new ObjectParameter("SR_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GET_SR_Result>("SP_GET_SR", sR_IDParameter);
        }
    }
}
