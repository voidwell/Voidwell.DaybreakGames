using System.Threading.Tasks;
using Moq;
using Moq.Language.Flow;

namespace Voidwell.DaybreakGames.Test
{
    public static class Extensions
    {
        public static Mock<T> AsMock<T>(this T obj)
            where T : class
        {
            return Mock.Get(obj);
        }

        public static IReturnsResult<TMock> ReturnsNoActionTask<TMock>(this ISetup<TMock, Task> setup)
            where TMock : class
        {
            return setup.Returns(Task.FromResult(0));
        }
    }
}
