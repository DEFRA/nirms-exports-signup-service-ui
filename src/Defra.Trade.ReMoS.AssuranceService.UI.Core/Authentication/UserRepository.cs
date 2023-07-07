namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.Authentication;

public interface IUserRepository
{
    bool ValidateLastChanged(string lastChanged);
}

public class UserRepository : IUserRepository
{
    public bool ValidateLastChanged(string lastChanged)
    {
        return false;
    }
}
