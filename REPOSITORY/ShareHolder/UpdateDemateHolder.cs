using Dapper;
using Entity.Common;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace REPOSITORY.ShareHolder
{
    public class UpdateDemateHolder : IUpdateDemateHolder
    {
        IOptions<ReadConfig> _connectionString;

        public UpdateDemateHolder(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse UploadHolderDetails(DataTable dt, string user)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                //BULK COPY

                // Establish a connection to the database
                SqlConnection conn = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection));
                conn.Open();


                // Delete existing data from the temporary table before bulk copy
                using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM temp_demate_holder_details", conn))
                {
                    deleteCmd.ExecuteNonQuery();
                }
                // SqlBulkCopy to upload data from the DataTable to the temporary table
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = "temp_demate_holder_details";
                    bulkCopy.WriteToServer(dt);
                }


                DynamicParameters param = new DynamicParameters();
                param.Add("@Action", "UploadFile");
                param.Add("@User", user);
                List<dynamic> reportData = conn.Query<dynamic>("Update_Demate_Holder_Details", param, null, commandType: CommandType.StoredProcedure).ToList();


                // Close the connection to the database
                conn.Close();


                jsonResponse.IsSuccess = true;
                jsonResponse.Message = "Success";
                jsonResponse.ResponseData = reportData;
            }
            catch (Exception e)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = e.Message;
            }
            return jsonResponse;
        }

        public JsonResponse SaveHolderDetails(string user)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                SqlConnection conn = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection));
                conn.Open();
                SqlCommand cmd = new SqlCommand("Update_Demate_Holder_Details", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@User", user));
                cmd.Parameters.Add(new SqlParameter("@Action", "SaveDetails"));
                cmd.ExecuteNonQuery();
                conn.Close();


                jsonResponse.IsSuccess = true;
                jsonResponse.Message = "Success";
            }
            catch (Exception e)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = e.Message;
            }
            return jsonResponse;
        }
    }
}
