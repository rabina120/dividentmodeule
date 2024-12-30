using CDSMODULE.Helper;
using Entity.Common;

using Interface.CDS;
using Interface.Certificate;
using Interface.Common;
using Interface.DakhilTransfer;
using Interface.DividendManagement;
using Interface.DividendProcessing;
using Interface.Parameter;
using Interface.Reports;
using Interface.Security;
using Interface.ShareHolder;
using Interface.Signature;
using INTERFACE.DividendManagement;

using INTERFACE.Parameter;

using INTERFACE.Reports;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Repository.CDS;
using Repository.Certificate;
using Repository.Common;
using Repository.DakhilTransfer;
using Repository.DividendManagement;
using Repository.DividendProcessing;
using Repository.Parameter;
using Repository.Reports;
using Repository.Security;
using Repository.ShareHolder;
using Repository.Signature;
using REPOSITORY.DividendManagement;
using REPOSITORY.Certificate;
using REPOSITORY.Parameter;
using REPOSITORY.Reports;
using System;
using System.IO;
using REPOSITORY.FundTransfer;
using INTERFACE.FundTransfer;
using REPOSITORY.ShareHolder;

namespace Shareplus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //check database conneciton
            services.AddTransient<ICheckDatabaseConnection, CheckDatabaseConnectionRepo>();
            //Check User Menu Access
            services.AddTransient<ICheckUserAccess, CheckUserAccessRepo>();

            services.AddTransient<ILoggedinUser, _loggedInUser>();
            services.AddTransient<ILoginUser, LoginUser>();
            services.AddTransient<IUserMenu, UserMenu>();
            services.AddTransient<IAudit, AuditRepo>();
            services.AddTransient<IRoleDefine, RoleDefineRepo>();
            services.AddTransient<IUserDetails, UserDetails>();
            //user report
            services.AddTransient<IUserReport, UserReportRepo>();
            //generic report
            services.AddTransient<IGenericReport, GenericReportRepo>();
            //shholder management 
            services.AddTransient<ICertDet, CertDet>();
            services.AddTransient<ICompanyDetails, CompanyDetails>();
            services.AddTransient<IHolderInformation, HolderInformation>();
            services.AddTransient<IDistrict, District>();
            services.AddTransient<IOccupation, Occupation>();
            services.AddTransient<IMinor, Minor>();
            services.AddTransient<IShOwnerType, ShOwnerType>();
            services.AddTransient<IShOwnerSubType, ShOwnerSubType>();
            services.AddTransient<IBoToBoTransfer, BoToBoTransferRepo>();
            services.AddTransient<ICDSCompanyParameter, CDSCompanyParameterRepo>();
            //shholderposting
            services.AddTransient<IShareHolderPosting, ShareHolderPostingRepo>();
            //shholder update application entry
            services.AddTransient<IEntryUpdateApplication, EntryUpdateApplicationRepo>();
            //shholder update application posting
            services.AddTransient<IPostingUpdateApplication, PostingUpdateApplicationRepo>();
            //shholder update
            services.AddTransient<IUpdateShHolder, UpdateShHolderRepo>();
            //shholderlockunlock
            services.AddTransient<IShHolderLockUnlockEntry, ShHolderLockUnlockRepo>();
            //shhiolder lock posting
            services.AddTransient<IShHolderLockUnlockPosting, ShHolderLockUnlockPosting>();
            //shholder relative entry
            services.AddTransient<IShareHolderRelativeEntry, ShareHolderRelativeEntryRepo>();
            //shholder relative posting
            services.AddTransient<IShareHolderRelativePosting, ShareHolderRelativePostingRepo>();
            //shholder merge entry
            services.AddTransient<IHolderMergeEntry, HolderMergeEntryRepo>();
            //holdermergeposting
            services.AddTransient<IHolderMergePosting, HolderMergePostingRepo>();
            //Demat holder record
            services.AddTransient<IUpdateDemateHolder, UpdateDemateHolder>();
            //Individual DakhilTransfer
            services.AddTransient<IDakhilIndividualTransferEntry, DakhilIndividualTransferEntryRepo>();
            //Many to One Dakhiltransfer
            services.AddTransient<IDakhilManyToOneTransferEntry, DakhilManyToOneTransferRepo>();
            //Dakhil Share Transfer
            services.AddTransient<IDakhilShareTransfer, DakhilShareTransferRepo>();
            //SignatureHandling
            //signature VertificationEntry\
            services.AddTransient<ISignatureVerificationEntry, SignatureVerificationEntryRepo>();
            //signature VertificationPosting
            services.AddTransient<ISignatureVerificationPosting, SignatureVerificationPostingRepo>();
            //signature Individual Entry
            services.AddTransient<ISignatureIndividualCapture, SignatureIndividualCaptureRepo>();
            //signature Batch Entry
            services.AddTransient<ISignatureBatchCapture, SignatureBatchCaptureRepo>();
            //signature ApprovedUnapproved
            services.AddTransient<ISignatureApprovedUnapproved, SignatureApprovedUnapprovedRepo>();
            //share transaction book
            services.AddTransient<IShareTransactionBook, ShareTransactionBookRepo>();
            //holders history
            services.AddTransient<IHoldersHistory, HoldersHistory>();
            services.AddTransient<ISignature, Signature>();
            services.AddTransient<ISignatureReport, SignatureReportRepo>();
            services.AddTransient<IDemateDividend, DemateDividend>();
            services.AddTransient<IPaymentCenter, PaymentCenter>();
            services.AddTransient<IShareType, CommonRepo>();
            services.AddTransient<IDividend, Dividend>();
            services.AddTransient<IDividendTable, DividendTableRepo>();
          
            services.AddTransient<ICashDividendEntry, CashDividendEntry>();
            services.AddTransient<ICashDividendPaymentEntry, CashDividendPaymentEntry>();
            services.AddTransient<ICashDemateDividendIssueEntry, CashDemateDividendIssueEntry>();
            services.AddTransient<IDividentPaymentEntry, DividentPaymentEntry>();
            services.AddTransient<IDividentPaymentEntryPosting, DividentPaymentPosting>();
            services.AddTransient<IDividendParameterSetUp, DividendParameterSetUp>();
            services.AddTransient<IHolderBOIDList, HolderBOIDListRepo>();
            services.AddTransient<IDividendParameterPosting, DividendParameterPosting>();
            services.AddTransient<ICashIssueDividendPosting, CashIssueDividendPosting>();
            services.AddTransient<ICashDividendPaymentPosting, CashDividendPaymentPosting>();
            //undo cash dividend payment
            services.AddTransient<IUndoDividendPayment, UndoDividendPaymentRepo>();
            //undo demate dividend payment
            services.AddTransient<IUndoDematePayment, UndoDematePaymentRepo>();

            services.AddTransient<ICashDemateIssuePosting, CashDemateIssuePosting>();
            services.AddTransient<IDemateDividentPaymentEntry, DemateDividentPaymentEntry>();
            services.AddTransient<IDemateDividentPaymentPosting, DemateDividentPaymentPosting>();
            services.AddTransient<ICashDematePaymentPosting, CashDematePaymentPosting>();
            services.AddTransient<ICashDividendReport, CashDividendReport>();
            services.AddTransient<IUploadFromExcel, UploadFromExcel>();
            services.AddTransient<IInformationFromSystem, InformationFromSystemRepo>();
            services.AddTransient<IPledgeRelease, PledgeReleaseRepo>();
            services.AddTransient<ICertificateList, CertificateListRepo>();
            services.AddTransient<ICertificateReports, CertificateReports>();
            services.AddTransient<IPaymentScheduleEntry, PaymentScheduleEntryRepo>();
            services.AddTransient<IPaymentSchedulePosting, PaymentSchedulePostingRepo>();
            services.AddTransient<IDPSetup, DPSetupRepo>();
            services.AddTransient<ICompanySetup, CompanySetupRepo>();
            services.AddTransient<ICompanyCharge, CompanyChargeRepo>();
            services.AddTransient<IBrokerSetup, BrokerSetupRepo>();
            services.AddTransient<IOccupationSetup, OccuptionSetupRepo>();
            services.AddTransient<IPaymentType, PaymentTypeRepo>();
            services.AddTransient<IDematerializeEntry, DematerializeEntryRepo>();
            services.AddTransient<IDematerializePosting, DematerializePostingRepo>();
            services.AddTransient<IReMaterializeEntry, ReMaterializeEntryRepo>();
            services.AddTransient<IRematerializePosting, RematerializePostingRepo>();
            //dematerematereport
            services.AddTransient<IDemateRemateReport, DemateRemateReportRepo>();
            //Certificate Entry
            services.AddTransient<ICertificateEntry, CertificateEntryRepo>();
            //certificate split
            services.AddTransient<ICertificateSplit, CertificateSplitRepo>();
            //certificate reversal
            services.AddTransient<IReversalEntry, ReversalEntryRepo>();
            //certigficate reversal posting
            services.AddTransient<IReversalPosting, ReversalPostingRepo>();

            //certificate distribution entry
            services.AddTransient<ICERTIFICATEDISTRIBUTIONENTRY, CERTIFICATEDISTRIBUTIONENTRYREPO>();
            //bulkca entry
            services.AddTransient<IBulkCAEntry, BulkCAEntryRepo>();
            //certificate distribution posting
            services.AddTransient<ICertificateDistributionPosting, CertificateDistributionPostingRepo>();
            //Certificate split report
            services.AddTransient<ICertificateSplitReport, CertificateSplitReportRepo>();
            //Clear PSL Entry
            services.AddTransient<IClearPSLEntry, ClearPSLEntryRepo>();
            //Clear PSL Posting
            services.AddTransient<IClearPSLPosting, ClearPSLPostingRepo>();
            //PSL Report
            services.AddTransient<IPSLReport, PSLReportRepo>();
            //Daily Reprot
            services.AddTransient<IDailyReport, DailyReport>();
            //shareholder report
            services.AddTransient<IShareHolderReport, ShareHolderReportRepo>();
            //CertificateConsolidate
            services.AddTransient<ICertificateConsolidate, CertificateConsolidateRepo>();
            services.AddTransient<ICertificateConsolidatePosting, CertificateConsolidatePostingRepo>();
            services.AddTransient<IConsolidateReport, ConsolidateReportRepo>();
            // batch processing
            services.AddTransient<IBatchProcessing, BatchProcessingRepo>();
            services.AddTransient<ITransactionStatus, TransactionStatusRepo>();
            services.AddTransient<ITransactionProcessing, TransactionProcessingRepo>();
            services.AddTransient<IEsewaStatusReport, EsewaStatusReportsRepo>();
            services.AddTransient<IEService, EServiceRepo>();
            //services.AddTransient<>();


            //PSLEntry
            services.AddTransient<IPSLEntry, PSLEntryRepo>();
            services.AddTransient<IPSLEntryPosting, PSLEntryPostingRepo>();

            services.AddTransient<ILogDetails, LogDetailRepo>();
            services.AddTransient<IGenerateReport, GenerateReportRepo>();
            services.AddTransient<IEsewaReports, EsewaReportsRepo>();
            services.AddTransient<ICommon, CommonRepo>();
            services.AddTransient<ICERTIFICATE, CERTIFICATEREPO>();

            //cash dividend bulk entry inseret
            services.AddTransient<IBulkInsert, BulkInsert>();
            //chas dividend bulk insert approved
            services.AddTransient<ICashDividendBulkEntryPosting, CashDividendBulkEntryPostingRepo>();
            services.AddTransient<ICertificateSplitPosting, CertificateSplitPostingRepo>();
            services.AddTransient<ICertificateSplitPosting, CertificateSplitPostingRepo>();
            //Dakhil Transfer Report
            services.AddTransient<IDakhilTransferReport, DakhilTransferReport>();

            services.AddTransient<ICalculationRepo, CalculationRepo>();

            services.AddTransient<IOwnerCategorySetup, OwnerCategorySetupRepo>();

            //print certificates
            services.AddTransient<IPrint, PrintRepo>();

            //changepin
            services.AddTransient<IChangePin, ChangePinRepo>();

            //pool account split
            services.AddTransient<IPoolAccountSplit, PoolAccountSplitRepo>();

            //services.AddHostedService<TransactionStatusScheduler>();

            // Register the transaction service
            //services.AddScoped<ITransactionStatusService, TransactionStatusService>();
            //services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.Configure<ReadConfig>(Configuration.GetSection("ConnectionStrings"));

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMvc(option =>
            {
                option.EnableEndpointRouting = false;
                option.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());



            }).AddSessionStateTempDataProvider();
            services.AddAntiforgery();
            var keysFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp-keys");
            services.AddDataProtection()
                .SetApplicationName("Shareplus")
                .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(14));
            services.AddLogging(loggingBuilder =>
            {
                var loggingSection = Configuration.GetSection("Logging");
                var fileName = Configuration.GetSection("Logging").GetSection("File").GetSection("Path").ToString();
                loggingBuilder.AddFile(loggingSection, fileLoggerOpts =>
                {
                    fileLoggerOpts.FormatLogFileName = fileName =>
                    {
                        return String.Format(fileName, DateTime.Now);
                    };
                    fileLoggerOpts.HandleFileError = (err) =>
                    {
                        err.UseNewLogFileName(Path.GetFileNameWithoutExtension(err.LogFileName) + "_alt" + Path.GetExtension(err.LogFileName));
                    };
                });
            });
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            }
            );
            #region AntiForgery Options
            services.AddAntiforgery(options =>
            {
                // Set Cookie properties using CookieBuilder properties†.
                options.HeaderName = "XSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;

                //options.Cookie.Name = "XSRF-TOKEN";
            });

            #endregion

            //new way
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
            CookieAuthenticationDefaults.AuthenticationScheme,
            options =>
            {
                options.LoginPath = "/Security/Login/index";
                options.AccessDeniedPath = "/Security/Login/index";
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Expiration = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;

            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });

            //old way
            //services.AddAuthentication("SHAREPLUSCDSCookie")
            // .AddCookie("SHAREPLUSCDSCookie", config =>
            // {
            //     config.Cookie.Name = "SHAREPLUSCDSCookie";
            //     config.LoginPath = "/Security/Login/index";
            //     config.AccessDeniedPath = "/Security/Login/index";

            // });
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            #region who are you?
            app.UseAuthentication();
            #endregion

            #region are you allowed? 
            app.UseAuthorization();
            #endregion

            app.UseSession();
            #region XSS Headers and X-Content Type Headers
            app.Use(async (context, next) =>
            {

                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });
            #endregion

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=controller}/{action=index}/{id?}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{area=Security}/{controller=Login}/{action=index}/{id?}"
                );

            });
            app.UseCookiePolicy( new CookiePolicyOptions { Secure=CookieSecurePolicy.Always});
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
            });

        }
    }
}
