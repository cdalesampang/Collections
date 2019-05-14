using Collection.Business.Services;
using Collection.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Collection.Data.Models;

namespace Collection.Web.Controllers
{
    public class CollectionController : Controller
    {
        private readonly CollectionDbContext dbContext;
        private readonly ICollectionTypeService collectionTypeService;
        private readonly ICollectionService collectionService;
        private int userId;
        public CollectionController()
        {
            this.dbContext = new CollectionDbContext();
            this.collectionService = new CollectionService(this.dbContext);
            this.collectionTypeService = new CollectionTypeService(this.dbContext);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string collectionId, string colDate)
        {
            userId = int.Parse(User.Identity.GetUserId());
            await collectionService.Delete(int.Parse(collectionId));
            await dbContext.SaveChangesAsync();
            return await GetCollections(DateTime.Parse(colDate));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteType(int id)
        {
            await collectionTypeService.Delete(id);
            await dbContext.SaveChangesAsync();
            return  _CollectionTypes();
        }

        // GET: Collection
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetCollections(DateTime colDate)
        {
            userId = int.Parse(User.Identity.GetUserId());
            var model = await collectionService.GetAllByDateAsync(userId, colDate);
            return PartialView("_Collections",model);
        }

        [HttpPost]
        public async Task<ActionResult> AddType(string collectionType,bool selType)
        {
            userId = int.Parse(User.Identity.GetUserId());

            CollectionType type = new CollectionType();
            type.UserId = userId;
            type.Sales = selType;
            type.Type = collectionType;

            collectionTypeService.Create(type);
            dbContext.SaveChanges();
            var model = collectionTypeService.GetDefaultType();
            model.AddRange(await collectionTypeService.GetAllByUserIDAsync(userId));
            return PartialView("_CollectionTypes", model); ;
        }
        [HttpPost]
        public async Task<ActionResult> AddCollection(string collectionType,string amount, string colDate)
        {
            userId = int.Parse(User.Identity.GetUserId());

            Data.Models.Collection collection = new Data.Models.Collection();
            collection.UserId = userId;
            collection.CollectionType = await collectionTypeService.GetAllByTypeID(int.Parse(collectionType));
            collection.CollectionDate = DateTime.Parse(colDate);
            collection.Amount =Decimal.Parse(amount);

            collectionService.Create(collection);
            dbContext.SaveChanges();

            var model = await collectionService.GetAllByDateAsync(userId, collection.CollectionDate);
            return PartialView("_Collections", model); ;
        }

        public ActionResult _CollectionTypes()
        {
            userId = int.Parse(User.Identity.GetUserId());
            var model =  collectionTypeService.GetDefaultType();
            model.AddRange(collectionTypeService.GetAllByUserID(userId));
            //var model = collectionTypeService.GetAllByUserID(userId);
            return PartialView("_CollectionTypes", model); ;
        }

        [Authorize]
        public ActionResult Report()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetCollectionsReport(int year, int month,string reportType)
        {
            userId = int.Parse(User.Identity.GetUserId());
            List<Data.Models.Collection> collections = collectionService.GetByMonthAndYear(userId, year,month);
            switch (reportType)
            {
                case "sales":
                    collections = collections.Where(x => x.CollectionType.Sales == true).ToList();
                    break;
                case "expense":
                    collections = collections.Where(x => x.CollectionType.Sales == false).ToList();
                    break;
                case "revenue":
                    List<Data.Models.Collection> collectionSales = collections.Where(x => x.CollectionType.Sales == true).ToList();
                    List<Data.Models.Collection> collectionExpense = collections.Where(x => x.CollectionType.Sales == false).ToList();
                    collections = new List<Data.Models.Collection>();

                    int day = DateTime.DaysInMonth(year, month);
                    for (int i = 1; i <= day; i++)
                    {
                        decimal sales = collectionSales.Where(b => b.CollectionDate.Day == i).Sum(x=>x.Amount);
                        decimal expense = collectionExpense.Where(b => b.CollectionDate.Day == i).Sum(x => x.Amount);
                        //if (sales != null)
                        //{
                        //    salesAmount = sales.Amount;
                        //}
                        //if (expense != null) { 
                        //    expenseAmount = expense.Amount;
                        //}
                        decimal revenueAmount = sales - expense;
                        collections.Add(new Data.Models.Collection { Amount = sales, CollectionDate = new DateTime(collectionSales.FirstOrDefault().CollectionDate.Year, collectionSales.FirstOrDefault().CollectionDate.Month,i), CollectionType = new CollectionType { Type="Sales" } });
                        collections.Add(new Data.Models.Collection { Amount = expense, CollectionDate = new DateTime(collectionSales.FirstOrDefault().CollectionDate.Year, collectionSales.FirstOrDefault().CollectionDate.Month, i), CollectionType = new CollectionType { Type = "Expense" } });
                        collections.Add(new Data.Models.Collection { Amount = revenueAmount, CollectionDate = new DateTime(collectionSales.FirstOrDefault().CollectionDate.Year, collectionSales.FirstOrDefault().CollectionDate.Month, i), CollectionType = new CollectionType { Type = "Revenue" } });
                    }
                    break;
            }
            if (reportType.Equals("monthend"))
            {
                //var allCollectionsSales = collections.Where(x=>x.CollectionType.Sales == true).Select(x => new CollectionModel { Type = x.CollectionType.Type, Amount = x.Amount, Day = x.CollectionDate.Day }).OrderBy(x => x.Day).GroupBy(h => h.Type).ToList();
                var allCollectionsSales = collections.Where(x => x.CollectionType.Sales == true).GroupBy(x=>x.CollectionTypeId,(key,y)=>y.Select(x => new CollectionModel { Type = x.CollectionType.Type, Amount = y.Sum(z=>z.Amount), Day = x.CollectionDate.Day }).OrderBy(x => x.Day)).ToList();
                var allCollectionsExpense = collections.Where(x => x.CollectionType.Sales == false).GroupBy(x => x.CollectionTypeId, (key, y) => y.Select(x => new CollectionModel { Type = x.CollectionType.Type, Amount = y.Sum(z => z.Amount), Day = x.CollectionDate.Day }).OrderBy(x => x.Day)).ToList();
                Dictionary<string, List<CollectionModel>> collectionTotalByTypes = new Dictionary<string, List<CollectionModel>>();
                List<CollectionModel> collectionSales = new List<CollectionModel>();
                foreach (var sales in allCollectionsSales)
                {
                    collectionSales.Add(sales.First());
                }
                collectionTotalByTypes.Add("sales", collectionSales);
                List<CollectionModel> collectionExpense = new List<CollectionModel>();
                foreach (var expense in allCollectionsExpense)
                {
                    collectionExpense.Add(expense.First());
                }
                collectionTotalByTypes.Add("Expense", collectionExpense);
                return Json(collectionTotalByTypes.ToList());
            }
            else
            {
                var allCollectionsByType = collections.Select(x => new CollectionModel { Type = x.CollectionType.Type, Amount = x.Amount, Day = x.CollectionDate.Day }).OrderBy(x => x.Day).GroupBy(h => h.Type).ToList();
                Dictionary<string, List<decimal>> collectionTotalByTypes = new Dictionary<string, List<decimal>>();

                int days = DateTime.DaysInMonth(year, month);

                foreach (var item in allCollectionsByType)
                {
                    List<CollectionModel> items = item.ToList();
                    List<decimal> collectionPerDay = new List<decimal>();
                    for (int i = 1; i <= days; i++)
                    {
                        decimal coll = items.Where(b => b.Day == i).Sum(x => x.Amount);
                        collectionPerDay.Add(coll);
                    }
                    collectionTotalByTypes.Add(items[0].Type, collectionPerDay);
                }
                return Json(collectionTotalByTypes.ToList());
            }
        }
    }
    public class CollectionModel
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public int Day { get; set; }
        public bool IsSale { get; set; }
    }
}