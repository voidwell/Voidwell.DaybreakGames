using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface ICharacterDirectiveService
    {
        Task<CharacterDirectivesOutline> GetCharacterDirectivesAsync(string characterId);
    }
}