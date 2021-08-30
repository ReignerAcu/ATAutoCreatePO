using PX.Data;
using PX.Data.BQL;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.AP;

namespace ATAutoCreatePO
{
    public class SOOrderEntryExt : PXGraphExtension<SOOrderEntry>
    {
        public override void Initialize()
        {
            base.Initialize();
            Base.action.AddMenuAction(CreateKandreaPO);

        }
        protected void _(Events.RowSelected<SOOrder> e)
        {
            var document = Base.Document.Current;
            if (document == null) return;
            var documentExt = document?.GetExtension<SOOrderExt>();
            if (documentExt.UsrAutoPOCreate != true)
            {
                createKandreaAutoPO();
                documentExt.UsrAutoPOCreate = true;
            }
        }
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            var companyID = Base.Accessinfo.BranchID;
            Branch currentbranch = PXSelect<Branch, Where<Branch.branchID, Equal<@P.AsInt>>>.Select(Base, companyID);
            if (currentbranch.BranchCD == "XIS")
            {
                var document = Base.Document.Current;
                if (document == null) return;
                var documentExt = document?.GetExtension<SOOrderExt>();
                if (documentExt.UsrAutoPOCreate != true)
                {
                    createKandreaAutoPO();
                    documentExt.UsrAutoPOCreate = true;
                }
            }
            baseMethod();
        }
        public PXAction<PX.Objects.SO.SOOrder> CreateKandreaPO;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Create Kandrea PO")]
        protected void createKandreaPO()
        {
            var companyID = Base.Accessinfo.BranchID;
            Branch currentbranch = PXSelect<Branch, Where<Branch.branchID, Equal<@P.AsInt>>>.Select(Base, companyID);
            if (currentbranch.BranchID == 35)
            {
                var document = Base.Document.Current;
                var lines = Base.Transactions.Current;
                if (document == null || lines == null) return;
                createKandreaAutoPO();

            }
        }
        protected void createKandreaAutoPO()
        {
            var document = Base.Document.Current;
            var lines = Base.Transactions.Current;
            if (document == null || lines == null) return;
            BAccount getcustomer = PXSelect<BAccount, Where<BAccount.bAccountID, Equal<@P.AsInt>>>.Select(Base, document.CustomerID);
            if (getcustomer != null)
            {
                if (getcustomer.IsBranch == true)
                {
                    int? customerbranch = 0;
                    Branch getbranch = PXSelect<Branch, Where<Branch.bAccountID, Equal<@P.AsInt>>>.Select(Base, document.CustomerID);
                    if (getbranch != null)
                    {
                        customerbranch = getbranch.BranchID;
                    }

                    BAccount baaccount = PXSelect<BAccount, Where<BAccount.acctCD, Equal<@P.AsString>>>.Select(Base, "XIS");
                    if (baaccount != null)
                    {
                        string termid = "";
                        int? paytovendor = 0;
                        Vendor getvendordetails = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<@P.AsInt>>>.Select(Base, baaccount.BAccountID);
                        if (getvendordetails != null)
                        {
                            termid = getvendordetails.TermsID;
                            paytovendor = getvendordetails.PayToVendorID;
                        }
                        int? locationid = 0;
                        Location location = PXSelect<Location, Where<Location.bAccountID, Equal<@P.AsString>>>.Select(Base, baaccount.BAccountID);
                        if (location != null)
                        {
                            locationid = location.LocationID;
                        }
                        POOrderEntry POinstance = PXGraph.CreateInstance<POOrderEntry>();
                        POOrder poorder = new POOrder();
                        POLine poline = new POLine();
                        poorder.OrderType = "RO";
                        poorder.VendorID = baaccount.BAccountID;
                        poorder.VendorLocationID = locationid;
                        poorder.BranchID = customerbranch;
                        poorder.TermsID = termid;
                        poorder.PayToVendorID = paytovendor;
                        poline.OrderType = "RO";
                        poline.BranchID = customerbranch;
                        //should be for loop
                        poline.InventoryID = lines.InventoryID;
                        poline.SiteID = lines.SiteID;
                        poline.OrderQty = lines.OrderQty;
                        poline.CuryUnitCost = lines.UnitPrice;
                        poline.ProjectID = document.ProjectID;
                        poline.TaskID = lines.TaskID;
                        poorder = POinstance.Document.Insert(poorder);
                        poline = POinstance.Transactions.Insert(poline);
                        POinstance.Save.Press();
                    }
                }
            }


        }
    }
}
