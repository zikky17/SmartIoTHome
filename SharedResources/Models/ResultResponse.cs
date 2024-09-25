namespace SharedResources.Models;

public class ResultResponse
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public object? Content { get; set; }
}

public class ResultResponse<T> : ResultResponse
{
    public T? Content { get; set; }
}