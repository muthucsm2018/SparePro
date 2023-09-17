using SparePro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparePro.Repository
{
    public interface IPurchaseRepository
    {
        long GeneratePurchaseID(int UserID);

        List<PurchaseModel> Purchase_FindAll(int? page, int? limit, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string sortBy, string direction, out int TotalCount);
        List<PurchaseDetailModel> PurchaseDetail_FindAll(long PurchaseID);
        PurchaseModel Purchase_Edit(long PurchaseID);
        ReturnMessageModel Purchase_Save(PurchaseModel objPurchase, List<PurchaseDetailModel> ObjPurchaseDetail);
        ReturnMessageModel Purchase_Insert(PurchaseModel objPurchase, List<PurchaseDetailModel> ObjPurchaseDetail);
        ReturnMessageModel Purchase_Update(PurchaseModel objPurchase, List<PurchaseDetailModel> ObjPurchaseDetail);
        ReturnMessageModel Purchase_Delete(long PurchaseID, string PurchaseStatus, int LastModifiedBy);

        ReturnMessageModel PurchaseDetail_Save(long PurchaseID, List<PurchaseDetailModel> ObjPurchaseDetail, int UserID, string PurchaseStatus);
        ReturnMessageModel PurchaseDetail_Delete(int PurchaseDetailID, int LastModifiedBy);

        ReturnMessageModel PurchasePayment_Insert(PurchasePaymentModel objPurchasePayments);
        ReturnMessageModel PurchasePayment_Update(PurchasePaymentModel objPurchasePayments);
        ReturnMessageModel PurchasePayment_Delete(int PurchasePaymentID, int LastModifiedBy);
        List<PurchasePaymentModel> PurchasePayment_FindAll(long PurchaseID, int? page, int? limit, string sortBy, string direction, out int TotalCount);
        ReturnMessageModel PurchasePayment_Save(PurchasePaymentModel objPurchasePayments);
        ReturnMessageModel PurchasePaid_Update(long PurchaseID, int LastModifiedBy);

        ReturnMessageModel PurchaseQuantity_PlusUpdate(long BrandID, long PartID, long ItemID, double? Quantity);
        ReturnMessageModel PurchaseQuantity_MinusUpdate(long BrandID, long PartID, long ItemID, double? Quantity);

        List<StockModel> Stock_FindAll(int? page, int? limit, string sortBy, long? BrandID, long? PartID, long? ItemID, string direction, out int TotalCount);
        List<OrderReportModel> Order_FindAll(int? page, int? limit, string sortBy, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string direction, out int TotalCount);
        List<PaymentReportModel> Payment_FindAll(int? page, int? limit, string sortBy, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string direction, out int TotalCount);
        List<MonthlyReportModel> MonthlyReport_FindAll(int? page, int? limit, string sortBy, int? RequestedBy, int? Year, string Month, string direction, out int TotalCount);
    }
}
