using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class PaymentsController : Controller
{
    private readonly IApiClient _api;
    public PaymentsController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Payments";
        ViewData["PageSubtitle"] = "Every payment ClearOrbit has received.";
        var response = await _api.GetAsync<ApiResult<List<PaymentListItem>>>("/api/Payments", Token);
        return View(response?.Data ?? new List<PaymentListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid invoiceId)
    {
        var invoiceResponse = await _api.GetAsync<ApiResult<InvoiceListItem>>($"/api/Invoices/{invoiceId}", Token);
        if (invoiceResponse is null || !invoiceResponse.Success || invoiceResponse.Data is null)
            return NotFound();

        var vm = new PaymentViewModel
        {
            InvoiceId = invoiceResponse.Data.Id,
            InvoiceCode = invoiceResponse.Data.Code,
            ClientName = invoiceResponse.Data.ClientName,
            BalanceDue = invoiceResponse.Data.BalanceDue,
            Currency = invoiceResponse.Data.Currency,
            Amount = invoiceResponse.Data.BalanceDue,
            PaymentDate = DateTime.UtcNow.Date
        };

        ViewData["PageTitle"] = "Record Payment";
        ViewData["PageSubtitle"] = $"For invoice {vm.InvoiceCode}";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["PageTitle"] = "Record Payment";
            ViewData["PageSubtitle"] = $"For invoice {model.InvoiceCode}";
            return View(model);
        }

        var payload = new
        {
            InvoiceId = model.InvoiceId,
            PaymentDate = model.PaymentDate,
            Amount = model.Amount,
            Method = model.Method,
            Reference = model.Reference,
            Notes = model.Notes
        };

        var response = await _api.PostAsync<ApiResult<PaymentListItem>>("/api/Payments", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to record payment.");
            ViewData["PageTitle"] = "Record Payment";
            ViewData["PageSubtitle"] = $"For invoice {model.InvoiceCode}";
            return View(model);
        }

        TempData["Success"] = $"Payment {response.Data?.Code} recorded.";
        return RedirectToAction("Details", "Invoices", new { id = model.InvoiceId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid invoiceId)
    {
        await _api.DeleteAsync<ApiResult<bool>>($"/api/Payments/{id}", Token);
        TempData["Success"] = "Payment reversed.";
        return RedirectToAction("Details", "Invoices", new { id = invoiceId });
    }
}