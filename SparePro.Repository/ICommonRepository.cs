using SparePro.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparePro.Repository
{
    public interface ICommonRepository
    {
        #region "Menu Master"
        List<MenuMasterHeader> MenuMasterHeader(string RoleID);
        List<MenuMasterDetail> MenuMasterDetail(string HeaderViewID, string RoleID);

        #endregion

        List<HomePage> Load_HomePage();

        #region "Error Master"
        bool GlobalError(Exception obj_exception, string str_pagename);
        bool LogPageError(System.Data.Entity.Validation.DbEntityValidationException exe, string str_pagename);

        #endregion

        #region "Common Functions"
      
        Decimal RoundDown(Decimal number);   
        List<LoadItems_Int> Load_Status();
        List<LoadItems_BigInt> Load_SystemUserDetail();
        List<LoadItems_String> Load_UserRole();
        List<ProfileMasterModel> Load_ProfileInformationOnUserID(int UserID = 0);
        List<LoadItems_BigInt> Load_Brand();
        List<LoadItems_BigInt> Load_Part();
        string EncryptString(string Password);
        string DecryptPassword(string Password);

        string GetFileSize(long Bytes);
        List<Query> Product_Autocomplete(string term, long BrandID, long PartID);
        List<Query> Product_Autocomplete_Order(string term, long BrandID, long PartID);

        #endregion

        #region "Login Master"

        List<SessionLoginDetail> GetLoginAccess(String LoginName, String Password);
        int GetPasswordAttemptCount(String LoginName);
        SessionInvalidLogin IsRoleMapInvalidPassword(String LoginName, String Password);

        #endregion

        #region "Default Master"
        List<DefaultHeaderNameItems> Load_CommonDefaultHeaderDetails();
        List<DefaultTypeItems> Load_CommonDefaultDetails(string DefaultTypeHeaderID);
        List<DefaultTypeItems> Load_DefaultType(string DefaultTypeHeaderID);
        List<DefaultTypeItems> Load_DefaultTypeSearch(string DefaultTypeHeaderID);
        List<DefaultTypeItems> Load_DefaultTypePage(string DefaultTypeHeaderID, bool IsPage);

        #endregion
    }
}
