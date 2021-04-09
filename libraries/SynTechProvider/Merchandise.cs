using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MerchandiseProvider
{
    public class MerchandiseItems : List<MerchandiseItem>
    {
        #region Private Fields
        IEnumerable<string> areas, categories, areasCategories, productCodes, stockCodes;
        #endregion

        #region Constructor
        public MerchandiseItems() { }
        public MerchandiseItems(IEnumerable<MerchandiseItem> items)
        {
            AddRange(items);
        }
        #endregion

        #region Public Properties
        public string ProviderName { get; set; }
        public string DataSourceUri { get; set; }

        public IEnumerable<string> Areas =>
                    areas == null ?
                    areas = this.Select(i => i.Area).Distinct() :
                    areas;

        public IEnumerable<string> Categories =>
                    categories == null ?
                    categories = this.Select(i => i.Category).Distinct() :
                    categories;

        public IEnumerable<string> AreasCategories =>
                    areasCategories == null ?
                    areasCategories = this.Select(i => $"{i.Area}, {i.Category}").Distinct() :
                    areasCategories;

        public IEnumerable<string> ProductCodes =>
                    productCodes == null ?
                    productCodes = productCodes = this.Select(i => i.ProductCode).Distinct() :
                    productCodes;

        public IEnumerable<string> StockCodes =>
                    stockCodes == null ?
                    stockCodes = stockCodes = this.Select(i => i.StockCode).Distinct() :
                    stockCodes;
        #endregion

        #region Public Methods
        public IEnumerable<MerchandiseItem> SelectByProductCode(string code) => this.Where(i => i.ProductCode == code);
        public MerchandiseItem GetByProductCode(string code) => this.FirstOrDefault(i => i.ProductCode == code);
        public MerchandiseItem GetByStockCode(string code) => this.FirstOrDefault(i => i.StockCode == code);
        #endregion
    }

    public class MerchandiseItem
    {
        #region Constant
        public const string SaleItemCodePrefix = "MCH-";
        #endregion

        #region Static Fields

        #endregion

        #region Static Methods

        #endregion

        #region Public Properties
        // PRODUCT
        public string Area { get; set; }
        public string Category { get; set; }
        /// <summary>
        /// Product Code (UPC - Universal Product Code)
        /// </summary>
        public string ProductCode { get; set; }
        public string ProductImage { get; set; }
        /// <summary>
        /// MCH-{ProductCode}
        /// </summary>
        public string SaleItemCode => $"{SaleItemCodePrefix}{ProductCode}";
        public bool Visible { get; set; }
        /// <summary>
        /// Stock keeping unit
        /// </summary>
        public string StockCode { get; set; }
        public string StockImage { get; set; }
        public string Name { get; set; }

        // RETAIL
        /// <summary>
        /// Cost incl. TAX
        /// </summary>
        public string Cost { get; set; }
        /// <summary>
        /// Markdown Selling Price incl. VAT
        /// </summary>
        public string Markdown { get; set; }
        /// <summary>
        /// Retail Selling Price incl. VAT
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// Units in Stock
        /// </summary>
        public string UiS { get; set; }

        // OUT OF STOCK
        public bool OoS_OrderAllowed { get; set; }
        public string OoS_OrderMinQty { get; set; }
        public string OoS_OrderETA { get; set; }

        // STYLE
        public string Type { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public string[] Images { get; set; }
        public string Dimensions { get; set; }
        public string Capacity { get; set; }

        // DETAILS
        public string Material { get; set; }
        public string CareGuide { get; set; }
        public string Guarantee { get; set; }
        public string DishwasherSafe { get; set; }
        public string MicrowaveSafe { get; set; }

        // WIDGETS
        public string[] WidgetTitles { get; set; }

        // CROSS-SELL
        public string[] CrossSellingProductCodes { get; set; }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return $"{Area}, {Category}, {ProductCode}, {StockCode}: {Name}";
        }
        #endregion
    }
}
