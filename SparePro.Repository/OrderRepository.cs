using SparePro.Repository.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SparePro.Model;
using SparePro.Entity;
using System.Security.Cryptography;
using System.Data.Entity;

namespace SparePro.Repository
{
    public class OrderRepository : IOrderRepository
    {
        SpareProEntities db = new SpareProEntities();
        CommonRepository ObjCom = new CommonRepository();

        #region "Order"
        public long GenerateOrderID(int UserID)
        {
            long maxId = 0;
            var _maxId = "";
            var count = db.TblOrders.AsEnumerable().Count();
            if (count > 0)
            {
                try
                {
                    maxId = db.TblOrders.Max(p => p.OrderID);
                    maxId = maxId + 1;
                    _maxId = Convert.ToInt64(maxId).ToString();
                }

                catch (Exception ex)
                {
                    maxId = 0;
                }
            }
            else
            {
                _maxId = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("D2") + "0001" + Convert.ToString(UserID);
            }
            
            return Convert.ToInt64(_maxId);
        }

        public List<OrderModel> Order_FindAll(int? page, string ItemName, int? limit, int? RequestedBy, string sortBy, string direction, out int TotalCount)
        {
            //List<OrderModel> ObjOrders = (from PU in db.TblOrders
            //                                          //join SM in db.TblOrderDetails on PU.OrderID equals SM.OrderID
            //                                          //where
            //                                          //((RequestedBy == 0 || RequestedBy == null) || PU.CreatedBy == RequestedBy)

            //                                          select new OrderModel
            //                                          {
            //                                              OrderID = PU.OrderID,
            //                                              CreatedDate = PU.CreatedDate,
            //                                              RequestedBy = (from UMC in db.TblUserMasters where UMC.UserID == PU.CreatedBy select UMC.FirstName + " " + UMC.LastName).FirstOrDefault(),
            //                                              TotalAmount= PU.TotalAmount
            //                                          }).ToList();

            List<OrderModel> ObjOrders = (from PU in db.sp_frm_getPurchases(RequestedBy, null, ItemName)

                                          select new OrderModel
                                          {
                                              OrderID = PU.OrderID,
                                              CreatedDate = PU.CreatedDate,
                                              Brands = PU.Brands,
                                              TotalAmount = PU.TotalAmount
                                          }).ToList();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {
                        case "OrderID":
                            ObjOrders = ObjOrders.OrderBy(q => q.OrderID).ToList();
                            break;

                        case "TotalAmount":
                            ObjOrders = ObjOrders.OrderBy(q => q.TotalAmount).ToList();
                            break;

                        //case "RequestedBy":
                        //    ObjOrders = ObjOrders.OrderBy(q => q.RequestedBy).ToList();
                        //    break;
                                             
                        case "DisplayCreatedDate":
                            ObjOrders = ObjOrders.OrderBy(q => q.CreatedDate).ToList();
                            break;

                    }
                }
                else
                {
                    // step 7 applying sorting desc

                    switch (sortBy.Trim())
                    {
                        case "OrderID":
                            ObjOrders = ObjOrders.OrderByDescending(q => q.OrderID).ToList();
                            break;

                        case "TotalAmount":
                            ObjOrders = ObjOrders.OrderByDescending(q => q.TotalAmount).ToList();
                            break;

                        //case "RequestedBy":
                        //    ObjOrders = ObjOrders.OrderByDescending(q => q.RequestedBy).ToList();
                        //    break;
                                             
                        case "DisplayCreatedDate":
                            ObjOrders = ObjOrders.OrderByDescending(q => q.CreatedDate).ToList();
                            break;
                    }
                }
            }
            else
            {
                //ObjOrders = ObjOrders.OrderByDescending(q => q.CreatedDate).ToList();
            }

            TotalCount = ObjOrders.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjOrders = ObjOrders.Skip(start).Take(limit.Value).ToList();
            }

            return ObjOrders;
        }

        public List<OrderDetailModel> OrderDetail_FindAll(long OrderID)
        {
            List<OrderDetailModel> ObjOrderDetail = null;

            try
            {

                ObjOrderDetail = (from PR in db.TblOrderDetails
                                     //join PU in db.TblOrders on PR.OrderID equals PU.OrderID
                                     where
                                     (OrderID == 0 || PR.OrderID == OrderID)
                                     select new OrderDetailModel
                                     {
                                         OrderDetailID = PR.OrderDetailID,
                                         OrderID = PR.OrderID,
                                         BrandID = PR.BrandID,
                                         PartID = PR.PartID,
                                         BrandName = (from DB in db.TblBrandMasters where DB.BrandID == PR.BrandID select DB.BrandName).FirstOrDefault(),
                                         PartName = (from DP in db.TblPartMasters where DP.PartID == PR.PartID select DP.PartName).FirstOrDefault(),
                                         ItemName = (from DI in db.TblItemMasters where DI.ItemID == PR.ItemID select DI.ItemName).FirstOrDefault(),
                                         ItemPrice = PR.ItemPrice ?? 0,
                                         Quantity = PR.Qty ?? 0,
                                         TotalAmount = PR.TotalAmount ?? 0,
                                         ItemID = PR.ItemID
                                     }).ToList();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "OrderDetail_FindAll");
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "OrderDetail_FindAll");
            }
            return ObjOrderDetail;
        }

        public OrderModel Order_Edit(long OrderID)
        {
            OrderModel ObjOrders = (from ct in db.TblOrders
                                                where ct.OrderID == OrderID
                                                select new OrderModel
                                                {
                                                    OrderID = ct.OrderID,
                                                    OrderDate = ct.CreatedDate,
                                                    RequestedBy = ct.CreatedBy.Value.ToString(),
                                                    TotalAmount= ct.TotalAmount
                                                }).FirstOrDefault();


            return ObjOrders;
        }

        public ReturnMessageModel Order_Save(OrderModel objOrder, List<OrderDetailModel> ObjOrderDetail)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var obj = (from tblSMod in db.TblOrders
                           where tblSMod.OrderID == objOrder.OrderID
                           select tblSMod).SingleOrDefault();

                if (obj == null)
                {
                    ObjMessage = Order_Insert(objOrder, ObjOrderDetail);////insert
                }
                else
                {
                    ObjMessage = Order_Update(objOrder, ObjOrderDetail);
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "Order_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "Order_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel Order_Insert(OrderModel objOrder, List<OrderDetailModel> ObjOrderDetail)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var entity = new TblOrder
                {
                    OrderID = objOrder.OrderID,
                    OrderDate = CommonRepository.GetTimeZoneDate(),             
                    CreatedBy = objOrder.CreatedBy,
                    CreatedDate = CommonRepository.GetTimeZoneDate(),
                    TotalAmount = objOrder.TotalAmount
                };
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                long int_OrderID = objOrder.OrderID;

                var CreatedBy = objOrder.CreatedBy ?? 0;

                ObjMessage = OrderDetail_Save(int_OrderID, ObjOrderDetail, CreatedBy);

                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "Order_Insert");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "Order_Insert");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel Order_Update(OrderModel objOrder, List<OrderDetailModel> ObjOrderDetail)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var objUpdate = (from ct in db.TblOrders where ct.OrderID == objOrder.OrderID select ct).ToList();

                if (objUpdate != null && objUpdate.Count == 1)
                {
                    objUpdate[0].LastModifiedBy = objOrder.LastModifiedBy;
                    objUpdate[0].LastModifiedDate = objOrder.LastModifiedDate;
                    objUpdate[0].TotalAmount = objOrder.TotalAmount;
                    db.SaveChanges();

                    var CreatedBy = objOrder.CreatedBy ?? 0;

                    ObjMessage = OrderDetail_Save(objOrder.OrderID, ObjOrderDetail, CreatedBy);

                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.UPDATE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATESUCCESS;
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "Order_update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "Order_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel Order_Delete(long OrderID, int LastModifiedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var objDelete = (from ct in db.TblOrders
                                 where ct.OrderID == OrderID
                                 select ct).ToList();

                if (objDelete != null && objDelete.Count == 1)
                {

                    var objDel = (from ct in db.TblOrderDetails
                                  where ct.OrderID == OrderID
                                  select ct).ToList();

                    if (objDel != null)
                    {
                        for (int i = 0; i < objDel.Count; i++)
                        {
                            var vQuantity = objDel[i].Qty;
                            var vBrandID = objDel[i].BrandID;
                            var vPartID = objDel[i].PartID;
                            var vItemID = objDel[i].ItemID;

                            var objProduct = (from tblSMod in db.TblOrderDetails
                                              where tblSMod.BrandID == vBrandID && tblSMod.PartID == vPartID && tblSMod.ItemID == vItemID
                                              select tblSMod).ToList();

                            if (objProduct != null && objProduct.Count > 0)
                            {
                                var Qty = objProduct[0].Qty ?? 0;
                                if (Qty != 0)
                                    Qty = ((Qty) - (vQuantity ?? 0));
                                else
                                    Qty = 0;

                                objProduct[0].Qty = Qty;
                                db.SaveChanges();
                            }

                        }
                    }

                    db.TblOrderDetails.RemoveRange(objDel);
                    db.TblOrders.RemoveRange(objDelete);

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "Order_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "Order_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }

            return ObjMessage;
        }

        public ReturnMessageModel OrderDetail_Save(long OrderID, List<OrderDetailModel> ObjOrderDetail, int UserID)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                if (ObjOrderDetail != null && ObjOrderDetail.Count != 0)
                {
                    var VEditFlag = 0;

                    var objSR = (from tblSMod in db.TblOrderDetails
                                 where tblSMod.OrderID == OrderID
                                 select tblSMod).ToList();

                    if (objSR != null && objSR.Count > 0)
                    {
                        VEditFlag = 1;
                        foreach (var tblSMod in objSR)
                        {
                            db.TblOrderDetails.Remove(tblSMod);
                            db.SaveChanges();
                        }
                    }

                    for (int i = 0; i < ObjOrderDetail.Count; i++)
                    {
                        var entity1 = new TblOrderDetail
                        {
                            OrderID = OrderID,
                            BrandID = ObjOrderDetail[i].BrandID,
                            PartID = ObjOrderDetail[i].PartID,
                            ItemID = ObjOrderDetail[i].ItemID,
                            Color = ObjOrderDetail[i].Color,
                            Qty = ObjOrderDetail[i].Quantity,
                            ItemPrice = ObjOrderDetail[i].ItemPrice,
                            TotalAmount= ObjOrderDetail[i].TotalAmount,
                            CreatedBy = UserID,
                            CreatedDate = CommonRepository.GetTimeZoneDate()
                        };
                        db.Entry(entity1).State = EntityState.Added;
                        db.SaveChanges();

                        //if (OrderStatus == SparePro.Repository.DEFAULTMASTER.DefaultDetailMaster_ApprovedOrderdStatus_ViewID)
                        //{
                        //    OrderQuantity_PlusUpdate(ObjOrderDetail[i].BrandID, ObjOrderDetail[i].PartID, ObjOrderDetail[i].ItemID, ObjOrderDetail[i].Quantity);
                        //}

                    }
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "OrderDetail_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "OrderDetail_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel OrderDetail_Delete(int OrderDetailID, int LastModifiedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var Detail = db.TblOrderDetails.Find(OrderDetailID);
                long int_OrderID = Detail.OrderID;
                if (Detail != null)
                {
                    db.TblOrderDetails.Remove(Detail);
                    db.SaveChanges();
                }
                OrderQuantity_PlusUpdate(Detail.BrandID, Detail.PartID, Detail.ItemID, Detail.Qty);
                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "OrderPayments_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "OrderPayments_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            return ObjMessage;
        }

        #endregion

        #region "Common Functions"

        public ReturnMessageModel OrderQuantity_PlusUpdate(long BrandID, long PartID, long ItemID, double? Quantity)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var ObjProduct = (from ct in db.TblOrderDetails where ct.BrandID == BrandID && ct.PartID == PartID && ct.ItemID == ItemID select ct).ToList();

                if (ObjProduct != null && ObjProduct.Count >= 1)
                {
                    double? Dec_Quantity = ((ObjProduct[0].Qty ?? 0) + (Quantity));
                    ObjProduct[0].Qty = Dec_Quantity;
                    db.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "OrderQuantity_PlusUpdate");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "OrderQuantity_PlusUpdate");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel OrderQuantity_MinusUpdate(long BrandID, long PartID, long ItemID, double? Quantity)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var ObjProduct = (from ct in db.TblOrderDetails where ct.BrandID == BrandID && ct.PartID == PartID && ct.ItemID == ItemID select ct).ToList();

                if (ObjProduct != null && ObjProduct.Count >= 1)
                {
                    double? Dec_Quantity = ((ObjProduct[0].Qty ?? 0) - (Quantity));
                    ObjProduct[0].Qty = Dec_Quantity;

                    db.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "OrderQuantity_MinusUpdate");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "OrderQuantity_MinusUpdate");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        #endregion
    }
}

