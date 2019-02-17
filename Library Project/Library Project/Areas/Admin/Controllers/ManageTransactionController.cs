using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Library.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace Library.Areas.Admin.Controllers
{

    /// <summary>
    ///  نمایش تراکنش های مالی سمت ادمین
    /// </summary>


    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class ManageTransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ManageTransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region################################# Index  ##############################
        [HttpGet]
        public async Task<IActionResult> Index(int page=1)
        {
            var model = (from pt in _context.TransactionPayments
                         join u in _context.Users on pt.UserId equals u.Id
                         select new PaymentTransactionViewModel {

                             ID = pt.ID,
                             Amount = pt.Amount,
                             Description = pt.Description,
                             Email = pt.Email,
                             Mobile = pt.Mobile,
                             TransactionDate = pt.TransactionDate,
                             TransactionNo = pt.TransactionNo,
                             TransactionTime = pt.TransactionTime,
                             FullNameUser = u.FirstName + " " + u.LastName,
                             UserId = pt.UserId
                         }).AsNoTracking().OrderByDescending(r=>r.ID);

            PagingList<PaymentTransactionViewModel> modelPaging =await PagingList.CreateAsync(model,5,page);

            return View(modelPaging);
        }

        #endregion#####################################################################

        #region#################################  SearchInTransactionAsync ##############################
        
        public async Task<IActionResult> SearchInTransactionAsync(string fromDate,string toDate,string userFullName, int page = 1)
        {
            var model = (from pt in _context.TransactionPayments
                         join u in _context.Users on pt.UserId equals u.Id
                         select new PaymentTransactionViewModel
                         {

                             ID = pt.ID,
                             Amount = pt.Amount,
                             Description = pt.Description,
                             Email = pt.Email,
                             Mobile = pt.Mobile,
                             TransactionDate = pt.TransactionDate,
                             TransactionNo = pt.TransactionNo,
                             TransactionTime = pt.TransactionTime,
                             FullNameUser = u.FirstName + " " + u.LastName,
                             UserId = pt.UserId
                         }).AsNoTracking().OrderByDescending(r => r.ID);

            PagingList<PaymentTransactionViewModel> modelPaging = await PagingList.CreateAsync(model, 5, page);

            if (userFullName!=null)
            {
                userFullName = userFullName.TrimEnd().TrimStart();
                model = model.Where(m => m.FullNameUser.Contains(userFullName)).AsNoTracking().OrderByDescending(m => m.FullNameUser);
                modelPaging = await PagingList.CreateAsync(model, 5, page);
            }
            if (fromDate!=null)
            {
                model = model.Where(m => m.TransactionDate.CompareTo(fromDate) > 0).AsNoTracking().OrderByDescending(m => m.TransactionDate);
                modelPaging = await PagingList.CreateAsync(model, 5, page);
            }
            if (toDate != null)
            {
                model = model.Where(m => m.TransactionDate.CompareTo(toDate) < 0).AsNoTracking().OrderByDescending(m => m.TransactionDate);
                modelPaging = await PagingList.CreateAsync(model, 5, page);
            }

            return View("Index",modelPaging);
        }

        #endregion#####################################################################

    }
}