using System;

namespace DemoFunc
{
    public class ProductDto
    {
        public string localBarcode { get; set; }
        public int RetailerId { get; set; }
        public decimal ItemTax { get; set; }
        public int itemDepartementId { get; set; }
        public int itemSupplierId { get; set; }
        public long itemInternalKey { get; set; }
        public bool isForExternalSale { get; set; }
        public string localName { get; set; }
        public decimal itemRegularPrice { get; set; }
        public DateTime? itemLastSaleDate { get; set; }
        public decimal? itemCasePrice { get; set; }
        public int? ItemsInCase { get; set; }
        public bool IsWeightable { get; set; }
        public int? QunatityLimit { get; set; }
        public string linkedItem { get; set; }
        public decimal itemTaxAmount { get; set; }
        public string itemGroupExternalId { get; set; }
        public string itemShortName { get; set; }
        public string itemFullName { get; set; }
        public decimal? packQuantity { get; set; }
        public decimal? packPrice { get; set; }
        public decimal? LocalWeight { get; set; }
        public decimal? AgeRestriction { get; set; }
        public decimal MarkupPercentage { get; set; }
        public decimal? PosSoldQty { get; set; }
        public int? QuantiityInStock { get; set; }
        public string barcodeToCheck { get; set; }
        public bool IsPrivateProduct { get; set; }
        public bool IsEmbeddedPriceBarcode { get; set; }
        public string NewproductFormattedBarCode { get; set; }
        public int BranchID { get; set; }
        public bool IsLegitBarcode { get; set; }
        public string ImageUrl { get; set; }
        public int? SpFamilyId { get; set; }
        public int? SpBrandId { get; set; }
        public int? SPUnitOfMeasure_id { get; set; }
        public string LongDescription { get; set; }
        public string Directions { get; set; }
        public string Warnings { get; set; }
        public string AttributesList { get; set; }
        public bool? IsSalePriceIncludedOnSticker { get; set; }
        public int? NumberOfItems { get; set; }
        public bool IsEbtEligible { get; set; }
        public bool IsEbtCashEligible { get; set; }
        public int? EntitySource { get; set; }
        public string ExtarnalFamilyId { get; set; }
        public int? RetailerBranchAisle_id { get; set; }
        public string AisleSide { get; set; }
        public int? Shelf { get; set; }
        public int? AisleSection { get; set; }
        public int HashCode { get; set; }
    }
}
