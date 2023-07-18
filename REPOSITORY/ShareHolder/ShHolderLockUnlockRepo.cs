
using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{
    public class ShHolderLockUnlockRepo : IShHolderLockUnlockEntry
    {
        IOptions<ReadConfig> connectionString;
        public ShHolderLockUnlockRepo(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }

        public JsonResponse GetHolderByLockId(string CompCode, string LockId, string RecordType, string SelectedAction, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_LockId", LockId);
                    param.Add("@P_RecordType", RecordType);
                    param.Add("@P_SelectedAction", SelectedAction);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IPAddress);
                    param.Add("@P_DATE_NOW", DateTime.Now);




                    ATTShHolderLockUnlock aTTShHolderLockUnlock = connection.Query<ATTShHolderLockUnlock>(sql: "GET_SHHOLDER_BY_LOCK_ID", param: param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (aTTShHolderLockUnlock != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTShHolderLockUnlock;
                    }
                    else
                    {
                        response.Message = "Cannot Find The Detail !!!";
                    }
                }
                catch (Exception ex)
                {

                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }

        }

        public JsonResponse GetHolderForLockUnlock(string CompCode, string ShHolderNo, string RecordType, string SelectedAction, string Username, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_ShHolderNo", ShHolderNo);
                    param.Add("@P_RecordType", RecordType);
                    param.Add("@P_SelectedAction", SelectedAction);
                    param.Add("@P_USERNAME", Username);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);



                    ATTShHolder shHolderToReturn = connection.Query<ATTShHolder>(sql: "Get_Holder_For_Lock_UnlockRK", param: param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (shHolderToReturn != null)
                    {
                        if (RecordType == "L")
                        {
                            if (shHolderToReturn.HolderLock == "Y")
                            {
                                response.Message = "Holder Already Locked !!!";
                            }
                            else
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CompCode", CompCode);
                                param.Add("@P_ShHolderNo", ShHolderNo);
                                param.Add("@P_USERNAME", Username);
                                param.Add("@P_IP_ADDRESS", IP);
                                param.Add("@P_DATE_NOW", DateTime.Now);

                                bool exists = connection.Query<bool>(sql: "Get_Holder_For_Lock_Unlock_In_Shholder_Luck_Detail", param: param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                                if (exists)
                                {
                                    if (SelectedAction == "A")
                                    {
                                        response.Message = "Holder Already In Process For Locking !!!";
                                    }
                                    else
                                    {
                                        response.IsSuccess = true;
                                        response.ResponseData = shHolderToReturn;
                                    }
                                }
                                else
                                {
                                    response.IsSuccess = true;
                                    response.ResponseData = shHolderToReturn;
                                }
                            }
                        }
                        else
                        {
                            if (shHolderToReturn.HolderLock == "N")
                            {
                                response.Message = "Share Holder Not Locked!!!!";
                            }
                            else
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CompCode", CompCode);
                                param.Add("@P_ShHolderNo", ShHolderNo);
                                bool exists = connection.Query<bool>(sql: "Get_Holder_For_Lock_Unlock_In_Shholder_Luck_Detail", param: param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                                if (exists)
                                {
                                    if (SelectedAction == "A")
                                    {
                                        response.Message = "Holder Already In Process For Locking !!!";
                                    }
                                    else
                                    {
                                        response.IsSuccess = true;
                                        response.ResponseData = shHolderToReturn;
                                    }
                                }
                                else
                                {
                                    response.IsSuccess = true;
                                    response.ResponseData = shHolderToReturn;
                                }
                            }
                        }

                    }
                    else
                    {
                        response.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse GetMaxLockId(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    int maxLockID = connection.Query<int>(sql: "GET_MAX_LOCK_ID", param: param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    maxLockID++;
                    response.IsSuccess = true;
                    response.ResponseData = maxLockID;
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse GetRecordShHolderLuckDetail(string CompCode, string Username, string IPAddress, string SelectedAction = null, string RecordType = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_SelectedAction", SelectedAction);
                    param.Add("@P_RecordType", RecordType);
                    param.Add("@P_USERNAME", Username);
                    param.Add("@P_IP_ADDRESS", IPAddress);
                    param.Add("@P_DATE_NOW", DateTime.Now);

                    List<ATTShHolderLockUnlock> aTTShHolders = connection.Query<ATTShHolderLockUnlock>(sql: "GET_SHHOLDER_LUCK_DETAIL", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTShHolders.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTShHolders;

                    }
                    else
                    {
                        response.Message = "No Holders Found !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse SaveHolderLockUnlock(string CompCode, string ShHolderNo, string RecordType, string SelectedAction, string LockId, string LockDate, string LockRemarks, string UserName, string IP, string UnlockDate, string UnlockRemarks)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_ShHolderNo", ShHolderNo);
                    param.Add("@P_RecordType", RecordType);
                    param.Add("@P_SelectedAction", SelectedAction);
                    param.Add("@P_LockId", LockId);
                    param.Add("@P_LockDate", LockDate);
                    param.Add("@P_LockRemarks", LockRemarks);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_UnlockDate", UnlockDate);
                    param.Add("@P_UnlockRemarks", UnlockRemarks);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);
                    response = connection.Query<JsonResponse>(sql: "SAVE_HOLDER_LOCK_UNLOCKRK", param: param, transaction: null, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    if (!response.IsSuccess)
                    {

                        if (RecordType == "L")
                        {
                            switch (SelectedAction) {
                                case "A":
                                    response.Message = ATTMessages.SHARE_HOLDER_LOCK_UNLOCK.LOCK_FAILED;
                                    break;
                                case "U":
                                    response.Message = ATTMessages.SHARE_HOLDER_LOCK_UNLOCK.UPDATE_FAILED;
                                    break;
                                default:
                                    response.Message = ATTMessages.SHARE_HOLDER_LOCK_UNLOCK.DELETE_FAILED;
                                    break;

                            }


                            
                        }
                        else
                        {
                            switch (SelectedAction)
                            {
                                case "A":
                                    response.Message = ATTMessages.SHARE_HOLDER_LOCK_UNLOCK.UNLOCK_FAILED;
                                    break;
                                case "U":
                                    response.Message = ATTMessages.SHARE_HOLDER_LOCK_UNLOCK.UNLOCK_UPDATE_FAILED;
                                    break;
                                default:
                                    response.Message = ATTMessages.SHARE_HOLDER_LOCK_UNLOCK.UNLOCK_DELETE_FAILED;
                                    break;

                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }
    }
}
