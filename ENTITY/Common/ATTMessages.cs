using System;

namespace Entity.Common
{
    public class ATTMessages
    {
        #region GENERAL
        public const string NO_RECORDS_FOUND = "Sorry, No Records Found .";
        public const string NO_TABLES_FOUND = "Sorry, No Tables Found .";
        public const string DUPLICATE_RECORDS_FOUND = "Duplicate Records Found, Please Re-Check The List .";
        public const string CANNOT_SAVE = "Failed To Save Data .";
        public const string SAVED_SUCCESS = "Data Saved Successfully .";
        public const string POSTING_SUCESS = "Record(s) Posted Sucessfully.";
        public const string POSTING_FAILED = "Failed To Post Record(s).";
        #endregion
        #region ShareHolderMessages
        public class SHARE_HOLDER
        {
            public const string NOT_FOUND = "Sorry, The Holder Doesn't Exist .";
            public const string FOUND = "Holder Found .";
            public const string LOCK = "The Holder is Locked .";
            public const string UNPOSTED = "The Holder Is Not Approved Yet .";
        }

        public class SHARE_HOLDER_RELATIVE
        {
            public const string ADD_SUCCESS = "Relative Holder Added Successfully .";
            public const string UPDATE_SUCCESS = "Relative Holder Updated Successfully .";
            public const string DELETE_SUCCESS = "Relative Holder Deleted Successfully .";
        }
        public class SHARE_HOLDER_LOCK_UNLOCK
        {
            public const string LOCK_FAILED = "Failed To Lock Holder!!";
            public const string DELETE_FAILED = "Failed To Delete For Holder Lock!!";
            public const string UPDATE_FAILED = "Failed To Update For Holder Lock!!";
            public const string UNLOCK_FAILED = "Failed To Unlock Holder!!";
            public const string UNLOCK_DELETE_FAILED = "Failed To Delete For Holder Unlock!!";
            public const string UNLOCK_UPDATE_FAILED = "Failed To Update For Holder Unlock!!";
        }
        #endregion

        #region CertificateMessages
        public class CERTIFICATE
        {
            public string getStatus(string Status)
            {
                string returnStatus = "";
                switch (Int32.Parse(Status))
                {
                    case 1:
                        returnStatus = "Normal";
                        break;
                    case 2:
                        returnStatus = "Normal";
                        break;
                    case 3:
                        returnStatus = "Normal";
                        break;
                    case 4:
                        returnStatus = "Normal";
                        break;
                    case 5:
                        returnStatus = "Normal";
                        break;
                    case 6:
                        returnStatus = "Normal";
                        break;
                    case 7:
                        returnStatus = "Normal";
                        break;
                    case 10:
                        returnStatus = "Normal";
                        break;
                    case 11:
                        returnStatus = "Normal";
                        break;
                    case 12:
                        returnStatus = "Normal";
                        break;
                    case 13:
                        returnStatus = "Normal";
                        break;
                    case 14:
                        returnStatus = "Normal";
                        break;
                    case 20:
                        returnStatus = "Normal";
                        break;
                    case 21:
                        returnStatus = "Normal";
                        break;

                }
                return returnStatus;

            }

            public const string FOUND = "Certificate Found .";
            public const string NOT_FOUND = "Sorry, The Certificate Cannot Be Found .";
            public const string DEMATE_ENTRY = "Selected Certificates Have Been Entried For Dematerialization .";
            public const string DEMATE_POSTING_ACCEPTED = "Selected Certificates Have Been Dematerialized .";
            public const string POSTING_REJECTED = "Selected Certificates Have Been Rejected .";
            public const string DEMATE_ENTRY_FALIED = "Selected Certificates Cannot Be Entried For Dematerialization .";
            public const string DEMATE_POSTING_FALIED = "Selected Certificates Cannot Be Dematerialized .";
            public const string REMATE_ENTRY = "Selected Certificates Have Been Entried For Rematerialization .";
            public const string REMATE_POSTING_ACCEPTED = "Selected Certificates Have Been Rematerialized .";
            public const string REMATE_ENTRY_FALIED = "Selected Certificates Cannot Be Entried For Rematerialization .";
            public const string REMATE_POSTING_FALIED = "Selected Certificates Cannot Be Rematerialized .";
            public const string DELETE_FAILED = "Error Deleteing Certificates .";
            public const string DELETE_SUCCESS = "Selected Certificates Deleted .";
            public const string REVERSE_ENTRY_SUCCESS = "Selected Certificates Have Been Entried For Reversal .";
            public const string REVERSE_FAILED = "Reversal For Selected Certificate Failed .";
            public const string REVERSE_REJECT_POSTING_SUCCESS = "Selected Certificates Have Been Rejected For Reversal .";
            public const string REVERSE_DELETE_POSTING_SUCCESS = "Selected Certificates Have Been Deleted For Reversal .";
            public const string REVERSE_POSTING_SUCCESS = "Selected Certificates Have Been Reversed .";
            public const string NOT_IN_CERT_BONUS_ISSUE = "Certificates Are Not In The Selected Bonus Issue .";

        }
        #endregion

        #region Database
        public class DATABASE
        {
            public const string CONNECTION_SUCCESS = "Connected with the Server Successfully .";
            public const string CONNECTION_FAILURE = "Failed To Connect To The Server .";
            public const string CONNECTION_TIMEOUT = "Connection With The Server Is Taking Time .";
        }
        #endregion
        #region Dividend
        public class DIVIDEND
        {
            public const string NOT_ISSUED = "Dividend Not Issued Yet ";
            public const string ISSUED_BUT_UNPAID = "Dividend Issued And Approved But Unpaid";
            public const string ISSUED_POSTED_APPROVED = "Dividend Already Issued And Approved";
            public const string ISSUED_UNPOSTED_UNAPPROVED = "Dividend Already Issued But Unposted";
            public const string PAID_POSTED_APPROVED = "Dividend Already Paid And Approved";
            public const string PAID_UNPOSTED_UNAPPROVED = "Dividend Already Paid But Unposted";
        }
        #endregion
        #region User
        public class USER
        {
            public const string LOGIN_SUCCESS = "User Logged In Succesfully .";
            public const string LOGIN_FAILURE = "User Logged In Failed .";
            public const string PASSWORD_CHANGE_SUCCESS = "Password Changed Succesfully .";
            public const string PASSWORD_CHANGE_FAILURE = "Failed To Change Password .";
        }
        #endregion
        #region DakhilTransfer
        public class DAKHIL_TRANSFER
        {
            public const string ENTRY_SUCCESS = "Dakhil Transfer Entried SUccessfully .";
            public const string POSTING_SUCCESS = "Dakhil Transferred Successfully .";
            public const string CANCEL_SUCCESS = "Dakhil Cancelled Successfully .";
            public const string FAILURE = "Dakhil Transfer Failed .";
        }
        #endregion
        #region Excel Upload
        public class EXCEL_UPLOAD
        {
            public const string XLSX_NOT_SUPPORTED = "Format not Supported!! <br/> Only .xls Supported!!";
        }
        #endregion
    }

}
