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
using System.Data.Entity.Core.Objects;

namespace SparePro.Repository
{
    public class PurchaseRepository : IPurchaseRepository
    {
        SpareProEntities db = new SpareProEntities();
        CommonRepository ObjCom = new CommonRepository();

        #region "Purchase"
        public long GeneratePurchaseID(int UserID)
        {
            long maxId = 0;
            var _maxId = "";
            var count = db.TblPurchases.AsEnumerable().Count();
            if (count > 0)
            {
                try
                {
                    maxId = db.TblPurchases.Max(p => p.PurchaseID);
                    _maxId = DateTime.Now.Year.ToString().Substring(2, 2) + DateTime.Now.Month.ToString("D2") + (Convert.ToInt16(count) + 1).ToString("D4") + Convert.ToString(UserID);
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
            maxId = maxId + 1;
            return Convert.ToInt64(_maxId);
        }

        public List<PurchaseModel> Purchase_FindAll(int? page, int? limit, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string sortBy, string direction, out int TotalCount)
        {
            //List<PurchaseModel> ObjPurchases = (from PU in db.TblPurchases
            //                                          //join SM in db.TblPurchaseDetails on PU.PurchaseID equals SM.PurchaseID 
            //                                          where
            //                                          ((RequestedBy == 0 || RequestedBy == null) || PU.RequestedBy == RequestedBy) &&
            //                                           (CreatedDate == null || EntityFunctions.TruncateTime(PU.CreatedDate) == EntityFunctions.TruncateTime(CreatedDate))


            //                                    select new PurchaseModel
            //                                          {
            //                                              PurchaseID = PU.PurchaseID,
            //                                              PurchaseStatus = PU.PurchaseStatus,
            //                                              PaymentStatus = PU.PaymentStatus,
            //                                              PurchaseStatusStr = (from DM in db.TblDefaultMasterDetails where DM.DefaultDetailID == PU.PurchaseStatus select DM.DefaultTextField).FirstOrDefault(),
            //                                              PaymentStatusStr = (from DM in db.TblDefaultMasterDetails where DM.DefaultDetailID == PU.PaymentStatus select DM.DefaultTextField).FirstOrDefault(),
            //                                              CreatedDate = PU.CreatedDate,
            //                                              Note = PU.Note,
            //                                              RequestedBy = (from UMC in db.TblUserMasters where UMC.UserID == PU.RequestedBy select UMC.FirstName + " " + UMC.LastName).FirstOrDefault(),
            //                                              TotalAmount = PU.TotalAmount,
            //                                              PaidAmount = (from DM in db.TblPaymentDetails where DM.PurchaseID == PU.PurchaseID select DM.PaymentAmount).Sum(),
            //                                              BalanceAmount = 0
            //                                              //Items = string.Join(",", SM.Select(i => i.ItemID))

            //                                          }).ToList();

            List<PurchaseModel> ObjPurchases = (from PU in db.sp_frm_getSales(RequestedBy, fromDate, toDate)

                                                select new PurchaseModel
                                                {
                                                    PurchaseID = PU.PurchaseID,
                                                    CreatedDate = PU.CreatedDate,
                                                    PurchaseStatus = PU.PurchaseStatus,
                                                    PaymentStatus = PU.PaymentStatus,
                                                    RequestedBy = PU.RequestedBy,
                                                    TotalAmount = PU.TotalAmount,
                                                    PaidAmount = Convert.ToDouble(PU.PaidAmount),
                                                    BalanceAmount = PU.BalanceAmount,
                                                    Items = PU.Items,
                                                    Parts = PU.Parts,
                                                    IsChecked = false

                                                }).ToList();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {
                        case "PurchaseID":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.PurchaseID).ToList();
                            break;                       
                        case "TotalAmount":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.TotalAmount).ToList();
                            break;
                        case "PaidAmount":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.PaidAmount).ToList();
                            break;
                        case "BalanceAmount":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.BalanceAmount).ToList();
                            break;
                        case "RequestedBy":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.RequestedBy).ToList();
                            break;
                        case "PurchaseStatus":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.PurchaseStatus).ToList();
                            break;
                        case "PaymentStatus":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.PaymentStatus).ToList();
                            break;                       
                        case "DisplayCreatedDate":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.CreatedDate).ToList();
                            break;

                    }
                }
                else
                {
                    // step 7 applying sorting desc

                    switch (sortBy.Trim())
                    {
                        case "PurchaseID":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.PurchaseID).ToList();
                            break;
                        case "TotalAmount":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.TotalAmount).ToList();
                            break;
                        case "PaidAmount":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.PaidAmount).ToList();
                            break;
                        case "BalanceAmount":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.BalanceAmount).ToList();
                            break;
                        case "RequestedBy":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.RequestedBy).ToList();
                            break;
                        case "PurchaseStatus":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.PurchaseStatus).ToList();
                            break;
                        case "PaymentStatus":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.PaymentStatus).ToList();
                            break;                       
                        case "DisplayCreatedDate":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.CreatedDate).ToList();
                            break;
                    }
                }
            }
            else
            {
                //ObjPurchases = ObjPurchases.OrderByDescending(q => q.p.CreatedDate).ToList();
            }

            TotalCount = ObjPurchases.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjPurchases = ObjPurchases.Skip(start).Take(limit.Value).ToList();
            }

            return ObjPurchases;
        }

        public List<PurchaseModel> Request_FindAll(int? page, int? limit, int? RequestedBy,  string sortBy, string direction, out int TotalCount)
        {
            List<PurchaseModel> ObjPurchases = (from PU in db.TblPurchases
                                                    //join SM in db.TblPurchaseDetails on PU.PurchaseID equals SM.PurchaseID
                                                where
                                                (PU.RequestedBy == RequestedBy) 
                                               

                                                select new PurchaseModel
                                                {
                                                    PurchaseID = PU.PurchaseID,
                                                    PurchaseStatus = PU.PurchaseStatus,
                                                    PaymentStatus = PU.PaymentStatus,
                                                    PurchaseStatusStr = (from DM in db.TblDefaultMasterDetails where DM.DefaultDetailID == PU.PurchaseStatus select DM.DefaultTextField).FirstOrDefault(),
                                                    PaymentStatusStr = (from DM in db.TblDefaultMasterDetails where DM.DefaultDetailID == PU.PaymentStatus select DM.DefaultTextField).FirstOrDefault(),
                                                    CreatedDate = PU.CreatedDate.Value.ToString("dd/MMM/yyyy"),
                                                    Note = PU.Note,
                                                    RequestedBy = (from UMC in db.TblUserMasters where UMC.UserID == PU.RequestedBy select UMC.FirstName + " " + UMC.LastName).FirstOrDefault(),
                                                    TotalAmount = PU.TotalAmount,
                                                    PaidAmount = (from DM in db.TblPaymentDetails where DM.PurchaseID == PU.PurchaseID select DM.PaymentAmount).Sum(),
                                                    BalanceAmount = 0

                                                }).ToList();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {
                        case "PurchaseID":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.PurchaseID).ToList();
                            break;
                        case "TotalAmount":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.TotalAmount).ToList();
                            break;
                        case "RequestedBy":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.RequestedBy).ToList();
                            break;
                        case "PurchaseStatus":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.PurchaseStatus).ToList();
                            break;
                        case "PaymentStatus":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.PaymentStatus).ToList();
                            break;
                        case "DisplayCreatedDate":
                            ObjPurchases = ObjPurchases.OrderBy(q => q.CreatedDate).ToList();
                            break;

                    }
                }
                else
                {
                    // step 7 applying sorting desc

                    switch (sortBy.Trim())
                    {
                        case "PurchaseID":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.PurchaseID).ToList();
                            break;
                        case "TotalAmount":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.TotalAmount).ToList();
                            break;
                        case "RequestedBy":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.RequestedBy).ToList();
                            break;
                        case "PurchaseStatus":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.PurchaseStatus).ToList();
                            break;
                        case "PaymentStatus":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.PaymentStatus).ToList();
                            break;
                        case "DisplayCreatedDate":
                            ObjPurchases = ObjPurchases.OrderByDescending(q => q.CreatedDate).ToList();
                            break;
                    }
                }
            }
            else
            {
                ObjPurchases = ObjPurchases.OrderByDescending(q => q.CreatedDate).ToList();
            }

            TotalCount = ObjPurchases.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjPurchases = ObjPurchases.Skip(start).Take(limit.Value).ToList();
            }

            return ObjPurchases;
        }

        public List<PurchaseDetailModel> PurchaseDetail_FindAll(long PurchaseID)
        {
            List<PurchaseDetailModel> ObjPurchaseDetail = null;

            try
            {

                ObjPurchaseDetail = (from PR in db.TblPurchaseDetails
                                     //join PU in db.TblPurchases on PR.PurchaseID equals PU.PurchaseID
                                     where
                                     (PurchaseID == 0 || PR.PurchaseID == PurchaseID)
                                     select new PurchaseDetailModel
                                     {
                                         PurchaseDetailsID = PR.PurchaseDetailID,
                                         PurchaseID = PR.PurchaseID,
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
                ObjCom.LogPageError(e, "PurchaseDetail_FindAll");
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchaseDetail_FindAll");
            }
            return ObjPurchaseDetail;
        }

        public PurchaseModel Purchase_Edit(long PurchaseID)
        {
            PurchaseModel ObjPurchases = (from ct in db.TblPurchases
                                                where ct.PurchaseID == PurchaseID
                                                select new PurchaseModel
                                                {
                                                    PurchaseID = ct.PurchaseID,
                                                    PurchaseStatus = ct.PurchaseStatus,
                                                    PaymentStatus = ct.PaymentStatus,
                                                    PurchaseDate = ct.PurchaseDate,
                                                    Note = ct.Note,
                                                    RequestedBy = ct.RequestedBy.Value.ToString(),
                                                    TotalAmount = ct.TotalAmount,
                                                    PaidAmount = (from DM in db.TblPaymentDetails where DM.PurchaseID == ct.PurchaseID select DM.PaymentAmount).Sum()
                                                }).FirstOrDefault();


            return ObjPurchases;
        }

        public ReturnMessageModel Purchase_Save(PurchaseModel objPurchase, List<PurchaseDetailModel> ObjPurchaseDetail)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var obj = (from tblSMod in db.TblPurchases
                           where tblSMod.PurchaseID == objPurchase.PurchaseID
                           select tblSMod).SingleOrDefault();

                if (obj == null)
                {
                    ObjMessage = Purchase_Insert(objPurchase, ObjPurchaseDetail);////insert
                }
                else
                {
                    ObjMessage = Purchase_Update(objPurchase, ObjPurchaseDetail);
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "Purchase_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "Purchase_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel Purchase_Insert(PurchaseModel objPurchase, List<PurchaseDetailModel> ObjPurchaseDetail)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var RequestedBy = Int16.Parse(objPurchase.RequestedBy);
                var entity = new TblPurchas
                {
                    PurchaseID = objPurchase.PurchaseID,
                    PurchaseDate = CommonRepository.GetTimeZoneDate(),
                    PurchaseStatus = objPurchase.PurchaseStatus,
                    RequestedBy = RequestedBy,
                    PaymentStatus = SparePro.Repository.DEFAULTMASTER.DefaultDetailMaster_PaymentStatusDue_ViewID,
                    Note = objPurchase.Note,
                    TotalAmount = objPurchase.TotalAmount,
                    CreatedBy = objPurchase.CreatedBy,
                    CreatedDate = CommonRepository.GetTimeZoneDate(),
                    Status = true
                };
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                long int_PurchaseID = objPurchase.PurchaseID;

                var CreatedBy = objPurchase.CreatedBy ?? 0;
                var PurchaseStatus = objPurchase.PurchaseStatus;

                ObjMessage = PurchaseDetail_Save(int_PurchaseID, ObjPurchaseDetail, CreatedBy, PurchaseStatus);

                //ObjMessage = Purchase_AccountDetailSave(objPurchase, int_PurchaseID, objPurchase.StoreID, CreatedBy);

                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "Purchase_Insert");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "Purchase_Insert");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel Purchase_Update(PurchaseModel objPurchase, List<PurchaseDetailModel> ObjPurchaseDetail)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var objUpdate = (from ct in db.TblPurchases where ct.PurchaseID == objPurchase.PurchaseID select ct).ToList();
                var RequestedBy = Int16.Parse(objPurchase.RequestedBy);

                if (objUpdate != null && objUpdate.Count == 1)
                {
                    objUpdate[0].PurchaseStatus = objPurchase.PurchaseStatus;
                    objUpdate[0].RequestedBy = RequestedBy;
                    objUpdate[0].TotalAmount = objPurchase.TotalAmount;
                    objUpdate[0].Note = objPurchase.Note;
                    objUpdate[0].LastModifiedBy = objPurchase.LastModifiedBy;
                    objUpdate[0].LastModifiedDate = objPurchase.LastModifiedDate;

                    db.SaveChanges();

                    var CreatedBy = objPurchase.CreatedBy ?? 0;
                    var PurchaseStatus = objPurchase.PurchaseStatus;

                    ObjMessage = PurchaseDetail_Save(objPurchase.PurchaseID, ObjPurchaseDetail, CreatedBy, PurchaseStatus);

                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.UPDATE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATESUCCESS;
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "Purchase_update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "Purchase_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel Purchase_Delete(long PurchaseID, string PurchaseStatus, int LastModifiedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var objDelete = (from ct in db.TblPurchases
                                 where ct.PurchaseID == PurchaseID
                                 select ct).ToList();

                if (objDelete != null && objDelete.Count == 1)
                {

                    var objDel = (from ct in db.TblPurchaseDetails
                                  where ct.PurchaseID == PurchaseID
                                  select ct).ToList();

                    if (objDel != null)
                    {
                        for (int i = 0; i < objDel.Count; i++)
                        {
                            var vQuantity = objDel[i].Qty;
                            var vBrandID = objDel[i].BrandID;
                            var vPartID = objDel[i].PartID;
                            var vItemID = objDel[i].ItemID;

                            objDel[i].LastModifiedBy = LastModifiedBy;
                            objDel[i].LastModifiedDate = DateTime.Now;

                            db.SaveChanges();
                            //if (PurchaseStatus == "Cancelled")
                            //{
                                //var objProduct = (from tblSMod in db.TblPurchaseDetails
                                //                  where tblSMod.PurchaseDetailID == objDel. vBrandID && tblSMod.PartID == vPartID && tblSMod.ItemID == vItemID
                                //                  select tblSMod).ToList();

                                if (objDel != null && objDel.Count > 0)
                                {
                                    var Qty = objDel[0].Qty ?? 0;
                                    if (Qty != 0)
                                        Qty = ((Qty) - (vQuantity ?? 0));
                                    else
                                        Qty = 0;

                                    objDel[0].Qty = Qty;
                                    db.SaveChanges();
                                }
                            //}

                        }
                    }

                    objDelete[0].LastModifiedBy = LastModifiedBy;
                    objDelete[0].LastModifiedDate = DateTime.Now;
                    objDelete[0].PurchaseStatus = SparePro.Repository.DEFAULTMASTER.DefaultDetailMaster_CancelledPurchaseStatus_ViewID;

                    db.SaveChanges();

                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "Purchase_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "Purchase_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }

            return ObjMessage;
        }

        public ReturnMessageModel PurchaseDetail_Save(long PurchaseID, List<PurchaseDetailModel> ObjPurchaseDetail, int UserID, string PurchaseStatus)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                if (ObjPurchaseDetail != null && ObjPurchaseDetail.Count != 0)
                {
                    var VEditFlag = 0;

                    var objSR = (from tblSMod in db.TblPurchaseDetails
                                 where tblSMod.PurchaseID == PurchaseID
                                 select tblSMod).ToList();

                    if (objSR != null && objSR.Count > 0)
                    {
                        VEditFlag = 1;
                        foreach (var tblSMod in objSR)
                        {
                            db.TblPurchaseDetails.Remove(tblSMod);
                            db.SaveChanges();
                        }
                    }


                    for (int i = 0; i < ObjPurchaseDetail.Count; i++)
                    {

                        var entity1 = new TblPurchaseDetail
                        {
                            PurchaseID = PurchaseID,
                            BrandID = ObjPurchaseDetail[i].BrandID,
                            PartID = ObjPurchaseDetail[i].PartID,
                            ItemID = ObjPurchaseDetail[i].ItemID,
                            ItemPrice = ObjPurchaseDetail[i].ItemPrice,
                            Color = ObjPurchaseDetail[i].Color,
                            Qty = ObjPurchaseDetail[i].Quantity,
                            TotalAmount = ObjPurchaseDetail[i].TotalAmount,
                            CreatedBy = UserID,
                            CreatedDate = CommonRepository.GetTimeZoneDate()
                        };
                        db.Entry(entity1).State = EntityState.Added;
                        db.SaveChanges();

                        //if (PurchaseStatus == SparePro.Repository.DEFAULTMASTER.DefaultDetailMaster_ApprovedPurchasedStatus_ViewID)
                        //{
                        //    PurchaseQuantity_MinusUpdate(ObjPurchaseDetail[i].BrandID, ObjPurchaseDetail[i].PartID, ObjPurchaseDetail[i].ItemID, ObjPurchaseDetail[i].Quantity);
                        //}

                        //if (PurchaseStatus == SparePro.Repository.DEFAULTMASTER.DefaultDetailMaster_CancelledPurchaseStatus_ViewID)
                        //{
                        //    PurchaseQuantity_PlusUpdate(ObjPurchaseDetail[i].BrandID, ObjPurchaseDetail[i].PartID, ObjPurchaseDetail[i].ItemID, ObjPurchaseDetail[i].Quantity);
                        //}

                    }
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PurchaseDetail_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchaseDetail_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel PurchaseDetail_Delete(int PurchaseDetailID, int LastModifiedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var Detail = db.TblPurchaseDetails.Find(PurchaseDetailID);
                long int_PurchaseID = Detail.PurchaseID;
                if (Detail != null)
                {
                    db.TblPurchaseDetails.Remove(Detail);
                    db.SaveChanges();
                }
                PurchaseQuantity_PlusUpdate(Detail.BrandID, Detail.PartID, Detail.ItemID, Detail.Qty);
                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PurchasePayments_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchasePayments_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            return ObjMessage;
        }

        #endregion

        #region "Purchase Payment"

        public ReturnMessageModel PurchasePayment_Insert(PurchasePaymentModel objPurchasePayments)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var entity = new TblPaymentDetail
                {
                    PurchaseID = objPurchasePayments.PurchaseID,
                    PaymentBy = objPurchasePayments.PaymentBy,
                    CreatedBy = objPurchasePayments.CreatedBy,
                    CreatedDate = CommonRepository.GetTimeZoneDate(),
                    PaymentDate = CommonRepository.GetTimeZoneDate(),
                    PaymentAmount = Convert.ToDouble(objPurchasePayments.PaidAmount)
                };

                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();

                PurchasePaid_Update(objPurchasePayments.PurchaseID, objPurchasePayments.CreatedBy ?? 0);

                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PurchasePayments_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchasePayments_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel PurchasePayment_Update(PurchasePaymentModel objPurchasePayments)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var entity = new TblPaymentDetail
                {                   
                    PaymentBy = objPurchasePayments.PaymentBy,                   
                    PaymentDate = CommonRepository.GetTimeZoneDate(),
                    PaymentAmount = Convert.ToDouble(objPurchasePayments.PaidAmount)
                };

                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();

                PurchasePaid_Update(objPurchasePayments.PurchaseID, objPurchasePayments.CreatedBy ?? 0);

                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PurchasePayments_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchasePayments_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel PurchasePayment_Delete(int PurchasePaymentID, int LastModifiedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var Detail = db.TblPaymentDetails.Find(PurchasePaymentID);
                long int_PurchaseID = Detail.PurchaseID;
                if (Detail != null)
                {
                    db.TblPaymentDetails.Remove(Detail);
                    db.SaveChanges();
                }
                PurchasePaid_Update(int_PurchaseID, LastModifiedBy);
                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PurchasePayments_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchasePayments_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            return ObjMessage;
        }

        public List<PurchasePaymentModel> PurchasePayment_FindAll(long PurchaseID, int? page, int? limit, string sortBy, string direction, out int TotalCount)
        {
            List<PurchasePaymentModel> ObjPaymentDetail = null;

            ObjPaymentDetail = (from PM in db.TblPaymentDetails
                                where PM.PurchaseID == PurchaseID
                                                                   
                                select new PurchasePaymentModel
                                {
                                    PaymentDetailsID = PM.PurchasePaymentID,
                                    //PaymentBy = PM.PaymentBy,
                                    PaymentBy = (from DM in db.TblDefaultMasterDetails where DM.DefaultDetailID == PM.PaymentBy select DM.DefaultTextField).FirstOrDefault(),
                                    PaidAmount = PM.PaymentAmount,
                                    CreatedBy = PM.CreatedBy,
                                    CreatedDate = PM.CreatedDate,
                                    LastModifiedBy = PM.LastModifiedBy,
                                    LastModifiedDate = PM.LastModifiedDate
                                }).ToList();

            TotalCount = ObjPaymentDetail.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjPaymentDetail = ObjPaymentDetail.Skip(start).Take(limit.Value).ToList();
            }
            return ObjPaymentDetail;

        }
     
        public ReturnMessageModel PurchasePayment_Save(PurchasePaymentModel objPurchasePayments)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                if (objPurchasePayments.PaymentDetailsID == 0)
                {
                    ObjMessage = PurchasePayment_Insert(objPurchasePayments);////insert
                }
                else
                {
                    ObjMessage = PurchasePayment_Update(objPurchasePayments);////update
                }

                PurchasePaid_Update(objPurchasePayments.PurchaseID, objPurchasePayments.LastModifiedBy ?? 0);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PurchasePayments_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchasePayments_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel PurchasePaid_Update(long PurchaseID, int LastModifiedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                string Str_PaymentStatus = "";

                Double? vRecdAmount = (from ct in db.TblPaymentDetails where ct.PurchaseID == PurchaseID select ct.PaymentAmount).DefaultIfEmpty(0).Sum();

                var objUpdate = (from ct in db.TblPurchases where ct.PurchaseID == PurchaseID select ct).ToList();

                if (objUpdate != null && objUpdate.Count == 1)
                {
                    var vGrandTotal = objUpdate[0].TotalAmount;
                    var vPaid = vRecdAmount;

                    var vBal = vGrandTotal - Convert.ToDecimal(vPaid);

                    if (vGrandTotal == Convert.ToDecimal(vPaid) || vBal == 0 || vGrandTotal <= Convert.ToDecimal(vPaid))
                    {
                        Str_PaymentStatus = SparePro.Repository.DEFAULTMASTER.DefaultDetailMaster_PaymentStatusPaid_ViewID;
                    }
                    else if (vPaid == 0 && vGrandTotal > 0)
                    {
                        Str_PaymentStatus = SparePro.Repository.DEFAULTMASTER.DefaultDetailMaster_PaymentStatusDue_ViewID;
                    }
                    else if (vPaid != 0 || vBal < 0)
                    {
                        Str_PaymentStatus = SparePro.Repository.DEFAULTMASTER.DefaultDetailMaster_PaymentStatusPartial_ViewID;
                    }
                    objUpdate[0].PaymentStatus = Str_PaymentStatus;
                    objUpdate[0].LastModifiedBy = LastModifiedBy;
                    objUpdate[0].LastModifiedDate = CommonRepository.GetTimeZoneDate();
                    db.SaveChanges();
                }
                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.UPDATE;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATESUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PurchasePaid_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchasePaid_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            return ObjMessage;
        }
       
        #endregion                            

        #region "Common Functions"

        public ReturnMessageModel PurchaseQuantity_PlusUpdate(long BrandID, long PartID, long ItemID, double? Quantity)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var ObjProduct = (from ct in db.TblPurchaseDetails where ct.BrandID == BrandID && ct.PartID == PartID && ct.ItemID == ItemID select ct).ToList();

                if (ObjProduct != null && ObjProduct.Count >= 1)
                {
                    double? Dec_Quantity = ((ObjProduct[0].Qty ?? 0) + (Quantity));
                    ObjProduct[0].Qty = Dec_Quantity;
                    db.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PurchaseQuantity_PlusUpdate");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchaseQuantity_PlusUpdate");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel PurchaseQuantity_MinusUpdate(long BrandID, long PartID, long ItemID, double? Quantity)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var ObjProduct = (from ct in db.TblPurchaseDetails where ct.BrandID == BrandID && ct.PartID == PartID && ct.ItemID == ItemID select ct).ToList();

                if (ObjProduct != null && ObjProduct.Count >= 1)
                {
                    double? Dec_Quantity = ((ObjProduct[0].Qty ?? 0) - (Quantity));
                    ObjProduct[0].Qty = Dec_Quantity;

                    db.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PurchaseQuantity_MinusUpdate");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PurchaseQuantity_MinusUpdate");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        #endregion

        #region "Stock"

        public List<StockModel> Stock_FindAll(int? page, int? limit, string sortBy, long? BrandID, long? PartID, long? ItemID, string direction, out int TotalCount)
        {
            List<StockModel> ObjStocks = (from PU in db.sp_frm_get_Stock_Report(BrandID, PartID, ItemID)

                                          select new StockModel
                                          {
                                              ItemDesc = PU.ItemDesc,
                                              OrderQty = PU.OrderQty,
                                              OrderTotal = PU.OrderTotal,
                                              OrderedCost = PU.OrderedCost,
                                              PurchasedQty = PU.PurchasedQty,
                                              PurchasedCost = PU.PurchasedCost,
                                              PurchasedTotal = PU.PurchasedTotal,
                                              Stock = PU.Stock,
                                              StockTotal = PU.StockTotal
                                          }).ToList();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {
                        case "OrderQty":
                            ObjStocks = ObjStocks.OrderBy(q => q.OrderQty).ToList();
                            break;
                        case "OrderedCost":
                            ObjStocks = ObjStocks.OrderBy(q => q.OrderedCost).ToList();
                            break;
                        case "PurchasedCost":
                            ObjStocks = ObjStocks.OrderBy(q => q.PurchasedCost).ToList();
                            break;
                        case "PurchasedQty":
                            ObjStocks = ObjStocks.OrderBy(q => q.PurchasedQty).ToList();
                            break;

                    }
                }
                else
                {
                    // step 7 applying sorting desc

                    switch (sortBy.Trim())
                    {
                        case "OrderQty":
                            ObjStocks = ObjStocks.OrderByDescending(q => q.OrderQty).ToList();
                            break;
                        case "OrderedCost":
                            ObjStocks = ObjStocks.OrderByDescending(q => q.OrderedCost).ToList();
                            break;
                        case "PurchasedQty":
                            ObjStocks = ObjStocks.OrderByDescending(q => q.PurchasedQty).ToList();
                            break;
                        case "PurchasedCost":
                            ObjStocks = ObjStocks.OrderByDescending(q => q.PurchasedCost).ToList();
                            break;
                    }
                }
            }
            else
            {
                ObjStocks = ObjStocks.OrderByDescending(q => q.Stock).ToList();
            }

            TotalCount = ObjStocks.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjStocks = ObjStocks.Skip(start).Take(limit.Value).ToList();
            }

            return ObjStocks;
        }

        #endregion

        #region "Order"

        public List<OrderReportModel> Order_FindAll(int? page, int? limit, string sortBy, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string direction, out int TotalCount)
        {
            List<OrderReportModel> ObjOrders = (from PU in db.sp_frm_get_Order_Report(RequestedBy, fromDate, toDate)

                                                select new OrderReportModel
                                                {
                                                    CreatedDate = PU.CreatedDate,
                                                    RequestedBy = PU.RequestedBy,
                                                    TotalAmount = PU.TotalAmount,
                                                    Items = PU.Items,
                                                    PaymentStatus = PU.PaymentStatus
                                                }).ToList();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {
                        case "TotalAmount":
                            ObjOrders = ObjOrders.OrderBy(q => q.TotalAmount).ToList();
                            break;

                    }
                }
                else
                {
                    // step 7 applying sorting desc

                    switch (sortBy.Trim())
                    {
                        case "OrderQty":
                            ObjOrders = ObjOrders.OrderByDescending(q => q.TotalAmount).ToList();
                            break;

                    }
                }
            }
            else
            {
                // ObjOrders = ObjOrders.OrderByDescending(q => q.Stock).ToList();
            }

            TotalCount = ObjOrders.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjOrders = ObjOrders.Skip(start).Take(limit.Value).ToList();
            }

            return ObjOrders;
        }

        #endregion

        #region "Payment"

        public List<PaymentReportModel> Payment_FindAll(int? page, int? limit, string sortBy, int? RequestedBy, DateTime? fromDate, DateTime? toDate, string direction, out int TotalCount)
        {
            List<PaymentReportModel> ObjPayment = (from PU in db.sp_frm_get_Payment_Report(RequestedBy, fromDate, toDate)

                                                   select new PaymentReportModel
                                                   {
                                                       CreatedDate = PU.CreatedDate,
                                                       RequestedBy = PU.RequestedBy,
                                                       TotalAmount = PU.TotalAmount,
                                                       PaidAmount = PU.PaidAmount,
                                                       BalanceAmount = PU.BalanceAmount,
                                                       PaymentStatus = PU.PaymentStatus
                                                   }).ToList();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {
                        case "TotalAmount":
                            ObjPayment = ObjPayment.OrderBy(q => q.TotalAmount).ToList();
                            break;

                    }
                }
                else
                {
                    
                    switch (sortBy.Trim())
                    {
                        case "TotalAmount":
                            ObjPayment = ObjPayment.OrderByDescending(q => q.TotalAmount).ToList();
                            break;

                    }
                }
            }
            else
            {
                ObjPayment = ObjPayment.ToList();
            }

            TotalCount = ObjPayment.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjPayment = ObjPayment.Skip(start).Take(limit.Value).ToList();
            }

            return ObjPayment;
        }

        #endregion

        #region "MonthlyReport"

        public List<MonthlyReportModel> MonthlyReport_FindAll(int? page, int? limit, string sortBy, int? RequestedBy, int? Year, string Month, string direction, out int TotalCount)
        {
            List<MonthlyReportModel> ObjPayment = (from PU in db.sp_frm_get_Monthly_Sales_Report(RequestedBy, Year, Month)

                                                   select new MonthlyReportModel
                                                   {
                                                       Year = PU.Year, 
                                                       Month = PU.Month,
                                                       RequestedBy = PU.RequestedBy,
                                                       TotalAmount = PU.TotalAmount,
                                                       PaidAmount = PU.PaidAmount,
                                                       BalanceAmount = PU.BalanceAmount
                                                   }).ToList();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {
                        case "TotalAmount":
                            ObjPayment = ObjPayment.OrderBy(q => q.TotalAmount).ToList();
                            break;

                    }
                }
                else
                {

                    switch (sortBy.Trim())
                    {
                        case "TotalAmount":
                            ObjPayment = ObjPayment.OrderByDescending(q => q.TotalAmount).ToList();
                            break;

                    }
                }
            }
            else
            {
                ObjPayment = ObjPayment.ToList();
            }

            TotalCount = ObjPayment.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjPayment = ObjPayment.Skip(start).Take(limit.Value).ToList();
            }

            return ObjPayment;
        }

        #endregion
    }
}

