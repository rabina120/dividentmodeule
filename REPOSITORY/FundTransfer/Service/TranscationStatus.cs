//using System;
//using System.Threading.Tasks;

//namespace REPOSITORY.FundTransfer.Service
//{
//    public class TranscationStatus
//    {
//        private readonly PaymentGatewayService _paymentGatewaySimulator = new PaymentGatewayService();
//        private readonly JobService _jobService = new JobService();
//        // Processes pending payments in batches.
//        public async Task ProcessPendingPaymentsAsync(Job job, int batchSize)
//        {
//            try
//            {
//                Logger.Log($"Starting batch processing for job {job.JobId}...");
//                using var context = new EFCoreDbContext();
//                // Fetch pending payments and initialize batch details.
//                var pendingPayments = await context.Payments.AsNoTracking()
//                    .Where(p => p.Status == "Pending")
//                    .ToListAsync();
//                job.TotalPayments = pendingPayments.Count;
//                job.BatchSize = batchSize;
//                job.TotalBatches = (int)Math.Ceiling((double)job.TotalPayments / batchSize);
//                Logger.Log($"Total payments: {job.TotalPayments}. Total batches: {job.TotalBatches}.");
//                // Process payments in batches.
//                for (int batchNumber = 1; batchNumber <= job.TotalBatches; batchNumber++)
//                {
//                    var currentBatch = pendingPayments
//                        .Skip((batchNumber - 1) * batchSize)
//                        .Take(batchSize)
//                        .ToList();
//                    if (!currentBatch.Any()) break;
//                    Logger.Log($"Processing Batch {batchNumber}/{job.TotalBatches}...");
//                    using var updateContext = new EFCoreDbContext();
//                    foreach (var payment in currentBatch)
//                    {
//                        var previousStatus = payment.Status;
//                        var failureReason = string.Empty;
//                        var newStatus = "Pending";
//                        var isSuccess = false;
//                        try
//                        {
//                            // Fetch updated status from the payment gateway.
//                            newStatus = await _paymentGatewaySimulator.GetUpdatedPaymentStatusAsync(payment);
//                            // If failed, store failure reason.
//                            if (newStatus == "Failed")
//                            {
//                                failureReason = "Payment failed due to gateway rejection.";
//                            }
//                            isSuccess = newStatus == "Completed";
//                        }
//                        catch (Exception ex)
//                        {
//                            Logger.Log($"Error for Payment ID {payment.PaymentId}: {ex.Message}");
//                        }
//                        // Update payment details.
//                        payment.Status = newStatus;
//                        payment.FailureReason = failureReason;
//                        updateContext.Entry(payment).State = EntityState.Modified;
//                        // Log job details.
//                        await _jobService.LogJobDetailsAsync(job, payment, previousStatus, isSuccess);
//                    }
//                    // Save the current batch.
//                    await updateContext.SaveChangesAsync();
//                    Logger.Log($"Batch {batchNumber} processed successfully.");
//                }
//                // Complete the job once all batches are processed.
//                await _jobService.CompleteJobAsync(job);
//            }
//            catch (Exception ex)
//            {
//                Logger.Log($"Error processing payments: {ex.Message}");
//            }
//        }
//    }
//}
//}
