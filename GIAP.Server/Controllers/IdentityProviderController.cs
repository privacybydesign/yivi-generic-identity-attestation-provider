using GIAP.Server.Models;
using GIAP.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace GIAP.Server.Controllers;

[ApiController]
public class IdentityProviderController(IIdentityProviderService identityProviderService) : ControllerBase
{
    [HttpGet("/api/identity-provider/{slug}")]
    public ActionResult<IdentityProvider?> Get(string slug)
    {
        var idp = identityProviderService.GetBySlug(slug);
        if (idp == null)
            return NotFound();

        return Ok(idp);
    }
}