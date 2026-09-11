using System.Security.Claims;
using System.Text.Json;
using ClearOrbit.Web.Services;
using ClearOrbit.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ClearOrbit.Web.Controllers;

public class AccountController : Controller
{
    private readonly IApiClient _api;

    public AccountController(IApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var payload = new
        {
            Email = model.Email,
            Password = model.Password,
            DeviceInfo = Request.Headers.UserAgent.ToString()
        };

        var response = await _api.PostAsync<ApiResult<AuthResponseData>>(
            "/api/Auth/login", payload);

        if (response is null || !response.Success || response.Data is null)
        {
            var error = response?.Message ?? "Login failed. Please try again.";
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, response.Data.UserId.ToString()),
            new(ClaimTypes.Name, response.Data.FullName),
            new(ClaimTypes.Email, response.Data.Email),
            new("jwt_token", response.Data.Token),
            new("jwt_expires", response.Data.ExpiresAt.ToString("o"))
        };

        var identity = new ClaimsIdentity(claims, "ClearOrbitCookie");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("ClearOrbitCookie", principal, new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = response.Data.ExpiresAt
        });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("ClearOrbitCookie");
        return RedirectToAction("Index", "Home");
    }
}

public class AuthResponseData
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}