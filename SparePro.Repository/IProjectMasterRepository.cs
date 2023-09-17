using SparePro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SparePro.Repository
{
    public interface IProjectMasterRepository
    {

        #region "Default Detail Master"
        List<DefaultDetailMasterModel> DefaultMasterDetail_FindAll(string DefaultHeaderID, bool? Status, int? page, int? limit, string sortBy, string direction, out int TotalCount);
        ReturnMessageModel DefaultMasterDetail_Save(DefaultDetailMasterModel ObjDefaultModel);
        ReturnMessageModel DefaultMasterDetail_Delete(string DefaultDetailID, int deletedBy);
        int DefaultMaxOrder(string DefaultHeaderID);

        #endregion 

        #region BrandMaster
        
        List<BrandMasterModel> BrandMaster_FindAll(int? page, int? limit, string BrandName, bool? Status, string sortBy, string direction, out Int32 TotalCount);
        ReturnMessageModel BrandMaster_Save(BrandMasterModel objBrandMaster);
        ReturnMessageModel BrandMaster_Delete(int BrandID, int LastModifiedBy);
        BrandMasterModel BrandMaster_Edit(int BrandID);
        byte[] GetBrandImage(long BrandID);
        #endregion

        #region PartMaster

        List<PartMasterModel> PartMaster_FindAll(int? page, int? limit, string PartName, bool? Status, string sortBy, string direction, out Int32 TotalCount);
        ReturnMessageModel PartMaster_Save(PartMasterModel objPartMaster);
        ReturnMessageModel PartMaster_Delete(int PartID, int LastModifiedBy);
        PartMasterModel PartMaster_Edit(int PartID);
        #endregion

        #region "Item Master"
        List<ItemMasterModel> ItemMaster_FindAll(int? page, int? limit, long? BrandID, long? PartID, string Item, string sortBy, string direction, bool? Status, out int TotalCount);
        ItemMasterModel ItemMaster_Edit(long ItemID);
        ReturnMessageModel ItemMaster_Save(ItemMasterModel ObjSCModel);
        ReturnMessageModel ItemMaster_Delete(long ItemID, int deletedBy);

        #endregion

        #region "Acc Payment"
        AccPaymentModel AccPayment_Total(int? paidby);
        List<AccPaymentModel> AccPayment_FindAll(int? page, int? limit, string sortBy, string direction, int? paidby, out int TotalCount);
        ReturnMessageModel AccPayment_Save(AccPaymentModel ObjSCModel);
        ReturnMessageModel AccPayment_Delete(long AccPaymentID);

        #endregion
    }
}
