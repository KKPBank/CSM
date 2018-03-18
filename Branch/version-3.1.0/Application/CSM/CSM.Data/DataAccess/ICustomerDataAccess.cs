﻿using System.Collections.Generic;
using CSM.Entity;

namespace CSM.Data.DataAccess
{
    public interface ICustomerDataAccess
    {
        IEnumerable<RelationshipEntity> GetAllRelationships(RelationshipSearchFilter searchFilter);
        RelationshipEntity GetRelationshipByID(int RelationshipId);
        bool IsDuplicateRelationship(RelationshipEntity relationEntity);
        bool SaveContactRelation(RelationshipEntity relationEntity);
        IEnumerable<CustomerEntity> SearchCustomer(CustomerSearchFilter searchFilter);
        //IEnumerable<CustomerEntity> GetCustomerList(CustomerSearchFilter searchFilter);
        CustomerEntity GetCustomerByID(int customerId);
        //List<CustomerEntity> GetCustomerByPhoneNo(int? customerId, List<string> lstPhoneNo);
        List<CustomerEntity> GetCustomerByName(string customerTHName);
        bool SaveCustomer(CustomerEntity customerEntity);
        bool IsDuplicateCardNo(int? customerId, int cbsCardTypeId, string cardNo);
        IEnumerable<NoteEntity> GetNoteList(NoteSearchFilter searchFilter);
        NoteEntity GetNoteByID(int noteId);
        bool SaveNote(NoteEntity noteEntity);
        IEnumerable<AttachmentEntity> GetAttachmentList(AttachmentSearchFilter searchFilter);
        AttachmentEntity GetAttachmentByID(int attachmentId, string documentLevel);
        void SaveCustomerAttachment(AttachmentEntity attachment);
        void DeleteCustomerAttachment(int attachmentId, int updateBy);
        IEnumerable<AccountEntity> GetAccountList(AccountSearchFilter searchFilter);
        IEnumerable<ContactEntity> GetContactList(ContactSearchFilter searchFilter);
        bool DeleteCustomerContact(int customerContactId, int updateBy);
        ContactEntity GetContactByID(int contactId);
        bool SaveContact(ContactEntity contactEntity, List<CustomerContactEntity> lstCustomerContact);
        List<CustomerContactEntity> GetContactRelationshipList(int contactId, int customerId);
        List<AccountEntity> GetAccountByCustomerId(int customerId);
        List<ContactEntity> GetContactByPhoneNo(int? contactId, string firstNameTh, string lastNameTh, string firstNameEn, string lastNameEn, List<string> lstPhoneNo);
        IEnumerable<SrEntity> GetSrList(SrSearchFilter searchFilter);
        IEnumerable<SrEntity> GetCustomerSrList(SrSearchFilter searchFilter);
        IEnumerable<CustomerLogEntity> GetCustomerLogList(CustomerLogSearchFilter searchFilter);
        bool SaveCallId(string callId, string phoneNo, string cardNo, string callType, int userId, string iVRLang);
        CallInfoEntity GetCallInfoByCallId(string callId);
        List<AccountEntity> GetAccountBranchByName(string searchTerm, int pageSize, int pageNum);
        int GetAccountBranchCountByName(string searchTerm);
        List<AccountEntity> GetAccountProductByName(string searchTerm, int pageSize, int pageNum);
        int GetAccountProductCountByName(string searchTerm);
        List<AccountEntity> GetAccountGradeByName(string searchTerm, string product, int pageSize, int pageNum);
        int GetAccountGradeCountByName(string searchTerm, string product);
        ExistingProductEntity GetExistingProductDetail(ExistingProductSearchFilter searchFilter);
        bool SaveContactSr(ContactEntity contactEntity);
        List<int?> GetCustomerIdWithCallId(string phoneNo);
    }
}