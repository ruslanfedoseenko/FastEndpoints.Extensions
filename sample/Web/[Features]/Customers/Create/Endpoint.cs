using System.Diagnostics;
using FastEndpoints.DiagnosticSources.Endpoint;
using Web.Services;

namespace Customers.Create;

public class Request
{
    public int CID { get; set; }
    
    public float Count { get; set; }

    // [From(Claim.UserName)]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// the name of the customer goes here
    /// </summary>
    public string? CustomerName { get; set; }

    public IEnumerable<string> PhoneNumbers { get; set; }

    // [HasPermission(Allow.Customers_Create)]
    public bool HasCreatePermission { get; set; }
    
    public SubRequest SubData { get; set; }
}

public class SubRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Endpoint : DiagnosticEndpoint<Request>
{
    private readonly IEmailService? _emailer;

    public Endpoint(IEmailService emailer)
    {
        _emailer = emailer;
    }

    public Endpoint()
    {
    }

    public override void Configure()
    {
        // Verbs(Http.POST, Http.GET);
        // Routes(
        //     "/customer/new/{RefererID}",
        //     "/customer/{cID}/new/{SourceID}",
        //     "/customer/save");
        
        AllowAnonymous();
        Verbs(Http.POST, Http.GET, Http.PUT, Http.DELETE);
        Routes(
            "/customer/{cID}/new",
            "/customer/{cID}/new/{customerName}"
            );
        DontAutoTag();
        Description(x => x.WithTags("Customer Save"));
    }

    public override Task HandleAsync(Request r, CancellationToken t)
    {
        Logger.LogInformation("customer creation has begun!");

        if (r.PhoneNumbers?.Count() < 2)
            ThrowError("Not enough phone numbers!");

        var msg = _emailer?.SendEmail() + " " + r.CreatedBy;

        return SendAsync(msg ?? "emailer not resolved!");
    }
}