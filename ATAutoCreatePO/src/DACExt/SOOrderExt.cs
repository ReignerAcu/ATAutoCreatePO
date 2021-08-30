using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.SO;

namespace ATAutoCreatePO
{
    public class SOOrderExt : PXCacheExtension<SOOrder>
    {
            #region UsrAutoPOCreate
            [PXDBBool]
            [PXUIField(DisplayName = "AutoPOCreate", Visible = false)]
            public bool? UsrAutoPOCreate { get; set; }
            public abstract class usrAutoPOCreate : PX.Data.BQL.BqlBool.Field<usrAutoPOCreate> { }
            #endregion
    }
}
