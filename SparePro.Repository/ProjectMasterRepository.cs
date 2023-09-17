using SparePro.Entity;
using SparePro.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Web;
using System.IO;
using System.Globalization;
using System.Drawing;

namespace SparePro.Repository
{
    public class ProjectMasterRepository : IProjectMasterRepository
    {
        CommonRepository ObjCom = new CommonRepository();
        SpareProEntities db = new SpareProEntities();

        #region "Default Master"

        public List<DefaultDetailMasterModel> DefaultMasterDetail_FindAll(string DefaultHeaderID, bool? Status, int? page, int? limit, string sortBy, string direction, out int TotalCount)
        {
            TotalCount = 0;
            List<DefaultDetailMasterModel> ObjProject = new List<DefaultDetailMasterModel>();

            try
            {


                ObjProject = (from DMD in db.TblDefaultMasterDetails
                              where DMD.DefaultHeaderID == DefaultHeaderID
                                               && ((Status == null && DMD.Status == true) || DMD.Status == Status)
                              select new DefaultDetailMasterModel
                              {
                                  DefaultDetailID = DMD.DefaultDetailID,
                                  DefaultHeaderID = DMD.DefaultHeaderID,
                                  DefaultValueField = DMD.DefaultValueField,
                                  DefaultTextField = DMD.DefaultTextField,
                                  DefaultOrder = DMD.DefaultOrder,
                                  Status = DMD.Status,
                                  CreatedDate = DMD.CreatedDate,
                                  LastModifiedDate = DMD.LastModifiedDate
                              }).ToList();


                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        switch (sortBy.Trim())
                        {
                            case "DefaultTextField":
                                ObjProject = ObjProject.OrderBy(q => q.DefaultTextField).ToList();
                                break;
                            case "DefaultOrder":
                                ObjProject = ObjProject.OrderBy(q => q.DefaultOrder).ToList();
                                break;
                            case "Status":
                                ObjProject = ObjProject.OrderBy(q => q.Status).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (sortBy.Trim())
                        {
                            case "DefaultTextField":
                                ObjProject = ObjProject.OrderByDescending(q => q.DefaultTextField).ToList();
                                break;
                            case "DefaultOrder":
                                ObjProject = ObjProject.OrderByDescending(q => q.DefaultOrder).ToList();
                                break;
                            case "Status":
                                ObjProject = ObjProject.OrderByDescending(q => q.Status).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    ObjProject = ObjProject.OrderByDescending(q => q.CreatedDate).ToList();
                }

                TotalCount = ObjProject.Count;

                if (page.HasValue && limit.HasValue)
                {
                    int start = (page.Value - 1) * limit.Value;
                    ObjProject = ObjProject.Skip(start).Take(limit.Value).ToList();
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "DefaultDetailMasterModel_FindAll");
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "DefaultDetailMasterModel_FindAll");
            }
            return ObjProject;
        }
        
        public int DefaultMaxOrder(string DefaultHeaderID)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            int DefaultMaxOrder = 0;
            try
            {

                DefaultMaxOrder = (from tblDefault in db.TblDefaultMasterDetails
                                   where tblDefault.DefaultHeaderID == DefaultHeaderID  
                                   select tblDefault.DefaultOrder??0).Max();

               
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "DefaultMaxOrder");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "DefaultMaxOrder");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }

            return DefaultMaxOrder+1;
        }

        public ReturnMessageModel DefaultMasterDetail_Save(DefaultDetailMasterModel objDefaultDetailModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {

                var objDupOrder = (from tblDefault in db.TblDefaultMasterDetails
                                   where tblDefault.DefaultHeaderID == objDefaultDetailModel.DefaultHeaderID
                                        && tblDefault.DefaultOrder == objDefaultDetailModel.DefaultOrder
                                       && tblDefault.DefaultDetailID != objDefaultDetailModel.DefaultDetailID
                                   && tblDefault.Status == true
                                   select tblDefault).ToList();

                if (objDupOrder != null && objDupOrder.Count == 0)
                {
                    var obj = (from tblDefault in db.TblDefaultMasterDetails
                               where tblDefault.DefaultTextField == objDefaultDetailModel.DefaultTextField
                                   && tblDefault.DefaultHeaderID == objDefaultDetailModel.DefaultHeaderID
                                       && tblDefault.DefaultDetailID != objDefaultDetailModel.DefaultDetailID
                               select tblDefault).ToList();

                    if (obj != null && obj.Count == 0)
                    {
                        if (objDefaultDetailModel.DefaultDetailID != "" && objDefaultDetailModel.DefaultDetailID != null)
                        {
                            ObjMessage = DefaultMasterDetail_Update(objDefaultDetailModel);
                        }
                        else
                        {
                            ObjMessage = DefaultMasterDetail_Insert(objDefaultDetailModel);
                        }
                    }
                    else
                    {
                        ObjMessage.Result = false;
                        ObjMessage.Status = WEBCONSTANTMESSAGECODE.DUPLICATE;
                        ObjMessage.Message = WEBCONSTANTMESSAGE.DUPLICATEFAIL;
                    }
                }
                else
                {
                    ObjMessage.Result = false;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.DUPLICATEORDER;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.DUPLICATEORDERFAIL;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "DefaultDetailMaster_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "DefaultDetailMaster_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }

            return ObjMessage;
        }

        public ReturnMessageModel DefaultMasterDetail_Insert(DefaultDetailMasterModel objDefaultDetailModel)
        {

            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            try
            {
                var entity = new TblDefaultMasterDetail
                {
                    DefaultDetailID = CommonRepository.RandomString(12),
                    DefaultHeaderID = objDefaultDetailModel.DefaultHeaderID,
                    DefaultOrder = objDefaultDetailModel.DefaultOrder,
                    DefaultTextField = objDefaultDetailModel.DefaultTextField,
                    DefaultValueField = objDefaultDetailModel.DefaultValueField,
                    IsPage=true,
                    IsSearch=true,
                    Status = true,
                    CreatedBy = objDefaultDetailModel.CreatedBy,
                    CreatedDate = CommonRepository.GetTimeZoneDate(),
                };
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();

                ObjMessage.Result = true;
                ObjMessage.DefaultUniqueID = entity.DefaultDetailID;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "DefaultMasterDetail_Insert");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "DefaultMasterDetail_Insert");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel DefaultMasterDetail_Update(DefaultDetailMasterModel ObjDefaultDetailModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var objDefault = (from tblDefault in db.TblDefaultMasterDetails
                                  where tblDefault.DefaultHeaderID == ObjDefaultDetailModel.DefaultHeaderID
                                  && tblDefault.DefaultDetailID == ObjDefaultDetailModel.DefaultDetailID
                                  select tblDefault).ToList();
                if (objDefault.Count > 0)
                {
                    // objDefault[0].ColorCode = ObjDefaultDetailModel.ColorCode;
                    objDefault[0].DefaultOrder = ObjDefaultDetailModel.DefaultOrder;
                    objDefault[0].DefaultTextField = ObjDefaultDetailModel.DefaultTextField.Trim();
                    objDefault[0].DefaultValueField = ObjDefaultDetailModel.DefaultValueField != null ? ObjDefaultDetailModel.DefaultValueField.Trim() : "";
                    objDefault[0].LastModifiedBy = ObjDefaultDetailModel.CreatedBy;
                    objDefault[0].LastModifiedDate = CommonRepository.GetTimeZoneDate();

                    db.SaveChanges();
                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.UPDATE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATESUCCESS;
                }
                else
                {
                    ObjMessage.Result = false;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.NORECORD;
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "DefaultDetailMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "DefaultDetailMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }

            return ObjMessage;
        }

        public ReturnMessageModel DefaultMasterDetail_Delete(string DefaultDetailID, int deletedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var objDefaultDetail = (from tblDefault in db.TblDefaultMasterDetails
                                        where tblDefault.DefaultDetailID == DefaultDetailID
                                        select tblDefault).ToList();

                objDefaultDetail[0].Status = false;
                objDefaultDetail[0].LastModifiedBy = deletedBy;
                objDefaultDetail[0].LastModifiedDate = CommonRepository.GetTimeZoneDate();
                db.SaveChanges();

                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "DefaultDetailMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "DefaultDetailMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }

            return ObjMessage;
        }
        #endregion 

        #region "BrandMaster" 
         
        public List<BrandMasterModel> BrandMaster_FindAll(int? page, int? limit, string BrandName, bool? Status, string sortBy, string direction, out Int32 TotalCount)
        {

            List<BrandMasterModel> ObjBrandMaster = (from ct in db.TblBrandMasters
                                                     where ((BrandName==null || BrandName=="") || ct.BrandName.Contains(BrandName))
                                                    
                                                     && ((Status == null && ct.Status == true) || ct.Status == Status)
                                                     select new BrandMasterModel
                                                     {
                                                         BrandID = ct.BrandID,
                                                         BrandName = ct.BrandName,
                                                         BrandImage = ct.BrandImage,
                                                         SortOrder = ct.SortOrder??0,
                                                         Status = ct.Status,
                                                         CreatedBy = ct.CreatedBy,
                                                         CreatedDate = ct.CreatedDate,
                                                         LastModifiedBy = ct.LastModifiedBy,
                                                         LastModifiedDate = ct.LastModifiedDate,

                                                     }).ToList();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {

                        case "BrandName":
                            ObjBrandMaster = ObjBrandMaster.OrderBy(q => q.BrandName).ToList();
                            break;                      

                        case "SortOrder":
                            ObjBrandMaster = ObjBrandMaster.OrderBy(q => q.SortOrder).ToList();
                            break;
                    }
                }
                else
                {
                    // step 7 applying sorting desc

                    switch (sortBy.Trim())
                    {
                        case "BrandName":
                            ObjBrandMaster = ObjBrandMaster.OrderByDescending(q => q.BrandName).ToList();
                            break;

                        case "SortOrder":
                            ObjBrandMaster = ObjBrandMaster.OrderByDescending(q => q.SortOrder).ToList();
                            break;
                    }
                }
            }
            else
            {
                ObjBrandMaster = ObjBrandMaster.OrderBy(q => q.CreatedDate).ToList();
            }


            TotalCount = ObjBrandMaster.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjBrandMaster = ObjBrandMaster.Skip(start).Take(limit.Value).ToList();
            }

            return ObjBrandMaster;
        }

        public byte[] GetBrandImage(long BrandID)
        {
            byte[] imageByteData = null;

            try
            {

                imageByteData = (from BI in db.TblBrandMasters
                                 where BI.BrandID == BrandID
                                 select BI.BrandImage).FirstOrDefault();


            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "GetBrandImage");
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "GetBrandImage");
            }

            return imageByteData;
        }

        public BrandMasterModel BrandMaster_Edit(int BrandID)
        {
            BrandMasterModel ObjBrandMaster = (from ct in db.TblBrandMasters where ct.BrandID==BrandID
                                               select new BrandMasterModel
                                               {
                                                   BrandID = ct.BrandID,
                                                   BrandName = ct.BrandName,
                                                   BrandImage = ct.BrandImage,
                                                   SortOrder = ct.SortOrder,
                                                   Status = ct.Status,
                                                   CreatedBy = ct.CreatedBy,
                                                   CreatedDate = ct.CreatedDate,
                                                   LastModifiedBy = ct.LastModifiedBy,
                                                   LastModifiedDate = ct.LastModifiedDate,
                                               }).FirstOrDefault();


            return ObjBrandMaster;
        }

        public ReturnMessageModel BrandMaster_Save(BrandMasterModel objBrandMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                if (objBrandMaster.BrandID == 0)
                {
                    ObjMessage = BrandMaster_Insert(objBrandMaster);////insert
                }
                else
                {
                    ObjMessage = BrandMaster_Update(objBrandMaster);////update
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "BrandMaster_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "BrandMaster_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }
       
        public ReturnMessageModel BrandMaster_Insert(BrandMasterModel objBrandMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {

                var entity = new TblBrandMaster
                {                   
                    BrandName = objBrandMaster.BrandName,
                    BrandImage = objBrandMaster.BrandImage !=null? ResizeImage(objBrandMaster.BrandImage) : null,
                    SortOrder = objBrandMaster.SortOrder,
                    Status = objBrandMaster.Status,
                    CreatedBy = objBrandMaster.CreatedBy,
                    CreatedDate = CommonRepository.GetTimeZoneDate(),
                };
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                ObjMessage.Result = true;
                ObjMessage.UniqueID = entity.BrandID;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "BrandMaster_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "BrandMaster_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            return ObjMessage;
        }

        internal static byte[] ResizeImage(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var image = Image.FromStream(ms);
                var width = 80;
                var height = 80;
                var newImage = new Bitmap(width, height);
                Graphics.FromImage(newImage).DrawImage(image, 0, 0, width, height);
                Bitmap bmp = new Bitmap(newImage);

                ImageConverter converter = new ImageConverter();

                data = (byte[])converter.ConvertTo(bmp, typeof(byte[]));

                return data;
            }
        }

        public ReturnMessageModel BrandMaster_Update(BrandMasterModel objBrandMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            try
            {
                var objUpdate = (from ct in db.TblBrandMasters where ct.BrandID == objBrandMaster.BrandID select ct).ToList();

                if (objUpdate != null && objUpdate.Count == 1)
                {                   
                    objUpdate[0].BrandName = objBrandMaster.BrandName;
                    if (objBrandMaster.BrandImage != null)
                    {
                        objUpdate[0].BrandImage = ResizeImage(objBrandMaster.BrandImage);
                    }
                   
                    objUpdate[0].SortOrder = objBrandMaster.SortOrder;
                    objUpdate[0].Status = objBrandMaster.Status;
                    objUpdate[0].LastModifiedBy = objBrandMaster.CreatedBy;
                    objUpdate[0].LastModifiedDate = CommonRepository.GetTimeZoneDate();

                    db.SaveChanges();
                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.UPDATE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATESUCCESS;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "BrandMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "BrandMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel BrandMaster_Delete(int BrandID, int LastModifiedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var objDelete = (from ct in db.TblBrandMasters
                                 where ct.BrandID == BrandID
                                 select ct).ToList();

                if (objDelete != null && objDelete.Count == 1)
                {

                    objDelete[0].LastModifiedBy = LastModifiedBy;
                    objDelete[0].LastModifiedDate = DateTime.Now;
                    objDelete[0].Status = false;

                    db.SaveChanges();
                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "BrandMaster_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "BrandMaster_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            return ObjMessage;
        }

        #endregion

        #region "PartMaster" 

        public List<PartMasterModel> PartMaster_FindAll(int? page, int? limit, string PartName, bool? Status, string sortBy, string direction, out Int32 TotalCount)
        {
            List<PartMasterModel> ObjPartMaster = (from ct in db.TblPartMasters
                                                     where ((PartName == null || PartName == "") || ct.PartName.Contains(PartName))

                                                     && ((Status == null && ct.Status == true) || ct.Status == Status)
                                                     select new PartMasterModel
                                                     {
                                                         PartID = ct.PartID,
                                                         PartName = ct.PartName,
                                                         SortOrder = ct.SortOrder ?? 0,
                                                         Status = ct.Status,
                                                         CreatedBy = ct.CreatedBy,
                                                         CreatedDate = ct.CreatedDate,
                                                         LastModifiedBy = ct.LastModifiedBy,
                                                         LastModifiedDate = ct.LastModifiedDate,

                                                     }).ToList();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {
                        case "PartName":
                            ObjPartMaster = ObjPartMaster.OrderBy(q => q.PartName).ToList();
                            break;

                        case "SortOrder":
                            ObjPartMaster = ObjPartMaster.OrderBy(q => q.SortOrder).ToList();
                            break;
                    }
                }
                else
                {
                    // step 7 applying sorting desc

                    switch (sortBy.Trim())
                    {
                        case "PartName":
                            ObjPartMaster = ObjPartMaster.OrderByDescending(q => q.PartName).ToList();
                            break;

                        case "SortOrder":
                            ObjPartMaster = ObjPartMaster.OrderByDescending(q => q.SortOrder).ToList();
                            break;
                    }
                }
            }
            else
            {
                ObjPartMaster = ObjPartMaster.OrderBy(q => q.CreatedDate).ToList();
            }


            TotalCount = ObjPartMaster.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjPartMaster = ObjPartMaster.Skip(start).Take(limit.Value).ToList();
            }

            return ObjPartMaster;
        }
              

        public PartMasterModel PartMaster_Edit(int PartID)
        {
            PartMasterModel ObjPartMaster = (from ct in db.TblPartMasters
                                               where ct.PartID == PartID
                                               select new PartMasterModel
                                               {
                                                   PartID = ct.PartID,
                                                   PartName = ct.PartName,
                                                   SortOrder = ct.SortOrder,
                                                   Status = ct.Status,
                                                   CreatedBy = ct.CreatedBy,
                                                   CreatedDate = ct.CreatedDate,
                                                   LastModifiedBy = ct.LastModifiedBy,
                                                   LastModifiedDate = ct.LastModifiedDate,
                                               }).FirstOrDefault();


            return ObjPartMaster;
        }

        public ReturnMessageModel PartMaster_Save(PartMasterModel objPartMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                if (objPartMaster.PartID == 0)
                {
                    ObjMessage = PartMaster_Insert(objPartMaster);////insert
                }
                else
                {
                    ObjMessage = PartMaster_Update(objPartMaster);////update
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PartMaster_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PartMaster_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel PartMaster_Insert(PartMasterModel objPartMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {

                var entity = new TblPartMaster
                {
                    PartName = objPartMaster.PartName,
                    SortOrder = objPartMaster.SortOrder,
                    Status = objPartMaster.Status,
                    CreatedBy = objPartMaster.CreatedBy,
                    CreatedDate = CommonRepository.GetTimeZoneDate(),
                };
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();
                ObjMessage.Result = true;
                ObjMessage.UniqueID = entity.PartID;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PartMaster_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PartMaster_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel PartMaster_Update(PartMasterModel objPartMaster)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            try
            {
                var objUpdate = (from ct in db.TblPartMasters where ct.PartID == objPartMaster.PartID select ct).ToList();

                if (objUpdate != null && objUpdate.Count == 1)
                {
                    objUpdate[0].PartName = objPartMaster.PartName;
                    
                    objUpdate[0].SortOrder = objPartMaster.SortOrder;
                    objUpdate[0].Status = objPartMaster.Status;
                    objUpdate[0].LastModifiedBy = objPartMaster.CreatedBy;
                    objUpdate[0].LastModifiedDate = CommonRepository.GetTimeZoneDate();

                    db.SaveChanges();
                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.UPDATE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATESUCCESS;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PartMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PartMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel PartMaster_Delete(int PartID, int LastModifiedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var objDelete = (from ct in db.TblPartMasters
                                 where ct.PartID == PartID
                                 select ct).ToList();

                if (objDelete != null && objDelete.Count == 1)
                {

                    objDelete[0].LastModifiedBy = LastModifiedBy;
                    objDelete[0].LastModifiedDate = DateTime.Now;
                    objDelete[0].Status = false;

                    db.SaveChanges();
                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "PartMaster_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "PartMaster_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            return ObjMessage;
        }

        #endregion

        #region "Item Master"
        public List<ItemMasterModel> ItemMaster_FindAll(int? page, int? limit, long? Brand, long? PartID, string Item, string sortBy, string direction, bool? Status, out int TotalCount)
        {
            
            List<ItemMasterModel> ObjSCList = (from SC in db.TblItemMasters
                                               where
                                                    ((Brand == 0 || Brand == null) || SC.BrandID == Brand)
                                                    && (PartID == null || PartID == 0 || SC.PartID == PartID)
                                                    && ((Item == "" || Item == null) || SC.ItemName.Contains(Item))
                                                    && ((Status == null && SC.Status == true) || SC.Status == Status)
                                                    select new ItemMasterModel
                                                    {
                                                        BrandID = SC.BrandID,
                                                        PartID = SC.PartID,
                                                        ItemID = SC.ItemID,
                                                        ItemName = SC.ItemName,
                                                        ItemPrice = SC.ItemPrice,
                                                        SortOrder = SC.SortOrder,
                                                        Status = SC.Status ?? true,
                                                        InStock = (from ST in db.TblOrderDetails where ST.BrandID == SC.BrandID && ST.PartID == SC.PartID && ST.ItemID == SC.ItemID select ST.Qty).DefaultIfEmpty(0).Sum() - (from ST in db.TblPurchaseDetails where ST.BrandID == SC.BrandID && ST.PartID == SC.PartID && ST.ItemID == SC.ItemID select ST.Qty).DefaultIfEmpty(0).Sum() > 0 ? true : false,
                                                        Quantity = (from ST in db.TblOrderDetails where ST.BrandID == SC.BrandID && ST.PartID == SC.PartID && ST.ItemID == SC.ItemID select ST.Qty).DefaultIfEmpty(0).Sum() - (from ST in db.TblPurchaseDetails where ST.BrandID == SC.BrandID && ST.PartID == SC.PartID && ST.ItemID == SC.ItemID select ST.Qty).DefaultIfEmpty(0).Sum(),
                                                        CreatedDate = SC.CreatedDate,
                                                        LastModifiedDate = SC.LastModifiedDate,
                                                        BrandName = (from UMC in db.TblBrandMasters where UMC.BrandID == SC.BrandID select UMC.BrandName).FirstOrDefault(),
                                                        PartName = (from SSC in db.TblPartMasters where SSC.PartID == SC.PartID select SSC.PartName).FirstOrDefault(),
                                                        CreatedUser = (from UMC in db.TblUserMasters where UMC.UserID == SC.CreatedBy select UMC.FirstName + " " + UMC.LastName).FirstOrDefault(),
                                                        LastModifiedUser = (from UMC in db.TblUserMasters where UMC.UserID == SC.LastModifiedBy select UMC.FirstName + " " + UMC.LastName).FirstOrDefault()
                                                    }).ToList();
            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim())
                    {
                        case "BrandName":
                            ObjSCList = ObjSCList.OrderBy(q => q.BrandName).ToList();
                            break;
                        case "PartName":
                            ObjSCList = ObjSCList.OrderBy(q => q.PartName).ToList();
                            break;
                        case "ItemName":
                            ObjSCList = ObjSCList.OrderBy(q => q.ItemName).ToList();
                            break;
                        case "SortOrder":
                            ObjSCList = ObjSCList.OrderBy(q => q.SortOrder).ToList();
                            break;
                        case "Quantity":
                            ObjSCList = ObjSCList.OrderBy(q => q.Quantity).ToList();
                            break;
                        case "Status":
                            ObjSCList = ObjSCList.OrderBy(q => q.Status).ToList();
                            break;
                    }
                }
                else
                {
                    // step 7 applying sorting desc

                    switch (sortBy.Trim())
                    {
                        case "BrandName":
                            ObjSCList = ObjSCList.OrderByDescending(q => q.BrandName).ToList();
                            break;
                        case "PartName":
                            ObjSCList = ObjSCList.OrderByDescending(q => q.PartName).ToList();
                            break;
                        case "ItemName":
                            ObjSCList = ObjSCList.OrderBy(q => q.ItemName).ToList();
                            break;
                        case "SortOrder":
                            ObjSCList = ObjSCList.OrderByDescending(q => q.SortOrder).ToList();
                            break;
                        case "Quantity":
                            ObjSCList = ObjSCList.OrderByDescending(q => q.Quantity).ToList();
                            break;
                        case "Status":
                            ObjSCList = ObjSCList.OrderByDescending(q => q.Status).ToList();
                            break;
                    }
                }
            }
            else
            {
                ObjSCList = ObjSCList.OrderByDescending(q => q.CreatedDate).ToList();
            }


            TotalCount = ObjSCList.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjSCList = ObjSCList.Skip(start).Take(limit.Value).ToList();
            }
            return ObjSCList;
        }
        
        public ItemMasterModel ItemMaster_Edit(long ItemID)
        {
            ItemMasterModel ObjSC = (from SC in db.TblItemMasters
                                          where SC.ItemID == ItemID
                                          select new ItemMasterModel
                                          {
                                              PartID = SC.PartID,
                                              BrandID = SC.BrandID,
                                              ItemID = SC.ItemID,
                                              ItemPrice= SC.ItemPrice,
                                              ItemName = SC.ItemName,
                                              SortOrder = SC.SortOrder,
                                              Status = SC.Status ?? true,
                                              BrandName = (from UMC in db.TblBrandMasters where UMC.BrandID == SC.BrandID select UMC.BrandName).FirstOrDefault(),
                                              PartName = (from SSC in db.TblPartMasters where SSC.PartID == SC.PartID select SSC.PartName).FirstOrDefault()
                                          }).FirstOrDefault();


            return ObjSC;
        }

        public ReturnMessageModel ItemMaster_Save(ItemMasterModel ObjSCModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            if (ObjSCModel.ItemID == 0)
            {
                ObjMessage = ItemMaster_Insert(ObjSCModel);
            }
            else
            {
                ObjMessage = ItemMaster_Update(ObjSCModel);
            }

            return ObjMessage;
        }

        public ReturnMessageModel ItemMaster_Insert(ItemMasterModel ObjSCModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            try
            {
                var entity = new TblItemMaster
                {
                    BrandID = ObjSCModel.BrandID,
                    PartID = ObjSCModel.PartID,
                    ItemName = ObjSCModel.ItemName,
                    ItemPrice = ObjSCModel.ItemPrice,
                    SortOrder = ObjSCModel.SortOrder,
                    Status = ObjSCModel.Status,
                    CreatedBy = ObjSCModel.CreatedBy,
                    CreatedDate = CommonRepository.GetTimeZoneDate()

                };
                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();

                var entity1 = new TblStock
                {
                    BrandID = ObjSCModel.BrandID,
                    PartID = ObjSCModel.PartID,
                    ItemID = entity.ItemID,
                    OrderedQty = 0,
                    SoldQty = 0

                };
                db.Entry(entity1).State = EntityState.Added;
                db.SaveChanges();

                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "ItemMaster_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "ItemMaster_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel ItemMaster_Update(ItemMasterModel ObjSCModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();

            try
            {
                var objModule = (from tblSMod in db.TblItemMasters
                                 where tblSMod.ItemID == ObjSCModel.ItemID
                                 select tblSMod).ToList();
                if (objModule != null && objModule.Count == 1)
                {
                    objModule[0].BrandID = ObjSCModel.BrandID;
                    objModule[0].PartID = ObjSCModel.PartID;
                    objModule[0].ItemName = ObjSCModel.ItemName;
                    objModule[0].ItemPrice = ObjSCModel.ItemPrice;
                    objModule[0].SortOrder = ObjSCModel.SortOrder;
                    objModule[0].Status = ObjSCModel.Status;
                    objModule[0].LastModifiedBy = ObjSCModel.CreatedBy;
                    objModule[0].LastModifiedDate = CommonRepository.GetTimeZoneDate();
                    db.SaveChanges();
                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.UPDATE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATESUCCESS;
                }


            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "ItemMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "ItemMaster_Update");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UPDATEFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel ItemMaster_Delete(long ItemID, int deletedBy)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {

                if (ItemID != 0)
                {

                    var objSC = (from tblSC in db.TblItemMasters
                                 where tblSC.ItemID == ItemID &&
                                 tblSC.Status == true
                                 select tblSC).ToList();
                    if (objSC.Count > 0)
                    {
                        objSC[0].Status = false;
                        objSC[0].LastModifiedBy = deletedBy;
                        objSC[0].LastModifiedDate = CommonRepository.GetTimeZoneDate();
                        db.SaveChanges();
                    }
                    ObjMessage.Result = true;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;
                }
                else
                {
                    ObjMessage.Result = false;
                    ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                    ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "ItemMaster_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "ItemMaster_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            return ObjMessage;
        }
        #endregion

        #region "Acc Payment"

        public AccPaymentModel AccPayment_Total(int? paidby)
        {

            AccPaymentModel ObjPaymentDetail = null;

            ObjPaymentDetail = (from PM in db.TblAccPaymentDetails
                                where ((paidby == null || paidby == 0) || PM.PaidBy == paidby)

                                select new AccPaymentModel
                                {
                                    PaidAmount = (from AP in db.TblAccPaymentDetails where (AP.PaidBy == 0 || AP.PaidBy == PM.PaidBy) select AP.PaymentAmount).Sum(),
                                    TotalAmount = (from TP in db.TblPurchases where (TP.RequestedBy == null || TP.RequestedBy == PM.PaidBy) && TP.PurchaseStatus != "7PS43KQ4OA1L" select TP.TotalAmount).Sum(),
                                }).FirstOrDefault();
           
            return ObjPaymentDetail;
        }
        public List<AccPaymentModel> AccPayment_FindAll(int? page, int? limit, string sortBy, string direction, int? paidby, out int TotalCount)
        {

            List<AccPaymentModel> ObjPaymentDetail = null;

            ObjPaymentDetail = (from PM in db.TblAccPaymentDetails
                                where ((paidby == null || paidby == 0) || PM.PaidBy == paidby)

                                select new AccPaymentModel
                                {
                                    AccPaymentID = PM.AccPaymentID,
                                    PaymentBy = (from DM in db.TblDefaultMasterDetails where DM.DefaultDetailID == PM.PaymentBy select DM.DefaultTextField).FirstOrDefault(),
                                    PaidAmount = PM.PaymentAmount,
                                    PaidBy = PM.PaidBy,
                                    PaidByName = (from UMC in db.TblUserMasters where UMC.UserID == PM.PaidBy select UMC.FirstName + " " + UMC.LastName).FirstOrDefault(),
                                    TotalAmount = (from TP in db.TblPurchases where TP.RequestedBy == paidby && TP.PurchaseStatus != "7PS43KQ4OA1L" select TP.TotalAmount).Sum(),
                                    PaymentDate = PM.PaymentDate
                                }).ToList();

            TotalCount = ObjPaymentDetail.Count;

            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                ObjPaymentDetail = ObjPaymentDetail.Skip(start).Take(limit.Value).ToList();
            }
            return ObjPaymentDetail;
        }

        public ReturnMessageModel AccPayment_Save(AccPaymentModel ObjSCModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                if (ObjSCModel.AccPaymentID == 0)
                {
                    ObjMessage = AccPayment_Insert(ObjSCModel);////insert
                }
                else
                {
                    ObjMessage = AccPayment_Update(ObjSCModel);////update
                }

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "AccPayments_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "AccPayments_Save");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.UNAUTHINSERTUPDATE;
            }
            return ObjMessage;
        }

        public ReturnMessageModel AccPayment_Insert(AccPaymentModel ObjSCModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var entity = new TblAccPaymentDetail
                {
                    AccPaymentID = ObjSCModel.AccPaymentID,
                    PaidBy = Convert.ToInt32(ObjSCModel.PaidBy),
                    PaymentBy = ObjSCModel.PaymentBy,                   
                    PaymentDate = ObjSCModel.PaymentDate,
                    PaymentAmount = Convert.ToDouble(ObjSCModel.PaidAmount)
                };

                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();

                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "AccPayments_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "AccPayments_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel AccPayment_Update(AccPaymentModel ObjSCModel)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var entity = new TblPaymentDetail
                {
                    PaymentBy = ObjSCModel.PaymentBy,
                    PaymentDate = ObjSCModel.PaymentDate,
                    PaymentAmount = Convert.ToDouble(ObjSCModel.PaidAmount)
                };

                db.Entry(entity).State = EntityState.Added;
                db.SaveChanges();

                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.INSERT;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTSUCCESS;

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "AccPayments_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "AccPayments_Insert");
                ObjMessage.Result = false;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.INSERTFAIL;
            }
            return ObjMessage;
        }

        public ReturnMessageModel AccPayment_Delete(long AccPaymentID)
        {
            ReturnMessageModel ObjMessage = new ReturnMessageModel();
            try
            {
                var Detail = db.TblAccPaymentDetails.Find(AccPaymentID);
                long int_PurchaseID = Detail.AccPaymentID;
                if (Detail != null)
                {
                    db.TblAccPaymentDetails.Remove(Detail);
                    db.SaveChanges();
                }
                ObjMessage.Result = true;
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.DELETE;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETESUCCESS;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                ObjCom.LogPageError(e, "AccPayments_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            catch (Exception e)
            {
                ObjCom.GlobalError(e, "AccPayments_Delete");
                ObjMessage.Status = WEBCONSTANTMESSAGECODE.ERROR;
                ObjMessage.Message = WEBCONSTANTMESSAGE.DELETEFAIL;
            }
            return ObjMessage;
        }
        #endregion

    }
}









