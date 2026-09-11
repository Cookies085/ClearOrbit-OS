using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

[Authorize]
public class InvoicesController : Controller
{
    private readonly IApiClient _api;
    public InvoicesController(IApiClient api) => _api = api;

    private string? Token => User.FindFirst("jwt_token")?.Value;

    public async Task<IActionResult> Index()
    {
        ViewData["PageTitle"] = "Invoices";
        ViewData["PageSubtitle"] = "Every bill ClearOrbit has issued.";
        var response = await _api.GetAsync<ApiResult<List<InvoiceListItem>>>("/api/Invoices", Token);
        return View(response?.Data ?? new List<InvoiceListItem>());
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var response = await _api.GetAsync<ApiResult<InvoiceListItem>>($"/api/Invoices/{id}", Token);
        if (response is null || !response.Success || response.Data is null) return NotFound();

        var paymentsResponse = await _api.GetAsync<ApiResult<List<PaymentListItem>>>($"/api/Payments/invoice/{id}", Token);
        ViewData["Payments"] = paymentsResponse?.Data ?? new List<PaymentListItem>();

        ViewData["PageTitle"] = response.Data.Code;
        ViewData["PageSubtitle"] = response.Data.ClientName;
        return View(response.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["PageTitle"] = "New Invoice";
        var vm = new InvoiceViewModel();
        vm.Items.Add(new InvoiceItemViewModel());
        await LoadDropdowns(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InvoiceViewModel model)
    {
        model.Items = model.Items.Where(i => !string.IsNullOrWhiteSpace(i.Description)).ToList();

        if (!ModelState.IsValid || model.Items.Count == 0)
        {
            if (model.Items.Count == 0)
                ModelState.AddModelError(string.Empty, "At least one invoice item is required.");
            ViewData["PageTitle"] = "New Invoice";
            await LoadDropdowns(model);
            return View(model);
        }

        var payload = new
        {
            ClientId = model.ClientId,
            ProjectId = model.ProjectId,
            DivisionId = model.DivisionId,
            IssueDate = model.IssueDate,
            DueDate = model.DueDate,
            Currency = model.Currency,
            TaxRate = model.TaxRate,
            Notes = model.Notes,
            Items = model.Items.Select(i => new
            {
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        var response = await _api.PostAsync<ApiResult<InvoiceListItem>>("/api/Invoices", payload, Token);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError(string.Empty, response?.Message ?? "Failed to create invoice.");
            ViewData["PageTitle"] = "New Invoice";
            await LoadDropdowns(model);
            return View(model);
        }

        TempData["Success"] = $"Invoice {response.Data?.Code} created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(Guid id)
    {
        await _api.PostAsync<ApiResult<InvoiceListItem>>($"/api/Invoices/{id}/send", new { }, Token);
        TempData["Success"] = "Invoice marked as sent.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _api.PostAsync<ApiResult<InvoiceListItem>>($"/api/Invoices/{id}/cancel", new { }, Token);
        TempData["Success"] = "Invoice cancelled.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadDropdowns(InvoiceViewModel vm)
    {
        var clientsTask = _api.GetAsync<ApiResult<List<ClientListItem>>>("/api/Clients", Token);
        var projectsTask = _api.GetAsync<ApiResult<List<ProjectListItem>>>("/api/Projects", Token);
        var divisionsTask = _api.GetAsync<ApiResult<List<DivisionOption>>>("/api/Services/divisions", Token);

        await Task.WhenAll(clientsTask, projectsTask, divisionsTask);

        vm.Clients = (clientsTask.Result?.Data ?? new List<ClientListItem>())
            .Select(c => new ClientOption { Id = c.Id, Name = c.Name }).ToList();

        vm.Projects = (projectsTask.Result?.Data ?? new List<ProjectListItem>())
            .Select(p => new ProjectOption
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                DivisionName = p.DivisionName
            }).ToList();

        vm.Divisions = divisionsTask.Result?.Data ?? new List<DivisionOption>();
    }
}