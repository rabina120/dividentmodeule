
using Entity.Common;
using Entity.Dividend;
using Entity.Reports;
using Interface.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Entity.Reports.ATTGenericReport;

namespace Repository.Reports
{
    public class GenericReportRepo : IGenericReport
    {
        IOptions<ReadConfig> _connectionString;
        private readonly IHostingEnvironment _webHostEnvironment;


        public GenericReportRepo(IOptions<ReadConfig> connectionString, IHostingEnvironment webHostEnvironment)
        {
            _connectionString = connectionString;
            _webHostEnvironment = webHostEnvironment;
        }


        //this is for generating new table header list
        public List<ATTGenericReport> GenTableHeader(ReportName name, bool withBankDetails = false)
        {

            List<ATTGenericReport> aTTTableHeaders = new List<ATTGenericReport>();
            aTTTableHeaders.Add(new ATTGenericReport(0.6f, "SN", "sn"));
            if (name == ReportName.UserAuditReport || name == ReportName.DailyReport)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1f, "USER Name", "USERNAME"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Form Name", "REFFILE"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Entry Date", "ACTIONDATE"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Entry Time", "ENTRYTIME"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "IPAddress", "ipaddress"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Remarks", "REMARKS"));
            }
            if (name == ReportName.ListUnPaidDividendWarrantPhysicalPosted
                || name == ReportName.ListUnPaidDividendWarrantPhysicalUnposted
                || name == ReportName.ListPaidDividendWarrantPhysicalPosted
                || name == ReportName.ListPaidDividendWarrantPhysicalUnposted)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1f, "H.Num", "shholderno"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Name", "shholdername"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Kitta", "totshkitta"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Warr.Amt", "warrantamt"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Div.Tax", "taxdamt"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Bon.Tax", "bonustax"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "NetPay", "netamt"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Warr.Num", "warrantno"));
                if (name == ReportName.ListUnPaidDividendWarrantPhysicalPosted
                || name == ReportName.ListUnPaidDividendWarrantPhysicalUnposted)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.Date", "IssuedDate"));
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.By", "IssuedBy"));
                }
                else
                {
                    if (name == ReportName.ListPaidDividendWarrantPhysicalPosted)
                    {
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.Date", "IssueDate"));
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "App.Date", "PaiApproveddDate"));
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "App.By", "PaidApprovedBy"));
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "App.Remarks", "PaidApprovedRemarks"));
                    }
                    else
                    {
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.Date", "IssueDate"));
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Piad.Date", "PaidDate"));
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Paid.By", "PaidBy"));
                    }
                }

                if (withBankDetails)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "Acc.Num", "bankaccno"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "BankName", "bankname"));//
                    aTTTableHeaders.Add(new ATTGenericReport(0.8f, "CreditedDate", "crediteddt"));//
                }

            }
            if (name == ReportName.ListIssuedDividendWarrantPhysicalPosted
                || name == ReportName.ListIssuedDividendWarrantPhysicalUnposted
                || name == ReportName.ListUnIssuedDividendWarrantPhysicalPosted
                || name == ReportName.ListUnIssuedDividendWarrantPhysicalUnposted
                || name == ReportName.listOfIssuedAndUnPaidDividendWarrantPhysicalPosted
                || name == ReportName.listOfIssuedAndUnPaidDividendWarrantPhysicalUnposted
                )
            {
                aTTTableHeaders.Add(new ATTGenericReport(1f, "H.Num", "shholderno"));//
                aTTTableHeaders.Add(new ATTGenericReport(2f, "Name", "shholdername"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Warr.Num", "warrantno"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Kitta", "totshkitta"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Div.Amt", "warrantamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Div.Tax", "taxdamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Bon.Tax", "bonustax"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Bon.Adj.", "bonusadj"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "NetPay", "netamt"));//
                if (withBankDetails)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "Acc.Num", "bankaccno"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "BankName", "bankname"));//
                    aTTTableHeaders.Add(new ATTGenericReport(0.8f, "CreditedDate", "crediteddt"));//
                }
                if (name == ReportName.ListIssuedDividendWarrantPhysicalPosted
                    || name == ReportName.ListIssuedDividendWarrantPhysicalUnposted
                    || name == ReportName.listOfIssuedAndUnPaidDividendWarrantPhysicalPosted
                    || name == ReportName.listOfIssuedAndUnPaidDividendWarrantPhysicalUnposted)
                {
                    if (name == ReportName.ListIssuedDividendWarrantPhysicalPosted
                        || name == ReportName.listOfIssuedAndUnPaidDividendWarrantPhysicalPosted)
                    {
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.Date", "IssueDate"));
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "App.Date", "ApprovedDate"));//
                        aTTTableHeaders.Add(new ATTGenericReport(0.8f, "App.By", "ApprovedBy"));//
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "App.Remarrks", "ApprovedRemarks"));//
                    }
                    else
                    {
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.Date", "IssuedDate"));//
                        aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Iss.By", "IssuedBy"));//
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.Remarks", "IssueRemarks"));//
                    }
                }

            }
            if (name == ReportName.ListIssuedDividendWarrantDematePosted
                || name == ReportName.ListIssuedDividendWarrantDemateUnposted
                || name == ReportName.ListUnIssuedDividendWarrantDematePosted
                || name == ReportName.ListUnIssuedDividendWarrantDemateUnposted
                || name == ReportName.listOfIssuedAndUnPaidDividendWarrantDematePosted
                || name == ReportName.listOfIssuedAndUnPaidDividendWarrantDemateUnposted)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "BOID", "bo_idno"));//
                aTTTableHeaders.Add(new ATTGenericReport(3f, "Name", "fullname"));
                aTTTableHeaders.Add(new ATTGenericReport(0.9f, "Warr.Num", "warrantno"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Kitta", "totshkitta"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.9f, "Div.Amt", "warrantamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Div.Tax", "taxdamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Bon.Tax", "bonustax"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Bon.Adj.", "bonusadj"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "NetPay", "netamt"));//
                if (withBankDetails)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "Acc.Num", "BANKACCNO"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "BankName", "bankname"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "CreditedDate", "crediteddt"));//
                }
                if (name == ReportName.ListIssuedDividendWarrantDematePosted
                    || name == ReportName.ListIssuedDividendWarrantDemateUnposted
                    || name == ReportName.listOfIssuedAndUnPaidDividendWarrantDematePosted
                    || name == ReportName.listOfIssuedAndUnPaidDividendWarrantDemateUnposted)
                {
                    if (name == ReportName.ListIssuedDividendWarrantDematePosted
                         || name == ReportName.listOfIssuedAndUnPaidDividendWarrantDematePosted)
                    {
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.Date", "IssueDate"));
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "App.Date", "ApprovedDate"));//
                        aTTTableHeaders.Add(new ATTGenericReport(0.8f, "App.By", "ApprovedBy"));//
                        aTTTableHeaders.Add(new ATTGenericReport(0.8f, "App.By", "ApprovedRemarks"));//

                    }
                    else
                    {
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.Date", "IssuedDate"));//
                        aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Iss.By", "IssuedBy"));//
                        aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Iss.Remark", "IssueRemarks"));//
                    }

                }

            }
            if (name == ReportName.ListPaidDividendWarrantDematePosted
                || name == ReportName.ListPaidDividendWarrantDemateUnposted
                || name == ReportName.ListUnPaidDividendWarrantDematePosted
                || name == ReportName.ListUnPaidDividendWarrantDemateUnposted)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "BOID", "bo_idno"));//
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Name", "fullname"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Warr.Num", "warrantno"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Kitta", "totshkitta"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Div.Tax", "warrantamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Div.Tax", "taxdamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Bon.Tax", "bonustax"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Bon.Adj.", "bonusadj"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "NetPay", "netamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Iss.Date", "IssueDate"));//
                if (name == ReportName.ListUnPaidDividendWarrantDematePosted
                || name == ReportName.ListUnPaidDividendWarrantDemateUnposted)
                {
                    
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.By", "IssuedBy"));//
                }
                else
                {
                    if (name == ReportName.ListPaidDividendWarrantDematePosted)
                    {
                        aTTTableHeaders.Add(new ATTGenericReport(0.7f, "App.Date", "PaiApproveddDate"));//
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "App.By", "PaidApprovedBy"));//
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "App.Remarks", "PaidApprovedRemarks"));//
                    }
                    else
                    {
                        aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Paid.Date", "PaidDate"));//
                        aTTTableHeaders.Add(new ATTGenericReport(1f, "Paid.By", "PaidBy"));//
                    }
                }

                if (withBankDetails)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "Acc.Num", "bankaccno"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "BankName", "bankname"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "CreditedDate", "crediteddt"));//
                }

            }
            if(name == ReportName.UndoReportDemate || name == ReportName.UndoReportPhysical)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Name", "shholdername"));//
                if (name == ReportName.UndoReportPhysical)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "H.No", "shholderno"));//
                }
                if(name == ReportName.UndoReportDemate)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "BOID", "bo_idno"));//
                }
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Kitta", "totshkitta"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Warr.Num", "warrantno"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Warr.Amt", "warrantamt"));//                             
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Div.Tax", "taxdamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Bon.Tax", "bonustax"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Bon.Adj.", "bonusadj"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "NetPay", "netamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Iss.Date", "IssueDate"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Issue.By", "IssueBy"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Paid.Date", "PaidDate"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Paid.By", "PaidBy"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Undo.Date", "UndoDate"));//
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Undo.By", "UndoBy"));//

                if (withBankDetails)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "Acc.Num", "bankaccno"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "BankName", "bankname"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1f, "CreditedDate", "crediteddt"));//
                }


            }

            if (name == ReportName.DemateRemateReport)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Dakhil No", "iono"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "SellerId", "SHolderNo"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Seller", "sellername"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "BuyerId", "BHolderNo"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Buyer", "buyername"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Cert No", "certno"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "SrNoFrom", "srnofrom"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "SrNoTo", "srnoto"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Kitta", "TranKitta"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "TransDate", "TransferDt"));
            }
            if (name == ReportName.ShareHolderShareHolderListInEnglish
                || name == ReportName.ShareHolderShareHolderslistInEnglishZeroKitta)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1f, "H.Num", "holderno"));
                aTTTableHeaders.Add(new ATTGenericReport(2f, "Full Name", "name"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Address", "address"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Father's Name", "fathername"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Grand Father's Name", "grandfathername"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Telephone No", "telno"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "No of Shares", "totalkitta"));
            }
            if (name == ReportName.ShareHolderShareHolderListInNepali)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1f, "z]o/ wlg g+", "holderno"));
                aTTTableHeaders.Add(new ATTGenericReport(2f, "z]o/ wlgsf] gfd", "name"));
                aTTTableHeaders.Add(new ATTGenericReport(2f, "&]ufgf", "address"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "lsQf", "totalkitta"));
            }
            if (name == ReportName.ShareHolderFractionList)
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "H.Num", "holderno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Full Name", "name"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Address", "address"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Father's Name", "fathername"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Grand Father's Name", "grandfathername"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Telephone No", "telno"));
                aTTTableHeaders.Add(new ATTGenericReport(0.75f, "No of Shares", "totalkitta"));
                aTTTableHeaders.Add(new ATTGenericReport(0.5f, "Fraction", "fraction"));
            }
            if (name == ReportName.AllShareHolderFractionList)
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "H.Num", "holderno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Full Name", "name"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Address", "address"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Father's Name", "fathername"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Grand Father's Name", "grandfathername"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Telephone No", "telno"));
                aTTTableHeaders.Add(new ATTGenericReport(0.75f, "No of Shares", "totalkitta"));
                aTTTableHeaders.Add(new ATTGenericReport(0.5f, "Fraction", "fraction"));
            }
            if (name == ReportName.ShareHolderShareHoldersDetailsList)
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "H.Num", "holderno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Full Name", "name"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Address", "address"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Father's Name", "fathername"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Grand Father's Name", "grandfathername"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Telephone No", "telno"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "No of Shares", "totalkitta"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Signature", "signature1"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Signature", "signature2"));

            }
            if (name == ReportName.ShareHolderShareHolderAttendanceList)
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "H.Num", "holderno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Full Name", "name"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Address", "address"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Father's Name", "fathername"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Telephone No", "telno"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "No of Shares", "totalkitta"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Signature", "signature1"));

            }
            if (name == ReportName.AllCertificateList)
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "H.Num", "SHHOLDERNO"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Name", "NAME"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "CertNo", "CERTNO"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Srno From", "SRNOFROM"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "SrNo To", "SRNOTO"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Kitta", "SHKITTA"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Issue Date", "Issuedate"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Share Type", "Description"));

            }
            if (name == ReportName.DistributedUndistributedCertificateList)
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "CertNo", "CERTNO"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "H.Num", "SHHOLDERNO"));

                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Name", "NAME"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Srno From", "SRNOFROM"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "SrNo To", "SRNOTO"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Kitta", "SHKITTA"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "Dist.Date", "CERTDISTDT"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Description", "DESCRIPTION"));
            }
            if (name == ReportName.HolderBOIDHistory)
            {

            }
            if (name == ReportName.SecurityMatrixForSCB)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Role/Profile Name", "ROLENAME"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Role Description", "ROLEDESCRIPTION"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Entitlements/ Permissions", "PERMISSIONS"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Index sheet", "INDEXSHEET"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "HP Profile Declaration with BO approval attached", "HPPROFILEDECLARATION"));

            }
            if (name == ReportName.UserRoleUpdateReport)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "IAM Admin ID", "ENTRYUSER"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Business User Bank ID/Login ID", "USERNAME"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Request Type", "REQUESTTYPE"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Old value", "OLDVALUE"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "New value", "NEWVALUE"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Actioned Date", "ENTRYDATE"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Actioned Time", "ENTRYTIME"));

            }

            if (name == ReportName.DakhilList)//groupby and sum kitta
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Tran No", "Tran No"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Reg No", "Reg No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.2f, "Dakhil Date", "Dakhil Date"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Letter No", "Letter No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.2f, "Seller", "Seller"));
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Seller No", "Seller No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.2f, "Buyer", "Buyer"));
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Buyer No", "Buyer No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Trade Type", "Trade Type"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Cert No", "Cert No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Srn From", "Srn From"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Srn To", "Srn To"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Trn Kitta", "Trn Kitta"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Broker", "Broker"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Entry User", "Entry User"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Remark", "Remarks"));

            }
            if (name == ReportName.TransferList)//sum kitta
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.4f, "Sn", "Sn"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Tran No", "Tran No"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Mut No", "Mut No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.2f, "Dakhil Date", "Tran Date"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Letter No", "Letter No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.2f, "Seller", "Seller"));
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Seller No", "Seller No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.2f, "Buyer", "Buyer"));
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Buyer No", "Buyer No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Trade Type", "Trade Type"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Cert No", "Cert No"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Srn From", "Srn From"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Srn To", "Srn To"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Trn Kitta", "Trn Kitta"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Broker", "Broker"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Remark", "Remarks"));

            }
            if (name == ReportName.DakhilKittaReport)//sum kitta
            {
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "H.Num", "bholderno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.25f, "Name", "name"));
                aTTTableHeaders.Add(new ATTGenericReport(2.6f, "Address", "address"));
                aTTTableHeaders.Add(new ATTGenericReport(1.2f, "ExistingKitta", "existingkitta"));
                aTTTableHeaders.Add(new ATTGenericReport(1.2f, "PurchasedKitta", "purchasedkitta"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "TotalKitta", "totalkitta"));


            }
            if (name == ReportName.DakhilKharejBook)//sum kitta
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Muta.Num.", "regno"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "LetterNum.", "letterno"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Muta.Date", "dakhildt"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Seller", "sname"));
                aTTTableHeaders.Add(new ATTGenericReport(3.0f, "SellerAdd.", "sadd"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Cert.Num.", "certno"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Folio.Num.", "foliono"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Broker", "brokercode"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "SN.FROM", "srnofrom"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "SN.TO", "srnoto"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Kitta", "trankitta"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Buyer", "bname"));
                aTTTableHeaders.Add(new ATTGenericReport(3.0f, "BuyerAdd.", "badd"));


            }
            if (name == ReportName.DakhilBuyerList)//sum kitta
            {

                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "SellerNum.", "bholderno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Name", "bname"));
                aTTTableHeaders.Add(new ATTGenericReport(3.0f, "Address", "address"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Contact", "telno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Father's Name", "faname"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "G.Father's Name", "grfaname"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Kitta", "trankitta"));


            }
            if (name == ReportName.DakhilSellerList)//sum kitta
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "SellerNum.", "sholderno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Name", "sname"));
                aTTTableHeaders.Add(new ATTGenericReport(3.0f, "Address", "address"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Contact", "telno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Father's Name", "faname"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "G.Father's Name", "grfaname"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Kitta", "trankitta"));


            }

            if (name == ReportName.BulkCA)//sum kitta
            {
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Cert No", "certNo"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Holder No.", "shHolderNo"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Holder Name", "holderName"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "SR No from", "srNoFrom"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "SR No To", "srNoTo"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Kitta", "kitta"));
                aTTTableHeaders.Add(new ATTGenericReport(3.0f, "BOID", "boid"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "DPID", "dpid"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "ISIN No.", "isiN_NO"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Description", "description"));
            }


            if (name == ReportName.SharePurchaseSalesReport)
            {
                aTTTableHeaders = new List<ATTGenericReport>();
                aTTTableHeaders.Add(new ATTGenericReport(3f, "Buyer Name", "BuyerName"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "BuyerNo", "BuyerNo"));
                aTTTableHeaders.Add(new ATTGenericReport(3f, "SellerName", "SellerName"));
                aTTTableHeaders.Add(new ATTGenericReport(1f, "SellerNo", "SellernNo"));
                aTTTableHeaders.Add(new ATTGenericReport(3f, "Transfer Date", "TransferDt"));
                aTTTableHeaders.Add(new ATTGenericReport(2f, "Certificate No", "CertNo"));
                aTTTableHeaders.Add(new ATTGenericReport(3f, "Remarks", "Remarks"));
            }
            if (name == ReportName.ValidAccount
                || name == ReportName.InValidAccount
                || name == ReportName.TranasctionProcessed
                || name == ReportName.TransactionFailedToProcess
                || name == ReportName.TransactionStatusFailed
                || name == ReportName.TransactionStatusProcessing
                || name == ReportName.TransactionStatusSuccess)
            {
                if (withBankDetails)
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Holder No.", "shholderno"));
                else
                    aTTTableHeaders.Add(new ATTGenericReport(1.6f, "BOID.", "boidno"));

                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Holder Name", "fullname"));
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Warr.Num", "warrantno"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Div.Tax", "warrantamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Div.Tax", "taxdamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Bon.Tax", "bonustax"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.7f, "Bon.Adj.", "bonusadj"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "NetPay", "totalamt"));//
                aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Bank", "Bankname"));//
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "AccNo.", "bankno"));//
                if (name == ReportName.InValidAccount
                   || name == ReportName.ValidAccount)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(0.8f, "Valid%.", "validpercentage"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Remarks.", "validatemessage"));//

                }
                else if (name == ReportName.TranasctionProcessed
                    || name == ReportName.TransactionFailedToProcess)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "TransactionMessage.", "transactionmessage"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "TransactionDetail.", "transactiondetail"));//

                }
                else if (name == ReportName.TransactionStatusFailed
                    || name == ReportName.TransactionStatusProcessing
                    || name == ReportName.TransactionStatusSuccess)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "TransactionMessage.", "transactionmessage"));//
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "TransactionDetail.", "transactiondetail"));//
                    aTTTableHeaders.Add(new ATTGenericReport(0.8f, "LastUpdatedOn.", "transactionupdateddate"));//
                    aTTTableHeaders.Add(new ATTGenericReport(0.8f, "LastUpdatedBy.", "transactionupdatedby"));//


                }
            }



            if (name == ReportName.PSLReportUnclearPosted
                || name == ReportName.PSLReportUnclearUnposted
                || name == ReportName.PSLReportUnclearRejected)
            {
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Sholder No", "SHHOLDERNO"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "PSL No.", "PSLNO"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "First Name", "FNAME"));
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "Last Name", "LNAME"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Cert No", "certno"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "SRN From", "SRNOFROM"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "SRN To", "SRNOTO"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "PSL Kitta", "PLEDGEKITTA"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "PSL Status.", "PSL Status"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "PSL At", "pledge_at"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "PSL Date", "PSL_Date"));



                if (name == ReportName.PSLReportUnclearUnposted)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Entry User.", "ENTRYUSER"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Remarks", "Remark"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Entry Date", "EntryDate"));
                }
                if (name == ReportName.PSLReportUnclearPosted
                || name == ReportName.PSLReportUnclearRejected)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "PSL Approved By", "ENTRYUSER"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "App Remarks", "Remark"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "App Date", "EntryDate"));
                }
            }
            if (name == ReportName.PSLReportClearPosted
                || name == ReportName.PSLReportClearUnposted
                || name == ReportName.PSLReportClearRejected)
            {
                aTTTableHeaders.Add(new ATTGenericReport(1.5f, "First Name", "FNAME"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Last Name", "LNAME"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "PSL Clear No", "PSL_CLEAR_NO"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Sholder No", "SHHOLDERNO"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Cert No", "CERTNO"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "SRN From", "SRNOFROM"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "SRN To.", "SRNOTO"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "PSL Status", "PSL Status"));
                aTTTableHeaders.Add(new ATTGenericReport(1.0f, "PSL Kitta", "PLEDGEKITTA"));
                if (name == ReportName.PSLReportClearUnposted)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "App Status", "appstatus_clear"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Clear Date", "clearDt"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Clear Entry User", "ClearentryUser"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Clear Remarks", "Remark"));
                }
                if (name == ReportName.PSLReportClearPosted
                || name == ReportName.PSLReportClearRejected)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "App Status", "appstatus_clear"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "App Date", "clearDt"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "Approved By", "ClearentryUser"));
                    aTTTableHeaders.Add(new ATTGenericReport(1.0f, "App Remarks", "Remark"));
                }

            }


            if (name == ReportName.ShareholderLockReportPosted
                || name == ReportName.ShareholderLockReportUnposted)
            {
                //           ( Lock
                //shholderno  FName LName    shownertype HolderLock lock_id lock_dt lock_remarks    approved_by approved_date   approved_remarks
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Shareholder No", "shholderno"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "First Name", "FName"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Last Name", "LName"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Sharewoner Type", "shownertype"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder Lock", "HolderLock"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Lock Id", "lock_id"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Lock Date", "lock_dt"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Lock Remarks", "lock_remarks"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Approved By", "approved_by"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Approved Date", "approved_date"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Approved Remarks", "approved_remarks"));
            }
            if (name == ReportName.ShareholderUnlockReportPosted
                || name == ReportName.ShareholderUnlockReportUnposted)
            {

                //                (Unlock
                //shholderno FName   LName   shownertype HolderLock  lock_id unlock_dt   unlock_remarks unlock_by   approved_by_unlock approved_unlock_dt  unlock_approved_remarks)

                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Shareholder No", "shholderno"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "First Name", "FName"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Last Name", "LName"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Sharewoner Type", "shownertype"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder Lock", "HolderLock"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Lock Id", "lock_id"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Unlock Date", "unlock_dt"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Unlock Remarks", "unlock_remarks"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Unlock By", "unlock_by"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Unlock Approved By", "approved_by_unlock"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Unlock Approved Date", "approved_unlock_dt"));
                aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Unlock Approved Remarks", "unlock_approved_remarks"));
            }
            
            if (name == ReportName.DematDetailReport 
                || name == ReportName.DematSummaryReport
                || name == ReportName.DematDrnReport
                || name == ReportName.RematDetailReport
                || name == ReportName.RematSummaryReport
                || name == ReportName.RematDrnReport)
            {
                if(name == ReportName.DematDetailReport
                    || name == ReportName.DematDrnReport)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(0.4f, "Reg No", "regno"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder No", "holderNO"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder Name", "name"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Cert No", "certno"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Srn From", "srnofrom"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Srn To", "srnoto"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Kitta", "kitta"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "DRN", "drn"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Entry By", "entryBy"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Entry Date", "entryDate"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Request Date", "tranDate"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "BOID", "boid"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "ISIN", "isin"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "DP", "dp"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Share Type", "shareType"));
                }
                if(name == ReportName.RematDetailReport
                    || name == ReportName.RematDrnReport)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(0.4f, "Rev No", "reversalNo"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.4f, "Seqno", "seqno"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.4f, "Reg No", "regno"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder No", "holderNO"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder Name", "name"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Cert No", "certno"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Srn From", "srnofrom"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Srn To", "srnoto"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Kitta", "kitta"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Demat Holder", "dematHolderNo"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "DRN", "drn"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Rev By", "revBy"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Rev Date", "revDate"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "BOID", "boid"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "ISIN", "isin"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "DP", "dp"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Share Type", "shareType"));
                }
                if(name == ReportName.DematSummaryReport)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(0.4f, "Reg No", "regno"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder No", "holderNO"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder Name", "name"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Kitta", "kitta"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Entry Date", "entryDate"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "BOID", "boid"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "DP", "dp"));
                }
                if (name == ReportName.RematSummaryReport)
                {
                    aTTTableHeaders.Add(new ATTGenericReport(0.4f, "Rev No", "reversalNo"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder No", "holderNO"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Holder Name", "name"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Kitta", "kitta"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "Rev Date", "revDate"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "BOID", "boid"));
                    aTTTableHeaders.Add(new ATTGenericReport(0.6f, "ISIN", "isin"));
                }
                
            }

            return aTTTableHeaders;
        }

        public static Font GetFont(string fontName, string filename)
        {

            if (!FontFactory.IsRegistered(fontName))
            {
                var fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\" + filename;
                FontFactory.Register(fontPath);
            }
            return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        }
        public JsonResponse GenerateReport(ReportName name, JsonResponse data, string[] ReportTitles, bool isNepali = false, bool isTotal = false,
            List<ATTShareHolderReportTotalBasedOn> ReportTotalBasedOn = null, bool addSerialNo = false)
        {

            string hoderType;
            string CompanyName;



            if (data.IsSuccess)
            {

                JsonResponse json = new JsonResponse();
                string folderName = "PDFReports";
                string webRootPath = _webHostEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                FileStream fs = new FileStream(_webHostEnvironment.WebRootPath + "\\PDFReports\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.pdf", FileMode.Create, FileAccess.ReadWrite,
                FileShare.None, 4096, FileOptions.DeleteOnClose);

                using (MemoryStream ms = new MemoryStream())
                {

                    List<ATTGenericReport> headers = GenTableHeader(name, data.withBankDetails);

                    headers.ForEach(x => x.DataBaseColumnName.ToUpper());

                    Document document = new Document();

                    if (headers.Count > 6)
                    {
                        document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                    }
                    else
                    {
                        document = new Document(PageSize.A4, 25, 25, 30, 30);
                    }
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    if (name != ReportName.SummaryReportOfIssuedAndPaidPhysical)
                        if (name != ReportName.SummaryReportOfIssuedAndPaidDemate)
                            writer.PageEvent = new ITextEventsV2(ReportTitles[0], ReportTitles[1], ReportTitles[2], headers, webRootPath);

                    //Phrase phrase = null;
                    PdfPCell cell = null;
                    PdfPTable table = null;
                    BaseColor color = null;
                    document.Open();

                    document.SetMargins(30, 30, 90, 30);


                    Font font1 = FontFactory.GetFont(FontFactory.TIMES, 13);
                    Font font2 = FontFactory.GetFont(FontFactory.TIMES, 11);
                    Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 11, Font.NORMAL, BaseColor.BLUE);
                    Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 11, Font.NORMAL, BaseColor.BLACK);
                    Font fontNormalNew = FontFactory.GetFont(FontFactory.TIMES, 10, Font.NORMAL, BaseColor.BLACK);
                    Font fontNormalNepali = FontFactory.GetFont("PCS NEPAL", 11, Font.NORMAL, BaseColor.BLACK);
                    Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 11, Font.BOLD, BaseColor.BLACK);
                    Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 12, Font.UNDERLINE, BaseColor.BLACK);
                    bool isRegistered = FontFactory.IsRegistered("PCS NEPAL");
                    if (!isRegistered)
                    {
                        fontNormalNepali = GetFont("PCS NEPAL", "PCS NEPAL Normal");
                    }


                    //InstalledFontCollection fontCol = new InstalledFontCollection();
                    //for (int x = 0; x <= fontCol.Families.Length - 1; x++)
                    //{
                    //    var hellp = fontCol.Families[x].Name;
                    //}


                    color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));

                    if (name == ReportName.SummaryReportOfIssuedAndPaidPhysical || name == ReportName.SummaryReportOfIssuedAndPaidDemate)
                    {
                        if (name == ATTGenericReport.ReportName.SummaryReportOfIssuedAndPaidPhysical)
                        {
                            hoderType = "Physical";
                        }
                        else
                        {
                            hoderType = "Demate";
                        }
                        ATTSummaryReportDividend SummaryDetails = (ATTSummaryReportDividend)data.ResponseData;
                        table = new PdfPTable(1);
                        ReportsCellConfig.CreateCell(table, ReportTitles[1], fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, ReportTitles[2], fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        table.SpacingAfter = 10f;
                        document.Add(table);
                        Paragraph lineSeparatorHeading = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                        lineSeparatorHeading.SetLeading(0.5F, 0.5F);
                        document.Add(lineSeparatorHeading);
                        table = new PdfPTable(1);
                        ReportsCellConfig.CreateCell(table, "Details of total dividend warrants", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        document.Add(table);

                        float[] columnDefinitionSize = { 2F, 2F, 2F, 2F };
                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };
                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };
                        ReportsCellConfig.CreateCell(table, "No. of Holders : " + SummaryDetails.aTTTotalDividendWarrants.totalshhno.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Kitta : " + SummaryDetails.aTTTotalDividendWarrants.totalkitta.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Amount : " + SummaryDetails.aTTTotalDividendWarrants.totalamt.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Net Amount : " + SummaryDetails.aTTTotalDividendWarrants.netamt.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                        table.SpacingAfter = 10f;
                        document.Add(table);

                        //second paragraph
                        table = new PdfPTable(1);
                        ReportsCellConfig.CreateCell(table, "Details of issued(Posted and Unposted) dividend warrants: ", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        document.Add(table);

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };
                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };
                        ReportsCellConfig.CreateCell(table, "No. of Holders : " + SummaryDetails.aTTIssuedDividendWarrants.totalshhno1.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Kitta : " + SummaryDetails.aTTIssuedDividendWarrants.totalkitta1.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Amount : " + SummaryDetails.aTTIssuedDividendWarrants.totalamt1.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Net Amount : " + SummaryDetails.aTTIssuedDividendWarrants.netamt1.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        table.SpacingAfter = 5f;
                        document.Add(table);

                        //third paragraph
                        table = new PdfPTable(1);
                        ReportsCellConfig.CreateCell(table, "Details of issued and Posted dividend warrants ", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        document.Add(table);
                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };
                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };
                        ReportsCellConfig.CreateCell(table, "No. of Holders : " + SummaryDetails.aTTIssuedPostedDividendWarrants.totalshhno2.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Kitta : " + SummaryDetails.aTTIssuedPostedDividendWarrants.totalkitta2.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Amount : " + SummaryDetails.aTTIssuedPostedDividendWarrants.totalamt2.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Net Amount : " + SummaryDetails.aTTIssuedPostedDividendWarrants.netamt2.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        table.SpacingAfter = 10f;
                        document.Add(table);

                        //fourth paragraph
                        table = new PdfPTable(1);
                        ReportsCellConfig.CreateCell(table, "Details of issued but Unposted dividend warrants: ", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                        document.Add(table);

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };
                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };
                        ReportsCellConfig.CreateCell(table, "No. of Holders : " + SummaryDetails.aTTIssuedUnpostedDividendWarrants.totalshhno3.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Kitta : " + SummaryDetails.aTTIssuedUnpostedDividendWarrants.totalkitta3.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Amount : " + SummaryDetails.aTTIssuedUnpostedDividendWarrants.totalamt3.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Net Amount : " + SummaryDetails.aTTIssuedUnpostedDividendWarrants.netamt3.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        table.SpacingAfter = 10f;
                        document.Add(table);

                        //fifth paragraph
                        table = new PdfPTable(1);
                        ReportsCellConfig.CreateCell(table, "Details of Unissued  dividend warrants:", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                        document.Add(table);

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };
                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };
                        ReportsCellConfig.CreateCell(table, "No. of Holders : " + SummaryDetails.aTTUnIssuedDividendWarrants.totalshhno4.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Kitta : " + SummaryDetails.aTTUnIssuedDividendWarrants.totalkitta4.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Amount : " + SummaryDetails.aTTUnIssuedDividendWarrants.totalamt4.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Net Amount : " + SummaryDetails.aTTUnIssuedDividendWarrants.netamt4.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);


                        table.SpacingAfter = 10f;
                        document.Add(table);

                        //sixth paragraph
                        table = new PdfPTable(1);
                        ReportsCellConfig.CreateCell(table, "Details of Paid Posted  dividend warrants:", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                        document.Add(table);

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };
                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };
                        ReportsCellConfig.CreateCell(table, "No. of Holders : " + SummaryDetails.aTTPaidPostedDividendWarrants.totalshhno5.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Kitta : " + SummaryDetails.aTTPaidPostedDividendWarrants.totalkitta5.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Amount : " + SummaryDetails.aTTPaidPostedDividendWarrants.totalamt5.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Net Amount : " + SummaryDetails.aTTPaidPostedDividendWarrants.netamt5.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                        table.SpacingAfter = 10f;
                        document.Add(table);

                        //seventh paragraph
                        table = new PdfPTable(1);
                        ReportsCellConfig.CreateCell(table, "Details of Paid Unposted  dividend warrants:", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                        document.Add(table);

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };
                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };
                        ReportsCellConfig.CreateCell(table, "No. of Holders : " + SummaryDetails.aTTPaidUnpostedDividendWarrants.totalshhno6.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Kitta : " + SummaryDetails.aTTPaidUnpostedDividendWarrants.totalkitta6.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Amount : " + SummaryDetails.aTTPaidUnpostedDividendWarrants.totalamt6.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Net Amount : " + SummaryDetails.aTTPaidUnpostedDividendWarrants.netamt6.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                        table.SpacingAfter = 10f;
                        document.Add(table);

                        //EIGHTH paragraph
                        table = new PdfPTable(1);
                        ReportsCellConfig.CreateCell(table, "Details of Unpaid  dividend warrants:", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                        document.Add(table);

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };
                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };
                        ReportsCellConfig.CreateCell(table, "No. of Holders : " + SummaryDetails.aTTUnpaidDividendWarrants.totalshhno7.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Kitta : " + SummaryDetails.aTTUnpaidDividendWarrants.totalkitta7.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total Amount : " + SummaryDetails.aTTUnpaidDividendWarrants.totalamt7.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Net Amount : " + SummaryDetails.aTTUnpaidDividendWarrants.netamt7.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                    }
                    else if (name == ReportName.ShareHolderAddressTableListReport)
                    {
                        var ReportDataFromSystem = (List<ATTShareHolderReport>)data.ResponseData;
                        float[] columnDefinitionSize = { 1F, 1F, 1F };
                        var count = 0;
                        for (int i = 0; i < ReportDataFromSystem.Count; i = i + 2)
                        {
                            count++;
                            table = new PdfPTable(columnDefinitionSize)
                            {

                                WidthPercentage = 100
                            };
                            cell = new PdfPCell
                            {
                                BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                            };
                            bool isJ = false;
                            bool isK = false;
                            if (i + 1 < ReportDataFromSystem.Count)
                                isJ = true;
                            if (i + 2 < ReportDataFromSystem.Count)
                                isK = true;
                            //Shholderno
                            ReportsCellConfig.CreateCell(table, "No: " + (ReportDataFromSystem[i].ShHolderNo ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isJ) ReportsCellConfig.CreateCell(table, "No: " + (ReportDataFromSystem[i + 1].ShHolderNo ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isK) ReportsCellConfig.CreateCell(table, "No: " + (ReportDataFromSystem[i + 2].ShHolderNo ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            //name
                            ReportsCellConfig.CreateCell(table, "Name: " + (ReportDataFromSystem[i].Name ?? "---") + (ReportDataFromSystem[i].ShHolderNo + "/" + ReportDataFromSystem[i].TotalKitta), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isJ) ReportsCellConfig.CreateCell(table, "Name: " + (ReportDataFromSystem[i + 1].Name ?? "---") + (ReportDataFromSystem[i].ShHolderNo + "/" + ReportDataFromSystem[i].TotalKitta), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isK) ReportsCellConfig.CreateCell(table, "Name: " + (ReportDataFromSystem[i + 2].Name ?? "---") + (ReportDataFromSystem[i].ShHolderNo + "/" + ReportDataFromSystem[i].TotalKitta), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            //Address
                            ReportsCellConfig.CreateCell(table, "Address: " + (ReportDataFromSystem[i].Address ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isJ) ReportsCellConfig.CreateCell(table, "Address: " + (ReportDataFromSystem[i + 1].Address ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isK) ReportsCellConfig.CreateCell(table, "Address: " + (ReportDataFromSystem[i + 2].Address ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            //EnDistName
                            ReportsCellConfig.CreateCell(table, "DistName: " + (ReportDataFromSystem[i].EnDistName ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isJ) ReportsCellConfig.CreateCell(table, "DistName: " + (ReportDataFromSystem[i + 1].EnDistName ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isK) ReportsCellConfig.CreateCell(table, "DistName: " + (ReportDataFromSystem[i + 2].EnDistName ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            //telno
                            ReportsCellConfig.CreateCell(table, "TelNo: " + (ReportDataFromSystem[i].TelNo ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isJ) ReportsCellConfig.CreateCell(table, "TelNo: " + (ReportDataFromSystem[i + 1].TelNo ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isK) ReportsCellConfig.CreateCell(table, "TelNo: " + (ReportDataFromSystem[i + 2].TelNo ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            //PboxNo
                            ReportsCellConfig.CreateCell(table, "PboxNo: " + (ReportDataFromSystem[i].PboxNo ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isJ) ReportsCellConfig.CreateCell(table, "PboxNo: " + (ReportDataFromSystem[i + 1].PboxNo ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                            if (isK) ReportsCellConfig.CreateCell(table, "PboxNo: " + (ReportDataFromSystem[i + 2].PboxNo ?? "---"), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);

                            table.SpacingAfter = 20f;
                            document.Add(table);
                            if (count % 7 == 0) document.NewPage();
                        }



                    }
                    else
                    {
                        table = new PdfPTable(2);

                        table.TotalWidth = document.PageSize.Width - 30f;
                        table.LockedWidth = true;
                        table.AddCell(ReportsCellConfig.PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.AddCell(ReportsCellConfig.PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.SpacingAfter = 50f;
                        document.Add(table);

                        document.SetMargins(25, 30, 90, 30);
                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                        Paragraph space = new Paragraph(new Chunk(String.Empty));
                        //document.Add(space);
                        //document.Add(line);
                        List<dynamic> ReportData = (List<dynamic>)data.ResponseData;
                        float[] columnDefinitionSize = headers.Select(x => x.ColumnDefinition).ToArray();
                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100
                        };

                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
                        };
                        var headings = ((IDictionary<string, object>)ReportData.FirstOrDefault()).Keys.Select(x => x.ToUpper()).ToList();

                        if (isTotal)
                        {
                            ReportTotalBasedOn.ForEach(x => x.TotalBasedOn = x.TotalBasedOn.ToUpper());
                            ReportTotalBasedOn.ForEach(x => x.TotalIndex = headers.FindIndex(hd => hd.DataBaseColumnName.ToUpper() == x.TotalBasedOn));
                            
                        }
                        List<dynamic> dataList = new List<dynamic>();
                       
                        dataList = ReportData;

                        decimal?[] totalList = headers.Select(x => (decimal?)null).ToArray();
                        for (int i = 0; i < dataList.Count; i++)
                        {
                            ReportsCellConfig.CreateCell(table, (i + 1).ToString(), fontNormalNew, horizontalAlignment: PdfPCell.ALIGN_CENTER, verticalAllignment: PdfPCell.ALIGN_CENTER);
                            for (int j = 1; j < headers.Count; j++)
                            {

                                var index = headings.IndexOf(headers[j].DataBaseColumnName.ToUpper());
                                var dataListValue = ((IDictionary<string, object>)dataList[i]).ToDictionary(k => k.Key.ToUpper(), v => v.Value);

                                if (isTotal)
                                {
                                    var totVal = ReportTotalBasedOn.Find(x => x.TotalIndex == j);
                                    if (totVal != null)
                                    {
                                        totVal.CalculateTotal = totVal.CalculateTotal + decimal.Parse(dataListValue[headings[index]].ToString());
                                    }
                                }
                                ReportsCellConfig.CreateCell(table, dataListValue[headings[index]] == null ? "---" : dataListValue[headings[index]].ToString(), fontNormalNew, horizontalAlignment: PdfPCell.ALIGN_CENTER, verticalAllignment: PdfPCell.ALIGN_CENTER);


                            }

                        }

                        if (isTotal)
                        {

                            table.SpacingBefore = 20f;
                            document.Add(table);
                            table = new PdfPTable(1);
                            Paragraph lineSeparatorTotal = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                            lineSeparatorTotal.SetLeading(0.5F, 0.5F);
                            document.Add(lineSeparatorTotal);
                            document.Add(table);
                            table = new PdfPTable(columnDefinitionSize)
                            {
                                WidthPercentage = 100
                            };
                            var minIndex = ReportTotalBasedOn.Min(x => x.TotalIndex);
                            var maxIndex = ReportTotalBasedOn.Max(x => x.TotalIndex);
                            for (int i = 0; i < headers.Count; i++)
                            {
                                if (i == 0) ReportsCellConfig.CreateCell(table, "Total: ", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                                else if (ReportTotalBasedOn.Find(x => x.TotalIndex == i) != null)
                                {
                                    if (isNepali)
                                        ReportsCellConfig.CreateCell(table, ReportTotalBasedOn.Find(x => x.TotalIndex == i).CalculateTotal.ToString(), fontNormalNepali, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                    else
                                        ReportsCellConfig.CreateCell(table, ReportTotalBasedOn.Find(x => x.TotalIndex == i).CalculateTotal.ToString(), fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT);

                                }
                                else
                                {
                                    ReportsCellConfig.CreateCell(table, " ", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER);

                                }
                            }


                        }
                    }
                    if (!isTotal)
                        table.SpacingBefore = 20f;
                    document.Add(table);
                    table = new PdfPTable(1);
                    Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                    lineSeparator.SetLeading(0.5F, 0.5F);
                    document.Add(lineSeparator);
                    document.Add(table);
                    document.Close();
                    document.CloseDocument();
                    document.Dispose();
                    writer.Close();
                    writer.Dispose();
                    String file = Convert.ToBase64String(ms.ToArray());
                    json.IsSuccess = true;
                    json.ResponseData = file;
                    fs.Close();
                    fs.Dispose();
                }

                return json;
            }
            else
            {
                return data;
            }
        }

        public JsonResponse GenerateDemateRemateReport(JsonResponse jsonResponse, ATTReportTypeForDemateRemate ReportData)
        {
            if (ReportData.ReportType == "D")
            {
                if (jsonResponse.ResponseData != null && Convert.ToInt32(jsonResponse.TotalRecords) != 0)
                {
                    jsonResponse.IsSuccess = true;
                    string folderName = "PDFReports";
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    FileStream fs = new FileStream(_webHostEnvironment.WebRootPath + "\\PDFReports\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.pdf", FileMode.Create, FileAccess.ReadWrite,
                    FileShare.None, 4096, FileOptions.DeleteOnClose);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        List<ATTTableHeader> tableHeaders = new List<ATTTableHeader>();
                        ATTTableHeader tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "RegNo";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Holder NO.";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "FullName";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Cert NO";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Sr No From";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Sr No To";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "No Of Shares";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Entry Date";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "DRN NO";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1.5f;
                        tableHeader.ColumnName = "Demate From";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1.5f;
                        tableHeader.ColumnName = "BO Acct No";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Prepared By";
                        tableHeaders.Add(tableHeader);



                        Document document = new Document(PageSize.A4.Rotate(), 10, 10, 30, 30);
                        document.SetMargins(30, 30, 100, 30);

                        PdfWriter writer = PdfWriter.GetInstance(document, ms);
                        string Header = "";
                        if (ReportData.ReportType == "D")
                        {
                            if (ReportData.DataType == "P")
                                Header = "Posted Demate Certificate List";
                            else if (ReportData.DataType == "U")
                                Header = "Unposted Demate Certificate List";
                            else
                                Header = "Rejected Demate Certificate List";
                        }
                        else if (ReportData.ReportType == "R")
                            if (ReportData.DataType == "P")
                                Header = "Posted Remat Certificate List";
                            else if (ReportData.DataType == "U")
                                Header = "Unposted Remat Certificate List";
                            else
                                Header = "Rejected Remat Certificate List";
                        else
                        {
                            if (ReportData.DataType == "P")
                                Header = "Posted Reverse Demate Certificate List";
                            else if (ReportData.DataType == "U")
                                Header = "Unposted Reverse Demate Certificate List";
                            else
                                Header = "Rejected Reverse Demate Certificate List";
                        }
                        writer.PageEvent = new ITextEvents(ReportData.CompCode.ToString(), ReportData.CompEnName, Header, tableHeaders, webRootPath);

                        Phrase phrase = null;
                        PdfPCell cell = null;
                        PdfPTable table = null;
                        BaseColor color = null;
                        document.Open();

                        //document.SetMargins(30, 30, 90, 30);


                        Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
                        Font font2 = FontFactory.GetFont(FontFactory.TIMES, 8);
                        Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLUE);
                        Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 9, Font.NORMAL, BaseColor.BLACK);
                        Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 8, Font.BOLD, BaseColor.BLACK);
                        Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 9, Font.UNDERLINE, BaseColor.BLACK);


                        color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));

                        List<ATTShholderCertificateListForReport> certificateDemateDetails = (List<ATTShholderCertificateListForReport>)jsonResponse.ResponseData;
                        table = new PdfPTable(2);

                        table.TotalWidth = document.PageSize.Width - 30f;
                        table.LockedWidth = true;
                        table.AddCell(ReportsCellConfig.PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.AddCell(ReportsCellConfig.PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.SpacingAfter = 1f;
                        document.Add(table);

                        //document.SetMargins(10, 10, 90, 30);

                        float[] columnDefinitionSize = { 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1.5F, 1.5F, 1F };

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100,

                        };

                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)

                        };


                        int totalShKittaSum = 0;
                        if (ReportData.SecondaryReportType == "D")
                        {
                            for (int i = 0; i < certificateDemateDetails.Count; i++)
                            {

                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRegNo.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Shholderno.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].fName + " " + certificateDemateDetails[i].lName, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sShkitta.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Sentrydate.Substring(0, 10), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sdp_code, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sdp_name, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sbo_acct_no, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sentry_user, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                totalShKittaSum += certificateDemateDetails[i].sShkitta;

                                for (int j = 0; j < certificateDemateDetails[i].aTTCertificates.Count; j++)
                                {
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].seq_no, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].certno, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].srnofrom, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].srnoto, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].shkitta, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].drn_no, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);

                                }


                            }
                        }
                        else if (ReportData.SecondaryReportType == "S")
                        {
                            for (int i = 0; i < certificateDemateDetails.Count; i++)
                            {

                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRegNo.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Shholderno.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].fName + " " + certificateDemateDetails[i].lName, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sShkitta.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Sentrydate.Substring(0, 10), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sdp_code, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sdp_name, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sbo_acct_no, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sentry_user, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                totalShKittaSum += certificateDemateDetails[i].sShkitta;
                            }
                        }
                        else
                        {

                        }
                        document.Add(table);
                        table = new PdfPTable(1);
                        Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                        lineSeparator.SetLeading(1F, 1F);
                        document.Add(lineSeparator);
                        document.Add(table);


                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100,
                        };
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total : ", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, totalShKittaSum.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);

                        document.Add(table);
                        table = new PdfPTable(1);
                        lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                        lineSeparator.SetLeading(1F, 1F);
                        document.Add(lineSeparator);
                        document.Add(table);


                        document.Close();
                        document.CloseDocument();
                        document.Dispose();
                        writer.Close();
                        writer.Dispose();
                        string file = Convert.ToBase64String(ms.ToArray());
                        jsonResponse.ResponseData = file;
                        jsonResponse.Message = ReportData.CompEnName + ' ' + Header + ".pdf";
                        fs.Close();
                        fs.Dispose();
                    }
                }

            }
            else if (ReportData.ReportType == "R")
            {
                if (jsonResponse.ResponseData != null && Convert.ToInt32(jsonResponse.TotalRecords) != 0)
                {
                    jsonResponse.IsSuccess = true;
                    string folderName = "PDFReports";
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    FileStream fs = new FileStream(_webHostEnvironment.WebRootPath + "\\PDFReports\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.pdf", FileMode.Create, FileAccess.ReadWrite,
                    FileShare.None, 4096, FileOptions.DeleteOnClose);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        List<ATTTableHeader> tableHeaders = new List<ATTTableHeader>();
                        ATTTableHeader tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "RegNo";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Holder NO.";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "FullName";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Cert NO";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Sr No From";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Sr No To";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "No Of Shares";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Entry Date";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "DRN NO";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1.5f;
                        tableHeader.ColumnName = "Demate From";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1.5f;
                        tableHeader.ColumnName = "BO Acct No";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Prepared By";
                        tableHeaders.Add(tableHeader);



                        Document document = new Document(PageSize.A4.Rotate(), 10, 10, 30, 30);
                        document.SetMargins(30, 30, 100, 30);

                        PdfWriter writer = PdfWriter.GetInstance(document, ms);
                        string Header = "";
                        if (ReportData.ReportType == "D")
                        {
                            if (ReportData.DataType == "P")
                                Header = "Posted Demate Certificate List";
                            else if (ReportData.DataType == "U")
                                Header = "Unposted Demate Certificate List";
                            else
                                Header = "Rejected Demate Certificate List";
                        }
                        else if (ReportData.ReportType == "R")
                            if (ReportData.DataType == "P")
                                Header = "Posted Remat Certificate List";
                            else if (ReportData.DataType == "U")
                                Header = "Unposted Remat Certificate List";
                            else
                                Header = "Rejected Remat Certificate List";
                        else
                        {
                            if (ReportData.DataType == "P")
                                Header = "Posted Reverse Demate Certificate List";
                            else if (ReportData.DataType == "U")
                                Header = "Unposted Reverse Demate Certificate List";
                            else
                                Header = "Rejected Reverse Demate Certificate List";
                        }
                        writer.PageEvent = new ITextEvents(ReportData.CompCode.ToString(), ReportData.CompEnName, Header, tableHeaders, webRootPath);

                        //Phrase phrase = null;
                        PdfPCell cell = null;
                        PdfPTable table = null;
                        BaseColor color = null;
                        document.Open();

                        //document.SetMargins(30, 30, 90, 30);


                        Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
                        Font font2 = FontFactory.GetFont(FontFactory.TIMES, 8);
                        Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLUE);
                        Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 9, Font.NORMAL, BaseColor.BLACK);
                        Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 11, Font.BOLD, BaseColor.BLACK);
                        Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 9, Font.UNDERLINE, BaseColor.BLACK);


                        color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));

                        List<ATTShholderCertificateListForReport> certificateDemateDetails = (List<ATTShholderCertificateListForReport>)jsonResponse.ResponseData;
                        table = new PdfPTable(2);

                        table.TotalWidth = document.PageSize.Width - 30f;
                        table.LockedWidth = true;
                        table.AddCell(ReportsCellConfig.PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.AddCell(ReportsCellConfig.PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.SpacingAfter = 1f;
                        document.Add(table);

                        //document.SetMargins(10, 10, 90, 30);

                        float[] columnDefinitionSize = { 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1.5F, 1.5F, 1F };

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100,

                        };

                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)

                        };


                        int totalShKittaSum = 0;
                        if (ReportData.SecondaryReportType == "D")
                        {
                            for (int i = 0; i < certificateDemateDetails.Count; i++)
                            {

                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRegNo.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Shholderno.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].fName + " " + certificateDemateDetails[i].lName, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sShkitta.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Sentrydate.Substring(0, 10), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sdp_code, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sdp_name, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sbo_acct_no, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sentry_user, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                totalShKittaSum += certificateDemateDetails[i].sShkitta;

                                for (int j = 0; j < certificateDemateDetails[i].aTTCertificates.Count; j++)
                                {
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].seq_no, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].certno, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].srnofrom, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].srnoto, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].shkitta, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].drn_no, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);

                                }


                            }
                        }
                        else if (ReportData.SecondaryReportType == "S")
                        {
                            for (int i = 0; i < certificateDemateDetails.Count; i++)
                            {

                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRegNo.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Shholderno.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].fName + " " + certificateDemateDetails[i].lName, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sShkitta.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Sentrydate.Substring(0, 10), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sdp_code, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sdp_name, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sbo_acct_no, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sentry_user, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                totalShKittaSum += certificateDemateDetails[i].sShkitta;
                            }
                        }
                        else
                        {

                        }
                        document.Add(table);
                        table = new PdfPTable(1);
                        Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                        lineSeparator.SetLeading(1F, 1F);
                        document.Add(lineSeparator);
                        document.Add(table);


                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100,
                        };
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total : ", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, totalShKittaSum.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);

                        document.Add(table);
                        table = new PdfPTable(1);
                        lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                        lineSeparator.SetLeading(1F, 1F);
                        document.Add(lineSeparator);
                        document.Add(table);


                        document.Close();
                        document.CloseDocument();
                        document.Dispose();
                        writer.Close();
                        writer.Dispose();
                        string file = Convert.ToBase64String(ms.ToArray());
                        jsonResponse.ResponseData = file;
                        jsonResponse.Message = "Company_" + ReportData.CompEnName + "_Code_" + ReportData.CompEnName + Header + ".pdf";
                        fs.Close();
                        fs.Dispose();
                    }
                }

            }
            else
            {
                if (jsonResponse.ResponseData != null && Convert.ToInt32(jsonResponse.TotalRecords) != 0)
                {
                    jsonResponse.IsSuccess = true;
                    string folderName = "PDFReports";
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    FileStream fs = new FileStream(_webHostEnvironment.WebRootPath + "\\PDFReports\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.pdf", FileMode.Create, FileAccess.ReadWrite,
                    FileShare.None, 4096, FileOptions.DeleteOnClose);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        List<ATTTableHeader> tableHeaders = new List<ATTTableHeader>();
                        ATTTableHeader tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "SNo";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Reverse No.";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Reg No";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Holder No";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Name";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Cert No";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Sr No From";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Sr No To";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "No Of Shares";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Tr Date";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Reversed Date";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Reversed By";
                        tableHeaders.Add(tableHeader);
                        tableHeader = new ATTTableHeader();
                        tableHeader.ColumnDefinition = 1f;
                        tableHeader.ColumnName = "Reversed Remarks";
                        tableHeaders.Add(tableHeader);



                        Document document = new Document(PageSize.A4.Rotate(), 10, 10, 30, 30);
                        document.SetMargins(30, 30, 100, 30);

                        PdfWriter writer = PdfWriter.GetInstance(document, ms);
                        string Header = "";
                        if (ReportData.ReportType == "D")
                        {
                            if (ReportData.DataType == "P")
                                Header = "Posted Demate Certificate List";
                            else if (ReportData.DataType == "U")
                                Header = "Unposted Demate Certificate List";
                            else
                                Header = "Rejected Demate Certificate List";
                        }
                        else if (ReportData.ReportType == "R")
                            if (ReportData.DataType == "P")
                                Header = "Posted Remat Certificate List";
                            else if (ReportData.DataType == "U")
                                Header = "Unposted Remat Certificate List";
                            else
                                Header = "Rejected Remat Certificate List";
                        else
                        {
                            if (ReportData.DataType == "P")
                                Header = "Posted Reverse Demate Certificate List";
                            else if (ReportData.DataType == "U")
                                Header = "Unposted Reverse Demate Certificate List";
                            else
                                Header = "Rejected Reverse Demate Certificate List";
                        }
                        writer.PageEvent = new ITextEvents(ReportData.CompCode.ToString(), ReportData.CompEnName, Header, tableHeaders, webRootPath);

                        Phrase phrase = null;
                        PdfPCell cell = null;
                        PdfPTable table = null;
                        BaseColor color = null;
                        document.Open();

                        //document.SetMargins(30, 30, 90, 30);


                        Font font1 = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10);
                        Font font2 = FontFactory.GetFont(FontFactory.TIMES, 8);
                        Font fontBlue = FontFactory.GetFont(FontFactory.TIMES, 8, Font.NORMAL, BaseColor.BLUE);
                        Font fontNormal = FontFactory.GetFont(FontFactory.TIMES, 9, Font.NORMAL, BaseColor.BLACK);
                        Font fontBold = FontFactory.GetFont(FontFactory.TIMES, 11, Font.BOLD, BaseColor.BLACK);
                        Font fontHeadings = FontFactory.GetFont(FontFactory.TIMES, 9, Font.UNDERLINE, BaseColor.BLACK);


                        color = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#A9A9A9"));

                        List<ATTShholderCertificateListForReport> certificateDemateDetails = (List<ATTShholderCertificateListForReport>)jsonResponse.ResponseData;
                        table = new PdfPTable(2);

                        table.TotalWidth = document.PageSize.Width - 30f;
                        table.LockedWidth = true;
                        table.AddCell(ReportsCellConfig.PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.AddCell(ReportsCellConfig.PhraseCellNoBorder(new Paragraph(string.Empty)));
                        table.SpacingAfter = 1f;
                        document.Add(table);

                        //document.SetMargins(10, 10, 90, 30);

                        float[] columnDefinitionSize = { 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F, 1F };

                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100,

                        };

                        cell = new PdfPCell
                        {
                            BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)

                        };


                        int totalShKittaSum = 0;
                        if (ReportData.SecondaryReportType == "D")
                        {
                            for (int i = 0; i < certificateDemateDetails.Count; i++)
                            {

                                ReportsCellConfig.CreateCell(table, (i + 1).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRevTranNo.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRegNo.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Shholderno.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].fName + " " + certificateDemateDetails[i].lName, fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sShkitta.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sTrDate.Substring(0, 10), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRevDate.Substring(0, 10), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sentry_user, fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRemarks, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                totalShKittaSum += certificateDemateDetails[i].sShkitta;

                                for (int j = 0; j < certificateDemateDetails[i].aTTCertificates.Count; j++)
                                {
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_LEFT, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].certno, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].srnofrom, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].srnoto, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].aTTCertificates[j].shkitta, fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);
                                    ReportsCellConfig.CreateCell(table, "", fontNormal, horizontalAlignment: PdfPCell.ALIGN_CENTER, header: false);

                                }


                            }
                        }
                        else if (ReportData.SecondaryReportType == "S")
                        {
                            for (int i = 0; i < certificateDemateDetails.Count; i++)
                            {

                                ReportsCellConfig.CreateCell(table, (i + 1).ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRevTranNo.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRegNo.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].Shholderno.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].fName + " " + certificateDemateDetails[i].lName, fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sShkitta.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sTrDate.Substring(0, 10), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRevDate.Substring(0, 10), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sentry_user, fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                                ReportsCellConfig.CreateCell(table, certificateDemateDetails[i].sRemarks, fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                                totalShKittaSum += certificateDemateDetails[i].sShkitta;
                            }
                        }
                        else
                        {

                        }
                        document.Add(table);
                        table = new PdfPTable(1);
                        Paragraph lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                        lineSeparator.SetLeading(1F, 1F);
                        document.Add(lineSeparator);
                        document.Add(table);


                        table = new PdfPTable(columnDefinitionSize)
                        {
                            WidthPercentage = 100,
                        };
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "Total : ", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, totalShKittaSum.ToString(), fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_LEFT);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);
                        ReportsCellConfig.CreateCell(table, "", fontBold, horizontalAlignment: PdfPCell.ALIGN_CENTER);

                        document.Add(table);
                        table = new PdfPTable(1);
                        lineSeparator = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, color, Element.ALIGN_LEFT, 1)));
                        lineSeparator.SetLeading(1F, 1F);
                        document.Add(lineSeparator);
                        document.Add(table);


                        document.Close();
                        document.CloseDocument();
                        document.Dispose();
                        writer.Close();
                        writer.Dispose();
                        string file = Convert.ToBase64String(ms.ToArray());
                        jsonResponse.ResponseData = file;
                        jsonResponse.Message = "Company_" + ReportData.CompEnName + "_Code_" + ReportData.CompEnName + Header + ".pdf";
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            return jsonResponse;
        }
    }




}
