
//using Entity.Common;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace REPOSITORY.FundTransfer.Service
//{
//    public class TransactionStatusScheduler : BackgroundService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        private readonly TimeSpan _interval = TimeSpan.FromMinutes(24);

//        public TransactionStatusScheduler(IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                try
//                {
//                    await ProcessTransactionStatusAsync();
//                }
//                catch (Exception ex)
//                {
//                    // Log the exception
//                    Console.WriteLine($"Error: {ex.Message}");
//                }

//                // Wait for the next interval
//                await Task.Delay(_interval, stoppingToken);
//            }
//        }
//        private async Task ProcessTransactionStatusAsync()
//        {
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionStatusService>();

//                // Example request setup
//                var req = new ATTDataListRequest()
//                {
//                    //PageNo = req.Start,
//                    //PageSize = req.Length,
//                    //SortColumn = req.Order[0].Column,
//                    //SortColumnName = req.Order[0].ColumnName,
//                    //SortDirection = req.Order[0].Dir,
//                    //SearchValue = req.Search != null ? req.Search.Value.Trim() : ""
//                };
//                // Example values
//                string CompCode = "001";   // Example value
//                string DivCode = "106";    // Example value
//                string BatchID = "1";      // Example value
//                string BatchStatus = "CO"; // Example value

//                await transactionService.CheckAndUpdateTransactionStatuses(req, CompCode, DivCode, BatchID, BatchStatus);
//            }
//        }

        
//    }
//}

