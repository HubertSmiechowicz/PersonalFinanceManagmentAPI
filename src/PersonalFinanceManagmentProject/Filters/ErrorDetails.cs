using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace PersonalFinanceManagmentProject.Exceptions;

public class ErrorDetails : IActionResult
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    public ErrorDetails(int statusCode, string? message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        return this.ExecuteResultAsync(context);
    }
}