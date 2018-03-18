//------------------------------------------------------------------------------
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
    
    public partial class SP_GET_SR_Result
    {
        public int SR_ID { get; set; }
        public string SR_NO { get; set; }
        public string SR_CALL_ID { get; set; }
        public string SR_ANO { get; set; }
        public Nullable<int> CUSTOMER_ID { get; set; }
        public Nullable<int> CUSTOMER_SUBSCRIPT_TYPE_ID { get; set; }
        public string CUSTOMER_SUBSCRIPT_TYPE_CODE { get; set; }
        public string CUSTOMER_SUBSCRIPT_TYPE_NAME { get; set; }
        public Nullable<int> CUSTOMER_TITLE_ID_TH { get; set; }
        public string CUSTOMER_TITLE_NAME_TH { get; set; }
        public string CUSTOMER_FIRST_NAME_TH { get; set; }
        public string CUSTOMER_LAST_NAME_TH { get; set; }
        public Nullable<int> CUSTOMER_TITLE_ID_EN { get; set; }
        public string CUSTOMER_TITLE_NAME_EN { get; set; }
        public string CUSTOMER_FIRST_NAME_EN { get; set; }
        public string CUSTOMER_LAST_NAME_EN { get; set; }
        public string CUSTOMER_CARD_NO { get; set; }
        public Nullable<System.DateTime> CUSTOMER_BIRTH_DATE { get; set; }
        public string CUSTOMER_EMAIL { get; set; }
        public Nullable<short> CUSTOMER_TYPE { get; set; }
        public Nullable<int> ACCOUNT_ID { get; set; }
        public string ACCOUNT_NO { get; set; }
        public string ACCOUNT_CAR_NO { get; set; }
        public string ACCOUNT_PRODUCT_GROUP { get; set; }
        public string ACCOUNT_PRODUCT { get; set; }
        public string ACCOUNT_BRANCH_CODE { get; set; }
        public string ACCOUNT_BRANCH_NAME { get; set; }
        public string ACCOUNT_STATUS { get; set; }
        public Nullable<int> CONTACT_ID { get; set; }
        public Nullable<int> CONTACT_SUBSCRIPT_TYPE_ID { get; set; }
        public string CONTACT_SUBSCRIPT_TYPE_CODE { get; set; }
        public string CONTACT_SUBSCRIPT_TYPE_NAME { get; set; }
        public Nullable<int> CONTACT_TITLE_ID_TH { get; set; }
        public string CONTACT_TITLE_NAME_TH { get; set; }
        public string CONTACT_FIRST_NAME_TH { get; set; }
        public string CONTACT_LAST_NAME_TH { get; set; }
        public Nullable<int> CONTACT_TITLE_ID_EN { get; set; }
        public string CONTACT_TITLE_NAME_EN { get; set; }
        public string CONTACT_FIRST_NAME_EN { get; set; }
        public string CONTACT_LAST_NAME_EN { get; set; }
        public string CONTACT_CARD_NO { get; set; }
        public Nullable<System.DateTime> CONTACT_BIRTH_DATE { get; set; }
        public string CONTACT_EMAIL { get; set; }
        public Nullable<int> CONTACT_RELATIONSHIP_ID { get; set; }
        public string CONTACT_RELATIONSHIP_NAME { get; set; }
        public string CONTACT_RELATIONSHIP_DESC { get; set; }
        public string CONTACT_ACCOUNT_NO { get; set; }
        public Nullable<int> MAP_PRODUCT_ID { get; set; }
        public Nullable<int> PRODUCTGROUP_ID { get; set; }
        public string PRODUCTGROUP_NAME { get; set; }
        public Nullable<int> PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public Nullable<int> CAMPAIGNSERVICE_ID { get; set; }
        public string CAMPAIGNSERVICE_NAME { get; set; }
        public Nullable<int> AREA_ID { get; set; }
        public string AREA_NAME { get; set; }
        public Nullable<int> SUBAREA_ID { get; set; }
        public string SUBAREA_NAME { get; set; }
        public Nullable<int> TYPE_ID { get; set; }
        public string TYPE_NAME { get; set; }
        public string SR_SUBJECT { get; set; }
        public string SR_REMARK { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> CLOSE_DATE { get; set; }
        public Nullable<int> SR_PAGE_ID { get; set; }
        public Nullable<bool> SR_IS_VERIFY { get; set; }
        public string SR_IS_VERIFY_PASS { get; set; }
        public Nullable<int> SR_PAGE_ID1 { get; set; }
        public string SR_PAGE_CODE { get; set; }
        public string SR_PAGE_NAME { get; set; }
        public Nullable<int> SR_STATUS_ID { get; set; }
        public string SR_STATUS_CODE { get; set; }
        public string SR_STATUS_NAME { get; set; }
        public Nullable<int> SR_DEF_ACCOUNT_ADDRESS_ID { get; set; }
        public string SR_DEF_ADDRESS_HOUSE_NO { get; set; }
        public string SR_DEF_ADDRESS_VILLAGE { get; set; }
        public string SR_DEF_ADDRESS_BUILDING { get; set; }
        public string SR_DEF_ADDRESS_FLOOR_NO { get; set; }
        public string SR_DEF_ADDRESS_ROOM_NO { get; set; }
        public string SR_DEF_ADDRESS_MOO { get; set; }
        public string SR_DEF_ADDRESS_SOI { get; set; }
        public string SR_DEF_ADDRESS_STREET { get; set; }
        public string SR_DEF_ADDRESS_TAMBOL { get; set; }
        public string SR_DEF_ADDRESS_AMPHUR { get; set; }
        public string SR_DEF_ADDRESS_PROVINCE { get; set; }
        public string SR_DEF_ADDRESS_ZIPCODE { get; set; }
        public Nullable<int> SR_AFS_ASSET_ID { get; set; }
        public string SR_AFS_ASSET_NO { get; set; }
        public string SR_AFS_ASSET_DESC { get; set; }
        public Nullable<System.DateTime> SR_NCB_CUSTOMER_BIRTHDATE { get; set; }
        public string SR_NCB_CHECK_STATUS { get; set; }
        public Nullable<int> SR_NCB_MARKETING_USER_ID { get; set; }
        public string SR_NCB_MARKETING_FULL_NAME { get; set; }
        public Nullable<int> SR_NCB_MARKETING_BRANCH_ID { get; set; }
        public string SR_NCB_MARKETING_BRANCH_NAME { get; set; }
        public Nullable<int> SR_NCB_MARKETING_BRANCH_UPPER_1_ID { get; set; }
        public string SR_NCB_MARKETING_BRANCH_UPPER_1_NAME { get; set; }
        public Nullable<int> SR_NCB_MARKETING_BRANCH_UPPER_2_ID { get; set; }
        public string SR_NCB_MARKETING_BRANCH_UPPER_2_NAME { get; set; }
        public string RULE_ASSIGN_FLAG { get; set; }
        public Nullable<int> DRAFT_SR_EMAIL_TEMPLATE_ID { get; set; }
        public string DRAFT_MAIL_SENDER { get; set; }
        public string DRAFT_MAIL_TO { get; set; }
        public string DRAFT_MAIL_CC { get; set; }
        public string DRAFT_MAIL_SUBJECT { get; set; }
        public string DRAFT_MAIL_BODY { get; set; }
        public Nullable<int> DRAFT_ACTIVITY_TYPE_ID { get; set; }
        public string DRAFT_ACTIVITY_DESC { get; set; }
        public string DRAFT_ACCOUNT_ADDRESS_TEXT { get; set; }
        public Nullable<bool> DRAFT_IS_SEND_EMAIL_FOR_DELEGATE { get; set; }
        public Nullable<bool> DRAFT_IS_CLOSE { get; set; }
        public string DRAFT_ATTACHMENT_JSON { get; set; }
        public string DRAFT_VERIFY_ANSWER_JSON { get; set; }
        public Nullable<int> CREATE_USER_ID { get; set; }
        public string CREATE_USER_FIRST_NAME { get; set; }
        public string CREATE_USER_LAST_NAME { get; set; }
        public string CREATE_USER_POSITION_CODE { get; set; }
        public Nullable<int> CREATE_BRANCH_ID { get; set; }
        public string CREATE_BRANCH_CODE { get; set; }
        public string CREATE_BRANCH_NAME { get; set; }
        public Nullable<int> UPDATE_USER_ID { get; set; }
        public string UPDATE_USER_FIRST_NAME { get; set; }
        public string UPDATE_USER_LAST_NAME { get; set; }
        public string UPDATE_USER_POSITION_CODE { get; set; }
        public Nullable<int> OWNER_USER_ID { get; set; }
        public string OWNER_USER_FIRST_NAME { get; set; }
        public string OWNER_USER_LAST_NAME { get; set; }
        public string OWNER_USER_POSITION_CODE { get; set; }
        public Nullable<int> OWNER_BRANCH_ID { get; set; }
        public string OWNER_BRANCH_CODE { get; set; }
        public string OWNER_BRANCH_NAME { get; set; }
        public Nullable<int> DELEGATE_USER_ID { get; set; }
        public string DELEGATE_USER_FIRST_NAME { get; set; }
        public string DELEGATE_USER_LAST_NAME { get; set; }
        public string DELEGATE_USER_POSITION_CODE { get; set; }
        public Nullable<int> DELEGATE_BRANCH_ID { get; set; }
        public string DELEGATE_BRANCH_CODE { get; set; }
        public string DELEGATE_BRANCH_NAME { get; set; }
        public Nullable<int> CHANNEL_ID { get; set; }
        public string CHANNEL_NAME { get; set; }
        public string CHANNEL_CODE { get; set; }
        public Nullable<int> MEDIA_SOURCE_ID { get; set; }
        public string MEDIA_SOURCE_NAME { get; set; }
        public string CUSTOMER_EMPLOYEE_CODE { get; set; }
    }
}
