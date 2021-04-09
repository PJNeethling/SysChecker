using MerchandiseProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using SyntechAPI = Syntech.API;

namespace BeingIT.Syntech.API.Commerce
{
    public class SyntechMerchandiseProvider : IMerchandiseProvider
    {
        #region Private Fields
        private const string PREFIX = "SYNTECH";
        #endregion

        #region Public Properties
        string IMerchandiseProvider.Name => "Syntech Catalogue";
        #endregion

        #region Public Methods
        public MerchandiseItems GetItems(string contentDownloadPath)
        {
            var items = new MerchandiseItems()
            {
                ProviderName = ((IMerchandiseProvider)this).Name,
                DataSourceUri = "https://www.syntech.co.za/stockfeeds/syntech.xml"
            };

            var cachedFilePath = Path.Combine(Environment.CurrentDirectory, ".cache", $"{items.ProviderName} - {DateTime.Now:yyyy-MM-dd.HH}.xml");
            var stockfeedResponse = File.Exists(cachedFilePath)
                ? SyntechAPI.Stockfeed.DeserializeFeed(cachedFilePath)
                : SyntechAPI.Stockfeed.DownloadFeed(true, cachedFilePath);

            int totalItemsProcessed = 0;
            foreach (var product in stockfeedResponse.Stock.Products)
            {
                if (string.IsNullOrEmpty(product.SKU))
                    continue;

                if (string.IsNullOrEmpty(product.Name))
                    continue;

                string areaName = null, categoryName = null;
                if (product.Categories != null)
                {
                    var categories = product.Categories.Split(',');
                    areaName = categories[0]?.Trim();
                    if (categories.Length > 1)
                        categoryName = categories[1]?.Trim();
                }

                if (string.IsNullOrEmpty(areaName))
                    continue;

                if (string.IsNullOrEmpty(categoryName))
                    continue;

                var merchItem = new MerchandiseItem
                {
                    Area = areaName,
                    Category = categoryName,

                    ProductCode = product.SKU,
                    StockCode = product.SKU,

                    Description = System.Text.RegularExpressions.Regex.Replace(product.ShortDescription, @"<[^>]*>", string.Empty),
                    Details = product.Description,

                    Cost = product.Price,
                    Price = product.ResalePriceTaxIncluded,
                    Weight = product.Weight,

                    UiS = int.Parse(product.CapeTownStockCount) < 0 ? $"{0}" : product.CapeTownStockCount,
                };

                merchItem.Name = product.Name.Contains("|") ? product.Name.Substring(0, product.Name.IndexOf("|")).Trim() : product.Name.Trim();
                if (merchItem.Name.Length > 128)
                {
                    int indexOfFullStop = merchItem.Name.IndexOf(". ", 32);
                    if (indexOfFullStop >= 0)
                        merchItem.Name = merchItem.Name.Substring(0, indexOfFullStop - 1);

                    if (merchItem.Name.Length > 128)
                        merchItem.Name = $"{merchItem.Name.Substring(0, 125)}...";
                }

                #region Download images
                if (!string.IsNullOrEmpty(product.AllImages))
                {
                    var images = new List<string>();
                    foreach (string imageURL in product.AllImages.Split('|'))
                    {
                        if (string.IsNullOrWhiteSpace(imageURL))
                            continue;

                        string fileName = $"{PREFIX}-{Path.GetFileName(imageURL.Trim())}";
                        string imagePath = Path.Combine(contentDownloadPath, fileName);
                        if (!File.Exists(imagePath))
                            using (var client = new WebClient())
                                client.DownloadFile(new Uri(imageURL), imagePath);

                        images.Add(fileName);
                    }
                    if (!images.Any())
                        continue;

                    merchItem.Images = images.ToArray();
                }
                #endregion

                if (merchItem.Images == null || merchItem.Images.Length == 0)
                    continue;

                merchItem.Visible = true; // it's safe to mark as visible; we've already validated all required fields.

                items.Add(merchItem);

                totalItemsProcessed++;
            }

            return items;
        }

        public void Dispose()
        {

        }
        #endregion
    }
}
