using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IDirectiveService
    {
        Task<DirectivesOutline> GetDirectivesOutlineAsync(int factionId);
        Task<CharacterDirectivesOutline> GetCharacterDirectivesAsync(string characterId);
    }
}