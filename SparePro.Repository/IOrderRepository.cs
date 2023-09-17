using SparePro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparePro.Repository
{
    public interface IOrderRepository
    {
        long GenerateOrderID(int UserID);

        List<OrderModel> Order_FindAll(int? page, string ItemName, int? limit, int? RequestedBy, string sortBy, string direction, out int TotalCount);
        List<OrderDetailModel> OrderDetail_FindAll(long OrderID);
        OrderModel Order_Edit(long OrderID);
        ReturnMessageModel Order_Save(OrderModel objOrder, List<OrderDetailModel> ObjOrderDetail);
        ReturnMessageModel Order_Insert(OrderModel objOrder, List<OrderDetailModel> ObjOrderDetail);
        ReturnMessageModel Order_Update(OrderModel objOrder, List<OrderDetailModel> ObjOrderDetail);
        ReturnMessageModel Order_Delete(long OrderID, int LastModifiedBy);
        ReturnMessageModel OrderQuantity_PlusUpdate(long BrandID, long PartID, long ItemID, double? Quantity);
        ReturnMessageModel OrderQuantity_MinusUpdate(long BrandID, long PartID, long ItemID, double? Quantity);

    }
}
