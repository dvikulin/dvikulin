using Taskify.Web.Models;

namespace Taskify.Web.Services;

public sealed class UserSession
{
    public UserDto? CurrentUser { get; private set; }

    public event Action? Changed;

    public void Select(UserDto user)
    {
        CurrentUser = user;
        Changed?.Invoke();
    }

    public void Clear()
    {
        CurrentUser = null;
        Changed?.Invoke();
    }
}
